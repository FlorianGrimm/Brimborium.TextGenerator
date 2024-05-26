namespace Brimborium.TextGenerator;
public interface IASTVisitor<T> {
    void VisitConstant(ASTConstant constant, T state);
    void VisitPlaceholder(ASTPlaceholder placeHolder, T state);
    void VisitSequence(ASTSequence sequence, T state);
    void VisitStartToken(ASTStartToken startToken, T state);
    void VisitFinishToken(ASTFinishToken finishToken, T state);
}

public class ASTVisitor<T> : IASTVisitor<T> {
    public virtual void VisitConstant(ASTConstant constant, T state) { }

    public virtual void VisitSequence(ASTSequence sequence, T state)
        => this.WalkSequence(sequence, state);

    public virtual void WalkSequence(ASTSequence sequence, T state) {
        foreach(var item in sequence.ListItem) {
            item.VisitorAccept(this, state);
        }
    }

    public virtual void VisitStartToken(ASTStartToken startToken, T state) { }

    public virtual void VisitFinishToken(ASTFinishToken finishToken, T state) { }

    public virtual void VisitPlaceholder(ASTPlaceholder placeHolder, T state)
        => this.WalkPlaceholder(placeHolder, state);

    public virtual void WalkPlaceholder(ASTPlaceholder placeHolder, T state) {
        foreach (var item in placeHolder.ListItem) {
            item.VisitorAccept(this, state);
        }
    }
}
