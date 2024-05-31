namespace Brimborium.TextGenerator;

public interface IPlaceholderTransformer {
    bool TransformPlaceholder(ASTPlaceholder placeholder, TransformerState state, ASTPlaceholder.Builder builder);
}

public interface IValueTransformer {
    bool TransformValue(ASTPlaceholder placeholder, TransformerState state, object? value, ASTPlaceholder.Builder builder);
}

public interface IValueASTTransformer {
    bool TransformValueAST(object? value, ASTPlaceholder.Builder builder);
}

public class TransformerState(
    TransformerContext transformerContext,
    ImmutableArray<ScopedNamedValue>? listNamedValue,
    TransformerState? parent
    ) {
    public TransformerContext TransformerContext { get; } = transformerContext;
    public ImmutableArray<ScopedNamedValue> ListNamedValue { get; } = listNamedValue ?? ImmutableArray<ScopedNamedValue>.Empty;
    public TransformerState? Parent { get; } = parent;

    public virtual TransformerState With(ImmutableArray<ScopedNamedValue>? values, TransformerState? parent)
        => new TransformerState(this.TransformerContext, values ?? this.ListNamedValue, parent ?? this.Parent);

    public virtual TransformerState WithValues(ImmutableArray<ScopedNamedValue> values)
        => new TransformerState(this.TransformerContext, values, this.Parent);

    public bool TryGetValue(StringSlice key, [MaybeNullWhen(false)] out ScopedNamedValue result) {
        foreach (var item in this.ListNamedValue) {
            if (item.Name.Equals(key)) {
                result = item;
                return true;
            }
        }
        {
            result = default;
            return false;
        }
    }
}

public record ScopedNamedValue(StringSlice Name, object Value) : IWithNameStringSlice;

public class TransformerContext
    : IPlaceholderTransformer, IValueTransformer, IValueASTTransformer {
    public TransformerContext() {
        this.PlaceholderTransformer = new DispatchPlaceholderTransformer();
        this.ValueTransformer = new DispatchValueTransformer();
        this.ValueASTTransformer = new DispatchValueASTTransformer();
    }

    public DispatchPlaceholderTransformer PlaceholderTransformer;
    public DispatchValueTransformer ValueTransformer;
    public DispatchValueASTTransformer ValueASTTransformer;

    public void AddPlaceholderTransformer(IPlaceholderTransformer placeholderTransformer) {
        this.PlaceholderTransformer.AddPlaceholderTransformer(placeholderTransformer);
    }

    public void AddValueTransformer(IValueTransformer valueTransformer) {
        this.ValueTransformer.AddValueTransformer(valueTransformer);
    }

    public void AddValueASTTransformer(IValueASTTransformer valueASTTransformer) {
        this.ValueASTTransformer.AddValueASTTransformer(valueASTTransformer);
    }

    public bool TransformPlaceholder(ASTPlaceholder placeholder, TransformerState state, ASTPlaceholder.Builder builder)
        => this.PlaceholderTransformer.TransformPlaceholder(placeholder, state, builder);

    public bool TransformValue(ASTPlaceholder placeholder, TransformerState state, object? value, ASTPlaceholder.Builder builder)
        => this.ValueTransformer.TransformValue(placeholder, state, value, builder);

    public bool TransformValueAST(object? value, ASTPlaceholder.Builder builder)
        => this.ValueASTTransformer.TransformValueAST(value, builder);
}

public sealed class DispatchValueTransformer
    : IValueTransformer {

    private static DispatchValueTransformer? _Instance;
    public static DispatchValueTransformer Instance => _Instance ??= new();

    public DispatchValueTransformer() { }

    public ImmutableArray<IValueTransformer> ListValueTransformer = ImmutableArray<IValueTransformer>.Empty;

    public void AddValueTransformer(IValueTransformer valueTransformer) {
        this.ListValueTransformer = this.ListValueTransformer.Add(valueTransformer);
    }

    public bool TransformValue(ASTPlaceholder placeholder, TransformerState state, object? value, ASTPlaceholder.Builder builder) {
        {

            var listValueTransformer = this.ListValueTransformer;
            for (int index= listValueTransformer.Length - 1; 0 <= index; index--) {
                var itemValueTransformer = listValueTransformer[index];
                if (itemValueTransformer.TransformValue(placeholder, state, value, builder)) {
                    return true;
                }
            }
        }
        {
            return false;
        }
    }
}

public sealed class DispatchValueASTTransformer
    : IValueASTTransformer {

    private static DispatchValueTransformer? _Instance;
    public static DispatchValueTransformer Instance => _Instance ??= new();

    public DispatchValueASTTransformer() { }

    public ImmutableArray<IValueASTTransformer> ListValueASTTransformer = ImmutableArray<IValueASTTransformer>.Empty;

    public void AddValueASTTransformer(IValueASTTransformer valueTransformer) {
        this.ListValueASTTransformer = this.ListValueASTTransformer.Add(valueTransformer);
    }

    public bool TransformValueAST(object? value, ASTPlaceholder.Builder builder) {
        if (value is null) {
            return false;
        }
        {
            if (value is ASTNode astNode) {
                builder.ListItem.Add(astNode);
                return true;
            }
        }
        {
            var listValueASTTransformer = this.ListValueASTTransformer;
            for (int index = listValueASTTransformer.Length - 1; 0 <= index; index--) {
                var itemValueTransformer = listValueASTTransformer[index];
                if (itemValueTransformer.TransformValueAST(value, builder)) {
                    return true;
                }
            }
        }
        return false;
    }
}

public sealed class DispatchPlaceholderTransformer
    : IPlaceholderTransformer {
    private static DispatchPlaceholderTransformer? _Instance;
    public static DispatchPlaceholderTransformer Instance => _Instance ??= new();
    public DispatchPlaceholderTransformer() { }

    public ImmutableArray<IPlaceholderTransformer> ListPlaceholderTransformer = ImmutableArray<IPlaceholderTransformer>.Empty;

    public void AddPlaceholderTransformer(IPlaceholderTransformer placeholderTransformer) {
        this.ListPlaceholderTransformer = ListPlaceholderTransformer.Add(placeholderTransformer);
    }
    public bool TransformPlaceholder(ASTPlaceholder placeholder, TransformerState state, ASTPlaceholder.Builder builder) {
        {
            var listPlaceholderTransformer = this.ListPlaceholderTransformer;
            for (int index = listPlaceholderTransformer.Length - 1; 0 <= index; index--) {
                var itemPlaceholderTransformer = listPlaceholderTransformer[index];
                if (itemPlaceholderTransformer.TransformPlaceholder(placeholder, state, builder)) {
                    return true;
                }
            }
        }
        {
            return false;
        }
    }
}

public class CoreReplacePlaceholderTransformer : IPlaceholderTransformer {
    public bool TransformPlaceholder(ASTPlaceholder placeholder, TransformerState state, ASTPlaceholder.Builder builder) {

        if (placeholder.Tag == "core:replace") {
            if (placeholder.ListParameter.FirstOrDefault(placeholder => placeholder.Name == "name") is { } parameterName) {
                if (state.TryGetValue(parameterName.Value, out var value)) {
                    if (state.TransformerContext.TransformValue(placeholder, state, value.Value, builder)) {
                        return true;
                    }
                }
            }
        }
        {
            if (state.TryGetValue(placeholder.Tag, out var value)) {
                if (state.TransformerContext.TransformValue(placeholder, state, value.Value, builder)) {
                    return true;
                }
            }
        }
        {
            return false;
        }
    }
}

public class BaseValueTransformer : IValueTransformer {
    public BaseValueTransformer() {
    }

    public virtual bool TransformValue(ASTPlaceholder placeholder, TransformerState state, object? value, ASTPlaceholder.Builder builder) {
        return false;
    }
}

public sealed class CoreValueTransformer : BaseValueTransformer {
    public override bool TransformValue(ASTPlaceholder placeholder, TransformerState state, object? value, ASTPlaceholder.Builder builder) {
        {
            ulong valueIdentity = 0;
            if (value is TracedValue tracedValue) {
                value = tracedValue.GetValue();
                valueIdentity = tracedValue.ValueIdentity;
            }
            if (value is string stringValue) {
                if (state.TransformerContext.TransformValueAST(value, builder)) {
                    return true;
                }
            } else if (value is IEnumerable enumerableValue) {
                int index = 0;
                var seperatorPlaceholder = placeholder.ListParameter.FirstOrDefault(placeholder => placeholder.Name == "seperator");

                foreach (var itemValue in enumerableValue) {
                    if (0 < index) {
                        if (seperatorPlaceholder is not null) {
                            builder.ListItem.Add(new ASTConstant(seperatorPlaceholder.Value));
                        }
                    }

                    if (state.TransformerContext.TransformValue(placeholder, state, itemValue, builder)) {
                        // OK
                    } else {
                        state.TransformerContext.TransformValueAST(itemValue, builder);
                    }

                    index++;
                }
                return true;
            } else {
                if (state.TransformerContext.ValueTransformer.TransformValue(placeholder, state, value, builder)) {
                    return true;
                } else if (state.TransformerContext.TransformValueAST(value, builder)) {
                    return true;
                }
            }
        }
        return false;
    }
}

public class BaseValueASTTransformer : IValueASTTransformer {
    public virtual bool TransformValueAST(object? value, ASTPlaceholder.Builder builder) {
        return false;
    }
}

public sealed class CoreValueASTTransformer : BaseValueASTTransformer {
    public override bool TransformValueAST(object? value, ASTPlaceholder.Builder builder) {
        if (value is null) {
            //
            return true;
        } else if (value is ASTNode astNodeValue) {
            builder.ListItem.Add(astNodeValue);
            return true;
        } else if (value is StringSlice stringSliceValue) {
            builder.ListItem.Add(new ASTConstant(stringSliceValue));
            return true;
        } else if (value is string stringValue) {
            builder.ListItem.Add(new ASTConstant(stringValue));
            return true;
        } else {
            /* builder.ListItem.Add(new ASTConstant(value.ToString())); */
            return false;
        }
    }
}

public sealed class CoreASTReplaceTransformer : ASTTransformer<TransformerState> {
    public CoreASTReplaceTransformer() { }

    public override ASTPlaceholder TransformPlaceholder(ASTPlaceholder placeholder, TransformerState state) {
        {
            var builder = placeholder.ToBuilder();
            builder.ListItem = [];
            if (state.TransformerContext.PlaceholderTransformer.TransformPlaceholder(placeholder, state, builder)) {
                return builder.Build();
            }
        }
        {
            return base.TransformPlaceholder(placeholder, state);
        }
    }
}
