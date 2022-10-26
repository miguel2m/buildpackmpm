using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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


namespace AutoDischange.View
{
    /// <summary>
    /// Lógica de interacción para ListComponentWindow.xaml
    /// </summary>
    public partial class ListComponentWindow : Window
    {
        public ListComponentWindow()
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
                    tbPathComponent.Text = fbd.SelectedPath;
                }
            }
        }

        //private void cbAllFeatures_CheckedChanged(object sender, RoutedEventArgs e)
        //{
        //    bool newVal = (cbAllFeatures.IsChecked == true);
        //    hogar_1.IsChecked = newVal;
        //    cuatro_uno.IsChecked = newVal;
        //    prod.IsChecked = newVal;
        //    hog_2.IsChecked = newVal;
        //}

        //private void cbFeature_CheckedChanged(object sender, RoutedEventArgs e)
        //{
        //    cbAllFeatures.IsChecked = null;
        //    if ((hogar_1.IsChecked == true) && (cuatro_uno.IsChecked == true) && (prod.IsChecked == true) && (hog_2.IsChecked == true))
        //        cbAllFeatures.IsChecked = true;
        //    if ((hogar_1.IsChecked == false) && (cuatro_uno.IsChecked == false) && (prod.IsChecked == false) && (hog_2.IsChecked == false))
        //        cbAllFeatures.IsChecked = false;
        //}
    }
}
