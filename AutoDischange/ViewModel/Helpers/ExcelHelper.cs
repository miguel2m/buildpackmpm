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

        //Create excel aand insert worksheet for Comparacion de componeentes
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
                sl.SetCellValue(_index, 6, item.FechaA);

                if (item.FechaA != null)
                {
                    sl.SetCellStyle(_index, 5, styleFecha);
                    sl.SetCellStyle(_index, 6, styleHora);
                }

                sl.SetCellValue(_index, 7, item.LenghtA);
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
                        sl.SetCellValue(_index, 14, "El componenete solo existe en el paquete 1");
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
                        sl.SetCellValue(_index, 15, "El componenete solo existe en el paquete 1");
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
      
        public static void ObtenerDatosScriptSql(string url)
        {
            List<ActividadDespliegue> lstActDesp = new List<ActividadDespliegue>();
            try
            {
                //VERIFIQUEMOS QUE LA RUTA EXISTE
                if (Directory.Exists(url))
                {
                    string ext = string.Empty;
                    //VAMOS A BUSCAR LOS SQL QUE CONTENGAN ESTE PAQUETE
                    //TENEMOS QUE VERIFICAR QUE CONTIENE LOS ARCHIVOS SQL
                    DirectoryInfo directoryInfo = new DirectoryInfo(url);
                    FileInfo[] filesSql = directoryInfo.GetFiles("*.sql");
                    foreach (FileInfo file in filesSql)
                    {
                        ActividadDespliegue actividadDespliegues = new ActividadDespliegue();
                        string[] divNameFile = file.ToString().Split('_');
                        actividadDespliegues.OrdenEjec = divNameFile[0];

                        for (int i = 0; i < divNameFile.Count(); i++)
                        {
                            //NECESITO VERIFICAR QUE EL ARCHIVO TENGA LA ESTRUCTURA
                            if (divNameFile[i] == "DML" || divNameFile[i] == "DDL" || divNameFile[i] == "SIN")
                            {
                                actividadDespliegues.TipoScrpt = divNameFile[i];
                            }

                            if (divNameFile[i] == "DOC" || divNameFile[i] == "SEG" || divNameFile[i] == "ECL" ||
                                divNameFile[i] == "APR" || divNameFile[i] == "HST")
                            {
                                actividadDespliegues.NombEsqum = divNameFile[i];
                            }
                            
                            if ((divNameFile[i].Length > 3) && (divNameFile[i].Substring(0, 3) == "ESQ" || divNameFile[i].Substring(0, 3) == "CON"))
                            {
                                actividadDespliegues.TipoUsr = divNameFile[i].Substring(0,3);
                            }
                        }
                        actividadDespliegues.NombArchv = file.Name;
                        lstActDesp.Add(actividadDespliegues);
                    }                   
                }
                //Cargar el Excel
                string rutaXlsx = "C:\\Users\\edgar.linarez\\OneDrive - MPM SOFTWARE SLU\\Escritorio\\pruebasAuth\\PrimerEntrega\\ExcelEntregaEjemplo.xlsx";
                string[] tipAmbiente = { "ActividadesParaDesplegarPre", "ActividadesParaDesplegarPro" };
                foreach (var item in tipAmbiente)
                {
                    CargarDatosActividadesDespliegue(lstActDesp, rutaXlsx, item);
                }
            }
            catch (Exception e)
            {
                var problem = e.ToString();
                throw;
            }
        }
        public static void CargarDatosActividadesDespliegue(List<ActividadDespliegue> actividads, string pathXlsx, string Hoja_amb)
        {
            try
            {
                               
                SLDocument sl = new SLDocument(pathXlsx, Hoja_amb);
                string cadAct = string.Empty;
                
                //ORDENO LA LISTA POR ORDEN DE EJECUCION
                IEnumerable<ActividadDespliegue> lstOrdenaActvdds = actividads.OrderBy(x => x.TipoScrpt)
                    .ThenBy(x => x.NombEsqum)
                    .ThenBy(x => x.TipoUsr).ToList();

                //VOY A CREAR UNA LISTA QUE VOY A USAR PARA AGREGAR EN EL EXCEL
                List<ActividadDespliegue> lstExcel = new List<ActividadDespliegue>();

                int cell = 2;
                int numReg = 1;
                string concatNomArch = string.Empty;
                bool flag = false;
                //voy a recorrer la lista que traigo y comparo lista con lista
                foreach (var item in actividads)
                {
                    if (!lstExcel.Contains(item))
                    {
                        foreach (var item0 in lstOrdenaActvdds)
                        {
                            if (item.OrdenEjec == item0.OrdenEjec)
                            {
                                lstExcel.Add(item0);
                                concatNomArch = item0.NombArchv + "\n";
                                flag = true;
                            }
                            else
                            {
                                if (item.TipoScrpt == item0.TipoScrpt && item.NombEsqum == item0.NombEsqum && item.TipoUsr == item0.TipoUsr)
                                {
                                    lstExcel.Add(item0);
                                    concatNomArch += item0.NombArchv;
                                    flag = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                    
                    if (flag)
                    {
                        //ACA EMPIEZA A CARGAR LOS ATRIBUTOS DEL SQL EN EL EXCEL 
                        sl.SetCellValue($"A{cell}", numReg);
                        sl.SetCellValue($"B{cell}", "Ninguna");
                        sl.SetCellValue($"C{cell}", "Despliegue");
                        if (Hoja_amb == "ActividadesParaDesplegarPre")
                        {
                            cadAct = $"Ejecutar los siguientes scripts {item.TipoScrpt} en el orden indicado: \n" +
                                $"{concatNomArch}\n\r" +
                                $"Servidor: DBNEUIVLMX01\n" +
                                $"Instancia: otmxdisp\n" +
                                $"Esquema: gchtm{item.NombEsqum.ToLower()}\n" +
                                $"Puerto: 1660\r";
                        }
                        else
                        {
                            cadAct = $"Ejecutar los siguientes scripts {item.TipoScrpt} en el orden indicado: \n" +
                                $"{concatNomArch}\n\r" +
                                $"Servidor: DBNEUPVLMX01 y DBNEUPVLMX02\n" +
                                $"Instancia: oemxdisp\n" +
                                $"Esquema: prhtm{item.NombEsqum.ToLower()}\n" +
                                $"Puerto: 1660\r";
                        }
                        sl.SetCellValue($"D{cell}", cadAct);
                        cell++;
                        numReg++;
                    }
                }

                //VAMOS A CARGAR LOS DATOS PARA LA TAREA IMPORTACION DE PROCESOS
                string ImpProc = string.Empty, PathImpProc = string.Empty, cadImpProc = string.Empty;
                PathImpProc = ConfigurationManager.AppSettings["PathServerBatchImpProc"];
                if (Hoja_amb == "ActividadesParaDesplegarPre")
                {
                    ImpProc = ConfigurationManager.AppSettings["ServerBatchImpProcPre"];
                }
                else
                {
                    ImpProc = ConfigurationManager.AppSettings["ServerBatchImpProcPro"];
                }
                cadImpProc = $"Entrar al servidor {ImpProc} y hacer las siguientes acciones: \n" +
                    $"1.- Eliminar el contenido de la carpeta {PathImpProc}";
                sl.SetCellValue($"A{cell}", numReg);
                sl.SetCellValue($"B{cell}", "Ninguna");
                sl.SetCellValue($"C{cell}", "Despliegue");
                sl.SetCellValue($"D{cell}", cadImpProc);
                cell++;
                numReg++;

                //VAMOS A BAJAR LOS POOLS
                sl.SetCellValue($"A{cell}", numReg);
                sl.SetCellValue($"B{cell}", "Ninguna");
                sl.SetCellValue($"C{cell}", "Despliegue");
                sl.SetCellValue($"D{cell}", "Bajar pools de todos los servidores.");                
                cell++;
                numReg++;

                int prec1 = cell - 3, prec2 = cell - 2;


                //DESPLEGAR LOS CR
                sl.SetCellValue($"A{cell}", numReg);
                sl.SetCellValue($"B{cell}", $"{prec1} y {prec2}");
                sl.SetCellValue($"C{cell}", "Despliegue");
                sl.SetCellValue($"D{cell}", $"Desplegar las siguientes CRs:\n Alojables \n Configurables");
                cell++;
                numReg++;

                prec1 = cell - 2;

                ///VALIDACIONES
                string serv1 = string.Empty, serv2 = string.Empty, serv3 = string.Empty, serv4 = string.Empty, serv5 = string.Empty;
                string ip1 = string.Empty, ip2 = string.Empty, ip3 = string.Empty, ip4 = string.Empty, ip5 = string.Empty;
                if (Hoja_amb == "ActividadesParaDesplegarPre")
                {
                    serv1 = "WEBNEUIVWMX03";
                    serv2 = "SRNEUIWM1MXR309";
                    serv3 = "SRVNEUIVWMX01";
                    serv4 = "SRNEUIWM1MXR307";
                    serv5 = "WEBNEUIVWMX04";
                    ip1 = "180.181.105.137";
                    ip2 = "180.228.64.204";
                    ip3 = "180.181.105.139";
                    ip4 = "180.228.64.206";
                    ip5 = "180.181.105.136";
                }
                else
                {
                    serv1 = "WEBNEUPVWMX03";
                    serv2 = "SRVNEUPVWMX09";
                    serv3 = "SRVNEUPVWMX01";
                    serv4 = "SRVNEUPVWMX07";
                    serv5 = "WEBNEUPVWMX04";
                    ip1 = "180.181.165.93";
                    ip2 = "180.181.167.59";
                    ip3 = "180.181.167.51";
                    ip4 = "180.181.167.57";
                    ip5 = "180.181.165.97";
                }

                //VALIDACIONES 1
                sl.SetCellValue($"A{cell}", numReg);
                sl.SetCellValue($"B{cell}", $"{prec1}");
                sl.SetCellValue($"C{cell}", "Despliegue");                
                sl.SetCellValue($"D{cell}", $"En el servidor {serv1} con dirección {ip1}, abrir la línea de comandos y dirigirse a D:\\MPM\\DIS para ejecutar el comando:\n " +
                    $"dir / s *.* / o:-d > webn03_caida.txt\n" +
                    $"Entregar al Gestor del cambio el archivo webn03_caida.txt.\n" +
                    $"Esperar validación para continuar con la siguiente actividad.\n");
                cell++;
                //VALIDACIONES 2
                sl.SetCellValue($"A{cell}", numReg);
                sl.SetCellValue($"B{cell}", $"{prec1}");
                sl.SetCellValue($"C{cell}", "Despliegue");
                sl.SetCellValue($"D{cell}", $"En el servidor {serv2} con dirección {ip2}, abrir la línea de comandos y dirigirse a D:\\MPM\\DIS para ejecutar el comando:\n " +
                    $"dir /s *.* /o:-d > srvn09_caida.txt\n" +
                    $"Entregar al Gestor del cambio el archivo srvn09_caida.txt.\n" +
                    $"Esperar validación para continuar con la siguiente actividad.\n");
                cell++;
                //VALIDACIONES 3
                sl.SetCellValue($"A{cell}", numReg);
                sl.SetCellValue($"B{cell}", $"{prec1}");
                sl.SetCellValue($"C{cell}", "Despliegue");
                sl.SetCellValue($"D{cell}", $"En el servidor {serv3} con dirección {ip3}, abrir la línea de comandos y dirigirse a D:\\MPM\\DIS para ejecutar el comando:\n " +
                    $"dir /s *.* /o:-d > srvn01_caida.txt\n" +
                    $"Entregar al Gestor del cambio el archivo srvn01_caida.txt.\n" +
                    $"Esperar validación para continuar con la siguiente actividad.\n");
                cell++;
                //VALIDACIONES 4
                sl.SetCellValue($"A{cell}", numReg);
                sl.SetCellValue($"B{cell}", $"{prec1}");
                sl.SetCellValue($"C{cell}", "Despliegue");
                sl.SetCellValue($"D{cell}", $"En el servidor {serv4} con dirección {ip4}, abrir la línea de comandos y dirigirse a D:\\MPM\\DIS para ejecutar el comando:\n " +
                    $"dir /s *.* /o:-d > srvn07_caida.txt\n" +
                    $"Entregar al Gestor del cambio el archivo srvn07_caida.txt.\n" +
                    $"Esperar validación para continuar con la siguiente actividad.\n");
                prec1 = numReg;
                cell++;
                numReg++;

                //PRENDER LOS POOLS
                sl.SetCellValue($"A{cell}", numReg);
                sl.SetCellValue($"B{cell}", $"{prec1}");
                sl.SetCellValue($"C{cell}", "Despliegue");
                sl.SetCellValue($"D{cell}", $"Prender pools de todos los servidores.\n");
                prec1 = numReg;
                cell++;
                numReg++;

                //VALIDACION DE LOGS
                sl.SetCellValue($"A{cell}", numReg);
                sl.SetCellValue($"B{cell}", $"{prec1}");
                sl.SetCellValue($"C{cell}", "Despliegue");
                sl.SetCellValue($"D{cell}", $"En el servidor {serv2} {ip2} Batch:\n " +
                    $"Abrir como Administrador la línea de comandos \n" +
                    $"Ir a la carpeta D:\\MPM\\DIS\\InstallBSM\\ \n" +
                    $"Ejecutar el archivo Install - DIS - Procesos.cmd \n" +
                    $"Al finalizar la ejecución compartir el archivo log - import - file.log que se encuentra en D:\\MPM\\DIS\\MPM.FullProcessImport\\Logs\\Import \n" +
                    $"Esperar validación de los logs para continuar con la siguiente actividad. \n");
                prec1 = numReg;
                cell++;
                numReg++;

                //REINICIO DE POOLS
                sl.SetCellValue($"A{cell}", numReg);
                sl.SetCellValue($"B{cell}", $"{prec1}");
                sl.SetCellValue($"C{cell}", "Despliegue");
                sl.SetCellValue($"D{cell}", $"Reinicio de Pools.\n" +
                    $"- {serv1} {ip1} Web. \n" +
                    $"- {serv2} {ip2} Web. \n" +
                    $"Reiniciar los siguientes pools: \n" +
                    $"- DIS_ecDataProvider. \n" +
                    $"- DIS_eClient. \n" +
                    $"- DIS_LoginManagerService. \n");
                prec1 = numReg;
                cell++;
                numReg++;

                //IMPORTAR RECURSOS
                sl.SetCellValue($"A{cell}", numReg);
                sl.SetCellValue($"B{cell}", $"{prec1}");
                sl.SetCellValue($"C{cell}", "Despliegue");
                sl.SetCellValue($"D{cell}", $"En el servidor {serv2} {ip2} Batch:\n" +
                    $"Abrir como Administrador la línea de comandos\n" +
                    $"Ir a la carpeta D:\\MPM\\DIS\\InstallBSM\\Resources\\ImportarRecursos\n" +
                    $"Ejecutar el archivo ImportarRecursos_1.cmd y al finalizar devolver:\n" +
                    $"- El archivo cookies.txt ubicado en D:\\MPM\\DIS\\InstallBSM\\Resources\\ImportarRecursos\n" +
                    $"- El archivo salida1_DDMMAAAA.html\n" +
                    $"- El archivo salida1_errores_DDMMAAAA.log\n" +
                    $"Esperar revisión de los logs de esta tarea para continuar con la siguiente actividad.\n");
                prec1 = numReg;
                cell++;
                numReg++;

                //SOLICITAR LOGS
                sl.SetCellValue($"A{cell}", numReg);
                sl.SetCellValue($"B{cell}", $"{prec1}");
                sl.SetCellValue($"C{cell}", "Despliegue");
                sl.SetCellValue($"D{cell}", $"Solicitar logs.\n");
                cell++;
                numReg++;

                sl.SaveAs(pathXlsx);
            }
            catch (Exception)
            {
               
                throw;
            }
            


        }

        public static MemoryStream ReadExcelEntrega()
        {

            string rtfFile = System.IO.Path.Combine(Environment.CurrentDirectory, "ExcelEntrega.xlsx");
            FileStream fs = new FileStream(rtfFile, FileMode.Open);
            MemoryStream msPass = new MemoryStream();
            SLDocument slOriginal = new SLDocument(fs);
            slOriginal.SaveAs(msPass);

            //SLDocument sl = new SLDocument(msPass);
            //foreach (ActivityModel item in ActivityModelList)
            //{
            //    sl.SelectWorksheet(item.Workbook);

            //    int _index = 2;
            //    int contador = 1;
            //    foreach (string fileType in item.ListFile)
            //    {
            //        sl.SetCellValue(_index, 1, contador);
            //        sl.SetCellValue(_index, 2, fileType);
            //        _index++;
            //        contador++;
            //    }
            //}          
            fs.Close();
            //char backSlash = Path.DirectorySeparatorChar;
            //sl.SaveAs($"{@filePath}{backSlash}ExcelEntrega_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
            //msPass.Position = 0;
            return msPass;

        }

        //Set ListadoAlojables
        public static MemoryStream ListadoAlojables(MemoryStream msPass, List<ActivityComponentListAlojables>  listData)
        {
            msPass.Position = 0;
            MemoryStream msPassTemp = new MemoryStream();
            SLDocument sl = new SLDocument(msPass);
            sl.SelectWorksheet(listData.First().Workbook);
            int _index = 2;
            int _contador = 1;
            foreach (ActivityComponentListAlojables item in listData)
            {

                
                //sl.SetCellValue(_index, 1, item.Id);
                int _indexComponet = _index;
                
                foreach (string itemComponent in item.DischangeComponentName)
                {


                        sl.SetCellValue(_index, 1, _contador);
                        sl.SetCellValue(_index, 2, itemComponent);
                        _index++;
                        _contador++;
                }
                
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
            sl.SelectWorksheet(listData.First().Workbook);
            int _index = 2;
            int _contador = 1;
            foreach (ActivityComponentListConfigurables item in listData)
            {
               
                foreach (string itemComponent in item.DischangeComponentName)
                {
                    sl.SetCellValue(_index, 1, _contador);
                    sl.SetCellValue(_index, 2, itemComponent);
                    sl.SetCellValue(_index, 3, item.ComponentEnv);
                    _index++;
                    _contador++;
                }
                
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
            sl.SelectWorksheet(listData.First().Workbook);
            int _index = 2;
            int _contador = 1;
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
    }
}
