namespace Renamer.Core.Models
{
    public class PhotoMetadata
    {
        public string FileName { get; set; } = string.Empty;
        public DateTime? CaptureDate { get; set; } = null;
        // Add other metadata properties as needed

        public PhotoMetadata()
        {
            FileName = string.Empty;
            CaptureDate = null;
        }
    }
}
