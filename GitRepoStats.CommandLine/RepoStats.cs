using LibGit2Sharp;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using Tag = HtmlGenerator.Tag;
using HtmlGenerator;
using System.Collections.ObjectModel;
using System.Net;

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
                .ToDictionary(x => x.Key, x => new ExtensionStats(x.Count(), x.Sum()));
        }

        private void IncrementAuthor(Signature author, int numLinesAdded, int numLinesDeleted)
        {
            AuthorStats authorStats;
            if(!authorStatistics.TryGetValue(author.Email, out authorStats))
            {
                authorStatistics[author.Email] = new AuthorStats(author.Name, author.Email);
                authorStats = authorStatistics[author.Email];
            }
            authorStats.LinesAdded += numLinesAdded;
            authorStats.LinesDeleted += numLinesDeleted;
        }

        public override string ToString()
        {            
            string authorsString = string.Concat(AuthorStatistics.SelectMany(x => x.Value.NameEmail + " " + x.Value + Environment.NewLine));
            string extensionsString = string.Concat(ExtensionStatistics.SelectMany(x => x.Key + " " + x.Value + Environment.NewLine));
            return RepoPath + Environment.NewLine + authorsString + extensionsString;
        }

        public HtmlElement ToHtml()
        {
            return AuthorsTable();
        }

        private HtmlElement AuthorsTable()
        {
            List<HtmlElement> rows = new List<HtmlElement> { Tag.Tr.WithInnerText("<td>Author</td><td>Added</td><td>Deleted</td>")};
            rows.AddRange(AuthorStatistics.Select(x => AuthorRow(x.Value.NameEmail, x.Value)));
            return Tag.Table.WithChildren(new Collection<HtmlElement>(rows));
        }

        private HtmlElement AuthorRow(string author, AuthorStats stats)
        {
            return Tag.Tr.WithChildren(new Collection<HtmlElement>
            {
                Tag.Td.WithInnerText(WebUtility.HtmlEncode(author)),
                Tag.Td.WithInnerText(stats.LinesAdded.ToString()),
                Tag.Td.WithInnerText(stats.LinesDeleted.ToString())
            });
        }
    }
}
