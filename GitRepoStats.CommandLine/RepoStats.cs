using LibGit2Sharp;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GitRepoStats.CommandLine
{
    public class RepoStats
    {        
        private Dictionary<string, AuthorStats> authorStatistics = new Dictionary<string, AuthorStats>();
        private Dictionary<string, int> linesPerExtension;
        private Dictionary<string, int> filesPerExtension;

        public RepoStats(Repository repo)
        {
            CountPerAuthor(repo);
            CountPerExtension(repo);
        }

        public IReadOnlyDictionary<string, AuthorStats> AuthorStatistics { get { return authorStatistics; } }
        public IReadOnlyDictionary<string, int> LinesPerExtension { get { return linesPerExtension; } }
        public IReadOnlyDictionary<string, int> FilesPerExtension { get { return filesPerExtension; } }

        private void CountPerAuthor(Repository repo)
        {
            foreach (Commit commit in repo.Commits)
            {
                if (commit.Parents.Count() != 1)
                {
                    continue;
                }
                PatchStats stats = repo.Diff.Compare<PatchStats>(commit.Parents.First().Tree, commit.Tree);
                IncrementAuthor(commit.Author.ToString(), stats.TotalLinesAdded, stats.TotalLinesDeleted);
            }
        }

        private void CountPerExtension(Repository repo)
        {
            List<string> paths = repo.Index.Select(indexEntry => indexEntry.Path).ToList();
            PatchStats patches = repo.Diff.Compare<PatchStats>(null, repo.Head.Tip.Tree);
            linesPerExtension = paths.GroupBy(p => Path.GetExtension(p), p => patches[p].LinesAdded).ToDictionary(x => x.Key, x => x.Sum());
            filesPerExtension = paths.GroupBy(p => Path.GetExtension(p)).ToDictionary(x => x.Key, x => x.Count());
        }

        private void IncrementAuthor(string author, int numLinesAdded, int numLinesDeleted)
        {
            AuthorStats authorStats;
            if(!authorStatistics.TryGetValue(author, out authorStats))
            {
                authorStatistics[author] = new AuthorStats();
                authorStats = authorStatistics[author];
            }
            authorStats.LinesAdded += numLinesAdded;
            authorStats.LinesDeleted += numLinesDeleted;
        }
    }
}
