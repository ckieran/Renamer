using Renamer.Core.Contracts;

namespace Renamer.Core.Execution;

public interface IApplyEngine
{
    RenameReport Execute(RenamePlan plan);
}
