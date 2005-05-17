
#include <iostream>

#include "DataLexer.hpp"
#include "DataParser.hpp"

int main(int argc,char* argv[])
{
	ANTLR_USING_NAMESPACE(std)
	ANTLR_USING_NAMESPACE(antlr)
	try {
		DataLexer lexer(cin);
		DataParser parser(lexer);
		parser.file();
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

