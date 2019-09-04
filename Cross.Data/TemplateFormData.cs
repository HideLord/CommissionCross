using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Data
{
    public class TemplateFormData : BaseNotify
    {
        public FormData Data { get; set; }
        public string FilePath { get; set; }
        public string TemplateName { get; set; }
    }
}
