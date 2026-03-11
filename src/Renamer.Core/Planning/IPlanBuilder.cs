using Renamer.Core.Contracts;

namespace Renamer.Core.Planning;

public interface IPlanBuilder
{
    RenamePlan Build(string rootPath);
}
