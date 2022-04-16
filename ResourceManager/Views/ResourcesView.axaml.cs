using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Avalonia.Diagnostics.ResourceTools.Views
{
    public partial class ResourcesView : UserControl
    {
        public ResourcesView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
