using LibGit2Sharp;
using System;

namespace GitRepoStats.CommandLine
{
    public class GitRepoAnalyser
    {
        static void Main(string[] args)
        {
            RepoStats repoStats = GetRepoStats(args[0]);
            Console.WriteLine(repoStats);
        }

        private static RepoStats GetRepoStats(string repoPath)
        {            
            Repository repo = new Repository(repoPath);
            RepoStats repoStats = new RepoStats(repo);
            return repoStats;
        }
    }
}
