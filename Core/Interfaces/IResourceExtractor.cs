namespace Core.Interfaces
{
    public interface IResourceExtractor
    {
        IEnumerable<KeyValuePair<object, object?>> Extract(object b);
    }
}