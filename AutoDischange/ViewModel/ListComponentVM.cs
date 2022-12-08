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
        public List<ListComponent> PathGU2 = new List<ListComponent>();

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
                                Log4net.log.Error(ListComponentStatus);
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
                            TodosItemTfs.Sort(delegate (TfsItem x, TfsItem y)
                            {
                                return x.version.CompareTo(y.version);
                            });
                            foreach (TfsItem item in TodosItemTfs)
                            {
                                if (item.hashValue == "xq4u8sRBez42CbBs1Yg15g==")
                                {
                                    int a = 0;
                                }
                                string findTfsBranch = BranchSeleccionadoTfs(_branchUses, item);
                                if (findTfsBranch.Contains("Error"))
                                {
                                    ListComponentStatus = findTfsBranch;
                                    Log4net.log.Error(ListComponentStatus);
                                    return;
                                }
                                else
                                {
                                    if (!item.path.Contains(@"/BSM Robot/"))
                                    {
                                        if (!item.changetype.Contains("delete, merge"))
                                        {
                                            ext = Path.GetExtension(item.path);
                                            list = item.path.Split('.').ToList();
                                            if (ext != ".csproj")
                                            {
                                                valueString = ObtenerValueString(ext, item.path, valueString, nameFile2);

                                                if (valueString != null)
                                                {
                                                    CargarPathGU(ext, findTfsBranch, ListComponent, valueString, item, DischangePathList, cad);
                                                }
                                            }
                                        }
                                    }                                    
                                }
                            }
                        }
                    }

                    FilesPacksTos.Clear();
                    ///////////////////////// CARGAMOS EN EL JENKINS /////////////////////////////////////////////////////////////
                    string rutaCont = PathComponent + "\\";
                    List<string> rutaCont2 = new List<string>();

                    PathGU.Sort(delegate (ListComponent x, ListComponent y)
                    {
                        return x.changeset.CompareTo(y.changeset);
                    });

                    _branchUses.Sort(delegate (BranchUse x, BranchUse y)
                    {
                        return x.NameBranch.CompareTo(y.NameBranch);
                    });

                    foreach (BranchUse bbranch in _branchUses)
                    {
                        PathGU2 = PathGU.Where(n => n.Branch == bbranch.NameBranch).ToList();
                        if (PathGU2.Count > 0)
                        {
                            ListComponentStatus = $"Transfiriendo archivos de Jenkins, para la rama {bbranch.NameBranch}.";
                            foreach (ListComponent pathFound in PathGU2)
                            {
                                if (!rutaF.Contains(pathFound.Branch))
                                {
                                    //voy agregar el directorio donde se va a agregar el paquete del Jenkins
                                    rutaF = $@"{rutaCont}{pathFound.Branch}_{DateTime.Now.ToString("yyMMddHHmmss")}";
                                    rutaCont2.Add(rutaF);
                                }

                                try
                                {
                                    FilesPacksTos = TransferFileJenkinsHelper.JenkinsTransferFile(pathFound.Path, rutaF, pathFound.Branch, pathFound.changeset);
                                }
                                catch (Exception ex)
                                {
                                    string chgst = "";
                                    if (pathFound.changeset.Length > 0)
                                    {
                                        chgst = pathFound.changeset;
                                    }
                                    Log4net.log.Error($"{chgst} - {ex.Message}" );
                                }
                            }

                            if (FilesPacksTos.Count > 0)
                            {
                                //METODO PARA GENERAR EL EXCEL
                                string rutaH = rutaCont2.FirstOrDefault(n => n.Contains(bbranch.NameBranch));
                                DetallarResultadoPack(rutaH);

                                //METODO PARA LA HOMOLOGACION DE LOS ARCHIVOS
                                HomologarArchivos();

                                FilesPacksTos.Clear();
                            }
                            else
                            {
                                foreach (ListComponent item in PathGU)
                                {
                                    Log4net.log.Error($"Error: Los componentes no fueron incluidos en el paquete: {item.Branch}, {item.changeset}, {item.Path}");
                                }
                            }
                        }
                        else
                        {
                            ListComponentStatus = $"No hay archivos para transferir, para la rama {bbranch.NameBranch}.";
                            Log4net.log.Info($"No hay archivos para transferir, para la rama{bbranch.NameBranch}.");
                        }
                    }
                    FilesPacksTos.Clear();
                    ListComponentStatus = $"Transferencia de archivos culminado.";
                    Log4net.log.Info($"Transferencia de archivos culminado.");
                    _ = MessageBox.Show("Transferencia de archivos culminado. ", "Transferencia de archivos culminado.", MessageBoxButton.OK, MessageBoxImage.Information);

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

        private string BranchSeleccionadoTfs(List<BranchUse> branchUses, TfsItem item)
        {
            string resultado;
            if (_branchUses.Count > 0)
            {
                //NECESITO VERIFICAR QUE SE HAYA SELECCIONADO LOS BRANCH CORRECTOS
                foreach (BranchUse branchUse in _branchUses)
                {
                    if (item.path.Contains(branchUse.NameBranch))
                    {
                        resultado = branchUse.NameBranch;
                        return resultado;
                    }
                }
                resultado = $"Error: {item.version} - Seleccione el branch asociado al changeset para generar el paquete";
            }
            else
            {
                resultado = $"Error: {item.version} - No fue seleccionado ningun branch para generar el paquete";
            }

            return resultado;
        }

        public static string nameFile(string url)
        {
            List<string> list = new List<string>();
            list = url.Split('\\').ToList();
            int count = list.Count;
            string result = list[count - 1];

            return result;
        }

        private void DetallarResultadoPack(string rutaF)
        {
            //Random random = new Random();
            string pathend = $@"{rutaF}\Resultado_Pack_Creado.csv";
            string separador = ",";
            StringBuilder salida = new StringBuilder();
            foreach (FilesPacksToUpdates item in FilesPacksTos)
            {
                string cadena = $@"{item.branchUse},{item.changeset},{item.nameFile},{item.pathFile.Replace(rutaF, "")},{item.Confirm}";
                _ = salida.AppendLine(string.Join(separador, cadena));
            }
            File.AppendAllText(pathend, salida.ToString());
            _ = salida.Clear();
        }

        private void HomologarArchivos()
        {
            IEnumerable<IGrouping<string, FilesPacksToUpdates>> GroupedByFiles = FilesPacksTos.GroupBy(user => user.nameFile);
            foreach (IGrouping<string, FilesPacksToUpdates> nameFileKey in GroupedByFiles)
            {
                if (!nameFileKey.Key.Contains(".config"))
                {
                    if (!nameFileKey.Key.Contains(".dll"))
                    {
                        if (!nameFileKey.Key.Contains("custom-context.xml"))
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
                }
            }
        }

        private void CargarPathGU(string ext, string findTfsBranch, ListComponent listComponent, string valueString, TfsItem item, List<DischangePath> dischangePathList, string cad)
        {
            bool flag = false;
            string rutaConf = string.Empty;
            DischangePath dischangePath = new DischangePath();

            //1ro Busco en la Guia de Ubicaciones
            dischangePathList = DatabaseHelper.Read<DischangePath>().Where(n => n.Path.Contains(valueString) && !n.Path.Contains("\\DIS\\Upgrade\\")).ToList();
            if (dischangePathList.Count > 0)
            {
                //CREO LA RUTA PARA LOS COMPONENTES QUE VAN A [DESA] EN LOS CONFIGURABLES EN CASO DE QUE NO EXISTA
                ConfigNoDesa(dischangePathList, rutaConf, item, findTfsBranch, cad, ext);

                for (int i = 0; i < dischangePathList.Count; i++)
                {
                    //ESTE ARCHIVO SOLO PUEDE SER ALOJABLE
                    if (dischangePathList[i].Path.Contains("custom-context.xml"))
                    {
                        if (!dischangePathList[i].Path.Contains("Configurables") && !dischangePathList[i].Path.Contains("BSM"))
                        {
                            if (item.path.Contains("mpm.seg.Customers.eClient.Web") && dischangePathList[i].Path.Contains(@"eClient\CustomerSettings"))
                            {
                                ExtraerBranchTFS2(dischangePathList[i].Path, item, findTfsBranch);
                                flag = true;
                            }
                            else if (item.path.Contains("mpm.seg.Customers.DataRecovers") && dischangePathList[i].Path.Contains(@"ecDataProvider\CustomerSettings"))
                            {
                                ExtraerBranchTFS2(dischangePathList[i].Path, item, findTfsBranch);
                                flag = true;
                            }
                            if (flag)
                            {
                                AddPathGu(findTfsBranch, item.version);
                                flag = false;
                            }
                        }
                    }
                    //ESTE ARCHIVO SOLO PUEDE SER ALOJABLE
                    else if (dischangePathList[i].Path.Contains("customer-operation-services.xml"))
                    {
                        if (dischangePathList[i].Path.Contains("Alojables"))
                        {
                            ExtraerBranchTFS2(dischangePathList[i].Path, item, findTfsBranch);
                            AddPathGu(findTfsBranch, item.version);
                        }
                    }
                    //SOLO CONFIGURABLES Y CMD
                    else if (ext == ".config" || ext == ".cmd")
                    {
                        //SI EL PATH CONTIENE ESTA CONDICION 
                        if (item.path.Contains("mpm.eClient.ecPortal.Web"))
                        {
                            if (item.path.Contains(findTfsBranch))
                            {
                                valueString = $@"\Alojables\DIS\eClient\{UtilHelper.nameFile(item.path, '/')}";
                                ExtraerBranchTFS2(valueString, item, findTfsBranch);
                            }
                        }
                        else
                        {
                            if (item.path.Contains(findTfsBranch))
                            {
                                cad = UtilHelper.extraerBranchTfs(dischangePathList[i].Path, '/', ext, findTfsBranch);
                                cad += dischangePathList[i].Path;
                                //CORRECCION DE NOMBRE DE ARCHIVOS DE LA GUIA DE UBICACION PARA LUEGO BUSCAR EN EL JENKINS
                                cad = CorregirNombrePathConfig(cad);
                                ExtraerBranchTFS2(cad, item, findTfsBranch);
                            }
                        }
                        AddPathGu(findTfsBranch, item.version);
                    }
                    //SI HAY UN JS REPETIDO MAS DE UNA VEZ
                    else if (ext == ".js" && dischangePathList.Count > 1)
                    {
                        flag = ObtenerSoloModificado(dischangePathList[i].Path, item, flag, findTfsBranch);
                        if (flag)
                        {
                            AddPathGu(findTfsBranch, item.version);
                        }
                    }
                    //PARA ESTOS DOS ARCHIVOS EN LA GUIA DE UBICACION NO ESTA LA RUTA DE CALCULOS SERVICES 
                    else if (valueString == "mpm.seg.Customers.DataRecovers.dll" || valueString == "mpm.seg.Customers.DataRecovers.Ahorro.dll")
                    {
                        if (dischangePathList.Where(n => n.Path.Contains("\\CalculusServices\\")).Count() == 0)
                        {
                            rutaConf = $@"\CalculusServices\bin\{valueString}";
                            cad = UtilHelper.extraerBranchTfs(rutaConf, '/', ext, findTfsBranch);
                            cad += rutaConf;
                        }
                        else
                        {
                            cad = dischangePathList[i].Path;
                        }

                        if (item.path.Contains(findTfsBranch))
                        {
                            ExtraerBranchTFS2(cad, item, findTfsBranch);
                        }
                        AddPathGu(findTfsBranch, item.version);
                    }
                    else
                    {
                        ExtraerBranchTFS2(dischangePathList[i].Path, item, findTfsBranch);
                        AddPathGu(findTfsBranch, item.version);
                    }
                }
            }
            else
            {
                if (ext == ".sql")
                {
                    if (item.path.Contains(findTfsBranch))
                    {
                        ExtraerBranchTFS2(valueString, item, findTfsBranch);
                    }
                    AddPathGu(findTfsBranch, item.version);
                }
                else
                {
                    //TENGO UN ARCHIVO EN EL TFS PERO NO ESTA LA RUTA EN LA GUIA DE UBICACIONES
                    NoGuiaUbicaciones(dischangePathList, valueString, item, rutaConf, findTfsBranch, cad, ext);
                }
            }
        }

        private string CorregirNombrePathConfig(string cad)
        {
            if (cad.Contains(@"\Des\"))
            {
                cad = cad.Replace(@"\Des\", @"\DESA\");
            }
            else if (cad.Contains(@"\Desa\"))
            {
                cad = cad.Replace(@"\Desa\", @"\DESA\");
            }
            else if (cad.Contains(@"\Cer\"))
            {
                cad = cad.Replace(@"\Cer\", @"\CERT\");
            }
            else if (cad.Contains(@"\Cert\"))
            {
                cad = cad.Replace(@"\Cert\", @"\CERT\");
            }
            else if (cad.Contains(@"\Pre\"))
            {
                cad = cad.Replace(@"\Pre\", @"\PRE\");
            }
            else if (cad.Contains(@"\Pro\"))
            {
                cad = cad.Replace(@"\Pro\", @"\PRO\");
            }
            return cad;
        }

        private void ConfigNoDesa(List<DischangePath> dischangePathList, string rutaConf, TfsItem item, string findTfsBranch, string cad, string ext)
        {
            //EVALUO ...
            if (item.path.Contains(@"/mpm.eClient.ecPortal.Web/Web.config"))
            {
                if (dischangePathList.Where(n => n.Path.Contains("\\Alojables\\")).Count() > 0)
                {
                    rutaConf = dischangePathList.FirstOrDefault(d => d.Path.Contains(@"\Alojables\")).Path;
                    if (item.path.Contains(findTfsBranch))
                    {
                        cad = UtilHelper.extraerBranchTfs(rutaConf, '/', ext, findTfsBranch);
                        cad += rutaConf;
                        ExtraerBranchTFS2(cad, item, findTfsBranch);

                    }
                    AddPathGu(findTfsBranch, item.version);
                }
            }
            else
            {
                //QUIERO EVALUAR SI LA CARPETA CONFIGURABLE ESTA COMPLETA EN LOS 4 AMBIENTES
                if ((dischangePathList.Where(n => n.Path.Contains("\\Configurables\\Cer")).Count() > 0 || dischangePathList.Where(n => n.Path.Contains("\\Configurables\\Cert")).Count() > 0) &&
                    dischangePathList.Where(n => n.Path.Contains("\\Configurables\\Pre")).Count() > 0 &&
                    dischangePathList.Where(n => n.Path.Contains("\\Configurables\\Pro")).Count() > 0 &&
                    (dischangePathList.Where(n => n.Path.Contains("\\Configurables\\Des")).Count() == 0 ||
                    dischangePathList.Where(n => n.Path.Contains("\\Configurables\\Desa")).Count() == 0))
                {
                    if (dischangePathList.Where(a => a.Path.Contains("custom-context.xml")).Count() == 0 &&
                        dischangePathList.Where(a => a.Path.Contains("customer-operation-services.xml")).Count() == 0)
                    {
                        foreach (DischangePath item2 in dischangePathList)
                        {
                            if (item2.Path.Contains("Cer"))
                            {
                                rutaConf = item2.Path;
                                rutaConf = rutaConf.Replace("Cer", "DESA");

                                if (item.path.Contains(findTfsBranch))
                                {
                                    cad = UtilHelper.extraerBranchTfs(rutaConf, '/', ext, findTfsBranch);
                                    cad += rutaConf;
                                    ExtraerBranchTFS2(cad, item, findTfsBranch);

                                }
                                AddPathGu(findTfsBranch, item.version);
                            }
                        }
                    }
                }
            }
        }

        private void AddPathGu(string findTfsBranch, int version)
        {
            if (PathGU.Count == 0)
            {
                PathGU.Add(ListComponent);
            }
            else
            {
                int canexist = PathGU.Where(n => n.Path == ListComponent.Path && n.Branch == findTfsBranch && n.changeset == version.ToString()).Count();
                if (canexist == 0)
                {
                    PathGU.Add(ListComponent);
                }
            }
        }

        private bool ObtenerSoloModificado(string path1, TfsItem item, bool flag, string findTfsBranch)
        {
            //EXTRAIGO UNA PARTE DE LA RUTA 
            string cutPath = path1.Replace(@"\Alojables\DIS\eClient\", "").Replace(@"\", "/");
            if (item.path.Contains(cutPath))
            {
                ExtraerBranchTFS2(path1, item, findTfsBranch);
                flag = true;
            }
            return flag;
        }

        private void NoGuiaUbicaciones(List<DischangePath> dischangePathList, string valueString, TfsItem item, string rutaConf, string findTfsBranch, string cad, string ext)
        {
            List<string> lstRutaConf = new List<string>();
            if (!item.path.Contains("BSM Robot"))
            {
                if (valueString.Count() > 0)
                {
                    if (item.path.Contains(".hbm.xml"))
                    {
                        //OBTENGO UNA PARTE DE LA RUTA DEL TFS
                        int positionMap = item.path.IndexOf("Mappings");
                        string rutaTfs = item.path.Substring(positionMap, item.path.Length - positionMap).Replace(@"/", @"\");
                        lstRutaConf.Add($@"\Batch\{rutaTfs}");
                        lstRutaConf.Add($@"\SegServices\bin\{rutaTfs}");
                        if (item.path.Contains(findTfsBranch))
                        {
                            foreach (string lstRuta in lstRutaConf)
                            {
                                cad = UtilHelper.extraerBranchTfs(lstRuta, '/', ext, findTfsBranch);
                                cad += lstRuta;
                                ExtraerBranchTFS2(cad, item, findTfsBranch);
                                AddPathGu(findTfsBranch, item.version);
                            }
                        }
                    }
                }
            }
            else
            {
                Log4net.log.Error($@"No es parte de los componentes --> {item.path}");
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

        private void ExtraerBranchTFS2(string rutaConf, TfsItem item, string findTfsBranch)
        {
            ListComponent = new ListComponent();
            ListComponent.Path = rutaConf;
            ListComponent.changeset = item.version.ToString();

            bool branchUse = _branchUses.FirstOrDefault(c => c.NameBranch == findTfsBranch).UseBranch;

            if (branchUse && item.path.Contains(findTfsBranch))
            {
                ListComponent.Branch = findTfsBranch;
                ListComponent.Confirm = branchUse;
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
                return $"Error: {item.Changeset} - Seleccione el branch asociado al changeset para generar el paquete";
            }
            else
            {
                return $"Error: {item.Changeset} - No fue seleccionado ningun branch para generar el paquete";
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
