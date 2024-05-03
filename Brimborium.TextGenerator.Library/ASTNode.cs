namespace Brimborium.TextGenerator;

public abstract class ASTNode {
    protected ASTNode() {
    }

    public abstract void VisitorAccept(IASTVisitor visitor);
    public abstract void VisitorAcceptChildren(IASTVisitor visitor);
    public abstract ASTTransformResult<T> TransformerAccept<T>(IASTTransformer<T> transformer);
}

