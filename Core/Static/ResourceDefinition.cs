using System.Reactive.Linq;
using FileSystem;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public class ResourceDefinition
{
    public string Key { get; }
    public string Name { get; }
    public string OriginalXaml { get; }
    public ZafiroPath Path { get; }
    public ResourceRoot ResourceRoot { get; }

    public ResourceDefinition(string key, string name, string originalXaml, ZafiroPath path, ResourceRoot resourceRoot)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        OriginalXaml = originalXaml ?? throw new ArgumentNullException(nameof(originalXaml));
        Path = path;
        ResourceRoot = resourceRoot;
    }

    public override string ToString()
    {
        return $"{nameof(Key)}: {Key}, {nameof(Name)}: {Name}";
    }

    public object Create() => ResourceRoot.XamlLoader.Load(OriginalXaml);

    public IObservable<ResourceUsage> FindUsages()
    {
        return ResourceRoot.GetUsages(Key);
    }
}