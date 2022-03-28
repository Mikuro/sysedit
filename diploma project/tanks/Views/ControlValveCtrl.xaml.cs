using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using tanks.Dialogs;
using tanks.ViewModels;

namespace tanks.Views
{
    /// <summary>
    /// Логика взаимодействия для ControlValveCtrl.xaml
    /// </summary>
    public partial class ControlValveCtrl : BaseCtrl
    {
        public ControlValveCtrl()
        {
            InitializeComponent();

            MouseLeftButtonDown += ControlValveCtrl_MouseLeftButtonDown;
        }

        private void ControlValveCtrl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            /*
            var dlg = new FaceControlValve();

            var cv = (DataContext as ControlValve);

            dlg.Title = cv.Id;
            dlg.mv.Text = cv.mv.ToString("#0.0#####", CultureInfo.InvariantCulture);

            if (dlg.ShowDialog()==true)
            {
                cv.mv=Double.Parse(dlg.mv.Text, CultureInfo.InvariantCulture);
            }
            */
        }
    }
}
