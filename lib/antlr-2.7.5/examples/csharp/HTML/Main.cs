/*
	Simple class for testing antlr-generated HTML parser/lexer.
	Alexander Hinds, Magelang Institute
	ahinds@magelang.com

*/

using System;
using antlr;

public class AppMain {
  public static void Main(string[] args) {
    try {
      HTMLLexer lexer = new HTMLLexer(new ByteBuffer(Console.OpenStandardInput()));
      TokenBuffer buffer = new TokenBuffer(lexer);
      HTMLParser parser = new HTMLParser(buffer);
      parser.document();
    } catch(Exception e) {
      Console.Error.WriteLine("exception: "+e);
      Environment.Exit(1);
    }
  }
}

