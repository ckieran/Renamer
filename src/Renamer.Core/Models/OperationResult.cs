namespace Renamer.Core.Models
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = [];
    }
}
