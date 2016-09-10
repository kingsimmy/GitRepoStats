using HtmlGenerator;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Attribute = HtmlGenerator.Attribute;
using Tag = HtmlGenerator.Tag;

namespace GitRepoStats.CommandLine
{
    public class GitRepoAnalyser
    {
        static void Main(string[] args)
        {
            List<string> argList = args.ToList();
            bool htmlOutput = argList.Remove("-html");
            string outFilePath = GetOutFilePath(argList);
            IEnumerable<RepoStats> repoStats = GetRepoStats(argList);
            string output = htmlOutput ? GenerateHtml(repoStats) : GenerateString(repoStats);
                        
            if (String.IsNullOrEmpty(outFilePath))
            {
                Console.WriteLine(output);
            }
            else
            {
                File.WriteAllText(outFilePath, output);
                Console.WriteLine("Output has been written to " + outFilePath);
            }
        }

        private static string GetOutFilePath(List<string> args)
        {
            int outFileIndex = args.IndexOf("-outFile");
            if (outFileIndex == -1)
            {
                return string.Empty;
            }
            if (outFileIndex == args.Count - 1)
            {
                Console.WriteLine("-outFile flag must be followed by a file name");
                Environment.Exit(1);
            }            
            string outFilePath = args[outFileIndex + 1];
            args.RemoveRange(outFileIndex, 2);
            if (Directory.Exists(outFilePath))
            {
                Console.WriteLine($"-outFile value {outFilePath} is a directory. Please pass a file path for -outFile");
                Environment.Exit(1);
            }
            return outFilePath;
        }

        private static string GenerateHtml(params RepoStats[] allStats)
        {
            return GenerateHtml(allStats);
        }

        private static string GenerateHtml(IEnumerable<RepoStats> allStats)
        {
            HtmlDocument document = new HtmlDocument();
            document.Body.Children.Add(Tag.Style.WithInnerText(LoadCssString()).WithAttribute(Attribute.Type("text/css")));
            Collection<HtmlElement> elements = new Collection<HtmlElement>(allStats.Select(x => x.ToHtml()).ToList());
            document.Body.AddChild(Tag.H1.WithClass("pageHeader").WithInnerText("Git Repo Stats"));
            document.Body.AddChild(Tag.Div.WithChildren(elements).WithClass("parent"));
            return document.Serialize().Replace("\r", Environment.NewLine);
        }

        private static string LoadCssString()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GitRepoStats.CommandLine.Resources.style.css"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return "\r" + reader.ReadToEnd().Replace(Environment.NewLine, "\r") + "\r\t";
            }
        }

        private static string GenerateString(params RepoStats[] allStats)
        {
            return GenerateHtml(allStats);
        }

        private static string GenerateString(IEnumerable<RepoStats> allStats)
        {
            Func<RepoStats, string> repoStatsToString = 
                repoStats => repoStats.ToString() + Environment.NewLine + Environment.NewLine;
            return new String(allStats.SelectMany(repoStatsToString).ToArray());
        }

        private static IEnumerable<RepoStats> GetRepoStats(params string[] repoPaths)
        {
            return GetRepoStats(repoPaths);
        }

        private static IEnumerable<RepoStats> GetRepoStats(IEnumerable<string> repoPaths)
        {
            foreach (string repoPath in repoPaths)
            {
                Repository repo = new Repository(repoPath);
                yield return new RepoStats(repo);
            }
        }
    }
}
