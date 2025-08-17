namespace Renamer.Core.Models
{
    public class RenamePlan
    {
        public List<FileOperation> Operations { get; set; } = new List<FileOperation>();
        public bool IsPreview { get; set; } = true;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
