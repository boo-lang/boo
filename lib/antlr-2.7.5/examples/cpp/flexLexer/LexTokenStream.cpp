//#include <iostream>
#include "LexTokenStream.hpp"
#include "JavaTokenTypes.hpp"
#include "antlr/CommonToken.hpp"

extern "C" {
	extern char* yytext;
	extern char string_buf[];		// used for strings
	extern int yylex();
	extern int yyrestart(std::FILE*);
}

static LexTokenStream* currentLexer=0;

extern "C"
	void newLine()
{
	currentLexer->newLine();
}

LexTokenStream::LexTokenStream(std::FILE* fp)
{
	yyrestart(fp);
	reachedEOF = false;
	line = 1;
}

void LexTokenStream::newLine()
{
	++line;
}

ANTLR_USE_NAMESPACE(antlr)RefToken LexTokenStream::nextToken()
{
	ANTLR_USE_NAMESPACE(antlr)RefToken ret;
	if (!reachedEOF)
	{
		int type;

		currentLexer = this;
		while ((type = yylex()) == -1) // Token::SKIP
			;

		if (type != 0)
		{
			if( type != JavaTokenTypes::STRING_LITERAL )
				ret = ANTLR_USE_NAMESPACE(antlr)RefToken(new ANTLR_USE_NAMESPACE(antlr)CommonToken(type,yytext));
			else
				ret = ANTLR_USE_NAMESPACE(antlr)RefToken(new ANTLR_USE_NAMESPACE(antlr)CommonToken(type,string_buf));

//			ANTLR_USE_NAMESPACE(std)cout << "Read token " << type << " [" << ret->getText() << "]" << ANTLR_USE_NAMESPACE(std)endl;
		}
		else
			reachedEOF = true;
	}
	if (reachedEOF)
	{
		ret = ANTLR_USE_NAMESPACE(antlr)RefToken(new ANTLR_USE_NAMESPACE(antlr)CommonToken(1,"EOF"));
		//		cout << "EOF reached" << endl;
	}
	ret->setLine(line);
	return ret;
}
