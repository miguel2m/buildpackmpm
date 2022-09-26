
using AutoDischange.Model;
using AutoDischange.View.CustomControl;
using AutoDischange.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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

namespace AutoDischange.View
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<BranchJenkins> lstNombDir = new List<BranchJenkins>();
        public MainWindow()
        {
            InitializeComponent();
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ActivityComponent_Click(object sender, RoutedEventArgs e)
        {
            ActivityReportWindow activityReportWindow = new ActivityReportWindow();
            activityReportWindow.ShowDialog();
        }

        

        private void ToolItem_Click(object sender, RoutedEventArgs e)
        {
            ToolWindow toolWindowWindow = new ToolWindow();
            toolWindowWindow.ShowDialog();
        }
        private void DiffComponent_Click(object sender, RoutedEventArgs e)
        {
            DiffComponentWindow diffWindowWindow = new DiffComponentWindow();
            diffWindowWindow.ShowDialog();
        }
        

        private void ListComponentItem_Click(object sender, RoutedEventArgs e)
        {
            ListComponentWindow listComponentWindow = new ListComponentWindow();
            listComponentWindow.ShowDialog();
        }

        private void SpeechButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //var selected = sidebar.SelectedItem as CustomControl.NavButton;

            //navframe.Navigate(selected.Navlink);

        }
    }
}
