// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

options {
	mangleLiteralPrefix = "TK_";
    language="Python";
}

class inherit_p extends tinyc_p;

 
// add initializers to variables
variable
{
    pass
}
	:	type declarator (ASSIGN aexpr)? SEMI
	;

