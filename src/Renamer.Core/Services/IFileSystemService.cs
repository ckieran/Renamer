using Renamer.Core.Models;

namespace Renamer.Core.Services
{
    public interface IFileSystemService
    {
        Task<FolderTree> BuildFolderTreeAsync(string rootPath);
        Task<bool> RenameFolderAsync(string oldPath, string newPath);
        Task<IEnumerable<string>> GetImageFilesAsync(string folderPath);
        Task<bool> ValidatePathAsync(string path);
    }
}