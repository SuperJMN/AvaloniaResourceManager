using System.Reactive.Concurrency;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using FileSystem;
using MoreLinq;
using Serilog;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public class ResourceAnalizer
{
    public Task<Result<IList<ColdResource>>> GetAllResources(IScheduler taskPoolScheduler,
        ZafiroPath path)
    {
        var sut = new ResourceFinder();

        var fs = new ZafiroFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None);

        return fs
            .GetDirectory(path)
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