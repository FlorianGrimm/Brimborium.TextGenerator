using System.Diagnostics;

namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTToken(
    StringSlice complete,
    StringSlice prefix, StringSlice tag, StringSlice parameter,
    bool isStartTag, bool isFinishTag
    ) : ASTNode {
    public static ASTToken Create(
        StringSlice complete,
        StringSlice prefix, StringSlice tag, StringSlice parameter
        ) {
        var isStartTag = prefix.Equals("<");
        var isFinishTag = prefix.Equals("</");
        // TODO: parameter
        return new ASTToken(complete, prefix, tag, parameter, isStartTag, isFinishTag);
    }

    public StringSlice Complete { get; } = complete;
    public StringSlice Prefix { get; } = prefix;
    public StringSlice Tag { get; } = tag;
    public StringSlice Parameter { get; } = parameter;
    public bool IsStartTag { get; } = isStartTag;
    public bool IsFinishTag { get; } = isFinishTag;

    public override void VisitorAccept(IASTVisitor visitor) { visitor.VisitToken(this); }
    public override void VisitorAcceptChildren(IASTVisitor visitor) { }
    public override ASTTransformResult<T> TransformerAccept<T>(IASTTransformer<T> transformer) => transformer.VisitToken(this);
    public override string ToString() => $"ParserASTToken #{this.Tag}";
    private string GetDebuggerDisplay() => $"ParserASTToken #{this.Tag}";
}

