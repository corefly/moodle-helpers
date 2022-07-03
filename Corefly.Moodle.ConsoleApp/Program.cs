using Corefly.Moodle.ConsoleApp.Commands.ConvertCommands;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Corefly.Moodle.ConsoleApp;

internal class Program
{
    private static int Main(string[] args)
    {
        AnsiConsole.Write(new FigletText("Moodle helpers").Color(Color.Plum1));

        var app = new CommandApp();

        app.Configure(config =>
        {
            config.AddBranch("convert", convert =>
            {
                convert.AddCommand<DocxToMoodleXmlConvertCommand>("docx-to-moodle-xml")
                    .WithDescription("Convert docx to moodle xml");
            });
        });

        return app.Run(args);
    }
}
