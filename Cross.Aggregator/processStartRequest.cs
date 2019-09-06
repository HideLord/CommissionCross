using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Aggregator
{
    // This should be IProcessService but I don't
    // want to add the reference just for one line of code
    public class ProcessStartRequest : PubSubEvent<object>
    {

    }
}
