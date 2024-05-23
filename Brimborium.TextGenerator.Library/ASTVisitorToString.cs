﻿namespace Brimborium.TextGenerator;

public sealed class ASTVisitorToString : ASTVisitor {
    private static ASTVisitorToString? _Instance;
    public static ASTVisitorToString Instance => _Instance ??= new ASTVisitorToString();


    private readonly StringBuilder _Result;

    public ASTVisitorToString() {
        this._Result = new StringBuilder();
    }
    public override string ToString() {
        return this._Result.ToString();
    }
    public override void VisitConstant(ASTConstant parserASTConstant) {
        this._Result.Append(parserASTConstant.Content);
        base.VisitConstant(parserASTConstant);
    }
    public override void VisitToken(ASTToken parserASTToken) {
        this._Result.Append(parserASTToken.Complete);
        base.VisitToken(parserASTToken);
    }
    public override void VisitPlaceholder(ASTPlaceholder parserASTPlaceHolder) {
        base.VisitPlaceholder(parserASTPlaceHolder);
    }
}