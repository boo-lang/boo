header {
#include "Main.hpp"
#include <string>
#include <iostream>
}

options {
	language = Cpp;
}

class PParser extends Parser;
{
public:
	void traceOut(ANTLR_USE_NAMESPACE(std)string rname) /*throws TokenStreamException*/  {
		ANTLR_USE_NAMESPACE(std)cout << "exit " << rname << "; LT(1)=" << LT(1) << ANTLR_USE_NAMESPACE(std)endl;
	}
	void traceIn(ANTLR_USE_NAMESPACE(std)string rname) /*throws TokenStreamException*/  {
		ANTLR_USE_NAMESPACE(std)cout << "enter " << rname << "; LT(1)=" << LT(1) << ANTLR_USE_NAMESPACE(std)endl;
	}
/*
	public void consume() throws IOException {
		try {
			System.out.println(LT(1));
		}
		catch (IOException ignore) {}
		super.consume();
	}
*/
}

startRule
	:	( decl )+
	;

decl:	INT a:ID {ANTLR_USE_NAMESPACE(std)cout << "decl " << a->getText() << ANTLR_USE_NAMESPACE(std)endl;}
		( COMMA b:ID {ANTLR_USE_NAMESPACE(std)cout << "decl " << b->getText() << ANTLR_USE_NAMESPACE(std)endl;} )*
		SEMI
	;

{
#include <fstream>
#include "PParser.hpp"
}

class PLexer extends Lexer;
options {
	charVocabulary = '\3'..'\377';
	k=2;
}

tokens {
	INT="int";
}

{
public:
	void uponEOF() /*throws TokenStreamException, CharStreamException*/ {
		if ( selector.getCurrentStream() != mainLexer ) {
			// don't allow EOF until main lexer.  Force the
			// selector to retry for another token.
			selector.pop(); // return to old lexer/stream
			selector.retry();
		}
		else {
			ANTLR_USE_NAMESPACE(std)cout << "Hit EOF of main file" << ANTLR_USE_NAMESPACE(std)endl;
		}
	}
}

SEMI:	';'
	;

COMMA
	:	','
	;

ID
	:	('a'..'z')+
	;

INCLUDE
	:	"#include" (WS_)? f:STRING
		{
		ANTLR_USING_NAMESPACE(std)
		// create lexer to handle include
		string name = f->getText();
		ifstream* input = new ifstream(name.c_str());
		if (!*input) {
			cerr << "cannot find file " << name << endl;
		}
		PLexer* sublexer = new PLexer(*input);
		// make sure errors are reported in right file
		sublexer->setFilename(name);
		parser->setFilename(name);

		// you can't just call nextToken of sublexer
		// because you need a stream of tokens to
		// head to the parser.  The only way is
		// to blast out of this lexer and reenter
		// the nextToken of the sublexer instance
		// of this class.

		selector.push(sublexer);
		// ignore this as whitespace; ask selector to try
		// to get another token.  It will call nextToken()
		// of the new instance of this lexer.
		selector.retry(); // throws TokenStreamRetryException
		}
	;

STRING
	:	'"'! ( ~'"' )* '"'!
	;

WS_	:	(	' '
		|	'\t'
		|	'\f'
			// handle newlines
		|	(	options {generateAmbigWarnings=false;}
			:	"\r\n"  // Evil DOS
			|	'\r'    // Macintosh
			|	'\n'    // Unix (the right way)
			)
			{ newline(); }
		)+
		{ $setType(ANTLR_USE_NAMESPACE(antlr)Token::SKIP); }
	;
