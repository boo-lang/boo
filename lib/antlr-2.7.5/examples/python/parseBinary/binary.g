// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

options {
    language=Python;
}

class binary_p extends Parser;

file:	(	sh:SHORT	{ print sh.getText()           }
		|	st:STRING	{ print "\"" + st.getText() + "\"" }
		)+
	;

class binary_l extends Lexer;
options {
	charVocabulary = '\u0000'..'\u00FF';
}

SHORT
	:	'\0' high:. lo:.
		{
            v = (ord(high)<<8) + ord(lo)
            $setText(str(v))
		}
	;

STRING
	:	'\1'!	// begin string (discard)
		( ~'\2' )*
		'\2'!	// end string (discard)
	;
