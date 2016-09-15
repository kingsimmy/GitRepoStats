
namespace GitRepoStats.CommandLine
{
    public class ExtensionStats
    {
        public ExtensionStats(int numberOfFiles, int numberOfLines)
        {
            NumberOfFiles = numberOfFiles;
            NumberOfLines = numberOfLines;
        }
        public int NumberOfFiles { get; }
        public int NumberOfLines { get; }

        public override string ToString()
        {
            return $"{NumberOfFiles.ToString("N0")} files totalling {NumberOfLines.ToString("N0")} lines.";
        }
    }
}
