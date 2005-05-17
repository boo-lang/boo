import java.io.*;

class Main {
	public static void main(String[] args) {
		try {
			TinyCLexer lexer = new TinyCLexer(new DataInputStream(System.in));
			MyCParser parser = new MyCParser(lexer);
			parser.program();
		} catch(Exception e) {
			System.err.println("exception: "+e);
		}
	}
}
