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

    public void Write(StringBuilder sbOut)
    {
        foreach (var tgRun in this._ListTGRun)
        {
            tgRun.AppendText(sbOut);
        }
    }
}
