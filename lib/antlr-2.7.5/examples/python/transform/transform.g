// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

options {
    language=Python;
}

class transform_p extends Parser;
options {
    buildAST = true;    // uses CommonAST by default
    ASTLabelType = "antlr.CommonAST";
}

expr
    :   mexpr (PLUS^ mexpr)* SEMI!
    ;

mexpr
    :   atom (STAR^ atom)*
    ;

atom:   INT
    ;

class transform_l extends Lexer;

WS  :   (' '
    |   '\t'
    |   '\n'
    |   '\r')
        { $skip; }
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

protected
DIGIT
    :   '0'..'9'
    ;

INT :   (DIGIT)+
    ;

class transform_w extends TreeParser;
options {
    buildAST = true;
    ASTLabelType = "antlr.CommonAST";
}

expr:!  #(PLUS left:expr right:expr)
        {
            if #right.getType()==INT and int(#right.getText()) == 0:
                #expr = #left
            elif #left.getType()==INT and int(#left.getText()) ==0:
                #expr = #right;
            else:
                #expr = #(PLUS, left, right);
        }
    |   #(STAR expr expr)
    |   i:INT
    ;
