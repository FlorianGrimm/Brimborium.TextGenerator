using Brimborium.TextGenerator.Contracts;

namespace Brimborium.TextGenerator;

public class Parser {
    private readonly Regex _RegexStartEndComment;

    //
    protected Parser(Regex regexStartEndComment) {
        this._RegexStartEndComment = regexStartEndComment;
    }

    private static Regex? _regexCSharp;
    public static Parser CreateForCSharp() {
        //                      1          2        3         4      5    6 
        _regexCSharp ??= new(@"([/][*]\s*)([<][/]?)([^> \t]+)([^>]*)([>])(\s*[*][/])", RegexOptions.Compiled);
        return new Parser(_regexCSharp);
    }

    private static Regex? _regexPowershell;
    public static Parser CreateForPowershell() {
        //                          1          2        3         4      5    6 
        _regexPowershell ??= new(@"([<][#]\s*)([<][/]?)([^> \t]+)([^>]*)([>])(\s*[#][>])", RegexOptions.Compiled);
        return new Parser(_regexPowershell);
    }

    public ASTSequence Parse(string content) {
        var listFlat = this.Scan(content);

        Stack<ASTSequence> stack = new();
        ASTSequence current = new();

        foreach (var item in listFlat) {
            {
                if (item is ASTStartToken startToken) {
                    stack.Push(current);
                    current = new ASTSequence();
                    current.List.Add(startToken);
                    continue;
                }
            }
            {
                if (item is ASTFinishToken finishToken) {
                    var list = current.List;
                    if ((list.Count == 0)
                        || !(list[0] is ASTStartToken startToken)) {
                        throw new InvalidOperationException($"No start tag {finishToken.Tag}");
                    }
                    list.RemoveAt(0);
                    var parserASTPlaceholder = new ASTPlaceholder(
                        startToken,
                        list,
                        finishToken);

                    current = stack.Pop();
                    current.List.Add(parserASTPlaceholder);
                    continue;
                }
            }
            {
                current.Add(item);
            }
        }
        return current;
    }

    public ASTSequence Scan(string content) {
        ASTSequence result = new();
        StringSlice ssContent = new StringSlice(content);

        int indexLast = 0;
        for (var match = _RegexStartEndComment.Match(content); match.Success; match = match.NextMatch()) {
            {
                var contentBefore = ssContent.Substring(indexLast, match.Index - indexLast);

                if (0 < contentBefore.Length) {
                    result.Add(new ASTConstant(contentBefore));
                }
            }
            var token = this.ScanMatch(ssContent, match);
            result.Add(token);
            indexLast = match.Index + match.Length;
        }
        {
            var contentRest = content.Substring(indexLast);
            if (!string.IsNullOrWhiteSpace(contentRest)) {
                result.Add(new ASTConstant(contentRest));
            }
        }
        return result;
    }

    private ASTNode ScanMatch(StringSlice ssContent, Match match) {
        //                        1          2        3         4      5    6 
        //_regexCSharp ??= new(@"([/][*]\s*)([<][/]?)([^> \t]+)([^>]*)([>])(\s*[*][/])", RegexOptions.Compiled);
        //var complete = GetStringSliceFromMatch(ssContent, match, 0);
        var prefix = GetStringSliceFromMatch(ssContent, match, 2);
        var tag = GetStringSliceFromMatch(ssContent, match, 3);
        var parameter = GetStringSliceFromMatch(ssContent, match, 4);
        if (prefix.Equals("</")) {
            var finishToken = new ASTFinishToken(tag);
            return finishToken;
        } else {
            var parseParameterResult = ParseParameter(parameter);
            if (!parseParameterResult.Remainder.IsEmpty) {
                throw new InvalidOperationException($"Syntax Error {parameter}");
            }
            var startToken = new ASTStartToken(tag, parseParameterResult.ListParameter.ToImmutableList());
            return startToken;
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

public record struct ASTParseParameterResult(
    List<ASTParameter> ListParameter,
    StringSlice Remainder
    );

public record class ASTParameter(StringSlice Name, StringSlice Value);