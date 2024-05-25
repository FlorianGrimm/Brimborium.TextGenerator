namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTConstant(StringSlice content) : ASTNode {
    public StringSlice Content { get; } = content;

    public override void VisitorAccept<T>(IASTVisitor<T> visitor, T state)
        => visitor.VisitConstant(this, state);

    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state) 
        => transformer.VisitConstant(this, state);

    public override string ToString() => $"ParserASTConstant #{this.Content}";

    private string GetDebuggerDisplay() => $"ParserASTConstant #{this.Content}";
}

