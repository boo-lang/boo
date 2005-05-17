// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

header {
    /* import calc module - need to import my AST Nodes */
    import calc
    
    // comment
    pass

    // another comment
}
/* no javadoc comments on file level except for classes and rules */
options {
        language="Python";
    //language="Java";  //<- should at least generate a warning
}

/** go ahead with jdoc comments here ..*/
class CalcParser extends Parser("calc.Parser");
options {
    k=3;
        buildAST = true;
    // contrary to original is this going to change the default
    // node type from CommonAST to calc.Tnode.
    ASTLabelType = "calc.TNode";
    exportVocab=Calc;
}


tokens {
    BODY<AST=calc.BodyNode>;
    STMT;
    EXPR<AST=calc.ExprNode>;
    TOK01;              
    UNARY_MINUS;
        UNARY_PLUS;
}

{
    /* a sample function */
    def hello(self):
        // just print hello ..
        print "hello, world"
    // end of sample function 
}

body
    : "begin"!
        (expr ";"!)+
      "end"!
    ;

id 
    : 
        VALID | TYPID | CLSID
    ;


expr
    : 
        expr1
        { ## = #(#[EXPR,"expr"],##); }
    ;

expr1
    :   
        expr2 ((MINUS^|PLUS^) expr2)*
    ;

expr2
    :   
        expr3 ((MULT^|DIV^) expr3)*
    ;

expr3
    : 
        (   p:PLUS   { #p.setType(UNARY_PLUS)  }
        |   m:MINUS  { #m.setType(UNARY_MINUS) }
        )* expr4
    ;

expr4
    : 
      NUMBER<AST=calc.NumberNode>
    | LPAREN^ expr1 RPAREN!
    ;


/** My tree parser ..*/
class CalcWalker extends TreeParser("calc.Walker");
options {
    //ASTLabelType = "TNode";
}
{
    /* sample function */
    def hello(self):
        print "Hello, world"
}


body returns [s = 0]
{
    r = 0
}
    : 
        ( 
            #(e:EXPR 
                {
                    // comment
                    //
                    //
                    //
                    s = self.expr(e)
                    r = s + r
                    
                    // comment -> you should see print e.toStringTree()
                    /* comment */ print e.toStringTree(),
                    /* a comment spanning 
                       three lines.
                    */
                    /* some comment that spans more
                    than one line */            print "=>",s

                    // comments work fine but there some limitations when  having 
                    // C style comments (/* .. */). In Python a comment ends with
                    // the end of line. Therefore I need to force a '\n' on 
                    // encountering '*/'. That works fine usually but be aware 
                    // something like
                    // 
                    //   x /* comment /* = /* comment /* 1 
                    // 
                    // is legal in C/C++/Java but would end up in Python as
                    //
                    //  x
                    //  # comment
                    //  =
                    //  # comment
                    //  1
                    //
                    // which is of course not valid.
                } 
            ) 
        )*
    ;

expr returns [r = 0]
    : #(EXPR r=expr0())
    ;

expr0 returns [a = 0] {
    b = 0
}
    : UNARY_MINUS a=expr0()         { a=-a }
    | UNARY_PLUS  a=expr0()
    | j:NUMBER                      { a = self.tofloat(j) }
    | #(PLUS   a=expr0() b=expr0()) { a = a + b }
    | #(MINUS  a=expr0() b=expr0()) { a = a - b } 
    | #(MULT   a=expr0() b=expr0()) { a = a * b }
    | #(DIV    a=expr0() b=expr0()) { a = a / b } 
    | #(LPAREN a=expr0())
        ;

/** a javdoc comment */
nullp
{
    i = 1;
    if #nullp == None : 
        return

    print(" error in parser tree .. ")
    print(#nullp.toStringTree())
 
    if i > 0:
        return 
    
}
    : #(INT INT) 
        {
        }
    ;


            /** 
  standard lexer - not of further interest here 


  */
class CalcLexer extends Lexer;

options {
    // className = "Scanner";
}

{
    /* sample function */
    def hello(self):
        print "Hello, world"
}

/** ws is supposed to be skipped as usual.
 *  note that you need to write self.newline().
 *  You could also write $newline instead. Trailing
 *  ';' are not harmful.
 */
WS
        :       ( ' '
                | '\t'
                | '\n' { $newline }
                | "\r\n" { $newline }
                | '\r' { $newline }
                )
                { $skip }
        ;

LPAREN
        :       '('
        ;

RPAREN
        :       ')'
        ;

MULT
        :       '*'
        ;

DIV
        :       '/'
        ;

PLUS
        :       '+'
        ;

MINUS
        :       '-'
        ;

SEMI
        :       ';'
        ;

AND
        :       '&'  
        ;

OR
        :       '|'
        ;

NOT
        :       '!'
        ;

EQ
        :       '='
        ;

protected
DIGIT
        :       '0'..'9'
        ;

protected
INT
        :       (DIGIT)+
        ;


NUMBER
        :       INT ("." INT)?
        ;
        

protected
LOWER
        :       'a'..'z'
        ;

protected
UPPER
        :       'A'..'Z'
        ;

protected
LETTER
        :       UPPER
        |       LOWER
        ;

ID 
        :       LOWER (LETTER|DIGIT|'-')*
                {
                    $setType(VALID)
                }
        |       (UPPER (UPPER|DIGIT|'-')*) => UPPER (UPPER|DIGIT|'-')*
                {
                    $setType(CLSID)
                }
        |       (UPPER (UPPER|LOWER|DIGIT|'-')*)
                {
                    buffer = $getText
                    $setType(TYPID)
                }
        ;
