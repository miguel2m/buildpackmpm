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
using System.IO;
using System.Configuration;

namespace AutoDischange.ViewModel.Helpers
{
    public class ExcelHelper
    {

        //READ EXCEL Para la lista De Dischange y lo guarda en BD
        public static async Task<List<DischangeChangeset>> ReadExcel (string path) 
        {
            List<DischangeChangeset> DischangeChangesets = new List<DischangeChangeset>();


            SLDocument sl = new SLDocument(path);

            //borramos la tabla para que al consultarla no tenga registros antiguos 
            await DatabaseHelper.Delete();

            int iRow = 2;
            int iRowJenkins = 2;
            if (!string.IsNullOrEmpty(sl.GetCellValueAsString(iRowJenkins, 3)))
            {
                while (!string.IsNullOrEmpty(sl.GetCellValueAsString(iRowJenkins, 3)))
                {

                    BranchJenkinsExcel BranchJenkin = new BranchJenkinsExcel
                    {
                        Id = iRowJenkins,
                  
                        Name = sl.GetCellValueAsString(iRowJenkins, 3),
                    };
                    
                    await DatabaseHelper.InsertReplaceBranchJenkinsExcel(BranchJenkin);
                    iRowJenkins++;

                }
            }
            else
            {
                throw new Exception("Debe indicar el branch donde ubicar los componenetes del jenkis");
            }
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

        //READ Local DIS_Changes (Lee archivo la Guia de ubicaciones del DIS-Change.xls Excel)
        public static async Task<bool> ReadExcelDIS_Changes(string filePath)
        {
            bool result = false;

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
            return result;

        }

        //Inicio Comparacion de componeentes
        //Create excel aand insert worksheet for Comparacion de componeentes
        public static void CreateExcelDiffComapre(List<DiffCompareModel> diffCompareModelList,
            List<DiffCompareModel> diffCompareModelListIguales,
            List<DiffCompareModel> diffCompareModelListDiferentes,
            List<DiffCompareModel> diffCompareModelListHuerfanos,
            string pathUser,
            DiffComponent diffComponent)
        {
            string rtfFile = System.IO.Path.Combine(pathUser, $"InformeComparacionComponentes_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");


            //bool result = false;
            //List<DischangePath> DischangeChangesets = new List<DischangePath>();


            SLDocument sl = new SLDocument();
            sl.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Comparación");
            string pathA="";
            string pathB="";
            if (!string.IsNullOrEmpty(diffCompareModelList.First().UbicacionA))
            {
                string test01 = diffCompareModelList.First().UbicacionA.Contains("CR") ? diffCompareModelList.First().UbicacionA.Contains("CRs") ? diffCompareModelList.First().UbicacionA.Split(new[] { "CRs" }, StringSplitOptions.None)[0] : diffCompareModelList.First().UbicacionA.Split(new[] { "CR" }, StringSplitOptions.None)[0] : diffCompareModelList.First().UbicacionA;
                pathA = Path.GetDirectoryName(test01);
            }

            if (!string.IsNullOrEmpty(diffCompareModelList.Last().UbicacionB))
            {
                string test02 = diffCompareModelList.Last().UbicacionB.Contains("CR") ? diffCompareModelList.Last().UbicacionB.Contains("CRs") ? diffCompareModelList.Last().UbicacionB.Split(new[] { "CRs" }, StringSplitOptions.None)[0] : diffCompareModelList.Last().UbicacionB.Split(new[] { "CR" }, StringSplitOptions.None)[0] : diffCompareModelList.Last().UbicacionB;
                pathB = Path.GetDirectoryName(test02);
            }
            sl.SetCellValue("B1", "PAQUETE 1 "+ pathA);
            sl.SetCellValue("H1", "PAQUETE 2 "+ pathB);
            sl.SetCellValue("N1", "Evaluaciones");
            // merge all cells in the cell range B2:G8
            sl.MergeWorksheetCells("B1", "G1");
            sl.MergeWorksheetCells("H1", "M1");
            sl.MergeWorksheetCells("N1", "P1");

            //SET Header
            sl.SetCellValue(2, 1, "Contador");
            //SET Header Paquete A
            sl.SetCellValue(2, 2, "Ubicación");
            sl.SetCellValue(2, 3, "Componente paquete A");
            sl.SetCellValue(2, 4, "Hash");
            sl.SetCellValue(2, 5, "Fecha Modificación");
            sl.SetCellValue(2, 6, "Hora");
            sl.SetCellValue(2, 7, "Tamaño Bytes");
            //SET Header Paquete B
            sl.SetCellValue(2, 8, "Ubicación");
            sl.SetCellValue(2, 9, "Componente paquete B");
            sl.SetCellValue(2, 10, "Hash");
            sl.SetCellValue(2, 11, "Fecha Modificación");
            sl.SetCellValue(2, 12, "Hora");
            sl.SetCellValue(2, 13, "Tamaño Bytes");
            //SET Header Evaluaciones
            sl.SetCellValue(2, 14, "Hash");
            sl.SetCellValue(2, 15, "Fecha");
            sl.SetCellValue(2, 16, "Tamaño Bytes");

            SLStyle styleFecha = sl.CreateStyle();

            
            //  styleFecha.Alignment.Indent = 5;
            styleFecha.Alignment.JustifyLastLine = true;
            styleFecha.Alignment.ReadingOrder = SLAlignmentReadingOrderValues.RightToLeft;
            styleFecha.Alignment.ShrinkToFit = true;
            styleFecha.FormatCode = "dd/mm/yy";

            SLStyle styleHora = sl.CreateStyle();


            //  styleFecha.Alignment.Indent = 5;
            styleHora.Alignment.JustifyLastLine = true;
            styleHora.Alignment.ReadingOrder = SLAlignmentReadingOrderValues.RightToLeft;
            styleHora.Alignment.ShrinkToFit = true;
            styleHora.FormatCode = "hh:mm:ss";
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
                sl.SetCellValue(_index, 6, item.FechaA);

                if (item.FechaA != null)
                {
                    sl.SetCellStyle(_index, 5, styleFecha);
                    sl.SetCellStyle(_index, 6, styleHora);
                }
                if (!string.IsNullOrEmpty(item.LenghtA))
                {
                    sl.SetCellValue(_index, 7, int.Parse(item.LenghtA));
                }
                
                //SET Header Paquete B
                sl.SetCellValue(_index, 8, item.UbicacionB);
                sl.SetCellValue(_index, 9, item.PathB);
                sl.SetCellValue(_index, 10, item.HashB);

                sl.SetCellValue(_index, 11, item.FechaB);
                sl.SetCellValue(_index, 12, item.FechaB);
                if (item.FechaB != null)
                {

                    sl.SetCellStyle(_index, 11, styleFecha);
                    sl.SetCellStyle(_index, 12, styleHora);
                }
                if (!string.IsNullOrEmpty(item.LenghtB) )
                {
                    sl.SetCellValue(_index, 13, int.Parse(item.LenghtB));
                }
                
                //SET Header Evaluaciones
                
                switch (item.HashResult)
                {
                    case 1://Iguales
                        // code block
                        sl.SetCellValue(_index, 14, "Iguales");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGreen, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Black;
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 2:// Iguales
                        sl.SetCellValue(_index, 14, "Iguales");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Red;
                        sl.SetRowStyle(_index, styleColor);
                        // code block
                        break;
                    case 3://A es mayor B
                        // code block
                        sl.SetCellValue(_index, 14, "Diferentes");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Red;
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 4://B es mayor A
                        // code block
                        sl.SetCellValue(_index, 14, "Diferentes");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Red;
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 5: // Diferentes
                        // code block
                        sl.SetCellValue(_index, 14, "El componenete solo existe en el paquete 1");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Red, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Blue;
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 6: // Diferentes
                        // code block
                        sl.SetCellValue(_index, 14, "El componenete solo existe en el paquete 2");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Red, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Blue;
                        sl.SetRowStyle(_index, styleColor);
                        break;
                }
                switch (item.FechaResult)
                {
                    case 1://Iguales
                        // code block
                        sl.SetCellValue(_index, 15, "Iguales");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGreen, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Black;
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 2:// Iguales
                        sl.SetCellValue(_index, 15, "Iguales");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Red;
                        sl.SetRowStyle(_index, styleColor);
                        // code block
                        break;
                    case 3://A es mayor B
                        // code block
                        sl.SetCellValue(_index, 15, "El paquete 1 es más actual que el paquete 2");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Red;
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 4://B es mayor A
                        // code block
                        sl.SetCellValue(_index, 15, "El paquete 2 es más actual que el paquete 1");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Red;
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 5: // Diferentes
                        // code block
                        sl.SetCellValue(_index, 15, "El componenete solo existe en el paquete 1");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Red, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Blue;
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 6: // Diferentes
                        // code block
                        sl.SetCellValue(_index, 15, "El componenete solo existe en el paquete 2");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Red, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Blue;
                        sl.SetRowStyle(_index, styleColor);
                        break;
                }

                switch (item.LenghtResult)
                {
                    case 1://Iguales
                        // code block
                        sl.SetCellValue(_index, 16, "Iguales");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGreen, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Black;
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 2:// Iguales
                        sl.SetCellValue(_index, 16, "Iguales");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Red;
                        sl.SetRowStyle(_index, styleColor);
                        // code block

                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 3://A es mayor B
                        // code block
                        sl.SetCellValue(_index, 16, "El paquete 1 es mayor que el paquete 2");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Red;
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 4://B es mayor A
                        // code block
                        sl.SetCellValue(_index, 16, "El paquete 2 es mayor que el paquete 1");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Red;
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 5: // Solo existe en A
                        // code block
                        sl.SetCellValue(_index, 16, "El componenete solo existe en el paquete 1");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Red, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Blue;
                        sl.SetRowStyle(_index, styleColor);
                        break;
                    case 6: // Solo existe en B
                        // code block
                        sl.SetCellValue(_index, 16, "El componenete solo existe en el paquete 2");
                        //styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Red, System.Drawing.Color.DarkSalmon);
                        styleColor.Font.FontColor = System.Drawing.Color.Blue;
                        sl.SetRowStyle(_index, styleColor);
                        break;
                }


                _index++;
            }


            sl.AddWorksheet("Resumen");
            sl.SetCellValue(1, 1, "Total de componentes comparados");

            sl.SetCellValue(2, 1, "Total de componentes iguales");
            sl.SetCellValue(3, 1, "Total de componentes Alojables iguales");
            sl.SetCellValue(4, 1, "Total de componentes Configurables iguales");
            sl.SetCellValue(5, 1, "Total de componentes Script iguales");

            sl.SetCellValue(6, 1, "Total de componentes con diferencias");
            sl.SetCellValue(7, 1, "Total de componentes Alojables con diferencias");
            sl.SetCellValue(8, 1, "Total de componentes Configurables con diferencias");
            sl.SetCellValue(9, 1, "Total de componentes Script con diferencias");

            sl.SetCellValue(10, 1, "Total de componentes huérfanos");
            sl.SetCellValue(11, 1, "Total de componentes Alojables huérfanos paquete A");
            sl.SetCellValue(12, 1, "Total de componentes Configurables huérfanos paquete A");
            sl.SetCellValue(13, 1, "Total de componentes Script huérfanos paquete A");

            
            sl.SetCellValue(14, 1, "Total de componentes Alojables huérfanos paquete B");
            sl.SetCellValue(15, 1, "Total de componentes Configurables huérfanos paquete B");
            sl.SetCellValue(16, 1, "Total de componentes Script huérfanos paquete B");

            sl.SetCellValue(1, 2, diffCompareModelList.Count());

            sl.SetCellValue(2, 2, diffCompareModelListIguales.Count());
            sl.SetCellValue(3, 2, diffCompareModelListIguales.Where((i) => i.UbicacionA.Contains(@"\Alojables") || i.UbicacionA.Contains(@"\alojables")).Count());
            sl.SetCellValue(4, 2, diffCompareModelListIguales.Where((i) => i.UbicacionA.Contains(@"\Configurables") || i.UbicacionA.Contains(@"\configurables")).Count());
            sl.SetCellValue(5, 2, diffCompareModelListIguales.Where((i) => i.UbicacionA.Contains(@"\Scripts") || i.UbicacionA.Contains(@"\scripts")).Count());

            sl.SetCellValue(6, 2, diffCompareModelListDiferentes.Count());
            sl.SetCellValue(7, 2, diffCompareModelListDiferentes.Where((i) => i.UbicacionA.Contains(@"\Alojables") || i.UbicacionA.Contains("alojables")).Count());
            sl.SetCellValue(8, 2, diffCompareModelListDiferentes.Where((i) => i.UbicacionA.Contains(@"\Configurables") || i.UbicacionA.Contains("configurables")).Count());
            sl.SetCellValue(9, 2, diffCompareModelListDiferentes.Where((i) => i.UbicacionA.Contains(@"\Scripts") || i.UbicacionA.Contains("scripts")).Count());

            
            sl.SetCellValue(10, 2, diffCompareModelListHuerfanos.Count());
            sl.SetCellValue(11, 2, diffCompareModelListHuerfanos.Where((i) => i.UbicacionA.Contains(@"\Alojables") || i.UbicacionA.Contains(@"\alojables")).Count());
            sl.SetCellValue(12, 2, diffCompareModelListHuerfanos.Where((i) => i.UbicacionA.Contains(@"\Configurables") || i.UbicacionA.Contains(@"\configurables")).Count());
            sl.SetCellValue(13, 2, diffCompareModelListHuerfanos.Where((i) => i.UbicacionA.Contains(@"\Scripts") || i.UbicacionA.Contains(@"\scripts")).Count());

           
            sl.SetCellValue(14, 2, diffCompareModelListHuerfanos.Where((i) => i.UbicacionB.Contains(@"\Alojables") || i.UbicacionB.Contains(@"\alojables")).Count());
            sl.SetCellValue(15, 2, diffCompareModelListHuerfanos.Where((i) => i.UbicacionB.Contains(@"\Configurables") || i.UbicacionB.Contains(@"\configurables")).Count());
            sl.SetCellValue(16, 2, diffCompareModelListHuerfanos.Where((i) => i.UbicacionB.Contains(@"\Scripts") || i.UbicacionB.Contains(@"\scripts")).Count());


            //styleColor.Alignment.JustifyLastLine = true;
            //styleColor.Alignment.ReadingOrder = SLAlignmentReadingOrderValues.RightToLeft;
            //styleColor.Alignment.ShrinkToFit = true;
            //styleColor.SetVerticalAlignment(VerticalAlignmentValues.Center);
            styleColor.Font.FontColor = System.Drawing.Color.Black;
            sl.SetRowStyle(2, styleColor);
            styleColor.Font.FontColor = System.Drawing.Color.Black;
            sl.SetRowStyle(3, styleColor);
            styleColor.Font.FontColor = System.Drawing.Color.Black;
            sl.SetRowStyle(4, styleColor);
            styleColor.Font.FontColor = System.Drawing.Color.Black;
            sl.SetRowStyle(5, styleColor);

            styleColor.Font.FontColor = System.Drawing.Color.Red;
            sl.SetRowStyle(6, styleColor);
            styleColor.Font.FontColor = System.Drawing.Color.Red;
            sl.SetRowStyle(7, styleColor);
            styleColor.Font.FontColor = System.Drawing.Color.Red;
            sl.SetRowStyle(8, styleColor);
            styleColor.Font.FontColor = System.Drawing.Color.Red;
            sl.SetRowStyle(9, styleColor);

            styleColor.Font.FontColor = System.Drawing.Color.Blue;
            sl.SetRowStyle(10, styleColor);
            styleColor.Font.FontColor = System.Drawing.Color.Blue;
            sl.SetRowStyle(11, styleColor);
            styleColor.Font.FontColor = System.Drawing.Color.Blue;
            sl.SetRowStyle(12, styleColor);
            styleColor.Font.FontColor = System.Drawing.Color.Blue;
            sl.SetRowStyle(13, styleColor);

            styleColor.Font.FontColor = System.Drawing.Color.Blue;
            sl.SetRowStyle(14, styleColor);
            styleColor.Font.FontColor = System.Drawing.Color.Blue;
            sl.SetRowStyle(15, styleColor);
            styleColor.Font.FontColor = System.Drawing.Color.Blue;
            sl.SetRowStyle(16, styleColor);

            sl.SaveAs($"{rtfFile}");
            //return result;


        }
        
        public static MemoryStream ReadExcelEntrega()
        {

            //string rtfFile = System.IO.Path.Combine(DatabaseHelper.dbFile, "ExcelEntrega.xlsx");
            string rtfFile = System.IO.Path.Combine(Environment.CurrentDirectory, "ExcelEntrega.xlsx");
            FileStream fs = new FileStream(rtfFile, FileMode.Open);
            MemoryStream msPass = new MemoryStream();
            SLDocument slOriginal = new SLDocument(fs);
            slOriginal.SaveAs(msPass);
            fs.Close();
            return msPass;

        }

        //Set ListadoAlojables
        public static MemoryStream ListadoAlojables(MemoryStream msPass, List<ActivityComponentListAlojables>  listData)
        {
            msPass.Position = 0;
            MemoryStream msPassTemp = new MemoryStream();
            SLDocument sl = new SLDocument(msPass);
            sl.SelectWorksheet("ListadoAlojables");
            int _index = 2;
            int _contador = 1;
            List<string> ListListadoAlojables = new List<string>();
            foreach (ActivityComponentListAlojables item in listData)
            {


                //sl.SetCellValue(_index, 1, item.Id);
                foreach (string item2 in item.DischangeComponentName)
                {
                    if (!ListListadoAlojables.Any(a => a == item2))
                    {
                        ListListadoAlojables.Add(item2);

                    }
                    
                }
                    
                //int _indexComponet = _index;
                //foreach (string itemComponent in item.DischangeComponentName)
                //{



                //        sl.SetCellValue(_index, 1, _contador);
                //        sl.SetCellValue(_index, 2, itemComponent);
                //        _index++;
                //        _contador++;
                //}
                
            }
            if (ListListadoAlojables.Any())
            {
                foreach (string itemComponent in ListListadoAlojables.Select(x => x).Distinct().OrderBy(o=>o))
                {



                    sl.SetCellValue(_index, 1, _contador);
                    sl.SetCellValue(_index, 2, itemComponent);
                    _index++;
                    _contador++;
                }
            }
            else
            {
                sl.SetCellValue(_index, 1, _contador);
                sl.SetCellValue(_index, 2, "El paquete no genera listado de alojables");
                _index++;
                _contador++;
            }
            //char backSlash = Path.DirectorySeparatorChar;

            sl.SaveAs(msPassTemp);
            //msPassTemp.Position = 0;
            return msPassTemp;

        }

        //Set ListadoConfigurables
        public static MemoryStream ListadoConfigurables(MemoryStream msPass, List<ActivityComponentListConfigurables> listData)
        {
            msPass.Position = 0;
            MemoryStream msPassTemp = new MemoryStream();
            SLDocument sl = new SLDocument(msPass);
            sl.SelectWorksheet("ListadoConfigurables");
            int _index = 2;
            int _contador = 1;
            List<string> ListListadoConfigurables = new List<string>();
            foreach (ActivityComponentListConfigurables item in listData)
            {
                foreach (string item2 in item.DischangeComponentName)
                {
                    if (!ListListadoConfigurables.Any(a => a == item2))
                    {

                        ListListadoConfigurables.Add(item2);

                    }
                }
                
                //foreach (string itemComponent in item.DischangeComponentName)
                //{
                //    sl.SetCellValue(_index, 1, _contador);
                //    sl.SetCellValue(_index, 2, itemComponent);
                //    sl.SetCellValue(_index, 3, item.ComponentEnv);
                //    _index++;
                //    _contador++;
                //}

            }
            if (ListListadoConfigurables.Any())
            {
                foreach (string itemComponent in ListListadoConfigurables.Select(x => x).Distinct().OrderBy(o => o))
                {



                    sl.SetCellValue(_index, 1, _contador);
                    sl.SetCellValue(_index, 2, itemComponent);
                    _index++;
                    _contador++;
                }
            }
            else
            {
                sl.SetCellValue(_index, 1, _contador);
                sl.SetCellValue(_index, 2, "El paquete no genera listado de configurables");
                _index++;
                _contador++;
            }
            //char backSlash = Path.DirectorySeparatorChar;
            sl.SaveAs(msPassTemp);

            return msPassTemp;

        }

        //Set ListadoScript
        public static MemoryStream ListadoScript(MemoryStream msPass, List<ActivityComponentListScript> listData)
        {
            msPass.Position = 0;
            MemoryStream msPassTemp = new MemoryStream();
            SLDocument sl = new SLDocument(msPass);
            sl.SelectWorksheet("ListadoScripts");
            int _index = 2;
            int _contador = 1;
            if (listData.Any())
            {
                foreach (ActivityComponentListScript item in listData)
                {
                    int _indexComponet = _index;
                    foreach (string itemComponent in item.DischangeComponentName)
                    {
                        sl.SetCellValue(_index, 1, _contador);
                        sl.SetCellValue(_index, 2, itemComponent);
                        sl.SetCellValue(_index, 3, item.TypeScript);
                        _index++;
                        _contador++;
                    }


                }
            }
            else
            {
                sl.SetCellValue(_index, 1, _contador);
                sl.SetCellValue(_index, 2, "El paquete no genera listado de scripts");
                _index++;
                _contador++;
            }
            
            
            //char backSlash = Path.DirectorySeparatorChar;
            sl.SaveAs(msPassTemp);

            return msPassTemp;

        }

        //Set Actividad Para Desplegar
        public static MemoryStream DeployActivity(MemoryStream msPass, List<ActivityComponentPrePro> listData,string _workbook)
        {
            msPass.Position = 0;
            MemoryStream msPassTemp = new MemoryStream();
            SLDocument sl = new SLDocument(msPass);
            sl.SelectWorksheet(_workbook);
            int _index = 2;
            int _contador = 1;
            SLStyle style = sl.CreateStyle();
            style.SetWrapText(true);
            style.Alignment.JustifyLastLine = true;
            //style.Font.FontSize = 14;
            foreach (ActivityComponentPrePro item in listData)
            {

                sl.SetCellValue(_index, 1, _contador);
                sl.SetCellStyle(_index, 1, style);
                sl.SetCellValue(_index, 2, item.PendindActivity);
                sl.SetCellStyle(_index, 2, style);
                sl.SetCellValue(_index, 3, item.TypeActivity);
                sl.SetCellStyle(_index, 3, style);
                sl.SetCellValue(_index, 4, item.rst);
                sl.SetCellStyle(_index, 4, style);
                _index++;
                _contador++;


            }
            //char backSlash = Path.DirectorySeparatorChar;
            sl.SaveAs(msPassTemp);

            return msPassTemp;

        }

        //SAVE EXCEL ACTIVITY
        public static void SaveExcelEntrega (MemoryStream msPass, string filePath)
        {
            msPass.Position = 0;
            SLDocument sl = new SLDocument(msPass);
            char backSlash = Path.DirectorySeparatorChar;
            //msPass.Position = 0;
            sl.SaveAs($"{@filePath}{backSlash}ExcelEntrega_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");

        }

        //FIN para las actividades de despliegue (Excel de entrega)
    }
}
