using System.Xml;
using FileSystem;
using MoreLinq;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public class ResourceUsage
{
    public string Name { get; }
    public ZafiroPath Path { get; }
    public IXmlLineInfo XmlLineInfo { get; }

    public ResourceUsage(string name, ZafiroPath path, IXmlLineInfo xmlLineInfo)
    {
        Name = name;
        Path = path;
        XmlLineInfo = xmlLineInfo;
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
}