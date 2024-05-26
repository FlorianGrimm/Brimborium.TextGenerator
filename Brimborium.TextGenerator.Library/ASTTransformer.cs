namespace Brimborium.TextGenerator;

public interface IASTTransformer<T> {
    ASTSequence VisitSequence(ASTSequence sequence, T state);
    ASTPlaceholder VisitPlaceholder(ASTPlaceholder placeholder, T state);
    ASTSequence TransformSequence(ASTSequence sequence, T state);
    ASTStartToken VisitStartToken(ASTStartToken parserASTToken, T state);
    ASTFinishToken VisitFinishToken(ASTFinishToken parserASTToken, T state);
    ASTNode VisitConstant(ASTConstant constant, T state);
}

public class ASTTransformer<T> : IASTTransformer<T> {
    protected ASTTransformer() { }

    // called by the ASTNode
    public virtual ASTSequence VisitSequence(ASTSequence sequence, T state) {
        return this.TransformSequence(sequence, state);
    }

    // override this to change the behavior
    public virtual ASTSequence TransformSequence(ASTSequence sequence, T state)
        => this.WalkSequence(sequence, state);

    // call the children
    public virtual ASTSequence WalkSequence(ASTSequence sequence, T state) {
        ASTSequence.Builder? builder = null;

        for (int index = 0; index < sequence.ListItem.Length; index++) {
            var item = sequence.ListItem[index];
            var itemResult = this.TransformSequenceChild(sequence, item, index, state);
            if (ReferenceEquals(item, itemResult)) {
                // no change
                if (builder is null) {
                    // no change before
                } else {
                    builder.ListItem.Add(itemResult);
                }
            } else {
                if (builder is null) {
                    builder = sequence.ToBuilder(0..index);
                }
                builder.ListItem.Add(itemResult);
            }
        }
        return ((builder is not null) ? builder.Build() : sequence);
    }

    // override this to change the behavior
    protected virtual ASTNode TransformSequenceChild(ASTSequence sequence, ASTNode item, int index, T state)
        => item.TransformerAccept(this, state);

    // called by the ASTNode
    public virtual ASTNode VisitConstant(ASTConstant constant, T state)
        => this.TransformConstant(constant, state);

    // override this to change the behavior
    public virtual ASTNode TransformConstant(ASTConstant constant, T state)
        => constant;

    // called by the ASTNode
    public virtual ASTPlaceholder VisitPlaceholder(ASTPlaceholder placeholder, T state)
        => this.TransformPlaceholder(placeholder, state);

    // override this to change the behavior
    public virtual ASTPlaceholder TransformPlaceholder(ASTPlaceholder placeholder, T state)
        => this.WalkPlaceholder(placeholder, state);

    public virtual ASTPlaceholder WalkPlaceholder(ASTPlaceholder placeholder, T state) {
        ASTPlaceholder.Builder? builder = null;

        for (int index = 0; index < placeholder.ListItem.Length; index++) {
            var item = placeholder.ListItem[index];

            var itemResult = this.TransformPlaceholderChild(placeholder, item, index, state);
            if (ReferenceEquals(item, itemResult)) {
                // no change
                if (builder is null) {
                    // no change before
                } else {
                    builder.ListItem.Add(itemResult);
                }
            } else {
                if (builder is null) {
                    builder = new ASTPlaceholder.Builder(placeholder.Tag, placeholder.ListParameter, placeholder.ListItem[0..index]);
                }
                builder.ListItem.Add(itemResult);
            }
        }

        if (builder is not null) {
            return builder.Build();
        } else {
            return placeholder;
        }
    }

    // override this to change the behavior
    protected virtual ASTNode TransformPlaceholderChild(ASTPlaceholder placeholder, ASTNode item, int index, T state)
        => item.TransformerAccept(this, state);

    public virtual ASTStartToken VisitStartToken(ASTStartToken startToken, T state)
        => this.TransformStartToken(startToken, state);

    public virtual ASTStartToken TransformStartToken(ASTStartToken startToken, T state)
        => startToken;

    public virtual ASTFinishToken VisitFinishToken(ASTFinishToken finishToken, T state)
        => this.TransformStartToken(finishToken, state);

    public virtual ASTFinishToken TransformStartToken(ASTFinishToken finishToken, T state)
        => finishToken;
}
