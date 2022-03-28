using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.ViewModels
{
    public class LiquidTank : Models.LiquidTank, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ContentType { get { return "LiquidTank"; } set { } }
        public LiquidTank()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Display")); 
        }
        public override void UpdateDisplay()
        {
        }
    }
}
