namespace Brimborium.TextGenerator;

public sealed class ASTTreeToString : ASTVisitor<StringBuilder> {
    private static ASTTreeToString? _Instance;
    public static ASTTreeToString Instance => _Instance ??= new ASTTreeToString();


    public static string GetAsString(ASTNode node) {
        var sb = Brimborium.Text.StringBuilderPool.GetStringBuilder();
        node.VisitorAccept(ASTTreeToString.Instance, sb);
        var result = sb.ToString();
        Brimborium.Text.StringBuilderPool.ReturnStringBuilder(sb);
        return result;
    }

    private ASTTreeToString() { }

    public override void VisitConstant(ASTConstant parserASTConstant, StringBuilder state) {
        state.Append(parserASTConstant.Content);
        base.VisitConstant(parserASTConstant, state);
    }

    public override void VisitStartToken(ASTStartToken startToken, StringBuilder state) {
        state.Append("/* <").Append(startToken.Tag);
        foreach(var parameter in startToken.ListParameter) {
            if (parameter.Name.Contains(' ')) {
                state.Append(" \"").Append(parameter.Name).Append('"');
            } else { 
                state.Append(' ').Append(parameter.Name);
            }
            state.Append("=\"").Append(parameter.Value).Append('"');
        }
        state.Append("> */");
    }

    public override void VisitFinishToken(ASTFinishToken finishToken, StringBuilder state) {
        state.Append("/* </").Append(finishToken.Tag).Append("> */");
    }
}