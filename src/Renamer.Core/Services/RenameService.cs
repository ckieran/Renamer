using Renamer.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Renamer.Core.Services
{
    public class RenameService : IRenameService
    {
        private readonly IFileSystemService _fs;
        private readonly IExifService _exif;

        public RenameService(IFileSystemService fs, IExifService exif)
        {
            _fs = fs;
            _exif = exif;
        }

        public async Task<RenamePlan> GenerateRenamePlanAsync(string rootPath)
        {
            var tree = await _fs.BuildFolderTreeAsync(rootPath).ConfigureAwait(false);
            var plan = new RenamePlan();

            foreach (var folder in tree.Folders)
            {
                if (folder.Photos == null || folder.Photos.Count == 0)
                    continue;

                var dates = folder.Photos.Where(p => p.CaptureDate.HasValue).Select(p => p.CaptureDate!.Value).ToList();
                if (dates.Count == 0) continue;

                var min = dates.Min();
                var max = dates.Max();
                folder.MinDate = min;
                folder.MaxDate = max;

                var newName = GenerateFolderName(min, max, Path.GetFileName(folder.Path));
                var dest = Path.Combine(Path.GetDirectoryName(folder.Path) ?? string.Empty, newName);
                if (!string.Equals(folder.Path, dest, StringComparison.OrdinalIgnoreCase))
                {
                    plan.Operations.Add(new FileOperation
                    {
                        SourcePath = folder.Path,
                        DestinationPath = dest,
                        OperationType = Enums.FileOperationType.FolderRename
                    });
                }
            }

            return plan;
        }

        private string GenerateFolderName(DateTime min, DateTime max, string existing)
        {
            if (min == max) return $"{min:yyyy-MM-dd} - {existing}";
            return $"{min:yyyy-MM-dd} - {max:yyyy-MM-dd} - {existing}";
        }

        public async Task<OperationResult> ExecuteRenamePlanAsync(RenamePlan plan)
        {
            var result = new OperationResult();
            result.Success = true;
            foreach (var op in plan.Operations)
            {
                try
                {
                    var ok = await _fs.RenameFolderAsync(op.SourcePath, op.DestinationPath).ConfigureAwait(false);
                    if (!ok)
                    {
                        op.Status = Enums.OperationStatus.Failed;
                        op.ErrorMessage = "Rename failed";
                        result.Errors.Add(op.ErrorMessage);
                        result.Success = false;
                    }
                    else
                    {
                        op.Status = Enums.OperationStatus.Completed;
                        op.ErrorMessage = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    op.Status = Enums.OperationStatus.Failed;
                    op.ErrorMessage = ex.Message;
                    result.Errors.Add(ex.Message);
                    result.Success = false;
                }
            }
            return result;
        }

        public Task<bool> ValidateOperationOrderAsync(List<FileOperation> operations)
        {
            var dests = operations.Select(o => o.DestinationPath).ToList();
            return Task.FromResult(dests.Distinct(StringComparer.OrdinalIgnoreCase).Count() == dests.Count);
        }
    }
}
