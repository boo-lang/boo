using System;
using antlr;
using antlr.collections;

public abstract class CalcAST : antlr.BaseAST {
	public abstract int Value();

	// satisfy abstract methods from BaseAST
	public override void initialize(int t, String txt) {
	}
	public override void initialize(AST t) {
	}
	public override void initialize(IToken tok) {
	}
}
