using Cross.Data;
using Cross.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Cross.ViewModels
{
    public class CustomFigure : INotifyPropertyChanged
    {
        public CustomFigure(Figure figure)
        {
            FigureObj = figure;
        }
        Figure figure;
        public Figure FigureObj
        {
            get => figure;
            set
            {
                figure = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FigureObj"));
            }
        }
        bool isSelected = false;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ThumbnailViewModel
    {
        private ILoadFiguresService loader;
        private IFigureManip manipulator;

        private int lastSelected = -1;

        public List<CustomFigure> Figures { get; set; }

        public ThumbnailViewModel(ILoadFiguresService loader, IFigureManip manipulator)
        {
            this.Figures = new List<CustomFigure>();
            this.loader = loader;
            this.manipulator = manipulator;
        }

        public void init(string fileName, double size)
        {
            var Figures = loader.LoadFigures(fileName);
            Figures = manipulator.Normalize(Figures, size);
            for (int i = 0; i < Figures.Count; i++) {
                this.Figures.Add(new CustomFigure(Figures[i]));
                this.Figures.Last().PropertyChanged += FigureUpdated;
            }
            this.Figures[0].IsSelected = true;
        }

        private void FigureUpdated(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (((CustomFigure)sender).IsSelected)
            {
                if(lastSelected>=0)Figures[lastSelected].IsSelected = false;
                lastSelected = 0;
                for (int i = 0; i < Figures.Count; i++)
                    if (Figures[i].IsSelected) lastSelected = i;
            }
        }
    }
}
