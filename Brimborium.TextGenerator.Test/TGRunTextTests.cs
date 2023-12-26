namespace Brimborium.TextGenerator.Test;

public class TGRunTextTests
{
    [Fact]
    public void TGRunText001()
    {
        var tgWriter = new TGWriter();
        tgWriter.Append(new TGRunText() { Text = "4" });
        tgWriter.Append(new TGRunText() { Text = "2" });
        var act = tgWriter.Write().ToString();
        Assert.Equal("42", act);
    }
}
