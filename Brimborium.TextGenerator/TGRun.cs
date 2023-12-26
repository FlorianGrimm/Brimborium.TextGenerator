namespace Brimborium.TextGenerator;

public interface ITGRun
{
    //string? GetText();
    TGSourceInformation GetGSourceInformation();

    void AppendText(StringBuilder sbOut);
    /*
    public void AppendText(StringBuilder sbOut) {
        if (this.GetText() is string text) { 
            sbOut.Append(text);
        }
    }
    */
}
