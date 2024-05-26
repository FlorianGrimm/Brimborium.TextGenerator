
namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTFinishToken : ASTNode {
    private readonly StringSlice _Tag;

    public StringSlice Tag => this._Tag;

    public ASTFinishToken(StringSlice tag) :base()
    {
        this._Tag = tag;
    }

    public override void VisitorAccept<T>(IASTVisitor<T> visitor, T state) 
        => visitor.VisitFinishToken(this, state);

    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state) 
        => transformer.VisitFinishToken(this, state);

    private string GetDebuggerDisplay() => $"Finish {this._Tag}";
}
