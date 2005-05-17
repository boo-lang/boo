import java.io.*;
import antlr.collections.AST;
import antlr.collections.impl.*;
import antlr.debug.misc.*;
import antlr.*;

class Main {
  // Define a selector that can handle nested include files.
  // These variables are public so the parser/lexer can see them.
  public static TokenStreamSelector selector = new TokenStreamSelector();
  public static PParser parser;
  public static PLexer mainLexer;

  public static void main(String[] args) {
    try {
      // open a simple stream to the input
      DataInputStream input = new DataInputStream(System.in);

      // attach java lexer to the input stream,
      mainLexer = new PLexer(input);

      // notify selector about starting lexer; name for convenience
      selector.addInputStream(mainLexer, "main");
      selector.select("main"); // start with main P lexer

      // Create parser attached to selector
      parser = new PParser(selector);

	  // Parse the input language: P
	  parser.setFilename("<stdin>");
      parser.startRule();
    }
    catch(Exception e) {
      System.err.println("exception: "+e);
      e.printStackTrace(System.err);   // so we can get stack trace
    }
  }
}
