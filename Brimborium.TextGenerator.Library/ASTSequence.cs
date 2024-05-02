﻿using System.Diagnostics;

namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTSequence(List<ASTNode>? list = default) : ASTNode, IEnumerable<ASTNode> {
    public override void Accept(ASTVisitor visitor) { visitor.VisitSequence(this); }
    public override void AcceptChildren(ASTVisitor visitor) {
        foreach (var item in this.List) {
            item.Accept(visitor);
        }
    }

    public List<ASTNode> List { get; } = list ?? [];

    public ASTSequence Add(ASTNode item) {
        this.List.Add(item);
        return this;
    }
    public ASTNode this[int index] { get { return this.List[index]; } }
    public int Count { get { return this.List.Count; } }
    public IEnumerator<ASTNode> GetEnumerator() => this.List.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => this.List.GetEnumerator();

    public override string ToString() => $"ParserASTSequence #{this.List.Count}";

    private string GetDebuggerDisplay() => $"ParserASTSequence #{this.List.Count}";
}
