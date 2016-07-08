using System.Collections.Generic;

namespace GitRepoStats.CommandLine
{
    public class RepoStats
    {
        private Dictionary<string, AuthorStats> statistics = new Dictionary<string, AuthorStats>();
        
        public IReadOnlyDictionary<string, AuthorStats> Statistics { get { return statistics; } }

        public void IncrementAuthor(string author, int numLinesAdded, int numLinesDeleted)
        {
            AuthorStats authorStats;
            if(!statistics.TryGetValue(author, out authorStats))
            {                
                statistics[author] = new AuthorStats();
                authorStats = statistics[author];
            }
            authorStats.LinesAdded += numLinesAdded;
            authorStats.LinesDeleted += numLinesDeleted;
        }
    }
}
