import java.io.*;
import antlr.CommonAST;
import antlr.collections.AST;
import antlr.DumpASTVisitor;

class Calc {
	public static void main(String[] args) {
		try {
			CalcLexer lexer = new CalcLexer(new DataInputStream(System.in));
			CalcParser parser = new CalcParser(lexer);
			// Parse the input expression
			parser.expr();
			CommonAST t = (CommonAST)parser.getAST();
			// Print the resulting tree out in LISP notation
			System.out.println(t.toStringList());
			CalcTreeWalker walker = new CalcTreeWalker();
			// Traverse the tree created by the parser
			walker.expr(t);
			t = (CommonAST)walker.getAST();
			System.out.println(t.toStringList());
		} catch(Exception e) {
			System.err.println("exception: "+e);
		}
	}
}
