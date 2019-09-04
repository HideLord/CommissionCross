using Cross.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Services.Contracts
{
    public interface ITemplateService
    {
        List<TemplateFormData> LoadTemplates(string directory);
        void SaveTemplate(FormData data, string templateName, string filePath);
        void SaveTemplate(TemplateFormData template);
    }
}
