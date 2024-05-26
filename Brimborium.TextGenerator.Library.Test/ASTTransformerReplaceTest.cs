namespace Brimborium.TextGenerator;
public class ASTTransformerReplaceTest {

    [Fact]
    public void Replace01Parse() {
        Parser sut = Parser.CreateForCSharp();
        string content = "1/* <a> */2/* </a> */3";
        var act = sut.Parse(content);
        Assert.Equal(3, act.ListItem.Length);

        Assert.Equal("1/* <a> */2/* </a> */3", ASTTreeToString.GetAsString(act));

        ASTNode? actCopy = null;
        {
            var transformerReplace = new ASTTransformerReplace<int>(
                replacePlaceholder: (transformer, placeholder, state) => {
                    if (placeholder.Tag.Equals("a", StringComparison.Ordinal)) {
                        return placeholder.WithListItem([new ASTConstant("XXX")]);
                    }
                    return placeholder;
                });
            actCopy = act.TransformerAccept(transformerReplace, 0);
        }

        Assert.NotNull(actCopy);
        Assert.Equal("1/* <a> */XXX/* </a> */3", ASTTreeToString.GetAsString(actCopy));
    }

    [Fact]
    public void Replace02Parse() {
        Parser sut = Parser.CreateForCSharp();
        string content = "1/* <a> */2/* </a> */3";
        var act = sut.Parse(content);
        Assert.Equal(3, act.ListItem.Length);
        Assert.Equal("1/* <a> */2/* </a> */3", ASTTreeToString.GetAsString(act));

        ASTNode? actCopy = null;
        {
            var transformerReplace = new ASTTransformerReplace<int>(
                replacePlaceholder: (transformer, placeholder, state) => {
                    if (placeholder.Tag.Equals("a", StringComparison.Ordinal)) {
                        return placeholder.WithListItem([new ASTConstant("XXX")]);
                    }
                    return placeholder;
                });
            actCopy = act.TransformerAccept(transformerReplace, 0);
        }

        Assert.Equal("1/* <a> */XXX/* </a> */3", ASTTreeToString.GetAsString(actCopy));
    }
}
