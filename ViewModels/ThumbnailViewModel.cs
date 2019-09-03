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

        private int lastSelected = 0;

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

        public ObservableCollection<ThumbnailFigureViewModel> Figures { get; set; }

        public ThumbnailViewModel(ILoadFiguresService loader, IFigureManip manipulator)
        {
            this.Figures = new ObservableCollection<ThumbnailFigureViewModel>();
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
            int currSelected = -1;
            if ((sender as ThumbnailFigureViewModel).IsSelected)
            {
                for (int i = 0; i < Figures.Count; i++)
                    if (Figures[i] == sender) currSelected = i;
                if (LastSelected != currSelected)
                {
                    Figures[LastSelected].IsSelected = false;
                    LastSelected = currSelected;
                }
            }
            else
            {
                for (int i = 0; i < Figures.Count; i++)
                    if (Figures[i].IsSelected) currSelected = i;
                if (sender == Figures[LastSelected] && currSelected == -1)
                {
                    Figures[LastSelected].IsSelected = true;
                }
            }
        }
    }
}
