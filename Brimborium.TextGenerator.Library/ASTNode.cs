namespace Brimborium.TextGenerator;

public abstract class ASTNode {
    protected ASTNode() { }

    public abstract void VisitorAccept<T>(IASTVisitor<T> visitor, T state);

    public abstract ASTNode TransformerAccept<T>(IASTTransformer<T> transformer, T state);
}