using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog_on_DotNet;

namespace Catalog_on_DotNet
{
    class Sqlite
    {
        private static string connectionString = "Data Source=myCatalogDB.db;";
        //private static string connectionString = "Data Source=myCatalogDB_ForWinForm.db;Version=3;";

        public static SqliteConnection GetConnection()
        {
            string fullPath = Path.GetFullPath("myCatalogDB.db");
            
            return new SqliteConnection($"Data Source={fullPath}");
        }

        public static void ExecuteSql(string sql, SqliteConnection connection)
        {
            using (var command = new SqliteCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
