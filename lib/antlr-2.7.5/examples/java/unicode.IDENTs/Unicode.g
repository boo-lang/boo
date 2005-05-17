/*
** Unicode example
** written by Matthew Ford (c)2000 Forward Computing and Control Pty. Ltd.
** email matthew.ford@forward.com.au
**
** The UnicodeLexer is the interesting part
*/


{
import java.io.*;
import antlr.collections.AST;
import antlr.*;
import antlr.collections.*;
import antlr.debug.misc.*;
} // end of Antlr intro block

class UnicodeParser extends Parser;

options {
	buildAST = false;	// skip the tree building
	defaultErrorHandler = false;     // Don't generate parser error handlers
}


program
	: (statement)* // perhaps none
	   EOF 
;

protected
statement
  {Token exprToken;}
	:	lhs:IDENT ASSIGNS rhs:IDENT SEMI!
	{ System.out.println(" Found statement:   " + lhs.getText()+ ":=" + rhs.getText() ); }
	| tt:TOTAL_TIME SEMI!
	{ System.out.println(" Found TOTAL_TIME statement: " + tt.getText()); }
	| SEMI! {System.out.println(" Found empty statement"); }
	;


class UnicodeLexer extends Lexer;

options {
	charVocabulary = '\u0000'..'\uFFFE';  // allow all possiable unicodes except -1 == EOF
	testLiterals = false;  // in general do not test literals 
	caseSensitiveLiterals=false;
	caseSensitive=false;  
	defaultErrorHandler = false;   // pass error back to parser
  k = 2; // two character lookahead for // versus /*	
}

tokens {
  TOTAL_TIME = "\u5408\u8A08\u6642\u9593"; // total_time
}


// an identifier.  Note that testLiterals is set to true!  This means
// that after we match the rule, we look in the literals table to see
// if it's a literal or really an identifer
// NOTE: any char > \u0080 can start an Ident
// may need to restrict this more in some cases
// \uFFFF is EOF so do not include it here, stop at \uFFFE
IDENT
	options {testLiterals=true;
	    paraphrase = "an identifier";}
	:	('a'..'z'|'_'|'$'|'\u0080'..'\uFFFE') ('a'..'z'|'_'|'0'..'9'|'$'|'\u0080'..'\uFFFE')*
	;


ASSIGNS options {paraphrase = ":=";}
	: ":="
	;
	
SEMI options {paraphrase = ";";}
	: ';';
	

// white space is skipped by the parser  
WS	:	(	' '			
		|	'\t'
		|	'\r'('\n')?	 {newline();}
		|	'\n' {newline();}		
		)
		{$setType(Token.SKIP);}		// way to set token type
	;