using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace AutoDischange.ViewModel.Commands
{
    public class NewNotebookCommand : ICommand
    {
        public MainVM ViewModel { get; set; }

        public event EventHandler CanExecuteChanged;

        public NewNotebookCommand(MainVM vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ViewModel.CreateNotebook();
        }
    }
}
