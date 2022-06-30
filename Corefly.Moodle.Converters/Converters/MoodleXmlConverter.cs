using System.Xml.Linq;
using Corefly.Moodle.Converters.Converters.Interfaces;
using Corefly.Moodle.Converters.Models;

namespace Corefly.Moodle.Converters.Converters;

public class MoodleXmlConverter : IConverter
{
    public byte[] Convert(ICollection<Question> questions)
    {
        var questionElements = questions.Select((x, i) => CreateQuestionElement(x, i + 1)).ToList();
        var xmlDocument = new XDocument(
            new XDeclaration("1.0", null, null),
            new XElement("quiz", questionElements));

        using var ms = new MemoryStream();
        xmlDocument.Save(ms);

        return ms.ToArray();
    }

    private static XElement CreateQuestionElement(Question question, int number)
    {
        return new XElement("question",
            new XAttribute(XName.Get("type"), "multichoice"),
            // TODO: Move to param
            new XElement("name", new XElement("text", $"Тема_8_{number}")),
            new XElement("questiontext",
                new XAttribute(XName.Get("format"), "html"),
                new XElement("text", question.Text)),
            CreateAnswerElements(question),
            new XElement("generalfeedback",
                new XAttribute(new XAttribute(XName.Get("format"), "html")),
                // TODO: Move to param
                new XElement("text", "Изучите раздел 8 \"Экономическая роль государства в Республике Беларусь\" учебника \"Национальная экономика Беларуси\" (под ред. В.Н. Шимова, 5-е изд., 2018 г.).")),
            new XElement("single", question.IsSingleCorrectAnswer ? "true" : "false"),
            new XElement("correctfeedback",
                new XAttribute(XName.Get("format"), "html"),
                // TODO: Move to param
                new XElement("text", "Ваш ответ верный")),
            new XElement("partiallycorrectfeedback",
                new XAttribute(XName.Get("format"), "html"),
                // TODO: Move to param
                new XElement("text", "Ваш ответ частично правильный")),
            new XElement("incorrectfeedback",
                new XAttribute(XName.Get("format"), "html"),
                // TODO: Move to param
                new XElement("text", "Ваш ответ неправильный"))
        );
    }

    private static ICollection<XElement> CreateAnswerElements(Question question)
    {
        var elements = new List<XElement>(question.Answers.Count);

        foreach (var answer in question.Answers)
        {
            var fractionValue = answer.IsCorrect ? question.CorrectAnswerCost: question.WrongAnswerCost;
            var answerElement = new XElement("answer",
                new XAttribute(XName.Get("fraction"), fractionValue),
                new XElement("text", answer.Text));

            elements.Add(answerElement);
        }

        return elements;
    }
}
