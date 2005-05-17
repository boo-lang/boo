header {
#include "antlr/TokenStreamSelector.hpp"
}

options {
	language="Cpp";
}

class DemoJavaLexer extends Lexer;
options {
	k=2;
	importVocab = Common;
	exportVocab = Java;
}

tokens {
	INT="int";
}

{
private:
	ANTLR_USE_NAMESPACE(antlr)TokenStreamSelector* selector;
public:
	void setSelector(ANTLR_USE_NAMESPACE(antlr)TokenStreamSelector* selector_) {
		selector=selector_;
	}
}

JAVADOC_OPEN
	:	"/**" {selector->push("doclexer");}
	;

ID	:	('a'..'z')+ ;
SEMI:	';' ;
WS_	:	(	' '
		|	'\t'
		|	'\f'
		// handle newlines
		|	(	"\r\n"  // Evil DOS
			|	'\r'    // Macintosh
			|	'\n'    // Unix (the right way)
			)
			{ newline(); }
		)
		{ $setType(ANTLR_USE_NAMESPACE(antlr)Token::SKIP); }
	;

