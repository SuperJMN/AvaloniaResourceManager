using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using Xunit;
using CSharpFunctionalExtensions;
using FileSystem;
using MoreLinq;
using Serilog;
using FileSystem = System.IO.Abstractions.FileSystem;

namespace Tests;

public class ResourceFinderTests
{
    [Fact]
    public async Task Extract()
    {
        var resources = await GetAllResources(Scheduler.CurrentThread);
    }

    [Fact]
    public async Task Extract2()
    {
        var fileProcessor = new FileProcessor<ColdResource>(stream =>
            {
                var resFinder = new ResourceFinder();
                return resFinder.FindAll(stream);
            }, DefaultScheduler.Instance, "E:\\Repos\\SuperJMN\\WalletWasabi",
            new ZafiroFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None),
            file => file.Path.ToString().EndsWith(".axaml"));
        var resources = await fileProcessor.Execute();
    }

    private static Task<Result<IList<ColdResource>>> GetAllResources(IScheduler taskPoolScheduler)
    {
        var sut = new ResourceFinder();

        var fs = new ZafiroFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None);

        return fs
            .GetDirectory("E:\\Repos\\SuperJMN\\WalletWasabi")
            .Map(d =>
            {
                var fileList = GetAllFiles(d);
                return fileList.ToObservable()
                    .Where(s => s.Path.Path.EndsWith(".axaml"))
                    .SelectMany(file => ProcessFile(file, sut, taskPoolScheduler));
            })
            .Map(async observable => await observable.Merge().ToList());
    }

    private static IEnumerable<IZafiroFile> GetAllFiles(IZafiroDirectory directory)
    {
        return MoreEnumerable
            .TraverseBreadthFirst(directory, d => d.Directories)
            .SelectMany(d => d.Files);
    }

    private static IObservable<IObservable<ColdResource>> ProcessFile(IZafiroFile file, ResourceFinder sut, IScheduler? scheduler = default)
    {
        return Observable.Using(file.OpenRead,
            s => Observable.Return(sut.FindAll(s).ToObservable(), scheduler ?? Scheduler.Default));
    }
}

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