// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

header {
    import hetero
}

options {
    language=Python;
}

/** This example demonstrates the heterogeneous tree construction
 *  mechanism.  Compare this example to examples/calc/calc.g
 *  to see that I use tree node methods not a tree walker to compute
 *  the result.
 */
class hetero_p extends Parser;
options {
	buildAST = true;	// uses CommonAST by default
}

// define a bunch of specific AST nodes to build.
// can override at actual reference of tokens in grammar
// below.
tokens {
	PLUS<AST=hetero.PLUSNode>;
	STAR<AST=hetero.MULTNode>;
}

expr
	:	mexpr (PLUS^ mexpr)* SEMI!
	;

mexpr
	:	atom (STAR^ atom)*
	;

atom:	INT<AST=hetero.INTNode>	// could have done in tokens{} section
	;

class hetero_l extends Lexer;

WS	:	(' '
	|	'\t'
	|	'\n'
	|	'\r')
		{ _ttype = SKIP; }
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
