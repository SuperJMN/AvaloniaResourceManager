using Avalonia.Controls;
using Avalonia.Styling;

namespace Avalonia.Diagnostics.ResourceTools.Core.Avalonia;

public class ResourceExtractor : IResourceExtractor
{
    public IEnumerable<KeyValuePair<object, object?>> Extract(object b)
    {
        return b switch
        {
            Window w => Extract(w),
            Application app => Extract(app),
            StyledElement v => Extract(v),
            _ => throw new InvalidOperationException()
        };
    }

    private IEnumerable<KeyValuePair<object, object?>> Extract(StyledElement visual)
    {
        var resourcesFromAllStyles = GetStyles(visual.Styles)
            .OfType<Style>()
            .SelectMany(style => style.Resources);

        return visual.Resources.Concat(visual.Styles.Resources).Concat(resourcesFromAllStyles);
    }

    private IEnumerable<KeyValuePair<object, object?>> Extract(Application application)
    {
        var resourcesFromAllStyles = GetStyles(application.Styles)
            .OfType<Style>()
            .SelectMany(style => style.Resources);

        return application.Resources.Concat(resourcesFromAllStyles);
    }

    private IEnumerable<IStyle> GetStyles(IStyle style)
    {
        return MoreLinq.MoreEnumerable.TraverseBreadthFirst(style, x => x.Children);
    }

    private IEnumerable<KeyValuePair<object, object?>> Extract(Window window)
    {
        var resourcesFromAllStyles = GetStyles(window.Styles)
            .OfType<Style>()
            .SelectMany(style => style.Resources);

        return window.Resources.Concat(window.Styles.Resources).Concat(resourcesFromAllStyles);
    }
}