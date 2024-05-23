namespace Brimborium.TextGenerator;
public class ASTTransformerReplaceTest {

    [Fact]
    public void Replace01Parse() {
        Parser sut = Parser.CreateForCSharp();
        string content = "1/* <a> */2/* </a> */3";
        var act = sut.Parse(content);
        Assert.Equal(3, act.Count);

        {
            var visitorToString = new ASTVisitorToString();
            act.VisitorAccept(visitorToString);
            Assert.Equal("1/* <a> */2/* </a> */3", visitorToString.ToString());
        }

        ASTNode? actCopy = null;
        {
            var transformerReplace = new ASTTransformerReplace<int>(
                replacePlaceholder: (that, placeholder, state) => {
                    //if (placeholder.Tag.Equals("a")) {
                    //    return new ASTConstant("XXX");
                    //}
                    return placeholder;
                });
            actCopy = act.TransformerAccept(transformerReplace, 0);
        }

        {
            var visitorToString = new ASTVisitorToString();
            actCopy?.VisitorAccept(visitorToString);
            Assert.Equal("1/* <a> */XXX/* </a> */3", visitorToString.ToString());
        }
    }


#if false
    

    [Fact]
    public void Replace02Parse() {
        Parser sut = Parser.CreateForCSharp();
        string content = "1/* <a> */2/* </a> */3";
        var act = sut.Parse(content);
        Assert.Equal(3, act.Count);

        {
            var visitorToString = new ASTVisitorToString();
            act.VisitorAccept(visitorToString);
            Assert.Equal("1/* <a> */2/* </a> */3", visitorToString.ToString());
        }

        ASTNode? actCopy = null;
        {
            var visitorReplace = new ASTTransformerReplace<ASTTransformerState>(
                (stack, current) => {
                    if (stack.Peek().Tag.Equals("a")) {
                        return new ASTConstant("XXX");
                    }
                    return current;
                }

                );
            act.VisitorAccept(visitorReplace);
            actCopy = visitorReplace.GetResult();
        }

        {
            var visitorToString = new ASTVisitorToString();
            actCopy?.VisitorAccept(visitorToString);
            Assert.Equal("1/* <a> */XXX/* </a> */3", visitorToString.ToString());
        }
    }

#endif
}
