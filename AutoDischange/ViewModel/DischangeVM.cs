using AutoDischange.ViewModel.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.ViewModel
{
    public class DischangeVM : INotifyPropertyChanged
    {

        public DischangeCommand DischangeCommand { get; set; }

        public DischangeVM()
        {
            DischangeCommand = new DischangeCommand(this);
        }

        public void GetExcel(string fileName)
        {
            Console.WriteLine(fileName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //private void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
