#include <iostream>
#include <fstream>
#include <exception>
#include <antlr/ANTLRException.hpp>

#include "L.hpp"

using std::cin;
using std::cerr;
using std::endl;
using std::istream;
using std::ifstream;

int main(int argc, char **argv)
{
	ifstream s;

	if( argc == 1 )
	{
		cerr << "Usage: " << argv[0] << "<files>|-" << endl;
		return 1;
	}

	try {
		for( int i = 1; i < argc; i++ )
		{
			istream *in = 0;
			if( argv[i][0] == '-' && strlen(argv[i]) == 1 )
				in = &cin;
			else
			{
				s.open(argv[i]);
				in = &s;
			}

			UnicodeCharBuffer input(*in);
			L lexer(input);
			lexer.done = false;

			while ( ! lexer.done )
			{
				antlr::RefToken t = lexer.nextToken();
				std::cout << "Token: " << t->getType() << " '" << t->getText() << "'" << std::endl;
			}
			s.close();
		}
	}
	catch (antlr::ANTLRException& e) {
		cerr << "parser exception: " << e.toString() << endl;
		return 2;
	}
	catch (std::exception& e) {
		cerr << "exception: " << e.what() << endl;
		return 2;
	}
	return 0;
}
