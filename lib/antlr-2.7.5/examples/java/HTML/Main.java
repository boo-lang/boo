/*
	Simple class for testing antlr-generated HTML parser/lexer.
	Alexander Hinds, Magelang Institute
	ahinds@magelang.com

*/


import java.io.*;
import antlr.*;

public class Main {
  public static void main(String[] args) {
    try {
      HTMLLexer lexer = new HTMLLexer(new DataInputStream(System.in));
      TokenBuffer buffer = new TokenBuffer(lexer);
      HTMLParser parser = new HTMLParser(buffer);
      parser.document();
    } catch(Exception e) {
      System.err.println("exception: "+e);
      System.exit(1);
    }
  }
}

