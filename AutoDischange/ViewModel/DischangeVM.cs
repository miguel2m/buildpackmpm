using AutoDischange.Model;
using AutoDischange.ViewModel.Commands;
using AutoDischange.ViewModel.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.ViewModel
{
    public class DischangeVM : INotifyPropertyChanged
    {
        public ObservableCollection<DischangeChangeset> DischangeChangesets { get; set; }
        public DischangeCommand DischangeCommand { get; set; }

        private DischangeChangeset selectedChangeset;
        public DischangeChangeset SelectedChangeset
        {
            get { return selectedChangeset; }
            set
            {
                selectedChangeset = value;
                OnPropertyChanged("SelectedChangeset");
                DischangeStatus = "Selección de changeset.";
                GetChangeset();
            }
        }

        private string dischangeStatus;

        public string DischangeStatus
        {
            get { return dischangeStatus; }
            set {
                dischangeStatus = value;
                OnPropertyChanged("DischangeStatus");
            }
        }

        private TfsModel tfs;
        public TfsModel Tfs
        {
            get { return tfs; }
            set
            {
                tfs = value;
                OnPropertyChanged("Tfs");
            }
        }


        public DischangeVM()
        {
            DischangeCommand = new DischangeCommand(this);
            DischangeChangesets = new ObservableCollection<DischangeChangeset>();
            DischangeStatus = "Nada que hacer.";

            DISChangeRequest.DischangeGraphClientAsync();
        }

        public void GetExcel(string fileName)
        {
            DischangeChangesets.Clear();          
            foreach (var changeset in ExcelHelper.ReadExcel(fileName))
            {
                DischangeChangesets.Add(changeset);
            }
            DischangeStatus = "Lista de changesets cargada.";
        }

        public async void GetChangeset()
        {
            if (SelectedChangeset != null)
            {
                DischangeStatus = "Obteniendo Path del TFS.";
                Tfs = await TFSRequest.GetChangeset(SelectedChangeset.Changeset);        
                DischangeStatus = $"Path Changesets del TFS Obtenido cantidad: {Tfs.count}.";
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
