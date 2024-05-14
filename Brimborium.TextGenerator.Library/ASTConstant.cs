using System.Diagnostics;

namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTConstant(StringSlice content) : ASTNode {
    public StringSlice Content { get; } = content;

    public override void VisitorAccept(IASTVisitor visitor) { visitor.VisitConstant(this); }
    public override void VisitorAcceptChildren(IASTVisitor visitor) { }

    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state) => transformer.VisitConstant(this, state);

    public override string ToString() => $"ParserASTConstant #{this.Content}";

    private string GetDebuggerDisplay() => $"ParserASTConstant #{this.Content}";
}

