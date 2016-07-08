using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GitRepoStats.CommandLine
{
    public class GitRepoAnalyser
    {
        static void Main(string[] args)
        {
            RepoStats repoStats = GetRepoStats(args[0]);
            foreach(KeyValuePair<string, AuthorStats> pair in repoStats.Statistics)
            {
                Console.WriteLine(pair.Key + " " + pair.Value);
            }
            Console.WriteLine();
        }

        private static RepoStats GetRepoStats(string repoPath)
        {
            RepoStats repoStats = new RepoStats();
            Repository repo = new Repository(repoPath);
            foreach (Commit commit in repo.Commits)
            {
                if (commit.Parents.Count() != 1)
                {
                    continue;
                }
                PatchStats stats = repo.Diff.Compare<PatchStats>(commit.Parents.First().Tree, commit.Tree);
                repoStats.IncrementAuthor(commit.Author.ToString(), stats.TotalLinesAdded, stats.TotalLinesDeleted);
            }
            return repoStats;
        }
    }
}
