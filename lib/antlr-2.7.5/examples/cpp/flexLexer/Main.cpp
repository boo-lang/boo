#include <iostream>
#include <fstream>

#include "LexTokenStream.hpp"
#include "JavaRecognizer.hpp"
#include "JavaTreeParser.hpp"

ANTLR_USING_NAMESPACE(std)
ANTLR_USING_NAMESPACE(antlr)

static void parseFile(const string& f);
static void doTreeAction(const string& f, RefAST t, ASTFactory* factory);

int main(int argc,char* argv[])
{
	// Use a try/catch block for parser exceptions
	try {
		// if we have at least one command-line argument
		if ( argc > 1 )
		{
			cerr << "Parsing..." << endl;

			// for each file specified on the command line
			for( int i = 1; i < argc; i++ )
			{
				cerr << "   " << argv[i] << endl;
				parseFile(argv[i]);
			}
		}
		else
		{
			cerr << "Usage: " << argv[0]
				  << " <file name(s)>" << endl;
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

// Here's where we do the real work...
static void parseFile(const string& f)
{
	try {
		FILE* fp = fopen(f.c_str(),"r");

		// Create a scanner that reads from the input stream
		LexTokenStream lexer(fp);

		// Create a parser that reads from the scanner
		JavaRecognizer parser(lexer);

		ASTFactory ast_factory;
		parser.initializeASTFactory(ast_factory);
		parser.setASTFactory(&ast_factory);

		// start parsing at the compilationUnit rule
		parser.compilationUnit();

		fclose(fp);

		// do something with the tree
		doTreeAction( f, parser.getAST(), &ast_factory );
	}
	catch (exception& e) {
		cerr << "parser exception: " << e.what() << endl;
	}
}

static void doTreeAction(const string&, RefAST t, ASTFactory* factory )
{
	if ( t==nullAST )
		return;

	JavaTreeParser tparse;
	tparse.initializeASTFactory(*factory);
	tparse.setASTFactory(factory);

	try {
		tparse.compilationUnit(t);
		// System.out.println("successful walk of result AST for "+f);
	}
	catch (RecognitionException& e) {
		cerr << e.getMessage() << endl;
	}
}

