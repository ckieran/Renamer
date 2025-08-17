using Microsoft.Extensions.Logging;
using Renamer.Core.Models;

namespace Renamer.Core.Services
{
    public class FileSystemService : IFileSystemService
    {
        private readonly IExifService _exif;
        private readonly ILogger<FileSystemService> _logger;

        public FileSystemService(IExifService exif, ILogger<FileSystemService> logger)
        {
            _exif = exif;
            _logger = logger;
        }

        public async Task<FolderTree> BuildFolderTreeAsync(string rootPath)
        {
            var tree = new FolderTree { RootPath = rootPath };
            await ScanDirectoryAsync(rootPath, tree).ConfigureAwait(false);
            return tree;
        }

        private async Task ScanDirectoryAsync(string dir, FolderTree tree, int depth = 0)
        {
            if (depth > 10) return; // guard depth
            var folder = new FolderInfo { Path = dir };
            var files = Directory.Exists(dir) ? Directory.EnumerateFiles(dir).ToList() : new List<string>();
            folder.FileCount = files.Count;
            folder.Photos = new List<PhotoMetadata>();
            foreach (var f in files)
            {
                try
                {
                    if (_exif.IsValidImageFile(f))
                    {
                        var meta = _exif.ExtractMetadata(f);
                        folder.Photos.Add(meta);
                    }
                }
                catch
                {
                    // ignore individual file errors
                    _logger.LogWarning("Failed to extract metadata for file {FilePath}", f);
                }
            }
            if (folder.Photos != null && folder.Photos.Any(p => p.CaptureDate.HasValue))
            {
                folder.MinDate = folder.Photos.Where(p => p.CaptureDate.HasValue).Min(p => p.CaptureDate);
                folder.MaxDate = folder.Photos.Where(p => p.CaptureDate.HasValue).Max(p => p.CaptureDate);
            }
            tree.Folders.Add(folder);

            // recurse into subdirectories
            try
            {
                foreach (var sd in Directory.EnumerateDirectories(dir))
                {
                    await ScanDirectoryAsync(sd, tree, depth + 1).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning directory {Directory}", dir);
            }
        }

        public async Task<bool> RenameFolderAsync(string oldPath, string newPath)
        {
            try
            {
                await Task.Run(() => Directory.Move(oldPath, newPath));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rename folder from {OldPath} to {NewPath}", oldPath, newPath);
                return false;
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
}