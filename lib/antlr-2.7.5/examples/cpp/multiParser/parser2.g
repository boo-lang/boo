/* This grammar demonstrates the use of two parsers sharing a token
 * vocabulary with a single lexer.
 */

header {
/* empty header */
}

options {
	language=Cpp;
}

class SimpleParser2 extends Parser;
options {
	k=3;
	importVocab=Simple;
}

simple : (x)+;
x 		 : (a | b);

a :  C B A;
b : D B A;
