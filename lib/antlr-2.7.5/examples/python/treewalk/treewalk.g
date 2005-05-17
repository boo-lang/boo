// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

options {
    language=Python;
}

class treewalk_p extends Parser;

options {
    codeGenMakeSwitchThreshold = 3;
    codeGenBitsetTestThreshold = 4;
    buildAST=true;
}

block
    :   LCURLY^ ( statement )* RCURLY!
    ;

statement
    :   expr SEMI!
    |   "if"^ LPAREN! expr RPAREN! statement
        ( "else"! statement )?
    |   "while"^ LPAREN! expr RPAREN! statement
    |!  b:block { statement_AST = b_AST; }
        // do some manual tree returning
    ;

expr:   assignExpr
    ;

assignExpr
    :   aexpr (ASSIGN^ assignExpr)?
    ;

aexpr
    :   mexpr (PLUS^ mexpr)*
    ;

mexpr
    :   atom (STAR^ atom)*
    ;

atom:   ID
    |   INT
    ;

class treewalk_w extends TreeParser;

block
    :   #( LCURLY ( stat )+ )
    ;

stat:   #("if" expr stat (stat)?)
    |   #("while" expr stat)
    |   expr
    |   block
    ;

expr:   #(ASSIGN expr expr)     {print "found assign"           }
    |   #(PLUS expr expr)       {print "found +"                }
    |   #(STAR expr expr)       {print "found *"                }
    |   a:ID                    {print "found ID " ,a.getText() }
    |   b:INT                   {print "found INT ",b.getText() }
    ;

class treewalk_l extends Lexer;

WS  :   (' '
    |   '\t'
    |   '\n'
    |   '\r')
        { _ttype = SKIP; }
    ;

LPAREN: '('
    ;

RPAREN: ')'
    ;

LCURLY: '{'
    ;

RCURLY: '}'
    ;

STAR:   '*'
    ;

PLUS:   '+'
    ;

ASSIGN
    :   '='
    ;

SEMI:   ';'
    ;

COMMA
    :   ','
    ;

protected
ESC :   '\\'
        (   'n'
        |   'r'
        |   't'
        |   'b'
        |   'f'
        |   '"'
        |   '\''
        |   '\\'
        |   ('0'..'3') ( DIGIT (DIGIT)? )?
        |   ('4'..'7') (DIGIT)?
        )
    ;

protected
DIGIT
    :   '0'..'9'
    ;

INT :   (DIGIT)+
    ;

ID  :   ('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'_'|'0'..'9')*
        {
            try:
              i = literals[self.getText()]
              _ttype =  i;
            except:
              pass
        }
    ;
