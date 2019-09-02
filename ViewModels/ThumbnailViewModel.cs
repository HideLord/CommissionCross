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
    public class ThumbnailViewModel : INotifyPropertyChanged
    {
        private ILoadFiguresService loader;
        private IFigureManip manipulator;

        private int lastSelected = -1;

        public int LastSelected
        {
            get => lastSelected;
            set
            {
                lastSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LastSelected"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<ThumbnailFigureViewModel> Figures { get; set; }

        public ThumbnailViewModel(ILoadFiguresService loader, IFigureManip manipulator)
        {
            this.Figures = new List<ThumbnailFigureViewModel>();
            this.loader = loader;
            this.manipulator = manipulator;
        }

        public void init(string fileName, double size)
        {
            var Figures = loader.LoadFigures(fileName);
            Figures = manipulator.Normalize(Figures, size);
            for (int i = 0; i < Figures.Count; i++) {
                this.Figures.Add(new ThumbnailFigureViewModel(Figures[i]));
                this.Figures.Last().PropertyChanged += FigureUpdated;
            }
            this.Figures[0].IsSelected = true;
        }

        private void FigureUpdated(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (((ThumbnailFigureViewModel)sender).IsSelected)
            {
                if(LastSelected >= 0)Figures[LastSelected].IsSelected = false;
                for (int i = 0; i < Figures.Count; i++)
                    if (Figures[i].IsSelected) LastSelected = i;
            }
        }
    }
}
