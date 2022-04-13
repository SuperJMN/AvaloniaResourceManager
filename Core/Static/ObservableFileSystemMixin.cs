using System.Reactive.Concurrency;
using System.Reactive.Linq;
using FileSystem;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public static class ObservableFileSystemMixin
{


    public static IObservable<IZafiroFile> Files(this IZafiroDirectory directory, IScheduler scheduler)
    {
        var ownFiles = directory.Files.ToObservable(scheduler);
        var fromChildren = directory.Directories
            .ToObservable(scheduler)
            .SelectMany(d => Files(d, scheduler));

        return ownFiles.Concat(fromChildren);
    }

    public static IObservable<T> Select<T>(this IObservable<IZafiroFile> fileObservable, Func<Stream, IObservable<T>> transform)
    {
        return fileObservable
            .Select(file => Observable.Using(file.OpenRead, transform))
            .Merge();
    }
}

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