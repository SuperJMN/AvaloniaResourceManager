using Avalonia;
using Avalonia.VisualTree;
using Core.Interfaces;

namespace Core
{
    public class VisualBranchProvider : IVisualBranchProvider
    {
        public IEnumerable<object> GetBranch(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var visualBranch = GetVisualTreeBranch((IVisual)target).ToList();
            var root = visualBranch.OfType<StyledElement>()
                .Select(parent => parent.Parent)
                .LastOrDefault(element => element is not null);

            var application = Application.Current;

            var rootList = root != null ? new object[] {root} : Enumerable.Empty<object>();
            return visualBranch.Concat(rootList).Concat(new object[] { application });
        }

        private IEnumerable<IVisual> GetVisualTreeBranch(IVisual textBlock)
        {
            var branch = GetVisualTreeBranchCore(textBlock);
            return branch;
        }

        private static IEnumerable<IVisual> GetVisualTreeBranchCore(IVisual visual)
        {
            return new[] { visual }.Concat(visual.VisualParent is { } parent
                ? GetVisualTreeBranchCore(parent)
                : Enumerable.Empty<IVisual>());
        }
    }
}