namespace Avalonia.Diagnostics.ResourceTools.Core;

public class ResourceNode
{
    public ResourceNode(object parent, List<KeyValuePair<object, object?>> resources)
    {
        Parent = parent;
        Resources = resources;
    }

    public object Parent { get; }
    public List<KeyValuePair<object, object?>> Resources { get;  }
}