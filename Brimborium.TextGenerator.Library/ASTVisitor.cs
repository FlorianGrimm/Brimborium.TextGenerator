namespace Brimborium.TextGenerator;

public class ASTVisitor {
    public virtual void VisitConstant(ASTConstant parserASTConstant) {  }

    public virtual void VisitSequence(ASTSequence parserASTSequence) => this.VisitChildrenSequence(parserASTSequence);
    public virtual void VisitChildrenSequence(ASTSequence parserASTSequence) => parserASTSequence.AcceptChildren(this);

    public virtual void VisitToken(ASTToken parserASTToken) { }

    public virtual void VisitPlaceholder(ASTPlaceholder parserASTPlaceHolder) => this.VisitChildrenPlaceholder(parserASTPlaceHolder);
    public virtual void VisitChildrenPlaceholder(ASTPlaceholder parserASTPlaceHolder) => parserASTPlaceHolder.AcceptChildren(this);
}
