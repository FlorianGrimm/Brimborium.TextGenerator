
namespace Brimborium.TextGenerator;

public class TGRunText: ITGRun
{
    public TGRunText() { }

    public string? Text { get; set; }

    public void AppendText(StringBuilder sbOut)
    {
        if (string.IsNullOrEmpty(this.Text)) { return; }

        sbOut.Append(this.Text);
    }

    // public string? GetText() => this.Text;
}
