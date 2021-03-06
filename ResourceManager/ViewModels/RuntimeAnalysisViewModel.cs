using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Diagnostics.ResourceTools.Core;
using Avalonia.Diagnostics.ResourceTools.Core.Dynamic;
using DynamicData;
using ReactiveUI;

namespace Avalonia.Diagnostics.ResourceTools.ViewModels;

public class RuntimeAnalysisViewModel : ViewModelBase
{
    private object selectedItem;
    private readonly ReadOnlyObservableCollection<ResourceGroup> resourceGroups;

    public RuntimeAnalysisViewModel(IResourceInventory resourceAnalyzer)
    {
        var currentResourceNodes = this
            .WhenAnyValue(model => model.SelectedItem)
            .Select(target =>
            {
                if (target is null)
                {
                    return new List<ResourceNode>();
                }

                var nodes = resourceAnalyzer.Get(target).ToList();
                return nodes;
            });

        SourceCache<Resource, CompositeKey> cache = new(r => r.ResourceKey);

        var obs = from resList in currentResourceNodes
            from resNode in resList
            select from n in resNode.Resources select new Resource(resNode.Parent, n.Key, n.Value);

        cache.PopulateFrom(obs);

        cache.Connect()
            .Group(r => r.Value.GetType())
            .Transform(r => new ResourceGroup(r.Key, r.Cache))
            .Bind(out resourceGroups)
            .Subscribe();
    }

    public ReadOnlyObservableCollection<ResourceGroup> ResourceGroups => resourceGroups;

    public object SelectedItem
    {
        get => selectedItem;
        set => this.RaiseAndSetIfChanged(ref selectedItem, value);
    }
}