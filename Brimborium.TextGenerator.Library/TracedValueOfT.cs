namespace Brimborium.TextGenerator;

public class TracedValue(ulong valueIdentity) {
    public ulong ValueIdentity { get; } = valueIdentity;
    public virtual object? GetValue() => null;
}

public class TracedValue<T>(T value, ulong valueIdentity) : TracedValue(valueIdentity) {
    public T Value { get; } = value;

    public override object? GetValue() => this.Value;
}

public sealed class TracedValueString : TracedValue<string>, IEquatable<TracedValueString> {
    public TracedValueString(string value, ulong valueIdentity)
        : base(value, valueIdentity) {
    }

    public override bool Equals(object? obj)
        => (obj is TracedValueString other)
            && (string.Equals(this.Value, other.Value, StringComparison.Ordinal));

    public bool Equals(TracedValueString? other)
        => (other is not null)
        && (string.Equals(this.Value, other.Value, StringComparison.Ordinal));

    public override int GetHashCode() => this.Value.GetHashCode();

    public override string ToString() => this.Value;
}

public sealed class TracedValueStringQ : TracedValue<string?>, IEquatable<TracedValueStringQ> {
    public TracedValueStringQ(string? value, ulong valueIdentity)
        : base(value, valueIdentity) {
    }

    public override bool Equals(object? obj)
        => (obj is TracedValueStringQ other)
        && string.Equals(this.Value, other.Value, StringComparison.Ordinal);

    public bool Equals(TracedValueStringQ? other)
        => (other is not null)
        && string.Equals(this.Value, other.Value, StringComparison.Ordinal);

    public override int GetHashCode() => (this.Value ?? string.Empty).GetHashCode();
}
