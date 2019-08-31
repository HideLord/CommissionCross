using Cross.Data;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Aggregator
{
    public class FormDataChanges : PubSubEvent<FormData>
    {
    }
}
