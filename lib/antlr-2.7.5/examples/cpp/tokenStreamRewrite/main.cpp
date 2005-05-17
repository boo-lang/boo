#include <iostream>
#include <fstream>

#include <antlr/TokenWithIndex.hpp>
#include <antlr/TokenStreamRewriteEngine.hpp>

#include "TinyCLexer.hpp"
#include "TinyCParser.hpp"

int main( int argc, char **argv )
{
	try {
		TinyCLexer lexer(ANTLR_USE_NAMESPACE(std)cin);
		ANTLR_USE_NAMESPACE(std)ifstream file;

		if ( argc > 1 )
		{
			file.open( argv[1] );
			if( ! file.is_open() )
			{
				ANTLR_USE_NAMESPACE(std)cerr << "Error opening file : " << argv[1] << ANTLR_USE_NAMESPACE(std)endl;
				return -1;
			}

			lexer.getInputState()->initialize(file,argv[1]);
		}

		lexer.setTokenObjectFactory(ANTLR_USE_NAMESPACE(antlr)TokenWithIndex::factory);

		ANTLR_USE_NAMESPACE(antlr)TokenStreamRewriteEngine rewriteEngine(lexer);

		rewriteEngine.discard(TinyCLexer::WS); // does nothing ?

		TinyCParser parser(rewriteEngine);
		parser.program();
		rewriteEngine.toStream(ANTLR_USE_NAMESPACE(std)cout);
		//ANTLR_USE_NAMESPACE(std)cout << "------------------------" << ANTLR_USE_NAMESPACE(std)endl;
		//rewriteEngine.toDebugStream(ANTLR_USE_NAMESPACE(std)cout);
	}
	catch( ANTLR_USE_NAMESPACE(antlr)ANTLRException& e )
	{
		ANTLR_USE_NAMESPACE(std)cerr << "parser exception: " << e.getMessage() << ANTLR_USE_NAMESPACE(std)endl;
		return -1;
	}
	catch( ANTLR_USE_NAMESPACE(std)exception& e )
	{
  		ANTLR_USE_NAMESPACE(std)cerr << "exception: " << e.what() << ANTLR_USE_NAMESPACE(std)endl;
		return -1;
	}
	return 0;
}
