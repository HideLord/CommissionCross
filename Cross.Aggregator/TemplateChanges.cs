using Cross.Data;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Aggregator
{
    /// <summary>
    /// Signals that a template has been added.
    /// </summary>
    public class TemplateChanges : PubSubEvent<TemplateFormData>
    {
    }
}
