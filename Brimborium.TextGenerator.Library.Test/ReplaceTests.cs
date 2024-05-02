namespace Brimborium.TextGenerator;

public class ReplaceTests {
    [Fact]
    public void Replace01Parse() {
        Parser sut = Parser.CreateForCSharp();
        string content = "1/* <a> */2/* </a> */3";
        var act = sut.Parse(content);
        Assert.Equal(3, act.Count);

        var visitor = new ASTVisitorToString();
        visitor.VisitSequence(act);
        Assert.Equal("1/* <a> */2/* </a> */3", visitor.ToString());
    }
}
