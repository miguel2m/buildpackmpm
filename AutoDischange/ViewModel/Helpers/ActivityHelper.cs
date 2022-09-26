using AutoDischange.Model;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.ViewModel.Helpers
{
    public class ActivityHelper
    {

        public static async Task ExportActivity(ActivityComponent activityComponent, string pathUser,string envExcel)
        {
            char backSlash           = Path.DirectorySeparatorChar;
            string pathAlojables     = $"{@activityComponent.PathStart}{backSlash}Alojables";
            string pathConfigurables = $"{@activityComponent.PathStart}{backSlash}Configurables";
            string pathScript        = $"{@activityComponent.PathStart}{backSlash}Scripts";

            System.IO.DirectoryInfo dirAlojables;
            System.IO.DirectoryInfo dirConfigurables ;
            System.IO.DirectoryInfo dirScript ;

            IEnumerable<System.IO.FileInfo> listAlojables;
            IEnumerable<System.IO.FileInfo> listConfigurables;
            IEnumerable<System.IO.FileInfo> listScript;

            

            List<ActivityComponentListAlojables> ActivityComponentListAlojablesList         = new List<ActivityComponentListAlojables>();
            List<ActivityComponentListConfigurables> ActivityComponentListConfigurablesList = new List<ActivityComponentListConfigurables>();
            List<ActivityComponentListScript> ActivityComponentListScriptList               = new List<ActivityComponentListScript>();

            //Actividades para despliegue
            List<ActivityComponentPrePro> ActivityResultListPre                             = new List<ActivityComponentPrePro>();//Despliegue Result PRE
            List<ActivityComponentPrePro> ActivityResultListPro                             = new List<ActivityComponentPrePro>();//Despliegue Result PRO

            List<ActivityComponentPrePro> ActivityProcessImportListPre                      = new List<ActivityComponentPrePro>();//Importancion de procesos PRE
            List<ActivityComponentPrePro> ActivityProcessImportListPro                      = new List<ActivityComponentPrePro>();//Importancion de procesos PRO

            List<ActivityComponentPrePro> ActivityPoolListPre                               = new List<ActivityComponentPrePro>(); //Manejo de POOLS PRE
            List<ActivityComponentPrePro> ActivityPoolListPro                               = new List<ActivityComponentPrePro>(); //Manejo de POOLS PRO

            List<ActivityComponentPrePro> ActivityResoruceImportListPre = new List<ActivityComponentPrePro>(); //Importancion de recursos
            List<ActivityComponentPrePro> ActivityResoruceImportListPro = new List<ActivityComponentPrePro>(); //Importancion de recursos


            if (!Directory.Exists(pathAlojables) && !Directory.Exists(pathConfigurables) && !Directory.Exists(pathScript))
            {
                throw new DirectoryNotFoundException("Debe seleccionar carpeta de componentes");
            }
            
            MemoryStream msPass = new MemoryStream();
            ExcelHelper.ReadExcelEntrega().WriteTo(msPass);

            //Listado de Alojables
            if (Directory.Exists(pathAlojables))
            {
                dirAlojables = new System.IO.DirectoryInfo(pathAlojables);
                listAlojables = dirAlojables.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
                List<string> DischangePathList = await ListPathDischange(listAlojables);
                ActivityComponentListAlojablesList = await ListAlojables(DischangePathList);        
                if (ActivityComponentListAlojablesList.Any())
                {
                    MemoryStream msPassTemp = ExcelHelper.ListadoAlojables(msPass, ActivityComponentListAlojablesList) ;
                    msPass = new MemoryStream();
                    //msPassTemp.Position = 0;
                    msPassTemp.WriteTo(msPass);
                   
                }
            }

            //Listado de Configurables
            if (Directory.Exists(pathConfigurables))
            {
                dirConfigurables = new System.IO.DirectoryInfo(pathConfigurables);
                listConfigurables = dirConfigurables.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
                List<string> DischangePathList = await ListPathDischange(listConfigurables);
                ActivityComponentListConfigurablesList = await ListConfigurables(DischangePathList, envExcel);
                if (ActivityComponentListConfigurablesList.Any())
                {
                    MemoryStream msPassTemp = new MemoryStream();
                    ExcelHelper.ListadoConfigurables(msPass, ActivityComponentListConfigurablesList).WriteTo(msPassTemp);
                    msPass = new MemoryStream();
                    msPassTemp.WriteTo(msPass);
                }
            }

            //Listado de Script
            if (Directory.Exists(pathConfigurables))
            {
                dirScript = new System.IO.DirectoryInfo(pathScript);
                listScript = dirScript.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
                List<string> DischangePathList = await ListPathDischange(listScript);
                ActivityComponentListScriptList = await ListScript(DischangePathList);
                if (ActivityComponentListScriptList.Any())
                {
                    MemoryStream msPassTemp = new MemoryStream();
                    ExcelHelper.ListadoScript(msPass, ActivityComponentListScriptList).WriteTo(msPassTemp);
                    msPass = new MemoryStream();
                    msPassTemp.WriteTo(msPass);
                }
            }

            //Listado de Actividades para despliegue ActividadesParaDesplegarPre  ActividadesParaDesplegarPro
            //Script TODO
            //Si traigo xml en ProcesosFull entonces debo ejecutar actividades de importación de procesos 
            //(importación de procesos)
            if (ActivityComponentListAlojablesList.Any())
            {

                List<ActivityComponentListAlojables> ProcesosFull = ActivityComponentListAlojablesList.FindAll(i => i.DischangeComponentName.FindAll(item => item.Contains($@"ProcesosFull")).Any());
                if (ProcesosFull.Any())
                {
                    List<ActivityComponentListAlojables> ProcesosFullXml = ProcesosFull.FindAll(i => i.DischangeComponentName.FindAll(item => item.Contains($@"xml")).Any());
                    if (ProcesosFullXml.Any())
                    {
                        ActivityProcessImportListPre = await ListProcessImport(0,false,0);//Listado PRE
                        
                        ActivityProcessImportListPro = await ListProcessImport(1, false, 0);//Listado PRO
                        if (ActivityProcessImportListPre.Any() ) //PRE
                        {
                            ActivityResultListPre.AddRange(ActivityProcessImportListPre);
                            MemoryStream msPassTemp = new MemoryStream();
                            ExcelHelper.DeployActivity(msPass, ActivityResultListPre, "ActividadesParaDesplegarPre").WriteTo(msPassTemp);
                            msPass = new MemoryStream();
                            msPassTemp.WriteTo(msPass);
                        }
                        if (ActivityProcessImportListPro.Any()) //PRO
                        { 
                            ActivityResultListPro.AddRange(ActivityProcessImportListPre);
                            MemoryStream msPassTemp = new MemoryStream();
                            ExcelHelper.DeployActivity(msPass, ActivityResultListPro, "ActividadesParaDesplegarPro").WriteTo(msPassTemp);
                            msPass = new MemoryStream();
                            msPassTemp.WriteTo(msPass);
                        }
                    }

                }

            }

            //Si traigo otros alojables y/o configurables entonces debo ejecutar actividades de despligue y de manejo de pools (prender y apagar)
            //manejo de pools
            if (ActivityComponentListAlojablesList.Any() && ActivityComponentListConfigurablesList.Any())
            {
                //PRE
                if (ActivityProcessImportListPre.Any())
                {
                    ActivityPoolListPre = await ListPoolManager(0, ActivityProcessImportListPre.Count());
                }
                else
                {
                    ActivityPoolListPre = await ListPoolManager(0, 0);
                }

                if (ActivityPoolListPre.Any())
                {
                    ActivityResultListPre.AddRange(ActivityPoolListPre);
                    if (ActivityProcessImportListPre.Any())
                    {

                        ActivityResultListPre.AddRange(await ListProcessImport(0, true, ActivityResultListPre.Count()));
                    }
                    MemoryStream msPassTemp = new MemoryStream();
                    ExcelHelper.DeployActivity(msPass, ActivityResultListPre, "ActividadesParaDesplegarPre").WriteTo(msPassTemp);
                    msPass = new MemoryStream();
                    msPassTemp.WriteTo(msPass);
                }
                //PRO
                if (ActivityProcessImportListPro.Any())
                {
                    ActivityPoolListPro = await ListPoolManager(1, ActivityProcessImportListPro.Count());
                }
                else
                {
                    ActivityPoolListPro = await ListPoolManager(1, 0);
                }

                if (ActivityPoolListPro.Any())
                {
                    ActivityResultListPro.AddRange(ActivityPoolListPro);
                    if (ActivityProcessImportListPro.Any())
                    {

                        ActivityResultListPro.AddRange(await ListProcessImport(1, true, ActivityResultListPro.Count()));
                    }
                    
                    MemoryStream msPassTemp = new MemoryStream();
                    ExcelHelper.DeployActivity(msPass, ActivityResultListPro, "ActividadesParaDesplegarPro").WriteTo(msPassTemp);
                    msPass = new MemoryStream();
                    msPassTemp.WriteTo(msPass);
                }


            }
            //Si traigo el componente customer-resources.csv en Resources entonces ejecutar importación de recursos
            //importación de recursos
            if (ActivityComponentListAlojablesList.Any())
            {
                List<ActivityComponentListAlojables> RecursosCsv = ActivityComponentListAlojablesList.FindAll(i => i.DischangeComponentName.FindAll(item => item.Contains($@"customer-resources.csv")).Any());
                if (RecursosCsv.Any())
                {
                    ActivityResoruceImportListPre = await ListResoruceImport(0, ActivityProcessImportListPre.Count());//Listado PRE

                    ActivityResoruceImportListPro = await ListResoruceImport(1, ActivityProcessImportListPro.Count());//Listado PRO

                    if (ActivityResoruceImportListPre.Any()) //PRE
                    {
                        ActivityResultListPre.AddRange(ActivityResoruceImportListPre);
                        ActivityResultListPre.AddRange(await ListEndActivity(0, ActivityResultListPre.Count)); //END EXCEL PRE
                        MemoryStream msPassTemp = new MemoryStream();
                        ExcelHelper.DeployActivity(msPass, ActivityResultListPre, "ActividadesParaDesplegarPre").WriteTo(msPassTemp);
                        msPass = new MemoryStream();
                        msPassTemp.WriteTo(msPass);
                    }
                    if (ActivityResoruceImportListPro.Any()) //PRO
                    {
                        ActivityResultListPro.AddRange(ActivityResoruceImportListPro);
                        ActivityResultListPro.AddRange(await ListEndActivity(1, ActivityResultListPro.Count)); //END EXCEL PRO
                        MemoryStream msPassTemp = new MemoryStream();
                        ExcelHelper.DeployActivity(msPass, ActivityResultListPro, "ActividadesParaDesplegarPro").WriteTo(msPassTemp);
                        msPass = new MemoryStream();
                        msPassTemp.WriteTo(msPass);
                    }
                }
                


            }
            if(msPass.CanRead)
                ExcelHelper.SaveExcelEntrega(msPass, pathUser);
        }
        //Read  ListPathDischange (Guia de ubicaciones)
        public static async Task<List<string>> ListPathDischange(IEnumerable<System.IO.FileInfo> listInput)
        {
            List<string> DischangePathList = new List<string>();

            Task task0 = new Task( () =>
            {
                foreach (System.IO.FileInfo item in listInput)
                {
                    DischangePathList.Add(item.Name);
                }
            });
            
            task0.Start();
            await task0;
            
            return (DischangePathList.Any())?DischangePathList.Distinct().ToList(): DischangePathList;
        }

        //ListadoAlojables Sheet
        public static async Task<List<ActivityComponentListAlojables>> ListAlojables(List<string> listInput)
        {
            
           
            Task<List<ActivityComponentListAlojables>> task0 = new Task<List<ActivityComponentListAlojables>>(() =>
            {
                List<ActivityComponentListAlojables> ActivityComponentListAlojablesListTask = new List<ActivityComponentListAlojables>();
                ActivityComponentListAlojables ActivityComponentListAlojables;
                int _contador = 1;
                foreach (string item in listInput)
                {
                    
                    List<DischangePath> dischangePathListTemp = (DatabaseHelper.Read<DischangePath>()).Where(n => n.Path.Contains(item)).ToList();
                    ActivityComponentListAlojables = new ActivityComponentListAlojables();
                    ActivityComponentListAlojables.Id = _contador;
                    ActivityComponentListAlojables.Workbook = "ListadoAlojables";
                    if (dischangePathListTemp.Any())
                    {
                        //DischangePathList.Add(dischangePathListTemp.First().Path);
                        foreach (DischangePath itemTemp in dischangePathListTemp)
                        {
                            ActivityComponentListAlojables.DischangeComponentName.Add(itemTemp.Path);

                        }
                    }
                    else
                    {
                        ActivityComponentListAlojables.DischangeComponentName.Add(item);
                    }
                    ActivityComponentListAlojablesListTask.Add(ActivityComponentListAlojables);
                    _contador++;
                }
                return ActivityComponentListAlojablesListTask;
            });
            task0.Start();

            return await task0;
        }

        //ListadoConfigurables Sheet
        public static async Task<List<ActivityComponentListConfigurables>> ListConfigurables(List<string> listInput,string envExcel)
        {
            
           
            Task<List<ActivityComponentListConfigurables>> task0 = new Task<List<ActivityComponentListConfigurables>>( () =>
            {
                List<ActivityComponentListConfigurables> ActivityComponentListConfigurablesList = new List<ActivityComponentListConfigurables>();
                ActivityComponentListConfigurables ActivityComponentConfigurables;
                int _contador = 1;
                foreach (string item in listInput)
                {
                   
                    List<DischangePath> dischangePathListTemp = (DatabaseHelper.Read<DischangePath>()).Where(n => n.Path.Contains($"{item}")).ToList();
                    ActivityComponentConfigurables = new ActivityComponentListConfigurables();
                    ActivityComponentConfigurables.Id = _contador;
                    ActivityComponentConfigurables.Workbook = "ListadoConfigurables";
                    ActivityComponentConfigurables.ComponentEnv = envExcel.ToUpper();

                    if (dischangePathListTemp.Any())
                    {
                        //DischangePathList.Add(dischangePathListTemp.First().Path);
                        foreach (DischangePath itemTemp in dischangePathListTemp)
                        {
                            if (itemTemp.Path.Contains($@"\{ envExcel}\"))
                                ActivityComponentConfigurables.DischangeComponentName.Add(itemTemp.Path);
                            else
                            {
                                if (envExcel.Equals("Pre Pro"))
                                {
                                    if (itemTemp.Path.Contains($@"\Pre\"))
                                        ActivityComponentConfigurables.DischangeComponentName.Add(itemTemp.Path);
                                    if (itemTemp.Path.Contains($@"\Pro\"))
                                        ActivityComponentConfigurables.DischangeComponentName.Add(itemTemp.Path);
                                }
                            }
                        }
                    }
                    else
                    {
                        ActivityComponentConfigurables.DischangeComponentName.Add(item);
                    }
                    ActivityComponentListConfigurablesList.Add(ActivityComponentConfigurables);
                    _contador++;
                }
                return ActivityComponentListConfigurablesList;
            });
            task0.Start();
            

            return await task0;
        }

        //ListadoScript Sheet
        public static async Task<List<ActivityComponentListScript>> ListScript(List<string> listInput)
        {
            
            Task<List<ActivityComponentListScript>> task0 = new Task<List<ActivityComponentListScript>>( () =>
            {
                List<ActivityComponentListScript> ActivityComponentScriptList = new List<ActivityComponentListScript>();
                ActivityComponentListScript ActivityComponentScript;
                int _contador = 1;
                foreach (string item in listInput)
                {
                    
                    List<DischangePath> dischangePathListTemp = (DatabaseHelper.Read<DischangePath>()).Where(n => n.Path.Contains(item)).ToList();
                    ActivityComponentScript = new ActivityComponentListScript();
                    ActivityComponentScript.Id = _contador;
                    ActivityComponentScript.Workbook = "ListadoScripts";
                    if (item.Contains("DDL")) ActivityComponentScript.TypeScript = "DDL";
                    if (item.Contains("DML")) ActivityComponentScript.TypeScript = "DML";
                    if (item.Contains("SIN")) ActivityComponentScript.TypeScript = "SIN";
                    if (item.Contains("SIB")) ActivityComponentScript.TypeScript = "SIB";
                    if (dischangePathListTemp.Any())
                    {
                        //DischangePathList.Add(dischangePathListTemp.First().Path);
                        foreach (DischangePath itemTemp in dischangePathListTemp)
                        {
                            ActivityComponentScript.DischangeComponentName.Add(itemTemp.Path);
                            

                        }
                    }
                    else
                    {
                        ActivityComponentScript.DischangeComponentName.Add(item);
                    }
                    ActivityComponentScriptList.Add(ActivityComponentScript);
                    _contador++;
                }
                return ActivityComponentScriptList;
            });
            task0.Start();
            

            return await task0;
        }

        //TODO Script ActivityComponentPrePro

        //(importación de procesos)
        public static async Task<List<ActivityComponentPrePro>> ListProcessImport(int _env, bool _poolManager, int? _pendingActivity)
        {

            Task<List<ActivityComponentPrePro>> task0 = new Task<List<ActivityComponentPrePro>>(() =>
            {
                List<ActivityComponentPrePro> ActivityComponentPreProList = new List<ActivityComponentPrePro>();
                ActivityComponentPrePro ActivityComponentPrePro = new ActivityComponentPrePro();
                ActivityComponentPrePro.font = new SLFont();
                ActivityComponentPrePro.font.Bold = true;
                //int _contador = 1;
                if (!_poolManager)
                {
                    if (_env == 0)
                    {   //PRE
                        ActivityComponentPrePro.PendindActivity = "Ninguna";
                        ActivityComponentPrePro.TypeActivity = "Despliegue";
                        //ActivityComponentPrePro.Activity = $"Entrar al servidor <b>SRNEUIWM1MXR309 180.228.64.204 Batch</b> y hacer las siguientes acciones:{ System.Environment.NewLine}";
                        ActivityComponentPrePro.rst.AppendText($"Entrar al servidor ");
                        ActivityComponentPrePro.rst.AppendText( $"SRNEUIWM1MXR309 180.228.64.204 Batch ", ActivityComponentPrePro.font);
                        ActivityComponentPrePro.rst.AppendText($"y hacer las siguientes acciones:{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($"1.-");
                        ActivityComponentPrePro.rst.AppendText($"Eliminar el contenido de la carpeta ", ActivityComponentPrePro.font);
                        ActivityComponentPrePro.rst.AppendText($@"D:\MPM\DIS\InstallBSM\Resources\ProcesosFull");
                        ActivityComponentPreProList.Add(ActivityComponentPrePro);
                    }
                    else
                    {   //PRO
                        ActivityComponentPrePro.PendindActivity = "Ninguna";
                        ActivityComponentPrePro.TypeActivity = "Despliegue";
                        ActivityComponentPrePro.rst.AppendText($"Entrar al servidor ");
                        ActivityComponentPrePro.rst.AppendText($"SRVNEUPVWMX09 180.181.167.59 Batch", ActivityComponentPrePro.font);
                        ActivityComponentPrePro.rst.AppendText($"y hacer las siguientes acciones:{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($"1.-");
                        ActivityComponentPrePro.rst.AppendText($"Eliminar el contenido de la carpeta", ActivityComponentPrePro.font);
                        ActivityComponentPrePro.rst.AppendText($@"D:\MPM\DIS\InstallBSM\Resources\ProcesosFull");

                        //ActivityComponentPrePro.Activity = $"Entrar al servidor <b>SRVNEUPVWMX09 180.181.167.59 Batch</b> y hacer las siguientes acciones:{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += @"1.- <b>Eliminar el contenido de la carpeta</b> D:\MPM\DIS\InstallBSM\Resources\ProcesosFull";
                        ActivityComponentPreProList.Add(ActivityComponentPrePro);
                    }
                }
                else
                {
                    if (_env == 0)
                    {   //PRE


                        ActivityComponentPrePro.PendindActivity = _pendingActivity.ToString();
                        ActivityComponentPrePro.TypeActivity = "Despliegue";

                        ActivityComponentPrePro.rst.AppendText($"Entrar al servidor ");
                        ActivityComponentPrePro.rst.AppendText($"SRNEUIWM1MXR309 180.228.64.204 Batch", ActivityComponentPrePro.font);
                        ActivityComponentPrePro.rst.AppendText($"Abrir como Administrador la línea de comandos{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($@"Ir a la carpeta D:\MPM\DIS\InstallBSM\{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($"Ejecutar el archivo ");
                        ActivityComponentPrePro.rst.AppendText($"Install-DIS-Procesos.cmd{ System.Environment.NewLine} ", ActivityComponentPrePro.font);
                        ActivityComponentPrePro.rst.AppendText($@"Al finalizar la ejecución compartir el archivo log-import-file.log que se encuentra en D:\MPM\DIS\MPM.FullProcessImport\Logs\Import{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($"Esperar validación de los logs para continuar con la siguiente actividad.{ System.Environment.NewLine}");
                        ActivityComponentPreProList.Add(ActivityComponentPrePro);

                        //ActivityComponentPrePro.Activity = $"En el servidor <b>SRNEUIWM1MXR309 180.228.64.204 Batch</b>:{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"Abrir como Administrador la línea de comandos{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"Ir a la carpeta D:\MPM\DIS\InstallBSM\{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"Ejecutar el archivo <b>Install-DIS-Procesos.cmd</b>{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"Al finalizar la ejecución compartir el archivo log-import-file.log que se encuentra en D:\MPM\DIS\MPM.FullProcessImport\Logs\Import{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"Esperar validación de los logs para continuar con la siguiente actividad.{ System.Environment.NewLine}";
                        //ActivityComponentPreProList.Add(ActivityComponentPrePro);

                        ActivityComponentPrePro = new ActivityComponentPrePro();
                        ActivityComponentPrePro.font = new SLFont();
                        ActivityComponentPrePro.font.Bold = true;

                        ActivityComponentPrePro.PendindActivity = (_pendingActivity+ActivityComponentPreProList.Count()).ToString();
                        ActivityComponentPrePro.TypeActivity = "Despliegue";

                        ActivityComponentPrePro.rst.AppendText($"Reinicio de Pools.{ System.Environment.NewLine}", ActivityComponentPrePro.font);
                        ActivityComponentPrePro.rst.AppendText($@"• WEBNEUIVWMX03 180.181.105.137 Web{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($@"• WEBNEUIVWMX04 180.181.105.136 Web{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($@"{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($@"Reiniciar los siguientes pools:{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($@"• DIS_ecDataProvider{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($@"• DIS_eClient { System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($@"• DIS_LoginManagerService{ System.Environment.NewLine}");
                        ActivityComponentPreProList.Add(ActivityComponentPrePro);

                        //ActivityComponentPrePro.Activity = $"<b>Reinicio de Pools.</b>:{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"• WEBNEUIVWMX03 180.181.105.137 Web{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"• WEBNEUIVWMX04 180.181.105.136 Web{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"Reiniciar los siguientes pools:{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"• DIS_ecDataProvider{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"• DIS_eClient { System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"• DIS_LoginManagerService{ System.Environment.NewLine}";
                        
                    }
                    else
                    {   //PRO
                        ActivityComponentPrePro.PendindActivity = _pendingActivity.ToString();
                        ActivityComponentPrePro.TypeActivity = "Despliegue";

                        ActivityComponentPrePro.rst.AppendText($"Entrar al servidor ");
                        ActivityComponentPrePro.rst.AppendText($"SRVNEUPVWMX09 180.181.167.59 Batch ", ActivityComponentPrePro.font);
                        ActivityComponentPrePro.rst.AppendText($"Abrir como Administrador la línea de comandos{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($@"Ir a la carpeta D:\MPM\DIS\InstallBSM\{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($"Ejecutar el archivo ");
                        ActivityComponentPrePro.rst.AppendText($"Install-DIS-Procesos.cmd{ System.Environment.NewLine}", ActivityComponentPrePro.font);
                        ActivityComponentPrePro.rst.AppendText($@"Al finalizar la ejecución compartir el archivo log-import-file.log que se encuentra en D:\MPM\DIS\MPM.FullProcessImport\Logs\Import{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($"Esperar validación de los logs para continuar con la siguiente actividad.{ System.Environment.NewLine}");
                        ActivityComponentPreProList.Add(ActivityComponentPrePro);

                        //ActivityComponentPrePro.Activity = $"En el servidor <b>SRVNEUPVWMX09 180.181.167.59 Batch</b>:{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"Abrir como Administrador la línea de comandos{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"Ir a la carpeta D:\MPM\DIS\InstallBSM\{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"Ejecutar el archivo <b>Install-DIS-Procesos.cmd</b>{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"Al finalizar la ejecución compartir el archivo log-import-file.log que se encuentra en D:\MPM\DIS\MPM.FullProcessImport\Logs\Import{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"Esperar validación de los logs para continuar con la siguiente actividad.{ System.Environment.NewLine}";
                        //ActivityComponentPreProList.Add(ActivityComponentPrePro);

                        ActivityComponentPrePro = new ActivityComponentPrePro();
                        ActivityComponentPrePro.font = new SLFont();
                        ActivityComponentPrePro.font.Bold = true;

                        ActivityComponentPrePro.PendindActivity = (_pendingActivity + ActivityComponentPreProList.Count()).ToString();
                        ActivityComponentPrePro.TypeActivity = "Despliegue";

                        ActivityComponentPrePro.rst.AppendText($"Reinicio de Pools.{ System.Environment.NewLine}", ActivityComponentPrePro.font);
                        ActivityComponentPrePro.rst.AppendText($@"• WEBNEUPVWMX03 180.181.165.93 Web{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($@"• WEBNEUPVWMX04 180.181.165.97 Web{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($@"{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($@"Reiniciar los siguientes pools:{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($@"• DIS_ecDataProvider{ System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($@"• DIS_eClient { System.Environment.NewLine}");
                        ActivityComponentPrePro.rst.AppendText($@"• DIS_LoginManagerService{ System.Environment.NewLine}");
                        ActivityComponentPreProList.Add(ActivityComponentPrePro);

                        //ActivityComponentPrePro.Activity = $"<b>Reinicio de Pools.</b>:{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"• WEBNEUPVWMX03 180.181.165.93 Web{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"• WEBNEUPVWMX04 180.181.165.97 Web{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"Reiniciar los siguientes pools:{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"• DIS_ecDataProvider{ System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"• DIS_eClient { System.Environment.NewLine}";
                        //ActivityComponentPrePro.Activity += $@"• DIS_LoginManagerService{ System.Environment.NewLine}";
                        //ActivityComponentPreProList.Add(ActivityComponentPrePro);
                    }
                }
                
                
                return ActivityComponentPreProList;
            });
            task0.Start();


            return await task0;
        }

        //manejo de pools (prender y apagar)
        public static async Task<List<ActivityComponentPrePro>> ListPoolManager(int _env,int _pendingActivity)
        {

            Task<List<ActivityComponentPrePro>> task0 = new Task<List<ActivityComponentPrePro>>(() =>
            {
                List<ActivityComponentPrePro> ActivityComponentPreProList = new List<ActivityComponentPrePro>();
                ActivityComponentPrePro ActivityComponentPrePro = new ActivityComponentPrePro();
                ActivityComponentPrePro.font = new SLFont();
                ActivityComponentPrePro.font.Bold = true;
                //int _contador = 1;
                if (_env == 0) //0 Pre 1 Pro
                {   //PRE
                    
                    ActivityComponentPrePro.PendindActivity = "Ninguna";
                    ActivityComponentPrePro.TypeActivity = "Despliegue";
                    ActivityComponentPrePro.rst.AppendText($"Bajar pools ", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($"de todos los servidores.");
                    //ActivityComponentPrePro.Activity = $@"<b>Bajar pools</b> de todos los servidores.";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);

                    ActivityComponentPrePro = new ActivityComponentPrePro();
                    ActivityComponentPrePro.font = new SLFont();
                    ActivityComponentPrePro.font.Bold = true;

                    ActivityComponentPrePro.PendindActivity = $"{_pendingActivity} y {_pendingActivity + _pendingActivity + 1 }";
                    ActivityComponentPrePro.TypeActivity = "Despliegue";
                    ActivityComponentPrePro.rst.AppendText($@"Desplegar las siguientes CRs:{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"Alojables", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"Configurables", ActivityComponentPrePro.font);
                    //ActivityComponentPrePro.Activity = $@"Desplegar las siguientes CRs:{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"<b>Alojables</b>{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"<b>Configurables</b>{ System.Environment.NewLine}";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);

                    int _actividadPending = _pendingActivity + ActivityComponentPreProList.Count();
                    ActivityComponentPrePro = new ActivityComponentPrePro();
                    ActivityComponentPrePro.font = new SLFont();
                    ActivityComponentPrePro.font.Bold = true;

                    ActivityComponentPrePro.PendindActivity = $"{_actividadPending}";
                    ActivityComponentPrePro.TypeActivity = "Despliegue";
                    ActivityComponentPrePro.rst.AppendText($@"En el servidor WEBNEUIVWMX03 con dirección ip 180.181.105.137, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"dir /s *.* /o:-d > webn03_caida.txt", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Entregar al Gestor del cambio el archivo webn03_caida.txt.{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}");
                    //ActivityComponentPrePro.Activity = $@"En el servidor WEBNEUIVWMX03 con dirección ip 180.181.105.137, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"<b>dir /s *.* /o:-d > webn03_caida.txt</b>{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Entregar al Gestor del cambio el archivo webn03_caida.txt.{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);

                    ActivityComponentPrePro = new ActivityComponentPrePro();
                    ActivityComponentPrePro.font = new SLFont();
                    ActivityComponentPrePro.font.Bold = true;

                    ActivityComponentPrePro.PendindActivity = $"{_actividadPending}";
                    ActivityComponentPrePro.TypeActivity = "Despliegue";
                    ActivityComponentPrePro.rst.AppendText($@"En el servidor SRNEUIWM1MXR309 con dirección ip 180.228.64.204, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"dir /s *.* /o:-d > srvn09_caida.txt", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Entregar al Gestor del cambio el archivo srvn09_caida.txt.{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}");
                    //ActivityComponentPrePro.Activity = $@"En el servidor SRNEUIWM1MXR309 con dirección ip 180.228.64.204, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"<b>dir /s *.* /o:-d > srvn09_caida.txt</b>{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Entregar al Gestor del cambio el archivo srvn09_caida.txt.{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);

                    ActivityComponentPrePro = new ActivityComponentPrePro();
                    ActivityComponentPrePro.font = new SLFont();
                    ActivityComponentPrePro.font.Bold = true;

                    ActivityComponentPrePro.PendindActivity = $"{_actividadPending}";
                    ActivityComponentPrePro.TypeActivity = "Despliegue";
                    ActivityComponentPrePro.rst.AppendText($@"En el servidor SRVNEUIVWMX01  con dirección ip 180.181.105.139, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"dir /s *.* /o:-d > srvn01_caida.txt", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Entregar al Gestor del cambio el archivo srvn01_caida.txt.{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}");
                    //ActivityComponentPrePro.Activity = $@"En el servidor SRVNEUIVWMX01  con dirección ip 180.181.105.139, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"<b>dir /s *.* /o:-d > srvn01_caida.txt</b>{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Entregar al Gestor del cambio el archivo srvn01_caida.txt.{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);

                    ActivityComponentPrePro = new ActivityComponentPrePro();
                    ActivityComponentPrePro.font = new SLFont();
                    ActivityComponentPrePro.font.Bold = true;

                    ActivityComponentPrePro.PendindActivity = $"{_actividadPending}";
                    ActivityComponentPrePro.TypeActivity = "Despliegue";
                    ActivityComponentPrePro.rst.AppendText($@"En el servidor SRNEUIWM1MXR307  con dirección ip 180.228.64.206, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"dir /s *.* /o:-d > srvn07_caida.txt", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Entregar al Gestor del cambio el archivo srvn07_caida.txt.{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}");
                    //ActivityComponentPrePro.Activity = $@"En el servidor SRNEUIWM1MXR307  con dirección ip 180.228.64.206, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"<b>dir /s *.* /o:-d > srvn07_caida.txt</b>{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Entregar al Gestor del cambio el archivo srvn07_caida.txt.{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);

                    ActivityComponentPrePro = new ActivityComponentPrePro();
                    ActivityComponentPrePro.font = new SLFont();
                    ActivityComponentPrePro.font.Bold = true;

                    ActivityComponentPrePro.PendindActivity = (_pendingActivity+ActivityComponentPreProList.Count()).ToString();
                    ActivityComponentPrePro.TypeActivity = "Despliegue";
                    ActivityComponentPrePro.rst.AppendText($"Prender pools ", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"de todos los servidores.{ System.Environment.NewLine}");
                    //ActivityComponentPrePro.Activity = $@"<b>Prender pools</b> de todos los servidores.";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);
                }
                else
                {   //PRO
                   
                    ActivityComponentPrePro.PendindActivity = "Ninguna";
                    ActivityComponentPrePro.TypeActivity = "Despliegue";
                    ActivityComponentPrePro.rst.AppendText($"Bajar pools ", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($"de todos los servidores.");
                    //ActivityComponentPrePro.Activity = $@"<b>Bajar pools</b> de todos los servidores.";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);

                    ActivityComponentPrePro = new ActivityComponentPrePro();
                    ActivityComponentPrePro.font = new SLFont();
                    ActivityComponentPrePro.font.Bold = true;

                    ActivityComponentPrePro.PendindActivity = $"{_pendingActivity} y {_pendingActivity + 1 }";
                    ActivityComponentPrePro.TypeActivity = "Despliegue";
                    ActivityComponentPrePro.rst.AppendText($@"Desplegar las siguientes CRs:{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"Alojables", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"Configurables", ActivityComponentPrePro.font);
                    //ActivityComponentPrePro.Activity = $@"Desplegar las siguientes CRs:{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"<b>Alojables</b>{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"<b>Configurables</b>{ System.Environment.NewLine}";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);

                    int _actividadPending = _pendingActivity + ActivityComponentPreProList.Count();
                    ActivityComponentPrePro = new ActivityComponentPrePro();
                    ActivityComponentPrePro.font = new SLFont();
                    ActivityComponentPrePro.font.Bold = true;

                    ActivityComponentPrePro.PendindActivity = $"{_actividadPending}";
                    ActivityComponentPrePro.TypeActivity = "Despliegue";
                    ActivityComponentPrePro.rst.AppendText($@"En el servidor WEBNEUPVWMX03 con dirección ip 180.181.165.93, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"dir /s *.* /o:-d > webn03_caida.txt", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Entregar al Gestor del cambio el archivo webn03_caida.txt.{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}");
                    //ActivityComponentPrePro.Activity = $@"En el servidor WEBNEUPVWMX03 con dirección ip 180.181.165.93, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"<b>dir /s *.* /o:-d > webn03_caida.txt</b>{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Entregar al Gestor del cambio el archivo webn03_caida.txt.{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);

                    ActivityComponentPrePro = new ActivityComponentPrePro();
                    ActivityComponentPrePro.font = new SLFont();
                    ActivityComponentPrePro.font.Bold = true;

                    ActivityComponentPrePro.PendindActivity = $"{_actividadPending}";
                    ActivityComponentPrePro.TypeActivity = "Despliegue";
                    ActivityComponentPrePro.rst.AppendText($@"En el servidor SRVNEUPVWMX09 con dirección ip 180.181.167.59, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"dir /s *.* /o:-d > srvn09_caida.txt", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Entregar al Gestor del cambio el archivo srvn09_caida.txt.{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}");
                    //ActivityComponentPrePro.Activity = $@"En el servidor SRVNEUPVWMX09 con dirección ip 180.181.167.59, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"<b>dir /s *.* /o:-d > srvn09_caida.txt</b>{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Entregar al Gestor del cambio el archivo srvn09_caida.txt.{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);

                    ActivityComponentPrePro = new ActivityComponentPrePro();
                    ActivityComponentPrePro.font = new SLFont();
                    ActivityComponentPrePro.font.Bold = true;

                    ActivityComponentPrePro.PendindActivity = $"{_actividadPending}";
                    ActivityComponentPrePro.TypeActivity = "Despliegue";

                    ActivityComponentPrePro.rst.AppendText($@"En el servidor SRVNEUPVWMX01  con dirección ip 180.181.167.51, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"dir /s *.* /o:-d > srvn01_caida.txt", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Entregar al Gestor del cambio el archivo srvn01_caida.txt.{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}");

                    //ActivityComponentPrePro.Activity = $@"En el servidor SRVNEUPVWMX01  con dirección ip 180.181.167.51, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"<b>dir /s *.* /o:-d > srvn01_caida.txt</b>{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Entregar al Gestor del cambio el archivo srvn01_caida.txt.{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);

                    ActivityComponentPrePro = new ActivityComponentPrePro();
                    ActivityComponentPrePro.font = new SLFont();
                    ActivityComponentPrePro.font.Bold = true;

                    ActivityComponentPrePro.PendindActivity = $"{_actividadPending}";
                    ActivityComponentPrePro.TypeActivity = "Despliegue";

                    ActivityComponentPrePro.rst.AppendText($@"En el servidor SRVNEUPVWMX07  con dirección ip 180.181.167.57, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"dir /s *.* /o:-d > srvn07_caida.txt", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Entregar al Gestor del cambio el archivo srvn07_caida.txt.{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}");
                    //ActivityComponentPrePro.Activity = $@"En el servidor SRVNEUPVWMX07  con dirección ip 180.181.167.57, abrir la línea de comandos y dirigirse a D:\MPM\DIS para ejecutar el comando:{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"<b>dir /s *.* /o:-d > srvn07_caida.txt</b>{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Entregar al Gestor del cambio el archivo srvn07_caida.txt.{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Esperar validación para continuar con la siguiente actividad.{ System.Environment.NewLine}";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);

                    ActivityComponentPrePro = new ActivityComponentPrePro();
                    ActivityComponentPrePro.font = new SLFont();
                    ActivityComponentPrePro.font.Bold = true;

                    ActivityComponentPrePro.PendindActivity = (_pendingActivity+ActivityComponentPreProList.Count()).ToString();
                    ActivityComponentPrePro.TypeActivity = "Despliegue";
                    ActivityComponentPrePro.rst.AppendText($"Prender pools ", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"de todos los servidores.{ System.Environment.NewLine}");
                    //ActivityComponentPrePro.Activity = $@"<b>Prender pools</b> de todos los servidores.";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);
                }

                return ActivityComponentPreProList;
            });
            task0.Start();


            return await task0;
        }

        //(importación de recursos)
        public static async Task<List<ActivityComponentPrePro>> ListResoruceImport(int _env, int _pendingActivity)
        {

            Task<List<ActivityComponentPrePro>> task0 = new Task<List<ActivityComponentPrePro>>(() =>
            {
                List<ActivityComponentPrePro> ActivityComponentPreProList = new List<ActivityComponentPrePro>();
                ActivityComponentPrePro ActivityComponentPrePro = new ActivityComponentPrePro();
                ActivityComponentPrePro.font = new SLFont();
                ActivityComponentPrePro.font.Bold = true;
                //int _contador = 1;

                if (_env == 0)
                {   //PRE
                    ActivityComponentPrePro.PendindActivity = _pendingActivity.ToString();
                    ActivityComponentPrePro.TypeActivity = "Despliegue";
                    ActivityComponentPrePro.rst.AppendText($@"En el servidor.{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"SRNEUIWM1MXR309 180.228.64.204 Batch:", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Abrir como Administrador la línea de comandos { System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Ir a la carpeta D:\MPM\DIS\InstallBSM\Resources\ImportarRecursos { System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Ejecutar el archivo { System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"ImportarRecursos_1.cmd ", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"y al finalizar devolver: { System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"-El archivo cookies.txt ubicado en D:\MPM\DIS\InstallBSM\Resources\ImportarRecursos { System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"-El archivo salida1_DDMMAAAA.html { System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"-El archivo salida1_errores_DDMMAAAA.log { System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Esperar revisión de los logs de esta tarea para continuar con la siguiente actividad. { System.Environment.NewLine}");
                    //ActivityComponentPrePro.Activity = $"En el servidor <b>SRNEUIWM1MXR309 180.228.64.204 Batch</b>:{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Abrir como Administrador la línea de comandos{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Ir a la carpeta D:\MPM\DIS\InstallBSM\Resources\ImportarRecursos{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Ejecutar el archivo <b>ImportarRecursos_1.cmd</b> y al finalizar devolver:{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"-El archivo cookies.txt ubicado en D:\MPM\DIS\InstallBSM\Resources\ImportarRecursos{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"-El archivo salida1_DDMMAAAA.html{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"-El archivo salida1_errores_DDMMAAAA.log{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Esperar revisión de los logs de esta tarea para continuar con la siguiente actividad.{ System.Environment.NewLine}";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);
                }
                else
                {   //PRO
                    ActivityComponentPrePro.PendindActivity = _pendingActivity.ToString();
                    ActivityComponentPrePro.TypeActivity = "Despliegue";
                    ActivityComponentPrePro.rst.AppendText($@"En el servidor.{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"SRVNEUPVWMX09 180.181.167.59 Batch:", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"{ System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Abrir como Administrador la línea de comandos { System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Ir a la carpeta D:\MPM\DIS\InstallBSM\Resources\ImportarRecursos { System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Ejecutar el archivo { System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($"ImportarRecursos_1.cmd ", ActivityComponentPrePro.font);
                    ActivityComponentPrePro.rst.AppendText($@"y al finalizar devolver: { System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"-El archivo cookies.txt ubicado en D:\MPM\DIS\InstallBSM\Resources\ImportarRecursos { System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"-El archivo salida1_DDMMAAAA.html { System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"-El archivo salida1_errores_DDMMAAAA.log { System.Environment.NewLine}");
                    ActivityComponentPrePro.rst.AppendText($@"Esperar revisión de los logs de esta tarea para continuar con la siguiente actividad. { System.Environment.NewLine}");
                    //ActivityComponentPrePro.Activity = $"En el servidor <b>SRVNEUPVWMX09 180.181.167.59 Batch</b>:{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Abrir como Administrador la línea de comandos{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Ir a la carpeta D:\MPM\DIS\InstallBSM\Resources\ImportarRecursos{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Ejecutar el archivo <b>ImportarRecursos_1.cmd</b> y al finalizar devolver:{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"-El archivo cookies.txt ubicado en D:\MPM\DIS\InstallBSM\Resources\ImportarRecursos{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"-El archivo salida1_DDMMAAAA.html{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"-El archivo salida1_errores_DDMMAAAA.log{ System.Environment.NewLine}";
                    //ActivityComponentPrePro.Activity += $@"Esperar revisión de los logs de esta tarea para continuar con la siguiente actividad.{ System.Environment.NewLine}";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);
                }
                


                return ActivityComponentPreProList;
            });
            task0.Start();


            return await task0;
        }

        //EndExcel
        public static async Task<List<ActivityComponentPrePro>> ListEndActivity(int _env, int _pendingActivity)
        {

            Task<List<ActivityComponentPrePro>> task0 = new Task<List<ActivityComponentPrePro>>(() =>
            {
                List<ActivityComponentPrePro> ActivityComponentPreProList = new List<ActivityComponentPrePro>();
                ActivityComponentPrePro ActivityComponentPrePro = new ActivityComponentPrePro();
                ActivityComponentPrePro.font = new SLFont();
                ActivityComponentPrePro.font.Bold = true;
                //int _contador = 1;
                ActivityComponentPrePro.PendindActivity = _pendingActivity.ToString();
                ActivityComponentPrePro.TypeActivity = "Despliegue";
                ActivityComponentPrePro.rst.AppendText($"Generar fingerprint { System.Environment.NewLine}", ActivityComponentPrePro.font);
                //ActivityComponentPrePro.Activity = $"<b>Generar fingerprint</b>{ System.Environment.NewLine}";
                ActivityComponentPreProList.Add(ActivityComponentPrePro);

                ActivityComponentPrePro = new ActivityComponentPrePro();
                ActivityComponentPrePro.font = new SLFont();
                ActivityComponentPrePro.font.Bold = true;

                ActivityComponentPrePro.PendindActivity = (_pendingActivity + ActivityComponentPreProList.Count()).ToString();
                ActivityComponentPrePro.TypeActivity = "Despliegue";
                ActivityComponentPrePro.rst.AppendText($"Solicitar logs { System.Environment.NewLine}", ActivityComponentPrePro.font);
                //ActivityComponentPrePro.Activity = $"<b>Solicitar logs</b>{ System.Environment.NewLine}";
                ActivityComponentPreProList.Add(ActivityComponentPrePro);

                if (_env == 0)
                {   //PRE
                    

                    ActivityComponentPrePro = new ActivityComponentPrePro();
                    ActivityComponentPrePro.font = new SLFont();
                    ActivityComponentPrePro.font.Bold = true;

                    ActivityComponentPrePro.PendindActivity = "";
                    ActivityComponentPrePro.TypeActivity = "";
                    ActivityComponentPrePro.rst.AppendText($"Post despliegue. { System.Environment.NewLine}", ActivityComponentPrePro.font);
                    //ActivityComponentPrePro.Activity = $"<b>Post despliegue.</b>{ System.Environment.NewLine}";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);
                }
                else
                {   //PRO
                    ActivityComponentPrePro = new ActivityComponentPrePro();
                    ActivityComponentPrePro.font = new SLFont();
                    ActivityComponentPrePro.font.Bold = true;

                    ActivityComponentPrePro.PendindActivity = "";
                    ActivityComponentPrePro.TypeActivity = "";
                    ActivityComponentPrePro.rst.AppendText($"Creación del producto y expedientes de hogar. { System.Environment.NewLine}", ActivityComponentPrePro.font);
                    //ActivityComponentPrePro.Activity = $"<b>Creación del producto y expedientes de hogar.</b>{ System.Environment.NewLine}";
                    ActivityComponentPreProList.Add(ActivityComponentPrePro);
                }

                return ActivityComponentPreProList;
            });
            task0.Start();


            return await task0;
        }
    }
    
}

