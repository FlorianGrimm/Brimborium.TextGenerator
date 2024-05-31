namespace Brimborium.TextGenerator;

public class Parser {
    //private const string _X = @"""([/][*]\s*)([<][/]?)([^> \t]+)((?:\s+(?:(?:[A-Za-z0-9.:]+)|(?:[""][^""=]+[""]))\s*[=]\s*(?:(?:[A-Za-z0-9.:]+)|(?:[""][^""=]+[""])))*)([/]?[>])(\s*[*][/])""";
    protected const string _RegexInner = """([<][/]?)([^> \t]+)((?:\s+(?:(?:[A-Za-z0-9.:]+)|(?:[""][^""=]+[""]))\s*[=]\s*(?:(?:[A-Za-z0-9.:]+)|(?:[""][^""=]+[""])))*)(?:\s*)([/]?[>])""";
    private readonly Regex _RegexStartEndComment;

    //
    protected Parser(Regex regexStartEndComment) {
        this._RegexStartEndComment = regexStartEndComment;
    }

    private static Regex? _regexCSharp;
    public static Parser CreateForCSharp() {
        //                      1          2        3         4       5           6
        //_regexCSharp ??= new(@"([/][*]\s*)([<][/]?)([^> \t]+)([^/>]*)([/]?[>])(\s*[*][/])", RegexOptions.Compiled);
        _regexCSharp ??= new($@"([/][*]\s*){_RegexInner}(\s*[*][/])");
        return new Parser(_regexCSharp);
    }

    private static Regex? _regexPowershell;
    public static Parser CreateForPowershell() {
        //                          1          2        3         4       5           6
        //_regexPowershell ??= new(@"([<][#]\s*)([<][/]?)([^> \t]+)([^/>]*)([/]?[>])(\s*[#][>])", RegexOptions.Compiled);
        _regexPowershell ??= new($@"([<][#]\s*){_RegexInner}(\s*[#][>])");
        return new Parser(_regexPowershell);
    }

    public TracedValue<ASTSequence> Parse(TracedValue<string> content) {
        var result = this.Parse(content.Value);
        return new TracedValue<ASTSequence>(result, content.ValueIdentity);
    }

    public ASTSequence Parse(string content) {
        var listFlat = this.Scan(content);

        Stack<ASTSequence.Builder> stack = new();
        ASTSequence.Builder current = new();

        foreach (var item in listFlat.ListItem) {
            {
                if (item is ASTStartToken startToken) {
                    stack.Push(current);
                    current = new ASTSequence.Builder();
                    current.ListItem.Add(startToken);
                    continue;
                }
            }
            {
                if (item is ASTFinishToken finishToken) {
                    var list = current.ListItem;
                    if ((list.Count == 0)
                        || list[0] is not ASTStartToken startToken) {
                        throw new InvalidOperationException($"No start tag {finishToken.Tag}");
                    }
                    list.RemoveAt(0);
                    var parserASTPlaceholder = new ASTPlaceholder(
                        startToken.Tag,
                        startToken.ListParameter,
                        list.ToImmutableArray());

                    current = stack.Pop();
                    current.ListItem.Add(parserASTPlaceholder);
                    continue;
                }
            }
            {
                current.ListItem.Add(item);
            }
        }
        return current.Build();
    }

    public ASTSequence Scan(string content) {
        ASTSequence.Builder result = new();
        StringSlice ssContent = new StringSlice(content);

        int indexLast = 0;
        for (var match = _RegexStartEndComment.Match(content); match.Success; match = match.NextMatch()) {
            {
                var contentBefore = ssContent.Substring(indexLast, match.Index - indexLast);

                if (0 < contentBefore.Length) {
                    result.ListItem.Add(new ASTConstant(contentBefore));
                }
            }
            var token = this.ScanMatch(ssContent, match);
            result.ListItem.Add(token);
            indexLast = match.Index + match.Length;
        }
        {
            var contentRest = content.Substring(indexLast);
            if (!string.IsNullOrWhiteSpace(contentRest)) {
                result.ListItem.Add(new ASTConstant(contentRest));
            }
        }
        return result.Build();
    }

    protected virtual ASTNode ScanMatch(StringSlice ssContent, Match match) {
        var prefix = GetStringSliceFromMatch(ssContent, match, 2).Equals("</");
        var tag = GetStringSliceFromMatch(ssContent, match, 3);
        var parameter = GetStringSliceFromMatch(ssContent, match, 4);
        var postfix = GetStringSliceFromMatch(ssContent, match, 5).Equals("/>");
        if (prefix) {
            var finishToken = new ASTFinishToken(tag);
            return finishToken;
        } else {
            var parseParameterResult = ParseParameter(parameter);
            if (!parseParameterResult.Remainder.IsEmpty) {
                throw new InvalidOperationException($"Syntax Error {parameter}");
            }
            if (postfix) {
                var placeholder = new ASTPlaceholder(tag, parseParameterResult.ListParameter.ToImmutableArray(), ImmutableArray<ASTNode>.Empty);
                return placeholder;
            } else {
                var startToken = new ASTStartToken(tag, parseParameterResult.ListParameter.ToImmutableArray());
                return startToken;
            }
        }
    }

    private static readonly char[] _ArrCharWhitespace = new char[] { ' ', '\t', '\n', '\r' };
    private static readonly char[] _ArrCharDoubleQuote = new char[] { '"' };
    private static readonly char[] _ArrCharEqual = new char[] { '=' };
    private static readonly char[] _ArrCharNameEndNotQuoted = new char[] { ' ', '\t', '\n', '\r', '=' };
    private static readonly char[] _ArrCharNameEndDoubleQuoted = new char[] { '"' };
    private static readonly char[] _ArrCharValueEndNotQuoted = new char[] { ' ', '\t', '\n', '\r' };
    private static readonly char[] _ArrCharValueEndDoubleQuoted = new char[] { '"' };

    public static ASTParseParameterResult ParseParameter(StringSlice ssParameter) {
        List<ASTParameter> result = new();
        ssParameter = ssParameter.TrimStart(_ArrCharWhitespace);
        if (ssParameter.IsEmpty) {
            return new(result, ssParameter);
        }
        while (!ssParameter.IsEmpty) {
            bool nameIsQuoted = (_ArrCharDoubleQuote.ReadWhileMatches(ref ssParameter, 1, out var _));
            StringSlice ssName;
            if (nameIsQuoted) {
                if (!_ArrCharNameEndDoubleQuoted.ReadWhileNotMatches(ref ssParameter, 256, out ssName)) {
                    return new(result, ssParameter);
                }
                if (!_ArrCharDoubleQuote.ReadWhileMatches(ref ssParameter, 1, out var _)) {
                    return new(result, ssParameter);
                }
            } else {
                if (!_ArrCharNameEndNotQuoted.ReadWhileNotMatches(ref ssParameter, 256, out ssName)) {
                    return new(result, ssParameter);
                }
            }

            ssParameter = ssParameter.TrimStart(_ArrCharWhitespace);
            if (!(_ArrCharEqual.ReadWhileMatches(ref ssParameter, 1, out var _))) {
                return new(result, ssParameter);
            }
            ssParameter = ssParameter.TrimStart(_ArrCharWhitespace);
            bool valueIsQuoted = (_ArrCharDoubleQuote.ReadWhileMatches(ref ssParameter, 1, out var _));
            StringSlice ssValue;
            if (valueIsQuoted) {
                if (!_ArrCharValueEndDoubleQuoted.ReadWhileNotMatches(ref ssParameter, 4096, out ssValue)) {
                    return new(result, ssParameter);
                }
                if (!_ArrCharDoubleQuote.ReadWhileMatches(ref ssParameter, 1, out var _)) {
                    return new(result, ssParameter);
                }
            } else {
                if (!_ArrCharValueEndNotQuoted.ReadWhileNotMatches(ref ssParameter, 4096, out ssValue)) {
                    return new(result, ssParameter);
                }
            }
            result.Add(new ASTParameter(ssName, ssValue));
            ssParameter = ssParameter.TrimStart(_ArrCharWhitespace);
        }
        return new(result, ssParameter);
    }

    private static StringSlice GetStringSliceFromMatch(StringSlice ssContent, Match match, int groupIndex) {
        return ssContent.Substring(match.Groups[groupIndex].Index, match.Groups[groupIndex].Length);
    }
    // 
}
