using AutoDischange.Model;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.ViewModel.Helpers
{
    public class ExcelHelper
    {

        //READ EXCEL
        public static List<DischangeChangeset> ReadExcel (string path) 
        {
            List<DischangeChangeset> DischangeChangesets = new List<DischangeChangeset>();
            
               
                SLDocument sl = new SLDocument(path);


                int iRow = 2;
                while (!string.IsNullOrEmpty(sl.GetCellValueAsString(iRow, 1)))
                {

                    DischangeChangeset DischangeChangeset = new DischangeChangeset
                    {
                        Id = Guid.NewGuid().ToString(),
                        Changeset = sl.GetCellValueAsString(iRow, 1),
                    };

                    DischangeChangesets.Add(DischangeChangeset);
                    iRow++;

                }
                //resultado.Add("OK", "true");
                //resultado.Add("msg", DischangeChangesets);
                //return DischangeChangesets;
            
            return DischangeChangesets;


        }

        //READ Local DIS_Changes
        public static async Task<bool> ReadExcelDIS_Changes(string filePath)
        {
            //string rtfFile = System.IO.Path.Combine(Environment.CurrentDirectory, "DIS_Changes.xlsx");

            bool result = false;
            //List<DischangePath> DischangeChangesets = new List<DischangePath>();


            SLDocument sl = new SLDocument(filePath, "GuíaDeUbicaciones");


            int iRow = 1;
            while (!string.IsNullOrEmpty(sl.GetCellValueAsString(iRow, 1)))
            {

                DischangePath DischangeChangeset = new DischangePath
                {
                    Path = sl.GetCellValueAsString(iRow, 1).Split(';')[0],
                };

                var notebooks = (DatabaseHelper.Read<DischangePath>()).Where(n => n.Path==DischangeChangeset.Path).ToList();
               

                if (notebooks.Count()==0)
                {               
                    result = await DatabaseHelper.InsertDischange(DischangeChangeset);
                }
                else
                {
                    DischangeChangeset.Id = notebooks[0].Id;
                    result = await DatabaseHelper.InsertReplaceDischange(DischangeChangeset);
                }
                
                
               
                iRow++;

            }
            //resultado.Add("OK", "true");
            //resultado.Add("msg", DischangeChangesets);
            //return DischangeChangesets;

            return result;


        }

        
    }
}
