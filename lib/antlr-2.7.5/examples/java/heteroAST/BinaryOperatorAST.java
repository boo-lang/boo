public abstract class BinaryOperatorAST extends CalcAST {
	public CalcAST left() {
		return (CalcAST)getFirstChild();
	}

	public CalcAST right() {
		CalcAST t = left();
		if ( t==null ) return null;
		return (CalcAST)t.getNextSibling();
	}
}
