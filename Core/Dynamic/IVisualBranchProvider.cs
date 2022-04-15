namespace Avalonia.Diagnostics.ResourceTools.Core.Dynamic;

public interface IVisualBranchProvider
{
    IEnumerable<object> GetBranch(object target);
}