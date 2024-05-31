namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTConstant(StringSlice content) : ASTNode, IEquatable<ASTConstant> {
    public StringSlice Content { get; } = content;

    public override bool Equals(object? obj) => obj is ASTConstant other && this.Equals(other);

    public override int GetHashCode() => this.Content.GetHashCode();

    public bool Equals(ASTConstant? other) {
        if (other is null) { return false; }
        return this.Content.Equals(other.Content);
    }

    public static bool operator ==(ASTConstant left, ASTConstant right) => left.Equals(right);
    public static bool operator !=(ASTConstant left, ASTConstant right) => !(left.Equals(right));
    public override void VisitorAccept<T>(IASTVisitor<T> visitor, T state)
        => visitor.VisitConstant(this, state);

    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state) 
        => transformer.VisitConstant(this, state);

    public override string ToString() => $"ParserASTConstant #{this.Content}";

    private string GetDebuggerDisplay() => $"ParserASTConstant #{this.Content}";
}

