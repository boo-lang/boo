options {
	language="Cpp";
}

{
#include <iostream>
}

class DataParser extends Parser;

file:	(	sh:SHORT	{std::cout << sh->getText() << std::endl;}
		|	st:STRING	{std::cout << "\"" << st->getText() << "\"" << std::endl;}
		)+
	;

{
#include "antlr/String.hpp"
}

class DataLexer extends Lexer;
options {
	charVocabulary = '\u0000'..'\u00FF';
}

SHORT
	:	'\0' high:. lo:.
		{
		ANTLR_USING_NAMESPACE(antlr) // to pick up operator+
		int v = (((int)high)<<8) + lo;
		$setText(std::string("")+v);
		}
	;

STRING
	:	'\1'!	// begin string (discard)
		( ~'\2' )*
		'\2'!	// end string (discard)
	;
