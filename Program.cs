using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FamilyTree
{
    class Program
    {
        //static CRUD crud = new CRUD();
        static SQLDatabase db = new SQLDatabase();
        static DataTable dt = new DataTable();
        static CRUD crud = new CRUD();
        static void Main(string[] args)
        {
            // kanske returnera en lista av objeketen efter dom har initialiserats och sedan skicka med listan till mainMenu?
            // någonting knasar med Idet, dom får det vid körning av Create, men av någon anledningen försvinner dessa. 


            db.CreateDatabase("Genealogy");
            db.CreateTable("Relatives",
                "Id int NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                "firstName varchar(50)," +
                "lastName varchar(50)," +
                "birthDate varchar(50)," +
                "deathDate varchar(50)," +
                "motherId int," +
                "fatherId int");

            //InitializeRelatives();
            MainMenu();
        }

        private static void MainMenu()
        {

            int choice;
            Console.WriteLine("Welcome to my family tree!\n");
            do
            {
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("[1]. Print full family tree");
                Console.WriteLine("[2]. Select specific person");
                Console.WriteLine("[3]. Find person by search");
                Console.WriteLine("[4]. Exit program");
                int.TryParse(Console.ReadLine(), out choice);
                if (choice > 4)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Chosen number to high! try again");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (choice <= 0)
                {
                    Console.Clear();
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
                case 3:
                    SearchMenu();
                    break;
                default:
                    break;
            }
        }

        private static void SearchMenu()
        {
            Console.Clear();
            Console.WriteLine("Select a category to search:");
            Console.WriteLine("[1]. First name");
            Console.WriteLine("[2]. Last name");
            Console.WriteLine("[3]. Birth date");
            Console.WriteLine("[4]. Death date");
            Console.WriteLine("[5]. Mother Id");
            Console.WriteLine("[6]. Father Id");
        }

        private static void SelectSpecificPerson()
        {
            var listOfRelatives = crud.List();
            int choice;
            do
            {
                Console.Clear();
                Console.WriteLine("Select which person you want to find out more about");
                int counter = 1;
                foreach (var person in listOfRelatives)
                {
                    Console.WriteLine($"[{counter}] {person.FirstName} {person.LastName}");
                    counter++;
                }
                int.TryParse(Console.ReadLine(), out choice);

                if (choice < 1)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Chosen number to low! try again");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (choice > listOfRelatives.Count)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Chosen number to high! try again.");
                    Console.ForegroundColor = ConsoleColor.White;
                }

            } while (choice < 1 || choice > listOfRelatives.Count);
            Console.Clear();
            SpecificPersonMenu(listOfRelatives[choice - 1]);


        }
        private static void SpecificPersonMenu(Person person)
        {
            Console.WriteLine($"Selected person: {person.FirstName} {person.LastName}");
            Console.WriteLine("Now select an option below\n");
            int choice;
            do
            {
                string firstMenuOption = person.DeathDate.Equals("Still alive") ? $"[1].Calculate age of {person.FirstName}" : $"[1].Calculate how old {person.FirstName} would be today if still alive";
                Console.WriteLine(firstMenuOption);
                Console.WriteLine($"[2] Show full info about {person.FirstName}");
                Console.WriteLine($"[3] Update {person.FirstName}");
                Console.WriteLine($"[4] Delete {person.FirstName}");
                Console.WriteLine($"[5] Find {person.FirstName} parents");
                Console.WriteLine($"[6] Find {person.FirstName} childrens");
                Console.WriteLine($"[7] Find {person.FirstName} grandparents");
                Console.WriteLine($"[8] Find {person.FirstName} siblings");
                Console.WriteLine($"[9] Return to person menu");
                Console.WriteLine($"[10] Return to main menu");
                int.TryParse(Console.ReadLine(), out choice);

                if (choice < 1)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Chosen number to low! try again");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (choice > 9)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Chosen number to high! try again.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            } while (choice < 1 || choice > 9);
            SpecificPersonMenuChoice(choice, person);

        }

        private static void SpecificPersonMenuChoice(int choice, Person person)
        {
            switch (choice)
            {
                case 1:
                    CalculateAge(person);
                    break;
                case 2:
                    Console.Clear();
                    Console.WriteLine(person);
                    SpecificPersonMenu(person);
                    break;
                case 3:
                    UpdatePersonMenu(person);
                    break;
                case 4:
                    crud.Delete(person);
                    Console.WriteLine("Person deleted successfully");
                    SelectSpecificPerson();
                    break;
                case 5:
                    FindParents(person);
                    break;
                case 6:
                    FindChildren(person);
                    break;
                case 7:
                    FindGrandParents(person);
                    break;
                case 8:
                    FindSiblings(person);
                    break;
                case 9:
                    SelectSpecificPerson();
                    break;
                case 10:
                    MainMenu();
                    break;
                default:
                    break;
            }
        }

        private static void FindSiblings(Person person)
        {

            var listOfSiblings = crud.GetSiblings(person);
            if (listOfSiblings.Count > 0)
            {
                Console.WriteLine($"siblings to {person.FirstName}");
                foreach (var sibling in listOfSiblings)
                {
                    Console.WriteLine(sibling);
                }
                SpecificPersonMenu(person);
            }
            else
            {
                Console.WriteLine($"Could not find any siblings to {person.FirstName}");
                SpecificPersonMenu(person);
            }
        }

        private static void FindGrandParents(Person person)
        {
            crud.GetGrandParents(person, out Person grandMother, out Person grandFather);
            string output = grandMother != null ? $"{person.FirstName}'s grandmother: {grandMother}" : $"Could not find {person.FirstName}'s grandmother in the database";
            Console.WriteLine(output);
            output = grandFather != null ? $"{person.FirstName}'s grandFather: {grandFather}" : $"Could not find {person.FirstName}'s grandfather in the database";
            Console.WriteLine(output);
            SpecificPersonMenu(person);

        }

        private static void FindChildren(Person person)
        {
            var listOfChildren = crud.GetChildren(person);

            if(listOfChildren.Count > 0)
            {
                Console.WriteLine($"{person.FirstName}'s children: ");
                foreach (var children in listOfChildren)
                {
                    Console.WriteLine(children);
                }
                SpecificPersonMenu(person);
            }
            else
            {
                Console.WriteLine($"Could not find any children that belongs to {person.FirstName} in the database");
                SpecificPersonMenu(person);
            }
            
        }

        private static void FindParents(Person person)
        {
            crud.GetParents(person, out Person mother, out Person father);
            string output = father != null ? $"{person.FirstName}´s father: {father}" : $"Could not find {person.FirstName}'s father in the database";
            Console.WriteLine(output);
            output = mother != null ? $"{person.FirstName}'s mother: {mother}" : $"Could not find {person.FirstName}'s mother in the database";
            Console.WriteLine(output);
            SpecificPersonMenu(person);

        }

        private static void UpdatePersonMenu(Person person)
        {
            int choice;
            do
            {
                //Console.Clear();
                Console.WriteLine($"What do you want to update about {person.FirstName}?");
                Console.WriteLine("[1] First name");
                Console.WriteLine("[2] Last name");
                Console.WriteLine("[3] Change mother");
                Console.WriteLine("[4] Change father");
                Console.WriteLine("[5] Return to Person menu");
                int.TryParse(Console.ReadLine(), out choice);
                if (choice > 7)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Chosen number to high! try again.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (choice < 1)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Chosen number to low! try again.");
                    Console.ForegroundColor = ConsoleColor.White;
                }

            } while (choice < 1 || choice > 7);
            UpdatePersonMenuChoice(choice, person);



        }

        private static void UpdatePersonMenuChoice(int choice, Person person)
        {
            switch (choice)
            {
                case 1:
                    Console.Write("Enter new first name: ");
                    person.FirstName = Console.ReadLine();
                    crud.Update(person);
                    UpdatePersonMenu(person);
                    break;
                case 2:
                    Console.Write("Enter new last name: ");
                    person.LastName = Console.ReadLine();
                    crud.Update(person);
                    UpdatePersonMenu(person);
                    break;
                case 3:
                    Console.Write("Enter new mother: ");
                    string motherName = Console.ReadLine();
                    Person newMother = crud.Read(motherName);
                    if (newMother != null)
                    {
                        person.MotherId = newMother.Id;
                        crud.Update(person);
                        Console.WriteLine($"{person.FirstName} {person.LastName} mother changed to {newMother.FirstName} {newMother.LastName}");
                        UpdatePersonMenu(person);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Cant find the name you typed in the datbase!");
                        UpdatePersonMenu(person);
                        break;
                    }
                case 4:
                    Console.Write("Enter new father: ");
                    string fatherName = Console.ReadLine();
                    Person newFather = crud.Read(fatherName);
                    if (newFather != null)
                    {
                        person.FatherId = newFather.Id;
                        crud.Update(person);
                        Console.WriteLine($"{person.FirstName} {person.LastName} father changed to {newFather.FirstName} {newFather.LastName}");
                        UpdatePersonMenu(person);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Cant find the name you typed in the datbase!");
                        UpdatePersonMenu(person);
                        break;
                    }
                case 5:
                    SpecificPersonMenu(person);
                    break;

                default:
                    break;
            }
        }

        private static void CalculateAge(Person person)
        {

            var dateArray = person.BirthDate.Split("-");
            var dateOfBirth = dateArray[0];
            var strYearOfBirth = person.BirthDate.Substring((person.BirthDate.Length - 4), 4);

            int.TryParse(strYearOfBirth, out int intYearOfBirth);
            int age = DateTime.Now.Year - intYearOfBirth;

            DateTime.TryParse(dateOfBirth, out DateTime dayBorn);

            if (dayBorn > DateTime.Now.Date)
            {
                age--;
            }
            string output = person.DeathDate == "Still alive" ? $"{person.FirstName} is {age} years old." : $"{person.FirstName} would have been {age} years old if still alive";
            Console.Clear();
            Console.WriteLine($"{output}\n");
            SpecificPersonMenu(person);






        }

        private static void PrintFullFamilyTree()
        {
            Console.Clear();
            var listOfRelatives = crud.List();
            foreach (var person in listOfRelatives)
            {
                Console.WriteLine(person.ToString());
            }
            MainMenu();
        }

        private static void InitializeRelatives()
        {
            Person oscar = new Person()
            {
                FirstName = "Henok",
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

            var listOfPersonObjects = new List<Person>()
            {
                oscar,
                mathilda,
                lena,
                daniel,
                willis,
                ingrid,
                birgitta,
                jan,
                gulli,
                bror,
                lenaA,
                sixten,
                david,
                annaLisa
            };

            foreach (var person in listOfPersonObjects)
            {
                crud.Create(person);
                crud.GiveIdToPersonObject(person);
            }
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
