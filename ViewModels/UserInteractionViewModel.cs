using Cross.Aggregator;
using Cross.Data;
using Cross.Services.Contracts;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Cross.ViewModels
{
    public class UserInteractionViewModel : INotifyPropertyChanged
    {
        private readonly IEventAggregator eventAggregator;
        private readonly ILoadFiguresService loader;
        private IProcessService processService;


        public event PropertyChangedEventHandler PropertyChanged;


        private string processOutput;
        public string ProcessOutput
        {
            get => processOutput;
            set
            {
                processOutput = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProcessOutput"));
            }
        }

        private Figure square;
        public Figure Square
        {
            get => square;
            set
            {
                square = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Square"));
            }
        }



        public ObservableCollection<OptionInFigureViewModel> Options {get;set;}

        public UserInteractionViewModel(IEventAggregator eventAggregator, ILoadFiguresService loader)
        {
            this.eventAggregator = eventAggregator;
            this.loader = loader;

            this.eventAggregator.GetEvent<ProcessStartRequest>().Subscribe(handleProcessStart);
            this.eventAggregator.GetEvent<SquareFigureChanges>().Subscribe(handleSquareChanges);

            Square = new Figure();
            Options = new ObservableCollection<OptionInFigureViewModel>();

            for (int i = 0; i < 3; i++)
            {
                Options.Add(new OptionInFigureViewModel());
                Options.Last().Square = Square;
            }
        }

        private void handleSquareChanges(Figure obj)
        {
            Square = (Figure)obj.Clone();
            for(int i = 0; i < Options.Count; i++)
            {
                Options[i].Square = Square;
            }
        }

        private void handleProcessStart(object obj)
        {
            processService = (IProcessService)obj;
            processService.SetHandler(outputHandler);
            processService.SpawnProcess("SVG_forger.exe");
        }

        private void outputHandler(object sender, DataReceivedEventArgs e)
        {
            ProcessOutput += e.Data + "\n";
        }
    }
}
