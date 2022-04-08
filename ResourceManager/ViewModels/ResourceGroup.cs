using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia.Diagnostics.ResourceTools.Core;
using DynamicData;
using ReactiveUI;

namespace Avalonia.Diagnostics.ResourceTools.ViewModels;

public class ResourceGroup : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<ColdResource> resources;
    private string searchText;
    public Type ResourceType { get; }

    public ResourceGroup(Type resourceType, IConnectableCache<ColdResource, CompositeKey> itemsCache)
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

    public ReadOnlyObservableCollection<ColdResource> Resources => resources;

    public string SearchText
    {
        get => searchText;
        set => this.RaiseAndSetIfChanged(ref searchText, value);
    }

    private static Func<ColdResource, bool> SearchItemFilterFunc(string? text)
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