using AutoDischange.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoDischange.ViewModel.Commands
{
    public class BranchUtilsComponentCommand : ICommand
    {
        public BranchsUtilsComponentVM ViewModel { get; set; }

        public event EventHandler CanExecuteChanged;

        public BranchUtilsComponentCommand(BranchsUtilsComponentVM vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "CSV Files (*.csv)|*.csv";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (dialog.ShowDialog() == true)
            {
                ViewModel.SyncBranchUtils(dialog.FileName);
            }
        }
    }
}
