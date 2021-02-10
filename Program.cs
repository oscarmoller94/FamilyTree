using System;
using System.Data;

namespace FamilyTree
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new SQLDatabase();
            db.DatabaseName = "Genealogy";
            //db.CreateDatabase("Genealogy");
            //db.CreateTable("Relatives",
            //    "Id int NOT NULL PRIMARY KEY," +
            //    "firstName varchar(50)," +
            //    "lastName varchar(50)," +
            //    "birthDate varchar(50)," +
            //    "deathDate varchar(50)," +
            //    "motherId int," +
            //    "fatherId int");

            

        }
    }
}
