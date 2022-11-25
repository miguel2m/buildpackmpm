using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Input;
using AutoDischange.Model;
using System.IO;

namespace AutoDischange.ViewModel.Commands
{
    public class ListComponentCommand : ICommand
    {
        public ListComponentVM ViewModel { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public ListComponentCommand(ListComponentVM vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            //bool selectedNotebook = parameter as bool;
            //return selectedNotebook != null ? true : false;
            ListComponent data = parameter as ListComponent;

            if (data == null)
                return false;
            if (string.IsNullOrEmpty(data.Path))
                return false;
            //if (string.IsNullOrEmpty(data.Branch))
            //    return false;

            if (!Path.IsPathRooted(data.Path)) //La ruta debe ser valida
                return false;
            if (Path.HasExtension(data.Path)) //la ruta debe ser un directorio
                return false;
            return true;
        }

        public async void Execute(object parameter)
        {
            await ViewModel.CopyToJenkinsAsync();
        }
    }
}
