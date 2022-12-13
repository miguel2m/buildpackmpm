using AutoDischange.Model;
using AutoDischange.ViewModel.Commands;
using AutoDischange.ViewModel.Helpers;
using Microsoft.Win32;
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

        private string displayName;

        public string DisplayName
        {
            get { return displayName; }
            set
            {
                displayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        private string commentAuth;

        public string CommentAuth
        {
            get { return commentAuth; }
            set
            {
                commentAuth = value;
                OnPropertyChanged("CommentAuth");
            }
        }

        private string changeCreated;

        public string ChangeCreated
        {
            get { return changeCreated; }
            set
            {
                changeCreated = value;
                OnPropertyChanged("ChangeCreated");
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

        public ObservableCollection<JenkinsItem> JenkinsListPath { get; set; }

        public List<DischangePath> DischangePathList = new List<DischangePath>();

        public List<BranchUse> _branchUsesFront = new List<BranchUse>();

        public DischangeVM()
        {
            DischangeCommand = new DischangeCommand(this);
            DischangeChangesets = new ObservableCollection<DischangeChangeset>();
            TfsList = new ObservableCollection<TfsItem>();
            ComponentList = new ObservableCollection<DischangePath>();
            JenkinsListPath = new ObservableCollection<JenkinsItem>();
            DischangeStatus = "Nada que hacer.";
            PackageCommand = new PackageCommand(this);
            SelectedChangeset = new DischangeChangeset();
            //SortPackDischange.SortPack(@"C:\Users\miguelangel.medina\Documents\pack\20211118_P4.1_20221007180020\");

            List<DischangeChangeset> DischangeChangeset = (DatabaseHelper.Read<DischangeChangeset>()).ToList();
            if (DischangeChangeset.Any())
            {
                foreach (DischangeChangeset item in DischangeChangeset)
                {
                    DatabaseHelper.Delete(item);
                }
            }

        }
        //METODO PARA VERIFICAR COMPARAR BRANCHES DEL CSV CON LOS BRANCHES UTILES
        public void LoadBranchFront()
        {
            try
            {
                DischangeChangesets.Clear();
                _branchUsesFront.Clear();
                List<BranchJenkinsExcel> BranchesList = DatabaseHelper.Read<BranchJenkinsExcel>()
                    .OrderBy(x => x.Name).ToList();
                if (BranchesList.Count > 0)
                {
                    foreach (BranchJenkinsExcel item in BranchesList)
                    {
                        _branchUsesFront.Add(new BranchUse()
                        {
                            NameBranch = item.Name,
                            UseBranch = true
                        });
                    }
                    Log4net.log.Info($"[AP-10]-Lectura de Branchs utiles para comparar con CSV cargados.");
                }
                else
                {
                    DischangeStatus = $"[AP-10e]-No han sido cargados los datos de Branches Utiles. El proceso no continuara.";
                    Log4net.log.Error(DischangeStatus);
                    return;
                }
            }
            catch (Exception ex)
            {

                Log4net.log.Error($"[AP-10e]-{ex.Message}");
                DischangeStatus = $"Se ha detenido la ejecucion: {ex.Message}";
                return;
            }
        }

        public async void GetCsv(string fileName)
        {
            try
            {
                //FUNCION PARA OBTENER LA LISTA DE BRANCHES DE USO
                LoadBranchFront();
                string noData = string.Empty;
                bool flag = false;
                DischangeChangesets.Clear();
                //Agrego en una Lista los Changeset
                List<DischangeChangeset> AssesChangesets = await CsvHelper.ReadCSVChangeset(fileName);
                if (AssesChangesets.Count > 0)
                {
                    foreach (DischangeChangeset changeset in await CsvHelper.ReadCSVChangeset(fileName))
                    {
                        //Evaluamos si los datos son null o si viene con espacio en blanco
                        if ((String.IsNullOrWhiteSpace(changeset.Branch) && String.IsNullOrWhiteSpace(changeset.Changeset)) || (String.IsNullOrEmpty(changeset.Branch) && String.IsNullOrEmpty(changeset.Changeset)))
                        {
                            noData += "No agrego datos de changeset y branch";
                        }
                        else
                        {
                            //EVALUO SI NO TIENE BRANCH
                            if (String.IsNullOrWhiteSpace(changeset.Branch) || String.IsNullOrEmpty(changeset.Branch))
                            {
                                noData += $"{changeset.Changeset}|No indico el Branch";
                            }
                            else
                            {
                                //EVALUEMOS SI EL BRANCH ESTA DENTRO DE LOS BRANCH UTILES
                                foreach (var itemBranch in _branchUsesFront)
                                {
                                    if (itemBranch.NameBranch.Contains(changeset.Branch))
                                    {
                                        flag = true;
                                    }
                                }
                                if (flag)
                                {
                                    //EVALUAMOS SI EL CHANGESET ESTA VACIO
                                    if (String.IsNullOrWhiteSpace(changeset.Changeset))
                                    {
                                        noData += $"No indico el Changeset|{changeset.Branch}";
                                    }
                                    else
                                    {
                                        DischangeChangesets.Add(changeset);
                                    }
                                }
                                else
                                {
                                    noData += $"{changeset.Changeset}|El Branch no esta dentro de la lista de Branchs de Uso";
                                }

                            }

                        }
                    }
                    if (noData != "")
                    {
                        DischangeStatus = noData;
                        Log4net.log.Error($"[AP-1e]-{DischangeStatus}");
                    }
                    else
                    {
                        //FUNCION PARA OBTENER EL PRIMER CHANGESET Y SU INFORMACION
                        GetChangesetIndividual();
                        DischangeStatus = "Lista de changesets cargada.";
                        Log4net.log.Info($"[AP-1]-{DischangeStatus}");
                    }
                }
                else
                {
                    DischangeStatus = $"[AP-1e]-No agrego datos de changeset y branch";
                    Log4net.log.Error(DischangeStatus);
                }
            }
            catch (Exception ex)
            {
                Log4net.log.Error($"[AP-1e]-{ex.Message}");
                DischangeStatus = $"[AP-1e]-Error en CSV: { ex.Message}.";
            }

        }
        //FUNCION PARA OBTENER LA INFORMACION DEL PRIMER CHANGESET
        public async void GetChangesetIndividual()
        {
            TfsList.Clear();
            ComponentList.Clear();
            JenkinsListPath.Clear();
            DischangeStatus = "Obteniendo Path del TFS.";
            DisplayName = "";
            CommentAuth = "";
            ChangeCreated = "";
            DischangeChangeset primerChangeset = new DischangeChangeset();
            primerChangeset.Changeset = DischangeChangesets.FirstOrDefault().Changeset;
            primerChangeset.Branch = DischangeChangesets.FirstOrDefault().Branch;
            SelectedChangeset = primerChangeset;
            List<TfsItem> OneItem = await TFSRequest.GetChangeset(primerChangeset.Changeset);
            TfsModelDetail Oneauthor = await TFSRequest.GetChangesetAuthor(primerChangeset.Changeset);
            DisplayName = Oneauthor.author.displayName;
            CommentAuth = Oneauthor.comment;
            ChangeCreated = Oneauthor.createdDate.ToString();
            if (!string.IsNullOrEmpty(primerChangeset.Branch))
            {
                OneItem = OneItem.FindAll((item) => item.path.Contains(primerChangeset.Branch));
            }
            if (OneItem.FirstOrDefault() != null)
            {
                if (OneItem.First().path.Contains(primerChangeset.Branch))
                {
                    if (OneItem.First().changetype.Contains("delete, merge"))
                    {
                        OneItem.First().path += " [Fusionar mediante combinación, eliminar]";
                    }
                    TfsList.Add(OneItem.First());
                }
                DischangeStatus = $"Path Changesets del TFS Obtenido cantidad: {TfsList.Count}.";
                Log4net.log.Info($"[AP-1]-{DischangeStatus}");
            }
            else
            {
                DischangeStatus = $"El changeset {SelectedChangeset.Changeset} en el branch {SelectedChangeset.Branch} no posee componentes.";
                Log4net.log.Error($"[AP-1e]-{DischangeStatus}");
                MessageBox.Show($"El changeset {SelectedChangeset.Changeset} en el branch {SelectedChangeset.Branch} no posee componentes.", $"El changeset {SelectedChangeset.Changeset} en el branch {SelectedChangeset.Branch} no posee componentes.", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public async void GetChangeset()
        {            
            if (SelectedChangeset == null)
            {
                TfsList.Clear();
                ComponentList.Clear();
                JenkinsListPath.Clear();
                DisplayName = "";
                CommentAuth = "";
                ChangeCreated = "";
            }
            else
            {
                try
                {
                    TfsList.Clear();
                    ComponentList.Clear();
                    JenkinsListPath.Clear();
                    DischangeStatus = "Obteniendo Path del TFS.";
                    DisplayName = "";
                    CommentAuth = "";
                    ChangeCreated = "";
                    List<TfsItem> allItem = await TFSRequest.GetChangeset(SelectedChangeset.Changeset);
                    TfsModelDetail author = await TFSRequest.GetChangesetAuthor(SelectedChangeset.Changeset);

                    DisplayName = author.author.displayName;
                    CommentAuth = author.comment;
                    ChangeCreated = author.createdDate.ToString();
                    if (!string.IsNullOrEmpty(SelectedChangeset.Branch))
                    {
                        allItem = allItem.FindAll((item) => item.path.Contains(SelectedChangeset.Branch));
                    }
                    if (allItem.FirstOrDefault() != null)
                    {
                        foreach (TfsItem itemLocal in allItem)
                        {
                            if (!string.IsNullOrEmpty(SelectedChangeset.Branch))
                            {
                                if (itemLocal.path.Contains(SelectedChangeset.Branch))
                                {
                                    if (itemLocal.changetype.Contains("delete, merge"))
                                    {
                                        itemLocal.path += " [Fusionar mediante combinación, eliminar]";
                                    }
                                    TfsList.Add(itemLocal);
                                }
                            }
                            else
                            {
                                TfsList.Add(itemLocal);
                            }
                        }
                        DischangeStatus = $"Path Changesets del TFS Obtenido cantidad: {TfsList.Count}.";
                    }
                    else
                    {
                        DischangeStatus = $"El changeset {SelectedChangeset.Changeset} en el branch {SelectedChangeset.Branch} no posee componentes.";
                        MessageBox.Show($"El changeset {SelectedChangeset.Changeset} en el branch {SelectedChangeset.Branch} no posee componentes.", $"El changeset {SelectedChangeset.Changeset} en el branch {SelectedChangeset.Branch} no posee componentes.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    Log4net.log.Error(ex.Message);
                    DischangeStatus = $"Error al intentar conectar con el TFS: { ex.Message}.";
                    MessageBox.Show("Error al intentar conectar con el TFS: " + ex.Message, "Error al intentar conectar con el TFS", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void GetTfsComponent()
        {
            string ext = string.Empty;
            if (TfsSelected != null)
            {
                try
                {
                    ComponentList.Clear();
                    JenkinsListPath.Clear();
                    string valueString = String.Empty, nameFile2 = String.Empty;
                    ext = Path.GetExtension(TfsSelected.path);
                    if (ext.Contains(" [Fusionar mediante combinación, eliminar]"))
                    {
                        ext = ext.Replace(" [Fusionar mediante combinación, eliminar]", "");
                        TfsSelected.path = TfsSelected.path.Replace(" [Fusionar mediante combinación, eliminar]", "");
                    }
                    if (ext == ".cs")
                    {
                        List<string> listValue = UtilHelper.fileList(TfsSelected.path, '/');
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
                            valueString = UtilHelper.nameFile(TfsSelected.path, '/');
                        }
                    }
                    else if (ext == ".config" && TfsSelected.path.Contains("mpm.eClient.ecPortal.Web"))
                    {
                        valueString = $@"\eClient\{UtilHelper.nameFile(TfsSelected.path, '/')}";
                    }
                    else
                    {
                        if (ext == ".sql")
                        {
                            valueString = UtilHelper.extraerBranchTfs(TfsSelected.path, '/', ext);
                            List<string> listValue = UtilHelper.fileList(TfsSelected.path, '/');
                            nameFile2 = listValue.First(i => i.Contains(".sql"));
                            valueString += $"\\{nameFile2}";
                        }
                        else
                        {
                            if (TfsSelected.path.Contains("/Configuracion/Procesos/"))///Configuracion/Procesos/
                            {
                                valueString = "\\ProcesosFull\\" + UtilHelper.nameFile(TfsSelected.path, '/');
                            }
                            else
                            {
                                valueString = UtilHelper.nameFile(TfsSelected.path, '/');
                            }
                        }
                    }
                    
                    //ACA BUSCO LOS SCRIPT DE SQL PERO EN EL JENKINS PARA MOSTRARLO EN FRONT
                    if (ext == ".sql")
                    {
                        JenkinsItem jenkinsItem = new JenkinsItem() { JkPath = valueString };
                        JenkinsListPath.Add(jenkinsItem);
                    }
                    else
                    {
                        DischangePath dischangePath = new DischangePath();
                        DischangePathList = DatabaseHelper.Read<DischangePath>().Where(n => n.Path.Contains(valueString) && !n.Path.Contains("\\DIS\\Upgrade\\")).ToList();

                        //TENGO UN ARCHIVO EN EL TFS PERO NO TENGO LA RUTA EN LA GUIA DE UBICACIONES 
                        NoGuiaUbicaciones(valueString, dischangePath, DischangePathList);

                        string rutaConf = string.Empty;
                        //QUIERO EVALUAR SI LA CARPETA CONFIGURABLE ESTA COMPLETA EN LOS 4 AMBIENTES
                        ConfigurableNoDesa(DischangePathList, rutaConf, dischangePath);                                

                        foreach (DischangePath itemLocal in DischangePathList)
                        {
                            if (itemLocal.Path.Contains("custom-context.xml"))
                            {
                                if (!itemLocal.Path.Contains("Configurables") && !itemLocal.Path.Contains("BSM"))
                                {
                                    if (TfsSelected.path.Contains("mpm.seg.Customers.eClient.Web") && itemLocal.Path.Contains(@"eClient\CustomerSettings"))
                                    {
                                        ComponentList.Add(itemLocal);
                                    }
                                    else if (TfsSelected.path.Contains("mpm.seg.Customers.DataRecovers") && itemLocal.Path.Contains(@"ecDataProvider\CustomerSettings"))
                                    {
                                        ComponentList.Add(itemLocal);
                                    }
                                }
                            }
                            else if (itemLocal.Path.Contains("customer-operation-services.xml"))
                            {
                                if (itemLocal.Path.Contains("Alojables"))
                                {
                                    ComponentList.Add(itemLocal);
                                }
                            }
                            else if (ext == ".js" && DischangePathList.Count > 1)
                            {
                                ObtenerSoloModificado(itemLocal, TfsSelected.path);
                            }
                            else
                            {
                                ComponentList.Add(itemLocal);
                            }

                            //EN CASO DE QUE EL COMPONENTE POSEA UN CHANGETYPE DE TIPO delete, merge DEBE IMPRIMIR UN MENSAJE
                            if (TfsSelected.changetype.Contains("delete, merge"))
                            {
                                foreach (DischangePath item in ComponentList)
                                {
                                    item.Path += " [Fusionar mediante combinación, eliminar]";
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log4net.log.Error(ex.Message);
                    DischangeStatus = $"Error al intentar conectar con el DIS-Change.xlsx: { ex.Message}.";
                    MessageBox.Show("Error al intentar conectar con el DIS-Change.xlsx : " + ex.Message, "Error al intentar conectar con el DIS-Change.xlsx", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }

        }

        private void ObtenerSoloModificado(DischangePath itemLocal, string path2)
        {
            //EXTRAIGO UNA PARTE DE LA RUTA 
            string cutPath = itemLocal.Path.Replace(@"\Alojables\DIS\eClient\", "").Replace(@"\", "/");
            if (path2.Contains(cutPath))
            {
                ComponentList.Add(itemLocal);
            }
        }

        private void ConfigurableNoDesa(List<DischangePath> dischangePathList, string rutaConf, DischangePath dischangePath)
        {
            if (DischangePathList.Where(n => n.Path.Contains("\\Configurables\\Cer")).Count() > 0 &&
                            DischangePathList.Where(n => n.Path.Contains("\\Configurables\\Pre")).Count() > 0 &&
                            DischangePathList.Where(n => n.Path.Contains("\\Configurables\\Pro")).Count() > 0 &&
                                (
                                    DischangePathList.Where(n => n.Path.Contains("\\Configurables\\Des")).Count() == 0 &&
                                    DischangePathList.Where(n => n.Path.Contains("\\Configurables\\Desa")).Count() == 0
                                )
                            )
            {
                foreach (var item in DischangePathList)
                {
                    if (!item.Path.Contains("custom-context.xml") && !item.Path.Contains("customer-operation-services.xml"))
                    {
                        if (!item.Path.Contains("Alojables"))
                        {
                            if (item.Path.Contains("Cer"))
                            {
                                rutaConf = item.Path;
                                dischangePath.Path = rutaConf.Replace("Cer", "Desa");
                                ComponentList.Add(dischangePath);
                            }
                        }
                    }
                }
            }
        }

        private void NoGuiaUbicaciones(string valueString, DischangePath dischangePath, List<DischangePath> dischangePathList)
        {
            //TENGO UN ARCHIVO EN EL TFS PERO NO TENGO LA RUTA EN LA GUIA DE UBICACIONES  
            if (DischangePathList.Count() == 0)
            {
                if (valueString.Count() > 0)
                {
                    if (TfsSelected.path.Contains("/Mappings/General/"))
                    {
                        dischangePath.Path = $@"\Alojables\DIS\Batch\Mappings\Partials\General\{valueString}";
                        ComponentList.Add(dischangePath);
                    }
                    if (valueString == "mpm.eClient.ecPortal.Web.dll" || valueString == "mpm.eClient.Common.Mvc.dll")
                    {
                        dischangePath.Path = $@"\Alojables\DIS\eClient\bin\{valueString}";
                        ComponentList.Add(dischangePath);
                    }
                    if (tfsSelected.path.Contains(@"/Configuracion/Procesos/"))
                    {
                        dischangePath.Path = $@"\Alojables\DIS\InstallBSM\Resources{valueString}";
                        ComponentList.Add(dischangePath);
                        //guardar en la BD Guia de Ubicaciones
                        //_ = DatabaseHelper.InsertDischange(dischangePath);
                        _ = DatabaseHelper.InsertReplaceDischange(dischangePath);
                    }
                    if (tfsSelected.path.Contains("/mpm.eClient.ecPortal.Web/Web.config"))
                    {
                        dischangePath.Path = $@"\Alojables\DIS{valueString}";
                        ComponentList.Add(dischangePath);
                        //guardar en la BD Guia de Ubicaciones
                        _ = DatabaseHelper.InsertReplaceDischange(dischangePath);
                        //_ = DatabaseHelper.InsertDischange(dischangePath);

                    }
                }
            }
            else
            {
                if (valueString == "mpm.seg.Customers.DataRecovers.dll" || valueString == "mpm.seg.Customers.DataRecovers.Ahorro.dll")
                {
                    if (DischangePathList.Where(n => n.Path.Contains("\\CalculusServices\\")).Count() == 0)
                    {
                        dischangePath.Path = $@"\Alojables\DIS\CalculusServices\bin\{valueString}";
                        ComponentList.Add(dischangePath);
                        _ = DatabaseHelper.InsertReplaceDischange(dischangePath);
                    }
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
