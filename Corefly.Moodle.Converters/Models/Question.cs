namespace Corefly.Moodle.Converters.Models;

public class Question
{
    public string Text { get; }
    public ICollection<Answer> Answers { get; }
    public decimal CorrectAnswerCost => Math.Round((decimal)100 / CorrectAnswerCount, 5);
    public decimal WrongAnswerCost => CorrectAnswerCount > 1 ? Math.Round((decimal)-100 / WrongAnswerCount, 5) : 0;
    public bool IsSingleCorrectAnswer => CorrectAnswerCount == 1;

    private int CorrectAnswerCount => Answers.Count(x => x.IsCorrect);
    private int WrongAnswerCount => Answers.Count - CorrectAnswerCount;

    public Question(string text, ICollection<Answer> answers)
    {
        Text = text;
        Answers = answers;
    }
}
