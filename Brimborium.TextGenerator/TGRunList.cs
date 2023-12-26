namespace Brimborium.TextGenerator;

public class TGRunList : ITGRun
{
    public TGRunList() { 
    }

    public readonly List<ITGRun> Target = new List<ITGRun>();

    public ITGRun Seperator { get; set; }

    public void AppendText(StringBuilder sbOut)
    {
        //if (this.Text is null) { return; }

        //sbOut.Append(this.Text);
    }

}
