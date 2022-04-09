using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia.Diagnostics.ResourceTools.Core;
using Avalonia.Diagnostics.ResourceTools.Core.Dynamic;
using DynamicData;
using ReactiveUI;

namespace Avalonia.Diagnostics.ResourceTools.ViewModels;

public class ResourceGroup : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<Resource> resources;
    private string searchText;
    public Type ResourceType { get; }

    public ResourceGroup(Type resourceType, IConnectableCache<Resource, CompositeKey> itemsCache)
    {
        var filterPredicate = this
            .WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.TaskpoolScheduler)
            .DistinctUntilChanged()
            .Select(SearchItemFilterFunc);

        itemsCache.Connect()
            .RefCount()
            .Filter(filterPredicate)
            .Bind(out resources)
            .DisposeMany()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe();

        ResourceType = resourceType;
    }

    public ReadOnlyObservableCollection<Resource> Resources => resources;

    public string SearchText
    {
        get => searchText;
        set => this.RaiseAndSetIfChanged(ref searchText, value);
    }

    private static Func<Resource, bool> SearchItemFilterFunc(string? text)
    {
        return searchItem =>
        {
            if (text is null)
            {
                return true;
            }

            var containsName = searchItem.Key.ToString()!.Contains(text, StringComparison.InvariantCultureIgnoreCase);
            return containsName;
        };
    }
}