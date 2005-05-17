#include <iostream>
#include <fstream>

#include "LangLexer.hpp"
#include "LangParser.hpp"
#include "LangWalker.hpp"

int main()
{
	ANTLR_USING_NAMESPACE(std)
	ANTLR_USING_NAMESPACE(antlr)
	try {
		LangLexer lexer(cin);
		LangParser parser(lexer);
		// set up the ast factory to use a custom AST type per default
		// note that here the Ref prefix for the reference counter is
		// strippped off.
		ASTFactory ast_factory("MyAST", MyAST::factory);

		// let the parser add it's stuff to the factory...
		parser.initializeASTFactory(ast_factory);
		parser.setASTFactory(&ast_factory);

		parser.block();

		cout << parser.getAST()->toStringList() << endl;

		LangWalker walker;
		// these two are  not really necessary
		// since we're not building an AST
		walker.initializeASTFactory(ast_factory);
		walker.setASTFactory(&ast_factory);

		walker.block(RefMyAST(parser.getAST()));	// walk tree
		cout << "done walking" << endl;

#if 0
	// disabled until configure/Makefile stuff stabilizes again
		cout << "Writing AST" << endl;
		ofstream xmlfile("out.xml");
		if( xmlfile )
		{
			xmlfile << parser.getAST();
			xmlfile.close();
		}
		cout << "Reading AST back" << endl;
		ifstream infile("out.xml");

		RefAST read_ast = ast_factory.LoadAST(infile);
		if( ! parser.getAST()->equalsList(read_ast) )
			cout << "AST's didn't match" << endl;
		else
			cout << "AST's matched!" << endl;
#endif
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
