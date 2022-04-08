using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics.ResourceTools;
using Avalonia.Markup.Xaml;

namespace AvaloniaApplication1.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
        this.AttachResourceTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}