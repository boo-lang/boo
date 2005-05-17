
#include <iostream>

#include "SimpleLexer.hpp"
#include "SimpleParser.hpp"
#include "SimpleParser2.hpp"

int main()
{
	ANTLR_USING_NAMESPACE(std);
	ANTLR_USING_NAMESPACE(antlr);
	try
	{
		SimpleLexer lexer(cin);

		/* Invoke first parser */
		cout << "first parser" << endl;
		SimpleParser parser(lexer);
		parser.simple();

		/* Now we need to get the inputState from the first parser
		 * this includes data about guessing and stuff like it.
		 * If we don't do this and create the second parser
		 * with just the lexer object we might (doh! will!) miss tokens
		 * read for lookahead tests.
		 */
		ParserSharedInputState inputstate(parser.getInputState());

		/* When first parser runs out, invoke secnond parser */
		cout << "second parser" << endl;
		SimpleParser2 parser2(inputstate);
		parser2.simple();
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

