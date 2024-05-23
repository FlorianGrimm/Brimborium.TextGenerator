namespace Brimborium.TextGenerator;

public interface IASTTransformer<T> {
    ASTSequence VisitSequence(ASTSequence sequence, T state);
    ASTPlaceholder VisitPlaceholder(ASTPlaceholder placeholder, T state);
    ASTSequence TransformSequence(ASTSequence sequence, T state);
    ASTToken VisitToken(ASTToken token, T state);
    ASTNode VisitConstant(ASTConstant constant, T state);
}

public class ASTTransformer<T> : IASTTransformer<T> {
    // called by the ASTNode
    public virtual ASTSequence VisitSequence(ASTSequence sequence, T state) {
        return this.TransformSequence(sequence, state);
    }

    // override this to change the behavior
    public virtual ASTSequence TransformSequence(ASTSequence sequence, T state)
        => this.TransformSequence(sequence, state);

    // call the children
    public virtual ASTSequence WalkSequence(ASTSequence sequence, T state) {
        var result = sequence;
        for (int index = 0; index < sequence.List.Count; index++)
        {
            var item = sequence.List[index];
            var itemResult = this.TransformSequenceChild(sequence, item, index, state);
            if (ReferenceEquals(item, itemResult)) {
                // no change
                if (ReferenceEquals(sequence, result)) {
                    // no change before
                } else {
                    result.List.Add(itemResult);
                }
            } else {
                if (ReferenceEquals(sequence, result)) {
                    result = new ASTSequence();
                    for (int indexInner = 0; indexInner < index; indexInner++) {
                        result.List.Add(sequence.List[indexInner]);
                    }
                } else { 
                    result.List.Add(itemResult);
                }
            }
        }
        return result;
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
        var result = placeholder;

        var nextStartToken = this.VisitToken(placeholder.StartToken, state);
        if (ReferenceEquals(placeholder.StartToken, nextStartToken)) {
            // no change
        } else { 
            result = new ASTPlaceholder(nextStartToken, placeholder.List, placeholder.FinishToken);
        }

        for (int index = 0; index < result.List.Count; index++) {
            var item = result.List[index];
            
            var itemResult = this.TransformPlaceholderChild(result, item, index, state);
            if (ReferenceEquals(item, itemResult)) {
                // no change
                if (ReferenceEquals(placeholder, result)) {
                    // no change before
                } else {
                    result.List.Add(itemResult);
                }
            } else {
                if (ReferenceEquals(this, result)) {
                    result = new ASTPlaceholder(placeholder.StartToken, placeholder.List[0..index], placeholder.FinishToken);
                } else {
                    result.List.Add(itemResult);
                }
            }
        }

        var nextFinishToken = this.VisitToken(placeholder.FinishToken, state);
        if (ReferenceEquals(placeholder.FinishToken, nextFinishToken)) {
            // no change
        } else {
            result = new ASTPlaceholder(result.StartToken, result.List, nextFinishToken);
        }
        return result;
    }

    // override this to change the behavior
    protected virtual ASTNode TransformPlaceholderChild(ASTPlaceholder placeholder, ASTNode item, int index, T state) 
        => item.TransformerAccept(this, state);

    // called by the ASTNode
    public virtual ASTToken VisitToken(ASTToken token, T state) 
        => this.TransformToken(token, state);

    // override this to change the behavior
    public virtual ASTToken TransformToken(ASTToken token, T state) 
        => token;
}
