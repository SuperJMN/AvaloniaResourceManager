using System.Xml;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public class ResourceFinder
{
    public IEnumerable<ColdResource> FindAll(Stream stream)
    {
        var xmlDocument = new XmlDocument();
        xmlDocument.Load(stream);
        return GetAllNodes(xmlDocument)
            .SelectMany(GetResourcesFromNode);
    }

    private IEnumerable<ColdResource> GetResourcesFromNode(XmlNode nodes)
    {
        var query = from node in nodes.ChildNodes.Cast<XmlNode>()
            let key = node.Attributes?["Key", "http://schemas.microsoft.com/winfx/2006/xaml"]
            where key != null
            select new ColdResource(key.Value, node.Name, node.OuterXml);

        return query;
    }

    private IEnumerable<XmlNode> GetAllNodes(XmlNode node)
    {
        var nodes = MoreLinq.MoreEnumerable
            .TraverseBreadthFirst(node, xmlNode => xmlNode.ChildNodes.Cast<XmlNode>())
            .ToList();

        return nodes;
    }
}