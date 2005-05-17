#include <iostream>
#include <fstream>

#include <antlr/CommonAST.hpp>

#include "JavaLexer.hpp"
#include "JavaRecognizer.hpp"
#include "JavaTreeParser.hpp"

ANTLR_USING_NAMESPACE(std)
ANTLR_USING_NAMESPACE(antlr)

size_t errors = 0;

static void doTreeAction( ASTFactory& factory, RefAST t)
{
	if ( t == nullAST )
		return;

	JavaTreeParser tparse;
	tparse.initializeASTFactory(factory);
	tparse.setASTFactory(&factory);

	try {
		tparse.compilationUnit(t);
	}
	catch (ANTLRException& e) {
		cerr << e.toString() << endl;
	}
}

// Here's where we do the real work...
static void parseFile(const string& f)
{
	try
	{
		ifstream s(f.c_str());

		// Create a scanner that reads from the input stream
		JavaLexer lexer(s);
		lexer.setFilename(f);

/*
		while (true) {
			RefToken t = lexer.nextToken();
			if (t->getType() == Token::EOF_TYPE)
				break;
			cout << t->getText() << ":" << t->getType() << endl;
		}
*/

		// Create a parser that reads from the scanner
		JavaRecognizer parser(lexer);
		parser.setFilename(f);

		// make an ast factory
		ASTFactory ast_factory;

		// initialize and put it in the parser...
		parser.initializeASTFactory(ast_factory);
		parser.setASTFactory(&ast_factory);

		// start parsing at the compilationUnit rule
		parser.compilationUnit();

		// do something with the tree
		doTreeAction( ast_factory, parser.getAST() );
	}
	catch (ANTLRException& e) {
		cerr << "parser exception: " << e.toString() << endl;
		errors++;
	}
	catch (exception& e) {
		cerr << "exception: " << e.what() << endl;
		errors++;
	}
}

int main(int argc,char* argv[])
{
	// Use a try/catch block for parser exceptions
	try
	{
		// if we have at least one command-line argument
		if ( argc > 1 )
		{
			cerr << "Parsing..." << endl;

			// for each file specified on the command line
			for(int i=1; i< argc;i++)
			{
				cerr << "   " << argv[i] << endl;
				parseFile(argv[i]);
			}
		}
		else
			cerr << "Usage: " << argv[0] << " <file name(s)>" << endl;
	}
	catch(exception& e) {
		cerr << "exception: " << e.what() << endl;
		return -1;
	}
	if( errors )
		return -1;

	return 0;
}
