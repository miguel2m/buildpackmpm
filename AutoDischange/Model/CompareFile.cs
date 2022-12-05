﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.Model
{
    public class CompareFile
    {
        public static string GetSizeByte(FileInfo filename)
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
    }

    // This implementation defines a very simple comparison  
    // between two FileInfo objects. It only compares the name  
    // of the files being compared and their length in bytes.  
    public class FileCompare : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo>
    {
        public FileCompare() { }

        public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2)
        {
            string f1Path = f1.Name;
            if (f1.FullName.Contains(@"\Alojables"))
            {
                f1Path = f1.FullName.Split(new[] { @"\Alojables" }, StringSplitOptions.None)[1];
            }
            if (f1.FullName.Contains(@"\Configurables"))
            {
                f1Path = f1.FullName.Split(new[] { @"\Configurables" }, StringSplitOptions.None)[1];
            }
            if (f1.FullName.Contains(@"\Script"))
            {
                f1Path = f1.FullName.Split(new[] { @"\Script" }, StringSplitOptions.None)[1];
            }
            string f2Path = f2.Name;
            if (f2.FullName.Contains(@"\Alojables"))
            {
                f2Path = f1.FullName.Split(new[] { @"\Alojables" }, StringSplitOptions.None)[1];
            }
            if (f2.FullName.Contains(@"\Configurables"))
            {
                f2Path = f1.FullName.Split(new[] { @"\Configurables" }, StringSplitOptions.None)[1];
            }
            if (f2.FullName.Contains(@"\Script"))
            {
                f2Path = f1.FullName.Split(new[] { @"\Script" }, StringSplitOptions.None)[1];
            }
            //return (f1Path == f2Path);
            return (f1Path == f2Path &&
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

    public class FileNameCompare : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo>
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
        public int GetHashCode(System.IO.FileInfo f1)
        {
            //string s = $"{fi.Name}";
            string f1Path = f1.Name;
            if (f1.FullName.Contains(@"\Alojables"))
            {
                f1Path = f1.FullName.Split(new[] { @"\Alojables" }, StringSplitOptions.None)[1];
            }
            if (f1.FullName.Contains(@"\Configurables"))
            {
                f1Path = f1.FullName.Split(new[] { @"\Configurables" }, StringSplitOptions.None)[1];
            }
            if (f1.FullName.Contains(@"\Script"))
            {
                f1Path = f1.FullName.Split(new[] { @"\Script" }, StringSplitOptions.None)[1];
            }
            return f1Path.GetHashCode();
        }
    }

    public class FileFullNameCompare : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo>
    {
        public FileFullNameCompare() { }

        public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2)
        {
            string f1Path = f1.Name;
            if (f1.FullName.Contains(@"\Alojables"))
            {
                f1Path = f1.FullName.Split(new[] { @"\Alojables" }, StringSplitOptions.None)[1];
            }
            if (f1.FullName.Contains(@"\Configurables"))
            {
                f1Path = f1.FullName.Split(new[] { @"\Configurables" }, StringSplitOptions.None)[1];
            }
            if (f1.FullName.Contains(@"\Script"))
            {
                f1Path = f1.FullName.Split(new[] { @"\Script" }, StringSplitOptions.None)[1];
            }
            string f2Path = f2.Name;
            if (f2.FullName.Contains(@"\Alojables"))
            {
                f2Path = f1.FullName.Split(new[] { @"\Alojables" }, StringSplitOptions.None)[1];
            }
            if (f2.FullName.Contains(@"\Configurables"))
            {
                f2Path = f1.FullName.Split(new[] { @"\Configurables" }, StringSplitOptions.None)[1];
            }
            if (f2.FullName.Contains(@"\Script"))
            {
                f2Path = f1.FullName.Split(new[] { @"\Script" }, StringSplitOptions.None)[1];
            }
            return (f1Path == f2Path);
        }

        // Return a hash that reflects the comparison criteria. According to the
        // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must  
        // also be equal. Because equality as defined here is a simple value equality, not  
        // reference identity, it is possible that two or more objects will produce the same  
        // hash code.  
        public int GetHashCode(System.IO.FileInfo f1)
        {
            //string s = $"{fi.Name}";
            string f1Path = f1.Name;
            if (f1.FullName.Contains(@"\Alojables"))
            {
                f1Path = f1.FullName.Split(new[] { @"\Alojables" }, StringSplitOptions.None)[1];
            }
            if (f1.FullName.Contains(@"\Configurables"))
            {
                f1Path = f1.FullName.Split(new[] { @"\Configurables" }, StringSplitOptions.None)[1];
            }
            if (f1.FullName.Contains(@"\Script"))
            {
                f1Path = f1.FullName.Split(new[] { @"\Script" }, StringSplitOptions.None)[1];
            }
            return f1Path.GetHashCode();
        }
    }
}
