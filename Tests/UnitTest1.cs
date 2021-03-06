using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Avalonia.Diagnostics.ResourceTools.Core.Static;
using Xunit;

namespace Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var doc = new XmlDocument();
        doc.Load(File.OpenRead("E:\\Repos\\zkSNACKs\\WalletWasabi\\WalletWasabi.Fluent\\Styles\\Themes\\Base.axaml"));

        var nodes = doc.SelectNodes("//*[contains(name(), 'Resources')]");
        var resources = ExtractChildren(nodes.Item(0).ChildNodes).ToList();
    }

    private IEnumerable<Resource> ExtractChildren(XmlNodeList nodes)
    {
        var m = from n in nodes.OfType<XmlElement>()
            let att = n.Attributes["x:Key"]
            where att != null
            select new Resource(n.Name, att.Value, n.OuterXml);

        return m;
    }
}