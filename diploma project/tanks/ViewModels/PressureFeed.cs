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
    public class PressureFeed : Models.PressureFeed, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ContentType { get { return "PressureFeed"; } set { } }
        
        public PressureFeed()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Display"));
        }
        public override void UpdateDisplay()
        {
        }
        /*
        public override object GetToolTip()
        {
            String s = "";

            for(int i=0; i< parent.Components.Count;i++)
                s+=String.Format(CultureInfo.InvariantCulture,"{0} {1}\n",
                    x[i].ToString("#0.0#####", CultureInfo.InvariantCulture), parent.Components[i].Id);
            return s;
        }
        */
    }
}
