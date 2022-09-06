using AutoDischange.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoDischange.ViewModel
{
    public class MainVM 
    {

        public MainVM()
        {
            //try
            //{
            //    DISChangeRequest.DischangeGraphClientAsync();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    MessageBox.Show("Error al intentar conectar con el OneDrive: " + ex.Message, "Error al intentar conectar con el OneDrive", MessageBoxButton.OK, MessageBoxImage.Error);
            //};
            DISChangeRequest.DischangeGraphClientAsync();
        }
        
    }
}
