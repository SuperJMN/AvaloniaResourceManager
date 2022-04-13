using System.Reactive.Concurrency;
using System.Reactive.Linq;
using FileSystem;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public class ResourceAnalizer
{
    public IObservable<ColdResource> GetAllResources(IZafiroDirectory directory)
    {
        var result = directory
                .Files().ToObservable()
                .Where(file => file.Path.ToString().EndsWith(".axaml"))
                .Select(stream => Observable.Start(() =>
                {
                    var resFinder = new ResourceFinder();
                    return resFinder.FindAll(stream).ToObservable();
                }, TaskPoolScheduler.Default))
                .Merge();

        return result;
    }
}