namespace Test
{
	using System;
	using System.IO;
	using antlr;
	
	class AppMain {
	  // Define a selector that can handle nested include files.
	  // These variables are public so the parser/lexer can see them.
	  public static TokenStreamSelector selector = new TokenStreamSelector();
	  public static PParser parser;
	  public static PLexer mainLexer;
	
	  public static void Main(string[] args) {
	    try {
	      // open a simple stream to the input
	      CharBuffer input = new CharBuffer(Console.In);
	
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
	      Console.Error.WriteLine("exception: "+e);
	      Console.Error.WriteLine(e.StackTrace);	// so we can get stack trace
	    }
	  }
	}
}