using FileSystem;

namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public class ColdResource
{
    public string Key { get; }
    public string Name { get; }
    public string Xaml { get; }
    public ZafiroPath Path { get; }

    public ColdResource(string key, string name, string xaml, ZafiroPath path)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Xaml = xaml ?? throw new ArgumentNullException(nameof(xaml));
    }

    public override string ToString()
    {
        return $"{nameof(Key)}: {Key}, {nameof(Name)}: {Name}";
    }
}