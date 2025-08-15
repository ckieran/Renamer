namespace Renamer.Core.Models
{
    public class FolderInfo
    {
        public string Path { get; set; } = string.Empty;
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public int FileCount { get; set; }
        public List<PhotoMetadata> Photos { get; set; } = [];
    }
}
