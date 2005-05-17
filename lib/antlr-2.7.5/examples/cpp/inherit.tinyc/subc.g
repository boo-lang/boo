options {
	mangleLiteralPrefix = "TK_";
	language="Cpp";
}

class MyCParser extends TinyCParser;

 
// add initializers to variables
variable
{
	// init action
}
	:	type declarator (ASSIGN aexpr)? SEMI
	;

