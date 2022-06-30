namespace Corefly.Moodle.Converters.Models;

public class Answer
{
    public string Text { get; }
    public bool IsCorrect { get; }

    public Answer(string text, bool isCorrect)
    {
        Text = text;
        IsCorrect = isCorrect;
    }
}
