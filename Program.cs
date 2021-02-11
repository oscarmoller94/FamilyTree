using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

namespace FamilyTree
{
    class Program
    {
        static void Main(string[] args)
        {
            //db.CreateDatabase("Genealogy");
            //db.CreateTable("Relatives",
            //    "Id int NOT NULL IDENTITY(1,1) PRIMARY KEY," +
            //    "firstName varchar(50)," +
            //    "lastName varchar(50)," +
            //    "birthDate varchar(50)," +
            //    "deathDate varchar(50)," +
            //    "motherId int," +
            //    "fatherId int");
            var crud = new CRUD();
            var db = new SQLDatabase("Genealogy");
            var dt = new DataTable();


            Person oscar = new Person
            {
                FirstName = "Oscar",
                LastName = "Möller",
                BirthDate = "4/10-1994",
                DeathDate = "Still alive",
                MotherId = 0,
                FatherId = 0
            };
            Person lena = new Person
            {
                FirstName = "Lena",
                LastName = "Hultberg",
                BirthDate = "3/4-1971",
                DeathDate = "Still alive",
                MotherId = 0,
                FatherId = 0
            };
            Person daniel = new Person
            {
                FirstName = "Daniel",
                LastName = "Hultberg",
                BirthDate = "19/3-1970",
                DeathDate = "Still alive",
                MotherId = 0,
                FatherId = 0,
            };
            crud.Create(oscar);
     
      

            //List<Person> listOfPersons = new List<Person>();
            //listOfPersons = crud.List();

            //foreach (var person in listOfPersons)
            //{
            //    Print(person);
            //}

        }
        private static void Print(Person person)
        {
            Console.WriteLine($"first name: {person.FirstName}\n" +
                $"last name: {person.LastName}\n" +
                $"birthdate: {person.BirthDate}\n" +
                $"death date: {person.DeathDate}\n" +
                $"mother: \n" +
                $"father: \n" +
                $"Id: {person.Id}");

        }
    }
}
