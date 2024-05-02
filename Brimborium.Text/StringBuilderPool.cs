namespace Brimborium.Text;

public sealed class StringBuilderPool {
    private static StringBuilderPool? _Instance;
    
    public static StringBuilderPool Instance => _Instance ??= new StringBuilderPool();

    public static StringBuilder GetStringBuilder() => Instance.Get();

    public static void ReturnStringBuilder(StringBuilder stringBuilder) => Instance.Return(stringBuilder);
    
    private readonly ObjectPool<StringBuilder> _Pool;

    public StringBuilderPool() {
        this._Pool = new DefaultObjectPoolProvider().CreateStringBuilderPool();
    }

    public StringBuilder Get() => this._Pool.Get();

    public void Return(StringBuilder stringBuilder) => this._Pool.Return(stringBuilder);
}