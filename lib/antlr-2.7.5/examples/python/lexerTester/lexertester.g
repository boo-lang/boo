// This is -*- ANTLR -*- code

header {
import sys
}

options {
  language = "Python";
}

//----------------------------------------------------------------------------
// The lexertester parser
//----------------------------------------------------------------------------

class LexerTester extends Parser;

options {
  k = 1;			// A lookahead depth of 1
  buildAST = false;		// no AST required
}

// This is a simple rule that can be used to test the Lexer. It will output
//   every token it sees using a complete description (including file, line
//   and column info).
source_text
	:	( token:.
		  { 
                    sys.stdout.write("lexertester: " + \
                                     self.getFilename() + ':' + \
                                     str(token) + '\n')
		  }
		)*
	;
