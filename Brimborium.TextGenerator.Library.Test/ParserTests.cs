namespace Brimborium.TextGenerator;

public class ParserTests {
    [Fact]
    public void Parser01ParseFlat() {
        Parser sut = Parser.CreateForCSharp();
        string content = "1/* <a> */2/* </a> */3";
        var act = sut.ParseFlat(content);
        Assert.Equal(5, act.Count);

        var visitor = new ASTVisitorToString();
        act.Accept(visitor);
        Assert.Equal("1/* <a> */2/* </a> */3", visitor.ToString());
    }

    [Fact]
    public void Parser10Parse() {
        Parser sut = Parser.CreateForCSharp();
        string content = "1/* <a> */2/* </a> */3";
        var act = sut.Parse(content);
        Assert.Equal(3, act.Count);

        var visitor = new ASTVisitorToString();
        act.Accept(visitor);
        Assert.Equal("1/* <a> */2/* </a> */3", visitor.ToString());
    }
}
