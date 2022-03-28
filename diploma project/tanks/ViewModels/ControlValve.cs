using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.ComponentModel;

namespace tanks.ViewModels
{
    public class ControlValve : Models.ControlValve, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ContentType { get { return "ControlValve"; } set { } }
        public String Display { get { return pos.ToString("#0.0#", CultureInfo.InvariantCulture); } set { } }
        public ControlValve()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Display"));
        }

        public override void UpdateDisplay()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Display"));
        }
    }
}
