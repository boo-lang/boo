/*
 * Different version of the calculator which parses the command line arguments.
 * To do this the argv[] strings are first written to a ostringstream then
 * a istringstream is constructed with the string from the ostringstream and
 * fed to the lexer.
 */

#include <iostream>
#include <sstream>
#include <antlr/AST.hpp>
#include "CalcLexer.hpp"
#include "CalcParser.hpp"
#include "CalcTreeWalker.hpp"

int main( int argc, char*argv[] )
{
	ANTLR_USING_NAMESPACE(std);
	ANTLR_USING_NAMESPACE(antlr);

	if( argc == 1 )
	{
		cerr << argv[0] << ": <expression>" << endl;
		return 1;
	}

	// write the argv strings to a ostringstream...
	ostringstream expr;
	for( int i = 1; i < argc; i++ )
	{
		if( i > 1 && i != (argc-1))
			expr << ' ';
		expr << argv[i];
	}

	istringstream input(expr.str());

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
