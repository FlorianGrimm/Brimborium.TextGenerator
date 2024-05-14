namespace Brimborium.TextGenerator;

public abstract class ASTNode {
    protected ASTNode() { }

    public abstract void VisitorAccept(IASTVisitor visitor);

    public abstract void VisitorAcceptChildren(IASTVisitor visitor);

    public abstract ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state);
}