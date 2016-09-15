
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
            return $"{NameEmail} {NumberOfCommits.ToString("N0")} commits. {LinesAdded.ToString("N0")} added. {LinesDeleted.ToString("N0")} deleted.";
        }
    }
}
