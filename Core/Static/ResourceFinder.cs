using System.Xml;
using Avalonia.Diagnostics.ResourceTools.Core.Dynamic;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public class ResourceFinder
{
    public IEnumerable<Resource> FindAll(XmlNode node)
    {
        var nodes = FindNodesWithResources(node);

        return ArraySegment<Resource>.Empty;
    }

    private IEnumerable<XmlNode> FindNodesWithResources(XmlNode node);
}