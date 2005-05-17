{
import antlr.ASTFactory;
import antlr.collections.ASTEnumeration;
}

class SupportTest extends Parser;
options {
	buildAST = true;
}

{
/** Test the equals, equalsSubtree, and findAll methods plus AST enumeration.
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
	public static void main(String[] args) {
		boolean r1,r2,r3,r4,r5,r6,r7,r8,r9,r10,r11,r12,r13,r14,r15,r16,r17;

		// define "astFactory" so translation of #(...) works
		ASTFactory astFactory = new ASTFactory();
		AST t = #([ASSIGN,"="], [ID,"a"], [INT,"1"]); // build "a=1" tree
		System.out.println("t is " + t.toStringList());
		AST u = #([ASSIGN,"="], [ID,"b"]); // build "b=?" tree
		System.out.println("u is " + u.toStringList());
		AST v = #([ASSIGN,"="], [INT,"4"]); // build "4=?" tree
		System.out.println("v is " + v.toStringList());
		AST w = #[ASSIGN,"="]; // build "=" tree
		System.out.println("w is " + w.toStringList());
		System.out.println("");

		System.out.println("t.equalsTree(t) is " + (r1=t.equalsTree(t)));
		System.out.println("t.equalsTree(u) is " + (r2=t.equalsTree(u)));
		System.out.println("t.equalsTree(v) is " + (r3=t.equalsTree(v)));
		System.out.println("t.equalsTree(w) is " + (r4=t.equalsTree(w)));
		System.out.println("t.equalsTree(null) is " + (r5=t.equalsTree(null)));
		System.out.println("");

		System.out.println("t.equalsTreePartial(t) is " + (r6=t.equalsTreePartial(t)));
		System.out.println("t.equalsTreePartial(u) is " + (r7=t.equalsTreePartial(u)));
		System.out.println("t.equalsTreePartial(v) is " + (r8=t.equalsTreePartial(v)));
		System.out.println("t.equalsTreePartial(w) is " + (r9=t.equalsTreePartial(w)));
		System.out.println("t.equalsTreePartial(null) is " + (r10=t.equalsTreePartial(null)));
		System.out.println("");

		/* (A (B C (A B)) (A B) (F (A B)) (A (A B)) ) J
		   Visually:
              A---------------------J
              |
              B-----A-----F----A
              |     |     |    |
              C--A  B     A    A
                 |        |    |
                 B        B    B
		*/
		AST a = #(null,
					([A,"A"],
						([B,"B"], [C,"C"], ([A,"A"],[B,"B"])),
						([A,"A"],[B,"B"]),
						([F,"F"], #([A,"A"], [B,"B"])),
						([A,"A"], #([A,"A"], [B,"B"]))),
					[J,"J"]); 
		System.out.println("a is "+a.toStringList()+"\n");
		System.out.println("              A---------------------J");
		System.out.println("              |");
		System.out.println("              B-----A-----F----A");
		System.out.println("              |     |     |    |");
		System.out.println("              C--A  B     A    A");
		System.out.println("                 |        |    |");
		System.out.println("                 B        B    B\n");

		AST x = a.getFirstChild().getNextSibling();
		System.out.println("x is second sibling of upperleftmost A: "+x.toStringList());
		AST y = a.getFirstChild().getNextSibling().getFirstChild();
		System.out.println("y is child B of x: "+y.toStringList());
		System.out.println("x.equalsTree(#(A B)) is "+(r11=x.equalsTree(#([A,"A"],[B,"B"]))));
		System.out.println("x.equalsList(#(A B)) is "+(r12=x.equalsList(#([A,"A"],[B,"B"]))));
		System.out.println("x.equalsListPartial(#(A B)) is "+(r13=x.equalsListPartial(#([A,"A"],[B,"B"]))));
		System.out.println("a.equalsTree(#(A B)) is "+(r14=a.equalsTree(#([A,"A"],[B,"B"]))));
		System.out.println("a.equalsTreePartial(#(A B)) is "+(r15=a.equalsTreePartial(#([A,"A"],[B,"B"]))));
		System.out.println("y.equalsList(#[B]) is "+(r16=y.equalsList(#[B,"B"])));
		System.out.println("y.equalsListPartial(#[B]) is "+(r17=y.equalsList(#[B,"B"])));

		ASTEnumeration myenum;
		System.out.println("\na.findAllPartial(#(A B)):");
		myenum = a.findAllPartial(#([A,"A"],[B,"B"]));
		while ( myenum.hasMoreNodes() ) {
			System.out.println(myenum.nextNode().toStringList());
		}

		System.out.println("\na.findAllPartial(#[A])):");
		myenum = a.findAllPartial(#[A,"A"]);
		while ( myenum.hasMoreNodes() ) {
			System.out.println(myenum.nextNode().toStringList());
		}

		System.out.println("\na.findAll(#(A B)):");
		myenum = a.findAll(#([A,"A"],[B,"B"]));
		while ( myenum.hasMoreNodes() ) {
			System.out.println(myenum.nextNode().toStringList());
		}

		// check results
		System.out.println("\nTest results:");
		if ( r1==true && r2==false && r3==false && r4==false &&
			 r5==false && r11==true && r14==false) {
			System.out.println("equalsTree is ok");
		}
		else {
			System.out.println("equalsTree is bad");
		}
		if ( r6==true && r7==false && r8==false && r9==true && r10==true ) {
			System.out.println("equalsTreePartial is ok");
		}
		else {
			System.out.println("equalsTreePartial is bad");
		}
		if ( r12==false && r16==true ) {
			System.out.println("equalsList is ok");
		}
		else {
			System.out.println("equalslist is bad");
		}
		if ( r13==true && r17==true ) {
			System.out.println("equalsListPartial is ok");
		}
		else {
			System.out.println("equalslistPartial is bad");
		}
	}
}

defTokenTypes
	:	ID INT ASSIGN PLUS A B C D E F G H I J K
	;

/*
rule[AST t] : BLAH;

another
{
 #another = on here.	// should translate
}
	:	rule[#another=foo] rule[#another] A
		// should get errors on those rule refs
	;
*/
