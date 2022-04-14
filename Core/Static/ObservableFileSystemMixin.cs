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

    public static IObservable<T> Select<T>(this IObservable<IZafiroFile> fileObservable, Func<Stream, ZafiroPath, IObservable<T>> transform)
    {
        return fileObservable
            .Select(file => Observable.Using(file.OpenRead, stream => transform(stream, file.Path)))
            .Merge();
    }

    public static IObservable<T> ForEachFileSelect<T>(this IZafiroDirectory directory, Func<Stream, IObservable<T>> func)
    {
        var result = directory.Files(TaskPoolScheduler.Default)
            .Where(file => file.Path.ToString().EndsWith(".axaml"))
            .Select(stream => Observable.Start(() => func(stream), TaskPoolScheduler.Default))
            .Merge(4);

        return result;
    }

    public static IObservable<T> ForEachFileSelect<T>(this IZafiroDirectory directory, Func<Stream, ZafiroPath, IObservable<T>> func)
    {
        var result = directory.Files(TaskPoolScheduler.Default)
            .Where(file => file.Path.ToString().EndsWith(".axaml"))
            .Select((stream, path) => Observable.Start(() => func(stream, path), TaskPoolScheduler.Default))
            .Merge(4);

        return result;
    }
}