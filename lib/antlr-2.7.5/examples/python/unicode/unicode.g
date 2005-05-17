// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

options {
  language = Python;
}


class unicode_l extends Lexer;

options {
	// Allow any char but \uFFFF (16 bit -1)
	charVocabulary='\u0000'..'\uFFFE';
}

{
    done = False

    def uponEOF(self):
        done=True

    def another(self):
        pass	

}

ID	:	ID_START_LETTER ( ID_LETTER )*
	;

WS	:	(' '|'\n') {$skip}
	;

protected
ID_START_LETTER
	:	'$'
	|	'_'
	|	'a'..'z'
	|	'\u0080'..'\ufffe'
	;

protected
ID_LETTER
	:	ID_START_LETTER  
{
            // got a LETTER_ID
            // handle it

            // whatever
}
	|	'0'..'9'
	;


// ANTLR should actually allow this here. Would enable me to write
// something like:
//{
//  if __name__ == '__main__' : 
//    ## test lexer
//
//}
