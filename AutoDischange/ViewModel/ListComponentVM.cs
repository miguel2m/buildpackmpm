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

        public List<TfsItem> TodosItemTfs = new List<TfsItem>();
        public List<ListComponent> PathGU = new List<ListComponent>();

        public FilesPacksToUpdates filesPacksToUpdates;
        public List<FilesPacksToUpdates> FilesPacksTos = new List<FilesPacksToUpdates>();
        public List<FilesPacksToUpdates> FilesPacksTos2 = new List<FilesPacksToUpdates>();

        public List<BranchJenkins> BranchJenkins = new List<BranchJenkins>();

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


            PathComponent = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            List<BranchJenkinsExcel> changesetList = DatabaseHelper.Read<BranchJenkinsExcel>()
                .OrderBy(x => x.Name).ToList();

        }

        public void LoadBranch()
        {
            _branchUses.Clear();
            List<BranchJenkinsExcel> BranchesList = DatabaseHelper.Read<BranchJenkinsExcel>()
                .OrderBy(x => x.Name).ToList();
            foreach (BranchJenkinsExcel item in BranchesList)
            {
                _branchUses.Add(new BranchUse()
                {
                    NameBranch = item.Name,
                    UseBranch = true
                });
            }

            //bool _hogar1_branch = Hogar1;
            //bool _cuatroUno_branch = CuatroUno;
            //bool _prod_branch = Prod;
            //bool _hogar2_branch = Hogar2;

            ////MEJORAR ESTO
            //if (_hogar1_branch)
            //{
            //    BranchUse branchUses1 = new BranchUse();
            //    branchUses1.UseBranch = _hogar1_branch;
            //    branchUses1.NameBranch = "20211118_HOG";
            //    _branchUses.Add(branchUses1);
            //}
            //if (_cuatroUno_branch)
            //{
            //    BranchUse branchUses2 = new BranchUse();
            //    branchUses2.UseBranch = _cuatroUno_branch;
            //    branchUses2.NameBranch = "20211118_P4.1";
            //    _branchUses.Add(branchUses2);
            //}
            //if (_prod_branch)
            //{
            //    BranchUse branchUses3 = new BranchUse();
            //    branchUses3.UseBranch = _prod_branch;
            //    branchUses3.NameBranch = "20220222_PRO";
            //    _branchUses.Add(branchUses3);
            //}
            //if (_hogar2_branch)
            //{
            //    BranchUse branchUses4 = new BranchUse();
            //    branchUses4.UseBranch = _hogar2_branch;
            //    branchUses4.NameBranch = "202211118_HOG";
            //    _branchUses.Add(branchUses4);
            //}
        }

        public async Task CopyToJenkinsAsync()
        {
            //VARIABLES
            string result = string.Empty, rutaPack = string.Empty, rutaF = string.Empty, branchFound = string.Empty;
            string valueString = String.Empty, ext = string.Empty, nameFile2 = String.Empty, nameBranch2 = string.Empty, cad = string.Empty;
            List<DischangePath> DischangePathList = new List<DischangePath>();
            List<TfsItem> TodosItmTfs2 = new List<TfsItem>();
            PathGU.Clear();
            FilesPacksTos.Clear();
            TodosItemTfs.Clear();
            TodosItmTfs2.Clear();
            try
            {
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
                        //VERIFIQUEMOS QUE LOS BRANCHES SELECCIONADOS SE ENCUENTREN ASOCIADOS A LOS CHANGESET

                        ////////////////////// LISTA DE CHANGESET ///////////////////////////////////////////////////////////////
                        foreach (DischangeChangeset item in changesetList)
                        {
                            ListComponentStatus = $"Comparando ramas seleccionadas con la lista de changeset";
                            branchFound = BranchSeleccionado(_branchUses, item);
                            if (branchFound.Contains("Error"))
                            {
                                ListComponentStatus = branchFound;
                                return;
                            }
                            else
                            {
                                if (!listaBranchs.Contains(branchFound))
                                {
                                    listaBranchs.Add(branchFound);
                                }
                                ////////////////////////////////////////// TFS //////////////////////////////////////////////
                                TodosItmTfs2 = await TFSRequest.GetChangeset(item.Changeset);
                                CargarTodosItemTfs(TodosItmTfs2, branchFound);
                            }
                        }

                        ///////////////////// GUIA DE UBICACIONES ///////////////////////////////////////////////////////////////

                        if (TodosItemTfs.Where(n => n.path.Contains(".csproj")).Count() > 0)
                        {
                            for (int i = 0; i < TodosItemTfs.Count(); i++)
                            {
                                if (TodosItemTfs[i].path.Contains(".csproj"))
                                {
                                    TodosItemTfs.RemoveAt(i);
                                }
                            }
                        }

                        if (TodosItemTfs.Count > 0)
                        {
                            ListComponentStatus = $"Cargando datos de TFS.";
                            List<string> list = new List<string>();
                            foreach (TfsItem item in TodosItemTfs)
                            {
                                ext = Path.GetExtension(item.path);
                                list = item.path.Split('.').ToList();
                                if (ext != ".csproj")
                                {
                                    valueString = ObtenerValueString(ext, item.path, valueString, nameFile2);

                                    if (valueString != null)
                                    {
                                        CargarPathGU(ext, _branchUses, ListComponent, valueString, item, DischangePathList, cad);
                                    }
                                }
                            }
                        }
                    }

                    FilesPacksTos.Clear();
                    ///////////////////////// CARGAMOS EN EL JENKINS /////////////////////////////////////////////////////////////
                    string rutaCont = PathComponent + "\\";
                    List<string> rutaCont2 = new List<string>();
                    if (PathGU.Count > 0)
                    {
                        ListComponentStatus = $"Transfiriendo archivos de Jenkins.";
                        foreach (var pathFound in PathGU)
                        {
                            if (!rutaF.Contains(pathFound.Branch))
                            {
                                //voy agregar el directorio donde se va a agregar el paquete del Jenkins
                                rutaF = $@"{rutaCont}{pathFound.Branch}_{DateTime.Now.ToString("yyMdHmss")}";
                                rutaCont2.Add(rutaF);
                            }

                            try
                            {
                                FilesPacksTos = TransferFileJenkinsHelper.JenkinsTransferFile(pathFound.Path, rutaF, pathFound.Branch, pathFound.changeset);
                            }
                            catch (Exception ex)
                            {
                                Log4net.log.Error(ex.Message);
                            }
                        }

                        if (FilesPacksTos.Count > 0)
                        {
                            HomologarArchivos();

                            //METODO PARA LA HOMOLOGACION DE LOS ARCHIVOS
                            foreach (var item in _branchUses)
                            {
                                FilesPacksTos2 = FilesPacksTos.Where(n => n.branchUse == item.NameBranch).ToList();

                                if (FilesPacksTos2.Count > 0)
                                {
                                    string rutaH = rutaCont2.Where(n => n.Contains(item.NameBranch)).FirstOrDefault();
                                    DetallarResultadoPack(rutaH);
                                    FilesPacksTos2.Clear();
                                }
                            }

                            FilesPacksTos.Clear();
                            ListComponentStatus = $"Transferencia de archivos culminado.";
                            Log4net.log.Info($"Transferencia de archivos culminado.");
                            MessageBox.Show("Transferencia de archivos culminado. ", "Transferencia de archivos culminado.", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            foreach (ListComponent item in PathGU)
                            {
                                Log4net.log.Warn($"Warn: Los componentes no fueron cargados: {item.Branch}, {item.changeset}, {item.Path}");
                            }
                            ListComponentStatus = $"Warn: Los componentes no fueron cargados.";
                        }
                    }
                    else
                    {
                        ListComponentStatus = $"No hay archivos para transferir.";
                        Log4net.log.Warn($"No hay archivos para transferir");
                    }
                }
                else
                {
                    ListComponentStatus = "Debe seleccionar al menos un branch para continuar";
                    Log4net.log.Warn($"Debe seleccionar al menos un branch para continuar");
                }
            }
            catch (Exception e)
            {
                Log4net.log.Error(e.Message);
                string exc = e.Message;
                ListComponentStatus = $"Exception:{exc}";
            }
        }

        private void DetallarResultadoPack(string rutaF)
        {
            //Random random = new Random();
            string pathend = $@"{rutaF}\Resultado_Pack_Creado.csv";
            string separador = ",";
            StringBuilder salida = new StringBuilder();
            foreach (FilesPacksToUpdates item in FilesPacksTos2)
            {
                string cadena = $@"{item.branchUse},{item.changeset},{item.nameFile},{item.pathFile.Replace(rutaF, "")}";
                _ = salida.AppendLine(string.Join(separador, cadena));
            }
            File.AppendAllText(pathend, salida.ToString());
            salida.Clear();
        }

        private void HomologarArchivos()
        {
            IEnumerable<IGrouping<string, FilesPacksToUpdates>> GroupedByFiles = FilesPacksTos.GroupBy(user => user.nameFile);
            foreach (IGrouping<string, FilesPacksToUpdates> nameFileKey in GroupedByFiles)
            {
                int cantFiles = FilesPacksTos.Where(x => x.nameFile == nameFileKey.Key).Count();
                if (cantFiles > 1)
                {
                    FilesPacksToUpdates mostCurrentFile = FilesPacksTos.Where(x => x.nameFile == nameFileKey.Key).OrderByDescending(y => y.dateTimeFile).FirstOrDefault();

                    //ruta inicial
                    string mostCurrentPathFile = mostCurrentFile.pathFile;
                    DateTime mostCurrentDateTimeFile = mostCurrentFile.dateTimeFile;
                    foreach (FilesPacksToUpdates item in nameFileKey)
                    {
                        if (mostCurrentPathFile != item.pathFile && mostCurrentDateTimeFile != item.dateTimeFile)
                        {
                            if (File.Exists(item.pathFile))
                            {
                                File.SetAttributes(item.pathFile, FileAttributes.Normal);
                                File.Delete(item.pathFile);
                            }
                            File.Copy(mostCurrentPathFile, item.pathFile);
                        }
                    }
                }
            }
        }

        private void CargarPathGU(string ext, List<BranchUse> branchUses, ListComponent listComponent, string valueString, TfsItem item, List<DischangePath> dischangePathList, string cad)
        {
            DischangePath dischangePath = new DischangePath();
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
                dischangePathList = DatabaseHelper.Read<DischangePath>().Where(n => n.Path.Contains(valueString) && !n.Path.Contains("\\DIS\\Upgrade\\")).ToList();
                string rutaConf = string.Empty;

                // TENGO UN ARCHIVO EN EL TFS PERO NO TENGO LA RUTA EN LA GUIA DE UBICACIONES
                NoGuiaUbicaciones(dischangePathList, valueString, item, rutaConf, _branchUses, cad, ext);

                //QUIERO EVALUAR SI LA CARPETA CONFIGURABLE ESTA COMPLETA EN LOS 4 AMBIENTES
                if (dischangePathList.Where(n => n.Path.Contains("\\Configurables\\Cer")).Count() > 0 &&
                    dischangePathList.Where(n => n.Path.Contains("\\Configurables\\Pre")).Count() > 0 &&
                    dischangePathList.Where(n => n.Path.Contains("\\Configurables\\Pro")).Count() > 0 &&
                    (dischangePathList.Where(n => n.Path.Contains("\\Configurables\\Des")).Count() == 0 || dischangePathList.Where(n => n.Path.Contains("\\Configurables\\Desa")).Count() == 0))
                {
                    foreach (var item2 in dischangePathList)
                    {
                        if (!item2.Path.Contains("custom-context.xml") && !item2.Path.Contains("customer-operation-services.xml"))
                        {
                            if (!item2.Path.Contains("Alojables"))
                            {
                                if (item2.Path.Contains("Cer"))
                                {
                                    rutaConf = item2.Path;
                                    rutaConf = rutaConf.Replace("Cer", "Desa");
                                }
                            }
                        }
                    }
                    if (rutaConf != "")
                    {
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

                for (int i = 0; i < dischangePathList.Count; i++)
                {
                    if (dischangePathList[i].Path.Contains("custom-context.xml"))
                    {
                        if (!dischangePathList[i].Path.Contains("Configurables") && !dischangePathList[i].Path.Contains("BSM"))
                        {
                            if (item.path.Contains("mpm.seg.Customers.eClient.Web") && dischangePathList[i].Path.Contains(@"eClient\CustomerSettings"))
                            {
                                ExtraerBranchTFS2(dischangePathList[i].Path, item);
                            }
                            else if (item.path.Contains("mpm.seg.Customers.DataRecovers") && dischangePathList[i].Path.Contains(@"ecDataProvider\CustomerSettings"))
                            {
                                ExtraerBranchTFS2(dischangePathList[i].Path, item);
                            }
                            if (!PathGU.Contains(ListComponent))
                            {
                                PathGU.Add(ListComponent);
                            }
                        }
                    }
                    else if (dischangePathList[i].Path.Contains("customer-operation-services.xml") && dischangePathList[i].Path.Contains("Alojables"))
                    {
                        ExtraerBranchTFS2(dischangePathList[i].Path, item);
                        if (!PathGU.Contains(ListComponent))
                        {
                            PathGU.Add(ListComponent);
                        }
                    }
                    else if (ext == ".config" || ext == ".cmd")
                    {
                        foreach (var item2 in _branchUses)
                        {
                            if (item.path.Contains(item2.NameBranch))
                            {
                                cad = UtilHelper.extraerBranchTfs(dischangePathList[i].Path, '/', ext, item2.NameBranch);
                                cad += dischangePathList[i].Path;
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
                    else if (ext == ".js" && dischangePathList.Count > 1)
                    {
                        ObtenerSoloModificado(dischangePathList[i].Path, item);
                        if (!PathGU.Contains(ListComponent))
                        {
                            PathGU.Add(ListComponent);
                        }
                    }
                    else
                    {
                        ExtraerBranchTFS2(dischangePathList[i].Path, item);
                        if (!PathGU.Contains(ListComponent))
                        {
                            PathGU.Add(ListComponent);
                        }
                    }
                }
            }
        }

        private void ObtenerSoloModificado(string path1, TfsItem item)
        {
            //EXTRAIGO UNA PARTE DE LA RUTA 
            string cutPath = path1.Replace(@"\Alojables\DIS\eClient\", "").Replace(@"\", "/");
            if (item.path.Contains(cutPath))
            {
                ExtraerBranchTFS2(path1, item);
            }
        }

        private void NoGuiaUbicaciones(List<DischangePath> dischangePathList, string valueString, TfsItem item, string rutaConf, List<BranchUse> branchUses, string cad, string ext)
        {
            if (dischangePathList.Count() == 0 && !item.path.Contains("BSM Robot"))
            {
                if (valueString.Count() > 0)
                {
                    if (item.path.Contains("/Mappings/General/"))
                    {
                        rutaConf = $@"\Batch\Mappings\Partials\General\{valueString}";
                    }
                    if (rutaConf != null)
                    {
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
            }
            else
            {
                if (item.path.Contains("BSM Robot"))
                {
                    Log4net.log.Error($@"No es parte de los componentes --> {item.path}");
                }
                if (valueString == "mpm.seg.Customers.DataRecovers.dll" || valueString == "mpm.seg.Customers.DataRecovers.Ahorro.dll")
                {
                    if (dischangePathList.Where(n => n.Path.Contains("\\CalculusServices\\")).Count() == 0)
                    {
                        rutaConf = $@"\CalculusServices\bin\{valueString}";
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
            }
        }

        private string ObtenerValueString(string ext, string pathTfs, string valueString, string nameFile2)
        {
            if (ext == ".cs")
            {
                List<string> listValue = UtilHelper.fileList(pathTfs, '/');
                if (listValue.Where(n => n.Contains("mpm.seg")).Count() > 0)
                {
                    valueString = listValue.First(i => i.Contains("mpm.seg"));
                    if (valueString.Contains("Customers.Workflow") ||
                        valueString.Contains("mpm.seg.Customers.Constants") ||
                        valueString.Contains("mpm.seg.Customers.PageRender"))
                    {
                        valueString = $"{valueString}.BSM.dll";
                    }
                    else
                    {
                        valueString += ".dll";
                    }
                }
                else if (listValue.Where(n => n.Contains("mpm.eClient")).Count() > 0)
                {
                    valueString = listValue.First(i => i.Contains("mpm.eClient")) + ".dll";
                    valueString = valueString.Contains("mpm.eClient.ecPortal.Web.dll") ? "mpm.eClient.Web.dll" : valueString;
                }
                else
                {
                    valueString = UtilHelper.nameFile(pathTfs, '/');
                }
            }
            else
            {
                if (ext == ".sql")
                {
                    valueString = UtilHelper.extraerBranchTfs(pathTfs, '/', ext);
                    List<string> listValue = UtilHelper.fileList(pathTfs, '/');
                    nameFile2 = listValue.First(i => i.Contains(".sql"));
                    valueString += $"\\{nameFile2}";
                }
                else
                {
                    if (pathTfs.Contains("/Configuracion/Procesos/"))///Configuracion/Procesos/
                    {
                        valueString = "\\ProcesosFull\\" + UtilHelper.nameFile(pathTfs, '/');
                    }
                    else
                    {
                        valueString = UtilHelper.nameFile(pathTfs, '/');
                    }
                }
            }
            return valueString;
        }

        private void CargarTodosItemTfs(List<TfsItem> todosItmTfs2, string branchFound)
        {
            todosItmTfs2 = todosItmTfs2.FindAll((item2) => item2.path.Contains(branchFound));
            if (todosItmTfs2.FirstOrDefault() != null)
            {
                foreach (TfsItem itemLocal in todosItmTfs2)
                {
                    if (itemLocal.path.Contains(branchFound))
                    {
                        TodosItemTfs.Add(itemLocal);
                    }
                }
            }
        }


        private async Task<List<TfsItem>> CargarTodosItemTfs2Async(string changeset, string branchFound)
        {
            List<TfsItem> TodosItmTfs2 = await TFSRequest.GetChangeset(changeset);
            List<TfsItem> TodosItemTfs3 = new List<TfsItem>();

            TodosItmTfs2 = TodosItmTfs2.FindAll((item2) => item2.path.Contains(branchFound));

            if (TodosItmTfs2.FirstOrDefault() != null)
            {
                foreach (TfsItem itemLocal in TodosItmTfs2)
                {
                    if (itemLocal.path.Contains(branchFound))
                    {
                        TodosItemTfs3.Add(itemLocal);
                    }
                }
            }
            return TodosItemTfs3;
        }

        private void ExtraerBranchTFS2(string rutaConf, TfsItem item)
        {
            ListComponent = new ListComponent();
            ListComponent.Path = rutaConf;
            ListComponent.changeset = item.version.ToString();
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
