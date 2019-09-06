#define ACTUAL_SIZE
using Cross.Aggregator;
using Cross.Data;
using Cross.Services.Contracts;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Cross.ViewModels
{
    public class CrossViewModel : INotifyPropertyChanged
    {
        private readonly IFigureManip figureManip;
        private readonly ILoadFiguresService loader;
        private readonly IEventAggregator eventAggregator;

        private List<Figure> AllSquares;
        private List<Figure> AllSets;
        private List<Figure> AllArrows;

        private FormData prevData, data;

        private int padding;
        private double arrowDefaultCoef = 0.23;

        public event PropertyChangedEventHandler PropertyChanged;

        public FormData Data {
            get => data;
            set
            {
                data = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Data"));
            }
        }
        public int FullSize { get; set; }
        public int DefaultSide { get; set; } = 14;
        public ObservableCollection<Figure> Squares { get; set; }
        public ObservableCollection<Figure> Sets { get; set; }
        public ObservableCollection<Figure> Arrows { get; set; }

        public CrossViewModel(IFigureManip figureManip, ILoadFiguresService loader, IEventAggregator eventAggregator)
        {
            prevData = new FormData();
            data = new FormData();

            this.figureManip = figureManip;
            this.loader = loader;
            this.eventAggregator = eventAggregator;

            this.padding = DefaultSide / 3;

            this.FullSize = 3 * DefaultSide + 2 * padding;

            Squares = new ObservableCollection<Figure>();
            Sets = new ObservableCollection<Figure>();
            Arrows = new ObservableCollection<Figure>();

            AllSquares = loader.LoadFigures("squares.txt");
            AllSets = loader.LoadFigures("sets.txt");
            AllArrows = loader.LoadFigures("arrows.txt");

            AllSquares = figureManip.Normalize(AllSquares, DefaultSide);
            AllSets = figureManip.Normalize(AllSets, DefaultSide);
            AllArrows = figureManip.Normalize(AllArrows, DefaultSide*arrowDefaultCoef);

            prevData.SquareIndex = prevData.SetIndex = prevData.ArrowIndex = -1;

            updateAll();

            eventAggregator.GetEvent<FormDataChanges>().Subscribe(handleFormDataChanges);
        }

        private void repositionArrows()
        {
            double delta = 0.05;

            if (Arrows.Count < 4) throw new System.IndexOutOfRangeException();
#if ACTUAL_SIZE

            double sqHeight = figureManip.Height(Squares[0]);
            double sqWidth = figureManip.Width(Squares[0]);

            Arrows[0] = figureManip.Rotate(Arrows[0], figureManip.FindCenterOfBound(Arrows[0]), 90);
            Arrows[0] = figureManip.NormalizeTranslate(Arrows[0]);
            Arrows[0] = figureManip.Translate(Arrows[0], padding + (0.5 + delta) * DefaultSide + sqWidth / 2, padding + sqHeight/6);

            Arrows[1] = figureManip.Rotate(Arrows[1], figureManip.FindCenterOfBound(Arrows[1]), 90);
            Arrows[1] = figureManip.NormalizeTranslate(Arrows[1]);
            Arrows[1] = figureManip.Translate(Arrows[1], padding + 2.5 * DefaultSide - figureManip.Width(Arrows[1]) / 2, padding + (0.5 + delta) * DefaultSide + sqHeight/2);

            Arrows[2] = figureManip.NormalizeTranslate(Arrows[2]);
            Arrows[2] = figureManip.Translate(Arrows[2], padding + sqWidth/6, padding + (0.5 + delta) * DefaultSide + sqHeight/2);

            Arrows[3] = figureManip.NormalizeTranslate(Arrows[3]);
            Arrows[3] = figureManip.Translate(Arrows[3], padding + (0.5+ delta) * DefaultSide + sqWidth/2, padding + (2) * DefaultSide + sqHeight/6);
#else
            Arrows[0] = figureManip.Rotate(Arrows[0], figureManip.FindCenterOfBound(Arrows[0]), 90);
            Arrows[0] = figureManip.NormalizeTranslate(Arrows[0]);
            Arrows[0] = figureManip.Translate(Arrows[0], padding + (0.5 + data.SquareWidth / 200.0 + delta) * DefaultSide, padding + (data.SquareHeight / 600.0) * DefaultSide);

            Arrows[1] = figureManip.Rotate(Arrows[1], figureManip.FindCenterOfBound(Arrows[1]), 90);
            Arrows[1] = figureManip.NormalizeTranslate(Arrows[1]);
            Arrows[1] = figureManip.Translate(Arrows[1], padding + 2.5 * DefaultSide - figureManip.Width(Arrows[1]) / 2, padding + (0.5 + data.SquareHeight / 200.0 + delta) * DefaultSide);

            Arrows[2] = figureManip.NormalizeTranslate(Arrows[2]);
            Arrows[2] = figureManip.Translate(Arrows[2], padding + (data.SquareWidth / 600.0) * DefaultSide, padding + (0.5 + data.SquareHeight / 200.0 + delta) * DefaultSide);

            Arrows[3] = figureManip.NormalizeTranslate(Arrows[3]);
            Arrows[3] = figureManip.Translate(Arrows[3], padding + (0.5 + data.SquareWidth / 200.0 + delta) * DefaultSide, padding + (2 + data.SquareHeight / 600.0) * DefaultSide);
#endif
        }

        private void updateRotation()
        {
            for(var i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Squares[i * 3 + j] = figureManip.Rotate(
                            Squares[i * 3 + j],
                            figureManip.FindCenterOfBound(Squares[i * 3 + j]),
                            Data.SquareRotation );
                    
                    Sets[i * 3 + j] = figureManip.Rotate(
                            Sets[i * 3 + j],
                            figureManip.FindCenterOfBound(Sets[i * 3 + j]),
                            Data.SetRotation );
                    
                }
            }
        }

        private void updateSize()
        {
            for (var i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    
                    Squares[i * 3 + j] = figureManip.UniformScale(Squares[i * 3 + j],
                            (double)(Data.SquareWidth) / 100.0,
                            (double)(Data.SquareHeight) / 100.0);
                    
                    
                    Sets[i * 3 + j] = figureManip.UniformScale(Sets[i * 3 + j],
                            (double)(Data.SetWidth) / 100.0,
                            (double)(Data.SetHeight) / 100.0);
                    
                    
                }
            }
            for (int i = 0; i < 4; i++)
            {
                Arrows[i] = figureManip.UniformScale(Arrows[i], (double)(Data.ArrowWidth) / 100.0, (double)(Data.ArrowHeight) / 100.0);
            }
        }

        private void updateType()
        {
            Squares.Clear();
            for (int i = 0; i < 9; i++)
            {
                Squares.Add((Figure)AllSquares[Data.SquareIndex].Clone());

                if (i == 0 || i == 2 || i == 6)
                {
                    int row = i / 3;
                    int col = i % 3;

                    Squares[i] = figureManip.Translate(Squares.Last(), row * DefaultSide + padding, col * DefaultSide + padding);
                }
                else
                {
                    Squares[i].Points.Clear();
                }
            }
            
            Sets.Clear();
            for (int i = 0; i < 9; i++)
            {
                Sets.Add((Figure)AllSets[Data.SetIndex].Clone());

                int row = i / 3;
                int col = i % 3;

                Sets[i] = figureManip.Translate(Sets.Last(), row * DefaultSide + padding, col * DefaultSide + padding);
            }
            
            Arrows.Clear();
            for (int i = 0; i < 4; i++)
                Arrows.Add((Figure)AllArrows[Data.ArrowIndex].Clone());
        }

        private void updateAll()
        {
            updateType();
            updateSize();
            updateRotation();
            repositionArrows();
            SendSizeInfo();
            SendSquareFigure();
        }

        private void SendSquareFigure()
        {
            eventAggregator.GetEvent<SquareFigureChanges>().Publish(Squares[0]);
        }

        private void SendSizeInfo()
        {
            SizePackage sizePackage = new SizePackage();
            sizePackage.SquareHeight = figureManip.Height(Squares[0]);
            sizePackage.SquareWidth = figureManip.Width(Squares[0]);

            sizePackage.SetHeight = figureManip.Height(Sets[0]);
            sizePackage.SetWidth = figureManip.Width(Sets[0]);

            sizePackage.ArrowHeight = figureManip.Height(Arrows[0]);
            sizePackage.ArrowWidth = figureManip.Width(Arrows[0]);
            eventAggregator.GetEvent<SizeChanges>().Publish(sizePackage);
        }

        private void handleFormDataChanges(FormData data)
        {
            this.Data = (FormData)data.Clone();
            updateAll();
            prevData = (FormData)data.Clone();
        }
    }
}
