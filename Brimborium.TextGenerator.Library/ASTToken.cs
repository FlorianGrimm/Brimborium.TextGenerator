using System.Diagnostics;

namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTToken(
    StringSlice complete,
    StringSlice prefix, StringSlice tag, StringSlice parameter,
    List<ASTParameter> listParameter,
    bool isStartTag, bool isFinishTag
    ) : ASTNode {
    public static ASTToken Create(
        StringSlice complete,
        StringSlice prefix, StringSlice tag,
        StringSlice parameter, List<ASTParameter> listParameter
        ) {
        var isStartTag = prefix.Equals("<");
        var isFinishTag = prefix.Equals("</");
        // TODO: parameter
        return new ASTToken(complete, prefix, tag, parameter, listParameter, isStartTag, isFinishTag);
    }

    public StringSlice Complete { get; } = complete;
    public StringSlice Prefix { get; } = prefix;
    public StringSlice Tag { get; } = tag;
    public StringSlice Parameter { get; } = parameter;
    public List<ASTParameter> ListParameter { get; } = listParameter;
    public bool IsStartTag { get; } = isStartTag;
    public bool IsFinishTag { get; } = isFinishTag;

    public override void VisitorAccept<T>(IASTVisitor<T> visitor, T state)
        => visitor.VisitToken(this, state);

    public override ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state)
        => transformer.VisitToken(this, state);

    public override string ToString() => $"ParserASTToken #{this.Tag}";
    private string GetDebuggerDisplay() => $"ParserASTToken #{this.Tag}";
}

