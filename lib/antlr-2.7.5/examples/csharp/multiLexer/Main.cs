using System;
using antlr.collections;
using antlr.collections.impl;
using antlr;

class MultiLexer {
  // Define a selector that can switch from java to javadoc; make visible to lexers
  public static TokenStreamSelector selector = new TokenStreamSelector();

  public static void Main(String[] args) {
    try {
      // open a simple stream to the input
      ByteBuffer input = new ByteBuffer(Console.OpenStandardInput());

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
      while ( (t=selector.nextToken()).Type!=main.EOF ) {
		Console.Out.WriteLine(t.ToString());
      }
      */
    }
    catch(Exception e) {
      Console.Error.WriteLine("exception: "+e);
      Console.Error.WriteLine(e.StackTrace); 		// so we can get stack trace
    }
  }

}
