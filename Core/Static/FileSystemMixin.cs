using FileSystem;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public static class FileSystemMixin
{
    public static IEnumerable<IZafiroFile> Files(this IZafiroDirectory directory)
    {
        var ownFiles = directory.Files;
        var fromChildren = directory.Directories
            .SelectMany(Files);

        return ownFiles.Concat(fromChildren);
    }
}