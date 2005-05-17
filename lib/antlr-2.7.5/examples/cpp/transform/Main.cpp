
#include <iostream>

#include "CalcLexer.hpp"
#include "CalcParser.hpp"
#include "CalcTreeWalker.hpp"

int main()
{
	ANTLR_USING_NAMESPACE(std)
	ANTLR_USING_NAMESPACE(antlr)
	try
	{
		CalcLexer lexer(cin);
		CalcParser parser(lexer);

		ASTFactory ast_factory;
		parser.initializeASTFactory(ast_factory);
		parser.setASTFactory(&ast_factory);

		// Parse the input expression
		parser.expr();
		RefAST t = parser.getAST();
		// Print the resulting tree out in LISP notation
		cout << t->toStringList() << endl;
		CalcTreeWalker walker;

		// if the walker happens to transform things and defines AST nodes
		// of itself, then it's necessary to have these added to the factory.
		walker.initializeASTFactory(ast_factory);
		walker.setASTFactory(&ast_factory);

		// Traverse the tree created by the parser
		walker.expr(t);
		t = walker.getAST();
		cout << t->toStringList() << endl;
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
