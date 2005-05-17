header {
	#include <iostream>
	#include "UnicodeCharBuffer.hpp"
	#include "UnicodeCharScanner.hpp"
}

options {
	language="Cpp";
//	genHashLines = false;
}

class L extends Lexer("UnicodeCharScanner");

options
{
	// Allow any char but \uFFFF (16 bit -1)
	// hmmm antlr does not allow \u10FFFE
	charVocabulary='\u0000'..'\uFFFE';
	noConstructors = true;
}

{
public:
	bool done;

	L( std::istream& in )
	: UnicodeCharScanner(new UnicodeCharBuffer(in),true)
	{
		initLiterals();
	}
	L( UnicodeCharBuffer& ib )
	: UnicodeCharScanner(ib,true)
	{
		initLiterals();
	}

	void uponEOF()
	{
		done = true;
	}
}

WORD : ( ~(' '|'\n'|'\r'|'\t') )+
	;

WS	:
(' '
|'\n'	{ newline(); } ('\r')?
|'\r'	{ newline(); }
|'\t' { tab(); }
)
	{$setType(ANTLR_USE_NAMESPACE(antlr)Token::SKIP);}
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
	|	'0'..'9'
	;
