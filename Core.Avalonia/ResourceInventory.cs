namespace Avalonia.Diagnostics.ResourceTools.Core.Avalonia;

public class ResourceInventory : IResourceInventory
{
    private readonly IResourceExtractor resourceExtractor;
    private readonly IVisualBranchProvider visualBranchProvider;

    public ResourceInventory(IResourceExtractor resourceExtractor, IVisualBranchProvider visualBranchProvider)
    {
        this.resourceExtractor = resourceExtractor;
        this.visualBranchProvider = visualBranchProvider;
    }

    public IEnumerable<ResourceNode> Get(object target)
    {
        var branch = visualBranchProvider.GetBranch(target).ToList();
        var nodes = branch
            .Select(o => new ResourceNode (o, resourceExtractor.Extract(o).ToList()));
        return nodes;
    }
}
