using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tanks.ViewModels;

namespace tanks
{
    public class MainWindowVM
    {
        public ObservableCollection<BaseSheetVM> Sheets { get; set; }
        public MainWindowVM()
        {
            Sheets = new ObservableCollection<BaseSheetVM>();
        }

        public void Initialize()
        {
            foreach (var sht in Sheets)
                sht.Initialize();
        }

        public void Stop()
        {
            foreach (var sht in Sheets)
                sht.Stop();
        }

        public void Done()
        {
            foreach (var sht in Sheets)
                sht.Done();
        }
    }
}
