/* Calculate an expression read from stdin or from the arguments passed to
 * the program (using stringstream's)
 */
#include <iostream>
#include <sstream>
#include "antlr/AST.hpp"
#include "CalcLexer.hpp"
#include "CalcParser.hpp"
#include "CalcTreeWalker.hpp"

int main( int argc, char* argv[] )
{
	ANTLR_USING_NAMESPACE(std)
	ANTLR_USING_NAMESPACE(antlr)
	try
	{
		ostringstream expr;
		istringstream input_string;
		istream *input = &cin;
		const char *filename = "<cin>";

		if( argc > 1 )
		{
			// write the argv strings to a ostringstream...
			for( int i = 1; i < argc; i++ )
			{
				if( i > 1 && i != (argc-1))
					expr << ' ';
				expr << argv[i];
			}
			input_string.str(expr.str());
			input = &input_string;
			filename = "<arguments>";
		}

		CalcLexer lexer(*input);
		lexer.setFilename(filename);

		CalcParser parser(lexer);
		parser.setFilename(filename);

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
