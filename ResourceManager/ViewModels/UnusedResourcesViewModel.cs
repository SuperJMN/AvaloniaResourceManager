using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;

namespace Avalonia.Diagnostics.ResourceTools.ViewModels;

public class UnusedResourcesViewModel : ViewModelBase, ITitled
{
    private readonly ReadOnlyObservableCollection<ResourceDefinitionViewModel> resources;

    private ResourceDefinitionViewModel selectedItem;
    private readonly ProjectWatcher projectWatcher;

    public UnusedResourcesViewModel(ProjectWatcher projectWatcher)
    {
        Load = ReactiveCommand.CreateFromObservable(() => OnLoad());

        SourceCache<ResourceDefinitionViewModel, string> sourceCache = new(x => x.Key);
        sourceCache.Connect()
            .Bind(out resources)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe();

        Load.Subscribe(list =>
        {
            sourceCache.Edit(r => r.Load(list));
        });

        this.projectWatcher = projectWatcher;
    }

    private IObservable<IList<ResourceDefinitionViewModel>> OnLoad()
    {
        return projectWatcher.Resources.Select(list => list
            //.Select(rd => new { Usages = rd.FindUsages().ToEnumerable().ToList(), Resource = rd })
            //.Where(ar => !ar.Usages.Any())
            //.Select(r => r.Resource)
            .Select(rd => new ResourceDefinitionViewModel(rd)).ToList());
    }

    public ReactiveCommand<Unit, IList<ResourceDefinitionViewModel>> Load { get; set; }

    public ReadOnlyObservableCollection<ResourceDefinitionViewModel> Resources => resources;

    public ResourceDefinitionViewModel SelectedItem
    {
        get => selectedItem;
        set => this.RaiseAndSetIfChanged(ref selectedItem, value);
    }

    public string Title => "Resources View";
}