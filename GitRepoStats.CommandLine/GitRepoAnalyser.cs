using HtmlGenerator;
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
            List<string> argList = args.ToList();
            bool htmlOutput = argList.Remove("-html");
            RepoStats repoStats = GetRepoStats(args[0]);
            string output;
            if (htmlOutput)
            {
                HtmlDocument document = new HtmlDocument();
                HtmlElement repoElement = repoStats.ToHtml();
                document.Body.AddChild(repoElement);
                output = document.Serialize().Replace("\r", Environment.NewLine);                
            }
            else
            {
                output = repoStats.ToString();
            }
            Console.WriteLine(output);
        }

        private static RepoStats GetRepoStats(string repoPath)
        {            
            Repository repo = new Repository(repoPath);
            RepoStats repoStats = new RepoStats(repo);
            return repoStats;
        }
    }
}
