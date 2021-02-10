using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FamilyTree
{
    class CRUD
    {
        public string DatabaseName { get; set; } = "Humans";
        public void Create(Person person)
        {
            var db = new SQLDatabase();

            var connString = string.Format(db.ConnectionString, DatabaseName);
            using (var cnn = new SqlConnection(connString))
            {
                cnn.Open();
                var sql = "INSERT INTO People (firstName, lastName, birthDate, deathDate, motherId, fatherId) VALUES(@FirstName, @LastName, @BirthDate, @DeathDate @MotherId, @FatherId)";
                using (var command = new SqlCommand(sql, cnn))
                {
                    command.Parameters.AddWithValue("@firstName", person.FirstName);
                    command.Parameters.AddWithValue("@lastName", person.LastName);
                    command.Parameters.AddWithValue("@birthDate", person.BirthDate);
                    command.Parameters.AddWithValue("@deathDate", person.DeathDate);
                    command.Parameters.AddWithValue("@motherId", person.MotherId);
                    command.Parameters.AddWithValue("@fatherId", person.FatherId);
                    command.ExecuteNonQuery();
                }
            }
        }
        public Person Read(int id)
        {
            var db = new SQLDatabase
            {
                DatabaseName = DatabaseName
            };
            var row = db.GetDataTable("SELECT TOP 1 * from People Where firstName LIKE @id", ("@id", id.ToString()));
            if (row.Rows.Count == 0)
                return null;

            return GetPersonObject(row.Rows[0]);
        }
        public Person Read(string name)
        {
            var db = new SQLDatabase
            {
                DatabaseName = DatabaseName
            };

            DataTable dt;
            if (name.Contains(" "))
            {
                var names = name.Split(' ');
                dt = db.GetDataTable("SELECT TOP 1 * from People Where firstName LIKE @fname AND lastName LIKE @lname",
                    ("@fname", names[0]),
                    ("@lname", names[^1])
                    );
            }
            else
            {
                dt = db.GetDataTable("SELECT TOP 1 * from People Where firstName LIKE @name OR lastName LIKE @name ", ("@name", name));
            }

            if (dt.Rows.Count == 0)
                return null;

            return GetPersonObject(dt.Rows[0]);
        }
        private static Person GetPersonObject(DataRow row)
        {
            return new Person
            {
                FirstName = row["firstName"].ToString(),
                LastName = row["lastName"].ToString(),
                BirthDate = row["birthDate"].ToString(),
                DeathDate = row["deathDate"].ToString(),
                MotherId = (int)row["motherId"],
                FatherId = (int)row["fatherId"]
            };
        }
        public void Update(Person person)
        {
            var db = new SQLDatabase
            {
                DatabaseName = DatabaseName
            };
            db.ExecuteSQL(@"
                Update People SET
                FirstName=@FirstName, LastName=@LastName, BirthDate=@BirthDate, DeathDate=@DeathDate, MotherId=@MotherId, FatherId=@FatherId
                WHERE Id = @id",
                ("@FirstName", person.FirstName),
                ("@LastName", person.LastName),
                ("@BirthDate", person.BirthDate),
                ("@DeathDate", person.DeathDate),
                ("@MotherId", person.MotherId.ToString()),
                ("@FatherId", person.FatherId.ToString())
                );
        }
        public void Delete(Person person)
        {
            var db = new SQLDatabase
            {
                DatabaseName = DatabaseName
            };
            db.ExecuteSQL("DELETE FROM People Where Id=@id",
                         ("@Id", person.Id.ToString())
                         );
        }
        public void Delete(string name)
        {
            var person = Read(name);
            if (person != null) Delete(person);
        }
        public List<Person> List(string filter = "", string orderBy = "lastName", int max = 10)
        {
            var db = new SQLDatabase
            {
                DatabaseName = DatabaseName
            };
            var sql = "SELECT";
            if (max > 0) sql += " TOP " + max.ToString();
            sql += "* From People";
            if (filter != "") sql += "WHERE " + filter;
            if (orderBy != "") sql += " ORDER BY " + orderBy;
            var data = db.GetDataTable(sql);
            var lst = new List<Person>();
            foreach (DataRow row in data.Rows)
            {
                lst.Add(GetPersonObject(row));
            }
            return lst;
        }

    }
}
