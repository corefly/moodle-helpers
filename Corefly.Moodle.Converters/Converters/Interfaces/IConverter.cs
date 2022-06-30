using Corefly.Moodle.Converters.Models;

namespace Corefly.Moodle.Converters.Converters.Interfaces;

public interface IConverter
{
    public byte[] Convert(ICollection<Question> questions);
}
