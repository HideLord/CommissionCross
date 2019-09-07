using Cross.Aggregator;
using Cross.Data;
using Cross.Services.Contracts;
using Newtonsoft.Json;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_Cross.Functional;

namespace WPF_Cross.ViewModels
{
    public class TemplatePanelViewModel : INotifyPropertyChanged
    {
        private void LoadTemplates()
        {
            AllTemplates = new ObservableCollection<TemplateFormData>(templateService.LoadTemplates("Templates/"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllTemplates"));
        }

        private void handleTemplateChanges(TemplateFormData data)
        {
            for (var i = 0; i < AllTemplates.Count; i++)
            {
                if (AllTemplates[i].TemplateName == data.TemplateName)
                {
                    AllTemplates[i] = data;
                    return;
                }
            }
            AllTemplates.Add(data);
        }

        private ICommand clickTemplateCommand;
        private ICommand deleteCommand;

        private readonly IEventAggregator eventAggregator;
        private readonly ITemplateService templateService;

        private TemplateFormData currentTemplate;
        public TemplateFormData CurrentTemplate
        {
            get => currentTemplate;
            set
            {
                currentTemplate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentTemplate"));
                eventAggregator.GetEvent<FormDataChanges>().Publish(currentTemplate.Data);
            }
        }

        public ObservableCollection<TemplateFormData> AllTemplates { get; set; }
        public TemplatePanelViewModel(IEventAggregator eventAggregator, ITemplateService templateService)
        {
            this.eventAggregator = eventAggregator;
            this.templateService = templateService;
            AllTemplates = new ObservableCollection<TemplateFormData>();

            eventAggregator.GetEvent<TemplateChanges>().Subscribe(handleTemplateChanges);

            LoadTemplates();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ClickTemplateCommand
        {
            get
            {
                if(clickTemplateCommand == null)
                {
                    return clickTemplateCommand = new RelayCommand<TemplateFormData>(handleTemplateChange);
                }
                return clickTemplateCommand;
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                {
                    return deleteCommand = new RelayCommand<TemplateFormData>(handleDeletion);
                }
                return deleteCommand;
            }
        }

        private void handleDeletion(TemplateFormData obj)
        {
            templateService.DeleteTemplate(obj);
            LoadTemplates();
        }

        private void handleTemplateChange(TemplateFormData obj)
        {
            eventAggregator.GetEvent<LoadFromTemplate>().Publish(obj);
        }
    }
}
