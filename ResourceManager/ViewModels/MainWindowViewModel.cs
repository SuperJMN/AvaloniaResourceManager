using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Core.Interfaces;
using ReactiveUI;

namespace ResourceManager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private object selectedItem;
        private readonly ObservableAsPropertyHelper<List<ResourceNode>> resourceNodes;
        private readonly ObservableAsPropertyHelper<List<KeyValuePair<object, object?>>> resources;
        public string Greeting => "Welcome to Avalonia!";

        public MainWindowViewModel(IResourceInventory resourceAnalyzer)
        {
            var currentResourceNodes = this.WhenAnyValue(model => model.SelectedItem)
                .Select(target =>
                {
                    if (target is null)
                    {
                        return new List<ResourceNode>();
                    }

                    var nodes = resourceAnalyzer.Get(target).ToList();
                    return nodes;
                });
            
            resourceNodes = currentResourceNodes.ToProperty(this, x => x.ResourcesNodes);
            resources = currentResourceNodes
                .Select(list => list.SelectMany(r => r.Resources).ToList())
                .ToProperty(this, x => x.Resources);
        }

        public List<KeyValuePair<object, object?>> Resources => resources.Value;

        public List<ResourceNode> ResourcesNodes => resourceNodes.Value;
        
        public object SelectedItem
        {
            get => selectedItem;
            set => this.RaiseAndSetIfChanged(ref selectedItem, value);
        }
    }
}
