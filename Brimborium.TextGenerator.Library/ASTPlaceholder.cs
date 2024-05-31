namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTPlaceholder(
    StringSlice tag,
    ImmutableArray<ASTParameter> listParameter,
    ImmutableArray<ASTNode> listItem
    ) : ASTNode, IWithListParameter, IEquatable<ASTPlaceholder> {
    public override void VisitorAccept<T>(IASTVisitor<T> visitor, T state)
        => visitor.VisitPlaceholder(this, state);

    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state)
        => transformer.VisitPlaceholder(this, state);

    public StringSlice Tag { get; } = tag;
    public ImmutableArray<ASTParameter> ListParameter { get; } = listParameter;
    public ImmutableArray<ASTNode> ListItem { get; } = listItem;

    public ASTPlaceholder WithListItem(ImmutableArray<ASTNode> listItem)
        => new ASTPlaceholder(this.Tag, this.ListParameter, listItem);

    public override bool Equals(object? obj) => obj is ASTPlaceholder other && this.Equals(other);

    public override int GetHashCode() {
        System.HashCode hashCode = new();
        hashCode.Add(this.Tag);
        for (int index = 0; index < this.ListParameter.Length; index++) {
            hashCode.Add(this.ListParameter[index]);
        }
        for (int index = 0; index < this.ListItem.Length; index++) {
            hashCode.Add(this.ListItem[index]);
        }
        return hashCode.ToHashCode();
    }

    public bool Equals(ASTPlaceholder? other) {
        if (other is null) { return false; }
        if (this.ListParameter.Length != other.ListParameter.Length) { return false; }
        if (this.ListItem.Length != other.ListItem.Length) { return false; }
        if (!(this.Tag.Equals(other.Tag))) { return false; }
        for (int index = 0; index < this.ListParameter.Length; index++) {
            if (!(this.ListParameter[index].Equals(other.ListParameter[index]))) {
                return false;
            }
        }
        for (int index = 0; index < this.ListItem.Length; index++) {
            if (!(this.ListItem[index].Equals(other.ListItem[index]))) {
                return false;
            }
        }
        return true;
    }

    public static bool operator ==(ASTPlaceholder left, ASTPlaceholder right) => left.Equals(right);
    public static bool operator !=(ASTPlaceholder left, ASTPlaceholder right) => !(left.Equals(right));

    public override string ToString() => $"ParserASTPlaceholder {this.Tag} #{this.ListItem.Length}";

    private string GetDebuggerDisplay() => $"ParserASTPlaceholder {this.Tag} #{this.ListItem.Length}";

    public Builder ToBuilder() {
        return new Builder(this, this.Tag, this.ListParameter, this.ListItem);
    }

    public sealed class Builder(ASTPlaceholder? placeholder, StringSlice tag, ImmutableArray<ASTParameter> listParameter, ImmutableArray<ASTNode> listItem) {
        private readonly ASTPlaceholder? _OrginalPlaceholder = placeholder;
        private readonly StringSlice _OrginalTag = tag;
        private readonly ImmutableArray<ASTParameter> _OrginalListParameter = listParameter;
        private readonly ImmutableArray<ASTNode> _OrginalListItem = listItem;
        private StringSlice? _ModifiedTag;
        private List<ASTParameter>? _ModifiedListParameter;
        private List<ASTNode>? _ModifiedListItem;

        public StringSlice Tag { get => this._ModifiedTag ?? this._OrginalTag; set => this._ModifiedTag = value; }
        public List<ASTParameter> ListParameter {
            get => (this._ModifiedListParameter ??= _OrginalListParameter.ToList());
            set => this._ModifiedListParameter = value;
        }
        public List<ASTNode> ListItem {
            get => (this._ModifiedListItem ??= this._OrginalListItem.ToList());
            set => this._ModifiedListItem = value;
        }

        public ASTPlaceholder Build() {
            if (this._OrginalPlaceholder is not null
                && this._ModifiedTag is null) {

                if (this._ModifiedListParameter is null && this._ModifiedListItem is null) {
                    return this._OrginalPlaceholder;
                }

                if (this._ModifiedListParameter is not null
                    && this._OrginalListParameter.Length == this._ModifiedListParameter.Count
                    && this._ModifiedListItem is not null
                    && this._OrginalListItem.Length == this._ModifiedListItem.Count
                    ) {
                    bool equal = true;
                    for (int index = this._OrginalListParameter.Length - 1; 0 <= index; index--) {
                        if (this._OrginalListParameter[index] != this._ModifiedListParameter[index]) {
                            equal = false;
                            break;
                        }
                    }
                    for (int index = this._OrginalListItem.Length - 1; 0 <= index; index--) {
                        if (this._OrginalListItem[index] != this._ModifiedListItem[index]) {
                            equal = false;
                            break;
                        }
                    }
                    if (equal) {
                        return this._OrginalPlaceholder;
                    }
                }
            }
            {
                return new ASTPlaceholder(
                    tag: this._ModifiedTag ?? this._OrginalTag,
                    listParameter: (this._ModifiedListParameter?.ToImmutableArray()) ?? this._OrginalListParameter,
                    listItem: (this._ModifiedListItem?.ToImmutableArray()) ?? this._OrginalListItem);
            }
        }
    }
}

