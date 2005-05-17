
#include <iostream>

#include "Rewrite.hpp"

int main()
{
	ANTLR_USING_NAMESPACE(std)
	ANTLR_USING_NAMESPACE(antlr)
	try
	{
		Rewrite lexer(cin);
		lexer.mSTART(true);
		cout << "result Token=" << lexer.getTokenObject()->toString() << endl;
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

