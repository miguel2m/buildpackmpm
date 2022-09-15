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

namespace AutoDischange.ViewModel
{
    public class ListComponentVM : INotifyPropertyChanged
    {
        public ObservableCollection<ListComponent> ListComponentObjs { get; set; }
        public ListComponentCommand ListComponentCommand { get; set; }
        private ListComponent listComponent;
        public ListComponent ListComponent
        {
            get { return listComponent; }
            set
            {
                listComponent = value;
                OnPropertyChanged("ListComponent");
            }
        }
        
        private string listComponentStatus;
        public string ListComponentStatus
        {
            get { return listComponentStatus; }
            set
            {
                listComponentStatus = value;               
                OnPropertyChanged("ListComponentStatus");
            }
        }
        private string pathComponent;
        public string PathComponent
        {
            get { return pathComponent; }
            set
            {
                pathComponent = value;
                ListComponent = new ListComponent
                {
                    Path = pathComponent,
                    Branch = this.SelectedBranch,
                };
                OnPropertyChanged("PathComponent");
            }
        }

        private string selectedBranch;
        public string SelectedBranch
        {
            get { return selectedBranch; }
            set
            {
                selectedBranch = value;
                ListComponent = new ListComponent
                {
                    Path = this.PathComponent,
                    Branch = selectedBranch,
                };
                OnPropertyChanged("SelectedBranch");
            }
        }
        
        public ListComponentVM()
        {
            ListComponentCommand = new ListComponentCommand(this);
            ListComponentObjs = new ObservableCollection<ListComponent>();
            ListComponent listcomponentobjtemp1 = new ListComponent();
            ListComponent listcomponentobjtemp2 = new ListComponent();
            listcomponentobjtemp1.Branch = "test1";
            listcomponentobjtemp2.Branch = "test2";
            ListComponentObjs.Add(listcomponentobjtemp1);
            ListComponentObjs.Add(listcomponentobjtemp2);
            //LoadBranch();
            ListComponent = new ListComponent();
            //SelectedBranch = new ListComponent();

        }

        public void LoadBranch()
        {

            string rutaBranch = ConfigurationManager.AppSettings["rutaJenkins"];
            string[] subDir = Directory.GetDirectories(rutaBranch);
            foreach (var item in subDir.Select((value, i) => new { i, value }))
            {
                var value = UtilHelper.nameFile(item.value, '\\');
                var index = item.i;
                ListComponent ListComponentObjTemp1 = new ListComponent();
                ListComponentObjTemp1.Branch = value;
                ListComponentObjs.Add(ListComponentObjTemp1);


            }



        }

        public async void copyToJenkins()
        {
            var changesetList = (DatabaseHelper.Read<DischangeChangeset>()).ToList();
            List<TfsItem> TodosItemTfs = new List<TfsItem>();
            List<DischangePath> DischangePathList = new List<DischangePath>();
            List<string> PathGU = new List<string>();
            if (changesetList.Count > 0)
            {
                foreach (DischangeChangeset item in changesetList)
                {
                    //TODOS LOS COMPONENTES DEL CHANGESET
                    List<TfsItem> TodosItmTfs2 = await TFSRequest.GetChangeset(item.Changeset);
                    if (!string.IsNullOrEmpty(item.Branch))
                    {
                        TodosItmTfs2 = TodosItmTfs2.FindAll((item2) => item2.path.Contains(item.Branch));
                    }

                    if (TodosItmTfs2.FirstOrDefault() != null)
                    {
                        foreach (TfsItem itemLocal in TodosItmTfs2)
                        {
                            if (!string.IsNullOrEmpty(item.Branch))
                            {
                                if (itemLocal.path.Contains(item.Branch))
                                {
                                    TodosItemTfs.Add(itemLocal);
                                }
                            }
                            //else
                            //{
                            //    TodosItemTfs.Add(itemLocal);
                            //}
                        }
                    }
                }

                //obtener lista guia de ubicaciones 

                if (TodosItemTfs.Count > 0)
                {
                    foreach (TfsItem item in TodosItemTfs)
                    {
                        string valueString = String.Empty;
                        if (item.path.Contains("cs"))
                        {
                            List<string> listValue = UtilHelper.fileList(item.path, '/');
                            valueString = listValue.First(i => i.Contains("mpm.seg"));

                        }
                        else
                        {
                            valueString = UtilHelper.nameFile(item.path, '/');
                        }
                        //guia de ubicaciones
                        DischangePathList = (DatabaseHelper.Read<DischangePath>()).Where(n => n.Path.Contains(valueString)).ToList();
                        for (int i = 0; i < DischangePathList.Count; i++)
                        {
                            PathGU.Add(DischangePathList[i].Path);
                        }
                    }
                }
            }
            string result = string.Empty;
            string branch = ListComponent.Branch;
            //string rutaCont = @"C:\Users\edgar.linarez\OneDrive - MPM SOFTWARE SLU\Documentos\Edgar\pruebas\";
            string rutaCont = ListComponent.Path;
            if (PathGU.Count > 0)
            {
                foreach (string itemPathGU in PathGU)
                {
                    result = TransferFileJenkinsHelper.JenkinsTransferFile(itemPathGU, rutaCont, branch);
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
