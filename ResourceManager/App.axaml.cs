using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Core;
using ResourceManager.ViewModels;
using ResourceManager.Views;

namespace ResourceManager
{
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
}
