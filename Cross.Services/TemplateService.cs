using Cross.Data;
using Cross.Services.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Services
{
    public class TemplateService : ITemplateService
    {
        public List<TemplateFormData> LoadTemplates(string directory)
        {
            List<TemplateFormData> Templates = new List<TemplateFormData>();
            string[] files = System.IO.Directory.GetFiles(directory, "*.json");
            foreach (var jsonTemplatePath in files)
            {
                using (StreamReader reader = new StreamReader(jsonTemplatePath))
                {
                    string content = reader.ReadToEnd();
                    TemplateFormData loadedTemplate = JsonConvert.DeserializeObject<TemplateFormData>(content);
                    Templates.Add(loadedTemplate);
                }
            }
            return Templates;
        }

        public void SaveTemplate(FormData data, string templateName, string filePath)
        {
            TemplateFormData newTemplate = new TemplateFormData();
            newTemplate.Data = data;
            newTemplate.TemplateName = templateName;
            newTemplate.FilePath = filePath;
            SaveTemplate(newTemplate);
        }

        public void SaveTemplate(TemplateFormData template)
        {
            string content = JsonConvert.SerializeObject(template);
            string tempFileName = (template.FilePath + template.TemplateName + ".json");
            string tempFilePath = Path.GetFullPath(Directory.GetCurrentDirectory());

            File.WriteAllText(Path.Combine(tempFilePath, tempFileName), content);   
        }

        public void DeleteTemplate(TemplateFormData template)
        {
            string tempFileName = (template.FilePath + template.TemplateName + ".json");
            string tempFilePath = Path.GetFullPath(Directory.GetCurrentDirectory());

            File.Delete(Path.Combine(tempFilePath, tempFileName));
        }
    }
}
