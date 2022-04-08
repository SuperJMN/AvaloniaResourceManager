namespace Avalonia.Diagnostics.ResourceTools.Core;

public interface IVisualBranchProvider
{
    IEnumerable<object> GetBranch(object target);
}