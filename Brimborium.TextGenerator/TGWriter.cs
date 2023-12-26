namespace Brimborium.TextGenerator;

public interface ITGWriter
{
    void Append(ITGRun run);
}

public class TGWriter : ITGWriter
{
    private readonly List<ITGRun> _ListTGRun = new List<ITGRun>();

    public TGWriter()
    {
    }

    public void Append(ITGRun run)
    {
        this._ListTGRun.Add(run);
    }

    public StringBuilder Write(StringBuilder? sbOut=default)
    {
        if (sbOut == null) { sbOut = new StringBuilder(); }
        foreach (var tgRun in this._ListTGRun)
        {
            tgRun.AppendText(sbOut);
        }
        return sbOut;
    }
}
