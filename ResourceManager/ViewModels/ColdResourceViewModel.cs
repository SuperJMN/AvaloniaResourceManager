using System;
using System.Reactive;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using Avalonia.Diagnostics.ResourceTools.Xaml;
using ReactiveUI;

namespace Avalonia.Diagnostics.ResourceTools.ViewModels;

public class ColdResourceViewModel : ViewModelBase
{
    private readonly ObservableAsPropertyHelper<object> preview;
    private static XamlLoader xamlLoader = new();

    public ColdResourceViewModel(ColdResource resource)
    {
        Key = resource.Key;
        Name = resource.Name;
        Load = ReactiveCommand.Create(() => xamlLoader.Load(resource.Xaml));

        preview = Load.ToProperty(this, x => x.Preview);
    }

    public object Preview => preview.Value;

    public ReactiveCommand<Unit, object> Load { get; }

    public string Name { get; set; }

    public string Key { get; set; }
}