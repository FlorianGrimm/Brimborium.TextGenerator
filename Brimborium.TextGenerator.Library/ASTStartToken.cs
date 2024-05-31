
namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTStartToken(
    StringSlice tag, 
    ImmutableArray<ASTParameter> listParameter
    ) : ASTNode, IEquatable<ASTStartToken> {
    private readonly StringSlice _Tag = tag;
    private readonly ImmutableArray<ASTParameter> _ListParameter = listParameter;

    public StringSlice Tag => this._Tag;
    public ImmutableArray<ASTParameter> ListParameter => this._ListParameter;

    public override bool Equals(object? obj) => obj is ASTStartToken other && this.Equals(other);

    public override int GetHashCode() {
        System.HashCode hashCode = new();
        hashCode.Add(this.Tag);
        for (int index = 0; index < this.ListParameter.Length; index++) {
            hashCode.Add(this.ListParameter[index]);
        }
        return hashCode.ToHashCode();
    }

    public bool Equals(ASTStartToken? other) {
        if (other is null) { return false; }
        if (this.ListParameter.Length != other.ListParameter.Length) { return false; }
        if (!(this.Tag.Equals(other.Tag))) { return false; }
        for (int index = 0; index < this.ListParameter.Length; index++) {
            if (!(this.ListParameter[index].Equals(other.ListParameter[index]))) {
                return false;
            }
        }
        return true;
    }

    public static bool operator ==(ASTStartToken left, ASTStartToken right) => left.Equals(right);
    public static bool operator !=(ASTStartToken left, ASTStartToken right) => !(left.Equals(right));

    public override void VisitorAccept<T>(IASTVisitor<T> visitor, T state)
        => visitor.VisitStartToken(this, state);

    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state)
        => transformer.VisitStartToken(this, state);

    private string GetDebuggerDisplay() => $"Start {this._Tag}";
}
