header {
#include "MyAST.h"
}

options {
	language="Cpp";
}

{
#include <iostream>
}

class LangParser extends Parser;

options {
	codeGenMakeSwitchThreshold = 3;
	codeGenBitsetTestThreshold = 4;
	ASTLabelType = "RefMyAST";
	buildAST = true;
}

block
	:	LCURLY^ ( statement )* RCURLY!
	;

statement
	:	expr SEMI!
	|	"if"^ LPAREN! expr RPAREN! statement
		( "else"! statement )?
	|	"while"^ LPAREN! expr RPAREN! statement
	|!	b:block { #statement = b_AST; }
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

{
#include <iostream>

void LangWalker::printAST( RefMyAST ast )
{
	ANTLR_USE_NAMESPACE(std)cout << "Found " << getTokenName(ast->getType())
		<< " '" << ast->getText()
		<< "' at line " << ast->getLine()
		<< ANTLR_USE_NAMESPACE(std)endl;
}

}

class LangWalker extends TreeParser;
options {
 	ASTLabelType = "RefMyAST";
}
{
	void printAST( RefMyAST ast );
}
block
	:	#( LCURLY ( stat )+ )
	;

stat:	#("if" expr stat (stat)?)
	|	#("while" expr stat)
	|	expr
	|	block
	;

expr:	#(asgn:ASSIGN expr expr)			{ printAST(asgn); }
	|	#(plus:PLUS expr expr)				{ printAST(plus); }
	|	#(star:STAR expr expr)				{ printAST(star); }
	|	a:ID										{ printAST(a); }
	|	b:INT										{ printAST(b); }
	;

class LangLexer extends Lexer;

WS_	:	(' '
	|	'\t' { tab(); }
	|	'\n' { newline(); }
	|	'\r')
		{ $setType(ANTLR_USE_NAMESPACE(antlr)Token::SKIP); }
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

ID
options {
	testLiterals = true;
}
	:	('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'_'|'0'..'9')*
	;
