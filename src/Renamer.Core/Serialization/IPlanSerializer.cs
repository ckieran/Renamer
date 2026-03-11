using Renamer.Core.Contracts;

namespace Renamer.Core.Serialization;

public interface IPlanSerializer
{
    RenamePlan Read(string inputPath);

    void Write(string outputPath, RenamePlan plan);
}
