using Cross.Services.Contracts;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_Cross.Functional;

namespace WPF_Cross.ViewModels
{
    public class CreateOpenViewModel : INotifyPropertyChanged
    {
        private readonly IProcessService processService;
        private readonly IEventAggregator eventAggregator;

        private ICommand startSVGForger;
        public ICommand StartSVGForger
        {
            get
            {
                if (startSVGForger == null) return startSVGForger = new RelayCommand<object>(handleStartSVGForger);
                return startSVGForger;
            }
        }

        private ICommand chooseCrossFile;
        public ICommand ChooseCrossFile
        {
            get
            {
                if (chooseCrossFile == null) return chooseCrossFile = new RelayCommand<object>(handleChooseCrossFile);
                return chooseCrossFile;
            }
        }

        private void handleChooseCrossFile(object obj)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.DefaultExt = ".ctb";
            openFileDlg.Filter = "Crossword files (.ctb)|*.ctb";
            openFileDlg.InitialDirectory = @"z:\cross\";

            bool? result = openFileDlg.ShowDialog();

            if (result == true)
            {
                CrossFileName = System.IO.Path.GetFileName(openFileDlg.FileName);
                crossFilePath = openFileDlg.FileName;
            }
        }

        private void handleStartSVGForger(object obj)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string crossFilePath;
        private string crossFileName;
        public string CrossFileName
        {
            get => crossFileName;
            set
            {
                crossFileName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CrossFileName"));
            }
        }

        public CreateOpenViewModel(IProcessService processService, IEventAggregator eventAggregator)
        {
            this.processService = processService;
            this.eventAggregator = eventAggregator;
        }
    }
}
