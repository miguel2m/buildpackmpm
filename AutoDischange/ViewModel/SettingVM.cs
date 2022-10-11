using AutoDischange.ViewModel.Commands;
using AutoDischange.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoDischange.ViewModel
{
    public class SettingVM : INotifyPropertyChanged
    {
        public SettingCommand SettingCommand { get; set; }

        private bool syncStatus;
        public bool SyncStatus
        {
            get { return syncStatus; }
            set
            {
                syncStatus = value;
                OnPropertyChanged("SyncStatus");
            }
        }

        private string statusExcel;

        public string StatusExcel
        {
            get { return statusExcel; }
            set
            {
                statusExcel = value;
                OnPropertyChanged("StatusExcel");
            }
        }

        public SettingVM()
        {
            SettingCommand = new SettingCommand(this);
            SyncStatus = true;
            StatusExcel = "Nada que hacer.";
        }

        public async void SyncDischanges(string filePath)
        {
            try
            {
               
                StatusExcel = "Cargando Excel....";
                SyncStatus = false ;
                //SyncStatus = await ExcelHelper.ReadExcelDIS_Changes(filePath);
                SyncStatus = await CsvHelper.ReadCSVDIS_Changes(filePath);              
                SyncStatus = true;
                //string dbFile = Path.Combine(Environment.CurrentDirectory, "dischangesPath.db3");
                StatusExcel = "Carga Excel finalizada.";
                MessageBox.Show("Listo ", "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
                //Console.WriteLine(File.Exists(dbFile));
                //if (File.Exists(dbFile) == false)
                //{

                //    //if (await ExcelHelper.ReadExcelDIS_Changes(filePath))
                //    //{
                //    //    //var notebooks = (DatabaseHelper.Read<DischangePath>()).Where(n => n.Path.Contains("pt")).ToList();
                //    //    //notebooks.ForEach(p => Console.WriteLine(p.Path));
                //    //};
                //}
                //Console.WriteLine(data.Path);
            }
            catch (Exception ex)
            {
                Log4net.log.Error(ex.Message);
                SyncStatus = true;
                StatusExcel = $"Error en Excel: { ex.Message}.";
                MessageBox.Show("Error en Excel: " + ex.Message, "Error en Excel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
