
namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTSequence(ImmutableArray<ASTNode>? list = default) : ASTNode
    //, IEnumerable<ASTNode>
    {
    public override void VisitorAccept<T>(IASTVisitor<T> visitor, T state)
        => visitor.VisitSequence(this, state);

    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state)
        => transformer.VisitSequence(this, state);

    public ImmutableArray<ASTNode> ListItem { get; } = list ?? ImmutableArray<ASTNode>.Empty;

    public ASTNode this[int index] => this.ListItem[index];

    public override string ToString() => $"ParserASTSequence #{this.ListItem.Length}";

    private string GetDebuggerDisplay() => $"ParserASTSequence #{this.ListItem.Length}";

#if false
    public IEnumerator<ASTNode> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);
    private struct Enumerator : IEnumerator<ASTNode> {
        private readonly ASTSequence _Sequence;
        private int _Index;

        public Enumerator(ASTSequence sequence) {
            this._Sequence = sequence;
            this._Index = -1;
        }

        public ASTNode Current => this._Sequence.List[this._Index];

        object IEnumerator.Current => this._Sequence.List[this._Index];

        public void Dispose() { }

        public bool MoveNext() => ++this._Index < this._Sequence.List.Length;

        public void Reset() => this._Index = -1;
    }
#endif
    public Builder ToBuilder(Range? range = default) {
        if (range.HasValue) {
            return new Builder([.. this.ListItem.AsSpan(range.Value)]);
        } else {
            return new Builder(this.ListItem.ToList());
        }
    }

    public sealed class Builder(List<ASTNode> list) {
        public List<ASTNode> ListItem { get; } = list;
        public ASTSequence Build()
            => new ASTSequence(this.ListItem.ToImmutableArray());
    }
}