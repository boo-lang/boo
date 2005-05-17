import java.io.*;
import antlr.*;

public class Main {
    public static void main(String[] args) {
	InputStream in = System.in;
	try {
	    if ( args.length>0 ) {
		in = new FileInputStream(args[0]);
	    }
	    XMLLexer lexer = new XMLLexer(in);
	    while ( lexer.nextToken().getType() != Token.EOF_TYPE );
	} catch(Throwable t) {
	    System.out.println("exception: "+t);
	    t.printStackTrace();
	}
    }
}
