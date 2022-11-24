using AutoDischange.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.ViewModel.Helpers
{
    public class DatabaseHelper
    {
        //Path DB
        //private static string dbDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{Path.DirectorySeparatorChar}Auto_pack";
        //public static string dbFile = Path.Combine(dbDirectory, "dischangesPath.db3");
        private static SQLiteOpenFlags dbFlag = SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite;
        private static string dbFile = Path.Combine(Environment.CurrentDirectory, "dischangesPath.db3");
        //private static void CreateDBFolder  (){
        //    if (!Directory.Exists(dbDirectory))
        //    {
        //        //Crear el directorio
        //        Directory.CreateDirectory(dbDirectory);
        //    }
        //}
        //INSERT INTO InsertDischange (Carga Masiva)
        public static async Task<bool> InsertDischange(DischangePath item)
        {
            
            bool result = false;
            ////CreateDBFolder();
            var db = new SQLiteAsyncConnection(dbFile, dbFlag, true);

            await db.CreateTableAsync<DischangePath>();
            int rows = await db.InsertAsync(item);
            if (rows > 0)
                result = true; 
            return result;
        }

        public static async Task Delete()
        {
            //CreateDBFolder();
            var db = new SQLiteAsyncConnection(dbFile, dbFlag, true);
            var ExistTable = await db.GetTableInfoAsync("DischangeChangeset");
            if (ExistTable.Count > 0)
            {
                await db.DeleteAllAsync<DischangeChangeset>();
            }
        }

        //INSERT INTO InsertDischange (Carga Masiva)
        public static async Task<bool> InsertReplaceDischange(DischangePath item)
        {

            bool result = false;
            //CreateDBFolder();
            var db = new SQLiteAsyncConnection(dbFile, dbFlag, true);

            await db.CreateTableAsync<DischangePath>();
            int rows = await db.InsertOrReplaceAsync(item);
            if (rows > 0)
                result = true;
            return result;
        }

        //INSERT INTO InsertDischange (Carga Masiva)
        public static async Task<bool> InsertReplaceChangeset(DischangeChangeset item)
        {
            bool result = false;
            //CreateDBFolder();
            var db = new SQLiteAsyncConnection(dbFile, dbFlag, true);

            await db.CreateTableAsync<DischangeChangeset>();
            int rows = await db.InsertOrReplaceAsync(item);
            if (rows > 0)
                result = true;
            return result;
        }

        //INSERT INTO Branch Jenkins
        public static async Task<bool> InsertReplaceBranchJenkinsExcel(BranchJenkinsExcel item)
        {
            bool result = false;
            //CreateDBFolder();
            var db = new SQLiteAsyncConnection(dbFile, dbFlag, true);

            await db.CreateTableAsync<BranchJenkinsExcel>();
            int rows = await db.InsertOrReplaceAsync(item);
            if (rows > 0)
                result = true;
            return result;
        }

        //INSERT INTO
        public static  bool Insert<T>(T item)
        {
            bool result = false;
            //CreateDBFolder();
            using (SQLiteConnection conn = new SQLiteConnection(dbFile, dbFlag, true))
            {
                conn.CreateTable<T>();
                int rows = conn.InsertOrReplace(item);
                if (rows > 0)
                    result = true;
            }

            return result;
        }

        //UPDATE INTO
        public static bool Update<T>(T item)
        {
            bool result = false;
            //CreateDBFolder();
            using (SQLiteConnection conn = new SQLiteConnection(dbFile, dbFlag, true))
            {
                conn.CreateTable<T>();
                int rows = conn.Update(item);
                if (rows > 0)
                    result = true;
            }

            return result;
        }

        //DELETE INTO ROW
        public static bool Delete<T>(T item)
        {
            bool result = false;
            //CreateDBFolder();
            using (SQLiteConnection conn = new SQLiteConnection(dbFile, dbFlag, true))
            {
                conn.CreateTable<T>();
                int rows = conn.Delete(item);
                if (rows > 0)
                    result = true;
            }

            return result;
        }

        //GET ROW
        public static List<T> Read<T>() where T : new()
        {
            List<T> items;
            //CreateDBFolder();
            using (SQLiteConnection conn = new SQLiteConnection(dbFile, dbFlag, true))
            {
                conn.CreateTable<T>();
                items = conn.Table<T>().ToList();
            }

            return items;
        }
    }
}
