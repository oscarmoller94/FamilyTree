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

        static SQLDatabase db = new SQLDatabase();
        static DataTable dt = new DataTable();
        static CRUD crud = new CRUD();
        static void Main(string[] args)
        {
            db.CreateDatabase("Genealogy");
            db.CreateTable("Relatives",
                "Id int NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                "firstName varchar(50)," +
                "lastName varchar(50)," +
                "birthDate varchar(50)," +
                "deathDate varchar(50)," +
                "birthCity varchar(50)," +
                "deathCity varchar(50)," +
                "motherId int," +
                "fatherId int");

            InitializeRelatives();
            MainMenu();
        }
        /// <summary>
        /// metod med huvudmeny som låter användaren välja vad hen vill göra. Tar emot ett val.
        /// </summary>
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
                Console.WriteLine("[4]. Create new person");
                Console.WriteLine("\n[5]. Exit program");
                int.TryParse(Console.ReadLine(), out choice);
                Console.Clear();
                if (choice > 5)
                {
                    PrintStringRed("Chosen number to high! try again");
                }
                else if (choice <= 0)
                {
                    PrintStringRed("Chosen number to low! try again");

                }

            } while (choice > 5 || choice <= 0);

            MainMenuChoice(choice);


        }
        /// <summary>
        /// metod som tar emot ett val gjort i main menu, och agerar utefter det valet.
        /// </summary>
        /// <param name="choice"></param>
        private static void MainMenuChoice(int choice)
        {
            switch (choice)
            {
                case 1:
                    PrintFullFamilyTree();
                    break;
                case 2:
                    var listOfRelatives = crud.List();
                    SelectSpecificPerson(listOfRelatives);
                    break;
                case 3:
                    SearchMenu();
                    break;
                case 4:
                    CreateNewPerson();
                    break;
                case 5:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// En metod som skapar en ny person baserat på användarens input.
        /// </summary>
        private static void CreateNewPerson()
        {
            Console.WriteLine("Press enter if you dont have an answer");
            Console.Write("Enter First name: ");
            string firstName = Console.ReadLine();
            Console.Write("Enter Last name: ");
            string lastName = Console.ReadLine();
            Console.Write("Enter birth date(dd/mm-yyyy)");
            string birthDate = Console.ReadLine();
            Console.Write("Enter death date(dd/mm-yyyy)");
            string deathDate = Console.ReadLine();
            Console.Write("Enter birth city: ");
            string birthCity = Console.ReadLine();
            Console.Write("Enter death city: ");
            string deathCity = Console.ReadLine();
            Console.Write("Enter mother id: ");
            int.TryParse(Console.ReadLine(), out int motherId);
            Person mother = crud.Read(motherId);
            if (mother == null)
            {
                PrintStringRed("person with that Id doesnt exsists in the database (Mother id will be set to 0)");
                motherId = 0;
            }
            Console.Write("Enter father id: ");
            int.TryParse(Console.ReadLine(), out int fatherId);
            Person father = crud.Read(fatherId);
            if (father == null)
            {
                PrintStringRed("person with that Id doesnt exsists in the database (Father id will be set to 0)");
                fatherId = 0;
            }
            Console.Clear();
            var person = new List<Person>();
            person.Add(new Person { FirstName = firstName, LastName = lastName, BirthDate = birthDate, DeathDate = deathDate, BirthCity = birthCity, DeathCity = deathCity, MotherId = motherId, FatherId = fatherId});
            foreach (var personObject in person)
            {
                crud.Create(personObject);
                crud.GiveIdToPersonObject(personObject);
                Console.WriteLine($"{personObject.FirstName} {personObject.LastName} Created succesfully!");
            }
            
        
            
            


        }

        /// <summary>
        /// metod som skriver ut vad användaren kan välja att söka på. Tar även emot ett svar.
        /// </summary>
        private static void SearchMenu()
        {
            int choice;
            do
            {
                Console.WriteLine("Select a category to search:");
                Console.WriteLine("[1]. First name");
                Console.WriteLine("[2]. Last name");
                Console.WriteLine("[3]. Birth year");
                Console.WriteLine("[4]. Death year");
                Console.WriteLine("[5]. Birth city");
                Console.WriteLine("[6]. Death city");
                Console.WriteLine("\n[7]. Main Menu");
                int.TryParse(Console.ReadLine(), out choice);
                Console.Clear();
                if (choice > 7)
                {
                    PrintStringRed("Chosen number to high! try again");
                }
                else if (choice < 1)
                {
                    PrintStringRed("Chosen number to low! try again");
                }
            } while (choice < 1 || choice > 7);
            SearchMenuChoice(choice);
        }
        /// <summary>
        /// metod som tar emot ett val, och baserat på det valet skickar olika parametrar till exequteSearch metoden.
        /// </summary>

        private static void SearchMenuChoice(int choice)
        {
            switch (choice)
            {
                case 1:
                    ExequteSearch("FirstName");
                    break;
                case 2:
                    ExequteSearch("LastName");
                    break;
                case 3:
                    ExequteSearch("BirthDate");
                    break;
                case 4:
                    ExequteSearch("DeathYear");
                    break;
                case 5:
                    ExequteSearch("BirthCity");
                    break;
                case 6:
                    ExequteSearch("DeathCity");
                    break;
                case 7:
                    MainMenu();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// metod som låter användaren skriva in sökord, sparar resultatet i en lista.
        /// </summary>
        private static void ExequteSearch(string filter)
        {

            Console.Write("Enter search: ");
            string userSearch = Console.ReadLine();
            Console.Clear();
            var searchResult = crud.Search(filter, userSearch);
            if (searchResult.Count == 1)
            {

                foreach (var person in searchResult)
                {
                    Console.WriteLine($"Your search result: {person}\n");
                    SpecificPersonMenu(person);
                }
            }
            else if (searchResult.Count > 1)
            {
                Console.WriteLine("Search results:");
                SelectSpecificPerson(searchResult);
            }
            else
            {
                PrintStringRed("No matches found");
                SearchMenu();
            }
        }
        /// <summary>
        /// metod som skriver ut en lista med personer och låter användaren välja vilken person 
        /// hen vill få reda på mer om.
        /// </summary>
        private static void SelectSpecificPerson(List<Person> listOfRelatives)
        {
            int choice;
            int counter = 1;
            do
            {
                foreach (var person in listOfRelatives)
                {
                    Console.WriteLine($"[{counter}] {person.FirstName} {person.LastName}");
                    counter++;
                }
                Console.WriteLine($"\n[{counter}] Return to Main menu");
                Console.Write("\nSelect the number next to the person you want to find out more about: ");
                int.TryParse(Console.ReadLine(), out choice);
                Console.Clear();
                if (choice == listOfRelatives.Count + 1)
                {
                    MainMenu();
                }
                if (choice < 1)
                {
                    PrintStringRed("Chosen number to low! try again");
                }
                else if (choice > listOfRelatives.Count + 1)
                {
                    PrintStringRed("Chosen number to high! try again");

                }

            } while (choice < 1 || choice > listOfRelatives.Count + 1);
            SpecificPersonMenu(listOfRelatives[choice - 1]);


        }
        /// <summary>
        /// metod skriver ut en meny om vad man vill få reda om en person. 
        /// Tar in val från användaren.
        /// </summary>
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
                Console.WriteLine($"[9] Find persons with the same birth city as {person.FirstName}");
                Console.WriteLine($"[10] Find persons with the same death city as {person.FirstName}");
                Console.WriteLine($"\n[11] Return");
                Console.WriteLine($"[12] Return to main menu");
                int.TryParse(Console.ReadLine(), out choice);
                Console.Clear();

                if (choice < 1)
                {
                    PrintStringRed("Chosen number to low! try again");
                }
                else if (choice > 12)
                {
                    PrintStringRed("Chosen number to high! try again");
                }
            } while (choice < 1 || choice > 12);
            SpecificPersonMenuChoice(choice, person);

        }

        /// <summary>
        /// tar emot ett val och agerar utifrån valet
        /// </summary>
        private static void SpecificPersonMenuChoice(int choice, Person person)
        {
            switch (choice)
            {
                case 1:
                    CalculateAge(person);
                    break;
                case 2:
                    Console.WriteLine(person);
                    SpecificPersonMenu(person);
                    break;
                case 3:
                    UpdatePersonMenu(person);
                    break;
                case 4:
                    crud.Delete(person);
                    Console.WriteLine("Person deleted successfully");
                    var listOfRelatives = crud.List();
                    SelectSpecificPerson(listOfRelatives);
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
                    FindSimiliarCity(person, "BirthCity");
                    break;
                case 10:
                    FindSimiliarCity(person, "DeathCity");
                    break;
                case 11:
                    listOfRelatives = crud.List();
                    SelectSpecificPerson(listOfRelatives);
                    break;
                case 12:
                    MainMenu();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// metod som tar reda på personer som har gemensamma städer.
        /// </summary>
        private static void FindSimiliarCity(Person person, string filter)
        {
            string city = filter == "BirthCity" ? city = person.BirthCity : city = person.DeathCity;
            var personsBirthCity = crud.Search(filter, city);
            if (personsBirthCity.Count > 0)
            {
                Console.WriteLine($"persons with the same city as {person.FirstName}");
                foreach (var item in personsBirthCity)
                {
                    Console.WriteLine(item);
                }
                SpecificPersonMenu(person);
            }
            else
            {
                PrintStringRed("Could not find any matches!");
                SpecificPersonMenu(person);
            }
        }

        /// <summary>
        /// skriver ut en persons syskon ifall personen har några
        /// </summary>
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
                PrintStringRed($"Could not find any siblings to {person.FirstName}\n");
                SpecificPersonMenu(person);
            }
        }
        /// <summary>
        /// en metod som skriver ut en persons farföräldrar om de finns i databasen.
        /// </summary>
        private static void FindGrandParents(Person person)
        {
            crud.GetGrandParents(person, out Person grandMother, out Person grandFather);
            string output = grandMother != null ? $"{person.FirstName}'s grandmother:\n{grandMother}" : $"Could not find {person.FirstName}'s grandmother in the database";
            Console.WriteLine(output);
            output = grandFather != null ? $"{person.FirstName}'s grandFather:\n{grandFather}" : $"Could not find {person.FirstName}'s grandfather in the database";
            Console.WriteLine(output);
            SpecificPersonMenu(person);

        }
        /// <summary>
        /// metod som skriver ut en persons barn ifall personen har några barn.
        /// </summary>
        private static void FindChildren(Person person)
        {
            var listOfChildren = crud.GetChildren(person);

            if (listOfChildren.Count > 0)
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
                PrintStringRed($"Could not find any children that belongs to {person.FirstName} in the database\n");
                SpecificPersonMenu(person);
            }

        }
        /// <summary>
        /// metod som skriver ut personens föräldrar ifall personen har några föräldar
        /// </summary>

        private static void FindParents(Person person)
        {
            crud.GetParents(person, out Person mother, out Person father);
            string output = father != null ? $"{person.FirstName}´s father:\n{father}" : $"Could not find {person.FirstName}'s father in the database";
            Console.WriteLine(output);
            output = mother != null ? $"{person.FirstName}'s mother:\n{mother}" : $"Could not find {person.FirstName}'s mother in the database";
            Console.WriteLine(output);
            SpecificPersonMenu(person);

        }
        /// <summary>
        /// Metod som skriver ut en meny på vad man kan uppdatera hos en person och tar emot svar.
        /// </summary>
        private static void UpdatePersonMenu(Person person)
        {
            int choice;
            do
            {
                Console.WriteLine($"What do you want to update about {person.FirstName}?");
                Console.WriteLine("[1] Update first name");
                Console.WriteLine("[2] Update last name");
                Console.WriteLine("[3] Update mother");
                Console.WriteLine("[4] Update father");
                Console.WriteLine("[5] Update birth city");
                Console.WriteLine("[6] Update death city");
                Console.WriteLine("\n[7] Return");
                int.TryParse(Console.ReadLine(), out choice);
                Console.Clear();
                if (choice > 7)
                {
                    PrintStringRed("Chosen number to high! try again.");
                }
                else if (choice < 1)
                {
                    PrintStringRed("Chosen number to low! try again.");
                }

            } while (choice < 1 || choice > 7);
            UpdatePersonMenuChoice(choice, person);



        }
        /// <summary>
        /// metod som uppdaterar en person baserat på det inskickade valet
        /// </summary>

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
                    UpdateParent(child: person, parent: newMother);
                    UpdatePersonMenu(person);
                    break;
                case 4:
                    Console.Write("Enter new father: ");
                    string fatherName = Console.ReadLine();
                    Person newFather = crud.Read(fatherName);
                    UpdateParent(child: person, parent: newFather);
                    UpdatePersonMenu(person);
                    break;

                case 5:
                    Console.Write("Enter new birth city: ");
                    string birthCity = Console.ReadLine();
                    Console.Clear();
                    UpdateBirthCity(person, birthCity);
                    break;
                case 6:
                    Console.Write("Enter new death city: ");
                    string deathCity = Console.ReadLine();
                    Console.Clear();
                    UpdateDeathCity(person, deathCity);
                    break;
                case 7:
                    SpecificPersonMenu(person);
                    break;

                default:
                    break;
            }
        }
        /// <summary>
        /// uppdaterar födelsestad hos en person
        /// </summary>

        private static void UpdateBirthCity(Person person, string birthCity)
        {
            if (person != null)
            {
                person.BirthCity = birthCity;
                crud.Update(person);
                Console.WriteLine("Person updated!");
                UpdatePersonMenu(person);
            }
            else
            {
                PrintStringRed("Something went wrong!");
                UpdatePersonMenu(person);
            }
        }
        /// <summary>
        /// Uppdaterar dödsstad till hos en person.
        /// </summary>

        private static void UpdateDeathCity(Person person, string birthCity)
        {
            if (person != null)
            {
                person.BirthCity = birthCity;
                crud.Update(person);
                Console.WriteLine("Person updated");
                UpdatePersonMenu(person);
            }
            else
            {
                PrintStringRed("Something went wrong!");
                UpdatePersonMenu(person);
            }
        }
        /// <summary>
        /// uppdaterar föräldrar till en person
        /// </summary>

        private static void UpdateParent(Person child, Person parent)
        {
            if (parent != null)
            {
                child.FatherId = parent.Id;
                crud.Update(child);
                Console.WriteLine($"{child.FirstName} {child.LastName} father changed to {parent.FirstName} {parent.LastName}");
            }
            else
            {
                PrintStringRed("Cannot find the name you typed in the database!");
            }
        }
        /// <summary>
        /// Metod som räknar ut åldern på en person genom att manipulera födelsedatum strängen.
        /// </summary>
        private static void CalculateAge(Person person)
        {
            var dateArray = person.BirthDate.Split("-");
            var dateOfBirth = dateArray[0];
            var strYearOfBirth = person.BirthDate.Substring((person.BirthDate.Length - 4), 4);

            int.TryParse(strYearOfBirth, out int intYearOfBirth);
            int age = DateTime.Now.Year - intYearOfBirth;

            DateTime.TryParse(dateOfBirth, out DateTime birthDay);

            if (birthDay > DateTime.Now.Date)
            {
                age--;
            }
            string output = person.DeathDate == "Still alive" ? $"{person.FirstName} is {age} years old." : $"{person.FirstName} would have been {age} years old";
            Console.Clear();
            Console.WriteLine($"{output}\n");
            SpecificPersonMenu(person);
        }
        /// <summary>
        /// Skriver ut alla i databasen
        /// </summary>
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
        /// <summary>
        /// skriver ut en sträng i röd färg
        /// </summary>
        private static void PrintStringRed(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void InitializeRelatives()
        {
            Person oscarMöllerHultberg = new Person()
            {
                FirstName = "Oscar",
                LastName = "Möller-Hultberg",
                BirthDate = "4/10-1994",
                DeathDate = "Still alive",
                BirthCity = "Stockholm",
                DeathCity = "Still alive"

            };
            Person mathildaMöllerHultberg = new Person()
            {
                FirstName = "Mathilda",
                LastName = "Möller-Hultberg",
                BirthDate = "23/7-1996",
                DeathDate = "Still alive",
                BirthCity = "Stockholm",
                DeathCity = "Still alive"
            };
            Person lenaHultberg = new Person()
            {
                FirstName = "Lena",
                LastName = "Hultberg",
                BirthDate = "3/4-1971",
                DeathDate = "Still alive",
                BirthCity = "Karlshamn",
                DeathCity = "Still alive"
            };
            Person johanHultberg = new Person()
            {
                FirstName = "Johan",
                LastName = "Hultberg",
                BirthDate = "8/11-1966",
                DeathDate = "Still alive",
                BirthCity = "Karlshamn",
                DeathCity = "Still alive"
            };
            Person danielMöller = new Person()
            {
                FirstName = "Daniel",
                LastName = "Möller",
                BirthDate = "19/3-1970",
                DeathDate = "Still alive",
                BirthCity = "Stockholm",
                DeathCity = "Still alive"
            };
            Person carinMöller = new Person()
            {
                FirstName = "Carin",
                LastName = "Möller",
                BirthDate = "30/5-1975",
                DeathDate = "Still alive",
                BirthCity = "Stockholm",
                DeathCity = "Still alive"
            };
            Person willisMöller = new Person()
            {
                FirstName = "Willis",
                LastName = "Möller",
                BirthDate = "19/7-1943",
                DeathDate = "Still alive",
                BirthCity = "Nässjö",
                DeathCity = "Still alive"
            };
            Person gullBrittMöller = new Person()
            {
                FirstName = "Gull-Britt",
                LastName = "Möller",
                BirthDate = "19/4-1946",
                DeathDate = "Still alive",
                BirthCity = "Nässjö",
                DeathCity = "Still alive"
            };
            Person ingridMöller = new Person()
            {
                FirstName = "Ingrid",
                LastName = "Möller",
                BirthDate = "16/9-1945",
                DeathDate = "Still alive",
                BirthCity = "Vaxholm",
                DeathCity = "Still alive"
            };
            Person birgittaJönsson = new Person()
            {
                FirstName = "Birgitta",
                LastName = "Jönsson",
                BirthDate = "2/7-1941",
                DeathDate = "Still alive",
                BirthCity = "Karlshamn",
                DeathCity = "Still alive"
            };
            Person janHultberg = new Person()
            {
                FirstName = "Jan",
                LastName = "Hultberg",
                BirthDate = "2/4-1939",
                DeathDate = "3/9-2013",
                BirthCity = "Mörrum",
                DeathCity = "Mörrum"
            };
            Person gulliPettersson = new Person()
            {
                FirstName = "Gulli",
                LastName = "Pettersson",
                BirthDate = "20/11-1918",
                DeathDate = "9/2-2005",
                BirthCity = "Karlshamn",
                DeathCity = "Karlshamn"
            };
            Person brorPettersson = new Person()
            {
                FirstName = "Bror",
                LastName = "Pettersson",
                BirthDate = "9/1-1918",
                DeathDate = "21/8-2001",
                BirthCity = "Karlshamn",
                DeathCity = "Karlshamn"
            };
            Person lenaArgulander = new Person()
            {
                FirstName = "Lena",
                LastName = "Argulander",
                BirthDate = "30/12-1921",
                DeathDate = "21/2-2016",
                BirthCity = "Vaxholm",
                DeathCity = "Vaxholm"
            };
            Person sixtenArgulander = new Person()
            {
                FirstName = "Sixten",
                LastName = "Argulander",
                BirthDate = "22/1-1919",
                DeathDate = "15/5-2007",
                BirthCity = "Vaxholm",
                DeathCity = "Vaxholm"
            };
            Person davidMöller = new Person()
            {
                FirstName = "David",
                LastName = "Möller",
                BirthDate = "17/9-1914",
                DeathDate = "27/12-1969",
                BirthCity = "Nässjö",
                DeathCity = "Nässjö"
            };
            Person annaLisaMöller = new Person()
            {
                FirstName = "Anna-Lisa",
                LastName = "Möller",
                BirthDate = "2/10-1919",
                DeathDate = "7/3-2003",
                BirthCity = "Mjölby",
                DeathCity = "Nässjö"
            };
            Person josefMöller = new Person()
            {
                FirstName = "Josef",
                LastName = "Möller",
                BirthDate = "6/4-1886",
                DeathDate = "16/6-1956",
                BirthCity = "Nöbbele",
                DeathCity = "Nöbbele"
            };
            Person amaliaMöller = new Person()
            {
                FirstName = "Amalia",
                LastName = "Möller",
                BirthDate = "19/12-1880",
                DeathDate = "12/2-1969",
                BirthCity = "Herråkra",
                DeathCity = "Nöbbele"
            };
            Person karlForsberg = new Person()
            {
                FirstName = "Karl",
                LastName = "Forsberg",
                BirthDate = "23/6-1868",
                DeathDate = "15/1-1955",
                BirthCity = "Västfärnebo",
                DeathCity = "Västfärnebo"
            };
            Person olgaForsberg = new Person()
            {
                FirstName = "Olga",
                LastName = "Forsberg",
                BirthDate = "0/0-1896",
                DeathDate = "0/0-1929",
                BirthCity = "Kila",
                DeathCity = "Västfärnebo"
            };

            var listOfPersonObjects = new List<Person>()
            {
                oscarMöllerHultberg,
                mathildaMöllerHultberg,
                lenaHultberg,
                johanHultberg,
                danielMöller,
                carinMöller,
                willisMöller,
                gullBrittMöller,
                ingridMöller,
                birgittaJönsson,
                janHultberg,
                gulliPettersson,
                brorPettersson,
                lenaArgulander,
                sixtenArgulander,
                davidMöller,
                annaLisaMöller,
                josefMöller,
                amaliaMöller,
                karlForsberg,
                olgaForsberg
            };

            foreach (var person in listOfPersonObjects)
            {
                crud.Create(person);
                crud.GiveIdToPersonObject(person);
            }
            crud.SetParents(child: oscarMöllerHultberg, mother: lenaHultberg, father: danielMöller);
            crud.SetParents(child: mathildaMöllerHultberg, mother: lenaHultberg, father: danielMöller);
            crud.SetParents(child: lenaHultberg, mother: birgittaJönsson, father: janHultberg);
            crud.SetParents(child: johanHultberg, mother: birgittaJönsson, father: janHultberg);
            crud.SetParents(child: danielMöller, mother: ingridMöller, father: willisMöller);
            crud.SetParents(child: carinMöller, mother: ingridMöller, father: willisMöller);
            crud.SetParents(child: ingridMöller, mother: lenaArgulander, father: sixtenArgulander);
            crud.SetParents(child: willisMöller, mother: annaLisaMöller, father: davidMöller);
            crud.SetParents(child: gullBrittMöller, mother: annaLisaMöller, father: davidMöller);
            crud.SetParents(child: birgittaJönsson, mother: gulliPettersson, father: brorPettersson);
            crud.SetParents(child: davidMöller, mother: amaliaMöller, father: josefMöller);
            crud.SetParents(child: lenaArgulander, mother: olgaForsberg, father: karlForsberg);


        }
    }
}
