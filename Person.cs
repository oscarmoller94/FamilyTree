using System;
using System.Collections.Generic;
using System.Text;

namespace FamilyTree
{
    class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string DeathDate { get; set; }
        public string BirthCity { get; set; }
        public string DeathCity { get; set; }
        public int MotherId { get; set; }
        public int FatherId { get; set; }

        public override string ToString()
        {
            return($"|Id:{Id}| |{FirstName} {LastName}| |{BirthDate} - {DeathDate}| |Birth city: {BirthCity}| |Death city: {DeathCity}| |Mother id: {MotherId}| |Father id : {FatherId}|\n");
        }
    }
}
