using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using FileSystem;
using ReactiveUI;

namespace Avalonia.Diagnostics.ResourceTools.ViewModels;

public class ResourceDefinitionViewModel : ViewModelBase
{
    private readonly ObservableAsPropertyHelper<object?> preview;
    private readonly ObservableAsPropertyHelper<IList<ResourceUsage>> usages;

    public ResourceDefinitionViewModel(ResourceDefinition resourceDefinition)
    {
        Key = resourceDefinition.Key;
        Type = resourceDefinition.Name;
        Load = ReactiveCommand.CreateFromObservable(() => OnLoad(resourceDefinition));
        Load.ThrownExceptions.Subscribe(exception => { });
        Path = resourceDefinition.Path;
        preview = Load.ToProperty(this, x => x.Preview);
        FindUsages = ReactiveCommand.CreateFromObservable(() => resourceDefinition.FindUsages().ToList());
        usages = FindUsages.ToProperty(this, model => model.Usages);
        Path = resourceDefinition.Path;
    }

    public IList<ResourceUsage> Usages => usages.Value;

    public ReactiveCommand<Unit, IList<ResourceUsage>> FindUsages { get; }

    public ZafiroPath Path { get; }

    public object? Preview => preview.Value;

    public ReactiveCommand<Unit, object?> Load { get; }

    public string Type { get; }

    public string Key { get; set; }

    private static IObservable<object?> OnLoad(ResourceDefinition resourceDefinition)
    {
        return Observable
            .Start(resourceDefinition.Create, RxApp.MainThreadScheduler)
            .Catch((Exception _) => Observable.Return<object?>(default));
    }
}