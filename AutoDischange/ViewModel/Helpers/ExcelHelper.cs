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
            List<DischangeChangeset> DischangeChangesets = new List<DischangeChangeset>() ;

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

            return DischangeChangesets;
        }
    }
}
