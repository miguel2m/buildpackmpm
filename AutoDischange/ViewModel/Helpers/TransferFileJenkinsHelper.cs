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
        public static string JenkinsTransferFile(string rutaUbicFile, string rutaUsr, string branch)
        {
            string ext = Path.GetExtension(rutaUbicFile);
            string rutaServer = string.Empty, rutaDisChanges = string.Empty, fileExamp = string.Empty;
            string rutaPack = string.Empty, rutaI = string.Empty, rutaPack2 = string.Empty, rutaF = string.Empty;
            if (ext == ".sql" || ext == ".config")
            {
                rutaServer = rutaUbicFile;
                fileExamp = nameFile(rutaServer);
                rutaPack = rutaNoFile(rutaServer, ext);
                rutaI = rutaNoFile(rutaServer);
                rutaF = $@"{rutaUsr}\{rutaPack}";
            }
            else
            {
                //Necesito extraer solo la parte que es necesaria de la ruta de la Guia de Ubicaciones
                rutaDisChanges = cutPath(rutaUbicFile);

                //ruta del servidor
                rutaServer = ConfigurationManager.AppSettings["rutaJenkins"];

                //Debo conectar la ruta del servidor jenkins con el branch seleccionado por el usuario
                rutaServer = rutaServer + $@"{branch}\Pack\Latest\";

                //Separo la ruta del dischange para obtener el nombre del archivo
                fileExamp = nameFile(rutaDisChanges);

                //Separo la ruta del dischange para quitar el nombre del archivo
                rutaPack = rutaNoFile(rutaDisChanges);

                rutaI = rutaServer + rutaPack;

                rutaPack2 = rutaNoFile(rutaUbicFile);

                rutaF = $@"{rutaUsr}{rutaPack2}";
            }


            //verifico que el directorio de busqueda exista
            if (!Directory.Exists(rutaI))
            {
                return "El directorio donde quiere acceder no existe";
            }

            //Verifico que el directorio de destino exista
            if (!Directory.Exists(rutaF))
            {
                //Crear el directorio
                Directory.CreateDirectory(rutaF);
            }

            string rutaFileFinal = rutaF + fileExamp;
            //Verifico que el archivo en directorio destino exista o no
            if (File.Exists(rutaFileFinal))
            {
                File.SetAttributes(rutaFileFinal, FileAttributes.Normal);
                File.Delete(rutaFileFinal);
            }


            if (File.Exists(rutaI + fileExamp))
            {
                //Copiar archivo
                File.Copy(rutaI + fileExamp, rutaFileFinal, true);
            }
            else
            {
                Log4net.log.Info(rutaI + fileExamp);
            }
            return "El conjunto de directorios fue copiado correctamente.";
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
            if (ext == ".sql" || ext == ".config")
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
}
