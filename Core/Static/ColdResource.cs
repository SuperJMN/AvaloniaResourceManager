namespace Avalonia.Diagnostics.ResourceTools.Core.Static;

public class ColdResource
{
    public string Key { get; }
    public string Name { get; }
    public string Xaml { get; }

    public ColdResource(string key, string name, string xaml)
    {
        Key = key;
        Name = name;
        Xaml = xaml;
    }

    public override string ToString()
    {
        return $"{nameof(Key)}: {Key}, {nameof(Name)}: {Name}";
    }
}