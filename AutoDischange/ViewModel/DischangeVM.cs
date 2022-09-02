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
                DischangeStatus = "Selección de changeset";
                //GetNotes();
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
                //DischangeStatus = "Selección de changeset";
                //GetNotes();
            }
        }


        public DischangeVM()
        {
            DischangeCommand = new DischangeCommand(this);
            DischangeChangesets = new ObservableCollection<DischangeChangeset>();
            DischangeStatus = "Nada que hacer";
        }

        public void GetExcel(string fileName)
        {
            DischangeChangesets.Clear();

            
            foreach (var changeset in ExcelHelper.ReadExcel(fileName))
            {
                DischangeChangesets.Add(changeset);
            }
            DischangeStatus = "Lista de changesets cargada";

            //Tfs = await TFSRequest.GetChangeset("162501");
            ////Tfs.value.ForEach(Console.WriteLine);
            //Console.WriteLine(Tfs.count);
            //foreach (Value element in Tfs.value)
            //{
            //    Console.WriteLine($"{element.item.path} ");
            //}
        }

        public async void GetChangeset()
        {


            if (SelectedChangeset != null)
            {
                DischangeStatus = "Obteniendo Changeset del TFS";

                Tfs = await TFSRequest.GetChangeset(SelectedChangeset.Changeset);
                //Console.WriteLine(Tfs.count);
                //foreach (Value element in Tfs.value)
                //{
                //    Console.WriteLine($"{element.item.path} ");
                //}

                DischangeStatus = "Changeet del TFS Obtenido";
            }
            
            

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
