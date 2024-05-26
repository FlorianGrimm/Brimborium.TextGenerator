
namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTStartToken(
    StringSlice tag, 
    ImmutableArray<ASTParameter> listParameter
    ) : ASTNode {
    private readonly StringSlice _Tag = tag;
    private readonly ImmutableArray<ASTParameter> _ListParameter = listParameter;

    public StringSlice Tag => this._Tag;
    public ImmutableArray<ASTParameter> ListParameter => this._ListParameter;

    public override void VisitorAccept<T>(IASTVisitor<T> visitor, T state)
        => visitor.VisitStartToken(this, state);

    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state)
        => transformer.VisitStartToken(this, state);

    private string GetDebuggerDisplay() => $"Start {this._Tag}";
}
