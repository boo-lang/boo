
#include <iostream>

#include "T.hpp"

int main( int, char** )
{
	ANTLR_USING_NAMESPACE(std)
	ANTLR_USING_NAMESPACE(antlr)

	try
	{
		T lexer(cin);

		for(;;)
		{
			RefToken t = lexer.nextToken();

			if ( t->getType() == Token::EOF_TYPE )
				break;

			cout << "Token: " << t->toString() << endl;
		}
		cout << "done lexing..." << endl;
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
