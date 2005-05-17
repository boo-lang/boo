/* This grammar demonstrates the use of two parsers sharing a token
 * vocabulary with a single lexer.
 */

header {
/* empty header */
}

options {
	language=Cpp;
}

class SimpleLexer extends Lexer;

options {
	exportVocab=Simple;
}

WS_	:	(' '
	|	'\t'	{ tab(); }
	|	'\n'	{ newline(); }
	|	'\r')
		{ $setType(ANTLR_USE_NAMESPACE(antlr)Token::SKIP); }
	;

A : 'a' | 'A' ;
B : 'b' | 'B' ;
C : 'c' | 'C' ;
D : 'd' | 'D' ;

