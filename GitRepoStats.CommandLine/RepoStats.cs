﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using GitRepoStats.CommandLine.Extensions;
using HtmlGenerator;
using LibGit2Sharp;
using Tag = HtmlGenerator.Tag;
using Attribute = HtmlGenerator.Attribute;

namespace GitRepoStats.CommandLine
{
    public class RepoStats
    {        
        private Dictionary<string, AuthorStats> authorStatistics = new Dictionary<string, AuthorStats>();
        private Dictionary<string, ExtensionStats> extensionStatistics;

        public RepoStats(Repository repo)
        {
            RepoPath = repo.Info.Path;
            CountPerAuthor(repo);
            CountPerExtension(repo);
        }

        public string RepoPath { get; }
        public string RepoName { get { return new DirectoryInfo(RepoPath).Parent.Name; } }
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
                IncrementAuthor(commit.Author, stats.TotalLinesAdded, stats.TotalLinesDeleted);
            }
        }

        private void CountPerExtension(Repository repo)
        {
            List<string> paths = repo.Index.Select(indexEntry => indexEntry.Path).ToList();
            PatchStats patches = repo.Diff.Compare<PatchStats>(null, repo.Head.Tip.Tree);            
            extensionStatistics = paths.GroupBy(p => Path.GetExtension(p), p => patches[p].LinesAdded)
                .Where(x => !string.IsNullOrEmpty(x.Key))
                .ToDictionary(x => x.Key, x => new ExtensionStats(x.Key, x.Count(), x.Sum()));
        }

        private void IncrementAuthor(Signature author, int numLinesAdded, int numLinesDeleted)
        {
            AuthorStats authorStats;
            if(!authorStatistics.TryGetValue(author.Email, out authorStats))
            {
                authorStatistics[author.Email] = new AuthorStats(author.Name, author.Email);
                authorStats = authorStatistics[author.Email];
            }
            authorStats.NumberOfCommits += 1;
            authorStats.LinesAdded += numLinesAdded;
            authorStats.LinesDeleted += numLinesDeleted;
        }

        public override string ToString()
        {            
            string authorsString = string.Concat(AuthorStatistics.Values.OrderByDescending(x => x.NumberOfCommits).SelectMany(x => x + Environment.NewLine));
            string extensionsString = string.Concat(ExtensionStatistics.OrderByDescending(x => x.Value.NumberOfFiles).SelectMany(x => x.Key + " " + x.Value + Environment.NewLine));
            return RepoName + Environment.NewLine + authorsString + extensionsString;
        }

        public HtmlElement ToHtml()
        {
            return Tag.Div.WithClass("child").WithChildren(Tag.H3.WithInnerText(RepoName).WithClass("repoHeader"),
                AuthorsTable(), ExtensionsTable());
        }

        private HtmlElement ExtensionsTable()
        {
            List<HtmlElement> rows = ExtensionStatistics.Values.OrderByDescending(x => x.NumberOfFiles).Select(x => ExtensionsRow(x)).ToList();
            return Tag.Table.WithChildren(Tag.Thead.WithChild(ExtensionsHeader()), Tag.Tbody.WithChildren(rows)).WithAttribute(Attribute.Id($"{RepoName}_e")).WithAttribute(Attribute.Class("eTable"));
        }

        private HtmlElement ExtensionsHeader()
        {
            return Tag.Tr.WithChildren(Tag.Th.WithInnerText("Extension"), Tag.Th.WithInnerText("Files"), Tag.Th.WithInnerText("Lines"));
        }

        private HtmlElement ExtensionsRow(ExtensionStats stats)
        {
            return Tag.Tr.WithChildren(Tag.Td.WithInnerText(WebUtility.HtmlEncode(stats.Extension)), Tag.Td.WithInnerText(stats.NumberOfFiles.ToString("N0")),
                Tag.Td.WithInnerText(stats.NumberOfLines.ToString("N0")));
        }

        private HtmlElement AuthorsTable()
        {
            List<HtmlElement> rows = AuthorStatistics.Values.OrderByDescending(x => x.NumberOfCommits).Select(x => AuthorRow(string.Concat(x.NameEmail.Take(50)), x)).ToList();
            return Tag.Table.WithChildren(Tag.Thead.WithChild(AuthorHeader()), Tag.Tbody.WithChildren(rows)).WithAttribute(Attribute.Id($"{RepoName}_a")).WithAttribute(Attribute.Class("aTable"));
        }

        private HtmlElement AuthorHeader()
        {
            return Tag.Tr.WithChildren(Tag.Th.WithInnerText("Author"), Tag.Th.WithInnerText("Commits"), Tag.Th.WithInnerText("Added"), Tag.Th.WithInnerText("Deleted"));
        }

        private HtmlElement AuthorRow(string author, AuthorStats stats)
        {
            return Tag.Tr.WithChildren(Tag.Td.WithInnerText(WebUtility.HtmlEncode(author)), Tag.Td.WithInnerText(stats.NumberOfCommits.ToString("N0")),
                Tag.Td.WithInnerText(stats.LinesAdded.ToString("N0")), Tag.Td.WithInnerText(stats.LinesDeleted.ToString("N0")));
        }
    }
}
