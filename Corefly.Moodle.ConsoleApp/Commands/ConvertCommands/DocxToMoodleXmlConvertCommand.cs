using Corefly.Moodle.Converters.Converters;
using Corefly.Moodle.Converters.Models;
using Corefly.Moodle.Converters.Parsers;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Corefly.Moodle.ConsoleApp.Commands.ConvertCommands;

public class DocxToMoodleXmlConvertCommand : AsyncCommand<DocxToMoodleXmlConvertCommand.Settings>
{
    public class Settings : ConvertSettings
    {
        public Settings(string sourceFile, string destinationFileLocation) : base(sourceFile, destinationFileLocation)
        {
        }
    }

    public override ValidationResult Validate(CommandContext context, Settings settings)
    {
        var unavailableSourceFile = !File.Exists(settings.SourceFile);

        if (unavailableSourceFile)
        {
            return ValidationResult.Error($"Source file not found - {settings.SourceFile}");
        }

        return base.Validate(context, settings);
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        ICollection<Question> questions;

        await using (var fileStream = File.OpenRead(settings.SourceFile))
        {
            questions = new DocxParser().Parse(fileStream);
        }

        if (questions.Any())
        {
            var xml = new MoodleXmlConverter().Convert(questions);
            var path = Path.Combine(settings.DestinationFileLocation, "moodle.xml");
            await File.WriteAllBytesAsync(path, xml);
        }

        return 0;
    }
}
