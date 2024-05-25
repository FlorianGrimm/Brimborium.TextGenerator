#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.

namespace Brimborium.TextGenerator;

public class ParserTests {
    [Fact]
    public void Parser01Scan() {
        Parser sut = Parser.CreateForCSharp();
        string content = "1/* <a b=2> */2/* </a> */3";
        var act = sut.Scan(content);
        Assert.Equal(5, act.Count);
        Assert.Equal("1/* <a b=2> */2/* </a> */3", ASTTreeToString.GetAsString(act));
    }

    [Fact]
    public void Parser02Parse() {
        Parser sut = Parser.CreateForCSharp();
        string content = "1/* <a b=2 c=3> */2/* </a> */3";
        var act = sut.Parse(content);
        Assert.Equal(3, act.Count);
        Assert.Equal("1/* <a b=2 c=3> */2/* </a> */3", ASTTreeToString.GetAsString(act));
        Assert.Equal(2, ((ASTPlaceholder)act.List[1]).StartToken.ListParameter.Count);
        Assert.Equal("b", ((ASTPlaceholder)act.List[1]).StartToken.ListParameter[0].Name.ToString());
        Assert.Equal("c", ((ASTPlaceholder)act.List[1]).StartToken.ListParameter[1].Name.ToString());
    }
}
