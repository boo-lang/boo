// This file is part of PyANTLR. See LICENSE.txt for license
// details..........Copyright (C) Wolfgang Haefelinger, 2004.
//
// $Id$

options {
    language=Python;
}

class ASTsupportParser extends Parser;
options {
    buildAST = true;
}

/*  Test the equals, equalsSubtree, and findAll methods plus AST enumeration.
 *  The output should be: 
    t is  ( = a 1 )
    u is  ( = b )
    v is  ( = 4 )
    w is  =
    
    t.equalsTree(t) is true
    t.equalsTree(u) is false
    t.equalsTree(v) is false
    t.equalsTree(w) is false
    t.equalsTree(null) is false
    
    t.equalsTreePartial(t) is true
    t.equalsTreePartial(u) is false
    t.equalsTreePartial(v) is false
    t.equalsTreePartial(w) is true
    t.equalsTreePartial(null) is true
    
    a is  ( A ( B C ( A B ) ) ( A B ) ( F ( A B ) ) ( A ( A B ) ) ) J
    
                  A---------------------J
                  |
                  B-----A-----F----A
                  |     |     |    |
                  C--A  B     A    A
                     |        |    |
                     B        B    B
    
    x is second sibling of upperleftmost A:  ( A B ) ( F ( A B ) ) ( A ( A B ) )
    y is child B of x:  B
    x.equalsTree(#(A B)) is true
    x.equalsList(#(A B)) is false
    x.equalsListPartial(#(A B)) is true
    a.equalsTree(#(A B)) is false
    a.equalsTreePartial(#(A B)) is true
    y.equalsList(#[B]) is true
    y.equalsListPartial(#[B]) is true
    
    a.findAllPartial(#(A B)):
     ( A ( B C ( A B ) ) ( A B ) ( F ( A B ) ) ( A ( A B ) ) ) J
     ( A B )
     ( A B ) ( F ( A B ) ) ( A ( A B ) )
     ( A B )
     ( A B )
    
    a.findAllPartial(#[A])):
     ( A ( B C ( A B ) ) ( A B ) ( F ( A B ) ) ( A ( A B ) ) ) J
     ( A B )
     ( A B ) ( F ( A B ) ) ( A ( A B ) )
     ( A B )
     ( A ( A B ) )
     ( A B )
    
    a.findAll(#(A B)):
     ( A B )
     ( A B ) ( F ( A B ) ) ( A ( A B ) )
     ( A B )
     ( A B )
    
    
    Test results:
    equalsTree is ok
    equalsTreePartial is ok
    equalsList is ok
    equalsListPartial is ok
 */

{
    def main(self):
        astFactory = antlr.ASTFactory()
        t = #([ASSIGN,"="], [ID,"a"], [INT,"1"])
        print("t is " + t.toStringList())
        u = #([ASSIGN,"="], [ID,"b"])
        print("u is " + u.toStringList())
        v = #([ASSIGN,"="], [INT,"4"])
        print("v is " + v.toStringList())
        w = #[ASSIGN,"="]
        print("w is " + w.toStringList())
        print("")
        r1=t.equalsTree(t);print "t.equalsTree(t) is ",r1
        r2=t.equalsTree(u);print "t.equalsTree(u) is ",r2
        r3=t.equalsTree(v);print "t.equalsTree(v) is ",r3
        r4=t.equalsTree(w);print "t.equalsTree(w) is ",r4
        r5=t.equalsTree(None);print "t.equalsTree(None) is ",r5
        print("")
        r6=t.equalsTreePartial(t);print "t.equalsTreePartial(t) is ",r6
        r7=t.equalsTreePartial(u);print "t.equalsTreePartial(u) is ",r7
        r8=t.equalsTreePartial(v);print "t.equalsTreePartial(v) is ",r8
        r9=t.equalsTreePartial(w);print "t.equalsTreePartial(w) is ",r9
        r10=t.equalsTreePartial(None);print "t.equalsTreePartial(None) is ",r10
        print("")
        a = #(None,
                    ([A,"A"],
                        ([B,"B"], [C,"C"], ([A,"A"],[B,"B"])),
                        ([A,"A"],[B,"B"]),
                        ([F,"F"], #([A,"A"], [B,"B"])),
                        ([A,"A"], #([A,"A"], [B,"B"]))),
                    [J,"J"]) 
        print("a is "+a.toStringList()+"\n")
        print("              A---------------------J")
        print("              |")
        print("              B-----A-----F----A")
        print("              |     |     |    |")
        print("              C--A  B     A    A")
        print("                 |        |    |")
        print("                 B        B    B\n")
        x = a.getFirstChild().getNextSibling()
        print("x is second sibling of upperleftmost A: "+x.toStringList())
        y = a.getFirstChild().getNextSibling().getFirstChild()
        print("y is child B of x: "+y.toStringList())
        r11=x.equalsTree(#([A,"A"],[B,"B"]));print "x.equalsTree(#(A B)) is ",r11
        r12=x.equalsList(#([A,"A"],[B,"B"]));print "x.equalsList(#(A B)) is ",r12
        r13=x.equalsListPartial(#([A,"A"],[B,"B"]));print "x.equalsListPartial(#(A B)) is ",r13
        r14=a.equalsTree(#([A,"A"],[B,"B"]));print "a.equalsTree(#(A B)) is ",r14
        r15=a.equalsTreePartial(#([A,"A"],[B,"B"]));print "a.equalsTreePartial(#(A B)) is ",r15
        r16=y.equalsList(#[B,"B"]);print "y.equalsList(#[B]) is ",r16
        r17=y.equalsList(#[B,"B"]);print "y.equalsListPartial(#[B]) is ",r17
        print("\na.findAllPartial(#(A B)):")
        enum = a.findAllPartial(#([A,"A"],[B,"B"]))
        for e in enum:  print (e.toStringList())
        print("\na.findAllPartial(#[A])):")
        enum = a.findAllPartial(#[A,"A"])
        for e in enum:  print (e.toStringList())
        print("\na.findAll(#(A B)):")
        enum = a.findAll(#([A,"A"],[B,"B"]))
        for e in enum:  print (e.toStringList())
        print("\nTest results:")
        if r1 and not r2 and not r3 and not r4 and \
           not r5 and r11 and not r14:
            print("equalsTree is ok")
        else:
            print("equalsTree is bad")
        if r6 and not r7 and not r8 and r9 and r10:
            print("equalsTreePartial is ok")
        else:
            print("equalsTreePartial is bad")
        if not r12 and r16:
            print("equalsList is ok")
        else :
            print("equalslist is bad")
        if r13 and r17:
            print("equalsListPartial is ok")
        else :
            print("equalslistPartial is bad")

}

defTokenTypes
    :   ID INT ASSIGN PLUS A B C D E F G H I J K
    ;

