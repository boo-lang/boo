// This is -*- ANTLR -*- code

header {
import sys
}

options {
  language = "Python";
}

//----------------------------------------------------------------------------
// The lexer
//----------------------------------------------------------------------------

class SimpleLexer5 extends Lexer;

options {
  k = 1;			// A lookahead depth of 1
  codeGenDebug = true;
}

A	:	( 'A' 	{ print "found an 'A'!" } )
	;
