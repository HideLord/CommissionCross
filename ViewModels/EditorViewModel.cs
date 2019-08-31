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
        public FormData Data;

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
