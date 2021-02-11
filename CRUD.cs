using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FamilyTree
{
    class CRUD
    {
        internal SQLDatabase SqlDatabase { get; set; } = new SQLDatabase("Genealogy");
        public void Create(Person person)
        {
            var sql = "INSERT INTO Relatives (firstName, lastName, birthDate, deathDate, motherId, fatherId) VALUES(@FirstName, @LastName, @BirthDate, @DeathDate, @MotherId, @FatherId)";
            (string, string)[] parameters = FillParameters(person);
            SqlDatabase.ExecuteSQL(sql, parameters);
            GiveIdToPersonObject(person);
        }

        private (string, string)[] FillParameters(Person person)
        {
            return new (string, string)[]
            {
            ("@FirstName", person.FirstName),
            ("@LastName", person.LastName),
            ("@BirthDate", person.BirthDate),
            ("@DeathDate", person.DeathDate),
            ("@MotherId", person.MotherId.ToString()),
            ("@FatherId", person.FatherId.ToString())
            };
        }

        public void GiveIdToPersonObject(Person person)
        {
            var dt = new DataTable();
            dt = SqlDatabase.GetDataTable("SELECT Id FROM Relatives WHERE FirstName = @firstName AND LastName = @lastName AND BirthDate = @birthDate",  
                ("@firstName", person.FirstName), ("@lastName", person.LastName), ("@birthDate", person.BirthDate));
         
            person.Id = (int)dt.Rows[0]["Id"];
        }

        public Person Read(int id)
        {

            var row = SqlDatabase.GetDataTable("SELECT TOP 1 * from Relatives Where Id LIKE @id", ("@id", id.ToString()));
            if (row.Rows.Count == 0)
                return null;

            return GetPersonObject(row.Rows[0]);
        }
        public Person Read(string name)
        {
            DataTable dt;
            if (name.Contains(" "))
            {
                var names = name.Split(' ');
                dt = SqlDatabase.GetDataTable("SELECT TOP 1 * from Relatives Where firstName LIKE @fname AND lastName LIKE @lname",
                    ("@fname", names[0]),
                    ("@lname", names[^1])
                    );
            }
            else
            {
                dt = SqlDatabase.GetDataTable("SELECT TOP 1 * from Relatives Where firstName LIKE @name OR lastName LIKE @name ", ("@name", name));
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

            SqlDatabase.ExecuteSQL(@"
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
            SqlDatabase.ExecuteSQL("DELETE FROM People Where Id=@id",
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

            var sql = "SELECT";
            if (max > 0) sql += " TOP " + max.ToString();
            sql += "* From Relatives";
            if (filter != "") sql += "WHERE " + filter;
            if (orderBy != "") sql += " ORDER BY " + orderBy;
            var data = SqlDatabase.GetDataTable(sql);
            var lst = new List<Person>();
            foreach (DataRow row in data.Rows)
            {
                lst.Add(GetPersonObject(row));
            }
            return lst;
        }

    }
}
