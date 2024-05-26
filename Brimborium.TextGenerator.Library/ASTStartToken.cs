
namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTStartToken : ASTNode {
    private readonly StringSlice _Tag;
    private readonly ImmutableList<ASTParameter> _ListParameter;

    public StringSlice Tag => this._Tag;
    public ImmutableList<ASTParameter> ListParameter => this._ListParameter;

    public ASTStartToken(StringSlice tag, ImmutableList<ASTParameter> listParameter) {
        this._Tag = tag;
        this._ListParameter = listParameter;
    }

    public override void VisitorAccept<T>(IASTVisitor<T> visitor, T state)
        => visitor.VisitStartToken(this, state);

    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state)
        => transformer.VisitStartToken(this, state);

    private string GetDebuggerDisplay() => $"Start {this._Tag}";
}
