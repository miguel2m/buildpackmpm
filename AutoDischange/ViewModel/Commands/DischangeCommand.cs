using AutoDischange.ViewModel.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace AutoDischange.ViewModel.Commands
{
    public class DischangeCommand :ICommand
    {
        public DischangeVM ViewModel { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DischangeCommand(DischangeVM vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            string dbFile = DatabaseHelper.dbFile;
            return (File.Exists(dbFile) == true)?true:false;
        }

        public void Execute(object parameter)
        {
            ViewModel.DischangeStatus = "Cargando lista de changesets...";
            //TODO: Call login from ViewModel
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (dialog.ShowDialog() == true)
            {
                ViewModel.GetExcel(dialog.FileName);
                //selectedImage.Source = new BitmapImage(new Uri(fileName));

                //MakePredictionAsync(fileName);
            }
            
        }
    }
}
