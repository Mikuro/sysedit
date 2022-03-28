using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.ViewModels
{
    public class Stream : Models.Stream, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ContentType { get { return "Stream"; } set { } }
        public Stream()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Display"));
        }
    }
}
