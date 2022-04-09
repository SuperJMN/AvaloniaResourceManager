using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using Xunit;
using CSharpFunctionalExtensions;
using FileSystem;
using MoreLinq;
using Serilog;

namespace Tests;

public class ResourceFinderTests
{
    [Fact]
    public async Task Extract()
    {
        var resources = await GetAllResources(Scheduler.CurrentThread);
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