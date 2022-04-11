using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using CSharpFunctionalExtensions;
using DynamicData;
using Portable.Xaml;
using ReactiveUI;

namespace Avalonia.Diagnostics.ResourceTools.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ReadOnlyObservableCollection<ColdResourceViewModel> resources;

    public MainViewModel(ResourceAnalizer analizer)
    {
        Load = ReactiveCommand.CreateFromTask(async () =>
        {
            var allResources = await analizer.GetAllResources(Scheduler.Default, "E:\\Repos\\SuperJMN\\WalletWasabi");
            return allResources.Map(r =>
            {
                return r.Select(cr => new ColdResourceViewModel(cr)).ToList();
            });
        });

        SourceCache<ColdResourceViewModel, string> sourceCache = new(x => x.Key);
        sourceCache.Connect()
            .Bind(out resources)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe();

        Load.Subscribe(r => r.OnSuccessTry(list => sourceCache.Edit(x => x.Load(list))));
    }

    public ReadOnlyObservableCollection<ColdResourceViewModel> Resources => resources;

    public ReactiveCommand<Unit, Result<List<ColdResourceViewModel>>> Load { get; }
}