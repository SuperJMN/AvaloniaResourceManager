using CSharpFunctionalExtensions;

namespace Avalonia.Diagnostics.ResourceTools.Core;

public class CompositeKey : ValueObject
{
    private readonly object[] components;

    public CompositeKey(params object[] components)
    {
        this.components = components;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return components;
    }
}