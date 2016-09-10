
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
        public int NumberOfCommits { get; set; }
        public int LinesAdded { get; set; }
        public int LinesDeleted { get; set; }
        
        public override string ToString()
        {
            return $"{NumberOfCommits} commits. {LinesAdded} added. {LinesDeleted} deleted.";
        }
    }
}
