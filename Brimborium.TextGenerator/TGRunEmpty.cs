namespace Brimborium.TextGenerator;

public class TGRunEmpty : ITGRun
{
    private static TGRunEmpty? _Instance;
    public static TGRunEmpty? Instance => (_Instance ??= new TGRunEmpty());

    public void AppendText(StringBuilder sbOut) { }

    //public string? GetText() => default;
}