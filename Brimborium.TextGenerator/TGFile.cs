using System.Collections.Immutable;

namespace Brimborium.TextGenerator;

public sealed record class TGFile(
    ImmutableList<TGTemplate> ListTemplate,
    ImmutableList<TGOperation> ListOperation,
    ImmutableList<TGRun> ListRun
    );

public sealed class TGFileBuilder
{
    public List<TGTemplate> ListTemplate { get; set; } = new List<TGTemplate>();
    public List<TGOperation> ListOperation { get; set; } = new List<TGOperation>();
    public List<TGRun> ListRun { get; set; } = new List<TGRun>();

    public TGFile Build()
    {
        return new TGFile(
            this.ListTemplate.ToImmutableList(),
            this.ListOperation.ToImmutableList(),
            this.ListRun.ToImmutableList());
    }
}


public sealed record class TGTemplate(string Name, int OperationId);

public sealed record class TGOperation(string Name, int OperationId);

public sealed record class TGRun(string? Text, int OperationId);

//