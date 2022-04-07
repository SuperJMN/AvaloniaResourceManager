using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Rendering;
using Avalonia.Styling;
using Core;
using MoreLinq.Extensions;
using ResourceManager.ViewModels;

namespace ResourceManager.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            var textBlock = this.Find<TextBlock>("TextBlock");
            var dc = (MainWindowViewModel) DataContext;
            dc.SelectedItem = textBlock;

            base.OnApplyTemplate(e);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
