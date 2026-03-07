using Renamer.Core.Exif;

namespace Renamer.Core.Planning;

public interface IFolderDateRangeCalculator
{
    FolderDateRangeResult Calculate(IEnumerable<ExifReadResult> photoMetadata);
}
