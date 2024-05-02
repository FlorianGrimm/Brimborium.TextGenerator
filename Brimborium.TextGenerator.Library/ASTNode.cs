namespace Brimborium.TextGenerator;

public abstract class ASTNode {
    protected ASTNode() {
    }

    public virtual void Accept(ASTVisitor visitor) {
    }
    public virtual void AcceptChildren(ASTVisitor visitor) {
    }
}

