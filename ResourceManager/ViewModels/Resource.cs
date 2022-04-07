namespace ResourceManager.ViewModels;

public class Resource
{
    public object Owner { get; }
    public object Key { get; }
    public object Value { get; }

    public Resource(object owner, object key, object value)
    {
        Owner = owner;
        Key = key;
        Value = value;
    }

    public CompositeKey ResourceKey => new(Owner, Key, Value);
}