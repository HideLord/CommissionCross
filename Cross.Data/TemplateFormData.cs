using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Data
{
    public class TemplateFormData : ICloneable, INotifyPropertyChanged
    {
        private FormData data;
        public FormData Data
        {
            get => data;
            set
            {
                data = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Data"));
            }
        }
        private string filePath = "";
        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilePath"));
            }
        }
        private string templateName = "";
        public string TemplateName
        {
            get => templateName;
            set
            {
                templateName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TemplateName"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public object Clone()
        {
            var clone = new TemplateFormData();
            clone.Data = (FormData)data.Clone();
            clone.FilePath = (string)filePath.Clone();
            clone.TemplateName = (string)templateName.Clone();
            return clone;
        }
    }
}
