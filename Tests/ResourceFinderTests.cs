using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using Xunit;
using CSharpFunctionalExtensions;
using FileSystem;
using Serilog;

namespace Tests;

public class ResourceFinderTests
{
    [Fact]
    public async Task Extract()
    {
        var fs = new ZafiroFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None);
        var dir = fs.GetDirectory("E:\\Repos\\SuperJMN\\WalletWasabi")
            .Map(dir => new ResourceAnalizer().GetResources(dir));

        var result = await dir.Value.ToList();
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

    [Fact]
    public async Task Extract3()
    {
        var fs = new ZafiroFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None);
        var dir = fs.GetDirectory("E:\\Repos\\SuperJMN\\WalletWasabi");

        var result = dir
            .Map(d => d
                .Files().ToObservable()
                .Where(file => file.Path.ToString().EndsWith(".axaml"))
                .Select(stream => Observable.Start(() =>
                {
                    var resFinder = new ResourceFinder();
                    return resFinder.FindAll(stream).ToObservable();
                }, DefaultScheduler.Instance))
                .Merge());

        var allResources = await result.Match(observable => observable.ToList().ToTask(),
            s => Task.FromResult((IList<ColdResource>)new List<ColdResource>()));
    }
}