import java.io.*;

class Main {
	public static void main(String[] args) {
		try {
			Rewrite lexer = new Rewrite(new DataInputStream(System.in));
			lexer.mSTART(true);
			System.out.println("result Token="+lexer.getTokenObject());
		} catch(Exception e) {
			System.err.println("exception: "+e);
		}
	}
}

