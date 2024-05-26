namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTPlaceholder(
    ASTStartToken startToken,
    List<ASTNode> list,
    ASTFinishToken finishToken
    ) : ASTNode, IEnumerable<ASTNode> {
    public override void VisitorAccept<T>(IASTVisitor<T> visitor, T state)
        => visitor.VisitPlaceholder(this, state);
    
    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state)
        => transformer.VisitPlaceholder(this, state);
    
    public ASTStartToken StartToken { get; set; } = startToken;
    public List<ASTNode> List { get; } = list;
    public ASTFinishToken FinishToken { get; set; } = finishToken;

    public ASTPlaceholder Add(ASTNode item) {
        this.List.Add(item);
        return this;
    }
    public ASTNode this[int index] { get { return this.List[index]; } }
    public int Count { get { return this.List.Count; } }

    public StringSlice Tag => this.StartToken.Tag;

    public ASTPlaceholder WithStartToken(ASTStartToken startToken) 
        => new ASTPlaceholder(startToken, new List<ASTNode>(this.List), this.FinishToken);

    public ASTPlaceholder WithFinishToken(ASTFinishToken finishToken) 
        => new ASTPlaceholder(this.StartToken, new List<ASTNode>(this.List), finishToken);

    public ASTPlaceholder WithList(List<ASTNode> list) 
        => new ASTPlaceholder(this.StartToken, list, this.FinishToken);

    public IEnumerator<ASTNode> GetEnumerator() => this.List.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => this.List.GetEnumerator();

    public override string ToString() => $"ParserASTPlaceholder {this.StartToken.Tag} #{this.List.Count}";

    private string GetDebuggerDisplay() => $"ParserASTPlaceholder {this.StartToken.Tag} #{this.List.Count}";

}

