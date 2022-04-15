using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using Xunit;
using CSharpFunctionalExtensions;
using FileSystem;
using Serilog;

namespace Tests;

public class ResourceTests
{
    //[Fact]
    //[Trait("Category", "Integration")]
    //public async Task Extract()
    //{
    //    var fs = new ZafiroFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None);
    //    var dir = fs.GetDirectory("E:\\Repos\\SuperJMN\\WalletWasabi")
    //        .Map(dir => new ResourceRoot().GetResources(dir));

    //    var allResources = await dir.Value.ToList();
    //}

    //[Fact]
    //[Trait("Category", "Integration")]
    //public async Task Find_usages()
    //{
    //    var fs = new ZafiroFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None);
    //    var dir = fs.GetDirectory("E:\\Repos\\SuperJMN\\WalletWasabi")
    //        .Map(dir => new ResourceRoot().GetUsages(dir));

    //    var allResources = await dir.Value.ToList();
    //    var grouped = allResources.GroupBy(r => r.Value)
    //        .OrderByDescending(r => r.Count());
    //}
}