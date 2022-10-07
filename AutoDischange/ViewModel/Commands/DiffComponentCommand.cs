using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Input;
using AutoDischange.Model;
using System.IO;
using System.Windows.Forms;
using AutoDischange.ViewModel.Helpers;

namespace AutoDischange.ViewModel.Commands
{
    public class DiffComponentCommand : ICommand
    {
        public DiffComponentVM ViewModel { get; set; }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DiffComponentCommand(DiffComponentVM vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            //bool selectedNotebook = parameter as bool;
            //return selectedNotebook != null ? true : false;
            DiffComponent data = parameter as DiffComponent;

            if (data == null)
                return false;
            if (string.IsNullOrEmpty(data.PathStart))
                return false;
            if (string.IsNullOrEmpty(data.PathEnd))
                return false;

            if (!Path.IsPathRooted(data.PathStart)) //La ruta debe ser valida
                return false;
            if (!Path.IsPathRooted(data.PathEnd)) //la ruta debe ser un directorio
                return false;
            if (data.PathStart == data.PathEnd) //la ruta no debe ser igual
                return false;

            //string dbFile = DatabaseHelper.dbFile;
            string dbFile = Path.Combine(Environment.CurrentDirectory, "dischangesPath.db3");
            if (File.Exists(dbFile) != true)
                return false;

            return true;
        }

        public void Execute(object parameter)
        {
           
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    ViewModel.GenericInput(fbd.SelectedPath);

                }
            }


        }
    }
}
