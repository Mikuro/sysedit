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
using System.Windows.Shapes;

namespace tanks.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для FaceControlValve.xaml
    /// </summary>
    public partial class FaceControlValve : Window
    {
        public FaceControlValve()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double mv=Double.Parse(this.mv.Text, CultureInfo.InvariantCulture);
            if((mv>=0.0)&&(mv<=1.0))
                this.DialogResult = true;
            else 
            {
                MessageBox.Show("mv not in range (0.0 - 1.0)");
            }
        }
    }
}
