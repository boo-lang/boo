// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

/*
 * Make sure to run antlr.Tool on the lexer.g file first!
 */
options {
	mangleLiteralPrefix = "TK_";
    language=Python;
}

class tinyc_l extends Lexer;
options {
	k=2;
	exportVocab=TinyC;
	charVocabulary = '\3'..'\377';
}

tokens {
	"int"; "char"; "if"; "else"; "while";
}

WS	:	(' '
	|	'\t'
	|	'\n'	{ $newline;}
	|	'\r')
		{ _ttype = SKIP; }
	;


SL_COMMENT : 
	"//" 
	(~'\n')* '\n'
	{ _ttype = Token.SKIP; $newline; }
	;

ML_COMMENT
	:	"/*"
		(	{ self.LA(2) != '/' }? '*'
		|	'\n' { $newline; }
		|	~('*'|'\n')
		)*
		"*/"
			{ $setType(SKIP); }
	;


LPAREN
options {
	paraphrase="'('";
}
	:	'('
	;

RPAREN
options {
	paraphrase="')'";
}
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
	:	'\'' (ESC|~'\'') '\''
	;

STRING_LITERAL
	:	'"' (ESC|~'"')* '"'
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
		|	'0'..'3'
			(
				options {
					warnWhenFollowAmbig = false;
				}
			:	DIGIT
				(
					options {
						warnWhenFollowAmbig = false;
					}
				:	DIGIT
				)?
			)?
		|	'4'..'7'
			(
				options {
					warnWhenFollowAmbig = false;
				}
			:	DIGIT
			)?
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
	paraphrase = "an identifier";
}
	:	('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'_'|'0'..'9')*
	;


