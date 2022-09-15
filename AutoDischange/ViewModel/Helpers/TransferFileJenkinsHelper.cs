using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.ViewModel.Helpers
{
    public class TransferFileJenkinsHelper
    {
        public static string JenkinsTransferFile(string rutaDisChanges, string rutaUsr, string branch)
        {
            //Necesito extraer solo la parte que es necesaria de la ruta de la Guia de Ubicaciones
            rutaDisChanges = cutPath(rutaDisChanges);

            //ruta del servidor
            string rutaServer = ConfigurationManager.AppSettings["rutaJenkins"];

            //Debo conectar la ruta del servidor jenkins con el branch seleccionado por el usuario
            rutaServer = rutaServer + $@"{branch}\Pack\Latest\";

            //Separo la ruta del dischange para obtener el nombre del archivo
            string fileExamp = nameFile(rutaDisChanges);

            //Separo la ruta del dischange para quitar el nombre del archivo
            string rutaPack = rutaNoFile(rutaDisChanges);

            string rutaI = rutaServer + rutaPack;
            string rutaF = rutaUsr + rutaPack;
            try
            {
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

                //Verifico que el archivo en directorio destino exista o no
                if (File.Exists(rutaI + fileExamp))
                {
                    //Copiar archivo
                    File.Copy(rutaI + fileExamp, rutaF + fileExamp, true);
                }
            }
            catch (Exception)
            {
                throw;
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
        public static string rutaNoFile(string url)
        {
            string result = string.Empty;
            List<string> list = new List<string>();
            list = url.Split('\\').ToList();
            for (int i = 0; i < list.Count - 1; i++)
            {
                result += list[i] + "\\";
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
            for(int i = 1; i < list.Count; i++)
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
