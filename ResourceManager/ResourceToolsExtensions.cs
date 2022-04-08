using Avalonia.Controls;
using Avalonia.Diagnostics.ResourceTools.Core.Avalonia;
using Avalonia.Diagnostics.ResourceTools.ViewModels;
using Avalonia.Diagnostics.ResourceTools.Views;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Diagnostics.ResourceTools;

public static class ResourceToolsExtensions
{
    public static void AttachResourceTools(this TopLevel topLevel)
    {
        var window = new MainWindow();
        window.Show();
        var mainWindowViewModel = new MainWindowViewModel(new ResourceInventory(new ResourceExtractor(), new VisualBranchProvider()));
        window.DataContext = mainWindowViewModel;

        topLevel.AddHandler(InputElement.PointerPressedEvent, (sender, args) =>
        {
            mainWindowViewModel.SelectedItem = args.Source;
        }, RoutingStrategies.Tunnel);
    }
}