using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace FamilyTree
{
    class SQLDatabase
    {
        internal string ConnectionString { get; } = @"Data Source=.\SQLExpress;Integrated Security=true;database={0}";
        internal string DatabaseName { get; set; }

        internal DataTable GetDataTable(string sqlString, params (string, string)[] parameters)
        {
            var dt = new DataTable();
            var connString = string.Format(ConnectionString, DatabaseName);
            using (var cnn = new SqlConnection(connString))
            {
                cnn.Open();
                using (var command = new SqlCommand(sqlString, cnn))
                {
                    SetParameters(parameters, command);
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
                cnn.Close();
            }
            return dt;
        }
        internal long ExecuteSQL(string sqlString, params (string, string)[] parameters)
        {
            long rowsAffected = 0;
            var connString = string.Format(ConnectionString, DatabaseName);
            using (var cnn = new SqlConnection(connString))
            {
                cnn.Open();
                using (var command = new SqlCommand(sqlString, cnn))
                {
                    SetParameters(parameters, command);
                    rowsAffected = command.ExecuteNonQuery();
                }
                cnn.Close();
            }
            return rowsAffected;
        }

        private static void SetParameters((string, string)[] parameters, SqlCommand command)
        {
            foreach (var item in parameters)
            {
                command.Parameters.AddWithValue(item.Item1, item.Item2);
            }
        }
        internal List<string> GetDatabaseFiles()
        {
            var list = new List<string>();
            var files = GetDataTable("SELECT physical_name, size FROM sys.database_files");
            foreach (DataRow row in files.Rows)
            {
                list.Add($"{row["physical_name"].ToString().Trim()}, {row["size"]}");
            }
            return list;
        }
        internal List<string> GetDatabases()
        {
            var list = new List<string>();
            var files = GetDataTable("SELECT name FROM sys.databases");
            foreach (DataRow row in files.Rows)
            {
                list.Add($"{row["name"].ToString().Trim()}");
            }
            return list;
        }
        internal bool DoesDatabaseExist(string name)
        {
            var theDB = GetDataTable("SELECT name FROM sys.databases Where name = @name", ("@name", name));
            return theDB?.Rows.Count > 0;
        }
        internal void ImportSQL(string filename)
        {
            if (File.Exists(filename))
            {
                var sql = File.ReadAllText(filename);
                ExecuteSQL(sql);
            }
        }
        internal void CreateDatabase(string name /*bool OpenNewDatabase = false*/)
        {
            if (DoesDatabaseExist(name))
            {
                Console.WriteLine("Database already exists!");
            }
            else
            {
                ExecuteSQL("CREATE DATABASE " + name);
                Console.WriteLine("Database created!");
                DatabaseName = name;
                //if (OpenNewDatabase) DatabaseName = name;
            }
        }
        internal void DropDatabase(string name)
        {
            DatabaseName = "Master";

            // Database is being used issue - https://stackoverflow.com/a/20569152/15032536
            ExecuteSQL(" alter database [" + name + "] set single_user with rollback immediate");

            ExecuteSQL("DROP DATABASE " + name);
        }
        internal void DropTable(string name)
        {
            ExecuteSQL($"DROP TABLE {name};");
        }
        internal void AlterTable(string name, string fields)
        {
            ExecuteSQL($"ALTER TABLE {name} {fields};");
        }
        internal void AlterTableAdd(string name, string fields)
        {
            ExecuteSQL($"ALTER TABLE {name} ADD {fields};");
        }
        internal void AlterTableDrop(string name, string field)
        {
            ExecuteSQL($"ALTER TABLE {name} DROP COLUMN {field};");
        }
        internal void CreateTable(string name, string fields)
        {
            if (DoesTableExists(name))
            {
                Console.WriteLine("Table already exists!");
            }
            else
            {
                ExecuteSQL($"CREATE TABLE {name} ({fields});");
                Console.WriteLine("Table created!");
            }
        }
        internal bool DoesTableExists(string name)
        {
            var table = GetDataTable("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @name", ("@name", name));
            return table?.Rows.Count > 0;
        }


    }
}
