using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using CSharpFunctionalExtensions;
using FileSystem;
using Serilog;

namespace Avalonia.Diagnostics.ResourceTools.ViewModels;

public class ProjectWatcher
{
    private readonly IObservable<string> folderPath;
    private readonly IZafiroFileSystem fileSystem = new ZafiroFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None);

    public ProjectWatcher(IObservable<string> folderPath)
    {
        this.folderPath = folderPath;
        Resources = OnLoad();
    }

    public IObservable<IList<ResourceDefinition>> Resources { get; }

    private IObservable<IList<ResourceDefinition>> OnLoad()
    {
        return folderPath
            .FirstAsync()
            .SelectMany(s => new ResourceRoot(fileSystem.GetDirectory(s).Value).GetResources().ToList());
    }
}