using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Renamer.Core.Services
{
    public class FileSystemService : IFileSystemService
    {
        public async Task<FolderTree> BuildFolderTreeAsync(string rootPath)
        {
            // Minimal stub for now
            return await Task.FromResult(new FolderTree { Path = rootPath });
        }

        public async Task<bool> RenameFolderAsync(string oldPath, string newPath)
        {
            try
            {
                Directory.Move(oldPath, newPath);
                return await Task.FromResult(true);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<IEnumerable<string>> GetImageFilesAsync(string folderPath)
        {
            var supportedExtensions = new[] { ".jpg", ".jpeg", ".jpe", ".dng", ".cr2", ".nef", ".arw", ".raf", ".tiff", ".tif", ".png", ".heic" };
            var files = Directory.EnumerateFiles(folderPath)
                .Where(f => supportedExtensions.Contains(Path.GetExtension(f).ToLower()));
            return await Task.FromResult(files);
        }

        public async Task<bool> ValidatePathAsync(string path)
        {
            return await Task.FromResult(Directory.Exists(path) || File.Exists(path));
        }
    }

    // Minimal FolderTree class for stub
    public class FolderTree
    {
        public string Path { get; set; }
    }
}