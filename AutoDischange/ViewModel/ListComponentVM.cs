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
            //ListComponent listcomponentobjtemp1 = new ListComponent();
            //ListComponent listcomponentobjtemp2 = new ListComponent();
            //listcomponentobjtemp1.Branch = "test1";
            //listcomponentobjtemp2.Branch = "test2";
            //ListComponentObjs.Add(listcomponentobjtemp1);
            //ListComponentObjs.Add(listcomponentobjtemp2);
            LoadBranch();
            ListComponent = new ListComponent();
            //SelectedBranch = new ListComponent();

        }

        public void LoadBranch()
        {

            string rutaBranch = ConfigurationManager.AppSettings["rutaJenkins"];
            string[] subDir = Directory.GetDirectories(rutaBranch, "20*", SearchOption.TopDirectoryOnly);
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
            string result = string.Empty, rutaPack = string.Empty, rutaF = string.Empty;
            bool flag = false;
            ListComponentStatus = $"Leyendo paquetes";
            var changesetList = (DatabaseHelper.Read<DischangeChangeset>()).ToList();
            List<TfsItem> TodosItemTfs = new List<TfsItem>();
            string branch = ListComponent.Branch;
            List<DischangePath> DischangePathList = new List<DischangePath>();
            List<string> PathGU = new List<string>();
            if (changesetList.Count > 0)
            {
                try
                {
                    ListComponentStatus = $"Leyendo Changeset";
                    foreach (DischangeChangeset item in changesetList)
                    {
                        //INDICO QUE LA OPCION SELECCIONADA DEBE CONTENER PARTE DE LOS DATOS DEL EXCEL DEL USUARIO
                        if (branch.Contains(item.Branch))
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
                                }
                            }
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                            ListComponentStatus = $"La opcion {branch} no coincide con el Excel cargado.";
                        }
                    }
                }
                catch (Exception e)
                {
                    string exc = e.Message;
                    ListComponentStatus = $"Exception:{exc}";
                }

                if (flag)
                {
                    //obtener lista guia de ubicaciones 
                    try
                    {
                        if (TodosItemTfs.Count > 0)
                        {
                            ListComponentStatus = $"Cargando datos de TFS.";
                            List<string> list = new List<string>();
                            string valueString = String.Empty, ext = string.Empty;
                            foreach (TfsItem item in TodosItemTfs)
                            {
                                ext = Path.GetExtension(item.path);

                                list = item.path.Split('.').ToList();
                                if (ext != ".csproj")
                                {
                                    if (ext == ".cs")
                                    {
                                        List<string> listValue = UtilHelper.fileList(item.path, '/');
                                        valueString = listValue.FirstOrDefault(i => i.Contains("mpm.seg"));
                                    }
                                    else
                                    {
                                        if (ext == ".sql")
                                        {
                                            bool flag2 = false;
                                            valueString = UtilHelper.extraerBranchTfs(item.path, '/');
                                            string[] info = item.path.Split('/');

                                            foreach (string s in info)
                                            {
                                                if (s == "BD")
                                                {
                                                    flag2 = true;
                                                }
                                                if (flag2 && s != "BD")
                                                {
                                                    valueString += $"\\{s}";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            valueString = UtilHelper.nameFile(item.path, '/');
                                        }
                                    }

                                    if (valueString != null)
                                    {
                                        if (ext == ".sql")
                                        {
                                            PathGU.Add(valueString);
                                        }
                                        else
                                        {
                                            //guia de ubicaciones
                                            DischangePathList = (DatabaseHelper.Read<DischangePath>()).Where(n => n.Path.Contains(valueString)).ToList();

                                            for (int i = 0; i < DischangePathList.Count; i++)
                                            {
                                                PathGU.Add(DischangePathList[i].Path);
                                            }

                                        }

                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string exc = ex.Message;
                        ListComponentStatus = $"Exception:{exc}";
                    }
                }
            }
            if (flag)
            {
                string rutaCont = ListComponent.Path + "\\";
                if (PathGU.Count > 0)
                {
                    ListComponentStatus = $"Transfiriendo archivos de Jenkins.";

                    //voy agregar el directorio donde se va a agregar el paquete del Jenkins
                    rutaF = rutaCont + $@"{branch}_{DateTime.Now.ToString("yyyyMMddHHmmss")}\";

                    if (!Directory.Exists(rutaF))
                    {
                        //Crear el directorio
                        Directory.CreateDirectory(rutaF);
                    }


                    foreach (string itemPathGU in PathGU)
                    {
                        result = TransferFileJenkinsHelper.JenkinsTransferFile(itemPathGU, rutaF, branch);
                    }
                    ListComponentStatus = $"Transferencia de archivos culminado.";
                }
                else
                {
                    ListComponentStatus = $"No hay archivos para transferir.";
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
