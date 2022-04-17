using System;
using System.Collections;
using System.Collections.Generic;
using ReactiveUI;

namespace Avalonia.Diagnostics.ResourceTools.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string rootFolder;

    public MainViewModel(IObserver<string> observer, IEnumerable<ITitled> items)
    {
        Items = items;
        RootFolder = "E:/Repos/SuperJMN/WalletWasabi";
        this.WhenAnyValue(r => r.RootFolder).Subscribe(observer);
    }

    public string RootFolder
    {
        get => rootFolder;
        set => this.RaiseAndSetIfChanged(ref rootFolder, value);
    }

    public IEnumerable<ITitled> Items { get; }
}