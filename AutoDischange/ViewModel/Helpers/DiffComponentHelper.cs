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

            //Los que estan A y B
            List<DiffCompareModel> diffCompareModelList = new List<DiffCompareModel>();
            DiffCompareModel diffCompareModel;
            int count = 1;
            foreach (var v in queryList1Intersect)
            {
                string s = $"{v.Name}{v.Length}{v.LastWriteTime.ToString()}";
                string sHash = s.GetHashCode().ToString();
                string sizeAll = GetSizeByte(v);

                diffCompareModel = new DiffCompareModel();
                diffCompareModel.Id = count;
                //PathA
                diffCompareModel.UbicacionA = v.FullName;
                diffCompareModel.PathA = v.Name;
                diffCompareModel.HashA = sHash;
                diffCompareModel.FechaA = v.LastWriteTime;
                diffCompareModel.LenghtA = sizeAll;
                //PathB
                var queryList2IntersectTemp = (from file in list2
                                            select file).Intersect(list1, myFileCompare).Where(i => i.Name == v.Name).First();
                diffCompareModel.UbicacionB = queryList2IntersectTemp.FullName;
                diffCompareModel.PathB = queryList2IntersectTemp.Name;
                diffCompareModel.HashB = sHash;
                diffCompareModel.FechaB = v.LastWriteTime;
                diffCompareModel.LenghtB = sizeAll;
                //Result
                diffCompareModel.HashResult = 1;
                diffCompareModel.FechaResult = 1;
                diffCompareModel.LenghtResult = 1;

                diffCompareModelList.Add(diffCompareModel);

                count++;
            }

            //Los que estan solo en A
            FileNameCompare myFileNameCompare = new FileNameCompare();
            var queryNameList1Only = (from file in list1
                                      select file).Except(list2, myFileNameCompare);
            foreach (var v in queryNameList1Only)
            {
                string s = $"{v.Name}{v.Length}{v.LastWriteTime.ToString()}";
                string sHash = s.GetHashCode().ToString();
                string sizeAll = GetSizeByte(v);

                diffCompareModel = new DiffCompareModel();
                diffCompareModel.Id = count;
                //PathA
                diffCompareModel.UbicacionA = v.FullName;
                diffCompareModel.PathA = v.Name;
                diffCompareModel.HashA = sHash;
                diffCompareModel.FechaA = v.LastWriteTime;
                diffCompareModel.LenghtA = sizeAll;
                //PathB
                diffCompareModel.UbicacionB = String.Empty;
                diffCompareModel.PathB = String.Empty;
                diffCompareModel.HashB = String.Empty;
                diffCompareModel.FechaB = DateTime.Now;
                diffCompareModel.LenghtB = String.Empty;
                //Result
                diffCompareModel.HashResult = 5;
                diffCompareModel.FechaResult = 5;
                diffCompareModel.LenghtResult = 5;

                diffCompareModelList.Add(diffCompareModel);

                count++;
            }

            //Los que estan en A y B pero no son giuales

            var queryList1Only = (from file in list1
                                  select file).Except(queryNameList1Only, myFileCompare); //Los que estan en A y B

            queryList1Only = (from file in queryList1Only
                              select file).Except(list2, myFileCompare); //Los que estan en A y B pero no son iguales en HASH LastWriteTime
            foreach (var v in queryList1Only)
            {
                string s = $"{v.Name}{v.Length}{v.LastWriteTime.ToString()}";
                string sHash = s.GetHashCode().ToString();
                string sizeAll = GetSizeByte(v);

                diffCompareModel = new DiffCompareModel();
                diffCompareModel.Id = count;
                //PathA
                diffCompareModel.UbicacionA = v.FullName;
                diffCompareModel.PathA = v.Name;
                diffCompareModel.HashA = sHash;
                diffCompareModel.FechaA = v.LastWriteTime;
                diffCompareModel.LenghtA = sizeAll;
                //PathB
                var queryList2IntersectTemp = (from file in list2
                                               select file).Intersect(queryList1Only, myFileNameCompare).Where(i => i.Name == v.Name).First();

                s = $"{queryList2IntersectTemp.Name}{queryList2IntersectTemp.Length}{queryList2IntersectTemp.LastWriteTime.ToString()}";
                sHash = queryList2IntersectTemp.GetHashCode().ToString();
                sizeAll = GetSizeByte(queryList2IntersectTemp);

                diffCompareModel.UbicacionB = queryList2IntersectTemp.FullName;
                diffCompareModel.PathB = queryList2IntersectTemp.Name;
                diffCompareModel.HashB = sHash;
                diffCompareModel.FechaB = queryList2IntersectTemp.LastWriteTime;
                diffCompareModel.LenghtB = sizeAll;
                //Result
                diffCompareModel.HashResult = diffCompareModel.HashA.CompareTo(diffCompareModel.HashB) > 0 ?
                    diffCompareModel.HashA.CompareTo(diffCompareModel.HashB) == 0 ? 2 : 3
                    : 4; // 2= Iguales , 3= A es mayor B y 4= B es mayor A
                diffCompareModel.FechaResult = DateTime.Compare(diffCompareModel.FechaA, diffCompareModel.FechaB)  > 0 ?
                    DateTime.Compare(diffCompareModel.FechaA, diffCompareModel.FechaB) == 0?2 : 3
                    :4; // 2= Iguales , 3= A es mayor B y 4= B es mayor A
                diffCompareModel.LenghtResult = diffCompareModel.HashA.CompareTo(diffCompareModel.LenghtA) > 0 ?
                    diffCompareModel.HashA.CompareTo(diffCompareModel.LenghtB) == 0 ? 2 : 3
                    : 4; // 2= Iguales , 3= A es mayor B y 4= B es mayor A

                diffCompareModelList.Add(diffCompareModel);

                count++;
            }

            ExcelHelper.CreateExcelDiffComapre(diffCompareModelList, pathUser, diffComponent);

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
