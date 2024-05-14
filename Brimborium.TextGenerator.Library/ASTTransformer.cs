namespace Brimborium.TextGenerator;

public interface IASTTransformer<T> {
    ASTSequence VisitSequence(ASTSequence sequence, T state);
    ASTPlaceholder VisitPlaceholder(ASTPlaceholder placeholder, T state);
    ASTSequence VisitChildrenSequence(ASTSequence sequence, T state);
    ASTToken VisitToken(ASTToken token, T state);
    ASTNode VisitConstant(ASTConstant constant, T state);
}

public class ASTTransformer<T> : IASTTransformer<T> {
    public virtual ASTSequence VisitSequence(ASTSequence sequence, T state) {
        return this.VisitChildrenSequence(sequence, state);
    }

    public virtual ASTSequence VisitChildrenSequence(ASTSequence sequence, T state) {
        var result = sequence;
        for (int index = 0; index < sequence.List.Count; index++)
        {
            var item = sequence.List[index];
            var itemResult = item.TransformerAccept(this, state);
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

    public virtual ASTNode VisitConstant(ASTConstant constant, T state) {
        return constant;
    }

    public virtual ASTPlaceholder VisitPlaceholder(ASTPlaceholder placeholder, T state) {
        return this.VisitPlaceholderChildren(placeholder, state);
    }

    public virtual ASTPlaceholder VisitPlaceholderChildren(ASTPlaceholder placeholder, T state) {
        var result = placeholder;

        var nextStartToken = this.VisitToken(placeholder.StartToken, state);
        if (ReferenceEquals(placeholder.StartToken, nextStartToken)) {
            // no change
        } else { 
            result = new ASTPlaceholder(nextStartToken, placeholder.List, placeholder.FinishToken);
        }

        for (int index = 0; index < result.List.Count; index++) {
            var item = result.List[index];
            var itemResult = item.TransformerAccept(this, state);
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


    public virtual ASTToken VisitToken(ASTToken token, T state) {
        return token;
    }
}

public class ASTTransformerReplace<T> : ASTTransformer<T> { 
}