using System.Reactive.Concurrency;
using System.Reactive.Linq;
using FileSystem;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public class ResourceAnalizer
{
    public IObservable<ColdResource> GetResources(IZafiroDirectory directory)
    {
        var result = directory.Files(TaskPoolScheduler.Default)
                .Where(file => file.Path.ToString().EndsWith(".axaml"))
                .Select(stream => Observable.Start(() =>
                {
                    var resFinder = new ResourceFinder();
                    return  resFinder.FindAll(stream).ToObservable();
                }, TaskPoolScheduler.Default))
                .Merge(4);

        return result;
    }
}