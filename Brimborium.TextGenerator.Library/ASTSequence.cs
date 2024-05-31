namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTSequence(
    ImmutableArray<ASTNode>? listItem = default
    ) : ASTNode, IEquatable<ASTSequence> {
    public override void VisitorAccept<T>(IASTVisitor<T> visitor, T state)
        => visitor.VisitSequence(this, state);

    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state)
        => transformer.VisitSequence(this, state);

    public ImmutableArray<ASTNode> ListItem { get; } = listItem ?? ImmutableArray<ASTNode>.Empty;

    public ASTNode this[int index] => this.ListItem[index];

    public override bool Equals(object? obj) => obj is ASTSequence other && this.Equals(other);

    public override int GetHashCode() {
        System.HashCode hashCode = new();
        for (int index = 0; index < this.ListItem.Length; index++) {
            hashCode.Add(this.ListItem[index]);
        }
        return hashCode.ToHashCode();
    }

    public bool Equals(ASTSequence? other) {
        if (other is null) { return false; }
        if (this.ListItem.Length != other.ListItem.Length) { return false; }
        for (int index = 0; index < this.ListItem.Length; index++) {
            if (!(this.ListItem[index].Equals(other.ListItem[index]))) {
                return false;
            }
        }
        return true;
    }

    public static bool operator ==(ASTSequence left, ASTSequence right) => left.Equals(right);
    public static bool operator !=(ASTSequence left, ASTSequence right) => !(left.Equals(right));

    public override string ToString() => $"ParserASTSequence #{this.ListItem.Length}";

    private string GetDebuggerDisplay() => $"ParserASTSequence #{this.ListItem.Length}";

    public Builder ToBuilder(Range? range = default)
        => range.HasValue
            ? new Builder(null, ImmutableArray<ASTNode>.Empty, [.. this.ListItem.AsSpan(range.Value)])
            : new Builder(this, this.ListItem, null);

    public sealed class Builder(ASTSequence? sequence, ImmutableArray<ASTNode> orginalListItem, List<ASTNode>? modifiedListItem) {
        public Builder()
            : this(null, ImmutableArray<ASTNode>.Empty, []) {
        }

        private readonly ASTSequence? _Sequence = sequence;
        private readonly ImmutableArray<ASTNode> _OrginalListItem = orginalListItem;
        private List<ASTNode>? _ModifiedListItem = modifiedListItem;

        public List<ASTNode> ListItem {
            get => (this._ModifiedListItem ??= this._OrginalListItem.ToList());
            set => this._ModifiedListItem = value;
        }

        public ASTSequence Build() {
            if (this._Sequence is not null
                && this._ModifiedListItem is not null
                && this._OrginalListItem.Length == this._ModifiedListItem.Count) {
                bool equal = true;
                for (int index = 0; index < this._OrginalListItem.Length; index++) {
                    if (this._OrginalListItem[index] != this._ModifiedListItem[index]) {
                        equal = false;
                        break;
                    }
                }
                if (equal) {
                    return this._Sequence;
                }
            }
            return new ASTSequence(
                listItem: (this._ModifiedListItem?.ToImmutableArray()) ?? this._OrginalListItem);
        }
    }
}