using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.ViewModels
{
    public class CalculationSystem : Models.CalculationSystem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //public string ContentType { get { return "System"; } set { } }
        public CalculationSystem()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Display"));
        }
    }
}
