﻿namespace Brimborium.TextGenerator;

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

        for (int index = 0; index < sequence.List.Length; index++) {
            var item = sequence.List[index];
            var itemResult = this.TransformSequenceChild(sequence, item, index, state);
            if (ReferenceEquals(item, itemResult)) {
                // no change
                if (builder is null) {
                    // no change before
                } else {
                    builder.List.Add(itemResult);
                }
            } else {
                if (builder is null) {
                    builder = sequence.ToBuilder(0..index);
                    builder.List.Add(itemResult);
                } else {
                    builder.List.Add(itemResult);
                }
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
        var result = placeholder;

        var nextStartToken = this.VisitStartToken(placeholder.StartToken, state);
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
                    List<ASTNode> list = new();
                    if (0 < index) {
                        list.AddRange(placeholder.List[0..index]);
                    }
                    list.Add(itemResult);
                    result = new ASTPlaceholder(placeholder.StartToken, list, placeholder.FinishToken);
                } else {
                    result.List.Add(itemResult);
                }
            }
        }

        var nextFinishToken = this.VisitFinishToken(placeholder.FinishToken, state);
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

    public virtual ASTStartToken VisitStartToken(ASTStartToken startToken, T state)
        => this.TransformStartToken(startToken, state);

    public virtual ASTStartToken TransformStartToken(ASTStartToken startToken, T state)
        => startToken;

    public virtual ASTFinishToken VisitFinishToken(ASTFinishToken finishToken, T state)
        => this.TransformStartToken(finishToken, state);

    public virtual ASTFinishToken TransformStartToken(ASTFinishToken finishToken, T state)
        => finishToken;
}
