using System.Xml;
using FileSystem;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public class ResourceUsage
{
    public ZafiroPath Path { get; }

    public ResourceUsage(XmlNode xmlNode, ZafiroPath path)
    {
        Path = path;
        Name = xmlNode.Name;
        Value = xmlNode.Value;
    }

    public string? Value { get; }

    public string Name { get;  }
}