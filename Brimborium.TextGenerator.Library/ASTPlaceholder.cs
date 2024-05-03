﻿using System.Diagnostics;

namespace Brimborium.TextGenerator;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ASTPlaceholder(
    ASTToken startToken,
    List<ASTNode> list,
    ASTToken finishToken
    ) : ASTNode, IEnumerable<ASTNode> {
    public override void VisitorAccept(IASTVisitor visitor) { visitor.VisitPlaceholder(this); }
    public override void VisitorAcceptChildren(IASTVisitor visitor) {
        this.StartToken.VisitorAccept(visitor);
        foreach (var item in this.List) {
            item.VisitorAccept(visitor);
        }
        this.FinishToken.VisitorAccept(visitor);
    }

    public override ASTTransformResult<T> TransformerAccept<T>(IASTTransformer<T> transformer) => transformer.VisitPlaceholder(this);

    public ASTToken StartToken { get; set; } = startToken;
    public List<ASTNode> List { get; } = list;
    public ASTToken FinishToken { get; set; } = finishToken;

    public ASTPlaceholder Add(ASTNode item) {
        this.List.Add(item);
        return this;
    }
    public ASTNode this[int index] { get { return this.List[index]; } }
    public int Count { get { return this.List.Count; } }

    public StringSlice Tag => this.StartToken.Tag;

    public IEnumerator<ASTNode> GetEnumerator() => this.List.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => this.List.GetEnumerator();

    public override string ToString() => $"ParserASTPlaceholder {this.StartToken.Tag} #{this.List.Count}";

    private string GetDebuggerDisplay() => $"ParserASTPlaceholder {this.StartToken.Tag} #{this.List.Count}";
}

