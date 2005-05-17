import antlr.Token;
import antlr.collections.AST;

public abstract class CalcAST extends antlr.BaseAST {
	public abstract int value();

	// satisfy abstract methods from BaseAST
	public void initialize(int t, String txt) {
	}
	public void initialize(AST t) {
	}
	public void initialize(Token tok) {
	}
}
