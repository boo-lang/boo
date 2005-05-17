import java.io.*;
import antlr.CommonAST;
import antlr.collections.AST;

class Main {
	public static void main(String[] args) {
		new INTNode();
		try {
			CalcLexer lexer = new CalcLexer(new DataInputStream(System.in));
			CalcParser parser = new CalcParser(lexer);
			// Parse the input expression
			parser.expr();
			CalcAST t = (CalcAST)parser.getAST();

			// Print the resulting tree out in LISP notation
			System.out.println(t.toStringTree());

			// XML serialize the tree, showing
			// different physical node class types
			Writer w = new OutputStreamWriter(System.out);
			t.xmlSerialize(w);
			w.write("\n");
			w.flush();

			// Compute value and return
			int r = t.value();
			System.out.println("value is "+r);
		} catch(Exception e) {
			System.err.println("exception: "+e);
			e.printStackTrace();
		}
	}
}
