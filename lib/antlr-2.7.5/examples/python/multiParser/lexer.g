/* This grammar demonstrates the use of two parsers sharing a token
 * vocabulary with a single lexer.
 */

header {
# empty header
}

options {
    language="Python";
}

class SimpleLexer extends Lexer;

options {
    exportVocab=Simple;
}

WS_	:	(' '
	|	'\t'	{ self.tab() }
	|	'\n'	{ self.newline() }
	|	'\r')
		{ $setType(Token.SKIP) }
	;

A : 'a' | 'A' ;
B : 'b' | 'B' ;
C : 'c' | 'C' ;
D : 'd' | 'D' ;

