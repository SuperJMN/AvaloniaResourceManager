using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private readonly ReadOnlyObservableCollection<ColdResourceViewModel> resources;
    

    public MainViewModel(ResourceAnalizer analizer)
    {
        var fs = new ZafiroFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None);

        Load = ReactiveCommand.CreateFromObservable(() => From(analizer, fs));
        
        SourceCache<ColdResourceViewModel, string> sourceCache = new(x => x.Key);
        sourceCache.Connect()
            .Bind(out resources)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe();

        Load.Subscribe(list =>
        {
            sourceCache.Edit(r => r.Load(list));
        });
    }

    public ReactiveCommand<Unit, IList<ColdResourceViewModel>> Load { get; set; }

    private IObservable<IList<ColdResourceViewModel>> From(ResourceAnalizer analizer, ZafiroFileSystem fs)
    {
        var dir = fs.GetDirectory("E:\\Repos\\SuperJMN\\WalletWasabi");
        return analizer
            .GetResources(dir.Value)
            .Select(resource => ToViewModel(resource, dir.Value))
            .ToList();
    }

    private static ColdResourceViewModel ToViewModel(ColdResource resource, IZafiroDirectory directory)
    {
        return new ColdResourceViewModel(resource, directory);
    }

    public ReadOnlyObservableCollection<ColdResourceViewModel> Resources => resources;
}