options
{
	language = "CSharp";
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

JAVADOC_OPEN
	:	"/**" {MultiLexer.selector.push("doclexer");}
	;

ID	:	('a'..'z')+ ;
SEMI:	';' ;
WS	:	(	' '
		|	'\t'
		|	'\f'
		// handle newlines
		|	(	"\r\n"  // Evil DOS
			|	'\r'    // Macintosh
			|	'\n'    // Unix (the right way)
			)
			{ newline(); }
		)
		{ $setType(Token.SKIP); }
	;

