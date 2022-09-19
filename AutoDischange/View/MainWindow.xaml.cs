
using AutoDischange.Model;
using AutoDischange.View.CustomControl;
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
            InitializeCmbBranch();
        }
        private void InitializeCmbBranch()
        {
            //string rutaBranch = ConfigurationManager.AppSettings["rutaJenkins"];
            //string[] subDir = Directory.GetDirectories(rutaBranch);
            //foreach (var item in subDir.Select((value, i) => new { i, value }))
            //{
            //    var value = nameDir(item.value);
            //    var index = item.i;
            //    lstNombDir.Add(new BranchJenkins { CodBranch = index, NameBranch = value });
            //}
            //this.CmbNameBranch.DisplayMemberPath = "NameBranch";
            //this.CmbNameBranch.SelectedValuePath = "CodBranch";
            //this.CmbNameBranch.ItemsSource = lstNombDir;
        }

        public string nameDir(string url)
        {
            List<string> list = new List<string>();
            list = url.Split('\\').ToList();
            int count = list.Count;
            string result = list[count - 1];

            return result;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
