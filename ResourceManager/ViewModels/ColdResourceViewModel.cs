using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using Avalonia.Diagnostics.ResourceTools.Xaml;
using FileSystem;
using ReactiveUI;

namespace Avalonia.Diagnostics.ResourceTools.ViewModels;

public class ColdResourceViewModel : ViewModelBase
{
    private static readonly XamlLoader XamlLoader = new();
    private readonly ObservableAsPropertyHelper<object> preview;
    private readonly ObservableAsPropertyHelper<IList<ResourceUsage>> usages;

    public ColdResourceViewModel(ColdResource resource, IZafiroDirectory root)
    {
        Key = resource.Key;
        Name = resource.Name;
        Load = ReactiveCommand.Create(() => XamlLoader.Load(resource.Xaml));
        Path = resource.Path;
        preview = Load.ToProperty(this, x => x.Preview);
        FindUsages = ReactiveCommand.CreateFromObservable(() => new ResourceAnalizer().GetUsages(root, resource.Key).ToList());
        usages = FindUsages.ToProperty(this, model => model.Usages);
    }

    public IList<ResourceUsage> Usages => usages.Value;

    public ReactiveCommand<Unit, IList<ResourceUsage>> FindUsages { get; set; }

    public ZafiroPath Path { get; }

    public object Preview => preview.Value;

    public ReactiveCommand<Unit, object> Load { get; }

    public string Name { get; }

    public string Key { get; set; }
}