options
{
	language = "CSharp";
}

class T extends Lexer;
options {
	k=2;
	filter=IGNORE;
	charVocabulary = '\3'..'\177';
}

P : "<p>" ;
BR: "<br>" ;

protected
IGNORE
	:	'<' (~'>')* '>' {Console.Out.WriteLine("invalid tag: "+$getText);}
	|	( "\r\n" | '\r' | '\n' ) {newline();}
	|	.
	;
