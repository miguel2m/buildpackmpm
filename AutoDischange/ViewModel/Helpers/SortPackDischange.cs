using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.ViewModel.Helpers
{
    public class SortPackDischange
    {
        public static void SortPack(string path)
        {

            string pathRoot = path;

            char backSlash = Path.DirectorySeparatorChar;
            string pathAlojables = $"{@pathRoot}{backSlash}Alojables{backSlash}DIS";
            string pathConfigurables = $"{@pathRoot}{backSlash}Configurables{backSlash}Cert";
            string pathScript = $"{@pathRoot}{backSlash}Scripts{backSlash}DIS";

            System.IO.DirectoryInfo dirRoot = new System.IO.DirectoryInfo(pathRoot); 

            //IEnumerable<System.IO.FileInfo> listPathRoot = dirRoot.GetFiles("*.*", System.IO.SearchOption.AllDirectories).Where(s => !(s.Name.EndsWith(".config") || s.Name.EndsWith(".sql"))); ;
            IEnumerable<System.IO.FileInfo> listPathAlojables = dirRoot.GetFiles("*.*", System.IO.SearchOption.AllDirectories).Where(s => !(s.Name.EndsWith(".config") || s.Name.EndsWith(".sql")));
            IEnumerable<System.IO.FileInfo> listPathConfigurables = dirRoot.GetFiles("*.*", System.IO.SearchOption.AllDirectories).Where(s => (s.Name.EndsWith(".config")));
            IEnumerable<System.IO.FileInfo> listPathScript = dirRoot.GetFiles("*.*", System.IO.SearchOption.AllDirectories).Where(s => (s.Name.EndsWith(".sql"))); ;

            //List<string> alojables = listPathRoot      
            foreach (System.IO.FileInfo item in listPathAlojables)
            {

               
                string outputFolder = NormalizePath(item.DirectoryName, pathRoot, pathAlojables);
                string outputFile = outputFolder + backSlash + item.Name;
                CreateFolderOutput(item.FullName, outputFolder, outputFile);



            }
            System.IO.FileInfo itemLast = listPathAlojables.Last();
            foreach (System.IO.FileInfo item in listPathConfigurables)
            {
                string outputFolder = NormalizePath(item.DirectoryName, pathRoot, pathAlojables);
                string outputFile = outputFolder + backSlash + item.Name;
                CreateFolderOutput(item.FullName, outputFolder, outputFile);
            }
           
            foreach (System.IO.FileInfo item in listPathScript)
            {
                string outputFolder = NormalizePath(item.DirectoryName, pathRoot, pathAlojables);
                string outputFile = outputFolder + backSlash + item.Name;
                CreateFolderOutput(item.FullName, outputFolder, outputFile);
            }

            processDirectory(pathRoot);
        }

        private static string NormalizePath(string pathInput,string pathRoot,string pathEnv) { 
            string directoryFile = pathInput.Replace(pathRoot, "");
     
            string directoryOut = $@"{pathEnv + directoryFile}"; ;

            return directoryOut;
        }

        private static void CreateFolderOutput(string rootFile,string outputFolder, string outputFile)
        {
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
                
            }
            File.Move(rootFile, outputFile);
        }
        private static void processDirectory(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                processDirectory(directory);
                if (Directory.GetFiles(directory).Length == 0 &&
                    Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }
    }
}
