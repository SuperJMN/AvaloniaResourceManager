using System.Xml;
using FileSystem;
using MoreLinq;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public class ResourceUsage
{
    public ZafiroPath Path { get; }

    public ResourceUsage(XmlNode xmlNode, ZafiroPath path)
    {
        Path = path;
        Key = GetKey(xmlNode);
        Value = xmlNode.Value;
    }

    private string GetKey(XmlNode xmlNode)
    {
        if (xmlNode is XmlAttribute attr)
        {
            return new string(attr.Value.SkipUntil(c => c == ' ').TakeWhile(c => c != '}').ToArray());
        }

        if (xmlNode is XmlElement element)
        {
            var value = element.Attributes["ResourceKey"]?.Value;
            if (value is null)
            {
                throw new InvalidOperationException("ResourceKey cannot be null");
            }
            return value;
        }

        throw new InvalidOperationException("Invalid node type");
    }

    public string? Value { get; }

    public string Key { get;  }
}