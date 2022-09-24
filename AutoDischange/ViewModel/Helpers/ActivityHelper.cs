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

            List<ActivityModel> ActivityModelList = new List<ActivityModel> ();
            ActivityModel ActivityModel;
            

            if (!Directory.Exists(pathAlojables) && !Directory.Exists(pathConfigurables) && !Directory.Exists(pathScript))
            {
                throw new DirectoryNotFoundException("Debe seleccionar carpeta de componentes");
            }
            

            //Listado de Alojables
            if (Directory.Exists(pathAlojables))
            {
                dirAlojables = new System.IO.DirectoryInfo(pathAlojables);
                listAlojables = dirAlojables.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
                
                
                ActivityModel = new ActivityModel();
                ActivityModel.ListFile = await ListPathDischange(listAlojables);
                ActivityModel.Workbook = "ListadoAlojables";
                ActivityModelList.Add(ActivityModel);
                //ExcelHelper.ReadExcelEntrega(listAlojables, pathUser, "ListadoAlojables");
            }
            //Listado de Configurables
            if (Directory.Exists(pathConfigurables))
            {
                dirConfigurables = new System.IO.DirectoryInfo(pathConfigurables);
                listConfigurables = dirConfigurables.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
                ActivityModel = new ActivityModel();
                ActivityModel.ListFile = await ListPathDischange(listConfigurables);
                ActivityModel.Workbook = "ListadoConfigurables";
                ActivityModelList.Add(ActivityModel);
                //msPass = ExcelHelper.ReadExcelEntrega(msPass, listConfigurables, pathUser, "ListadoConfigurables");
            }
            //Listado de Script
            if (Directory.Exists(pathScript))
            {
                dirScript = new System.IO.DirectoryInfo(pathScript);
                listScript = dirScript.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
                ActivityModel = new ActivityModel();
                ActivityModel.ListFile = await ListPathDischange(listScript);
                ActivityModel.Workbook = "ListadoScripts";
                ActivityModelList.Add(ActivityModel);
                //msPass = ExcelHelper.ReadExcelEntrega(msPass, listScript, pathUser, "ListadoScripts");
            }

            if (ActivityModelList.Any())
            {
                //ExcelHelper.ReadExcelEntrega(listAlojables, pathUser, "ListadoAlojables");
                ExcelHelper.ReadExcelEntrega(ActivityModelList, pathUser);
            }
        }

        public static async Task<List<string>> ListPathDischange(IEnumerable<System.IO.FileInfo> listInput)
        {
            List<string> DischangePathList = new List<string>();

            Task task0 = new Task(async () =>
            {
                foreach (System.IO.FileInfo item in listInput)
                {
                    Task<List<DischangePath>> task1 = new Task<List<DischangePath>>(() =>
                    {
                        return (DatabaseHelper.Read<DischangePath>()).Where(n => n.Path.Contains(item.Name)).ToList();
                    });
                    task1.Start();
                    List<DischangePath> dischangePathListTemp = await task1;
                    if (dischangePathListTemp.Any())
                    {
                        //DischangePathList.Add(dischangePathListTemp.First().Path);
                        //foreach (DischangePath itemTemp in dischangePathListTemp)
                        //{
                        //    DischangePathList.Add(itemTemp.Path);
                        //}
                    }
                    else
                    {
                        DischangePathList.Add(item.Name);
                    }
                }
            });
            task0.Start();
            await task0;
            
            return DischangePathList;
        }
    }
}
