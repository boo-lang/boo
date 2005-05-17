import java.io.*;

class Main {
	public static void main(String[] args) {
		try {
			DataLexer lexer = new DataLexer(new DataInputStream(System.in));
			DataParser parser = new DataParser(lexer);
			parser.file();
		} catch(Exception e) {
			System.err.println("exception: "+e);
		}
	}
}

