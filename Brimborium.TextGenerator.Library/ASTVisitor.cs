namespace Brimborium.TextGenerator;
public interface IASTVisitor<T> {
    void VisitConstant(ASTConstant parserASTConstant, T state);
    void VisitPlaceholder(ASTPlaceholder parserASTPlaceHolder, T state);
    void VisitSequence(ASTSequence parserASTSequence, T state);
    void VisitStartToken(ASTStartToken startToken, T state);
    void VisitFinishToken(ASTFinishToken finishToken, T state);
}

public class ASTVisitor<T> : IASTVisitor<T> {
    public virtual void VisitConstant(ASTConstant parserASTConstant, T state) { }

    public virtual void VisitSequence(ASTSequence parserASTSequence, T state)
        => this.WalkSequence(parserASTSequence, state);

    public virtual void WalkSequence(ASTSequence parserASTSequence, T state) {
        for (int index = 0; index < parserASTSequence.List.Count; index++) {
            var item = parserASTSequence.List[index];
            item.VisitorAccept(this, state);
        }
    }

    public virtual void VisitStartToken(ASTStartToken startToken, T state) { }

    public virtual void VisitFinishToken(ASTFinishToken finishToken, T state) { }

    public virtual void VisitPlaceholder(ASTPlaceholder parserASTPlaceHolder, T state)
        => this.WalkPlaceholder(parserASTPlaceHolder, state);

    public virtual void WalkPlaceholder(ASTPlaceholder parserASTPlaceHolder, T state) {
        parserASTPlaceHolder.StartToken.VisitorAccept(this, state);
        foreach (var item in parserASTPlaceHolder.List) {
            item.VisitorAccept(this, state);
        }
        parserASTPlaceHolder.FinishToken.VisitorAccept(this, state);
    }
}
