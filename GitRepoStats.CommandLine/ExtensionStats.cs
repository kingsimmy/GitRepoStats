
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
            return $"{NumberOfFiles} files totalling {NumberOfLines} lines.";
        }
    }
}
