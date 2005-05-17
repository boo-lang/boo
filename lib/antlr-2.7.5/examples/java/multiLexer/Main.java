import java.io.*;
import antlr.collections.AST;
import antlr.collections.impl.*;
import antlr.debug.misc.*;
import antlr.*;

class Main {
  // Define a selector that can switch from java to javadoc; make visible to lexers
  static TokenStreamSelector selector = new TokenStreamSelector();

  public static void main(String[] args) {
    try {
      // open a simple stream to the input
      DataInputStream input = new DataInputStream(System.in);

      // attach java lexer to the input stream, which also creates a shared input state object
      DemoJavaLexer main = new DemoJavaLexer(input);

      // create javadoc lexer; attach to same shared input state as java lexer
      DemoJavaDocLexer doclexer = new DemoJavaDocLexer(main.getInputState());

      // notify selector about various lexers; name them for convenient reference later
      selector.addInputStream(main, "main");
      selector.addInputStream(doclexer, "doclexer");
      selector.select("main"); // start with main java lexer

      // Create parser attached to selector
      DemoJavaParser parser = new DemoJavaParser(selector);

      // Pull in one or more int decls with optional javadoc
      parser.input();

      /*
      // spin thru all tokens generated via the SELECTOR.
      Token t;
      while ( (t=selector.nextToken()).getType()!=main.EOF ) {
	System.out.println(t.toString());
      }
      */
    }
    catch(Exception e) {
      System.err.println("exception: "+e);
      e.printStackTrace(System.err);   // so we can get stack trace
    }
  }

}
