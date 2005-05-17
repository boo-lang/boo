// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

options {
    language=Python;
}

class multilex_l extends Lexer;
options {
	k=2;
	importVocab = Common;
	exportVocab = Java;
}

tokens {
	INT="int";
}

JAVADOC_OPEN
	:	"/**" { import multilex ; multilex.selector.push("doclexer");}
	;

ID	:	('a'..'z')+ ;
SEMI:	';' ;
WS	:	(	' '
		|	'\t'
		|	'\f'
		// handle newlines
		|	(	"\r\n"  // Evil DOS
			|	'\r'    // Macintosh
			|	'\n'    // Unix (the right way)
			)
			{ self.newline(); }
		)
		{ $setType(Token.SKIP); }
	;

