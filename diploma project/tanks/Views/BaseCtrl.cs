using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using tanks.Dialogs;
using tanks.ViewModels;

namespace tanks.Views
{
    public class BaseCtrl : UserControl
    {
        public BaseCtrl()
        {
            MouseLeftButtonDown += BaseCtrl_MouseLeftButtonDown;
            //MouseEnter += BaseCtrl_MouseEnter;
        }

        private void BaseCtrl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //ToolTip = (DataContext as Models.ModelObject).GetToolTip();
        }

        private void BaseCtrl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dlg = new Face { DataContext = new FaceVM { mobj = DataContext as Models.ModelObject } };

            dlg.tab.SelectedIndex = 0;

            if (dlg.ShowDialog() == true)
            {
                (dlg.DataContext as FaceVM).Commit();
            }

            //throw new NotImplementedException();
            e.Handled = true;
            //Trace.WriteLine("MouseLeftButtonDown "+GetType().Name);
        }
    }
}
