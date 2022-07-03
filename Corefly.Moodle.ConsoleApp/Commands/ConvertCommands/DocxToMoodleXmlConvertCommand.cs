﻿using System.ComponentModel;
using Corefly.Moodle.Converters.Converters;
using Corefly.Moodle.Converters.Models;
using Corefly.Moodle.Converters.Parsers;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Corefly.Moodle.ConsoleApp.Commands.ConvertCommands;

public class DocxToMoodleXmlConvertCommand : AsyncCommand<DocxToMoodleXmlConvertCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Path to source file")]
        [CommandArgument(0, "<PATH_TO_SOURCE_FILE>")]
        public string SourceFile { get; set; }

        [Description("Base name for all questions. Example: \"Question_1_{number}\" ({number} - is autogenerated)")]
        [CommandOption("--baseQuestionName")]
        public string BaseQuestionName { get; set; }

        [Description("General feedback")]
        [CommandOption("--generalfeedback")]
        public string GeneralFeedback { get; set; }

        [Description("Correct feedback")]
        [CommandOption("--correctfeedback")]
        [DefaultValue("Ваш ответ верный")]
        public string CorrectFeedback { get; set; }

        [Description("Partially correct feedback")]
        [CommandOption("--partiallyCorrectFeedback")]
        [DefaultValue("Ваш ответ частично правильный")]
        public string PartiallyCorrectFeedback { get; set; }

        [Description("Incorrect feedback")]
        [CommandOption("--IncorrectFeedback")]
        [DefaultValue("Ваш ответ неправильный")]
        public string IncorrectFeedback { get; set; }
    }

    public override ValidationResult Validate(CommandContext context, Settings settings)
    {
        var unavailableSourceFile = !File.Exists(settings.SourceFile);

        if (unavailableSourceFile)
        {
            return ValidationResult.Error($"Source file not found - {settings.SourceFile}");
        }

        if (string.IsNullOrEmpty(settings.BaseQuestionName))
        {
            return ValidationResult.Error("Input --baseQuestionName param");
        }

        if (string.IsNullOrEmpty(settings.GeneralFeedback))
        {
            return ValidationResult.Error("Input --generalfeedback param");
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
            var options = new MoodleXmlConverter.Options(settings.BaseQuestionName, settings.GeneralFeedback,
                settings.CorrectFeedback, settings.PartiallyCorrectFeedback, settings.IncorrectFeedback);
            var xml = new MoodleXmlConverter().Convert(questions, options);
            await File.WriteAllBytesAsync("moodle.xml", xml);
        }

        return 0;
    }
}
