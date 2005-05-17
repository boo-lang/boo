options {
	mangleLiteralPrefix = "TK_";
}

class MyCParser extends TinyCParser;

 
// add initializers to variables
variable
{
	// init action
}
	:	type declarator (ASSIGN aexpr)? SEMI
	;

