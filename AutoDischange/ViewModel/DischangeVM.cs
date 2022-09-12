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
using System.Windows;

namespace AutoDischange.ViewModel
{
    public class DischangeVM : INotifyPropertyChanged
    {
        public ObservableCollection<DischangeChangeset> DischangeChangesets { get; set; }
        public DischangeCommand DischangeCommand { get; set; }

        public PackageCommand PackageCommand { get; set; }

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

        public ObservableCollection<TfsItem> TfsList{ get; set; }

      

        private TfsItem tfsSelected;
        public TfsItem TfsSelected
        {
            get { return tfsSelected; }
            set
            {
                tfsSelected = value;
                OnPropertyChanged("TfsSelected");
                GetTfsComponent();
            }
        }

        public ObservableCollection<DischangePath> ComponentList { get; set; }

        public DischangeVM()
        {
            DischangeCommand = new DischangeCommand(this);
            DischangeChangesets = new ObservableCollection<DischangeChangeset>();
            TfsList = new ObservableCollection<TfsItem>();
            ComponentList = new ObservableCollection<DischangePath>();
            DischangeStatus = "Nada que hacer.";
            PackageCommand = new PackageCommand(this);

        }

        public async void GetExcel(string fileName)
        {
            try
            {
                DischangeChangesets.Clear();
                foreach (var changeset in await ExcelHelper.ReadExcel(fileName))
                {
                    DischangeChangesets.Add(changeset);
                }
                DischangeStatus = "Lista de changesets cargada.";
            }
            catch (Exception ex)
            {
                DischangeStatus = $"Error en Excel: { ex.Message}.";
                MessageBox.Show("Error en Excel: " + ex.Message, "Error en Excel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        public async void GetChangeset()
        {
            if (SelectedChangeset != null)
            {
                try
                {
                    TfsList.Clear();
                    ComponentList.Clear();
                    DischangeStatus = "Obteniendo Path del TFS.";
                    //Tfs = await TFSRequest.GetChangeset(SelectedChangeset.Changeset);
                    foreach (TfsItem itemLocal in await TFSRequest.GetChangeset(SelectedChangeset.Changeset))
                    {
                        TfsList.Add(itemLocal);
                    }
                    
                    DischangeStatus = $"Path Changesets del TFS Obtenido cantidad: {TfsList.Count}.";
                }
                catch (Exception ex)
                {
                    DischangeStatus = $"Error al intentar conectar con el TFS: { ex.Message}.";
                    MessageBox.Show("Error al intentar conectar con el TFS: " + ex.Message, "Error al intentar conectar con el TFS", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }

        }

        public void GetTfsComponent()
        {
            if (TfsSelected != null)
            {
                try
                {
                    ComponentList.Clear();
                    string valueString = String.Empty;
                    if (TfsSelected.path.Contains("cs"))
                    {
                        List<string> listValue = UtilHelper.fileList(TfsSelected.path, '/');
                        valueString = listValue.First(i => i.Contains("mpm.seg"));

                    }
                    else
                    {
                        valueString = UtilHelper.nameFile(TfsSelected.path, '/');
                    }
                    var DischangePathList = (DatabaseHelper.Read<DischangePath>()).Where(n => n.Path.Contains(valueString)).ToList();
                    
                    foreach (DischangePath itemLocal in DischangePathList)
                    {
                        ComponentList.Add(itemLocal);
                    }

                    //DischangeStatus = $"Path Changesets del TFS Obtenido cantidad: {TfsList.Count}.";
                }
                catch (Exception ex)
                {
                    DischangeStatus = $"Error al intentar conectar con el DIS-Change.xlsx: { ex.Message}.";
                    MessageBox.Show("Error al intentar conectar con el DIS-Change.xlsx : " + ex.Message, "Error al intentar conectar con el DIS-Change.xlsx", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
