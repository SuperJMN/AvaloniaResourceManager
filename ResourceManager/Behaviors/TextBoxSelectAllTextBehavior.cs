using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Diagnostics.ResourceTools.Behaviors;

internal class TextBoxSelectAllTextBehavior : Avalonia.Xaml.Interactivity.Behavior<TextBox>
{
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        AssociatedObject?.SelectAll();
        AssociatedObject.GotFocus += (sender, args) => AssociatedObject.SelectAll();
    }
}