using AutoDischange.Model;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

namespace AutoDischange.ViewModel.Helpers
{
    public class ExcelHelper
    {

        //READ EXCEL
        public static async Task<List<DischangeChangeset>> ReadExcel (string path) 
        {
            List<DischangeChangeset> DischangeChangesets = new List<DischangeChangeset>();
               
            SLDocument sl = new SLDocument(path);

            //borramos la tabla para que al consultarla no tenga registros antiguos 
            await DatabaseHelper.Delete();

            int iRow = 2;
            while (!string.IsNullOrEmpty(sl.GetCellValueAsString(iRow, 1)))
            {

                DischangeChangeset DischangeChangeset = new DischangeChangeset
                {
                    Id = iRow,
                    Changeset = sl.GetCellValueAsString(iRow, 1),
                    Branch = sl.GetCellValueAsString(iRow, 2),
                };

                DischangeChangesets.Add(DischangeChangeset);
                await DatabaseHelper.InsertReplaceChangeset(DischangeChangeset);
                iRow++;

            }            
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

        //Create excel aand insert worksheet
        public static void CreateExcelDiffComapre(List<DiffCompareModel> diffCompareModelList,string pathUser, DiffComponent diffComponent)
        {
            string rtfFile = System.IO.Path.Combine(pathUser, $"InformeComparacionComponentes_{Math.Abs(diffCompareModelList.GetHashCode())}.xlsx");


            //bool result = false;
            //List<DischangePath> DischangeChangesets = new List<DischangePath>();


            SLDocument sl = new SLDocument();

            sl.SetCellValue("B1", "PAQUETE 1");
            sl.SetCellValue("H1", "PAQUETE 2");
            sl.SetCellValue("N1", "Evaluaciones");
            // merge all cells in the cell range B2:G8
            sl.MergeWorksheetCells("B1", "G1");
            sl.MergeWorksheetCells("H1", "M1");
            sl.MergeWorksheetCells("N1", "P1");

            //SET Header
            sl.SetCellValue(2, 1, "Contador");
            //SET Header Paquete A
            sl.SetCellValue(2, 2, "Ubicación");
            sl.SetCellValue(2, 3, diffComponent.PathStart);
            sl.SetCellValue(2, 4, "Hash");
            sl.SetCellValue(2, 5, "Fecha Modificación");
            sl.SetCellValue(2, 6, "Hora");
            sl.SetCellValue(2, 7, "Tamaño");
            //SET Header Paquete B
            sl.SetCellValue(2, 8, "Ubicación");
            sl.SetCellValue(2, 9, diffComponent.PathEnd);
            sl.SetCellValue(2, 10, "Hash");
            sl.SetCellValue(2, 11, "Fecha Modificación");
            sl.SetCellValue(2, 12, "Hora");
            sl.SetCellValue(2, 13, "Tamaño");
            //SET Header Evaluaciones
            sl.SetCellValue(2, 14, "Hash");
            sl.SetCellValue(2, 15, "Fecha");
            sl.SetCellValue(2, 16, "Tamaño");

            SLStyle styleFecha = sl.CreateStyle();

            
            //  styleFecha.Alignment.Indent = 5;
            styleFecha.Alignment.JustifyLastLine = true;
            styleFecha.Alignment.ReadingOrder = SLAlignmentReadingOrderValues.RightToLeft;
            styleFecha.Alignment.ShrinkToFit = true;
            styleFecha.FormatCode = "dd/mm/yyyy";

            SLStyle styleHora = sl.CreateStyle();


            //  styleFecha.Alignment.Indent = 5;
            styleHora.Alignment.JustifyLastLine = true;
            styleHora.Alignment.ReadingOrder = SLAlignmentReadingOrderValues.RightToLeft;
            styleHora.Alignment.ShrinkToFit = true;
            styleHora.FormatCode = "h:m:s";
            styleHora.SetVerticalAlignment(VerticalAlignmentValues.Center);
            //styleFecha.SetWrapText(true);

            SLStyle styleNumber = sl.CreateStyle();


            //  styleFecha.Alignment.Indent = 5;
            styleNumber.Alignment.JustifyLastLine = true;
            styleNumber.Alignment.ReadingOrder = SLAlignmentReadingOrderValues.RightToLeft;
            styleNumber.Alignment.ShrinkToFit = true;
            styleNumber.FormatCode = "#,##0.00";
            styleNumber.SetWrapText(true);
            styleNumber.SetVerticalAlignment(VerticalAlignmentValues.Center);


            SLStyle styleRed = sl.CreateStyle();
            styleRed.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Red, System.Drawing.Color.DarkSalmon);

            SLStyle styleColor = sl.CreateStyle();
            
            int _index = 3;
            foreach (DiffCompareModel item in diffCompareModelList)
            {
                
                
                //SET Header
                sl.SetCellValue(_index, 1, item.Id);
                //SET Header Paquete A
                sl.SetCellValue(_index, 2, item.UbicacionA);
                sl.SetCellValue(_index, 3, item.PathA);
                sl.SetCellValue(_index, 4, item.HashA);

                sl.SetCellValue(_index, 5, item.FechaA);
                sl.SetCellStyle(_index, 5, styleFecha);
                sl.SetCellValue(_index, 6, item.FechaA);
                sl.SetCellStyle(_index, 6, styleHora);
                sl.SetCellValue(_index, 7, item.LenghtA);
                //SET Header Paquete B
                sl.SetCellValue(_index, 8, item.UbicacionB);
                sl.SetCellValue(_index, 9, item.PathB);
                sl.SetCellValue(_index, 10, item.HashB);

                sl.SetCellValue(_index, 11, item.FechaB);
                sl.SetCellStyle(_index, 11, styleFecha);
                sl.SetCellValue(_index, 12, item.FechaB);
                sl.SetCellStyle(_index, 12, styleHora);
                sl.SetCellValue(_index, 13, item.LenghtB);
                //SET Header Evaluaciones
                
                switch (item.HashResult)
                {
                    case 1://Iguales
                        // code block
                        sl.SetCellValue(_index, 14, "Iguales");
                        styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGreen, System.Drawing.Color.DarkSalmon);
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 2:// Iguales
                        sl.SetCellValue(_index, 14, "Iguales");
                        styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        sl.SetRowStyle(_index, styleColor);
                        // code block
                        break;
                    case 3://A es mayor B
                        // code block
                        sl.SetCellValue(_index, 14, "Diferentes");
                        styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 4://B es mayor A
                        // code block
                        sl.SetCellValue(_index, 14, "Diferentes");
                        styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 5: // Diferentes
                        // code block
                        sl.SetCellValue(_index, 16, "El componenete solo existe en el paquete 1");
                        styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Red, System.Drawing.Color.DarkSalmon);
                        sl.SetRowStyle(_index, styleColor);
                        break;
                }
                switch (item.FechaResult)
                {
                    case 1://Iguales
                        // code block
                        sl.SetCellValue(_index, 15, "Iguales");
                        styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGreen, System.Drawing.Color.DarkSalmon);
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 2:// Iguales
                        sl.SetCellValue(_index, 15, "Iguales");
                        styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        sl.SetRowStyle(_index, styleColor);
                        // code block
                        break;
                    case 3://A es mayor B
                        // code block
                        sl.SetCellValue(_index, 15, "El paquete 1 es más actual que el paquete 2");
                        styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 4://B es mayor A
                        // code block
                        sl.SetCellValue(_index, 15, "El paquete 2 es más actual que el paquete 1");
                        styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 5: // Diferentes
                        // code block
                        sl.SetCellValue(_index, 16, "El componenete solo existe en el paquete 1");
                        styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Red, System.Drawing.Color.DarkSalmon);
                        sl.SetRowStyle(_index, styleColor);
                        break;
                }
                switch (item.LenghtResult)
                {
                    case 1://Iguales
                        // code block
                        sl.SetCellValue(_index, 16, "Iguales");
                        styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGreen, System.Drawing.Color.DarkSalmon);
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 2:// Iguales
                        sl.SetCellValue(_index, 16, "Iguales");
                        styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        sl.SetRowStyle(_index, styleColor);
                        // code block

                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 3://A es mayor B
                        // code block
                        sl.SetCellValue(_index, 16, "El paquete 1 es mayor que el paquete 2");
                        styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 4://B es mayor A
                        // code block
                        sl.SetCellValue(_index, 16, "El paquete 2 es mayor que el paquete 1");
                        styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 5: // Solo existe en A
                        // code block
                        sl.SetCellValue(_index, 16, "El componenete solo existe en el paquete 1");
                        styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Red, System.Drawing.Color.DarkSalmon);
                        sl.SetRowStyle(_index, styleColor);
                        break;
                }


                _index++;
            }
            sl.SaveAs($"{rtfFile}");
            //return result;


        }


    }
}
