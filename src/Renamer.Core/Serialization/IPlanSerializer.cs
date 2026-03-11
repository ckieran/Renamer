using Renamer.Core.Contracts;

namespace Renamer.Core.Serialization;

public interface IPlanSerializer
{
    void Write(string outputPath, RenamePlan plan);
}
