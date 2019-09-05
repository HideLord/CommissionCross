using Cross.Aggregator;
using Cross.Data;
using Cross.Services.Contracts;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_Cross.Functional;

namespace WPF_Cross.ViewModels
{
    public class SaveTemplateViewModel
    {
        private readonly ITemplateService templateService;
        private readonly IEventAggregator eventAggregator;
        private ICommand saveTemplateCommand;

        public TemplateFormData CurrentData { get; set; }

        private void handleFormDataChanges(FormData obj)
        {
            CurrentData.Data = (FormData)obj.Clone();
        }

        private void saveTemplate(object obj)
        {
            templateService.SaveTemplate(CurrentData);
            eventAggregator.GetEvent<TemplateChanges>().Publish((TemplateFormData)CurrentData.Clone());
        }

        public SaveTemplateViewModel(ITemplateService templateService, IEventAggregator eventAggregator)
        {
            this.templateService = templateService;
            this.eventAggregator = eventAggregator;
            eventAggregator.GetEvent<FormDataChanges>().Subscribe(handleFormDataChanges);
            CurrentData = new TemplateFormData();
            CurrentData.FilePath = "Templates/";
            CurrentData.Data = new FormData();
        }
        
        public ICommand SaveTemplateCommand
        {
            get
            {
                if (saveTemplateCommand == null)
                {
                    saveTemplateCommand = new RelayCommand<object>(saveTemplate);
                }
                return saveTemplateCommand;
            }
        }

    }
}
