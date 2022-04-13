using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
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

        Load = ReactiveCommand.CreateFromTask(() =>
        {
            var dir = fs.GetDirectory("E:\\Repos\\SuperJMN\\WalletWasabi");
            var observable = dir.Match(
                d =>
                {
                    var list = analizer.GetAllResources(d).ToList();
                    return list
                        .Select(cr => cr.Select(x => new ColdResourceViewModel(x)));
                },
                s => Observable.Empty<List<ColdResourceViewModel>>());
            return observable.ToTask();
        });

        SourceCache<ColdResourceViewModel, string> sourceCache = new(x => x.Key);
        sourceCache.Connect()
            .Bind(out resources)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe();

        Load.Subscribe(list => sourceCache.Edit(x => x.Load(list)));
    }

    public ReadOnlyObservableCollection<ColdResourceViewModel> Resources => resources;

    public ReactiveCommand<Unit, IEnumerable<ColdResourceViewModel>> Load { get; }
}