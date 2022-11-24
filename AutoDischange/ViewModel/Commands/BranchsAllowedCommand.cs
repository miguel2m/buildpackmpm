using AutoDischange.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AutoDischange.ViewModel.Commands
{
    public class BranchsAllowedCommand : ICommand
    {
        public DischangeVM ViewModel { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value;  }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public BranchsAllowedCommand(DischangeVM vm)
        {
            ViewModel = vm;
        }
        public bool CanExecute(object parameter)
        {
            DischangeChangeset changesetList = parameter as DischangeChangeset;

            return changesetList != null ? true : false;
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
