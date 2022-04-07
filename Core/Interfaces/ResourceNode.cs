namespace Core.Interfaces
{
    public class ResourceNode
    {
        public ResourceNode(object parent, IEnumerable<KeyValuePair<object, object?>> resources)
        {
            Parent = parent;
            Resources = resources;
        }

        public object Parent { get; }
        public IEnumerable<KeyValuePair<object, object?>> Resources { get;  }
    }
}