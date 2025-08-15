using Renamer.Core.Models;

namespace Renamer.Core.Services
{
    public interface IRenameService
    {
        Task<RenamePlan> GenerateRenamePlanAsync(string rootPath);
        Task<OperationResult> ExecuteRenamePlanAsync(RenamePlan plan);
        Task<bool> ValidateOperationOrderAsync(List<FileOperation> operations);
    }
}