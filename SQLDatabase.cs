using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace FamilyTree
{
    internal class SQLDatabase
    {
        internal string ConnectionString { get; } = @"Data Source=.\SQLExpress;Integrated Security=true;database={0}";
        internal string DatabaseName { get; set; }

        public SQLDatabase()
        {
        }

        public SQLDatabase(string databaseName)
        {
            DatabaseName = databaseName;
        }

        /// <summary>
        /// hämtar en datatabell från databasen
        /// </summary>

        internal DataTable GetDataTable(string sqlString, params (string, string)[] parameters)
        {
            var connString = string.Format(ConnectionString, DatabaseName);
            var dt = new DataTable();
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

        /// <summary>
        /// Execute Sql code (without returning table)
        /// </summary>

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

        /// <summary>
        ///metod för att lägga till parametrar till ett sql command.
        /// </summary>
        private static void SetParameters((string, string)[] parameters, SqlCommand command)
        {
            foreach (var item in parameters)
            {
                command.Parameters.AddWithValue(item.Item1, item.Item2);
            }
        }

        /// <summary>
        /// hämtar en lista på alla filer användna av databasen
        /// </summary>

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

        /// <summary>
        /// hämtar en lista med alla tillgängliga databaser på servern
        /// </summary>
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

        /// <summary>
        /// kollar ifall databasen existerar
        /// </summary>

        internal bool DoesDatabaseExist(string name)
        {
            DataTable dt = new DataTable();
            dt = GetDataTable("SELECT name FROM sys.databases Where name = @name", ("@name", name));
            return dt?.Rows.Count > 0;
        }

        /// <summary>
        /// importerar en sql fil och exekverar dess innehåll
        /// </summary>
        internal void ImportSQL(string filename)
        {
            if (File.Exists(filename))
            {
                var sql = File.ReadAllText(filename);
                ExecuteSQL(sql);
            }
        }

        /// <summary>
        /// skapar en databas om inte det redan finns en databas med samma namn
        /// </summary>

        internal void CreateDatabase(string name)
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
            }
        }

        /// <summary>
        /// tar bort en databas
        /// </summary>

        internal void DropDatabase(string name)
        {
            DatabaseName = "Master";
            var connString = string.Format(ConnectionString, DatabaseName);

            // Database is being used issue - https://stackoverflow.com/a/20569152/15032536
            ExecuteSQL(" alter database [" + name + "] set single_user with rollback immediate");

            ExecuteSQL("DROP DATABASE " + name);
        }

        /// <summary>
        /// tar bort en tabell
        /// </summary>
        internal void DropTable(string name)
        {
            ExecuteSQL($"DROP TABLE {name};");
        }

        /// <summary>
        /// ändrar en tabell
        /// </summary>

        internal void AlterTable(string name, string fields)
        {
            ExecuteSQL($"ALTER TABLE {name} {fields};");
        }

        /// <summary>
        /// ändrar en tabell och lägger till fält
        /// </summary>

        internal void AlterTableAdd(string name, string fields)
        {
            ExecuteSQL($"ALTER TABLE {name} ADD {fields};");
        }

        /// <summary>
        /// ändrar en tabell och tar bort fält
        /// </summary>

        internal void AlterTableDrop(string name, string field)
        {
            ExecuteSQL($"ALTER TABLE {name} DROP COLUMN {field};");
        }

        /// <summary>
        /// skapar en tabell om den inte finns redan
        /// </summary>

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

        /// <summary>
        /// kollar om en tabell redan finns
        /// </summary>

        internal bool DoesTableExists(string name)
        {
            var table = GetDataTable("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @name", ("@name", name));
            return table?.Rows.Count > 0;
        }
    }
}