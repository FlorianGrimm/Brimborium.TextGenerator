namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTFinishToken : ASTNode, IEquatable<ASTFinishToken> {
    private readonly StringSlice _Tag;

    public StringSlice Tag => this._Tag;

    public ASTFinishToken(StringSlice tag) : base() {
        this._Tag = tag;
    }

    public override bool Equals(object? obj) => obj is ASTFinishToken other && this.Equals(other);

    public override int GetHashCode() => this.Tag.GetHashCode();

    public bool Equals(ASTFinishToken? other) {
        if (other is null) { return false; }
        return this.Tag.Equals(other.Tag);
    }

    public static bool operator ==(ASTFinishToken left, ASTFinishToken right) => left.Equals(right);
    public static bool operator !=(ASTFinishToken left, ASTFinishToken right) => !(left.Equals(right));

    public override void VisitorAccept<T>(IASTVisitor<T> visitor, T state)
        => visitor.VisitFinishToken(this, state);

    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state)
        => transformer.VisitFinishToken(this, state);

    private string GetDebuggerDisplay() => $"Finish {this._Tag}";
}
