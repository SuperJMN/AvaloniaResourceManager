namespace Avalonia.Diagnostics.ResourceTools.Core.Dynamic;

public interface IResourceExtractor
{
    IEnumerable<KeyValuePair<object, object?>> Extract(object b);
}