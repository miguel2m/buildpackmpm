using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Input;

namespace AutoDischange.ViewModel.Commands
{
    public class SettingCommand : ICommand
    {
        public SettingVM ViewModel { get; set; }
        public event EventHandler CanExecuteChanged;

        public SettingCommand(SettingVM vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            //bool selectedNotebook = parameter as bool;
            //return selectedNotebook != null ? true : false;
            return true;
        }

        public void Execute(object parameter)
        {
            //ViewModel.DischangeStatus = "Cargando lista de changesets...";
            //TODO: Call login from ViewModel
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (dialog.ShowDialog() == true)
            {
                ViewModel.SyncDischanges(dialog.FileName);
                //selectedImage.Source = new BitmapImage(new Uri(fileName));

                //MakePredictionAsync(fileName);
            }

        }
    }
}
