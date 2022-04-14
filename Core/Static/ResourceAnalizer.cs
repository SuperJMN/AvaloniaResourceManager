using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Xml;
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

    public IObservable<ResourceUsage> GetUsages(IZafiroDirectory directory)
    {
        return directory
            .ForEachFileSelect((stream, path) => FindUsages(stream, path).ToObservable());
    }

    private IEnumerable<ResourceUsage> FindUsages(Stream stream, ZafiroPath zafiroPath)
    {
        var doc = new XmlDocument();
        doc.Load(stream);
        var allNodes = GetAllNodes(doc).ToList();
        var fromNodes = allNodes
            .Where(node => node.Name.StartsWith("DynamicResource") || node.Name.StartsWith("StaticResource"))
            .Select(b => new ResourceUsage(b, zafiroPath));

        var fromAttributes = from n in allNodes
            from attr in n.Attributes?.Cast<XmlAttribute>() ?? new List<XmlAttribute>()
            where attr.Value.StartsWith("{StaticResource") || attr.Value.StartsWith("{DynamicResource")
            select new ResourceUsage(attr, zafiroPath);

        return fromNodes.Concat(fromAttributes);
    }

    private IEnumerable<XmlNode> GetAllNodes(XmlNode node)
    {
        var nodes = MoreLinq.MoreEnumerable
            .TraverseBreadthFirst(node, xmlNode => xmlNode.ChildNodes.Cast<XmlNode>())
            .ToList();

        return nodes;
    }
}