options {
	language="Cpp";
}

{
#include <iostream>
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
IGNORE:
	'<' (~'>')* '>'
	{
		std::cout << "invalid tag: " << $getText << std::endl;
	}
|	( "\r\n" | '\r' | '\n' ) {newline();}
|  '\t' { tab(); }
|	.
;
