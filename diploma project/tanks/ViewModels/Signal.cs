using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.ViewModels
{
    public class Signal : Models.Signal, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ContentType { get { return "Signal"; } set { } }
        public Signal()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Display"));
        }
    }
}
