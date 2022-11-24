using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoDischange.ViewModel.Helpers
{
    public class TransferFileJenkinsHelper
    {

        public static FilesPacksToUpdates filesPacksToUpdates;
        public static List<FilesPacksToUpdates> FilesPacksTos = new List<FilesPacksToUpdates>();
        public static List<FilesPacksToUpdates> JenkinsTransferFile(string rutaUbicFile, string rutaUsr, string branch, string changeset)
        {
            string ext = Path.GetExtension(rutaUbicFile);
            string rutaServer = string.Empty, rutaDisChanges = string.Empty, fileExamp = string.Empty;
            string rutaPack = string.Empty, rutaI = string.Empty, rutaPack2 = string.Empty, rutaF = string.Empty;
            if (ext == ".sql" || ext == ".config" || ext == ".cmd")//ESTE LADO SOLO LA CHUSMA
            {
                rutaServer = rutaUbicFile;
                fileExamp = nameFile(rutaServer);
                rutaPack = rutaNoFile(rutaServer, ext);
                if (ext == ".sql")
                {
                    rutaPack += @"DIS\";
                }
                rutaI = rutaNoFile(rutaServer);
                rutaF = $@"{rutaUsr}\{rutaPack}";
            }
            else//ESTE LADO SON SOLO ALOJABLES
            {
                if (!rutaUbicFile.Contains(@"\\ci-jenkins\branches\BSM\"))
                {
                    //ruta del servidor
                    rutaServer = ConfigurationManager.AppSettings["rutaJenkins"];

                    //Necesito extraer solo la parte que es necesaria de la ruta de la Guia de Ubicaciones
                    rutaDisChanges = cutPath(rutaUbicFile);


                    //Debo conectar la ruta del servidor jenkins con el branch seleccionado por el usuario
                    rutaServer = rutaServer + $@"{branch}\Pack\Latest\";

                    //Separo la ruta del dischange para obtener el nombre del archivo
                    fileExamp = nameFile(rutaDisChanges);
                    //Separo la ruta del dischange para quitar el nombre del archivo
                    rutaPack = rutaNoFile(rutaDisChanges);
                }
                else
                {
                    fileExamp = nameFile(rutaUbicFile);
                    rutaPack = rutaNoFile(rutaUbicFile);
                }

                //ARMO LA RUTA PARA BUSCAR EN JENKINS
                rutaI = rutaPack.Contains($@"\ci-jenkins\branches\BSM\{branch}\Pack\Latest\") ? rutaPack : rutaServer + rutaPack;

                rutaPack2 = rutaNoFile(rutaUbicFile);
                //ARMO LA RUTA PARA COPIAR EN LOCAL
                rutaF = rutaUbicFile.Contains($@"\\ci-jenkins\branches\BSM\{branch}\Pack\Latest\") ? rutaUsr + "\\Alojables\\DIS" +  rutaNoFile(rutaUbicFile.Replace($@"\\ci-jenkins\branches\BSM\{branch}\Pack\Latest", "")) : $@"{rutaUsr}{rutaPack2}";
            }


            //verifico que el directorio de busqueda exista
            if (Directory.Exists(rutaI))
            {
                if (File.Exists($"{rutaI}{fileExamp}"))
                {
                    string pathFileStart = $"{rutaI}{fileExamp}";
                    string pathFileEnd = rutaF + fileExamp;
                    //Verifico que el directorio de destino exista
                    if (!Directory.Exists(rutaF))
                    {
                        //Crear el directorio
                        Directory.CreateDirectory(rutaF);
                    }

                    //Verifico que el archivo en directorio destino exista o no
                    if (File.Exists(pathFileEnd))
                    {
                        File.SetAttributes(pathFileEnd, FileAttributes.Normal);
                        File.Delete(pathFileEnd);
                    }
                    //SI EXISTE EL ARCHIVO EN EL JENKINS
                    if (File.Exists(pathFileStart))
                    {
                        //Copiar archivo
                        File.Copy(pathFileStart, pathFileEnd, true);

                        //TENGO UNA 
                        //CREAMOS UNA LISTA PARA GUARDAR TODAS LAS RUTAS CON SU PESO Y FECHA DE CREACION
                        filesPacksToUpdates = new FilesPacksToUpdates();
                        filesPacksToUpdates.pathFile = pathFileEnd;
                        filesPacksToUpdates.nameFile = fileExamp;
                        FileInfo fileInfo = new FileInfo(pathFileStart);
                        filesPacksToUpdates.dateTimeFile = fileInfo.LastWriteTime;
                        filesPacksToUpdates.weightFile = (int)fileInfo.Length;
                        filesPacksToUpdates.changeset = changeset;
                        filesPacksToUpdates.branchUse = branch;
                        filesPacksToUpdates.Confirm = true;
                        FilesPacksTos.Add(filesPacksToUpdates);
                    }
                    else
                    {
                        Log4net.log.Error($@"El archivo {fileExamp} no esta en la ruta {rutaI}");
                    }
                }
                else
                {
                    Log4net.log.Error($@"El archivo {fileExamp} no esta en la ruta {rutaI}");
                }
            }
            else
            {
                Log4net.log.Error($@"El directorio {rutaI} donde quiere acceder no existe");
            }
            return FilesPacksTos;
        }
        public static string nameFile(string url)
        {
            List<string> list = new List<string>();
            list = url.Split('\\').ToList();
            int count = list.Count;
            string result = list[count - 1];

            return result;
        }
        public static string rutaNoFile(string url, string ext = "")
        {
            string result = string.Empty;
            List<string> list = new List<string>();
            list = url.Split('\\').ToList();
            if (ext == ".sql" || ext == ".config" || ext == ".cmd")
            {
                int a = ext == ".sql" ? list.IndexOf("Scripts") : list.IndexOf("Configurables");
                for (int i = a; i < list.Count - 1; i++)
                {
                    result += list[i] + "\\";
                }
            }
            else
            {
                for (int i = 0; i < list.Count - 1; i++)
                {
                    result += list[i] + "\\";
                }
            }
            return result;
        }
        public static string cutPath(string url)
        {
            //NECESITO SOLO OBTENER UNA PARTE DEL PATH
            //TENGO ESTA URL DE EJEMPLO \Alojables\DIS\eClient\workflowConfiguration\agenda.tareainc-mapping.xml
            //LOS DOS PRIMEROS ELEMENTOS NO ME SIRVEN \Alojables\DIS\
            string result = string.Empty;
            List<string> list = new List<string>();
            list = url.Split('\\').ToList();
            for (int i = 1; i < list.Count; i++)
            {
                //CUANDO LO CONSTRUYO COLOCO LOS SLASH INVERTIDOS AL FINAL
                if (list[i] != "Alojables" && list[i] != "Configurables" && list[i] != "Cer" && list[i] != "Des" &&
                    list[i] != "DIS" && list[i] != "Pre" && list[i] != "Pro")
                {
                    if (i == list.Count - 1)
                    {
                        result += list[i];
                    }
                    else
                    {
                        result += list[i] + "\\";
                    }

                }
            }
            return result;
        }
    }

    public class FilesPacksToUpdates
    {
        public string changeset { get; set; }
        public string branchUse { get; set; }
        public string pathFile { get; set; }
        public string nameFile { get; set; }
        public DateTime dateTimeFile { get; set; }
        public int weightFile { get; set; }
        public bool Confirm { get; set; } = false;
    }
}
