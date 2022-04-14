using System.Reactive.Concurrency;
using System.Reactive.Linq;
using FileSystem;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public static class ObservableFileSystemMixin
{
    public static IObservable<IZafiroFile> Files(this IZafiroDirectory directory, IScheduler scheduler)
    {
        var ownFiles = directory.Files.ToObservable();
        var fromChildren = directory.Directories
            .ToObservable(scheduler)
            .SelectMany(d => Files(d, CurrentThreadScheduler.Instance));

        return ownFiles.Concat(fromChildren);
    }

    public static IObservable<T> Select<T>(this IObservable<IZafiroFile> fileObservable, Func<Stream, IObservable<T>> transform)
    {
        return fileObservable
            .Select(file => Observable.Using(file.OpenRead, transform))
            .Merge();
    }
}