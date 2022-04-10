using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FileSystem;
using MoreLinq;

namespace Tests;

public class FileProcessor<T>
{
    private readonly Func<Stream, IEnumerable<T>> func;
    private readonly IScheduler taskPoolScheduler;
    private readonly ZafiroPath path;
    private readonly ZafiroFileSystem zafiroFileSystem;
    private readonly Func<IZafiroFile, bool> fileFilter;

    public FileProcessor(Func<Stream, IEnumerable<T>> func, IScheduler taskPoolScheduler, ZafiroPath path, ZafiroFileSystem zafiroFileSystem, Func<IZafiroFile, bool> fileFilter)
    {
        this.func = func;
        this.taskPoolScheduler = taskPoolScheduler;
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

    private IObservable<IEnumerable<T>> SelectMany(IZafiroDirectory d)
    {
        return GetAllFiles(d)
            .ToObservable()
            .Where(fileFilter)
            .SelectMany(file => ProcessFile(file, taskPoolScheduler));
    }

    private IEnumerable<IZafiroFile> GetAllFiles(IZafiroDirectory directory)
    {
        return MoreEnumerable
            .TraverseBreadthFirst(directory, d => d.Directories)
            .SelectMany(d => d.Files);
    }

    private IObservable<IEnumerable<T>> ProcessFile(IZafiroFile file, IScheduler? scheduler = default)
    {
        return Observable
            .Using(
                file.OpenRead,
                s => Observable.Return(func(s), scheduler ?? Scheduler.Default));
    }
}