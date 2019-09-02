using Cross.Aggregator;
using Cross.Data;
using Cross.Services.Contracts;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Cross.ViewModels
{
    public class CrossViewModel : BaseNotify
    {
        private readonly IFigureManip figureManip;
        private readonly ILoadFiguresService loader;
        private readonly IEventAggregator eventAggregator;

        private List<Figure> AllSquares;
        private List<Figure> AllSets;
        private List<Figure> AllArrows;

        private FormData prevData;
        private FormData data;

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

            Squares = new ObservableCollection<Figure>();
            Sets = new ObservableCollection<Figure>();
            Arrows = new ObservableCollection<Figure>();

            AllSquares = loader.LoadFigures("squares.txt");
            AllSets = loader.LoadFigures("sets.txt");
            AllArrows = loader.LoadFigures("arrows.txt");

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
                    if (prevData.SquareRotation != data.SquareRotation || prevData.SquareIndex != data.SquareIndex)
                    {
                        Squares[i * 3 + j] = figureManip.Rotate(
                            Squares[i * 3 + j],
                            figureManip.FindCenterOfBound(Squares[i * 3 + j]),
                            data.SquareRotation);
                    }
                    if (prevData.SetRotation != data.SetRotation || prevData.SetIndex != data.SetIndex)
                    {
                        Sets[i * 3 + j] = figureManip.Rotate(
                            Sets[i * 3 + j],
                            figureManip.FindCenterOfBound(Sets[i * 3 + j]),
                            data.SetRotation);
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
                    if (prevData.SquareHeight != data.SquareHeight || prevData.SquareWidth != data.SquareWidth || prevData.SquareIndex != data.SquareIndex) 
                        Squares[i * 3 + j] = figureManip.UniformScale(Squares[i * 3 + j], (double)data.SquareWidth / 100.0, (double)data.SquareHeight / 100.0);
                    if (prevData.SetHeight != data.SetHeight || prevData.SetWidth != data.SetWidth || prevData.SetIndex != data.SetIndex)
                        Sets[i * 3 + j] = figureManip.UniformScale(Sets[i * 3 + j], (double)data.SetWidth / 100.0, (double)data.SetHeight / 100.0);
                    
                }
            }
            if (prevData.ArrowHeight != data.ArrowHeight || prevData.ArrowWidth != data.ArrowWidth || prevData.ArrowIndex != data.ArrowIndex)
                for (int i = 0; i < 4; i++)Arrows[i] = figureManip.UniformScale(Arrows[i], (double)data.ArrowWidth / 100.0, (double)data.ArrowHeight / 100.0);
        }

        private void updateType()
        {
            if (prevData.SquareIndex != data.SquareIndex)
            {
                Squares.Clear();
                for (int i = 0; i < 9; i++)
                {
                    Squares.Add((Figure)AllSquares[prevData.SquareIndex].Clone());
                }
            }
            if (prevData.SetIndex != data.SetIndex)
            {
                Sets.Clear();
                for (int i = 0; i < 9; i++)
                {
                    Sets.Add((Figure)AllSets[prevData.SetIndex].Clone());
                }
            }
            if (prevData.ArrowIndex != data.ArrowIndex)
            {
                Arrows.Clear();
                for (int i = 0; i < 4; i++)
                {
                    Arrows.Add((Figure)AllArrows[prevData.ArrowIndex].Clone());
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
            this.data = data;
            updateAll();
        }
    }
}
