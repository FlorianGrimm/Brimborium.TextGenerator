namespace Brimborium.TextGenerator;

public class ReplaceTests {
    [Fact]
    public void Replace01Parse() {
        Parser sut = Parser.CreateForCSharp();
        string content = "1/* <a> */2/* </a> */3";
        var act = sut.Parse(content);
        Assert.Equal(3, act.Count);

        {
            var visitorToString = new ASTVisitorToString();
            act.Accept(visitorToString);
            Assert.Equal("1/* <a> */2/* </a> */3", visitorToString.ToString());
        }

        ASTNode? actCopy = null;
        {
            var visitorReplace = new ASTVisitorReplace(
                (stack, current) => {
                    if (stack.Peek().Tag.Equals("a")) {
                        return new ASTConstant("XXX");
                    }
                    return current;
                });
            act.Accept(visitorReplace);
            actCopy = visitorReplace.GetResult();
        }

        {
            var visitorToString = new ASTVisitorToString();
            actCopy?.Accept(visitorToString);
            Assert.Equal("1/* <a> */XXX/* </a> */3", visitorToString.ToString());
        }
    }
}


public class ASTVisitorReplace : ASTVisitor {
    private List<ASTNode> _Results = new();
    private Stack<ASTPlaceholder> _StackASTPlaceholder = new();
    private readonly Func<Stack<ASTPlaceholder>, ASTNode, ASTNode> _Convert;

    public ASTVisitorReplace(
        Func<Stack<ASTPlaceholder>, ASTNode, ASTNode> convert
        ) {
        this._Convert = convert;
    }

    public ASTNode? GetResult() {
        if (this._Results.Count == 1) {
            return this._Results[0];
        } else {
            return null;
        }
    }

    public override void VisitConstant(ASTConstant parserASTConstant) {
        // base.VisitConstant(parserASTConstant);
        if (this._StackASTPlaceholder.Count == 0) {
            this._Results.Add(parserASTConstant);
        } else {
            this._Results.Add(this._Convert(this._StackASTPlaceholder, parserASTConstant));
        }
    }

    public override void VisitSequence(ASTSequence parserASTSequence) {
        var oldResults = this._Results;
        this._Results = new List<ASTNode>();
        ASTSequence astSequence = new ASTSequence(this._Results);
        base.VisitSequence(parserASTSequence);
        this._Results = oldResults;
        this._Results.Add(astSequence);
    }

    public override void VisitToken(ASTToken parserASTToken) {
        // base.VisitToken(parserASTToken);
        this._Results.Add(parserASTToken);
    }

    public override void VisitPlaceholder(ASTPlaceholder parserASTPlaceHolder) {
        var oldResults = this._Results;
        this._Results = new List<ASTNode>();
        var astPlaceholder = new ASTPlaceholder(
            parserASTPlaceHolder.StartToken,
            this._Results,
            parserASTPlaceHolder.FinishToken
        );
        this._StackASTPlaceholder.Push(parserASTPlaceHolder);
        foreach (var item in parserASTPlaceHolder.List) {
            item.Accept(this);
        }
        this._StackASTPlaceholder.Pop();
        this._Results = oldResults;
        this._Results.Add(astPlaceholder);
    }
}