#include <iostream>

#include "IDLLexer.hpp"
#include "IDLParser.hpp"

int main()
{
	ANTLR_USING_NAMESPACE(std)
	ANTLR_USING_NAMESPACE(antlr)

	try {
		IDLLexer lexer(cin);
		IDLParser parser(lexer);
		parser.specification();
	}
	catch( ANTLRException& e )
	{
		cerr << "exception: " << e.getMessage() << endl;
		return -1;
	}
	catch(exception& e)
	{
		cerr << "exception: " << e.what() << endl;
		return -1;
	}
	return 0;
}
