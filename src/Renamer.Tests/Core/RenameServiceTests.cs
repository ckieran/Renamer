using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Renamer.Core.Models;
using Renamer.Core.Services;
using Xunit;

namespace Renamer.Tests.Core
{
    // Minimal fake file system that tracks folder paths
    class FakeFileSystem : IFileSystemService
    {
        private readonly FolderTree _tree;

        public FakeFileSystem(FolderTree tree)
        {
            _tree = tree;
        }

        public Task<bool> RenameFolderAsync(string oldPath, string newPath)
        {
            var f = _tree.Folders.FirstOrDefault(x => x.Path == oldPath);
            if (f != null)
            {
                f.Path = newPath;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<IEnumerable<string>> GetImageFilesAsync(string folderPath)
        {
            return Task.FromResult<IEnumerable<string>>(Array.Empty<string>());
        }

        public Task<FolderTree> BuildFolderTreeAsync(string rootPath)
        {
            return Task.FromResult(_tree);
        }

        public Task<bool> ValidatePathAsync(string path)
        {
            return Task.FromResult(true);
        }
    }

    // Simple fake EXIF service (not used much by RenameService in tests)
    class FakeExifService : IExifService
    {
        public PhotoMetadata ExtractMetadata(string filePath) => new PhotoMetadata { FileName = System.IO.Path.GetFileName(filePath) };
        public DateTime? GetCaptureDate(string filePath) => null;
        public bool IsValidImageFile(string filePath) => false;
        public IEnumerable<string> GetSupportedExtensions() => Array.Empty<string>();
    }

    public class RenameServiceTests
    {
        [Fact]
        public async Task GenerateRenamePlan_ReturnsOperations_ForFoldersWithPhotos()
        {
            var folder = new FolderInfo
            {
                Path = "/root/FolderA",
                Photos = new List<PhotoMetadata>
                {
                    new PhotoMetadata { CaptureDate = new DateTime(2020,1,1) }
                }
            };

            var tree = new FolderTree { RootPath = "/root", Folders = new List<FolderInfo> { folder } };
            var fs = new FakeFileSystem(tree);
            var exif = new FakeExifService();

            var svc = new RenameService(fs, exif);
            var plan = await svc.GenerateRenamePlanAsync("/root");

            Assert.NotNull(plan);
            Assert.Single(plan.Operations);
            var op = plan.Operations[0];
            Assert.Equal(folder.Path, op.SourcePath);
            Assert.Contains("2020-01-01", op.DestinationPath);
        }

        [Fact]
        public async Task ExecuteRenamePlan_PerformsRenames_AndReturnsSuccess()
        {
            var folder = new FolderInfo
            {
                Path = "/root/FolderA",
                Photos = new List<PhotoMetadata>
                {
                    new PhotoMetadata { CaptureDate = new DateTime(2021,5,4) }
                }
            };
            var tree = new FolderTree { RootPath = "/root", Folders = new List<FolderInfo> { folder } };
            var fs = new FakeFileSystem(tree);
            var exif = new FakeExifService();
            var svc = new RenameService(fs, exif);

            var plan = await svc.GenerateRenamePlanAsync("/root");
            var result = await svc.ExecuteRenamePlanAsync(plan);

            Assert.True(result.Success);
            Assert.Empty(result.Errors);
            Assert.Contains("2021-05-04", tree.Folders[0].Path);
        }

        [Fact]
        public async Task ValidateOperationOrderAsync_DetectsDuplicateDestinations()
        {
            var ops = new List<FileOperation>
            {
                new FileOperation { SourcePath = "/a", DestinationPath = "/x" },
                new FileOperation { SourcePath = "/b", DestinationPath = "/x" }
            };

            var fs = new FakeFileSystem(new FolderTree());
            var exif = new FakeExifService();
            var svc = new RenameService(fs, exif);

            var ok = await svc.ValidateOperationOrderAsync(ops);
            Assert.False(ok);
        }
    }
}
