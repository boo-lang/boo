import java.io.*;
import antlr.DumpASTVisitor;

class Main {
	public static void main(String[] args) {
		try {
			LangLexer lexer = new LangLexer(new DataInputStream(System.in));
			LangParser parser = new LangParser(lexer);
			parser.block();
			System.out.println(parser.getAST().toStringList());
			LangWalker walker = new LangWalker();
			walker.block(parser.getAST());	// walk tree
			System.out.println("done walking");
		} catch(Exception e) {
			System.err.println("exception: "+e);
		}
	}
}

