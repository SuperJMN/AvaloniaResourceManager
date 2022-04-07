namespace Core.Interfaces
{
    public interface IResourceInventory
    {
        IEnumerable<ResourceNode> Get(object target);
    }
}