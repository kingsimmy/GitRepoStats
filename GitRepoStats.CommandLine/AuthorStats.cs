
namespace GitRepoStats.CommandLine
{
    public class AuthorStats
    {
        public AuthorStats(string name, string email)
        {
            Name = name;
            Email = email;
        }
        public string Name { get; }
        public string Email { get; }
        public string NameEmail { get { return $"{Name} <{Email}>"; } }
        public int LinesAdded { get; set; }
        public int LinesDeleted { get; set; }
        
        public override string ToString()
        {
            return $"{LinesAdded} added. {LinesDeleted} deleted.";
        }
    }
}
