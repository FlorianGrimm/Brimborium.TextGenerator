
namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTSequence(ImmutableArray<ASTNode>? list = default) : ASTNode, IEnumerable<ASTNode> {
    public override void VisitorAccept<T>(IASTVisitor<T> visitor, T state)
        => visitor.VisitSequence(this, state);

    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state)
        => transformer.VisitSequence(this, state);

    public ImmutableArray<ASTNode> List { get; } = list ?? ImmutableArray<ASTNode>.Empty;

    public ASTSequence Add(ASTNode item) => new ASTSequence(this.List.Add(item));

    public ASTNode this[int index] => this.List[index];
    public int Count => this.List.Length;
    public IEnumerator<ASTNode> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public override string ToString() => $"ParserASTSequence #{this.List.Length}";

    private string GetDebuggerDisplay() => $"ParserASTSequence #{this.List.Length}";

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

    public Builder ToBuilder(Range? range = default) {
        if (range.HasValue) {
            return new Builder([..this.List.AsSpan(range.Value)]);
        } else {
            return new Builder(this.List.ToList());
        }
    }

    public sealed class Builder(List<ASTNode> list) {
        public List<ASTNode> List { get; } = list;
        public ASTSequence Build()
            => new ASTSequence(this.List.ToImmutableArray());
    }
}