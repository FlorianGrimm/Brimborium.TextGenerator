#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.

namespace Brimborium.TextGenerator;

public class ParserTests {
    [Fact]
    public void Parser01ScanSimple() {
        Parser parser = Parser.CreateForCSharp();
        string content = "1/* <a> */2/* </a> */3";
        var act = parser.Scan(content);
        Assert.Equal(5, act.ListItem.Length);
        Assert.Equal("1/* <a> */2/* </a> */3", ASTTreeToString.GetAsString(act));
    }

    [Fact]
    public void Parser02ScanParameter() {
        Parser parser = Parser.CreateForCSharp();
        string content = "1/* <a b=2> */2/* </a> */3";
        var act = parser.Scan(content);
        Assert.Equal(5, act.ListItem.Length);
        Assert.Equal("""1/* <a b="2"> */2/* </a> */3""", ASTTreeToString.GetAsString(act));
    }

    [Fact]
    public void Parser03ScanPostfix() {
        Parser parser = Parser.CreateForCSharp();
        string content = "1/* <a b=2 /> */3";
        var act = parser.Scan(content);
        Assert.Equal(3, act.ListItem.Length);
        Assert.Equal("""1/* <a b="2"> *//* </a> */3""", ASTTreeToString.GetAsString(act));
    }

    [Fact]
    public void Parser05ParseSimple() {
        Parser sut = Parser.CreateForCSharp();
        string content = "1/* <a> */2/* </a> */3";
        var act = sut.Parse(content);
        Assert.Equal(3, act.ListItem.Length);
        Assert.Equal("1/* <a> */2/* </a> */3", ASTTreeToString.GetAsString(act));
        Assert.Equal(0, ((ASTPlaceholder)act.ListItem[1]).ListParameter.Length);
    }

    [Fact]
    public void Parser06ParseParameter() {
        Parser sut = Parser.CreateForCSharp();
        string content = "1/* <a b=2 c=3> */2/* </a> */3";
        var act = sut.Parse(content);
        Assert.Equal(3, act.ListItem.Length);
        Assert.Equal("""1/* <a b="2" c="3"> */2/* </a> */3""", ASTTreeToString.GetAsString(act));
        Assert.Equal(2, ((ASTPlaceholder)act.ListItem[1]).ListParameter.Length);
        Assert.Equal("b", ((ASTPlaceholder)act.ListItem[1]).ListParameter[0].Name.ToString());
        Assert.Equal("c", ((ASTPlaceholder)act.ListItem[1]).ListParameter[1].Name.ToString());
    }
}
