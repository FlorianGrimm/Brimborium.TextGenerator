namespace Brimborium.TextGenerator;

public interface IWithNameStringSlice {
    StringSlice Name { get; }
}

public sealed record class ASTParameter(
    StringSlice Name,
    StringSlice Value
    ) : IWithNameStringSlice;

public record struct ASTParseParameterResult(
    List<ASTParameter> ListParameter,
    StringSlice Remainder
    );

public interface IWithListParameter {
    ImmutableArray<ASTParameter> ListParameter { get; }
}

public static class ASTParameterExtension {
    public static bool TryGetFirstByName<T>(this List<T> self, StringSlice name, [MaybeNullWhen(false)] out T result)
        where T : class, IWithNameStringSlice {
        foreach (var item in self) {
            if (item.Name.Equals(name)) {
                result = item;
                return true;
            }
        }
        {
            result = default;
            return false;
        }
    }

    public static bool TryGetFirstByName<T>(this ImmutableArray<T> self, StringSlice name, [MaybeNullWhen(false)] out T result)
        where T : class, IWithNameStringSlice {
        foreach (var item in self) {
            if (item.Name.Equals(name)) {
                result = item;
                return true;
            }
        }
        {
            result = default;
            return false;
        }
    }

    public static List<T> FilterByName<T>(this List<T> self, StringSlice name)
        where T : class, IWithNameStringSlice {
        List<T> result = [];
        foreach (var item in self) {
            if (item.Name.Equals(name)) {
                result.Add(item);
            }
        }
        return result;
    }

    public static List<T> FilterByName<T>(this ImmutableArray<T> self, StringSlice name)
        where T : class, IWithNameStringSlice {
        List<T> result = [];
        foreach (var item in self) {
            if (item.Name.Equals(name)) {
                result.Add(item);
            }
        }
        return result;
    }
}
