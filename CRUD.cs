using System;
using System.Collections.Generic;
using System.Data;

namespace FamilyTree
{
    internal class CRUD
    {
        internal SQLDatabase SqlDatabase { get; set; } = new SQLDatabase("Genealogy");

        /// <summary>
        /// tar emot ett person objekt och skickar in det till databasen. Ifall personen redan finns i databasen, läggs personen inte in igen.
        /// </summary>
        public void Create(Person person)
        {
            var dt = new DataTable();
            var sql = "SELECT Id FROM Relatives WHERE FirstName = @firstName AND LastName = @lastName AND BirthDate = @birthDate";
            dt = SqlDatabase.GetDataTable(sql, ("@firstName", person.FirstName), ("@lastName", person.LastName), ("@birthDate", person.BirthDate));
            if (dt.Rows.Count == 0)
            {
                sql = "INSERT INTO Relatives (firstName, lastName, birthDate, deathDate, birthCity, deathCity, motherId, fatherId) " +
                    "VALUES(@FirstName, @LastName, @BirthDate, @DeathDate, @BirthCity, @DeathCity, @MotherId, @FatherId)";
                (string, string)[] parameters = FillParameters(person);
                SqlDatabase.ExecuteSQL(sql, parameters);
            }
            else
            {
                Console.WriteLine("Person already exists!");
            }
        }

        /// <summary>
        /// tar bort en person via ett person objekt
        /// </summary>
        public void Delete(Person person)
        {
            SqlDatabase.ExecuteSQL("DELETE FROM Relatives Where Id=@id",
                         ("@Id", person.Id.ToString())
                         );
        }

        /// <summary>
        /// tar bort en person via en sträng namn
        /// </summary>
        public void Delete(string name)
        {
            var person = Read(name);
            if (person != null) Delete(person);
        }

        /// <summary>
        /// hämtar en persons barn
        /// </summary>
        public List<Person> GetChildren(Person person)
        {
            var listOfChildrens = new List<Person>();
            DataTable dt = new DataTable();
            var sql = "SELECT * FROM Relatives WHERE MotherId = @Id OR FatherId = @Id";
            dt = SqlDatabase.GetDataTable(sql, ("@Id", person.Id.ToString()));
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    listOfChildrens.Add(GetPersonObject(row));
                }
            }
            return listOfChildrens;
        }

        /// <summary>
        /// hämtar en persons farföräldrar
        /// </summary>
        public void GetGrandParents(Person person, out Person dadsFather, out Person dadsMother, out Person momsFather, out Person momsMother)
        {
            var father = Read(person.FatherId);
            dadsFather = Read(father.FatherId);
            dadsMother = Read(father.MotherId);
            var mother = Read(person.MotherId);
            momsFather = Read(mother.FatherId);
            momsMother = Read(mother.MotherId);
        }

        /// <summary>
        /// sätter en persons föräldar
        /// </summary>
        public void GetParents(Person person, out Person mother, out Person father)
        {
            mother = Read(person.MotherId);
            father = Read(person.FatherId);
        }

        /// <summary>
        /// hämtar en persons syskon
        /// </summary>
        public List<Person> GetSiblings(Person person)
        {
            var listOfSiblings = new List<Person>();
            DataTable dt = new DataTable();
            var sql = "SELECT * FROM Relatives WHERE Id != @Id AND MotherId = @motherId AND FatherId = @fatherId";
            dt = SqlDatabase.GetDataTable(sql, ("@Id", person.Id.ToString()), ("@motherId", person.MotherId.ToString()), ("@fatherId", person.FatherId.ToString()));
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    listOfSiblings.Add(GetPersonObject(row));
                }
            }
            return listOfSiblings;
        }

        /// <summary>
        /// metod som tar emot ett person objekt och med hjälp av en datatable hämtar Idet från databasen och ger det till objektet.
        /// </summary>
        public void GiveIdToPersonObject(Person person)
        {
            var dt = new DataTable();
            dt = SqlDatabase.GetDataTable("SELECT Id FROM Relatives WHERE FirstName = @firstName AND LastName = @lastName AND BirthDate = @birthDate",
                ("@firstName", person.FirstName), ("@lastName", person.LastName), ("@birthDate", person.BirthDate));

            person.Id = (int)dt.Rows[0]["Id"];
        }

        /// <summary>
        /// skapar en lista av person och för över de skapade personerna från Databasen in till listan.
        /// </summary>
        public List<Person> List(string filter = "", string orderBy = "Id", int max = 30)
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

        /// <summary>
        /// metod som tar emot ett id och hämtar person baserat på id. Ifall personen ej hittas returneras null.
        /// </summary>
        public Person Read(int id)
        {
            var row = SqlDatabase.GetDataTable("SELECT TOP 1 * from Relatives Where Id = @id", ("@id", id.ToString()));
            if (row.Rows.Count == 0)
                return null;

            return GetPersonObject(row.Rows[0]);
        }

        /// <summary>
        /// metod som tar emot ett namn och hämtar person baserat på namn. Funkar att skriva förnamn och för-efternamn
        /// </summary>
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

        /// <summary>
        /// metod som söker på det som användaren skriver in
        /// </summary>
        public List<Person> Search(string filter, string searchInput)
        {
            DataTable dt = new DataTable();
            var searchResult = new List<Person>();
            var sql = $"SELECT * FROM Relatives WHERE {filter} LIKE @searchInput";
            dt = SqlDatabase.GetDataTable(sql, ("@searchInput", $"%{searchInput}%"));
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    searchResult.Add(GetPersonObject(row));
                }
            }
            return searchResult;
        }

        /// <summary>
        /// Metod som skriver in det användaren skriver in, ser till att perosnen som söker inte kommer med i sökningen.
        /// </summary>

        public List<Person> Search(string filter, string searchInput, Person person)
        {
            DataTable dt = new DataTable();
            var searchResult = new List<Person>();
            var sql = $"SELECT * FROM Relatives WHERE Id != @Id AND {filter} LIKE @searchInput";
            dt = SqlDatabase.GetDataTable(sql, ("@Id", person.Id.ToString()), ("@searchInput", $"%{searchInput}%"));
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    searchResult.Add(GetPersonObject(row));
                }
            }
            return searchResult;
        }

        public void SetParents(Person child, Person mother, Person father)
        {
            child.MotherId = mother.Id;
            child.FatherId = father.Id;

            Update(child);
        }

        /// <summary>
        /// Uppdaterar en person med alla dess properties
        /// </summary>
        /// <param name="person"></param>
        public void Update(Person person)
        {
            SqlDatabase.ExecuteSQL(@"
                Update dbo.Relatives SET
                FirstName = @FirstName, LastName = @LastName, BirthDate = @BirthDate, DeathDate = @DeathDate, BirthCity = @BirthCity, DeathCity = @DeathCity, MotherId = @MotherId, FatherId = @FatherId
                WHERE Id = @Id",
           ("@FirstName", person.FirstName),
           ("@LastName", person.LastName),
           ("@BirthDate", person.BirthDate),
           ("@DeathDate", person.DeathDate),
           ("@BirthCity", person.BirthCity),
           ("@DeathCity", person.DeathCity),
           ("@MotherId", person.MotherId.ToString()),
           ("@FatherId", person.FatherId.ToString()),
           ("@Id", person.Id.ToString())
           );
        }

        public void CreateDatabase(string databaseName)
        {
            var dt = new SQLDatabase();
            dt.CreateDatabase(databaseName);
        }

        public void CreateTable(string tableName, string fields)
        {
            SqlDatabase.CreateTable(tableName, fields);
        }

        /// <summary>
        /// metod som för över informationen från en datarow till ett person objekt.
        /// </summary>
        private static Person GetPersonObject(DataRow row)
        {
            return new Person
            {
                FirstName = row["firstName"].ToString(),
                LastName = row["lastName"].ToString(),
                BirthDate = row["birthDate"].ToString(),
                DeathDate = row["deathDate"].ToString(),
                BirthCity = row["birthCity"].ToString(),
                DeathCity = row["deathCity"].ToString(),
                MotherId = (int)row["motherId"],
                FatherId = (int)row["fatherId"],
                Id = (int)row["Id"]
            };
        }

        /// <summary>
        /// en metod som fyller på alla parametrar i en tuple och returnerar denne.
        /// </summary>
        private (string, string)[] FillParameters(Person person)
        {
            return new (string, string)[]
            {
            ("@FirstName", person.FirstName),
            ("@LastName", person.LastName),
            ("@BirthDate", person.BirthDate),
            ("@DeathDate", person.DeathDate),
            ("@BirthCity", person.BirthCity),
            ("@DeathCity", person.DeathCity),
            ("@MotherId", person.MotherId.ToString()),
            ("@FatherId", person.FatherId.ToString())
            };
        }
    }
}