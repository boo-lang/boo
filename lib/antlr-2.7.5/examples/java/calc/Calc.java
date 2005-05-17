import java.io.*;
import antlr.CommonAST;
import antlr.collections.AST;
import antlr.DumpASTVisitor;
import antlr.RecognitionException;
import antlr.TokenStreamException;

class Calc {
	public static void main(String[] args) {
		try {
			CalcLexer lexer = new CalcLexer(new DataInputStream(System.in));
			lexer.setFilename("<stdin>");
			CalcParser parser = new CalcParser(lexer);
			parser.setFilename("<stdin>");
			// Parse the input expression
			parser.expr();
			CommonAST t = (CommonAST)parser.getAST();
			// Print the resulting tree out in LISP notation
			System.out.println(t.toStringTree());
			CalcTreeWalker walker = new CalcTreeWalker();
			// Traverse the tree created by the parser
			float r = walker.expr(t);
			System.out.println("value is "+r);
		}
		catch(TokenStreamException e) {
			System.err.println("exception: "+e);
		}
		catch(RecognitionException e) {
			System.err.println("exception: "+e);
		}
	}
}
