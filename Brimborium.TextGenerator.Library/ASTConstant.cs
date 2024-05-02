using System.Diagnostics;

namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTConstant(StringSlice content) : ASTNode {
    public StringSlice Content { get; } = content;

    public override void Accept(ASTVisitor visitor) { visitor.VisitConstant(this); }
    public override void AcceptChildren(ASTVisitor visitor) { }

    public override string ToString() => $"ParserASTConstant #{this.Content}";

    private string GetDebuggerDisplay() => $"ParserASTConstant #{this.Content}";
}

