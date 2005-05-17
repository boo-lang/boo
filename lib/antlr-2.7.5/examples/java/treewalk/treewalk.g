class LangParser extends Parser;

options {
	codeGenMakeSwitchThreshold = 3;
	codeGenBitsetTestThreshold = 4;
	buildAST=true;
}

block
	:	LCURLY^ ( statement )* RCURLY!
	;

statement
	:	expr SEMI!
	|	"if"^ LPAREN! expr RPAREN! statement
		( "else"! statement )?
	|	"while"^ LPAREN! expr RPAREN! statement
	|!	b:block { statement_AST = b_AST; }
		// do some manual tree returning
	;

expr:	assignExpr
	;

assignExpr
	:	aexpr (ASSIGN^ assignExpr)?
	;

aexpr
	:	mexpr (PLUS^ mexpr)*
	;

mexpr
	:	atom (STAR^ atom)*
	;

atom:	ID
	|	INT
	;

class LangWalker extends TreeParser;

block
	:	#( LCURLY ( stat )+ )
	;

stat:	#("if" expr stat (stat)?)
	|	#("while" expr stat)
	|	expr
	|	block
	;

expr:	#(ASSIGN expr expr)		{System.out.println("found assign");}
	|	#(PLUS expr expr)		{System.out.println("found +");}
	|	#(STAR expr expr)		{System.out.println("found *");}
	|	a:ID					{System.out.println("found ID "+a.getText());}
	|	b:INT					{System.out.println("found INT "+b.getText());}
	;

class LangLexer extends Lexer;

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

LCURLY:	'{'
	;

RCURLY:	'}'
	;

STAR:	'*'
	;

PLUS:	'+'
	;

ASSIGN
	:	'='
	;

SEMI:	';'
	;

COMMA
	:	','
	;

protected
ESC	:	'\\'
		(	'n'
		|	'r'
		|	't'
		|	'b'
		|	'f'
		|	'"'
		|	'\''
		|	'\\'
		|	('0'..'3') ( DIGIT (DIGIT)? )?
		|	('4'..'7') (DIGIT)?
		)
	;

protected
DIGIT
	:	'0'..'9'
	;

INT	:	(DIGIT)+
	;

ID	:	('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'_'|'0'..'9')*
		{
			Integer i = (Integer)literals.get(getText());
			if ( i!=null ) {
				_ttype =  i.intValue();
			}
		}
	;
