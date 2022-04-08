namespace Avalonia.Diagnostics.ResourceTools.Core;

public interface IResourceInventory
{
    IEnumerable<ResourceNode> Get(object target);
}