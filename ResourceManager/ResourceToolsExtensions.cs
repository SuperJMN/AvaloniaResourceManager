using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Core;
using ResourceManager.ViewModels;
using ResourceManager.Views;

namespace ResourceManager;

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