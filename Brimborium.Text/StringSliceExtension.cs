namespace Brimborium.Text;

public static class StringSliceExtension {

    public static StringSlice AsStringSlice(this string value)
        => new StringSlice(value);

    public static StringSlice AsStringSlice(this string value, int pos)
        => new StringSlice(value, new Range(pos, value.Length));

    public static StringSlice AsStringSlice(this string value, int pos, int length) 
        => new StringSlice(value, new Range(pos, pos + length));

    public static StringSlice AsStringSlice(this string value, Range range)
        => new StringSlice(value, range);

    public static StringBuilder Append(this StringBuilder stringBuilder, StringSlice value)
        => stringBuilder.Append(value.AsSpan());

    
    public static bool ReadWordIfMatches(this string word, ref StringSlice slice, StringComparison comparisonType = StringComparison.Ordinal) {
        if (slice.StartsWith(word, comparisonType)) {
            slice = slice.Substring(word.Length);
            return true;
        } else {
            return false;
        }
    }

    public static bool ReadWordIfMatches(this string word, ref StringSlice slice, ref int count, StringComparison comparisonType=StringComparison.Ordinal) {
        if (slice.StartsWith(word, comparisonType)) {
            slice = slice.Substring(word.Length);
            count += word.Length;
            return true;
        } else {
            return false;
        }
    }

    public static bool ReadWhileMatches(
        this char[] matches,
        ref StringSlice slice
        ) {
        var old = slice.Range.Start.Value;
        slice = slice.TrimStart(matches);
        return old != slice.Range.Start.Value;
    }
}
