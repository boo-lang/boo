options {
	language=Cpp;
}

class CalcParser extends Parser;
options {
	buildAST = true;	// uses CommonAST by default
//	ASTLabelType = "antlr.CommonAST";
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

class CalcTreeWalker extends TreeParser;
options {
	buildAST = true;
//	ASTLabelType = "antlr.CommonAST";
}

expr:!	#(PLUS left:expr right:expr)
		{
			if ( #right->getType()==INT &&
				 atoi(#right->getText().c_str())==0 ) // x+0
			{
				#expr = #left;
			}
			else if ( #left->getType()==INT &&
					  atoi(#left->getText().c_str())==0 ) // 0+x
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
