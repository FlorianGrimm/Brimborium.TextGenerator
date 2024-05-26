namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTPlaceholder(
    StringSlice tag,
    ImmutableArray<ASTParameter> listParameter,
    ImmutableArray<ASTNode> listItem
    ) : ASTNode
    //, IEnumerable<ASTNode>
    {
    public override void VisitorAccept<T>(IASTVisitor<T> visitor, T state)
        => visitor.VisitPlaceholder(this, state);

    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state)
        => transformer.VisitPlaceholder(this, state);

    public StringSlice Tag { get; } = tag;
    public ImmutableArray<ASTParameter> ListParameter { get; } = listParameter;
    public ImmutableArray<ASTNode> ListItem { get; } = listItem;

    public ASTNode this[int index] { get { return this.ListItem[index]; } }

    //public ASTPlaceholder WithStartToken(ASTStartToken startToken) 
    //    => new ASTPlaceholder(startToken, this.ListItem, this.FinishToken);

    //public ASTPlaceholder WithFinishToken(ASTFinishToken finishToken) 
    //    => new ASTPlaceholder(this.StartToken, new List<ASTNode>(this.ListItem), finishToken);

    public ASTPlaceholder WithListItem(ImmutableArray<ASTNode> listItem)
        => new ASTPlaceholder(this.Tag, this.ListParameter, listItem);

    //public IEnumerator<ASTNode> GetEnumerator() => this.ListItem.GetEnumerator();
    //IEnumerator IEnumerable.GetEnumerator() => this.ListItem.GetEnumerator();

    public override string ToString() => $"ParserASTPlaceholder {this.Tag} #{this.ListItem.Length}";

    private string GetDebuggerDisplay() => $"ParserASTPlaceholder {this.Tag} #{this.ListItem.Length}";

    public Builder ToBuilder() {
        return new Builder(this.Tag, this.ListParameter, this.ListItem);
    }
    public class Builder(StringSlice tag, ImmutableArray<ASTParameter> listParameter, ImmutableArray<ASTNode> listItem) {
        private StringSlice _Tag = tag;
        private ImmutableArray<ASTParameter> _OrginalListParameter = listParameter;
        private ImmutableArray<ASTNode> _OrginalListItem = listItem;

        private List<ASTParameter>? _ModifiedListParameter;
        private List<ASTNode>? _ModifiedListItem;

        public StringSlice Tag { get => this._Tag; set => this._Tag = value; }
        public List<ASTParameter> ListParameter {
            get => (this._ModifiedListParameter ??= _OrginalListParameter.ToList());
            set => this._ModifiedListParameter = value;
        }
        public List<ASTNode> ListItem {
            get => (this._ModifiedListItem ??= this._OrginalListItem.ToList());
            set => this._ModifiedListItem = value;
        }

        public ASTPlaceholder Build() {
            return new ASTPlaceholder(
                this._Tag, 
                this._ModifiedListParameter?.ToImmutableArray() ?? this._OrginalListParameter, 
                this._ModifiedListItem?.ToImmutableArray() ?? this._OrginalListItem);
        }
    }
}

