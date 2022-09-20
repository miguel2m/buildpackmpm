using AutoDischange.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.ViewModel.Helpers
{
    public class DiffComponentHelper
    {
        //Files in Directories and Sub Directories
        //https://stackoverflow.com/questions/2106877/is-there-a-faster-way-than-this-to-find-all-the-files-in-a-directory-and-all-sub
        public static IEnumerable<string> GetFileList( string rootFolderPath)
        {
            Queue<string> pending = new Queue<string>();
            pending.Enqueue(rootFolderPath);
            string[] tmp;
            while (pending.Count > 0)
            {
                rootFolderPath = pending.Dequeue();
                try
                {
                    tmp = Directory.GetFiles(rootFolderPath);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                for (int i = 0; i < tmp.Length; i++)
                {
                    yield return tmp[i];
                }
                tmp = Directory.GetDirectories(rootFolderPath);
                for (int i = 0; i < tmp.Length; i++)
                {
                    pending.Enqueue(tmp[i]);
                }
            }


        }

        public static IEnumerable<string> GetSearchFileList(string fileSearchPattern, string rootFolderPath)
        {
            Queue<string> pending = new Queue<string>();
            pending.Enqueue(rootFolderPath);
            string[] tmp;
            while (pending.Count > 0)
            {
                rootFolderPath = pending.Dequeue();
                try
                {
                    tmp = Directory.GetFiles(rootFolderPath, fileSearchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                for (int i = 0; i < tmp.Length; i++)
                {
                    yield return tmp[i];
                }
                tmp = Directory.GetDirectories(rootFolderPath);
                for (int i = 0; i < tmp.Length; i++)
                {
                    pending.Enqueue(tmp[i]);
                }
            }
        }

        /// <summary>
        /// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-compare-the-contents-of-two-folders-linq
        /// </summary>
        /// <param name="pathA">Ruta Inicio</param>
        /// <param name="pathB">Ruta Destino</param>
        public static void DiffFiles(DiffComponent diffComponent, string pathUser)
        {
            System.IO.DirectoryInfo dir1 = new System.IO.DirectoryInfo(diffComponent.PathStart);
            System.IO.DirectoryInfo dir2 = new System.IO.DirectoryInfo(diffComponent.PathEnd);

            // Take a snapshot of the file system.  
            IEnumerable<System.IO.FileInfo> list1 = dir1.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
            IEnumerable<System.IO.FileInfo> list2 = dir2.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

            //A custom file comparer defined below  
            FileCompare myFileCompare = new FileCompare();

            // Find the common files. It produces a sequence and doesn't
            // execute until the foreach statement.  
            //var queryCommonFiles = list1.Intersect(list2, myFileCompare);

            //if (queryCommonFiles.Any())
            //{
            //    Console.WriteLine("The following files are in both folders:");
            //    foreach (var v in queryCommonFiles)
            //    {
            //        Console.WriteLine(v.FullName); //shows which items end up in result list  
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("There are no common files in the two folders.");
            //}

            // Find the set difference between the two folders.  
            // For this example we only check one way.  
            //var queryList1Only = (from file in list1
            //                      select file).Except(list2, myFileCompare);
            //Console.WriteLine("Data list1");
            //foreach (var v in list1)
            //{
            //    Console.WriteLine($"FullName {v.FullName}");
            //    Console.WriteLine($"Length {GetSizeByte(v)}");
            //    Console.WriteLine($"LastWriteTime {v.LastWriteTime.ToShortDateString()}");
            //    string s = $"{v.Name}{v.Length}{v.LastWriteTime.ToString()}";
            //    Console.WriteLine($"GetHashCode {s.GetHashCode()}");
            //}
            //Console.WriteLine("Data list2");
            //foreach (var v in list2)
            //{
            //    Console.WriteLine($"FullName {v.FullName}");
            //    Console.WriteLine($"Length {GetSizeByte(v)}");
            //    Console.WriteLine($"LastWriteTime {v.LastWriteTime.ToShortDateString()}");
            //    string s = $"{v.Name}{v.Length}{v.LastWriteTime.ToString()}";
            //    Console.WriteLine($"GetHashCode {s.GetHashCode()}");
            //}

            var queryList1Intersect = (from file in list1
                                  select file).Intersect(list2, myFileCompare);

            //Console.WriteLine("The following files are in list1 AND list2:");
            List<DiffCompareModel> diffCompareModelList = new List<DiffCompareModel>();
            int count = 1;
            foreach (var v in queryList1Intersect)
            {
                DiffCompareModel diffCompareModel = new DiffCompareModel();
                diffCompareModel.Id = count;

                diffCompareModel.UbicacionA = v.FullName;
                diffCompareModel.PathA = v.Name;
                string s = $"{v.Name}{v.Length}{v.LastWriteTime.ToString()}";
                string sHash = s.GetHashCode().ToString();
                diffCompareModel.HashA = sHash;
                diffCompareModel.FechaA = v.LastWriteTime;

                string sizeAll = GetSizeByte(v);
                diffCompareModel.LenghtA = sizeAll;

                diffCompareModel.UbicacionB = v.FullName;
                diffCompareModel.PathB = v.Name;
                diffCompareModel.HashB = sHash;
                diffCompareModel.FechaB = v.LastWriteTime;
                diffCompareModel.LenghtB = sizeAll;

                diffCompareModel.HashResult = 1;
                diffCompareModel.FechaResult = 1;
                diffCompareModel.LenghtResult = 1;
                diffCompareModelList.Add(diffCompareModel);

                count++;
            }
            ExcelHelper.CreateExcelDiffComapre(diffCompareModelList, pathUser, diffComponent);

            //var queryList1Only = (from file in list1
            //                      select file).Except(list2, myFileCompare);

            //Console.WriteLine("The following files are in list1 but not list2:");
            //foreach (var v in queryList1Only)
            //{
            //    Console.WriteLine($"FullName {v.FullName}");
            //    Console.WriteLine($"Length {GetSizeByte(v)}");
            //    Console.WriteLine($"LastWriteTime {v.LastWriteTime.ToShortDateString()}");
            //    string s = $"{v.Name}{v.Length}{v.LastWriteTime.ToString()}";
            //    Console.WriteLine($"GetHashCode {s.GetHashCode()}");
            //}


            //FileNameCompare myFileNameCompare = new FileNameCompare();
            //var queryNameList1Only = (from file in list1
            //                      select file).Except(list2, myFileNameCompare);
            //Console.WriteLine("The following Name files are in list1 but not list2:");
            //foreach (var v in queryNameList1Only)
            //{
            //    Console.WriteLine($"FullName {v.FullName}");
            //    Console.WriteLine($"Length {GetSizeByte(v)}");
            //    Console.WriteLine($"LastWriteTime {v.LastWriteTime.ToShortDateString()}");
            //    string s = $"{v.Name}{v.Length}{v.LastWriteTime.ToString()}";
            //    Console.WriteLine($"GetHashCode {s.GetHashCode()}");
            //}

        }

        private static string GetSizeByte(FileInfo filename)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = filename.Length;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }

        // This implementation defines a very simple comparison  
        // between two FileInfo objects. It only compares the name  
        // of the files being compared and their length in bytes.  
        private class FileCompare : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo>
        {
            public FileCompare() { }

            public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2)
            {
                return (f1.Name == f2.Name &&
                        f1.Length == f2.Length &&
                        f1.LastWriteTime.ToString() == f2.LastWriteTime.ToString()
                        );
            }

            // Return a hash that reflects the comparison criteria. According to the
            // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must  
            // also be equal. Because equality as defined here is a simple value equality, not  
            // reference identity, it is possible that two or more objects will produce the same  
            // hash code.  
            public int GetHashCode(System.IO.FileInfo fi)
            {
                string s = $"{fi.Name}{fi.Length}{fi.LastWriteTime.ToString()}";
                return s.GetHashCode();
            }
        }

        private class FileNameCompare : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo>
        {
            public FileNameCompare() { }

            public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2)
            {
                return (f1.Name == f2.Name);
            }

            // Return a hash that reflects the comparison criteria. According to the
            // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must  
            // also be equal. Because equality as defined here is a simple value equality, not  
            // reference identity, it is possible that two or more objects will produce the same  
            // hash code.  
            public int GetHashCode(System.IO.FileInfo fi)
            {
                string s = $"{fi.Name}";
                return s.GetHashCode();
            }
        }
    }
}
