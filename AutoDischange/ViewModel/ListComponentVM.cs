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
    public class ListComponentVM : INotifyPropertyChanged
    {
        public ObservableCollection<ListComponent> ListComponentObjs { get; set; }
        public ListComponentCommand ListComponentCommand { get; set; }
        private ListComponent listComponent;
        List<string> listaBranchs = new List<string>();
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
                    Path = pathComponent
                    //Branch = this.SelectedBranch,
                };
                OnPropertyChanged("PathComponent");
            }
        }

       
        private bool hogar1;
        public bool Hogar1
        {
            get { return hogar1; }
            set
            {
                hogar1 = value;
                OnPropertyChanged("Hogar1");
            }
        }
        private bool cuatroUno;
        public bool CuatroUno
        {
            get { return cuatroUno; }
            set
            {
                cuatroUno = value;
                OnPropertyChanged("CuatroUno");
            }
        }
        private bool prod;
        public bool Prod
        {
            get { return prod; }
            set
            {
                prod = value;
                OnPropertyChanged("Prod");
            }
        }

        private bool hogar2;
        public bool Hogar2
        {
            get { return hogar2; }
            set
            {
                hogar2 = value;
                OnPropertyChanged("Hogar2");
            }
        }

        //private string selectedBranch;
        //public string SelectedBranch
        //{
        //    get { return selectedBranch; }
        //    set
        //    {
        //        selectedBranch = value;
        //        ListComponent = new ListComponent
        //        {
        //            Path = this.PathComponent,
        //            Branch = selectedBranch,
        //        };
        //        OnPropertyChanged("SelectedBranch");
        //    }
        //}

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
            //LoadBranch();
            CuatroUno = true;
            ListComponent = new ListComponent();
            //SelectedBranch = new ListComponent();
            PathComponent=Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

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
            bool _hogar1_branch = Hogar1;
            bool _cuatroUno_branch = CuatroUno;
            bool _prod_branch = Prod;
            bool _hogar2_branch = Hogar2;

            Console.WriteLine("_hogar1_branch "+ Hogar1);
            Console.WriteLine("_cuatroUno_branch " + CuatroUno);
            Console.WriteLine("_prod_branch " + Prod);
            Console.WriteLine("_hogar2_branch " + Hogar2);

            string result = string.Empty, rutaPack = string.Empty, rutaF = string.Empty;
            ListComponentStatus = $"Leyendo paquetes";
            List<DischangeChangeset> changesetList = DatabaseHelper.Read<DischangeChangeset>().OrderBy(x => x.Branch).ToList();
            List<TfsItem> TodosItemTfs = new List<TfsItem>();
            //string branch = ListComponent.Branch;
            List<DischangePath> DischangePathList = new List<DischangePath>();
            List<string> PathGU = new List<string>();
            if (changesetList.Count > 0)
            {
                try
                {
                    ListComponentStatus = $"Leyendo Changeset";
                    foreach (DischangeChangeset item in changesetList)
                    {
                        List<TfsItem> TodosItmTfs2 = await TFSRequest.GetChangeset(item.Changeset);

                        //FUNCION PARA EXTRAER EL NOMBRE COMPLETO DE LOS BRANCHS A UTILIZAR
                        ExtraerBranch(TodosItmTfs2, item.Branch);

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
                    }
                }
                catch (Exception e)
                {
                    Log4net.log.Error(e.Message);
                    string exc = e.Message;
                    ListComponentStatus = $"Exception:{exc}";
                }

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
                                        valueString = UtilHelper.extraerBranchTfs(item.path, '/', ext);
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
                                        if (valueString == "mpm.seg.Customers.Workflow")
                                        {
                                            valueString += ".BSM.dll";
                                        }
                                        if (valueString == "mpm.seg.Customers.DataRecovers")
                                        {
                                            valueString += ".dll";
                                        }
                                        //guia de ubicaciones
                                        DischangePathList = DatabaseHelper.Read<DischangePath>().Where(n => n.Path.Contains(valueString)).ToList();

                                        for (int i = 0; i < DischangePathList.Count; i++)
                                        {
                                            if (ext == ".config")
                                            {
                                                foreach (var item2 in listaBranchs)
                                                {
                                                    string cad = UtilHelper.extraerBranchTfs(DischangePathList[i].Path, '/', ext, item2);
                                                    PathGU.Add(cad + DischangePathList[i].Path);
                                                }
                                            }
                                            else
                                            {
                                                PathGU.Add(DischangePathList[i].Path);
                                            }

                                        }

                                    }

                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log4net.log.Error(ex.Message);
                    string exc = ex.Message;
                    ListComponentStatus = $"Exception:{exc}";
                }
            }

            string rutaCont = ListComponent.Path + "\\";
            if (PathGU.Count > 0)
            {
                ListComponentStatus = $"Transfiriendo archivos de Jenkins.";

                foreach (var branch in listaBranchs)
                {
                    //voy agregar el directorio donde se va a agregar el paquete del Jenkins
                    rutaF = rutaCont + $@"{branch}_{DateTime.Now.ToString("yyyyMMddHHmmss")}\";

                    //CREAR ESTRUCTURA DE ARCHIVOS
                    UtilHelper.buildStructure(rutaF);

                    try
                    {
                        foreach (string itemPathGU in PathGU)
                        {
                            result = TransferFileJenkinsHelper.JenkinsTransferFile(itemPathGU, rutaF, branch);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log4net.log.Error(ex.Message);
                        MessageBox.Show("Error al ejecutar transferencia de paquetes Jenkins: " + ex.Message, "Error al ejecutar transferencia de paquetes Jenkins", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    SortPackDischange.SortPack(@rutaF);
                }


                ListComponentStatus = $"Transferencia de archivos culminado.";
                MessageBox.Show("Transferencia de archivos culminado. ", "Transferencia de archivos culminado.", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ListComponentStatus = $"No hay archivos para transferir.";
            }
        }

        private void ExtraerBranch(List<TfsItem> todosItmTfs2, string branch)
        {
            List<string> valBranch = new List<string>();
            foreach (TfsItem item2 in todosItmTfs2)
            {
                valBranch = item2.path.Split('/').ToList();
                foreach (string item3 in valBranch)
                {
                    if (item3.Contains(branch))
                    {
                        if (listaBranchs.Count == 0)
                        {
                            listaBranchs.Add(item3);
                            return;
                        }
                        else
                        {
                            if (!listaBranchs.Contains(item3))
                            {
                                listaBranchs.Add(item3);
                                return;
                            }
                        }
                    }
                }
            }
            return;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
