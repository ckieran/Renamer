namespace Renamer.Core.Models
{
    public class RenamePlan
    {
        public List<FileOperation> Operations { get; set; }
        public bool IsPreview { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
