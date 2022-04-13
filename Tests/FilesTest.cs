using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using CSharpFunctionalExtensions;
using FileSystem;
using Serilog;
using Xunit;

namespace Tests;

public class FilesTest
{
    [Fact]
    public async Task GetAllFiles()
    {
        var fs = new ZafiroFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None);
        var dir = fs.GetDirectory("E:\\Repos\\SuperJMN\\WalletWasabi")
            .Map(d => d.Files());

        var all = dir.Value;
    }
}