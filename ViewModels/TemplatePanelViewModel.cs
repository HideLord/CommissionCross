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

namespace WPF_Cross.ViewModels
{
    public class TemplatePanelViewModel : INotifyPropertyChanged
    {
        private void LoadTemplates()
        {
            AllTemplates = new ObservableCollection<TemplateFormData>(templateService.LoadTemplates("Templates/"));
        }

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
        public ObservableCollection<TemplateFormData> AllTemplates;
        public TemplatePanelViewModel(IEventAggregator eventAggregator, ITemplateService templateService)
        {
            this.eventAggregator = eventAggregator;
            this.templateService = templateService;
            AllTemplates = new ObservableCollection<TemplateFormData>();

            LoadTemplates();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
