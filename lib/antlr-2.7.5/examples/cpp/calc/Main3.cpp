/*
 * Different version of the calculator which parses the 
 * first command line argument. E.g. pass your expression as ./calc3 "1+1;"
 * argv[1] is mapped to a CharInputBuffer and then fed to the lexer
 */

#include <iostream>
#include <antlr/AST.hpp>
#include <antlr/CharInputBuffer.hpp>
#include "CalcLexer.hpp"
#include "CalcParser.hpp"
#include "CalcTreeWalker.hpp"

int main( int argc, char*argv[] )
{
	ANTLR_USING_NAMESPACE(std);
	ANTLR_USING_NAMESPACE(antlr);

	if( argc != 2 )
	{
		cerr << argv[0] << ": \"<expression>\"" << endl;
		return 1;
	}

	CharInputBuffer input( reinterpret_cast<unsigned char*>(argv[1]), strlen(argv[1]) );

	try
	{
		CalcLexer lexer(input);
		lexer.setFilename("<arguments>");

		CalcParser parser(lexer);
		parser.setFilename("<arguments>");

		ASTFactory ast_factory;
		parser.initializeASTFactory(ast_factory);
		parser.setASTFactory(&ast_factory);

		// Parse the input expression
		parser.expr();
		RefAST t = parser.getAST();
		if( t )
		{
			// Print the resulting tree out in LISP notation
			cout << t->toStringTree() << endl;
			CalcTreeWalker walker;

			// Traverse the tree created by the parser
			float r = walker.expr(t);
			cout << "value is " << r << endl;
		}
		else
			cout << "No tree produced" << endl;
	}
	catch(ANTLRException& e)
	{
		cerr << "Parse exception: " << e.toString() << endl;
		return -1;
	}
	catch(exception& e)
	{
		cerr << "exception: " << e.what() << endl;
		return -1;
	}
	return 0;
}
