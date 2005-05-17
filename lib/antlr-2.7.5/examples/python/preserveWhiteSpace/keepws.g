// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

header {
    import keepws
}

options {
    language=Python;
}
class keepws_p extends Parser;
options {
    buildAST = true;
    k=2;
}

tokens {
    CALL; // define imaginary token CALL
}

slist
    :   ( stat )+
    ;

stat:   LBRACE^ (stat)+ RBRACE
    |   "if"^ expr "then" stat ("else" stat)?
    |   ID ASSIGN^ expr SEMI
    |   call
    ;

expr
    :   mexpr (PLUS^ mexpr)*
    ;

mexpr
    :   atom (STAR^ atom)*
    ;

atom:   INT
    |   ID
    ;

call:   ID LPAREN (expr)? RPAREN SEMI
        {#call = #(#[CALL,"CALL"], #call);}
    ;

class keepws_l extends Lexer;
options {
    charVocabulary = '\3'..'\377';
}

WS  :   (' '
        |   '\t'
        |   ('\n'|'\r'('\n')?) {$newline;}
        )+
    ;

// Single-line comments
SL_COMMENT
    :   "//"
        (~('\n'|'\r'))* ('\n'|'\r'('\n')?)
        {$newline;}
    ;

LBRACE: '{'
    ;

RBRACE: '}'
    ;

LPAREN: '('
    ;

RPAREN: ')'
    ;

STAR:   '*'
    ;

PLUS:   '+'
    ;

SEMI:   ';'
    ;

ASSIGN
    :   '='
    ;

protected
DIGIT
    :   '0'..'9'
    ;

INT :   (DIGIT)+
    ;

ID  :   ('a'..'z')+
    ;

class keepws_w extends TreeParser;

slist
    :   {keepws.dumpHidden(keepws.getstream().getInitialHiddenToken());}
        (stat)+
    ;

stat:   #(LBRACE {keepws.pr(#LBRACE);} (stat)+ RBRACE {keepws.pr(#RBRACE);})
    |   #(i:"if" {keepws.pr(i);} expr t:"then" {keepws.pr(t);} stat (e:"else" {keepws.pr(e);} stat)?)
    |   #(ASSIGN ID {keepws.pr(#ID); keepws.pr(#ASSIGN);} expr SEMI {keepws.pr(#SEMI);} )
    |   call
    ;

expr
    :   #(PLUS expr {keepws.pr(#PLUS);} expr)
    |   #(STAR expr {keepws.pr(#STAR);} expr)
    |   INT {keepws.pr(#INT);}
    |   ID  {keepws.pr(#ID);}
    ;

call:   {
        self.callDumpInstrumentation(#call);
        }
        #(CALL ID {keepws.pr(#ID);}
          LPAREN {keepws.pr(#LPAREN);} (expr)? RPAREN {keepws.pr(#RPAREN);}
          SEMI
          {
          keepws.write(#SEMI.getText())
          keepws.write("}")
          keepws.dumpHidden(#SEMI.getHiddenAfter())
          }
        )
    ;

/** Dump instrumentation for a call statement.
 *  The reference to rule expr prints out the arg
 *  and then at the end of this rule, we close the
 *  generated called to dbg.invoke().
 */
callDumpInstrumentation
    :   #(CALL id:ID
          {keepws.write("{dbg.invoke(\""+id.getText()+"\", \"");}
          LPAREN (e:expr)? RPAREN SEMI
          {keepws.write("\"); ");}
        )
    ;

