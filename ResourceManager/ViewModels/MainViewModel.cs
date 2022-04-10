using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;

namespace Avalonia.Diagnostics.ResourceTools.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ReadOnlyObservableCollection<ColdResource> resources;

    public MainViewModel(ResourceAnalizer analizer)
    {
        Load = ReactiveCommand.CreateFromTask(() =>
            analizer.GetAllResources(Scheduler.Default, "E:\\Repos\\SuperJMN\\WalletWasabi"));

        SourceCache<ColdResource, string> sourceCache = new(x => x.Key);
        sourceCache.Connect()
            .Bind(out resources)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe();

        Load.Subscribe(r => r.OnSuccessTry(list => sourceCache.Edit(x => x.Load(list))));
    }

    public ReadOnlyObservableCollection<ColdResource> Resources => resources;

    public ReactiveCommand<Unit, Result<IList<ColdResource>>> Load { get; }
}