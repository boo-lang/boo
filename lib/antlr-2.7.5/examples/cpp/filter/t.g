options {
	language="Cpp";
}

class T extends Lexer;
options {
	k=2;
	filter=true;
}

P : "<p>" ;
BR: "<br>" ;

NEWLINE: "\n" 
{
	newline(); 
	$setType(ANTLR_USE_NAMESPACE(antlr)Token::SKIP); 
};

TAB: "\t"
{
	tab();
	$setType(ANTLR_USE_NAMESPACE(antlr)Token::SKIP); 
};
