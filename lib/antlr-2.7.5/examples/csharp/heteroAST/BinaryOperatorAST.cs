using System;

public abstract class BinaryOperatorAST : CalcAST {
	public CalcAST Left() {
		return (CalcAST)getFirstChild();
	}

	public CalcAST Right() {
		CalcAST t = Left();
		if ( t==null ) return null;
		return (CalcAST)t.getNextSibling();
	}
}
