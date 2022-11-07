using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDischange.Model;

namespace AutoDischange.ViewModel.Helpers
{
    public class UtilHelper
    {
        //Deveulve el archivo en la ruta
        public static string nameFile(string url, char separator)
        {
            List<string> list = new List<string>();
            list = url.Split(separator).ToList();
            int count = list.Count;
            string result = list[count - 1];

            return result;
        }

        public static string nameDir(string url)
        {
            List<string> list = new List<string>();
            list = url.Split('\\').ToList();
            int count = list.Count;
            string result = list[count - 1];

            return result;
        }

        //Deveulve el archivo en la ruta
        public static List<string> fileList(string url, char separator)
        {
            List<string> list = new List<string>();
            list = url.Split(separator).ToList();
            int count = list.Count;
            return list;
        }

        public static string extraerBranchTfs(string url, char separator, string ext, string branch = null)
        {
            string brancho = ConfigurationManager.AppSettings["rutaJenkins"];
            string[] pathTfs = url.Split(separator);

            List<BranchJenkins> nombRama = ListaBranchesJenkins();
            int cant = nombRama.Count;
            int count = 0;
            foreach (string busqueda in pathTfs)
            {
                if (branch != null)
                {
                    if (busqueda.Contains(ext))
                    {
                        if (ext == ".sql")
                        {
                            brancho += $"{branch}\\Pack\\Latest\\Scripts";
                        }
                        else
                        {
                            brancho += $"{branch}\\Pack\\Latest";
                        }
                    }
                }
                else
                {
                    while (count < cant)
                    {
                        if (nombRama[count].NameBranch != "Customers" && nombRama[count].NameBranch != "Logs" && nombRama[count].NameBranch != "utils")
                        {
                            if (nombRama[count].NameBranch == busqueda)
                            {
                                brancho += nombRama[count].NameBranch + "\\Pack\\Latest\\Scripts";
                            }
                        }
                        count++;
                    }
                    count = 0;
                }
            }
            return brancho;
        }

        public static List<BranchJenkins> ListaBranchesJenkins()
        {
            List<BranchJenkins> lstNombDir = new List<BranchJenkins>();
            string rutaBranch = ConfigurationManager.AppSettings["rutaJenkins"];
            string[] subDir = Directory.GetDirectories(rutaBranch);
            foreach (var item in subDir.Select((value, i) => new { i, value }))
            {
                var value = nameDir(item.value);
                var index = item.i;
                lstNombDir.Add(new BranchJenkins { CodBranch = index, NameBranch = value });
            }
            return lstNombDir;
        }

        public static string rutaNoFile(string url, string ext = "")
        {
            string result = string.Empty;
            List<string> list = new List<string>();
            list = url.Split('\\').ToList();
            int a = list.IndexOf("Scripts");
            if (ext == ".sql")
            {
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

        public static void buildStructure(string url)
        {
            if (!Directory.Exists(url))
            {
                //Crear el directorio
                Directory.CreateDirectory(url);
                //Alojables
                Directory.CreateDirectory($@"{url}\\Alojables\\DIS\\");
                //CONFIGURABLES PARA CERT
                Directory.CreateDirectory($@"{url}\\Configurables\\Cert\\DIS\\");
                //CONFIGURABLES PARA DESA
                Directory.CreateDirectory($@"{url}\\Configurables\\Desa\\DIS\\");
                //CONFIGURABLES PARA Pre
                Directory.CreateDirectory($@"{url}\\Configurables\\Pre\\DIS\\");
                //CONFIGURABLES PARA Pro
                Directory.CreateDirectory($@"{url}\\Configurables\\Pro\\DIS\\");
                //SCRIPTS
                Directory.CreateDirectory($@"{url}\\Scripts\\DIS\\");
            }
        }
    }
}
