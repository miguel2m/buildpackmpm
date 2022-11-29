using AutoDischange.Model;
using AutoDischange.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoDischange.ViewModel
{
    public class MainVM 
    {

        public MainVM()
        {

            
            //try
            //{
            //    DISChangeRequest.DischangeGraphClientAsync();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    MessageBox.Show("Error al intentar conectar con el OneDrive: " + ex.Message, "Error al intentar conectar con el OneDrive", MessageBoxButton.OK, MessageBoxImage.Error);
            //};
            //DISChangeRequest.DischangeGraphClientAsync();
            //ExcelHelper.ReadExcelDIS_Changes();
            //SyncDischange();

        }

        public async void SyncDischange()
        {
            //DatabaseHelper.Insert(ExcelHelper.ReadExcelDIS_Changes());
            //if (await ExcelHelper.ReadExcelDIS_Changes()) {
            //    var notebooks = (DatabaseHelper.Read<DischangePath>()).Where(n => n.Path.Contains("JScript.js")).ToList();
            //    notebooks.ForEach(p => Console.WriteLine(p.Path));
            //};
            //try
            //{
            //    string dbFile = Path.Combine(Environment.CurrentDirectory, "dischangesPath.db3");
            //    //Console.WriteLine(File.Exists(dbFile));
            //    if (File.Exists(dbFile) == false)
            //    {
            //        if (await ExcelHelper.ReadExcelDIS_Changes())
            //        {
            //            var notebooks = (DatabaseHelper.Read<DischangePath>()).Where(n => n.Path.Contains("pt")).ToList();
            //            notebooks.ForEach(p => Console.WriteLine(p.Path));
            //        };
            //    }
            //    //Console.WriteLine(data.Path);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            


        }

    }
}
