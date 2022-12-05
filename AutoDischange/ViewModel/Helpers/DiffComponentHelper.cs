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
        public static async Task<bool> DiffFiles(DiffComponent diffComponent, string pathUser)
        {
            char backSlash = Path.DirectorySeparatorChar;
            string pathAlojablesStart = $"{@diffComponent.PathStart}{backSlash}Alojables";
            string pathConfigurablesStart = $"{@diffComponent.PathStart}{backSlash}Configurables";
            string pathScriptStart = $"{@diffComponent.PathStart}{backSlash}Scripts";

            string pathAlojablesEnd = $"{@diffComponent.PathEnd}{backSlash}Alojables";
            string pathConfigurablesEnd = $"{@diffComponent.PathEnd}{backSlash}Configurables";
            string pathScriptEnd = $"{@diffComponent.PathEnd}{backSlash}Scripts";

            if (!Directory.Exists(pathAlojablesStart) && !Directory.Exists(pathConfigurablesStart) && !Directory.Exists(pathScriptStart))
            {
                Log4net.log.Error("Debe seleccionar carpeta de componentes para el paquete Inicio");
                throw new DirectoryNotFoundException("Debe seleccionar carpeta de componentes para el paquete Inicio");

            }

            if (!Directory.Exists(pathAlojablesEnd) && !Directory.Exists(pathConfigurablesEnd) && !Directory.Exists(pathScriptEnd))
            {
                Log4net.log.Error("Debe seleccionar carpeta de componentes para el paquete a comparar");
                throw new DirectoryNotFoundException("Debe seleccionar carpeta de componentes para el paquete a comparar (Paquete B)");

            }

            System.IO.DirectoryInfo dir1 = new System.IO.DirectoryInfo(@diffComponent.PathStart);
                System.IO.DirectoryInfo dir2 = new System.IO.DirectoryInfo(@diffComponent.PathEnd);

                // Take a snapshot of the file system.  
                IEnumerable<System.IO.FileInfo> list1 = dir1.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
                IEnumerable<System.IO.FileInfo> list2 = dir2.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

                //A custom file comparer defined below  
                FileCompare myFileCompare = new FileCompare();
                FileNameCompare myFileNameCompare = new FileNameCompare();
            FileFullNameCompare myFileFullNameCompare = new FileFullNameCompare();


            //Los que estan A y B
            List<DiffCompareModel> diffCompareModelList = new List<DiffCompareModel>();
                List<DiffCompareModel> diffCompareModelListIguales = new List<DiffCompareModel>();
                List<DiffCompareModel> diffCompareModelListDiferentes = new List<DiffCompareModel>();
                List<DiffCompareModel> diffCompareModelListHuerfanos = new List<DiffCompareModel>();
                List<DiffCompareModel> diffCompareModelListHuerfanosB = new List<DiffCompareModel>();
            DiffCompareModel diffCompareModel;
                string s;
                string sHash;
                string sizeAll;
                int count = 1;

                Task<IEnumerable<System.IO.FileInfo>> task0 = new Task<IEnumerable<System.IO.FileInfo>>(() =>
                {
                    //Los que estan solo en A y B
                    return (from file in list1
                            select file).Intersect(list2, myFileCompare);
                });
                task0.Start();
                IEnumerable<System.IO.FileInfo> queryList1Intersect = await task0;
                if (queryList1Intersect.Count() > 0)
                {
                    Task task1 = new Task(() =>
                    {
                    
                        foreach (var v in queryList1Intersect)
                        {
                            s = $"{v.Name}{v.Length}{v.LastWriteTime.ToString()}";
                            //sHash = s.GetHashCode().ToString();
                            sHash = MD5_Compare.CreateMD5(s);
                            //sizeAll = CompareFile.GetSizeByte(v);
                            sizeAll = v.Length.ToString();

                            diffCompareModel = new DiffCompareModel();
                            diffCompareModel.Id = count;
                            //PathA
                            //diffCompareModel.UbicacionA = v.FullName;
                            diffCompareModel.UbicacionA = v.FullName.Contains("CR")? v.FullName.Contains("CRs") ? v.FullName.Split(new[] { "CRs" }, StringSplitOptions.None)[1]: v.FullName.Split(new[] { "CR" }, StringSplitOptions.None)[1] : v.FullName;
                            diffCompareModel.PathA = v.Name;
                            diffCompareModel.HashA = sHash;
                            diffCompareModel.FechaA = v.LastWriteTime;
                            diffCompareModel.LenghtA = sizeAll;
                            //PathB
                            var queryList2IntersectTemp = (from file in list2
                                                           select file).Intersect(list1, myFileCompare).Where(i => i.Name == v.Name).First();
                            //diffCompareModel.UbicacionB = queryList2IntersectTemp.FullName;
                            diffCompareModel.UbicacionB = queryList2IntersectTemp.FullName.Contains("CR") ? queryList2IntersectTemp.FullName.Contains("CRs") ? queryList2IntersectTemp.FullName.Split(new[] { "CRs" }, StringSplitOptions.None)[1] : queryList2IntersectTemp.FullName.Split(new[] { "CR" }, StringSplitOptions.None)[1] : queryList2IntersectTemp.FullName;

                            diffCompareModel.PathB = queryList2IntersectTemp.Name;

                            diffCompareModel.HashB = sHash;
                            diffCompareModel.FechaB = v.LastWriteTime;
                            diffCompareModel.LenghtB = sizeAll;
                            //Result
                            diffCompareModel.HashResult = 1;
                            diffCompareModel.FechaResult = 1;
                            diffCompareModel.LenghtResult = 1;

                            //diffCompareModelList.Add(diffCompareModel);
                            diffCompareModelListIguales.Add(diffCompareModel);
                            count++;
                        }
                    });
                    task1.Start();
                    await task1;
                    if (diffCompareModelListIguales.Any())
                    {
                        diffCompareModelList.AddRange(diffCompareModelListIguales.OrderBy(o => o.UbicacionA).ToList());
                    }
                }

            
                Task<IEnumerable<System.IO.FileInfo>> task2 = new Task<IEnumerable<System.IO.FileInfo>>(() =>
                {
                    //Los que estan solo en A
                    return (from file in list1 select file).Intersect(list2, myFileFullNameCompare);
                });
                task2.Start();
                IEnumerable<System.IO.FileInfo> queryNameList1Only = await task2;


            if (queryNameList1Only.Count() > 0)
            {
                Task task5 = new Task(() =>
                {
                    foreach (var v in queryNameList1Only)
                    {
                        if (!diffCompareModelListIguales.Exists(e => e.PathA.Equals(v.Name)))
                        {
                            s = $"{v.Name}{v.Length}{v.LastWriteTime.ToString()}";
                            //sHash = s.GetHashCode().ToString();
                            sHash = MD5_Compare.CreateMD5(s);
                            //sizeAll = CompareFile.GetSizeByte(v);
                            sizeAll = v.Length.ToString();
                            diffCompareModel = new DiffCompareModel();
                            diffCompareModel.Id = count;
                            //PathA
                            //diffCompareModel.UbicacionA = v.FullName;
                            diffCompareModel.UbicacionA = v.FullName.Contains("CR") ? v.FullName.Contains("CRs") ? v.FullName.Split(new[] { "CRs" }, StringSplitOptions.None)[1] : v.FullName.Split(new[] { "CR" }, StringSplitOptions.None)[1] : v.FullName;
                            diffCompareModel.PathA = v.Name;
                            diffCompareModel.HashA = sHash;
                            diffCompareModel.FechaA = v.LastWriteTime;
                            diffCompareModel.LenghtA = sizeAll;
                            //PathB
                            List<System.IO.FileInfo> queryList2IntersectTempList = (from file in list2
                                                                                    select file).Intersect(queryNameList1Only, myFileNameCompare).Where(i => i.Name == v.Name).ToList();
                            if (queryList2IntersectTempList.Any())
                            {
                                var queryList2IntersectTemp = queryList2IntersectTempList.First();
                                s = $"{queryList2IntersectTemp.Name}{queryList2IntersectTemp.Length}{queryList2IntersectTemp.LastWriteTime.ToString()}";
                                //sHash = queryList2IntersectTemp.GetHashCode().ToString();
                                sHash = MD5_Compare.CreateMD5(s);
                                //sizeAll = CompareFile.GetSizeByte(queryList2IntersectTemp);
                                sizeAll = queryList2IntersectTemp.Length.ToString();

                                //diffCompareModel.UbicacionB = queryList2IntersectTemp.FullName;
                                diffCompareModel.UbicacionB = queryList2IntersectTemp.FullName.Contains("CR") ? queryList2IntersectTemp.FullName.Contains("CRs") ? queryList2IntersectTemp.FullName.Split(new[] { "CRs" }, StringSplitOptions.None)[1] : queryList2IntersectTemp.FullName.Split(new[] { "CR" }, StringSplitOptions.None)[1] : queryList2IntersectTemp.FullName;
                                diffCompareModel.PathB = queryList2IntersectTemp.Name;
                                diffCompareModel.HashB = sHash;
                                diffCompareModel.FechaB = queryList2IntersectTemp.LastWriteTime;
                                diffCompareModel.LenghtB = sizeAll;
                                //Result
                                diffCompareModel.HashResult = diffCompareModel.HashA.CompareTo(diffCompareModel.HashB) > 0 ?
                                    diffCompareModel.HashA.CompareTo(diffCompareModel.HashB) == 0 ? 2 : 3
                                    : 4; // 2= Iguales , 3= A es mayor B y 4= B es mayor A
                                diffCompareModel.FechaResult = DateTime.Compare(diffCompareModel.FechaA, diffCompareModel.FechaB) > 0 ?
                                    DateTime.Compare(diffCompareModel.FechaA, diffCompareModel.FechaB) == 0 ? 2 : 3
                                    : 4; // 2= Iguales , 3= A es mayor B y 4= B es mayor A
                                diffCompareModel.LenghtResult = diffCompareModel.LenghtA.CompareTo(diffCompareModel.LenghtB) > 0 ?
                                    diffCompareModel.LenghtA.CompareTo(diffCompareModel.LenghtB) == 0 ? 2 : 3
                                    : 4; // 2= Iguales , 3= A es mayor B y 4= B es mayor A

                                //diffCompareModelList.Add(diffCompareModel);
                                diffCompareModelListDiferentes.Add(diffCompareModel);
                                count++;

                            }
                        }
                    }
                });
                task5.Start();
                await task5;

                if (diffCompareModelListDiferentes.Any())
                {
                    diffCompareModelList.AddRange(diffCompareModelListDiferentes.OrderBy(o => o.UbicacionA).ToList());
                }

            }

                //if (queryList1Intersect.Count() > 0 && queryNameList1Only.Count() > 0)
                //{
                //    //Los que estan en A y B pero no son giuales

                //    Task<IEnumerable<System.IO.FileInfo>> task3 = new Task<IEnumerable<System.IO.FileInfo>>(() =>
                //    {
                //        //Los que estan solo en A

                //        return (from file in list1
                //                select file).Except(queryNameList1Only, myFileCompare);  //Los que estan solo en A
                //    });
                //    task3.Start();
                //    IEnumerable<System.IO.FileInfo> queryList1Only = await task3;


                //    Task<IEnumerable<System.IO.FileInfo>> task4 = new Task<IEnumerable<System.IO.FileInfo>>(() =>
                //    {
                //        //Los que estan solo en A
                //        return (from file in queryList1Only
                //                select file).Except(list2, myFileCompare); //Los que estan en A y B pero no son iguales en HASH LastWriteTime
                //    });
                //    task4.Start();
                //    IEnumerable<System.IO.FileInfo> queryList1OnlyTemp = await task4;

                //    Task task5 = new Task(() =>
                //    {
                //        foreach (var v in queryList1OnlyTemp)
                //        {
                //            s = $"{v.Name}{v.Length}{v.LastWriteTime.ToString()}";
                //            //sHash = s.GetHashCode().ToString();
                //            sHash = MD5_Compare.CreateMD5(s);
                //            //sizeAll = CompareFile.GetSizeByte(v);
                //            sizeAll = v.Length.ToString();
                //            diffCompareModel = new DiffCompareModel();
                //            diffCompareModel.Id = count;
                //            //PathA
                //            //diffCompareModel.UbicacionA = v.FullName;
                //            diffCompareModel.UbicacionA = v.FullName.Contains("CR") ? v.FullName.Contains("CRs") ? v.FullName.Split(new[] { "CRs" }, StringSplitOptions.None)[1]: v.FullName.Split(new[] { "CR" }, StringSplitOptions.None)[1] : v.FullName;
                //            diffCompareModel.PathA = v.Name;
                //            diffCompareModel.HashA = sHash;
                //            diffCompareModel.FechaA = v.LastWriteTime;
                //            diffCompareModel.LenghtA = sizeAll;
                //            //PathB
                //            List<System.IO.FileInfo> queryList2IntersectTempList = (from file in list2
                //                                           select file).Intersect(queryList1Only, myFileNameCompare).Where(i => i.Name == v.Name).ToList();
                //            if (queryList2IntersectTempList.Any())
                //            {
                //                var queryList2IntersectTemp = queryList2IntersectTempList.First();
                //                s = $"{queryList2IntersectTemp.Name}{queryList2IntersectTemp.Length}{queryList2IntersectTemp.LastWriteTime.ToString()}";
                //                //sHash = queryList2IntersectTemp.GetHashCode().ToString();
                //                sHash = MD5_Compare.CreateMD5(s);
                //                //sizeAll = CompareFile.GetSizeByte(queryList2IntersectTemp);
                //                sizeAll = queryList2IntersectTemp.Length.ToString();

                //                //diffCompareModel.UbicacionB = queryList2IntersectTemp.FullName;
                //                diffCompareModel.UbicacionB = queryList2IntersectTemp.FullName.Contains("CR") ? queryList2IntersectTemp.FullName.Contains("CRs") ? queryList2IntersectTemp.FullName.Split(new[] { "CRs" }, StringSplitOptions.None)[1]: queryList2IntersectTemp.FullName.Split(new[] { "CR" }, StringSplitOptions.None)[1] : queryList2IntersectTemp.FullName;
                //                diffCompareModel.PathB = queryList2IntersectTemp.Name;
                //                diffCompareModel.HashB = sHash;
                //                diffCompareModel.FechaB = queryList2IntersectTemp.LastWriteTime;
                //                diffCompareModel.LenghtB = sizeAll;
                //                //Result
                //                diffCompareModel.HashResult = diffCompareModel.HashA.CompareTo(diffCompareModel.HashB) > 0 ?
                //                    diffCompareModel.HashA.CompareTo(diffCompareModel.HashB) == 0 ? 2 : 3
                //                    : 4; // 2= Iguales , 3= A es mayor B y 4= B es mayor A
                //                diffCompareModel.FechaResult = DateTime.Compare(diffCompareModel.FechaA, diffCompareModel.FechaB) > 0 ?
                //                    DateTime.Compare(diffCompareModel.FechaA, diffCompareModel.FechaB) == 0 ? 2 : 3
                //                    : 4; // 2= Iguales , 3= A es mayor B y 4= B es mayor A
                //                diffCompareModel.LenghtResult = diffCompareModel.LenghtA.CompareTo(diffCompareModel.LenghtB) > 0 ?
                //                    diffCompareModel.LenghtA.CompareTo(diffCompareModel.LenghtB) == 0 ? 2 : 3
                //                    : 4; // 2= Iguales , 3= A es mayor B y 4= B es mayor A

                //                //diffCompareModelList.Add(diffCompareModel);
                //                diffCompareModelListDiferentes.Add(diffCompareModel);
                //                count++;

                //            }
                //        }
                //    });
                //    task5.Start();
                //    await task5;

                //    if (diffCompareModelListDiferentes.Any())
                //    {
                //        diffCompareModelList.AddRange(diffCompareModelListDiferentes.OrderBy(o => o.UbicacionA).ToList());
                //    }
                //}

            Task<IEnumerable<System.IO.FileInfo>> task244 = new Task<IEnumerable<System.IO.FileInfo>>(() =>
            {
                //Los que estan solo en A
                return (from file in list1 select file).Except(list2, FileFullNameCompare);
            });
            task244.Start();
            queryNameList1Only = await task244;
            if (queryNameList1Only.Count() > 0)
                {
                    Task task6 = new Task(() =>
                    {
                        //Los que estan solo en A
                        foreach (var v in queryNameList1Only)
                        {
                            if (!diffCompareModelListDiferentes.Exists(e => e.PathA.Equals(v.Name)) )
                            {
                                s = $"{v.Name}{v.Length}{v.LastWriteTime.ToString()}";
                                //sHash = s.GetHashCode().ToString();
                                sHash = MD5_Compare.CreateMD5(s);
                                //sizeAll = CompareFile.GetSizeByte(v);
                                sizeAll = v.Length.ToString();
                                diffCompareModel = new DiffCompareModel();
                                diffCompareModel.Id = count;
                                //PathA
                                //diffCompareModel.UbicacionA = v.FullName;
                                diffCompareModel.UbicacionA = v.FullName.Contains("CR") ? v.FullName.Contains("CRs") ? v.FullName.Split(new[] { "CRs" }, StringSplitOptions.None)[1] : v.FullName.Split(new[] { "CR" }, StringSplitOptions.None)[1] : v.FullName;
                                diffCompareModel.PathA = v.Name;
                                diffCompareModel.HashA = sHash;
                                diffCompareModel.FechaA = v.LastWriteTime;
                                diffCompareModel.LenghtA = sizeAll;
                                //PathB
                                diffCompareModel.UbicacionB = String.Empty;
                                diffCompareModel.PathB = String.Empty;
                                diffCompareModel.HashB = String.Empty;
                                //diffCompareModel.FechaB = null;
                                diffCompareModel.LenghtB = String.Empty;
                                //Result
                                diffCompareModel.HashResult = 5;
                                diffCompareModel.FechaResult = 5;
                                diffCompareModel.LenghtResult = 5;

                                //diffCompareModelList.Add(diffCompareModel);
                                diffCompareModelListHuerfanos.Add(diffCompareModel);
                                count++;
                            }
                        }
                            
                    });
                    task6.Start();
                    await task6;

                    if (diffCompareModelListHuerfanos.Any())
                    {

                        
                        diffCompareModelList.AddRange(diffCompareModelListHuerfanos.OrderBy(o => o.UbicacionA).ToList());
                    }
                }

            Task<IEnumerable<System.IO.FileInfo>> task19 = new Task<IEnumerable<System.IO.FileInfo>>(() =>
            {
                //Los que estan solo en B (huerfanos B)
                return (from file in list2 select file).Except( list1, FileFullNameCompare);
            });
            task19.Start();
            IEnumerable<System.IO.FileInfo> queryNameList2Only = await task19;
            
            if (queryNameList2Only.Count() > 0)
            {
                Task task7 = new Task(() =>
                {
                    //Los que estan solo en A
                    foreach (var v in queryNameList2Only)
                    {
                        if (!diffCompareModelListDiferentes.Exists(e => e.PathB.Equals(v.Name)))
                        {
                            s = $"{v.Name}{v.Length}{v.LastWriteTime.ToString()}";
                            //sHash = s.GetHashCode().ToString();
                            sHash = MD5_Compare.CreateMD5(s);
                            //sizeAll = CompareFile.GetSizeByte(v);
                            sizeAll = v.Length.ToString();
                            diffCompareModel = new DiffCompareModel();
                            diffCompareModel.Id = count;
                            //PathA
                            //diffCompareModel.UbicacionA = v.FullName;
                            diffCompareModel.UbicacionB = v.FullName.Contains("CR") ? v.FullName.Contains("CRs") ? v.FullName.Split(new[] { "CRs" }, StringSplitOptions.None)[1] : v.FullName.Split(new[] { "CR" }, StringSplitOptions.None)[1] : v.FullName;
                            diffCompareModel.PathB = v.Name;
                            diffCompareModel.HashB = sHash;
                            diffCompareModel.FechaB = v.LastWriteTime;
                            diffCompareModel.LenghtB = sizeAll;
                            //PathB
                            diffCompareModel.UbicacionA = String.Empty;
                            diffCompareModel.PathA = String.Empty;
                            diffCompareModel.HashA = String.Empty;
                            //diffCompareModel.FechaB = null;
                            diffCompareModel.LenghtA = String.Empty;
                            //Result
                            diffCompareModel.HashResult = 6;
                            diffCompareModel.FechaResult = 6;
                            diffCompareModel.LenghtResult = 6;

                            //diffCompareModelList.Add(diffCompareModel);
                            diffCompareModelListHuerfanosB.Add(diffCompareModel);

                            count++;
                        }
                        
                    }
                });
                task7.Start();
                await task7;

                if (diffCompareModelListHuerfanosB.Any())
                {
                    
                    diffCompareModelList.AddRange(diffCompareModelListHuerfanosB.OrderBy(o => o.UbicacionB).ToList());
                    diffCompareModelListHuerfanos.AddRange(diffCompareModelListHuerfanosB.OrderBy(o => o.UbicacionB).ToList());
                }
            }

            ExcelHelper.CreateExcelDiffComapre(diffCompareModelList,
                    diffCompareModelListIguales,
                    diffCompareModelListDiferentes,
                    diffCompareModelListHuerfanos,
                    pathUser,
                    diffComponent);
                return true;
            
            

            

        }

        

        
    }
}
