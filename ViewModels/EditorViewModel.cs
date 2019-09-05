using Autofac;
using Cross.Aggregator;
using Cross.Data;
using Cross.Services;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Cross.ViewModels
{
    public class EditorViewModel : INotifyPropertyChanged
    {
        private FormData data;
        public FormData Data {
            get => data;
            set
            {
                data = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Data"));
            }
        }
        private SizePackage actualSizes;
        public SizePackage ActualSizes {
            get => actualSizes;
            set
            {
                actualSizes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ActualSizes"));
            }
        }
        public bool PreserveSquareRatio { get; set; } = true;
        public bool PreserveSetRatio { get; set; } = true;
        public bool PreserveArrowRatio { get; set; } = true;
        public ThumbnailViewModel Squares { get; set; }
        public ThumbnailViewModel Sets { get; set; }
        public ThumbnailViewModel Arrows { get; set; }

        private IEventAggregator eventAggregator;

        public event PropertyChangedEventHandler PropertyChanged;

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

            Squares.PropertyChanged += SelectedSquareUpdated;
            Sets.PropertyChanged += SelectedSetUpdated;
            Arrows.PropertyChanged += SelectedArrowUpdated;

            eventAggregator.GetEvent<SizeChanges>().Subscribe(HandleSizeChanges);
            eventAggregator.GetEvent<LoadFromTemplate>().Subscribe(HandleTemplateChanges);
        }

        private void HandleTemplateChanges(TemplateFormData obj)
        {
            Data = (FormData)obj.Data.Clone();
            Data.PropertyChanged += Data_Updated;
            eventAggregator.GetEvent<FormDataChanges>().Publish((FormData)Data.Clone());
        }

        private void HandleSizeChanges(SizePackage data)
        {
            ActualSizes = (SizePackage)data.Clone();
        }

        private void SelectedArrowUpdated(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Data.ArrowIndex = Arrows.LastSelected;
            eventAggregator.GetEvent<FormDataChanges>().Publish((FormData)Data.Clone());
        }

        private void SelectedSetUpdated(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Data.SetIndex = Sets.LastSelected;
            eventAggregator.GetEvent<FormDataChanges>().Publish((FormData)Data.Clone());
        }

        private void SelectedSquareUpdated(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Data.SquareIndex = Squares.LastSelected;
            eventAggregator.GetEvent<FormDataChanges>().Publish((FormData)Data.Clone());
        }

        private void Data_Updated(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PreserveSquareRatio) Data.SquareHeight = Data.SquareWidth;
            if (PreserveSetRatio) Data.SetHeight = Data.SetWidth;
            if (PreserveArrowRatio) Data.ArrowHeight = Data.ArrowWidth;
            eventAggregator.GetEvent<FormDataChanges>().Publish((FormData)Data.Clone());
        }
    }
}
