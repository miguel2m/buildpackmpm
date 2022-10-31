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
        public ObservableCollection<DischangePath> ComponentList { get; set; }
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

        public List<BranchUse> _branchUses = new List<BranchUse>();
        public ListComponentVM()
        {
            ListComponentCommand = new ListComponentCommand(this);
            ListComponentObjs = new ObservableCollection<ListComponent>();
            CuatroUno = true;
            ListComponent = new ListComponent();
            PathComponent =Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        }

        public void LoadBranch()
        {
            bool _hogar1_branch = Hogar1;
            bool _cuatroUno_branch = CuatroUno;
            bool _prod_branch = Prod;
            bool _hogar2_branch = Hogar2;
            
            //MEJORAR ESTO
            if (_hogar1_branch)
            {
                BranchUse branchUses1 = new BranchUse();
                branchUses1.UseBranch = _hogar1_branch;
                branchUses1.NameBranch = "20211118_HOG";
                _branchUses.Add(branchUses1);
            }
            if (_cuatroUno_branch)
            {
                BranchUse branchUses2 = new BranchUse();
                branchUses2.UseBranch = _cuatroUno_branch;
                branchUses2.NameBranch = "20211118_P4.1";
                _branchUses.Add(branchUses2);
            }
            if (_prod_branch)
            {
                BranchUse branchUses3 = new BranchUse();
                branchUses3.UseBranch = _prod_branch;
                branchUses3.NameBranch = "20220222_PRO";
                _branchUses.Add(branchUses3);
            }
            if (_hogar2_branch)
            {
                BranchUse branchUses4 = new BranchUse();
                branchUses4.UseBranch = _hogar2_branch;
                branchUses4.NameBranch = "202211118_HOG";
                _branchUses.Add(branchUses4);
            }
        }

        public async void copyToJenkins()
        {
            string result = string.Empty, rutaPack = string.Empty, rutaF = string.Empty, branchFound = string.Empty;
            List<TfsItem> TodosItemTfs = new List<TfsItem>();
            List<DischangePath> DischangePathList = new List<DischangePath>();
            List<ListComponent> PathGU = new List<ListComponent>();

            //CARGAMOS LA LISTA DE BRANCHES SELECCIONADAS
            LoadBranch();

            //VERIFICAMOS QUE SE HAYAN SELECCIONADO LOS BRANCHES
            if (_branchUses.Count > 0)
            {
                //VERIFICAMOS QUE EXSITA UNA LISTA DE CHANGESET
                ListComponentStatus = $"Leyendo Changeset";
                List<DischangeChangeset> changesetList = DatabaseHelper.Read<DischangeChangeset>().OrderBy(x => x.Branch).ToList();
                if (changesetList.Count > 0)
                {
                    try
                    {
                        //VERIFIQUEMOS QUE LOS BRANCHES SELECCIONADOS SE ENCUENTREN ASOCIADOS A LOS CHANGESET

                        foreach (DischangeChangeset item in changesetList)
                        {
                            ListComponentStatus = $"Comparando ramas seleccionadas con la lista de changeset";
                            branchFound = BranchSeleccionado(_branchUses, item);
                            if (branchFound.Contains("Error"))
                            {
                                MessageBox.Show(branchFound);
                                return;
                            }
                            else
                            {
                                if (!listaBranchs.Contains(branchFound))
                                {
                                    listaBranchs.Add(branchFound);
                                }
                            }

                            //LO DEL TFS //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            List<TfsItem> TodosItmTfs2 = await TFSRequest.GetChangeset(item.Changeset);

                            TodosItmTfs2 = TodosItmTfs2.FindAll((item2) => item2.path.Contains(branchFound));

                            if (TodosItmTfs2.FirstOrDefault() != null)
                            {
                                foreach (TfsItem itemLocal in TodosItmTfs2)
                                {
                                    if (itemLocal.path.Contains(branchFound))
                                    {
                                        TodosItemTfs.Add(itemLocal);
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
                            string valueString = String.Empty, ext = string.Empty, nameFile2 = String.Empty, nameBranch2 = string.Empty, cad = string.Empty;
                            foreach (TfsItem item in TodosItemTfs)
                            {
                                ext = Path.GetExtension(item.path);

                                list = item.path.Split('.').ToList();
                                if (ext != ".csproj")
                                {
                                    if (ext == ".cs")
                                    {
                                        List<string> listValue = UtilHelper.fileList(item.path, '/');
                                        if (listValue.Where(n => n.Contains("mpm.seg")).Count() > 0)
                                        {
                                            valueString = listValue.First(i => i.Contains("mpm.seg"));
                                        }
                                        else
                                        {
                                            valueString = UtilHelper.nameFile(item.path, '/');
                                        }
                                    }
                                    else
                                    {
                                        if (ext == ".sql")
                                        {
                                            valueString = UtilHelper.extraerBranchTfs(item.path, '/', ext);
                                            List<string> listValue = UtilHelper.fileList(item.path, '/');
                                            nameFile2 = listValue.First(i => i.Contains(".sql"));
                                            valueString += $"\\{nameFile2}";
                                        }
                                        else
                                        {
                                            if (item.path.Contains("/Configuracion/Procesos/"))///Configuracion/Procesos/
                                            {
                                                valueString = "\\ProcesosFull\\" + UtilHelper.nameFile(item.path, '/');
                                            }
                                            else
                                            {
                                                valueString = UtilHelper.nameFile(item.path, '/');
                                            }
                                        }
                                    }
                                    if (valueString != null)
                                    {
                                        if (ext == ".sql")
                                        {
                                            foreach (BranchUse item2 in _branchUses)
                                            {
                                                if (item.path.Contains(item2.NameBranch))
                                                {
                                                    ExtraerBranchTFS2(valueString, item);
                                                }
                                            }
                                            if (!PathGU.Contains(ListComponent))
                                            {
                                                PathGU.Add(ListComponent);
                                            }
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
                                            //DischangePathList = DatabaseHelper.Read<DischangePath>().Where(n => n.Path.Contains(valueString)).ToList();
                                            DischangePathList = DatabaseHelper.Read<DischangePath>().Where(n => n.Path.Contains(valueString) && !n.Path.Contains("Upgrade")).ToList();

                                            //QUIERO EVALUAR SI TIENE LAS CARPETAS COMPLETAS
                                            if (DischangePathList.Where(n => n.Path.Contains("\\Configurables\\")).Count() > 0 && DischangePathList.Where(n => n.Path.Contains("\\Configurables\\")).Count() < 4)//n.Path.Contains("appCustomerDataRecoverSettingsAhorro.config") &&  
                                            {
                                                if ((DischangePathList.Where(n => n.Path.Contains("\\Configurables\\Des")).Count() == 0) || (DischangePathList.Where(n => n.Path.Contains("\\Configurables\\Desa")).Count() == 0))
                                                {
                                                    DischangePath dischangePath = new DischangePath();
                                                    string rutaConf = "\\Configurables\\Desa\\DIS\\ecDataProvider\\CustomerSettings\\appCustomerDataRecoverSettingsAhorro.config";
                                                    foreach (BranchUse item2 in _branchUses)
                                                    {
                                                        if (item.path.Contains(item2.NameBranch))
                                                        {
                                                            cad = UtilHelper.extraerBranchTfs(rutaConf, '/', ext, item2.NameBranch);
                                                            cad += rutaConf;
                                                            ExtraerBranchTFS2(cad, item);

                                                        }
                                                    }
                                                    if (!PathGU.Contains(ListComponent))
                                                    {
                                                        PathGU.Add(ListComponent);
                                                    }
                                                }
                                            }

                                            for (int i = 0; i < DischangePathList.Count; i++)
                                            {
                                                if (DischangePathList[i].Path.Contains("custom-context.xml"))
                                                {
                                                    if (!DischangePathList[i].Path.Contains("Configurables") &&
                                                        !DischangePathList[i].Path.Contains("ecDataProvider") &&
                                                        !DischangePathList[i].Path.Contains("BSM"))
                                                    {
                                                        ExtraerBranchTFS2(DischangePathList[i].Path, item);
                                                        if (!PathGU.Contains(ListComponent))
                                                        {
                                                            PathGU.Add(ListComponent);
                                                        }
                                                    }
                                                }
                                                else if (ext == ".config")
                                                {
                                                    //ExtraerBranchTFS2(DischangePathList[i].Path, _branchUses, item);
                                                    foreach (var item2 in _branchUses)
                                                    {
                                                        if (item.path.Contains(item2.NameBranch))
                                                        {
                                                            cad = UtilHelper.extraerBranchTfs(DischangePathList[i].Path, '/', ext, item2.NameBranch);
                                                            cad += DischangePathList[i].Path;
                                                            if (cad.Contains("Des"))
                                                            {
                                                                cad = cad.Replace("Des", "DESA");
                                                            }
                                                            else if (cad.Contains("Cer"))
                                                            {
                                                                cad = cad.Replace("Cer", "CERT");
                                                            }
                                                            else if (cad.Contains("Pre"))
                                                            {
                                                                cad = cad.Replace("Pre", "PRE");
                                                            }
                                                            else if (cad.Contains("Pro"))
                                                            {
                                                                cad = cad.Replace("Pro", "PRO");
                                                            }
                                                            ExtraerBranchTFS2(cad, item);

                                                        }
                                                    }
                                                    if (!PathGU.Contains(ListComponent))
                                                    {
                                                        PathGU.Add(ListComponent);
                                                    }
                                                }
                                                else
                                                {
                                                    ExtraerBranchTFS2(DischangePathList[i].Path, item);
                                                    if (!PathGU.Contains(ListComponent))
                                                    {
                                                        PathGU.Add(ListComponent);
                                                    }
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

                string rutaCont = PathComponent + "\\";
                if (PathGU.Count > 0)
                {
                    ListComponentStatus = $"Transfiriendo archivos de Jenkins.";

                    foreach (var pathFound in PathGU)
                    {
                        //voy agregar el directorio donde se va a agregar el paquete del Jenkins
                        rutaF = $@"{rutaCont}{pathFound.Branch}_{DateTime.Now.ToString("yyyyMMddHHmmss")}";

                        try
                        {
                            //CREAR ESTRUCTURA DE ARCHIVOS
                            UtilHelper.buildStructure(rutaF);

                            result = TransferFileJenkinsHelper.JenkinsTransferFile(pathFound.Path, rutaF, pathFound.Branch);
                        }
                        catch (Exception ex)
                        {
                            Log4net.log.Error(ex.Message);
                            MessageBox.Show("Error al ejecutar transferencia de paquetes Jenkins: " + ex.Message, "Error al ejecutar transferencia de paquetes Jenkins", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        //SortPackDischange.SortPack(@rutaF);
                    }


                    ListComponentStatus = $"Transferencia de archivos culminado.";
                    MessageBox.Show("Transferencia de archivos culminado. ", "Transferencia de archivos culminado.", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ListComponentStatus = $"No hay archivos para transferir.";
                }

            }
            else
            {
                MessageBox.Show("Debe seleccionar al menos un branch para continuar");
            }
        }

        private void ExtraerBranchTFS2(string rutaConf, TfsItem item)
        {
            ListComponent = new ListComponent();
            ListComponent.Path = rutaConf;
            foreach (BranchUse item2 in _branchUses)
            {
                if (item2.UseBranch)
                {
                    if (item.path.Contains(item2.NameBranch))
                    {
                        ListComponent.Branch = item2.NameBranch;
                    }
                }
            }
        }


        public string BranchSeleccionado(List<BranchUse> branchUses, DischangeChangeset item)
        {
            string resultado = string.Empty;
            if (_branchUses.Count > 0)
            {
                //NECESITO VERIFICAR QUE SE HAYA SELECCIONADO LOS BRANCH CORRECTOS
                foreach (BranchUse branchUse in _branchUses)
                {
                    //BUSCO LO QUE FUE SELECCIONADO
                    if (branchUse.NameBranch.Contains(item.Branch))
                    {
                        return resultado = branchUse.NameBranch;
                    }
                }
                return "Error: Seleccione el branch asociado al changeset para generar el paquete";
            }
            else
            {
                return "Error: No fue seleccionado ningun branch para generar el paquete";
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
