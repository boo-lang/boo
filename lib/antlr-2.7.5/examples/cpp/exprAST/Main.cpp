
#include <iostream>

#include "ExprLexer.hpp"
#include "ExprParser.hpp"
#include "antlr/AST.hpp"
#include "antlr/CommonAST.hpp"

int main( int, char** )
{
	ANTLR_USING_NAMESPACE(std)
	ANTLR_USING_NAMESPACE(antlr)
	try {
		ASTFactory my_factory;
		ExprLexer lexer(cin);
		ExprParser parser(lexer);

		parser.initializeASTFactory(my_factory);
		parser.setASTFactory(&my_factory);

		parser.expr();
		RefCommonAST ast = RefCommonAST(parser.getAST());

		if (ast)
			cout << ast->toStringList() << endl;
		else
			cout << "null AST" << endl;
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

