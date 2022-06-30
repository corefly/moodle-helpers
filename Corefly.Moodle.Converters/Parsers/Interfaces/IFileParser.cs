using Corefly.Moodle.Converters.Models;

namespace Corefly.Moodle.Converters.Parsers.Interfaces;

public interface IFileParser
{
    ICollection<Question> Parse(Stream file);
}
