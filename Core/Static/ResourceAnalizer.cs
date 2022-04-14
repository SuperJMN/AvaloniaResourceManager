using System.Reactive.Linq;
using System.Xml;
using FileSystem;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public class ResourceAnalizer
{
    public IObservable<ColdResource> GetResources(IZafiroDirectory directory)
    {
        return directory
            .ForEachFileSelect((stream, path) => GetResources(stream, path).ToObservable());
    }

    public IObservable<ResourceUsage> GetUsages(IZafiroDirectory directory)
    {
        return directory
            .ForEachFileSelect((stream, path) => FindUsages(stream, path).ToObservable());
    }

    private IEnumerable<ColdResource> GetResources(Stream stream, ZafiroPath zafiroPath)
    {
        var doc = new XmlDocument();
        doc.Load(stream);
        var allNodes = GetAllNodes(doc).ToList();

        var resources = from n in allNodes
            let key = n.Attributes?["Key", "http://schemas.microsoft.com/winfx/2006/xaml"]
            where key is not null
            select new ColdResource(key.Value, n.Name, n.OuterXml, zafiroPath);

        return resources;
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

    public IObservable<ResourceUsage> GetUsages(IZafiroDirectory directory, string key)
    {
        return GetUsages(directory).Where(r => r.Key == key);
    }
}