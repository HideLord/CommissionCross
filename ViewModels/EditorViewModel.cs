using Cross.Aggregator;
using Cross.Data;
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

        private IEventAggregator eventAggregator;

        public EditorViewModel(IEventAggregator eventAggregator)
        {
            Data = new FormData();
            Data.PropertyChanged += Data_Updated;
            this.eventAggregator = eventAggregator;
        }

        private void Data_Updated(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            eventAggregator.GetEvent<FormDataChanges>().Publish(Data);
        }
    }
}
