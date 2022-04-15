namespace Avalonia.Diagnostics.ResourceTools.Core.Dynamic;

public interface IResourceInventory
{
    IEnumerable<ResourceNode> Get(object target);
}