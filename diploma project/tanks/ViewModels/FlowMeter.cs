using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.ComponentModel;

namespace tanks.ViewModels
{
    public class FlowMeter : Models.FlowMeter, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ContentType { get { return "FlowMeter"; } set { } }
        public String Display { get { return F.ToString("#0.0#", CultureInfo.InvariantCulture); } set { } }
        public FlowMeter()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Display"));
        }

        public override void UpdateDisplay()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Display"));
        }
    }
}
