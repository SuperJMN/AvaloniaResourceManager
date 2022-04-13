using System.Reactive.Concurrency;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using FileSystem;
using MoreLinq;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public class FileProcessor<T>
{
    private readonly Func<Stream, IEnumerable<T>> func;
    private readonly IScheduler scheduler;
    private readonly ZafiroPath path;
    private readonly ZafiroFileSystem zafiroFileSystem;
    private readonly Func<IZafiroFile, bool> fileFilter;

    public FileProcessor(Func<Stream, IEnumerable<T>> func, IScheduler scheduler, ZafiroPath path,
        ZafiroFileSystem zafiroFileSystem, Func<IZafiroFile, bool> fileFilter)
    {
        this.func = func;
        this.scheduler = scheduler;
        this.path = path;
        this.zafiroFileSystem = zafiroFileSystem;
        this.fileFilter = fileFilter;
    }

    public Task<Result<IEnumerable<T>>> Execute()
    {
        return zafiroFileSystem
            .GetDirectory(path)
            .Map(SelectMany)
            .Map(async observable => await observable);
    }

    private IObservable<IEnumerable<T>> SelectMany(IZafiroDirectory directory)
    {
        return Files(directory)
            .Where(fileFilter)
            .SelectMany(ProcessFile);
    }

    private IObservable<IZafiroFile> Files(IZafiroDirectory directory)
    {
        return MoreEnumerable
            .TraverseBreadthFirst(directory, d => d.Directories)
            .ToObservable()
            .SelectMany(d => Observable.Start(() => d.Files, scheduler))
            .SelectMany(s => s);
    }

    private IObservable<IEnumerable<T>> ProcessFile(IZafiroFile file)
    {
        return Observable.Using(file.OpenRead, s => Observable.Start(() => func(s), scheduler));
    }
}