
#include <iostream>

#include "antlr/CommonASTWithHiddenTokens.hpp"
#include "antlr/CommonHiddenStreamToken.hpp"
#include "InstrLexer.hpp"
#include "InstrParser.hpp"
#include "InstrTreeWalker.hpp"

ANTLR_USING_NAMESPACE(std)
ANTLR_USING_NAMESPACE(antlr)

int main()
{
	// make lexer that generates CommonHiddenStreamToken's
	InstrLexer lexer(cin);
	lexer.setTokenObjectFactory(&CommonHiddenStreamToken::factory);

	TokenStreamHiddenTokenFilter filter(lexer);
	filter.hide(InstrParser::WS_);
	filter.hide(InstrParser::SL_COMMENT);

	// make parser with custom ASTFactory generating CommonASTWithHiddenTokens
	InstrParser parser(filter);

	// make factory
	ASTFactory my_factory("CommonASTWithHiddenTokens",
								 &CommonASTWithHiddenTokens::factory);

	// let the parser initialize the factory
	parser.initializeASTFactory(my_factory);
	// tell the parser about the factory
	parser.setASTFactory(&my_factory);

	try
	{
		// Parse the input statements
		parser.slist();
	}
	catch(ANTLRException& e)
	{
		cerr << "exception: " << e.getMessage() << endl;
		return -1;
	}

	RefAST t = parser.getAST();

	InstrTreeWalker walker;
	walker.setFilter(filter);
	// This is only needed for walkers that modify the tree.
	// so it's kindoff redundant...
	walker.setASTFactory(&my_factory);
	//walker.initializeASTFactory();

	try
	{
		walker.slist(t);
	}
	catch(ANTLRException& e)
	{
		cerr << "exception: " << e.getMessage() << endl;
		return -1;
	}
	return 0;
}

void InstrTreeWalker::setFilter(TokenStreamHiddenTokenFilter& filter_)
{
	filter = &filter_;
}

/** walk list of hidden tokens in order, printing them out */
void InstrTreeWalker::dumpHidden(RefToken t)
{
	for ( ; t ; t=filter->getHiddenAfter(t) )
	{
		cout << t->getText();
	}
}

void InstrTreeWalker::pr(RefAST p)
{
	cout << p->getText();
	dumpHidden( (RefCommonASTWithHiddenTokens(p))->getHiddenAfter() );
}
