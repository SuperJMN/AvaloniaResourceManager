using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using CSharpFunctionalExtensions;
using DynamicData;
using FileSystem;
using ReactiveUI;
using Serilog;

namespace Avalonia.Diagnostics.ResourceTools.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ReadOnlyObservableCollection<ResourceDefinitionViewModel> resources;
    private string rootFolder;
    private readonly IZafiroFileSystem fileSystem = new ZafiroFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None);
    private ResourceDefinitionViewModel selectedItem;

    public MainViewModel()
    {
        RootFolder = "E:/Repos/SuperJMN/WalletWasabi";
        Load = ReactiveCommand.CreateFromObservable(OnLoad);
        
        SourceCache<ResourceDefinitionViewModel, string> sourceCache = new(x => x.Key);
        sourceCache.Connect()
            .Bind(out resources)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe();

        Load.Subscribe(list =>
        {
            sourceCache.Edit(r => r.Load(list));
        });
    }

    private IObservable<IList<ResourceDefinitionViewModel>> OnLoad()
    {
        var rootFolderChanged = this.WhenAnyValue(r => r.RootFolder);
        var resourcesObs = rootFolderChanged.SelectMany(s => new ResourceRoot(fileSystem.GetDirectory(s).Value).GetResources().ToList());
        var loadMethod = resourcesObs.Select(list => list.Select(ToViewModel).ToList());
        return loadMethod;
    }

    public string RootFolder
    {
        get => rootFolder;
        set => this.RaiseAndSetIfChanged(ref rootFolder, value);
    }

    public ReactiveCommand<Unit, IList<ResourceDefinitionViewModel>> Load { get; set; }

    private static ResourceDefinitionViewModel ToViewModel(ResourceDefinition resourceDefinition)
    {
        return new ResourceDefinitionViewModel(resourceDefinition);
    }

    public ReadOnlyObservableCollection<ResourceDefinitionViewModel> Resources => resources;

    public ResourceDefinitionViewModel SelectedItem
    {
        get => selectedItem;
        set => this.RaiseAndSetIfChanged(ref selectedItem, value);
    }
}