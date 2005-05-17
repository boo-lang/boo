using System;
using antlr;
using antlr.collections;
using antlr.debug.misc;

class InstrMain {
  /** Parser will query filter for information */
  public static TokenStreamHiddenTokenFilter filter;

public static void Main(string[] args) {
  InstrLexer lexer = new InstrLexer(new ByteBuffer(Console.OpenStandardInput()));
  lexer.setTokenObjectClass("antlr.CommonHiddenStreamToken");
  filter = new TokenStreamHiddenTokenFilter(lexer);
  filter.hide(InstrParser.WS);
  filter.hide(InstrParser.SL_COMMENT);
  InstrParser parser = new InstrParser(filter);
  parser.setASTNodeClass("antlr.CommonASTWithHiddenTokens");
  try {
    // Parse the input statements
    parser.slist();
  }
  catch (TokenStreamException io) {
    Console.Error.WriteLine("IOException while parsing");
  }
  catch(RecognitionException e) {
    Console.Error.WriteLine("exception: "+e);
  }
  CommonASTWithHiddenTokens t =
    (CommonASTWithHiddenTokens)parser.getAST();

/* UNCOMMENT THIS TO SEE THE TREE STRUCTURE
  ASTFactory factory = new ASTFactory();
  AST r = factory.create(0,"AST ROOT");
  r.setFirstChild(t);
  ASTFrame frame = new ASTFrame("Preserve Whitespace Example AST", r);
  frame.setVisible(true);
*/

  InstrTreeWalker walker = new InstrTreeWalker();
  try {
    walker.slist(t);
  }
  catch(RecognitionException e) {
    Console.Error.WriteLine("exception: "+e);
  }
}
}

