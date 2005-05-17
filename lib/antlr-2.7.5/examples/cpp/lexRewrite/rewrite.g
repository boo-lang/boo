options {
	language="Cpp";
}

{
#include <iostream>
}

class Rewrite extends Lexer;

//protected
START
options {
	ignore=WS_;
}
	:	id:ID ":="! '('! expr:EXPR ')'!
		{
			// can access text matched for any rule
			std::cout << "found " << id->getText() << "," << expr->getText() << std::endl;
			// text will be ID+EXPR minus whitespace
		}
	;

protected
ID	:	( let:LETTER {std::cout << "letter " << let->getText() << std::endl;} )+
	;

protected
LETTER
	:	'a'..'z'
		{
		std::string s = $getText;		// get access text of this rule
//		$setText(s.toUpperCase());	// can reset it too
		$setText(s);	// can reset it too
		}
	;

protected
EXPR:	i:INT!	// don't include, but i->getText() has access
		{$setText(i->getText());} // effect is if no "!" and no "i:"
	|	ID
	;

protected
INT	:	('0'..'9')+
	;

// what if ! on rule itself and invoker had !...should
// rule return anything in the token to the invoker?  NO!
// make sure 'if' is in the right spot
// What about no ! on caller but ! on called rule?
protected
WS_!	:	(	' '			// whitespace not saved
		|	'\t'
		|	'\n' {newline();}
		)+
		{$setType(ANTLR_USE_NAMESPACE(antlr)Token::SKIP);}		// way to set token type
	;


