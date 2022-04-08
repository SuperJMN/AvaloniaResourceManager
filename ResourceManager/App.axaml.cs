using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Diagnostics.ResourceTools.Core.Avalonia;
using Avalonia.Diagnostics.ResourceTools.ViewModels;
using Avalonia.Diagnostics.ResourceTools.Views;
using Avalonia.Markup.Xaml;

namespace Avalonia.Diagnostics.ResourceTools;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(new ResourceInventory(new ResourceExtractor(), new VisualBranchProvider())),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}