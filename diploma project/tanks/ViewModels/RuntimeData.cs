using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tanks.Models;

namespace tanks.ViewModels
{
    public class StreamInfo : INotifyPropertyChanged
    {
        public string From { get; set; }
        public string To { get; set; }
        public double P { get; set; }
        public double F { get; set; }
        public double T { get; set; }
        public double H { get; set; }
        public double ro { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateDisplay()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("F"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("P"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("T"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("H"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ro"));
        }
    }

    public class RuntimeData : INotifyPropertyChanged
    {
        public ObservableCollection<StreamInfo> Items {get;set;} = new ObservableCollection<StreamInfo>();

        public event PropertyChangedEventHandler PropertyChanged;
        public void buildData(FlowDiagram fd)
        {
            for (int i = 0; i < fd.Links.Count; i++)
            {
                var stream = fd.Links[i] as Models.Stream;

                if (stream == null) continue;

                var si = new StreamInfo
                {
                    From = stream.From,
                    To = stream.To,
                    P = stream.P,
                    F = stream.F,
                    T = stream.T,
                    H = stream.HL * (1.0 - stream.RV) + stream.HV * stream.RV,
                    ro = stream.Mw / stream.V
                };

                Items.Add(si);
            }
        }
        public void UpdateData(FlowDiagram fd)
        {
            int j = 0;
            for (int i = 0; i < fd.Links.Count; i++)
            {
                var stream = fd.Links[i] as Models.Stream;

                if (stream == null) continue;

                Items[j].P = stream.P;
                Items[j].F = stream.F;
                Items[j].T = stream.T;
                Items[j].H = stream.HL * (1.0 - stream.RV) + stream.HV * stream.RV; 
                Items[j].ro = stream.Mw / stream.V;

                Items[j].UpdateDisplay();
                j++;
            }
        }
    }
}
