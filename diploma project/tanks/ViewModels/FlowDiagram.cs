using System;
using System.ComponentModel;

namespace tanks.ViewModels
{
    public class FlowDiagram : Models.FlowDiagram, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
