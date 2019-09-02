using Cross.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Cross.ViewModels
{
    public class ThumbnailFigureViewModel : INotifyPropertyChanged
    {
        public ThumbnailFigureViewModel(Figure figure)
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
        int thumbnailSize = 80;
        public int ThumbnailSize
        {
            get => thumbnailSize;
            set
            {
                thumbnailSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ThumbnailSize"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
