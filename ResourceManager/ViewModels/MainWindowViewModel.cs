using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Core.Interfaces;
using DynamicData;
using ReactiveUI;

namespace ResourceManager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private object selectedItem;
        private readonly ReadOnlyObservableCollection<Resource> resources;

        public MainWindowViewModel(IResourceInventory resourceAnalyzer)
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
                .Bind(out resources)
                .Subscribe();
        }

        public ReadOnlyObservableCollection<Resource> Resources => resources;

        public object SelectedItem
        {
            get => selectedItem;
            set => this.RaiseAndSetIfChanged(ref selectedItem, value);
        }
    }
}
