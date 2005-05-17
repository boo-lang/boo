
#include <iostream>

#include "TinyCLexer.hpp"
#include "TinyCParser.hpp"

int main()
{
	ANTLR_USING_NAMESPACE(std)
	ANTLR_USING_NAMESPACE(antlr)

	try {
		TinyCLexer lexer(std::cin);
		TinyCParser parser(lexer);
		parser.program();
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
