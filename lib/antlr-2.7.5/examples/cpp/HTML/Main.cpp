/*
	Simple class for testing antlr-generated HTML parser/lexer.
	Alexander Hinds, Magelang Institute
	ahinds@magelang.com

*/

#include <iostream>
#include "HTMLLexer.hpp"
#include "HTMLParser.hpp"
#include "antlr/TokenBuffer.hpp"

int main( int, char** )
{
	ANTLR_USING_NAMESPACE(std)
	ANTLR_USING_NAMESPACE(antlr)
	try {
		HTMLLexer lexer(cin);
		TokenBuffer buffer(lexer);
		HTMLParser parser(buffer);
		parser.document();
	}
	catch( ANTLRException& e )
	{
		cerr << "exception: " << e.getMessage() << endl;
		return -1;
	}
	catch( exception& e )
	{
		cerr << "exception: " << e.what() << endl;
		return -1;
	}
	return 0;
}

