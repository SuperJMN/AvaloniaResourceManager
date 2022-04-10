using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Xml;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using Xunit;
using CSharpFunctionalExtensions;
using FileSystem;
using Serilog;
using FileSystem = System.IO.Abstractions.FileSystem;

namespace Tests;

public class ResourceFinderTests
{
    [Fact]
    public async Task Extract()
    {
        var resources = await new ResourceAnalizer().GetAllResources(Scheduler.CurrentThread, "E:\\Repos\\SuperJMN\\WalletWasabi");
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
}