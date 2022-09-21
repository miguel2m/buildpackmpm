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
        private static string dbFile = Path.Combine(Environment.CurrentDirectory, "dischangesPath.db3");


        //INSERT INTO InsertDischange (Carga Masiva)
        public static async Task<bool> InsertDischange(DischangePath item)
        {
            
            bool result = false;

            var db = new SQLiteAsyncConnection(dbFile);

            await db.CreateTableAsync<DischangePath>();
            int rows = await db.InsertAsync(item);
            if (rows > 0)
                result = true; 
            return result;
        }

        public static async Task Delete()
        {
            var db = new SQLiteAsyncConnection(dbFile);
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

            var db = new SQLiteAsyncConnection(dbFile);

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

            var db = new SQLiteAsyncConnection(dbFile);

            await db.CreateTableAsync<DischangeChangeset>();
            int rows = await db.InsertOrReplaceAsync(item);
            if (rows > 0)
                result = true;
            return result;
        }

        //INSERT INTO
        public static  bool Insert<T>(T item)
        {
            bool result = false;

            using (SQLiteConnection conn = new SQLiteConnection(dbFile))
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

            using (SQLiteConnection conn = new SQLiteConnection(dbFile))
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

            using (SQLiteConnection conn = new SQLiteConnection(dbFile))
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

            using (SQLiteConnection conn = new SQLiteConnection(dbFile))
            {
                conn.CreateTable<T>();
                items = conn.Table<T>().ToList();
            }

            return items;
        }
    }
}
