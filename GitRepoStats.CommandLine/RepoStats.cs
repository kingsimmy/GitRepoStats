using LibGit2Sharp;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace GitRepoStats.CommandLine
{
    public class RepoStats
    {        
        private Dictionary<string, AuthorStats> authorStatistics = new Dictionary<string, AuthorStats>();
        private Dictionary<string, ExtensionStats> extensionStatistics;

        public RepoStats(Repository repo)
        {
            CountPerAuthor(repo);
            CountPerExtension(repo);
        }

        public IReadOnlyDictionary<string, AuthorStats> AuthorStatistics { get { return authorStatistics; } }
        public IReadOnlyDictionary<string, ExtensionStats> ExtensionStatistics { get { return extensionStatistics; } }

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
            extensionStatistics = paths.GroupBy(p => Path.GetExtension(p), p => patches[p].LinesAdded)
                .ToDictionary(x => x.Key, x => new ExtensionStats(x.Count(), x.Sum()));
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

        public override string ToString()
        {            
            string authorsString = string.Concat(AuthorStatistics.SelectMany(x => x.Key + " " + x.Value + Environment.NewLine));
            string extensionsString = string.Concat(ExtensionStatistics.SelectMany(x => x.Key + " " + x.Value + Environment.NewLine));
            return authorsString + extensionsString;
        }
    }
}
