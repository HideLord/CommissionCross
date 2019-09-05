using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Data
{
    public class TemplateFormData : BaseNotify, ICloneable
    {
        public FormData Data { get; set; }
        public string FilePath { get; set; }
        public string TemplateName { get; set; }

        public object Clone()
        {
            var clone = new TemplateFormData();
            clone.Data = (FormData)Data.Clone();
            clone.FilePath = (string)FilePath.Clone();
            clone.TemplateName = (string)TemplateName.Clone();
            return clone;
        }
    }
}
