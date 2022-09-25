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

        public static async Task ExportActivity(ActivityComponent activityComponent, string pathUser)
        {
            char backSlash = Path.DirectorySeparatorChar;
            string pathAlojables = $"{@activityComponent.PathStart}{backSlash}Alojables";
            string pathConfigurables = $"{@activityComponent.PathStart}{backSlash}Configurables";
            string pathScript = $"{@activityComponent.PathStart}{backSlash}Scripts";

            System.IO.DirectoryInfo dirAlojables;
            System.IO.DirectoryInfo dirConfigurables ;
            System.IO.DirectoryInfo dirScript ;

            IEnumerable<System.IO.FileInfo> listAlojables;
            IEnumerable<System.IO.FileInfo> listConfigurables;
            IEnumerable<System.IO.FileInfo> listScript;

            

            List<ActivityComponentListAlojables> ActivityComponentListAlojablesList;
            List<ActivityComponentListConfigurables> ActivityComponentListConfigurablesList = new List<ActivityComponentListConfigurables>();
            List<ActivityComponentListScript> ActivityComponentListScriptList = new List<ActivityComponentListScript>();


            

            if (!Directory.Exists(pathAlojables) && !Directory.Exists(pathConfigurables) && !Directory.Exists(pathScript))
            {
                throw new DirectoryNotFoundException("Debe seleccionar carpeta de componentes");
            }

            //MemoryStream msPass = ExcelHelper.ReadExcelEntrega();
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

                    MemoryStream msPassTemp = new MemoryStream();
                    ExcelHelper.ListadoAlojables(msPass, ActivityComponentListAlojablesList).WriteTo(msPassTemp) ;
                    msPass = new MemoryStream();
                    msPassTemp.WriteTo(msPass);
                   
                }
            }
            //Listado de Configurables
            if (Directory.Exists(pathConfigurables))
            {
                dirConfigurables = new System.IO.DirectoryInfo(pathConfigurables);
                listConfigurables = dirConfigurables.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
                List<string> DischangePathList = await ListPathDischange(listConfigurables);
                ActivityComponentListConfigurablesList = await ListConfigurables(DischangePathList);
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

            ExcelHelper.SaveExcelEntrega(msPass, pathUser);
        }

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
            
            return DischangePathList;
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
                    
                    List<DischangePath> dischangePathListTemp = (DatabaseHelper.Read<DischangePath>()).Where(n => n.Path.Contains(item)).ToList(); ;
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
        public static async Task<List<ActivityComponentListConfigurables>> ListConfigurables(List<string> listInput)
        {
            
           
            Task<List<ActivityComponentListConfigurables>> task0 = new Task<List<ActivityComponentListConfigurables>>( () =>
            {
                List<ActivityComponentListConfigurables> ActivityComponentListConfigurablesList = new List<ActivityComponentListConfigurables>();
                ActivityComponentListConfigurables ActivityComponentConfigurables;
                int _contador = 1;
                foreach (string item in listInput)
                {
                   
                    List<DischangePath> dischangePathListTemp = (DatabaseHelper.Read<DischangePath>()).Where(n => n.Path.Contains(item)).ToList();
                    ActivityComponentConfigurables = new ActivityComponentListConfigurables();
                    ActivityComponentConfigurables.Id = _contador;
                    ActivityComponentConfigurables.Workbook = "ListadoConfigurables";
                    ActivityComponentConfigurables.ComponentEnv = "TEST";

                    if (dischangePathListTemp.Any())
                    {
                        //DischangePathList.Add(dischangePathListTemp.First().Path);
                        foreach (DischangePath itemTemp in dischangePathListTemp)
                        {
                            ActivityComponentConfigurables.DischangeComponentName.Add(itemTemp.Path);

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
    }
}
