using System;
using System.IO;
using System.Reactive;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using Avalonia.Markup.Xaml;
using Portable.Xaml;
using ReactiveUI;

namespace Avalonia.Diagnostics.ResourceTools.ViewModels;

public class ColdResourceViewModel : ViewModelBase
{
    private readonly ObservableAsPropertyHelper<object> preview;

    public ColdResourceViewModel(ColdResource resource)
    {
        Key = resource.Key;
        Name = resource.Name;
        Load = ReactiveCommand.Create(() =>
        {
            return new object();
        });

        preview = Load.ToProperty(this, x => x.Preview);

        Load
            .Execute()
            .Subscribe(o => { });
    }

    public object Preview => preview.Value;

    public ReactiveCommand<Unit, object> Load { get; }

    public string Name { get; set; }

    public string Key { get; set; }
}