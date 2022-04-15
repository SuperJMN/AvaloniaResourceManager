using System.Reactive.Linq;
using System.Xml;
using System.Xml.Linq;
using Avalonia.Diagnostics.ResourceTools.Core.Static.Xaml;
using FileSystem;
using MoreLinq;
using MoreLinq.Extensions;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public class ResourceRoot
{
    private readonly IZafiroDirectory directory;

    public ResourceRoot(IZafiroDirectory directory)
    {
        this.directory = directory;
        XamlLoader = new XamlLoader();
    }

    public IXamlLoader XamlLoader { get; }

    public IObservable<ResourceDefinition> GetResources()
    {
        return directory
            .ForEachFileSelect((stream, path) => GetResources(stream, path, this).ToObservable());
    }

    public IObservable<ResourceUsage> GetUsages()
    {
        return directory.ForEachFileSelect((stream, path) => GetUsages(stream, path).ToObservable());
    }

    public IObservable<ResourceUsage> GetUsages(string key)
    {
        return GetUsages().Where(r => r.Name == key);
    }

    private IEnumerable<ResourceDefinition> GetResources(Stream stream, ZafiroPath zafiroPath,
        ResourceRoot resourceRoot)
    {
        var doc = new XmlDocument();
        doc.Load(stream);
        var allNodes = GetAllNodes(doc).ToList();

        var resources = from n in allNodes
            let key = n.Attributes?["Key", "http://schemas.microsoft.com/winfx/2006/xaml"]
            where key is not null
            select new ResourceDefinition(key.Value, n.Name, n.OuterXml, zafiroPath, resourceRoot);

        return resources;
    }


    private IList<ResourceUsage> GetUsages(Stream stream, ZafiroPath zafiroPath)
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
        return new string(SkipUntilExtension.SkipUntil(value, c => c == ' ').TakeWhile(c => c != '}').ToArray());
    }

    private IEnumerable<XmlNode> GetAllNodes(XmlNode node)
    {
        var nodes = MoreEnumerable
            .TraverseBreadthFirst(node, xmlNode => xmlNode.ChildNodes.Cast<XmlNode>())
            .ToList();

        return nodes;
    }
}