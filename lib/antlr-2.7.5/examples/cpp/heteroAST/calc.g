header {
#include "PLUSNode.hpp"
#include "MULTNode.hpp"
#include "INTNode.hpp"
}

options {
	language = Cpp;
}

/** This example demonstrates the heterogeneous tree construction
 *  mechanism.  Compare this example to examples/calc/calc.g
 *  to see that I use tree node methods not a tree walker to compute
 *  the result.
 */
class CalcParser extends Parser;
options {
	buildAST = true;	// uses CommonAST by default
}

// define a bunch of specific AST nodes to build.
// can override at actual reference of tokens in grammar
// below.
tokens {
	PLUS<AST=PLUSNode>;
	STAR<AST=MULTNode>;
	INT;
	SEMI;
}

expr
	:	mexpr (PLUS^ mexpr)* SEMI!
	;

mexpr
	:	atom (STAR^ atom)*
	;

atom:	INT <AST=INTNode>		// also possible in tokens section
	;

class CalcLexer extends Lexer;

WS_	:	(' '
	|	'\t'
	|	'\n'
	|	'\r')
		{ _ttype = ANTLR_USE_NAMESPACE(antlr)Token::SKIP; }
	;

LPAREN:	'('
	;

RPAREN:	')'
	;

STAR:	'*'
	;

PLUS:	'+'
	;

SEMI:	';'
	;

protected
DIGIT
	:	'0'..'9'
	;

INT	:	(DIGIT)+
	;
