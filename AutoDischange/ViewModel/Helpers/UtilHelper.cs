using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        //Deveulve la ruta sin el archivo
        public static string rutaNoFile(string url, char separator)
        {
            string result = string.Empty;
            List<string> list = new List<string>();
            list = url.Split(separator).ToList();
            for (int i = 0; i < list.Count - 1; i++)
            {
                result += list[i] + separator;
            }
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
    }
}
