using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace FamilyTree
{
    class Program
    {
        static CRUD crud = new CRUD();
        static SQLDatabase db = new SQLDatabase();
        static DataTable dt = new DataTable();
        static void Main(string[] args)
        {


            db.CreateDatabase("Genealogy");
            db.CreateTable("Relatives",
                "Id int NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                "firstName varchar(50)," +
                "lastName varchar(50)," +
                "birthDate varchar(50)," +
                "deathDate varchar(50)," +
                "motherId int," +
                "fatherId int");

            InitializeRelatives();
            MainMenu();

            foreach (var person in listOfRelatives)
            {
                Console.WriteLine(person.ToString());
            }
            

            //Person mother = new Person();
            //Person father = new Person();
            //foreach (var person in listOfRelatives)
            //{
            //    if (person.MotherId != 0 && person.FatherId != 0)
            //    {
            //        Console.WriteLine($"Parents of {person.FirstName} {person.LastName}\n");
            //        crud.GetParents(person, out mother, out father);
            //        Print(mother);
            //        Print(father);
            //    }

            //}

        }

        private static void MainMenu()
        {
            int choice;
            do
            {
                Console.WriteLine("Welcome to my family tree!\n");
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1. Print full family tree");
                Console.WriteLine("2. Select specific person");
                Console.WriteLine("3. Find person by search");
                Console.WriteLine("4. Exit program");
                int.TryParse(Console.ReadLine(), out choice);
                if (choice > 4)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Chosen number to high! try again");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (choice <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Chosen number to low! try again.");
                    Console.ForegroundColor = ConsoleColor.White;
                }

            } while (choice > 4 || choice <= 0);

            MainMenuChoice(choice);
            
            
        }

        private static void MainMenuChoice(int choice)
        {
            switch (choice)
            {
                case 1:
                    PrintFullFamilyTree();
                    break;
                case 2:
                    SelectSpecificPerson();
                    break;
                default:
                    break;
            }
        }

        private static void SelectSpecificPerson()
        {
            var listOfRelatives = crud.List();
            Console.Clear();
            Console.WriteLine("Select which person you want to find out more about");
            
            int counter = 1;
            foreach (var person in listOfRelatives)
            {
                Console.WriteLine($"[{counter}] {person.FirstName} {person.LastName}");
                counter++;
            }
        }

        private static void PrintFullFamilyTree()
        {
            var listOfRelatives = crud.List();
            foreach (var person in listOfRelatives)
            {
                Console.WriteLine(person.ToString());
            }
        }

        private static void InitializeRelatives()
        {
            Person oscar = new Person()
            {
                FirstName = "Oscar",
                LastName = "Möller",
                BirthDate = "4/10-1994",
                DeathDate = "Still alive",
                MotherId = 0,
                FatherId = 0
            };
            Person mathilda = new Person()
            {
                FirstName = "Mathilda",
                LastName = "Hultberg",
                BirthDate = "23/7-1996",
                DeathDate = "Still alive",
                MotherId = 0,
                FatherId = 0
            };
            Person lena = new Person()
            {
                FirstName = "Lena",
                LastName = "Hultberg",
                BirthDate = "3/4-1971",
                DeathDate = "Still alive",
                MotherId = 0,
                FatherId = 0
            };
            Person daniel = new Person()
            {
                FirstName = "Daniel",
                LastName = "Möller",
                BirthDate = "19/3-1970",
                DeathDate = "Still alive",
                MotherId = 0,
                FatherId = 0,
            };
            Person willis = new Person()
            {
                FirstName = "Willis",
                LastName = "Möller",
                BirthDate = "19/7-1943",
                DeathDate = "Still alive",
                MotherId = 0,
                FatherId = 0
            };
            Person ingrid = new Person()
            {
                FirstName = "Ingrid",
                LastName = "Möller",
                BirthDate = "16/9-1945",
                DeathDate = "Still alive",
                MotherId = 0,
                FatherId = 0
            };
            Person birgitta = new Person()
            {
                FirstName = "Birgitta",
                LastName = "Jönsson",
                BirthDate = "16/9-1945",
                DeathDate = "Still alive",
                MotherId = 0,
                FatherId = 0
            };
            Person jan = new Person()
            {
                FirstName = "Jan",
                LastName = "Hultberg",
                BirthDate = "16/9-1945",
                DeathDate = "Still alive",
                MotherId = 0,
                FatherId = 0
            };
            Person gulli = new Person()
            {
                FirstName = "Gulli",
                LastName = "Petterson",
                BirthDate = "20/11-1918",
                DeathDate = "9/2-2005",
                MotherId = 0,
                FatherId = 0
            };
            Person bror = new Person()
            {
                FirstName = "Bror",
                LastName = "Pettersson",
                BirthDate = "9/1-1918",
                DeathDate = "21/8-2001",
                MotherId = 0,
                FatherId = 0
            };
            Person lenaA = new Person()
            {
                FirstName = "Lena",
                LastName = "Argulander",
                BirthDate = "30/12-1921",
                DeathDate = "21/2-2016",
                MotherId = 0,
                FatherId = 0
            };
            Person sixten = new Person()
            {
                FirstName = "Sixten",
                LastName = "Argulander",
                BirthDate = "22/1-1919",
                DeathDate = "15/5-2007",
                MotherId = 0,
                FatherId = 0
            };
            Person david = new Person()
            {
                FirstName = "David",
                LastName = "Möller",
                BirthDate = "17/9-1914",
                DeathDate = "27/12-1969",
                MotherId = 0,
                FatherId = 0
            };
            Person annaLisa = new Person()
            {
                FirstName = "Anna-Lisa",
                LastName = "Möller",
                BirthDate = "2/10-1919",
                DeathDate = "7/3-2003",
                MotherId = 0,
                FatherId = 0
            };

            crud.Create(oscar);
            crud.Create(mathilda);
            crud.Create(lena);
            crud.Create(daniel);
            crud.Create(ingrid);
            crud.Create(willis);
            crud.Create(birgitta);
            crud.Create(jan);
            crud.Create(gulli);
            crud.Create(bror);
            crud.Create(lenaA);
            crud.Create(sixten);
            crud.Create(annaLisa);
            crud.Create(david);

            crud.SetParents(child: oscar, mother: lena, father: daniel);
            crud.SetParents(child: mathilda, mother: lena, father: daniel);
            crud.SetParents(child: lena, mother: birgitta, father: jan);
            crud.SetParents(child: daniel, mother: ingrid, father: willis);
            crud.SetParents(child: ingrid, mother: lenaA, father: sixten);
            crud.SetParents(child: willis, mother: annaLisa, father: david);
            crud.SetParents(child: birgitta, mother: gulli, father: bror);
        }
    }
}
