using Cross.Aggregator;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_Cross.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly IEventAggregator eventAggregator;

        private Visibility editorVisibility = Visibility.Visible;
        public Visibility EditorVisibility
        {
            get => editorVisibility;
            set
            {
                editorVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EditorVisibility"));
            }
        }

        public MainPageViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            eventAggregator.GetEvent<ProcessStartRequest>().Subscribe(handleProcessStart);
        }

        private void handleProcessStart(object obj)
        {
            EditorVisibility = Visibility.Hidden;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
