
#include <iostream>

#include "DemoJavaLexer.hpp"
#include "DemoJavaDocLexer.hpp"
#include "DemoJavaParser.hpp"
#include "antlr/TokenStreamSelector.hpp"

int main()
{
	ANTLR_USING_NAMESPACE(std)
	ANTLR_USING_NAMESPACE(antlr)

	// Define a selector that can switch from java to javadoc
	TokenStreamSelector selector;

	try {
		// attach java lexer to the input stream, which also creates a shared input state object
		DemoJavaLexer main(cin);
		main.setSelector(&selector);

		// create javadoc lexer; attach to same shared input state as java lexer
		DemoJavaDocLexer doclexer(main.getInputState());
		doclexer.setSelector(&selector);

		// notify selector about various lexers; name them for convenient reference later
		selector.addInputStream(&main, "main");
		selector.addInputStream(&doclexer, "doclexer");
		selector.select("main"); // start with main java lexer

		// Create parser attached to selector
		DemoJavaParser parser(selector);

		// Pull in one or more int decls with optional javadoc
		parser.input();

		// spin thru all tokens generated via the SELECTOR.
		RefToken t;
		while ( (t=selector.nextToken())->getType()!=Token::EOF_TYPE ) {
			cout << t->toString() << endl;
		}
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

