using AutoDischange.Model;
using AutoDischange.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace AutoDischange.ViewModel.Commands
{
    public class ActivityComponentCommand : ICommand
    {
        public ActivityComponentVM ViewModel { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public ActivityComponentCommand(ActivityComponentVM vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            ActivityComponent data = parameter as ActivityComponent;

            if (data == null)
                return false;
            if (string.IsNullOrEmpty(data.PathStart))
                return false;
            if (string.IsNullOrEmpty(data.PathDownload))
                return false;

            if (!Path.IsPathRooted(data.PathStart)) //La ruta debe ser valida
                return false;
            if (!Path.IsPathRooted(data.PathDownload)) //La ruta debe ser valida
                return false;
            if (data.PathStart == data.PathDownload) //la ruta no debe ser igual
                return false;
            //string dbFile = DatabaseHelper.dbFile;
            string dbFile = Path.Combine(Environment.CurrentDirectory, "dischangesPath.db3");
            if (File.Exists(dbFile) != true)
                return false;

            return true;
        }

        public void Execute(object parameter)
        {
            //using (var fbd = new FolderBrowserDialog())
            //{
            //    DialogResult result = fbd.ShowDialog();

            //    if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            //    {
            //        //ViewModel.GenericInput(fbd.SelectedPath);
            //        ViewModel.ExcuteActivityExport(fbd.SelectedPath);

            //    }
            //}
            ViewModel.ExcuteActivityExport();
        }
    }
}
