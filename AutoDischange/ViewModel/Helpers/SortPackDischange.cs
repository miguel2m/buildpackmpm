using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoDischange.ViewModel.Helpers
{
    public class SortPackDischange
    {
        public static void SortPack(string path)
        {
            try
            {

                string pathRoot = path;

                char backSlash = Path.DirectorySeparatorChar;
                string pathAlojables = $"{@pathRoot}Alojables{backSlash}DIS{backSlash}";
                //string pathConfigurables = $"{@pathRoot}Configurables{backSlash}Cert{backSlash}";
                string pathConfigurables = $"{@pathRoot}Configurables{backSlash}";
                string pathScript = $"{@pathRoot}Scripts{backSlash}DIS{backSlash}";

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
                foreach (System.IO.FileInfo item in listPathConfigurables)
                {
                    string outputFolder = NormalizePath(item.DirectoryName, pathRoot, pathConfigurables);
                    string outputFile = outputFolder + backSlash + item.Name;
                    CreateFolderOutput(item.FullName, outputFolder, outputFile);
                }

                foreach (System.IO.FileInfo item in listPathScript)
                {
                    string outputFolder = NormalizePath(item.DirectoryName, pathRoot, pathScript);
                    string outputFile = outputFolder + backSlash + item.Name;
                    CreateFolderOutput(item.FullName, outputFolder, outputFile);
                }

                processDirectory(pathRoot);
            }
            catch (Exception ex)
            {
                Log4net.log.Error(ex.Message);
                MessageBox.Show("Error clasificación de paquetes: " + ex.Message, "Error clasificación de paquetes", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
