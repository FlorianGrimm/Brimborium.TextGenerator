namespace Brimborium.TextGenerator;

public interface IASTTransformer<T>
{
    ASTTransformResult<T> VisitPlaceholder(ASTPlaceholder placeholder);
    ASTTransformResult<T> VisitSequence(ASTSequence sequence);
    ASTTransformResult<T> VisitChildrenSequence(ASTSequence sequence);
    ASTTransformResult<T> VisitToken(ASTToken token);
}

public struct ASTTransformResult<T>(
    T? instanceReplace = default,
    List<T>? listReplace = default,
    bool hasChanged = default)
{
    public T? InstanceReplace { get; } = instanceReplace;
    public List<T>? ListReplace { get; } = listReplace;
    public bool HasChanged { get; } = hasChanged;
}

public class ASTTransformer<T> : IASTTransformer<T>
    where T:ASTTransformerState {
    public ASTTransformer()
    {
    }

    public virtual ASTTransformResult<T> VisitPlaceholder(ASTPlaceholder placeholder)
    {
        return default;
    }
    public virtual void HandlePlaceholder(ASTPlaceholder placeholder, ASTTransformResult<T> transformResult)
    {
        if (transformResult.HasChanged) {
            if (transformResult.InstanceReplace is not null && transformResult.ListReplace is null) {
                placeholder.
                placeholder.Replace(transformResult.InstanceReplace);
            } else if (transformResult.InstanceReplace is null && transformResult.ListReplace is not null) {
                placeholder.Replace(transformResult.ListReplace);
            }
        }
    }

    public virtual ASTTransformResult<T> VisitSequence(ASTSequence sequence)
    {
        this.VisitChildrenSequence(sequence);
        return default;
    }

    public virtual ASTTransformResult<T> VisitChildrenSequence(ASTSequence sequence)
    {
        //var result = new ASTTransformResult<T>(default, false);
        foreach (var item in sequence.List)
        {
            //result = item.TransformerAccept(this);
            // TODO: join
            
        }
        return default;
    }

    public virtual ASTTransformResult<T> VisitToken(ASTToken token)
    {
        return default;
    }
}

public class ASTTransformerState {
    public ASTTransformerState(
        ) {
        /*
         
         */
    }
}

//public class ASTTransformerReplace : ASTTransformerReplace<T> { }
public class ASTTransformerReplace<T> : ASTTransformer<T>
    where T : ASTTransformerState {
    private readonly Func<List<ASTPlaceholder>, ASTNode, ASTTransformResult<T>> _Convert;

    public ASTTransformerReplace(
        Func<List<ASTPlaceholder>, ASTNode, ASTTransformResult<T>> convert
        )
    {
        this._Convert = convert;
    }

    public override ASTTransformResult<ASTTransformerState> VisitPlaceholder(ASTPlaceholder placeholder) {
        return base.VisitPlaceholder(placeholder);

        /*
             var oldResults = this._Results;
        this._Results = new List<ASTNode>();
        var astPlaceholder = new ASTPlaceholder(
            parserASTPlaceHolder.StartToken,
            this._Results,
            parserASTPlaceHolder.FinishToken
        );
        this._StackASTPlaceholder.Add(parserASTPlaceHolder);
        foreach (var item in parserASTPlaceHolder.List) {
            item.VisitorAccept(this);
        }
        this._StackASTPlaceholder.RemoveAt(this._StackASTPlaceholder.Count-1);
        this._Results = oldResults;
        this._Results.Add(astPlaceholder);
         */
    }
}