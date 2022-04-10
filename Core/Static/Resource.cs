namespace Tests;

public class Resource
{
    public string Name { get; }
    public string Key { get; }
    public string Xaml { get; }

    public Resource(string name, string key, string xaml)
    {
        Name = name;
        Key = key;
        Xaml = xaml;
    }
}