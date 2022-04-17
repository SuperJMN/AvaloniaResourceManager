using System;
using System.Reactive.Subjects;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Diagnostics.ResourceTools.Core.Avalonia;
using Avalonia.Diagnostics.ResourceTools.ViewModels;
using Avalonia.Diagnostics.ResourceTools.Views;
using Avalonia.Markup.Xaml;
using CSharpFunctionalExtensions;
using FileSystem;
using Serilog;

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
            var subject = new ReplaySubject<string>();
            var projectWatcher = new ProjectWatcher(subject);
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(subject, new ITitled[] { new ResourcesViewModel(projectWatcher), new UnusedResourcesViewModel(projectWatcher) }),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}