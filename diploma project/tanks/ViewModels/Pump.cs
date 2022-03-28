using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.ViewModels
{
    public class Pump : Models.Pump, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ContentType { get { return "Pump"; } set { } }
        public Pump()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Display"));
        }
        public override void UpdateDisplay()
        {
        }
    }
}
