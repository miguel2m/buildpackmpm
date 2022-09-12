using System;
using System.Windows;
using System.Windows.Input;

namespace AutoDischange.ViewModel.Commands
{
    public class PackageCommand :ICommand
    {
        public DischangeVM ViewModel { get; set; }
        public event EventHandler CanExecuteChanged;
        public PackageCommand(DischangeVM vm)
        {
            ViewModel = vm;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            

        }
    }
}
