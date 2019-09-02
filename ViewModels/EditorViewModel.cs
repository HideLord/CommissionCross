using Autofac;
using Cross.Aggregator;
using Cross.Data;
using Cross.Services;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Cross.ViewModels
{
    public class EditorViewModel
    {
        public FormData Data { get; set; }
        public bool PreserveSquareRatio { get; set; } = true;
        public bool PreserveSetRatio { get; set; } = true;
        public bool PreserveArrowRatio { get; set; } = true;
        public ThumbnailViewModel Squares { get; set; }
        public ThumbnailViewModel Sets { get; set; }
        public ThumbnailViewModel Arrows { get; set; }

        private IEventAggregator eventAggregator;

        public EditorViewModel(IEventAggregator eventAggregator)
        {
            Data = new FormData();
            Data.PropertyChanged += Data_Updated;
            this.eventAggregator = eventAggregator;

            Squares = Bootstrapper.Container.Resolve<ThumbnailViewModel>();
            Sets = Bootstrapper.Container.Resolve<ThumbnailViewModel>();
            Arrows = Bootstrapper.Container.Resolve<ThumbnailViewModel>();
          
            Squares.init("squares.txt", 50);
            Sets.init("sets.txt", 50);
            Arrows.init("arrows.txt", 50);
        }

        private void Data_Updated(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PreserveSquareRatio) Data.SquareHeight = Data.SquareWidth;
            if (PreserveSetRatio) Data.SetHeight = Data.SetWidth;
            if (PreserveArrowRatio) Data.ArrowHeight = Data.ArrowWidth;
            eventAggregator.GetEvent<FormDataChanges>().Publish(Data);
        }
    }
}
