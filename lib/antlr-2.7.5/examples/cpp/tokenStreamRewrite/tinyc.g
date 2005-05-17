header
{
	#include <antlr/TokenStreamRewriteEngine.hpp>
}

options
{
	language = "Cpp";
	genHashLines = true;
}

{
ANTLR_USING_NAMESPACE(antlr);
}
class TinyCParser extends Parser;

options
{
	exportVocab = TinyC;
	noConstructors = true;	// disable generation of default constructors
}

{
	ANTLR_USE_NAMESPACE(antlr)TokenStreamRewriteEngine& engine;
public:
	TinyCParser(ANTLR_USE_NAMESPACE(antlr)TokenStreamRewriteEngine& lexer)
	: ANTLR_USE_NAMESPACE(antlr)LLkParser(lexer,1)
	, engine(lexer)
	{
	}
}

program
	:	( declaration )* EOF
	;

declaration
	:	(globalVariable) => globalVariable
	|	function
	;

declarator
	:	id:ID
	|	STAR id2:ID
	;

variable
	:	type declarator SEMI
	;

/** Convert "int foo;" into "extern int foo;" */
globalVariable:
{
	RefTokenWithIndex t(LT(1));
	engine.insertBefore(t, "extern ");
}
	variable
	;

/** Convert "int foo() {...}" into "extern int foo();" */
function
	:
	{
		RefTokenWithIndex t(LT(1));
		engine.insertBefore(t, "extern ");
	}
	type id:ID LPAREN (formalParameter (COMMA formalParameter)*)? RPAREN
   block[true]
	;

formalParameter
	:	type declarator
	;

type:	"int" | "char" | ID ;

block[bool functionLevel]
	:	a:LCURLY ( statement )* b:RCURLY
        {
			  if ( functionLevel )
			  {
				  RefTokenWithIndex aa(a), bb(b);

				  size_t prevTokenIndex = aa->getIndex()-1;
				  RefTokenWithIndex prevToken(engine.getToken(prevTokenIndex));

				  if ( prevToken->getType() == RPAREN )
				  {
					  //std::cout << "function aa = " << aa->getIndex() << std::endl;
					  //std::cout << "function bb = " << bb->getIndex() << std::endl;
					  engine.replace(aa, bb, ";");
				  }
				  else
				  {
					  //std::cout << "function pt = " << prevToken->getIndex() << std::endl;
					  //std::cout << "function bb = " << bb->getIndex() << std::endl;
					  engine.replace(prevToken, RefTokenWithIndex(b), ";"); // replace whitespace too
				  }
			  }
        }
;

statement
	:	(variable)=>variable
	|	expr SEMI
	|	"if" LPAREN expr RPAREN statement
		( "else" statement )?
	|	"while" LPAREN expr RPAREN statement
	|	block[false]
	;

expr:	assignExpr
	;

assignExpr
	:	aexpr (ASSIGN assignExpr)?
	;

aexpr
	:	mexpr (PLUS mexpr)*
	;

mexpr
	:	atom (STAR atom)*
	;

atom:	ID
	|	INT
	|	CHAR_LITERAL
	|	STRING_LITERAL
	;

class TinyCLexer extends Lexer;

options
{
	k =	2;
	charVocabulary = '\3'..'\377';
}

WS	:	(' '
	|	'\t'  {tab();}
	|	'\n'	{newline();}
	|	'\r')+
	;

SL_COMMENT :
	"//"
	(~'\n')* '\n'
	{ $setType(antlr::Token::SKIP); newline(); }
	;

ML_COMMENT
	:	"/*"
		(	{ LA(2)!='/' }? '*'
		|	'\n' { newline(); }
		|	~('*'|'\n')
		)*
		"*/"
			{ $setType(antlr::Token::SKIP); }
	;

LPAREN
	:	'('
	;

RPAREN
	:	')'
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

CHAR_LITERAL
	:	'\'' (options {greedy=false;}:.)* '\''
	;

STRING_LITERAL
	:	'"' (options {greedy=false;}:.)* '"'
	;

protected
DIGIT
	:	'0'..'9'
	;

INT	:	(DIGIT)+
	;

ID	:	('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'_'|'0'..'9')*
	;

