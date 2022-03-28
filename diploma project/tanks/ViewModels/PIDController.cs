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
    public class PIDController : Models.PIDController, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ContentType { get { return "PIDController"; } set { } }

        public String Display { get { return SVM.ToString("#0.0#", CultureInfo.InvariantCulture); } set { } }

        public PIDController()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Display"));
        }
        public override void UpdateDisplay()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Display"));
        }
    }
}
