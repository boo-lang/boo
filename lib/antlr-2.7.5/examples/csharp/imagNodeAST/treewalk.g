header {
}

options {
    language="CSharp";
}

class LangParser extends Parser;

options {
	buildAST=true;
}

tokens {
	BLOCK; // imaginary token
}

block
	:	LCURLY! ( statement )* RCURLY!
		// add imaginary BLOCK node on top of statement list
		{#block = #([BLOCK, "BLOCK"], #block);}
	;

statement
	:	ID ASSIGN^ expr SEMI!
	|	"if"^ LPAREN! expr RPAREN! statement
		( "else"! statement )?
	|	"while"^ LPAREN! expr RPAREN! statement
	|!	b:block { statement_AST = #b; }
		// do some manual tree returning
	;

// add an EXPR node on top of an expression
// note that the two alternatives behave exactly
// the same way.
expr:!	id:ID		{#expr = #([EXPR,"EXPR"],#id);}
	|	INT			{#expr = #([EXPR,"EXPR"],#expr);}
	;

class LangWalker extends TreeParser;

block	:	#( BLOCK ( stat )+ )
	;

stat:	#("if" expr stat (stat)?)
	|	#("while" expr stat)
	|	expr
	|	block
	|	#( ASSIGN ID expr )
	;

expr:	#(	EXPR
			(	a:ID	{Console.Out.WriteLine("found ID "+a.getText());}
			|	b:INT	{Console.Out.WriteLine("found INT "+b.getText());}
			)
		 )
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

ASSIGN
	:	'='
	;

SEMI:	';'
	;

protected
DIGIT
	:	'0'..'9'
	;

INT	:	(DIGIT)+
	;

ID
options {
	testLiterals = true;
}
	:	('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'_'|DIGIT)*
	;
