﻿using AutoDischange.Model;
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

        public DischangeVM()
        {
            DischangeCommand = new DischangeCommand(this);
            DischangeChangesets = new ObservableCollection<DischangeChangeset>();
            TfsList = new ObservableCollection<TfsItem>();
            ComponentList = new ObservableCollection<DischangePath>();
            JenkinsListPath = new ObservableCollection<JenkinsItem>();
            DischangeStatus = "Nada que hacer.";
            PackageCommand = new PackageCommand(this);
            //SortPackDischange.SortPack(@"C:\Users\miguelangel.medina\Documents\pack\20211118_P4.1_20221007180020\");

        }

        public async void GetExcel(string fileName)
        {
            try
            {
                string noData = string.Empty;
                DischangeChangesets.Clear();
                foreach (var changeset in await ExcelHelper.ReadExcel(fileName))
                {
                    if (changeset.Branch != "" && changeset.Changeset != "")
                    {
                        DischangeChangesets.Add(changeset);
                    }
                    else
                    {
                        noData += changeset.Changeset + " | ";
                    }
                }
                if (noData != "")
                {
                    DischangeStatus = $"Indicar Branch en los siguientes changeset: {noData}";
                    DischangeStatus += "| Lista de changesets cargada.";
                }
                else
                {
                    DischangeStatus = "Lista de changesets cargada.";
                }
            }
            catch (Exception ex)
            {
                Log4net.log.Error(ex.Message);
                DischangeStatus = $"Error en Excel: { ex.Message}.";
                MessageBox.Show("Error en Excel: " + ex.Message, "Error en Excel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        public async void GetChangeset()
        {
            if (SelectedChangeset == null)
            {
                TfsList.Clear();
                ComponentList.Clear();
                JenkinsListPath.Clear();
            }
            else
            {
                try
                {
                    TfsList.Clear();
                    ComponentList.Clear();
                    JenkinsListPath.Clear();
                    DischangeStatus = "Obteniendo Path del TFS.";
                    List<TfsItem> allItem = await TFSRequest.GetChangeset(SelectedChangeset.Changeset);
                    TfsModelDetail author = await TFSRequest.GetChangesetAuthor(SelectedChangeset.Changeset);

                    DisplayName = author.author.displayName;
                    CommentAuth = author.comment;
                    ChangeCreated = author.createdDate.ToShortDateString();
                    if (!string.IsNullOrEmpty(SelectedChangeset.Branch))
                    {
                        allItem = allItem.FindAll((item)=> item.path.Contains(SelectedChangeset.Branch));
                    }
                    if (allItem.FirstOrDefault() != null)
                    {
                        foreach (TfsItem itemLocal in allItem)
                        {
                            if (!string.IsNullOrEmpty(SelectedChangeset.Branch))
                            {
                                if (itemLocal.path.Contains(SelectedChangeset.Branch))
                                {
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
                    if (ext == ".cs")
                    {
                        List<string> listValue = UtilHelper.fileList(TfsSelected.path, '/');
                        if (listValue.Where(n => n.Contains("mpm.seg")).Count() > 0)
                        {
                            valueString = listValue.First(i => i.Contains("mpm.seg"));
                        }
                        else
                        {
                            valueString = UtilHelper.nameFile(TfsSelected.path, '/');
                        }
                        
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
                        if (valueString == "mpm.seg.Customers.Workflow")
                        {
                            valueString += ".BSM.dll";
                        }
                        if (valueString == "mpm.seg.Customers.DataRecovers")
                        {
                            valueString += ".dll";
                        }
                        List<DischangePath> DischangePathList = DatabaseHelper.Read<DischangePath>().Where(n => n.Path.Contains(valueString) && !n.Path.Contains("Upgrade")).ToList();
                        
                        //QUIERO EVALUAR SI TIENE LAS CARPETAS COMPLETAS
                        
                        if (DischangePathList.Where(n => n.Path.Contains("\\Configurables\\")).Count() > 0 && DischangePathList.Where(n => n.Path.Contains("\\Configurables\\")).Count() < 4)//n.Path.Contains("appCustomerDataRecoverSettingsAhorro.config") &&  
                        {
                            if ((DischangePathList.Where(n => n.Path.Contains("\\Configurables\\Des")).Count() == 0) || (DischangePathList.Where(n => n.Path.Contains("\\Configurables\\Desa")).Count() == 0))
                            {
                                DischangePath dischangePath = new DischangePath();
                                dischangePath.Path = "\\Configurables\\Des\\DIS\\ecDataProvider\\CustomerSettings\\appCustomerDataRecoverSettingsAhorro.config";
                                ComponentList.Add(dischangePath);
                            }
                        }
                        foreach (DischangePath itemLocal in DischangePathList)
                        {
                            if (itemLocal.Path.Contains("custom-context.xml"))
                            {
                                if (!itemLocal.Path.Contains("Configurables") && 
                                    !itemLocal.Path.Contains("ecDataProvider") &&
                                    !itemLocal.Path.Contains("BSM"))
                                {
                                    ComponentList.Add(itemLocal);
                                }
                            }
                            else
                            {
                                ComponentList.Add(itemLocal);
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        

    }
}
