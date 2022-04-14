using System.Reactive.Linq;
using System.Xml;
using System.Xml.Linq;
using FileSystem;
using MoreLinq.Extensions;

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
        return directory.ForEachFileSelect((stream, path) => FindUsages(stream, path).ToObservable());
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


    private IList<ResourceUsage> FindUsages(Stream stream, ZafiroPath zafiroPath)
    {
        var doc = XDocument.Load(stream, LoadOptions.SetLineInfo);
        var fromAttributes = from element in doc.Descendants()
            from attribute in element.Attributes()
            where attribute.Value.StartsWith("{StaticResource") ||
                  attribute.Value.StartsWith("{DynamicResource")
            select new ResourceUsage(GetKey(attribute.Value), zafiroPath, element);

        var fromElements = from element in doc.Descendants()
            where element.Name.LocalName.StartsWith("StaticResource") ||
                  element.Name.LocalName.StartsWith("DynamicResource")
            let resourceKey = element.Attributes().FirstOrDefault(a => a.Name == "ResourceKey")?.Value
            select new ResourceUsage(resourceKey, zafiroPath, element);

        return fromElements.Concat(fromAttributes).ToList();
    }

    private string GetKey(string value)
    {
        return new string(value.SkipUntil(c => c == ' ').TakeWhile(c => c != '}').ToArray());
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
        return GetUsages(directory).Where(r => r.Name == key);
    }
}