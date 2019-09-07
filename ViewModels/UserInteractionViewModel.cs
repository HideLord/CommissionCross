using Cross.Aggregator;
using Cross.Data;
using Cross.Services.Contracts;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WPF_Cross.Functional;

namespace WPF_Cross.ViewModels
{
    public class UserInteractionViewModel : INotifyPropertyChanged
    {
        private readonly IEventAggregator eventAggregator;
        private readonly ILoadFiguresService loader;
        private IProcessService processService;

        private ICommand clickedCommand;
        public ICommand ClickedCommand
        {
            get
            {
                if (clickedCommand == null) clickedCommand = new RelayCommand<int>(handleOptionClick);
                return clickedCommand;
            }
        }

        private void handleOptionClick(int obj)
        {
            if (waitingForInput)
            {
                waitingForInput = false;
                writer.WriteLine(obj);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private bool startListening = false, addOption = false, waitingForInput = false;

        private bool isExpanded = false;
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                isExpanded = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsExpanded"));
            }
        }

        private string actualWords = "";
        public string ActualWords
        {
            get => actualWords;
            set
            {
                actualWords = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ActualWords"));
            }
        }

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
        private StreamWriter writer;

        public Figure Square
        {
            get => square;
            set
            {
                square = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Square"));
            }
        }


        private List<OptionInFigureViewModel> options;
        public List<OptionInFigureViewModel> Options
        {
            get => options;
            set
            {
                options = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Options"));
            }
        }

        public UserInteractionViewModel(IEventAggregator eventAggregator, ILoadFiguresService loader)
        {
            this.eventAggregator = eventAggregator;
            this.loader = loader;

            this.eventAggregator.GetEvent<ProcessStartRequest>().Subscribe(handleProcessStart);
            this.eventAggregator.GetEvent<SquareFigureChanges>().Subscribe(handleSquareChanges);
            
            Square = new Figure();
            Options = new List<OptionInFigureViewModel>();

            for (int i = 0; i < 3; i++)
            {
                Options.Add(new OptionInFigureViewModel());
                Options.Last().Square = Square;
                Options.Last().OptionOne += "What is even happening\nAnymore my dude";
                Options.Last().Index = Options.Count;
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
            writer = processService.SpawnProcess("SVG_forger.exe");
        }

        private void outputHandler(object sender, DataReceivedEventArgs e)
        {
            if (e.Data.IndexOf("quiting") > -1)
            {
                processService.Kill();
                Environment.Exit(0);
            }
            if (e.Data.IndexOf("waitforme") > -1)
            {
                Options = new List<OptionInFigureViewModel>();
                ActualWords = "";
            }

            string bla = e.Data.Replace('|', '\u25A0');

            ProcessOutput += bla + "\n";
            IsExpanded = true;
            if (e.Data.IndexOf("actualwords") > -1)
            {
                ActualWords = "Words: " + e.Data.Split(':')[1];
            }
            if (e.Data.IndexOf("optionend") > -1)
            {
                addOption = false;
                startListening = false;
            }
            if (e.Data.IndexOf("custom") > -1)
            {
                writer.WriteLine("emptystring");
            }
            if (startListening)
            {
                if (e.Data.IndexOf("!!!") > -1)
                {
                    addOption = true;
                }
                else
                {
                    if (addOption)
                    {
                        addOption = false;
                        Options.Add(new OptionInFigureViewModel());
                        Options.Last().Square = this.Square;
                        Options.Last().Index = Options.Count;
                    }
                    Options.Last().OptionOne += e.Data + "\n";
                }
            }
            if (e.Data.IndexOf("optionstart") > -1)
            {
                Options = new List<OptionInFigureViewModel>();
                waitingForInput = true;
                startListening = true;
                addOption = true;
            }
        }
    }
}
