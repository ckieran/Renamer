using Renamer.Core.Enums;

namespace Renamer.Core.Models
{
    public class FileOperation
    {
        public required string SourcePath { get; set; }
        public required string DestinationPath { get; set; }
        public FileOperationType OperationType { get; set; }
        public OperationStatus Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}
