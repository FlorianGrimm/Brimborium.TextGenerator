namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTSequence(List<ASTNode>? list = default) : ASTNode, IEnumerable<ASTNode> {
    public override void VisitorAccept<T>(IASTVisitor<T> visitor, T state)
        => visitor.VisitSequence(this, state);
    
    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state)
        => transformer.VisitSequence(this, state);

    public List<ASTNode> List { get; } = list ?? [];

    public ASTSequence Add(ASTNode item) {
        this.List.Add(item);
        return this;
    }
    public ASTNode this[int index] { get { return this.List[index]; } }
    public int Count { get { return this.List.Count; } }
    public IEnumerator<ASTNode> GetEnumerator() => this.List.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => this.List.GetEnumerator();

    public override string ToString() => $"ParserASTSequence #{this.List.Count}";

    private string GetDebuggerDisplay() => $"ParserASTSequence #{this.List.Count}";
}

