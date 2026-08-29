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

expr returns [float r]
{
	float a,b;
	r=0;
}
	:	#(PLUS a=expr b=expr)	{r = a+b;}
	|	#(STAR a=expr b=expr)	{r = a*b;}
	|	i:INT			{r = (float)Integer.parseInt(i.getText());}
	;

