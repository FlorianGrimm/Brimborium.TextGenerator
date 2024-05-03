namespace Brimborium.TextGenerator;

//public class ASTTransformVisitor { }

public class ASTVisitorReplace : ASTVisitor {
    private List<ASTNode> _Results = new();
    private List<ASTPlaceholder> _StackASTPlaceholder = new();
    private readonly Func<List<ASTPlaceholder>, ASTNode, ASTNode> _Convert;

    public ASTVisitorReplace(
        Func<List<ASTPlaceholder>, ASTNode, ASTNode> convert
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
        this._StackASTPlaceholder.Add(parserASTPlaceHolder);
        foreach (var item in parserASTPlaceHolder.List) {
            item.VisitorAccept(this);
        }
        this._StackASTPlaceholder.RemoveAt(this._StackASTPlaceholder.Count-1);
        this._Results = oldResults;
        this._Results.Add(astPlaceholder);
    }
}