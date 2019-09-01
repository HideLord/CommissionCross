using System;
using PropertyChanged;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Data
{
    [AddINotifyPropertyChangedInterface]
    public abstract class BaseNotify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender,e) => { };
    }
}
