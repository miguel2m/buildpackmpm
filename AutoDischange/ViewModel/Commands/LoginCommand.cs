using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AutoDischange.ViewModel.Commands
{
    public class LoginCommand :ICommand
    {
        public LoginVM ViewModel { get; set; }
        public event EventHandler CanExecuteChanged;

        public LoginCommand(LoginVM vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //TODO: Call login from ViewModel
        }
    }
}
