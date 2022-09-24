using AutoDischange.Model;
using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.ViewModel.Helpers
{
    public class CsvHelper
    {

        //READ Local DIS_Changes
        public static async Task<bool> ReadCSVDIS_Changes(string filePath)
        {
            //string rtfFile = System.IO.Path.Combine(Environment.CurrentDirectory, "DIS_Changes.xlsx");

            bool resultMethod = false;
            //List<DischangePath> DischangeChangesets = new List<DischangePath>();

            var engine = new FileHelperEngine<DisChangeData>();

            // To Read Use:
            var result = engine.ReadFile(filePath);
            foreach (DisChangeData item in result)
            {
                DischangePath DischangeChangeset = new DischangePath
                {
                    Path = item.PathData,
                };

                var notebooks = (DatabaseHelper.Read<DischangePath>()).Where(n => n.Path == DischangeChangeset.Path).ToList();


                if (notebooks.Count() == 0)
                {
                    resultMethod = await DatabaseHelper.InsertDischange(DischangeChangeset);
                }
                else
                {
                    DischangeChangeset.Id = notebooks[0].Id;
                    resultMethod = await DatabaseHelper.InsertReplaceDischange(DischangeChangeset);
                }
            }

      

               
            
            

            return resultMethod;


        }
    }

    [DelimitedRecord(";")]
    public class DisChangeData
    {
        public string PathData;

        public string HashData;

        public string SizeData;
        

    }
}
