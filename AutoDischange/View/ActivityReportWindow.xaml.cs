using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Button = System.Windows.Controls.Button;
using TextBox = System.Windows.Controls.TextBox;

namespace AutoDischange.View
{
    /// <summary>
    /// Lógica de interacción para ActivityReportWindow.xaml
    /// </summary>
    public partial class ActivityReportWindow : Window
    {
        public ActivityReportWindow()
        {
            InitializeComponent();
        }

        private void PickupFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Button button = (Button)sender;
                    string buttonId = button.Name;
                    TextBox txtNumber = (TextBox)this.inputContainer.FindName(buttonId + "Text");
                    txtNumber.Text = fbd.SelectedPath;

                }
            }
        }
    }

}
