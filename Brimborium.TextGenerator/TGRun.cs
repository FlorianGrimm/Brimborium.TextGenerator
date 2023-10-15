namespace Brimborium.TextGenerator;

public interface ITGRun
{
    string? GetText() { return null; }
    public void AppendText(StringBuilder sbOut) {
        if (this.GetText() is string text) { 
            sbOut.Append(text);
        }
    }
}


public class TGRunText: ITGRun
{
    public TGRunText() { }

    public string? Text { get; set; }

    public string? GetText() => this.Text;
}