
namespace GitRepoStats.CommandLine
{
    public class AuthorStats
    {
        public int LinesAdded { get; set; }
        public int LinesDeleted { get; set; }

        public override string ToString()
        {
            return $"{LinesAdded} added. {LinesDeleted} deleted.";
        }
    }
}
