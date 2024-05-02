namespace Brimborium.Text;

/// <summary>
/// StringSplice allows to replace a part of a string.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class StringSplice {
    private readonly StringSlice _Text;
    private readonly Range _Range;
    private List<StringSplice>? _LstPart;
    private StringBuilder? _ReplacementBuilder;
    private string? _ReplacementText;

    public StringSplice(string text) {
        this._Text = new StringSlice(text);
        this._Range = new Range(0, text.Length);
    }

    public StringSplice(StringSlice text) {
        this._Text = text;
        this._Range = new Range(0, text.Length);
    }

    public StringSplice(
        StringSlice text,
        int start,
        int length) {
        this._Text = text;
        var range = new Range(start, start + length);
        if (range.Start.Value < 0 || range.Start.Value > text.Length) { throw new ArgumentOutOfRangeException(nameof(start)); }
        if (range.End.Value < 0 || range.End.Value > text.Length) { throw new ArgumentOutOfRangeException(nameof(length)); }
        this._Range = range;
    }

    public StringSplice(
        StringSlice text,
        Range range) {
        this._Text = text;
        if (range.Start.IsFromEnd || range.End.IsFromEnd) {

            var (rangeOffset, rangeLength) = range.GetOffsetAndLength(text.Length);

            var rangeEnd = rangeOffset + rangeLength;

            range = new Range(rangeOffset, rangeEnd);
        }
        if (range.Start.Value < 0 || range.Start.Value > text.Length) { throw new ArgumentOutOfRangeException(nameof(range)); }
        if (range.End.Value < 0 || range.End.Value > text.Length) { throw new ArgumentOutOfRangeException(nameof(range)); }
        if (range.End.Value < range.Start.Value) { throw new ArgumentOutOfRangeException(nameof(range)); }
        this._Range = range;
    }

    public StringSlice AsSubString() => this._Text.Substring(this._Range);

    public string GetText() => this.AsSubString().ToString();

    public Range Range => _Range;

    public int Length {
        get {
            var (_, length) = this.Range.GetOffsetAndLength(this._Text.Length);
            return length;
        }
    }

    public string? GetReplacementText() { return this._ReplacementText; }

    public void SetReplacementText(string? value) {
        if (this._ReplacementBuilder is not null) {
            throw new InvalidOperationException("Use only one of ReplacmentText and ReplacmentBuilder.");
        }
        if (this._LstPart is not null) {
            throw new InvalidOperationException("Use only one of ReplacmentText and Parts.");
        }
        this._ReplacementText = value;
    }

    public void SetReplacementBuilder(StringBuilder? value) {
        if (this._ReplacementText is not null) {
            throw new InvalidOperationException("Use only one of ReplacmentText and ReplacmentBuilder.");
        }
        if (this._LstPart is not null) {
            throw new InvalidOperationException("Use only one of ReplacmentBuilder and Parts.");
        }

        this._ReplacementBuilder = value;
    }

    public StringBuilder GetReplacementBuilder() {
        if (this._ReplacementText is not null) {
            throw new InvalidOperationException("Use only one of ReplacmentText and ReplacmentBuilder.");
        }
        if (this._LstPart is not null) {
            throw new InvalidOperationException("Use only one of ReplacmentBuilder and Parts.");
        }

        return this._ReplacementBuilder ??= StringBuilderPool.GetStringBuilder();
    }

    public StringSplice[]? GetArrayPart() => this._LstPart?.ToArray();

    public bool IsRangeValid(
         int start,
         int length
    ) {
        if (start < 0) { return false; }
        if (length < 0) { return false; }

        // skip the length is positive
        // if (this.Length < start) { return false; }

        if (this.Length < (start + length)) { return false; }
        return true;
    }

    public StringSplice? CreatePart(int start, int length) {
        if (!this.IsRangeValid(start, length)) {
            return null;
        }
        if (this._LstPart is null) {
            if (this._ReplacementText is not null) {
                throw new InvalidOperationException("Use only one of ReplacmentText and Parts.");
            }
            if (this._ReplacementBuilder is not null) {
                throw new InvalidOperationException("Use only one of ReplacmentBuilder and Parts.");
            }
            this._LstPart = new List<StringSplice>();
        }

        for (int idx = 0; idx < this._LstPart.Count; idx++) {
            var item = this._LstPart[idx];
            if (item.Range.Start.Value < start) {
                if (item.Range.End.Value > start) {
                    return null;
                } else {
                    continue;
                }
            }
            if (item.Range.Start.Value == start) {
                // special case for length==0 add behind the with this start.
                if (length == 0) {
                    while ((idx + 1) < this._LstPart.Count) {
                        if (item.Range.Start.Value == start) {
                            idx++;
                            continue;
                        } else {
                            break;
                        }
                    }
                    {
                        var result = this.Factory(start, length);
                        if (result is not null) {
                            this._LstPart.Insert(idx + 1, result);
                        }
                        return result;
                    }
                }
                return null;
            }
            {
                // within the span?
                if (item.Range.Start.Value < (start + length)) {
                    return null;
                }
                var result = this.Factory(start, length);
                if (result is not null) {
                    this._LstPart.Insert(idx, result);
                }
                return result;
            }
        }
        {
            var result = this.Factory(start, length);
            if (result is not null) {
                this._LstPart.Add(result);
            }
            return result;
        }
    }

    public StringSplice? CreatePart(Range range) {
        var (offset, length) = range.GetOffsetAndLength(this._Text.Length);
        return this.CreatePart(offset, length);
    }

    public StringSplice? GetOrCreatePart(int start, int length) {
        if (!this.IsRangeValid(start, length)) { return null; }

        if (this._LstPart is null) {
            if (this._ReplacementText is not null) {
                throw new InvalidOperationException("Use only one of ReplacmentText and Parts.");
            }
            if (this._ReplacementBuilder is not null) {
                throw new InvalidOperationException("Use only one of ReplacmentBuilder and Parts.");
            }
            this._LstPart = new List<StringSplice>();
        }

        for (int idx = 0; idx < this._LstPart.Count; idx++) {
            var item = this._LstPart[idx];
            if (item.Range.Start.Value < start) {
                continue;
            }
            if (item.Range.Start.Value == start) {
                if (item.Length == length) {
                    return item;
                }
                return null;
            }
            {
                if (item.Range.Start.Value < (start + length)) {
                    return null;
                }
                var result = this.Factory(start, length);
                if (result is not null) {
                    this._LstPart.Insert(idx, result);
                }
                return result;
            }
        }
        {
            var result = this.Factory(start, length);
            if (result is not null) {
                this._LstPart.Add(result);
            }
            return result;
        }
    }

    public IEnumerable<StringSplice> GetLstPartInRange(int start, int length) {
        if (!this.IsRangeValid(start, length)) {
            yield break;
        } else if (this._LstPart is null) {
            yield break;
        } else {
            var end = start + length;
            for (int idx = 0; idx < this._LstPart.Count; idx++) {
                var item = this._LstPart[idx];
                if (start <= item.Range.Start.Value && item.Range.Start.Value < end) {
                    yield return item;
                    continue;
                }
                if (start < item.Range.End.Value && item.Range.End.Value <= end) {
                    yield return item;
                    continue;
                }
                if (item.Range.End.Value < start) {
                    yield break;
                }
            }
        }
    }

    public string BuildReplacement() {
        if (this._LstPart is not null && this._LstPart.Count > 0) {
            var result = StringBuilderPool.GetStringBuilder();
            this.BuildReplacementStringBuilder(result);
            var resultValue = result.ToString();
            return resultValue;
        } else {
            return this.GetText();
        }
    }

    public void BuildReplacementStringBuilder(StringBuilder result) {
        if (this._LstPart is null) {
        } else {
            int posEnd = 0;
            for (int idx = 0; idx < this._LstPart.Count; idx++) {
                var item = this._LstPart[idx];
                if (posEnd < item.Range.Start.Value) {
                    var span = this._Text.Substring(this.Range)
                        .AsSpan()[new Range(posEnd, item.Range.Start.Value)];
                    result.Append(span);
                }

                if (item._ReplacementText is not null) {
                    result.Append(item._ReplacementText!);
                } else if (item._ReplacementBuilder is not null) {
                    result.Append(item._ReplacementBuilder!);
                } else if (item._LstPart is not null) {
                    item.BuildReplacementStringBuilder(result);
                }

                posEnd = item.Range.End.Value;
            }

            // add the tail
            if (posEnd < this.Length) {
                var span = this._Text.Substring(this.Range).AsSpan();
                if (posEnd == 0) {
                    result.Append(span);
                } else {
                    span = span[posEnd..^0];
                    result.Append(span);
                }
            }
        }
    }

    public override string ToString() {
        return this.BuildReplacement();
    }

    // if you want to add any custom data
    protected virtual StringSplice Factory(int start, int length) {
        return new StringSplice(this.AsSubString(), start, length);
    }

    private string GetDebuggerDisplay() {
        var span = this.AsSubString().AsSpan();
        if (span.Length > 32) { span = span[..32]; }
        return $"{span}; #Part:{this._LstPart?.Count};";
    }
}
