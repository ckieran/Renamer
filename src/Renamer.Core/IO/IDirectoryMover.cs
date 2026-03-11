namespace Renamer.Core.IO;

public interface IDirectoryMover
{
    bool Exists(string path);

    void Move(string sourcePath, string destinationPath);
}
