namespace Renamer.Core.Models
{
    public class FolderTree
    {
        public string RootPath { get; set; } = string.Empty;
        public List<FolderInfo> Folders { get; set; } = [];
    }
}
