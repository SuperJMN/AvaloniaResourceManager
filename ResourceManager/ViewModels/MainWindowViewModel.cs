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
        private readonly ObservableAsPropertyHelper<List<ResourceNode>> resources;
        public string Greeting => "Welcome to Avalonia!";

        public MainWindowViewModel(IResourceInventory inventory)
        {
            resources = this.WhenAnyValue(model => model.SelectedItem)
                .Select(o =>
                {
                    if (o is null)
                    {
                        return new List<ResourceNode>();
                    }
                    return inventory.Get(o).ToList();
                })
                .ToProperty(this, x => x.Resources);
        }

        public List<ResourceNode> Resources => resources.Value;
        
        public object SelectedItem
        {
            get => selectedItem;
            set => this.RaiseAndSetIfChanged(ref selectedItem, value);
        }
    }
}
