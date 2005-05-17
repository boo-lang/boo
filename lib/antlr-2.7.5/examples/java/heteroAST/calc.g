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
}

expr
	:	mexpr (PLUS^ mexpr)* SEMI!
	;

mexpr
	:	atom (STAR^ atom)*
	;

atom:	INT<AST=INTNode>	// could have done in tokens{} section
	;

class CalcLexer extends Lexer;

WS	:	(' '
	|	'\t'
	|	'\n'
	|	'\r')
		{ _ttype = Token.SKIP; }
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
