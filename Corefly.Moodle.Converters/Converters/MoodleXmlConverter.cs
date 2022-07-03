using System.Xml.Linq;
using Corefly.Moodle.Converters.Converters.Interfaces;
using Corefly.Moodle.Converters.Models;

namespace Corefly.Moodle.Converters.Converters;

public class MoodleXmlConverter : IConverter
{
    public class Options
    {
        public Options(string baseQuestionName, string generalFeedback, string correctFeedback, string partiallyCorrectFeedback, string incorrectFeedback)
        {
            BaseQuestionName = baseQuestionName;
            GeneralFeedback = generalFeedback;
            CorrectFeedback = correctFeedback;
            PartiallyCorrectFeedback = partiallyCorrectFeedback;
            IncorrectFeedback = incorrectFeedback;
        }

        public string BaseQuestionName { get; }
        public string GeneralFeedback { get; }
        public string CorrectFeedback { get; }
        public string PartiallyCorrectFeedback { get; }
        public string IncorrectFeedback { get; }
    }

    public byte[] Convert(ICollection<Question> questions, Options options)
    {
        var questionElements = questions.Select((x, i) => CreateQuestionElement(x, i + 1, options)).ToList();
        var xmlDocument = new XDocument(
            new XDeclaration("1.0", null, null),
            new XElement("quiz", questionElements));

        using var ms = new MemoryStream();
        xmlDocument.Save(ms);

        return ms.ToArray();
    }

    private static XElement CreateQuestionElement(Question question, int number, Options options)
    {
        return new XElement("question",
            new XAttribute(XName.Get("type"), "multichoice"),
            new XElement("name", new XElement("text", $"{options.BaseQuestionName}_{number}")),
            new XElement("questiontext",
                new XAttribute(XName.Get("format"), "html"),
                new XElement("text", question.Text)),
            CreateAnswerElements(question),
            new XElement("generalfeedback",
                new XAttribute(new XAttribute(XName.Get("format"), "html")),
                new XElement("text", options.GeneralFeedback)),
            new XElement("single", question.IsSingleCorrectAnswer ? "true" : "false"),
            new XElement("correctfeedback",
                new XAttribute(XName.Get("format"), "html"),
                new XElement("text", options.CorrectFeedback)),
            new XElement("partiallycorrectfeedback",
                new XAttribute(XName.Get("format"), "html"),
                new XElement("text", options.PartiallyCorrectFeedback)),
            new XElement("incorrectfeedback",
                new XAttribute(XName.Get("format"), "html"),
                new XElement("text", options.IncorrectFeedback))
        );
    }

    private static ICollection<XElement> CreateAnswerElements(Question question)
    {
        var elements = new List<XElement>(question.Answers.Count);

        foreach (var answer in question.Answers)
        {
            var fractionValue = answer.IsCorrect ? question.CorrectAnswerCost : question.WrongAnswerCost;
            var answerElement = new XElement("answer",
                new XAttribute(XName.Get("fraction"), fractionValue),
                new XElement("text", answer.Text));

            elements.Add(answerElement);
        }

        return elements;
    }
}
