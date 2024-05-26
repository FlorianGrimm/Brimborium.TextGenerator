namespace Brimborium.Text;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct StringSlice : IEquatable<StringSlice> {
    public static StringSlice Empty => new StringSlice(string.Empty);

    // the text is not null
    [System.Text.Json.Serialization.JsonInclude()]
    [System.Text.Json.Serialization.JsonPropertyOrder(0)]
    public readonly string Text = String.Empty;

    // range start and stop are from start
    public readonly Range Range;

    public StringSlice() {
        this.Text = string.Empty;
        this.Range = new Range(0, 0);
    }

    public StringSlice(
       string text
     ) {
        this.Text = text;
        this.Range = new Range(0, text.Length);
    }

    public StringSlice(
        string text,
        Range range) {
        if (range.Start.IsFromEnd || range.End.IsFromEnd) {
            var (offset, length) = range.GetOffsetAndLength(text.Length);
            range = new Range(offset, offset + length);
        }
        if (text.Length < range.Start.Value) { throw new ArgumentOutOfRangeException(nameof(range)); }
        if (text.Length < range.End.Value) { throw new ArgumentOutOfRangeException(nameof(range)); }
        if (range.End.Value < range.Start.Value) { throw new ArgumentOutOfRangeException(nameof(range)); }

        this.Text = text;
        this.Range = range;
    }

    public char this[int index] {
        get {
            var offset = this.Range.Start.Value;
            var end = this.Range.End.Value;
            var length = end - offset;
            if (index < 0) { throw new ArgumentOutOfRangeException(nameof(index)); }
            if (length <= index) { throw new ArgumentOutOfRangeException(nameof(index)); }
            return this.Text[offset + index];
        }
    }

    public StringSlice Substring(int start) {
        if (start < 0) { throw new ArgumentOutOfRangeException(nameof(start)); }
        var thisOffset = this.Range.Start.Value;
        var thisEnd = this.Range.End.Value;
        var thisLength = thisEnd - thisOffset;
        if (thisLength < start) { throw new ArgumentOutOfRangeException(nameof(start)); }
        var nextRange = new Range(thisOffset + start, thisOffset + thisLength);
        return new StringSlice(
            this.Text,
            nextRange
            );
    }

    public StringSlice Substring(int start, int length) {
        if (start < 0) { throw new ArgumentOutOfRangeException(nameof(start)); }
        if (length < 0) { throw new ArgumentOutOfRangeException(nameof(length)); }

        var thisOffset = this.Range.Start.Value;
        var thisEnd = this.Range.End.Value;
        var thisLength = thisEnd - thisOffset;

        if (thisLength < length) { throw new ArgumentOutOfRangeException(nameof(length)); }
        var nextRange = new Range(thisOffset + start, thisOffset + start + length);
        return new StringSlice(
            this.Text,
            nextRange
            );
    }

    public StringSlice Substring(Range range) {
        // shortcut because range is from start
        var thisOffset = this.Range.Start.Value;
        var thisEnd = this.Range.End.Value;
        var thisLength = thisEnd - thisOffset;

        // range can be from end so I use GetOffsetAndLength
        var (rangeOffset, rangeLength) = range.GetOffsetAndLength(thisLength);

        var nextRange = new Range(thisOffset + rangeOffset, thisOffset + rangeOffset + rangeLength);
        if (nextRange.Start.Value > nextRange.End.Value) { throw new ArgumentOutOfRangeException(nameof(range)); }
        if (thisEnd < nextRange.Start.Value) { throw new ArgumentOutOfRangeException(nameof(range)); }
        if (thisEnd < nextRange.End.Value) { throw new ArgumentOutOfRangeException(nameof(range)); }

        return new StringSlice(
            this.Text,
            nextRange
            );
    }

    public StringSliceState GetTextAndRange()
        => new StringSliceState(this.Text, this.Range);

    public void Deconstruct(out string text, out Range range) {
        text = this.Text;
        range = this.Range;
    }

    [System.Text.Json.Serialization.JsonIgnore()]
    public int Length {
        get {
            // shortcut because this.Range is from start
            var offset = this.Range.Start.Value;
            var end = this.Range.End.Value;
            var length = end - offset;

            return length;
        }
    }

    [System.Text.Json.Serialization.JsonIgnore()]
    public bool IsEmpty {
        get {
            // shortcut because this.Range is from start
            var offset = this.Range.Start.Value;
            var end = this.Range.End.Value;
            return offset == end;
        }
    }

    public override string ToString() {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;

        if (length == 0) { return string.Empty; }
        if (offset == 0 && length == this.Text.Length) {
            return this.Text;
        } else {
            return this.Text.Substring(offset, length);
        }
    }

    public ReadOnlySpan<char> AsSpan() {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;

        return this.Text.AsSpan(offset, length);
    }

    public ReadOnlyMemory<char> AsMemory()
        => this.Text.AsMemory(this.Range);

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    private string GetDebuggerDisplay() {
        if (this.Text is null) { return "null"; }
        if (this.Length < 32) {
            return this.Text[this.Range];
        } else {
            return this.Text[new Range(this.Range.Start, this.Range.Start.Value + 32)];
        }
    }

    public bool IsNullOrEmpty() {
        return this.Text is null || this.Length == 0;
    }

    public bool IsNullOrWhiteSpace() {
        if (this.Text == null) { return true; }

        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;

        return this.Text.AsSpan(offset, length).IsWhiteSpace();
    }

    public int IndexOf(char search) {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;
        return this.Text.AsSpan(offset, length).IndexOf(search);
    }

    public int IndexOf(char search, Range range) {
        var thisOffset = this.Range.Start.Value;
        var thisEnd = this.Range.End.Value;
        var thisLength = thisEnd - thisOffset;

        var (offset, length) = range.GetOffsetAndLength(thisLength);
        var result = this.Text.AsSpan(offset + thisOffset, length).IndexOf(search);
        if (result < 0) {
            return -1;
        } else {
            return result + offset;
        }
    }

    public int IndexOfAny(char[] search) {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;
        return this.Text.AsSpan(offset, length).IndexOfAny(search);
    }

    public int IndexOfAny(char[] search, Range range) {
        var thisOffset = this.Range.Start.Value;
        var thisEnd = this.Range.End.Value;
        var thisLength = thisEnd - thisOffset;

        var (offset, length) = range.GetOffsetAndLength(thisLength);
        var result = this.Text.AsSpan(offset + thisOffset, length).IndexOfAny(search);
        if (result < 0) {
            return -1;
        } else {
            return result + offset;
        }
    }

    public bool Contains(char value) {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;
        return this.Text.AsSpan(offset, length).Contains(value);
    }

    public bool Contains(ReadOnlySpan<char> value, StringComparison comparisonType = StringComparison.Ordinal) {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;
        return this.Text.AsSpan(offset, length).Contains(value, comparisonType);
    }

    public bool StartsWith(string search, StringComparison comparisonType = StringComparison.Ordinal) => this.AsSpan().StartsWith(search, comparisonType);

    public bool StartsWith(StringSlice search, StringComparison comparisonType = StringComparison.Ordinal) => this.AsSpan().StartsWith(search.AsSpan(), comparisonType);

    public bool StartsWith(ReadOnlySpan<char> search, StringComparison comparisonType = StringComparison.Ordinal) => this.AsSpan().StartsWith(search, comparisonType);

    public StringSlice TrimStart() {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;
        var span = this.Text.AsSpan(offset, length).TrimStart();
        var nextOffset = end - span.Length;
        return new StringSlice(this.Text, nextOffset..end);
    }

    public StringSlice TrimStart(char[] chars) {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;
        var span = this.Text.AsSpan(offset, length).TrimStart(chars);
        var nextOffset = end - span.Length;
        return new StringSlice(this.Text, nextOffset..end);
    }

    public StringSlice TrimWhile(Func<char, int, int> decide) {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;

        if (length == 0) { return this; }
        for (int idx = offset; idx < end; idx++) {
            var decision = decide(this.Text[idx], idx - offset);
            if (decision == 0) {
                continue;
            } else if (decision < 0) {
                return new StringSlice(this.Text, idx..idx);
            } else if (decision > 0) {
                if (idx == offset) {
                    return this;
                } else {
                    return new StringSlice(this.Text, idx..end);
                }
            }
        }
        return new StringSlice(this.Text, end..end);
    }

    public StringSlice TrimEnd() {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;
        var span = this.Text.AsSpan(offset, length).TrimEnd();
        var nextEnd = offset + span.Length;
        return new StringSlice(this.Text, offset..nextEnd);
    }

    public StringSlice TrimEnd(char[] chars) {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;
        var span = this.Text.AsSpan(offset, length).TrimEnd(chars);
        var nextEnd = offset + span.Length;
        return new StringSlice(this.Text, offset..nextEnd);
    }

    public StringSlice Trim() {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;
        var span = this.Text.AsSpan(offset, length).TrimEnd();
        var nextEnd = offset + span.Length;
        span = span.TrimStart();
        var nextOffset = nextEnd - span.Length;
        return new StringSlice(this.Text, nextOffset..nextEnd);
    }

    public StringSlice Trim(char[] chars) {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;
        var span = this.Text.AsSpan(offset, length).TrimEnd(chars);
        var nextEnd = offset + span.Length;
        span = span.TrimStart(chars);
        var nextOffset = nextEnd - span.Length;
        return new StringSlice(this.Text, nextOffset..nextEnd);
    }

    public SplitInto SplitInto(char[] arraySeparator, char[]? arrayStop = default) {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;
        if (length == 0) {
            return new SplitInto(this, this);
        }

        int posStop;
        if (arrayStop is not null && arrayStop.Length > 0) {
            posStop = this.IndexOfAny(arrayStop);
            if (posStop >= 0) {
                end = offset + posStop;
            }
        }

        int posSep = this.IndexOfAny(arraySeparator, 0..(end - offset));
        if (posSep < 0) {
            return new SplitInto(
                new StringSlice(this.Text, new Range(offset, end)),
                new StringSlice(this.Text, end..end));
        } else {
            var endSep = offset + posSep;
            return new SplitInto(
                new StringSlice(this.Text, new Range(offset, endSep)),
                (new StringSlice(this.Text, endSep..end)).TrimStart(arraySeparator)
                );
        }
    }

    /// <summary>
    /// Split the string into two parts, 
    /// the first part is the string until the decide function returns not 0.
    /// The second part is the rest of the string if <paramref name="decide"/> returns greater than 0.
    /// The second part is empty if <paramref name="decide"/> returns less than 0.
    /// </summary>
    /// <param name="decide">a callback to decide to 
    ///     0 continue,
    ///     greater 0 to return 2 parts the part Found (until before) and the Tail,
    ///     less 0 to return 2 parts the part Found (until before) and an empty Tail.</param>
    /// <returns>2 SubStrings the Found and the Tail.</returns>
    public SplitInto SplitIntoWhile(Func<char, int, int> decide) {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;
        if (length == 0) { return new SplitInto(this, this); }

        for (int idx = offset; idx < end; idx++) {
            var result = decide(this.Text[idx], idx);
            if (result == 0) {
                continue;
            } else if (result > 0) {
                return new SplitInto(
                    new StringSlice(this.Text, new Range(offset, idx)),
                    new StringSlice(this.Text, idx..end));
            } else if (result < 0) {
                return new SplitInto(
                    new StringSlice(this.Text, new Range(offset, idx)),
                    new StringSlice(this.Text, idx..idx));
            }
        }
        return new SplitInto(this, new StringSlice(this.Text, end..end));
    }

    public override bool Equals([NotNullWhen(true)] object? obj) {
        if (obj is StringSlice other) { return this.Equals(other); }
        if (obj is string text) { return this.Equals(text.AsSpan(), StringComparison.Ordinal); }
        return false;
    }

    public bool Equals(StringSlice other) {
        var t = this.AsSpan();
        var o = other.AsSpan();
        if (t.Length != o.Length) { return false; }
        return t.StartsWith(o, StringComparison.Ordinal);
    }

    public bool Equals(string other, StringComparison comparisonType = StringComparison.Ordinal) {
        var t = this.AsSpan();
        var o = other.AsSpan();
        if (t.Length != o.Length) { return false; }
        return t.StartsWith(o, comparisonType);
    }

    public bool Equals(StringSlice other, StringComparison comparisonType = StringComparison.Ordinal) {
        if (ReferenceEquals(this.Text, other.Text)) {
            return this.Range.Equals(other.Range);
        } else {
            var t = this.AsSpan();
            var o = other.AsSpan();
            if (t.Length != o.Length) { return false; }
            return t.StartsWith(o, comparisonType);
        }
    }

    public bool Equals(ReadOnlySpan<char> other, StringComparison comparisonType = StringComparison.Ordinal) {
        var t = this.AsSpan();
        if (t.Length != other.Length) { return false; }
        return t.StartsWith(other, comparisonType);
    }

    public override int GetHashCode() => string.GetHashCode(this.AsSpan());

    public CharEnumerator GetEnumerator() => new CharEnumerator(this.AsSpan());

    public StringSlice Replace(char from, char to) {
        if (0 == this.Length) { return this; }
        if (from == to) { return this; }
        if (!this.Contains(from)) { return this; }
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;
        var result = new char[length];
        for (int idx = offset; idx < end; idx++) {
            var c = this.Text[idx];
            result[idx - offset] = c == from ? to : c;
        }
        return new StringSlice(new string(result));
    }

    public StringSlice ReadWhile(Func<char, int, bool> predicate) {
        var offset = this.Range.Start.Value;
        var end = this.Range.End.Value;
        var length = end - offset;
        var found = false;
        for (int idx = offset; idx < end; idx++) {
            if (!predicate(this.Text[idx], idx)) {
                return new StringSlice(this.Text, new Range(offset, idx));
            }
            found = true;
        }
        if (found) {
            return this;
        } else {
            return new StringSlice(this.Text, new Range(offset, offset));
        }
    }

    public ref struct CharEnumerator {
        private readonly ReadOnlySpan<char> _Span;
        private int _Index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal CharEnumerator(ReadOnlySpan<char> span) {
            _Span = span;
            _Index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() {
            var index = _Index + 1;
            if (index < _Span.Length) {
                _Index = index;
                return true;
            }

            return false;
        }

        public ref readonly char Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref this._Span[_Index];
        }
    }

    public static implicit operator StringSlice(string? value)
        => new StringSlice(value ?? string.Empty);

    public static bool operator ==(StringSlice left, StringSlice right) => left.Equals(right, StringComparison.Ordinal);
    public static bool operator !=(StringSlice left, StringSlice right) => !(left.Equals(right, StringComparison.Ordinal));
}

public readonly record struct SplitInto(StringSlice Found, StringSlice Tail);
public record struct StringSliceState(string Text, Range Range);