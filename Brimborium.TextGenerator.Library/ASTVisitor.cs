namespace Brimborium.TextGenerator;
public interface IASTVisitor {
    void VisitChildrenPlaceholder(ASTPlaceholder parserASTPlaceHolder);
    void VisitChildrenSequence(ASTSequence parserASTSequence);
    void VisitConstant(ASTConstant parserASTConstant);
    void VisitPlaceholder(ASTPlaceholder parserASTPlaceHolder);
    void VisitSequence(ASTSequence parserASTSequence);
    void VisitToken(ASTToken parserASTToken);
}
public class ASTVisitor :IASTVisitor{
    public virtual void VisitConstant(ASTConstant parserASTConstant) {  }

    public virtual void VisitSequence(ASTSequence parserASTSequence) => this.VisitChildrenSequence(parserASTSequence);
    public virtual void VisitChildrenSequence(ASTSequence parserASTSequence) => parserASTSequence.VisitorAcceptChildren(this);

    public virtual void VisitToken(ASTToken parserASTToken) { }

    public virtual void VisitPlaceholder(ASTPlaceholder parserASTPlaceHolder) => this.VisitChildrenPlaceholder(parserASTPlaceHolder);
    public virtual void VisitChildrenPlaceholder(ASTPlaceholder parserASTPlaceHolder) => parserASTPlaceHolder.VisitorAcceptChildren(this);
}
