/* This grammar demonstrates the use of two parsers sharing a token
 * vocabulary with a single lexer.
 */

header {
# empty header
}

options {
    language="Python";
}

class SimpleParser extends Parser;

options {
    k=1;
    importVocab=Simple;
}

simple:  ( x )+
	;

x:
	(a) =>  a
|	b
;

a :  A B C;
b : A B D;
