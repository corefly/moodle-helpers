using System.Text.RegularExpressions;
using Corefly.Moodle.Converters.Models;
using Corefly.Moodle.Converters.Parsers.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Corefly.Moodle.Converters.Parsers;

public class DocxParser : IFileParser
{
    private const string AnswerPattern = @"^\*?(\w{1}|\w+)\W*";

    public ICollection<Question> Parse(Stream file)
    {
        var paragraphs = ReadParagraphs(file);
        var questions = CollectQuestions(paragraphs);

        return questions;
    }

    private static List<List<string>> ReadParagraphs(Stream file)
    {
        var paragraphs = new List<List<string>>();
        using var document = WordprocessingDocument.Open(file, false);
        var body = document.MainDocumentPart?.Document.Body;

        if (body == default)
        {
            return paragraphs;
        }

        foreach (var paragraph in body.Descendants<Paragraph>())
        {
            var text = paragraph.InnerText.Trim();
            var isEmptyText = text == string.Empty;

            if (!paragraphs.Any() || isEmptyText)
            {
                paragraphs.Add(new List<string>());
            }

            if (!isEmptyText)
            {
                paragraphs[^1].Add(text);
            }
        }

        return paragraphs;
    }

    private static ICollection<Question> CollectQuestions(List<List<string>> paragraphs)
    {
        var questions = new List<Question>();

        foreach (var paragraph in paragraphs.Where(x => x.Any()))
        {
            var questionText = paragraph[0];
            var answers = GetAnswersFromParagraph(paragraph);

            questions.Add(new Question(questionText, answers));
        }

        return questions;
    }

    private static ICollection<Answer> GetAnswersFromParagraph(List<string> paragraphLines)
    {
        var answerLines = paragraphLines.Skip(1);

        return answerLines
            .Skip(1)
            .Where(text => IsAnswer(text))
            .Select(text => CreateAnswer(text))
            .ToList();
    }

    private static bool IsAnswer(string text)
    {
        return Regex.IsMatch(text, AnswerPattern);
    }

    private static Answer CreateAnswer(string text)
    {
        var isCorrectAnswer = text.StartsWith('*');
        var answerText = Regex
            .Replace(text, AnswerPattern, string.Empty)
            .TrimEnd('.', ';');

        return new Answer(answerText, isCorrectAnswer);
    }
}
