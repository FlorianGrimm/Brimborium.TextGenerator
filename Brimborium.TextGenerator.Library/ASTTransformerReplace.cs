namespace Brimborium.TextGenerator;

public class ASTTransformerReplace<T> : ASTTransformerWithStack<T> {
    private readonly Func<ASTTransformerReplace<T>, ASTPlaceholder, T, ASTPlaceholder>? _ReplacePlaceholder;

    public ASTTransformerReplace(
        Func<ASTTransformerReplace<T>, ASTPlaceholder, T, ASTPlaceholder>? replacePlaceholder
    ) {
        this._ReplacePlaceholder = replacePlaceholder;
    }

    public override ASTPlaceholder TransformPlaceholder(ASTPlaceholder placeholder, T state) {
        if (this._ReplacePlaceholder is { } replacePlaceholder) {
            if (replacePlaceholder(this, placeholder, state) is { } next) {
                return next;
            }
        }
        return base.TransformPlaceholder(placeholder, state);
    }

}