using AutoDischange.Model;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoDischange.ViewModel.Helpers
{
    public class ExcelHelper
    {

        //READ EXCEL
        public static List<DischangeChangeset> ReadExcel (string path) 
        {
            List<DischangeChangeset> DischangeChangesets = new List<DischangeChangeset>();
            try
            {
                

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en Excel: " + ex.Message, "Error en Excel", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            return DischangeChangesets;


        }
    }
}
