using Avalonia;
using Avalonia.Controls;
using Core.Interfaces;

namespace Core
{
    public class ResourceExtractor : IResourceExtractor
    {
        public IEnumerable<KeyValuePair<object, object?>> Extract(object b)
        {
            return b switch
            {
                Window w => Extract(w),
                Application app => Extract(app),
                StyledElement v => Extract(v),
                _ => throw new InvalidOperationException()
            };
        }

        private IEnumerable<KeyValuePair<object, object?>> Extract(StyledElement visual)
        {
            return visual.Resources;
        }

        private IEnumerable<KeyValuePair<object, object?>> Extract(Application application)
        {
            return application.Resources;
        }

        private IEnumerable<KeyValuePair<object, object?>> Extract(Window window)
        {
            return window.Resources;
        }
    }
}