namespace FamilyTree
{
    internal class Person
    {
        public string BirthCity { get; set; }
        public string BirthDate { get; set; }
        public string DeathCity { get; set; }
        public string DeathDate { get; set; }
        public int FatherId { get; set; }
        public string FirstName { get; set; }
        public int Id { get; set; }
        public string LastName { get; set; }
        public int MotherId { get; set; }
        public override string ToString()
        {
            return ($"|Id:{Id}| |{FirstName} {LastName}| |{BirthDate} - {DeathDate}| |Born: {BirthCity}| |Died: {DeathCity}| |M.id: {MotherId}| |F.id : {FatherId}|\n");
        }
    }
}