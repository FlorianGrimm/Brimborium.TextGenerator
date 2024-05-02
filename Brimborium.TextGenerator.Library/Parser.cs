namespace Brimborium.TextGenerator;

public class Parser {
    private readonly Regex _Regex;

    protected Parser(Regex regex) {
        this._Regex = regex;
    }

    private static Regex? _regexCSharp;
    public static Parser CreateForCSharp() {
        _regexCSharp ??= new(@"([/][*]\s*)([<][/]?)([^> \t]+)([^>]*)([>])(\s*[*][/])", RegexOptions.Compiled);
        return new Parser(_regexCSharp);
    }

    private static Regex? _regexPowershell;
    public static Parser CreateForPowershell() {
        _regexPowershell ??= new(@"([<][#]\s*)([<][/]?)([^> \t]+)([^>]*)([>])(\s*[#][>])", RegexOptions.Compiled);
        return new Parser(_regexPowershell);
    }

    public ASTSequence Parse(string content) {
        var listFlat = this.ParseFlat(content);

        Stack<ASTSequence> stack = new();
        ASTSequence current = new();

        foreach (var item in listFlat) {
            if (item is ASTToken token) {
                if (token.IsStartTag) {
                    stack.Push(current);
                    current = new ASTSequence();
                    current.List.Add(token);
                    continue;
                } else if (token.IsFinishTag) {
                    var list = current.List;
                    if ((list.Count == 0)
                        || !(list[0] is ASTToken startToken)) {
                        throw new InvalidOperationException($"No start tag {token.Tag}");
                    }
                    list.RemoveAt(0);
                    var parserASTPlaceholder = new ASTPlaceholder(
                        startToken,
                        list,
                        token);

                    current = stack.Pop();
                    current.List.Add(parserASTPlaceholder);
                    continue;
                }
            }
            current.Add(item);
        }
        return current;
    }

    public ASTSequence ParseFlat(string content) {
        ASTSequence result = new();
        StringSlice ssContent = new StringSlice(content);

        int indexLast = 0;
        for (var match = _Regex.Match(content); match.Success; match = match.NextMatch()) {
            {
                var contentBefore = ssContent.Substring(indexLast, match.Index - indexLast);

                if (0 < contentBefore.Length) {
                    result.Add(new ASTConstant(contentBefore));
                }
            }
            {
                var complete = GetStringSliceFromMatch(ssContent, match, 0);
                var prefix = GetStringSliceFromMatch(ssContent, match, 2);
                var tag = GetStringSliceFromMatch(ssContent, match, 3);
                var parameter = GetStringSliceFromMatch(ssContent, match, 4);
                var token = ASTToken.Create(complete, prefix, tag, parameter);
                result.Add(token);
            }
            {
                indexLast = match.Index + match.Length;
            }
        }
        {
            var contentRest = content.Substring(indexLast);
            if (!string.IsNullOrWhiteSpace(contentRest)) {
                result.Add(new ASTConstant(contentRest));
            }
        }
        return result;

        static StringSlice GetStringSliceFromMatch(StringSlice ssContent, Match match, int groupIndex) {
            return ssContent.Substring(match.Groups[groupIndex].Index, match.Groups[groupIndex].Length);
        }
    }
    // 
}
