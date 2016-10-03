
namespace GitRepoStats.CommandLine
{
    public class ExtensionStats
    {
        public ExtensionStats(string extension, int numberOfFiles, int numberOfLines)
        {
            Extension = extension;
            NumberOfFiles = numberOfFiles;
            NumberOfLines = numberOfLines;
        }

        public string Extension { get; }
        public int NumberOfFiles { get; }
        public int NumberOfLines { get; }

        public override string ToString()
        {
            return $"{NumberOfFiles.ToString("N0")} files totalling {NumberOfLines.ToString("N0")} lines.";
        }
    }
}
