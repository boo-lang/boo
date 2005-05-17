class CalcParser extends Parser;
options {
	buildAST = true;	// uses CommonAST by default
	ASTLabelType = "antlr.CommonAST";
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
options {
	buildAST = true;
	ASTLabelType = "antlr.CommonAST";
}

expr:!	#(PLUS left:expr right:expr)
		{
			if ( #right.getType()==INT &&
				 Integer.parseInt(#right.getText())==0 ) // x+0
			{
				#expr = #left;
			}
			else if ( #left.getType()==INT &&
					  Integer.parseInt(#left.getText())==0 ) // 0+x
			{
				#expr = #right;
			}
			else { // x+y
				#expr = #(PLUS, left, right);
			}
		}
	|	#(STAR expr expr)
	|	i:INT
	;
