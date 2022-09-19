using AutoDischange.ViewModel;
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
    /// Lógica de interacción para DiffComponentWindow.xaml
    /// </summary>
    public partial class DiffComponentWindow : Window
    {
        
        DiffComponentVM viewModel;
        public DiffComponentWindow()
        {
            InitializeComponent();
            viewModel = Resources["vm"] as DiffComponentVM;
            viewModel.AddInput += ViewModel_AddInput;
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

        public void GenerateControls()
        {
            Button btnClickMe = new Button();
            btnClickMe.Content = "Click Me";
            btnClickMe.Name = "btnClickMe";
            btnClickMe.Click += new RoutedEventHandler(this.CallMeClick);
            inputContainer.Children.Add(btnClickMe);
            TextBox txtNumber = new TextBox();
            txtNumber.Name = "btnClickMe_txt";
            txtNumber.Text = "1776";
            txtNumber.Text = "{ Binding PathComponent, Mode = TwoWay, UpdateSourceTrigger = PropertyChanged}";
            inputContainer.Children.Add(txtNumber);
            inputContainer.RegisterName(txtNumber.Name, txtNumber);
        }
        protected void CallMeClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string buttonId = button.Name;
            TextBox txtNumber = (TextBox)this.inputContainer.FindName(buttonId+"_txt");
            string message = string.Format("The number is {0}", txtNumber.Text);
            System.Windows.MessageBox.Show(message);
        }

        private void ViewModel_AddInput(object sender, EventArgs e)
        {
            //GenerateControls();
        }
    }
}
