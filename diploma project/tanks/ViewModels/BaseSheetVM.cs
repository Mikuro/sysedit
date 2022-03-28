using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.ViewModels
{
    public enum SheetContentType
    {
        FlowDiagram
    }
    public abstract class BaseSheetVM : INotifyPropertyChanged
    {
        public SheetContentType SheetContentTypeValue { get; set; }
        public string DocName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract void Stop();
        public abstract void Initialize();
        public abstract void Done();
    }
}
