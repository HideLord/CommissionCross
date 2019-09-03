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

        private int padding = 15;

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
        public int DefaultSide { get; set; } = 50;
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

            this.FullSize = 3 * DefaultSide + 2 * padding;

            Squares = new ObservableCollection<Figure>();
            Sets = new ObservableCollection<Figure>();
            Arrows = new ObservableCollection<Figure>();

            AllSquares = loader.LoadFigures("squares.txt");
            AllSets = loader.LoadFigures("sets.txt");
            AllArrows = loader.LoadFigures("arrows.txt");

            AllSquares = figureManip.Normalize(AllSquares, DefaultSide);
            AllSets = figureManip.Normalize(AllSets, DefaultSide);
            AllArrows = figureManip.Normalize(AllArrows, DefaultSide);

            prevData.SquareIndex = prevData.SetIndex = prevData.ArrowIndex = -1;

            updateAll();

            eventAggregator.GetEvent<FormDataChanges>().Subscribe(handleFormDataChanges);
        }

        private void updateRotation()
        {
            for(var i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (prevData.SquareRotation != Data.SquareRotation || prevData.SquareIndex != Data.SquareIndex)
                    {
                        Squares[i * 3 + j] = figureManip.Rotate(
                            Squares[i * 3 + j],
                            figureManip.FindCenterOfBound(Squares[i * 3 + j]),
                            Data.SquareRotation);
                    }
                    if (prevData.SetRotation != Data.SetRotation || prevData.SetIndex != Data.SetIndex)
                    {
                        Sets[i * 3 + j] = figureManip.Rotate(
                            Sets[i * 3 + j],
                            figureManip.FindCenterOfBound(Sets[i * 3 + j]),
                            Data.SetRotation);
                    }
                }
            }
        }

        private void updateSize()
        {
            for (var i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (prevData.SquareHeight != Data.SquareHeight || prevData.SquareWidth != Data.SquareWidth || prevData.SquareIndex != Data.SquareIndex) 
                        Squares[i * 3 + j] = figureManip.UniformScale(Squares[i * 3 + j], (double)Data.SquareWidth / 100.0, (double)Data.SquareHeight / 100.0);
                    if (prevData.SetHeight != Data.SetHeight || prevData.SetWidth != Data.SetWidth || prevData.SetIndex != Data.SetIndex)
                        Sets[i * 3 + j] = figureManip.UniformScale(Sets[i * 3 + j], (double)Data.SetWidth / 100.0, (double)Data.SetHeight / 100.0);
                    
                }
            }
            if (prevData.ArrowHeight != Data.ArrowHeight || prevData.ArrowWidth != Data.ArrowWidth || prevData.ArrowIndex != Data.ArrowIndex)
                for (int i = 0; i < 4; i++)Arrows[i] = figureManip.UniformScale(Arrows[i], (double)Data.ArrowWidth / 100.0, (double)Data.ArrowHeight / 100.0);
        }

        private void updateType()
        {
            if (prevData.SquareIndex != Data.SquareIndex)
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
            }
            if (prevData.SetIndex != Data.SetIndex)
            {
                Sets.Clear();
                for (int i = 0; i < 9; i++)
                {
                    Sets.Add((Figure)AllSets[Data.SetIndex].Clone());

                    int row = i / 3;
                    int col = i % 3;

                    Sets[i] = figureManip.Translate(Sets.Last(), row * DefaultSide + padding, col * DefaultSide + padding);
                }
            }
            if (prevData.ArrowIndex != Data.ArrowIndex)
            {
                Arrows.Clear();
                for (int i = 0; i < 4; i++)
                {
                    Arrows.Add((Figure)AllArrows[Data.ArrowIndex].Clone());
                }
            }
        }

        private void updateAll()
        {
            updateType();
            updateSize();
            updateRotation();
        }

        private void handleFormDataChanges(FormData data)
        {
            this.Data = data;
            updateAll();
        }
    }
}
