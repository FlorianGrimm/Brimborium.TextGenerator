namespace Brimborium.TextGenerator;

public class CoreTransformerTest {
     [Fact]
    public void CoreTransformer01Simple() {
        var input = """/* <a seperator=","> *//* </a> */""";
        TransformerContext transformerContext = new();
        transformerContext.AddPlaceholderTransformer(new CoreReplacePlaceholderTransformer());
        transformerContext.AddValueTransformer(new CoreValueTransformer());
        transformerContext.AddValueASTTransformer(new CoreValueASTTransformer());
        var state = new TransformerState(
            transformerContext,
            ImmutableArray.Create(new ScopedNamedValue("a", new List<string> { "a", "b" })),
            null);
        var output = """/* <a seperator=","> */a,b/* </a> */""";
        var parser = Parser.CreateForCSharp();
        var astInput = parser.Parse(input);
        var coreASTReplaceTransformer = new CoreASTReplaceTransformer();
        var astOutput = astInput.TransformerAccept(coreASTReplaceTransformer, state);
        var act = ASTTreeToString.GetAsString(astOutput);
        Assert.Equal(output, act);
    }

    [Fact]
    public void CoreTransformer02Seperator() {
        var input = """/* <a seperator="/"> *//* </a> */""";
        TransformerContext transformerContext = new();
        transformerContext.AddPlaceholderTransformer(new CoreReplacePlaceholderTransformer());
        transformerContext.AddValueTransformer(new CoreValueTransformer());
        transformerContext.AddValueASTTransformer(new CoreValueASTTransformer());
        var state = new TransformerState(
            transformerContext,
            ImmutableArray.Create(new ScopedNamedValue("a", new List<string> { "a", "b" })),
            null);
        var output = """/* <a seperator="/"> */a/b/* </a> */""";
        var parser = Parser.CreateForCSharp();
        var astInput = parser.Parse(input);
        var astOutput = astInput.TransformerAccept(new CoreASTReplaceTransformer(), state);
        var act = ASTTreeToString.GetAsString(astOutput);
        Assert.Equal(output, act);
    }
    [Fact]
    public void CoreTransformer03TransformerTwice() {
        var input = """111(/*<core:replace name="ab" seperator=",">*/xxx/*</core:replace>*/)222""";
        TransformerContext transformerContext = new();
        transformerContext.AddPlaceholderTransformer(new CoreReplacePlaceholderTransformer());
        transformerContext.AddValueTransformer(new CoreValueTransformer());
        transformerContext.AddValueASTTransformer(new CoreValueASTTransformer());
        var state = new TransformerState(
            transformerContext,
            ImmutableArray.Create(
                new ScopedNamedValue("ab", new List<string> { "a", "b" }),
                new ScopedNamedValue("cd", new List<string> { "c", "d" })
                ),
            null);
        var output = """111(/* <core:replace name="ab" seperator=","> */a,b/* </core:replace> */)222""";
        var parser = Parser.CreateForCSharp();
        var astInput = parser.Parse(input);
        var astOutput = astInput.TransformerAccept(new CoreASTReplaceTransformer(), state);
        var act = ASTTreeToString.GetAsString(astOutput);
        Assert.Equal(output, act);
        var astOutput2 = astInput.TransformerAccept(new CoreASTReplaceTransformer(), state);
        Assert.Equal(astOutput, astOutput2);
    }

#if false
    [Fact]
    public void Gna04() {
        var input = """
            /* <core:merge seperator="/"> */
            <core:replace name="ab"></core:replace>
            <core:replace name="ac"></core:replace>
            /* </core:merge> */
            """;
        var data = new GnaState();
        data.Values["ab"] = ["a", "b"];
        data.Values["cd"] = ["c", "d"];
        var output = """/* <a seperator="/"> */a/b/* </a> */""";
        var astInput = Parser.CreateForCSharp().Parse(input);
        var astOutput = astInput.TransformerAccept(new Gna(), data);
        var act = ASTTreeToString.GetAsString(astOutput);
        Assert.Equal(output, act);
    }
#endif
}
