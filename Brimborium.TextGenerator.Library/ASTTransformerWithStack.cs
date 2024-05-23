namespace Brimborium.TextGenerator;

public class ASTTransformerWithStack<T> : ASTTransformer<T> {
    public ImmutableList<ASTNode> StackNode = ImmutableList<ASTNode>.Empty;
    public ImmutableList<ASTPlaceholder> StackPlaceholder = ImmutableList<ASTPlaceholder>.Empty;

    public override ASTPlaceholder VisitPlaceholder(ASTPlaceholder placeholder, T state) {
        var oldStackNode = this.StackNode;
        var oldStackPlaceholder = this.StackPlaceholder;
        this.StackNode = oldStackNode.Add(placeholder);
        this.StackPlaceholder = oldStackPlaceholder.Add(placeholder);
        var result = base.VisitPlaceholder(placeholder, state);
        this.StackNode = oldStackNode;
        this.StackPlaceholder = oldStackPlaceholder;
        return result;
    }

    public override ASTSequence VisitSequence(ASTSequence sequence, T state) {
        var oldStackNode = this.StackNode;
        this.StackNode = oldStackNode.Add(sequence);
        var result = base.VisitSequence(sequence, state);
        this.StackNode = oldStackNode;
        return result;
    }
}
