using Renamer.Core.Contracts;

namespace Renamer.Core.Serialization;

public interface IReportSerializer
{
    void Write(string outputPath, RenameReport report);
}
