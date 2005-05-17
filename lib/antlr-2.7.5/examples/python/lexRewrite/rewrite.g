// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

options {
    language=Python;
}

class rewrite_l extends Lexer;

protected
START
options {
    ignore=WS;
}
    :   id:ID ":="! '('! expr:EXPR ')'!
        {
            // can access text matched for any rule
            print "found "+ id.getText() + "," + expr.getText()
            // text will be ID+EXPR minus whitespace
        }
    ;

protected
ID  :   ( let:LETTER { print "letter " + let.getText() } )+
    ;

protected
LETTER
    :   'a'..'z'
        {
        s = $getText;       
        $setText(s.upper()) 
        }
    ;

protected
EXPR:   i:INT!                   // don't include, but i.getText() has access
        {$setText(i.getText())} // effect is if no "!" and no "i:"
    |   ID
    ;

protected
INT :   ('0'..'9')+
    ;

// what if ! on rule itself and invoker had !...should
// rule return anything in the token to the invoker?  NO!
// make sure 'if' is in the right spot
// What about no ! on caller but ! on called rule?
protected
WS! :   (   ' '         // whitespace not saved
        |   '\t'
        |   '\n' {$newline}
        )+
        {$skip}     // way to set token type
    ;


