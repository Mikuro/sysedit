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
    /// Логика взаимодействия для PIDControllerCtrl.xaml
    /// </summary>
    public partial class PIDControllerCtrl : BaseCtrl
    {
        public PIDControllerCtrl()
        {
            InitializeComponent();
            MouseLeftButtonDown += PIDControllerCtrl_MouseLeftButtonDown;
        }

        private void PIDControllerCtrl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            /*
            var dlg = new FacePIDController();

            var cv = (DataContext as PIDController);

            dlg.MVSPAN = cv.MVSPAN;
            dlg.PVSPAN = cv.PVSPAN[0];
            dlg.PVBASE = cv.PVSPAN[1];

            dlg.Title = cv.Id;
            dlg.CMOD.Text = cv.CMOD.ToString("#0", CultureInfo.InvariantCulture);
            dlg.SVM.Text = cv.SVM.ToString("#0.0#####", CultureInfo.InvariantCulture);
            dlg.MVM.Text = cv.MVM.ToString("#0.0#####", CultureInfo.InvariantCulture);

            if (dlg.ShowDialog() == true)
            {
                cv.CMOD = Int32.Parse(dlg.CMOD.Text, CultureInfo.InvariantCulture);
                cv.SVM = Double.Parse(dlg.SVM.Text, CultureInfo.InvariantCulture);
                cv.MVM = Double.Parse(dlg.MVM.Text, CultureInfo.InvariantCulture);
            }
            */
        }
    }
}
