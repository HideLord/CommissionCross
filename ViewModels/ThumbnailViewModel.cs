using Cross.Data;
using Cross.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Cross.ViewModels
{
    public class ThumbnailViewModel
    {
        private ILoadFiguresService loader;
        private IFigureManip manipulator;

        public List<Figure> Figures { get; set; }

        public ThumbnailViewModel(ILoadFiguresService loader, IFigureManip manipulator)
        {
            this.loader = loader;
            this.manipulator = manipulator;
        }

        public void init(string fileName, double size)
        {
            Figures = loader.LoadFigures(fileName);
            Figures = manipulator.Normalize(Figures, size);
        }
    }
}
