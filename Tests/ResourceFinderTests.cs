using System.IO;
using System.Xml;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using Xunit;

namespace Tests;

public class ResourceFinderTests
{
    [Fact]
    public void Extract()
    {
        var sut = new ResourceFinder();

        var xmlDocument = new XmlDocument();
        xmlDocument.Load("TestFiles\\Styles.axaml");
        var resources = sut.FindAll(xmlDocument);
    }
}