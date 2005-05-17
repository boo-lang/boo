options {
	language = "Boo";
}

class CalcParser extends Parser;
options {
	buildAST = true;	// uses CommonAST by default
}

expr
	:	mexpr (PLUS^ mexpr)* SEMI!
	;

mexpr
	:	atom (STAR^ atom)*
	;

atom:	INT
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

class CalcTreeWalker extends TreeParser;

expr returns [single r]
{
a as single
b as single
r = 0
}:
	#(PLUS a=expr b=expr)	{ r = a+b; }
	|	#(STAR a=expr b=expr)	{ r = a*b; }
	|	i:INT		{ r = Convert.ToSingle(i.getText()); }
;

