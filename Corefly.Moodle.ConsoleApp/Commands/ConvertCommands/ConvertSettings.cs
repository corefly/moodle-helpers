using System.ComponentModel;
using Spectre.Console.Cli;

namespace Corefly.Moodle.ConsoleApp.Commands.ConvertCommands;

public class ConvertSettings : CommandSettings
{
    public ConvertSettings(string sourceFile, string destinationFileLocation)
    {
        SourceFile = sourceFile;
        DestinationFileLocation = destinationFileLocation;
    }

    [Description("Path to source file")]
    [CommandArgument(0, "<PATH_TO_SOURCE_FILE>")]
    public string SourceFile { get; }

    [Description("Path to destination folder")]
    [CommandOption("-d|--destination <DESTINATION_FOLDER>")]
    [DefaultValue(".")]
    public string DestinationFileLocation { get; }
}
