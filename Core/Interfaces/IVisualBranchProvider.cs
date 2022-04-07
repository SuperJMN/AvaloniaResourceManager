namespace Core.Interfaces
{
    public interface IVisualBranchProvider
    {
        IEnumerable<object> GetBranch(object target);
    }
}