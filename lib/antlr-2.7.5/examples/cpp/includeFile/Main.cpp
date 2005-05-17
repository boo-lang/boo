#include <iostream>

#include "Main.hpp"
#include "PLexer.hpp"
#include "PParser.hpp"

ANTLR_USING_NAMESPACE(std)
ANTLR_USING_NAMESPACE(antlr)

// Define a selector that can handle nested include files.
// These variables are public so the parser/lexer can see them.
TokenStreamSelector selector;
PParser* parser;
PLexer* mainLexer;

int main( int, char** )
{
	try {
		// attach java lexer to the input stream,
		mainLexer = new PLexer(cin);

		// notify selector about starting lexer; name for convenience
		selector.addInputStream(mainLexer, "main");
		selector.select("main"); // start with main P lexer

		// Create parser attached to selector
		parser = new PParser(selector);

		// Parse the input language: P
		parser->setFilename("<stdin>");
		parser->startRule();
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
