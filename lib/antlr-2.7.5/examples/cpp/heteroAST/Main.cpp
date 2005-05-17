
#include <iostream>

#include "CalcLexer.hpp"
#include "CalcParser.hpp"
#include "CalcAST.hpp"

int main()
{
	ANTLR_USING_NAMESPACE(std)
	ANTLR_USING_NAMESPACE(antlr)

	try {
		CalcLexer lexer(cin);
		CalcParser parser(lexer);

		ASTFactory ast_factory;

		parser.initializeASTFactory(ast_factory);
		parser.setASTFactory(&ast_factory);

		// Parse the input expression
		parser.expr();
		RefCalcAST t = RefCalcAST(parser.getAST());

		if( t )
		{
			// Print the resulting tree out in LISP notation
			cout << t->toStringTree() << endl;

			// Compute value and return
			int r = t->value();
			cout << "value is " << r << endl;
		}
		else
			cerr << "Errors encountered during parse.." << endl;
	}
	catch( ANTLRException& e )
	{
		cerr << "ANTLRException: " << e.getMessage() << endl;
		return -1;
	}
	catch(exception& e)
	{
		cerr << "exception: " << e.what() << endl;
		return -1;
	}
	return 0;
}
