using AutoDischange.Model;
using AutoDischange.ViewModel.Commands;
using AutoDischange.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoDischange.ViewModel
{
    public class BranchsUtilsComponentVM : INotifyPropertyChanged
    {
        public BranchUtilsComponentCommand UtilsComponentCommand { get; set; }

        private bool branchUtilsComponentStatus;
        public bool BranchUtilsComponentStatus
        {
            get { return branchUtilsComponentStatus; }
            set
            {
                branchUtilsComponentStatus = value;
                OnPropertyChanged("BranchUtilsComponentStatus");
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
        public BranchsUtilsComponentVM()
        {
            UtilsComponentCommand = new BranchUtilsComponentCommand(this);
            BranchUtilsComponentStatus = true;
            StatusExcel = "Nada que hacer";
        }

        public async void SyncBranchUtils(string filePath)
        {
            try
            {
                StatusExcel = "Cargando Archivo...";
                BranchUtilsComponentStatus = false;
                BranchUtilsComponentStatus = await CsvHelper.ReadCSVBranch_Utiles(filePath);
                BranchUtilsComponentStatus = true;
                StatusExcel = "Carga Finalizada.";
                MessageBox.Show("Listo ", "Listo", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                Log4net.log.Error(ex.Message);
                BranchUtilsComponentStatus = true;
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
