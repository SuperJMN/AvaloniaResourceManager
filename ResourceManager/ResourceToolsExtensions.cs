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
        var window = new RuntimeAnalysisWindow();
        var viewModel = new RuntimeAnalysisViewModel(new ResourceInventory(new ResourceExtractor(), new VisualBranchProvider()));
        window.DataContext = viewModel;
        window.Show();

        topLevel.AddHandler(InputElement.PointerPressedEvent, (sender, args) =>
        {
            viewModel.SelectedItem = args.Source;
        }, RoutingStrategies.Tunnel);
    }
}