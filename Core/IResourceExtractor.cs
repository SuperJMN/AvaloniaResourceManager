namespace Avalonia.Diagnostics.ResourceTools.Core;

public interface IResourceExtractor
{
    IEnumerable<KeyValuePair<object, object?>> Extract(object b);
}