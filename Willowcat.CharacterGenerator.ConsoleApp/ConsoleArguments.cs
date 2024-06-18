using CommandLine;

namespace Willowcat.CharacterGenerator.ConsoleApp
{
    [Verb("initialize", HelpText = "Initialize database from flat files")]
    public class ConsoleArguments
    {
        [Option('d', "delete", HelpText = "Delete existing database before initializing.")]
        public bool DeleteExistingDatabase { get; private set; }
    }
}
