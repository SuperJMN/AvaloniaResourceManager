using System.Xml;
using Avalonia.Markup.Xaml;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static.Xaml;

public class XamlLoader : IXamlLoader
{
    public object Load(string xaml)
    {
        var compXaml = CreateCompatibleXaml(xaml);
        var result = AvaloniaRuntimeXamlLoader.Load(compXaml);
        return ((Container)result).Content;
    }

    private string CreateCompatibleXaml(string xaml)
    {
        var originalNode = CreateOriginalNode(xaml);
        RemoveKey(originalNode);
        var wrappingNode = CrateWrappingNode(originalNode);
        CopyXmlNsDefinitionsToRoot(wrappingNode);

        return wrappingNode.OuterXml;
    }

    private static void RemoveKey(XmlNode originalNode)
    {
        originalNode?.Attributes?.RemoveNamedItem("Key", "http://schemas.microsoft.com/winfx/2006/xaml");
    }

    private static void CopyXmlNsDefinitionsToRoot(XmlElement wrappingNode)
    {
        var definitions = GetAllNamespaceDefinitions(wrappingNode).ToList();
        foreach (var xmlAttribute in definitions)
        {
            wrappingNode.Attributes.Append(xmlAttribute);
        }
    }

    private static IEnumerable<XmlAttribute> GetAllNamespaceDefinitions(XmlElement wrappingNode)
    {
        var children = MoreLinq.MoreEnumerable.TraverseBreadthFirst(wrappingNode.FirstChild,
            x => x?.ChildNodes.Cast<XmlNode>() ?? new List<XmlNode>());
        var allAttrs = children.SelectMany(x => x?.Attributes?.Cast<XmlAttribute>() ?? new List<XmlAttribute>());
        return allAttrs.Where(x => x.Name.Contains("xmlns"));
    }

    private XmlElement CrateWrappingNode(XmlElement originalNode)
    {
        var doc = new XmlDocument();
        var rootNode = CreateRootNode(doc);
        doc.AppendChild(rootNode);
        var imported = doc.ImportNode(originalNode, true);
        rootNode.AppendChild(imported);
        return rootNode;
    }

    private static XmlElement CreateOriginalNode(string xaml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xaml);
        return doc.DocumentElement!;
    }

    private XmlElement CreateRootNode(XmlDocument xmlDocument)
    {
        return xmlDocument.CreateElement("local", nameof(Container), "using:" + typeof(Container).Namespace);
    }
}