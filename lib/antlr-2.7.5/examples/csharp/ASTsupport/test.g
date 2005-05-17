options {
	language =  "CSharp";
}

{
using IEnumerator			= System.Collections.IEnumerator;
}

class SupportTest extends Parser;
options {
	buildAST = true;
}

{
/** Test the Equals, EqualsSubtree, and findAll methods plus AST enumeration.
 *  The output should be: 
	t is  ( = a 1 )
	u is  ( = b )
	v is  ( = 4 )
	w is  =
	
	t.EqualsTree(t) is true
	t.EqualsTree(u) is false
	t.EqualsTree(v) is false
	t.EqualsTree(w) is false
	t.EqualsTree(null) is false
	
	t.EqualsTreePartial(t) is true
	t.EqualsTreePartial(u) is false
	t.EqualsTreePartial(v) is false
	t.EqualsTreePartial(w) is true
	t.EqualsTreePartial(null) is true
	
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
	x.EqualsTree(#(A B)) is true
	x.EqualsList(#(A B)) is false
	x.EqualsListPartial(#(A B)) is true
	a.EqualsTree(#(A B)) is false
	a.EqualsTreePartial(#(A B)) is true
	y.EqualsList(#[B]) is true
	y.EqualsListPartial(#[B]) is true
	
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
	EqualsTree is ok
	EqualsTreePartial is ok
	EqualsList is ok
	EqualsListPartial is ok
 */
	public static void Main(string[] args) 
	{
		bool r1,r2,r3,r4,r5,r6,r7,r8,r9,r10,r11,r12,r13,r14,r15,r16,r17;

		// define "astFactory" so translation of #(...) works
		SupportTest astSupport = new SupportTest( new ParserSharedInputState() );
		ASTFactory astFactory = astSupport.getASTFactory();

		AST t = #([ASSIGN,"="], [ID,"a"], [INT,"1"]); // build "a=1" tree
		Console.Out.WriteLine("t is " + t.ToStringList());
		AST u = #([ASSIGN,"="], [ID,"b"]); // build "b=?" tree
		Console.Out.WriteLine("u is " + u.ToStringList());
		AST v = #([ASSIGN,"="], [INT,"4"]); // build "4=?" tree
		Console.Out.WriteLine("v is " + v.ToStringList());
		AST w = #[ASSIGN,"="]; // build "=" tree
		Console.Out.WriteLine("w is " + w.ToStringList());
		Console.Out.WriteLine("");

		Console.Out.WriteLine("t.EqualsTree(t) is " + (r1=t.EqualsTree(t)));
		Console.Out.WriteLine("t.EqualsTree(u) is " + (r2=t.EqualsTree(u)));
		Console.Out.WriteLine("t.EqualsTree(v) is " + (r3=t.EqualsTree(v)));
		Console.Out.WriteLine("t.EqualsTree(w) is " + (r4=t.EqualsTree(w)));
		Console.Out.WriteLine("t.EqualsTree(null) is " + (r5=t.EqualsTree(null)));
		Console.Out.WriteLine("");

		Console.Out.WriteLine("t.EqualsTreePartial(t) is " + (r6=t.EqualsTreePartial(t)));
		Console.Out.WriteLine("t.EqualsTreePartial(u) is " + (r7=t.EqualsTreePartial(u)));
		Console.Out.WriteLine("t.EqualsTreePartial(v) is " + (r8=t.EqualsTreePartial(v)));
		Console.Out.WriteLine("t.EqualsTreePartial(w) is " + (r9=t.EqualsTreePartial(w)));
		Console.Out.WriteLine("t.EqualsTreePartial(null) is " + (r10=t.EqualsTreePartial(null)));
		Console.Out.WriteLine("");

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
		Console.Out.WriteLine("a is "+a.ToStringList()+"\n");
		Console.Out.WriteLine("              A---------------------J");
		Console.Out.WriteLine("              |");
		Console.Out.WriteLine("              B-----A-----F----A");
		Console.Out.WriteLine("              |     |     |    |");
		Console.Out.WriteLine("              C--A  B     A    A");
		Console.Out.WriteLine("                 |        |    |");
		Console.Out.WriteLine("                 B        B    B\n");

		AST x = a.getFirstChild().getNextSibling();
		Console.Out.WriteLine("x is second sibling of upperleftmost A: "+x.ToStringList());
		AST y = a.getFirstChild().getNextSibling().getFirstChild();
		Console.Out.WriteLine("y is child B of x: "+y.ToStringList());
		Console.Out.WriteLine("x.EqualsTree(#(A B)) is "+(r11=x.EqualsTree(#([A,"A"],[B,"B"]))));
		Console.Out.WriteLine("x.EqualsList(#(A B)) is "+(r12=x.EqualsList(#([A,"A"],[B,"B"]))));
		Console.Out.WriteLine("x.EqualsListPartial(#(A B)) is "+(r13=x.EqualsListPartial(#([A,"A"],[B,"B"]))));
		Console.Out.WriteLine("a.EqualsTree(#(A B)) is "+(r14=a.EqualsTree(#([A,"A"],[B,"B"]))));
		Console.Out.WriteLine("a.EqualsTreePartial(#(A B)) is "+(r15=a.EqualsTreePartial(#([A,"A"],[B,"B"]))));
		Console.Out.WriteLine("y.EqualsList(#[B]) is "+(r16=y.EqualsList(#[B,"B"])));
		Console.Out.WriteLine("y.EqualsListPartial(#[B]) is "+(r17=y.EqualsList(#[B,"B"])));

		IEnumerator ienum;
		Console.Out.WriteLine("\na.findAllPartial(#(A B)):");
		ienum = a.findAllPartial(#([A,"A"],[B,"B"]));
		while (ienum.MoveNext()) 
		{
			Console.Out.WriteLine(((AST)ienum.Current).ToStringList());
		}

		Console.Out.WriteLine("\na.findAllPartial(#[A])):");
		ienum = a.findAllPartial(#[A,"A"]);
		while (ienum.MoveNext()) 
		{
			Console.Out.WriteLine(((AST)ienum.Current).ToStringList());
		}

		Console.Out.WriteLine("\na.findAll(#(A B)):");
		ienum = a.findAll(#([A,"A"],[B,"B"]));
		while ( ienum.MoveNext() ) {
			Console.Out.WriteLine(((AST)ienum.Current).ToStringList());
		}

		// check results
		Console.Out.WriteLine("\nTest results:");
		if ( r1==true && r2==false && r3==false && r4==false &&
			 r5==false && r11==true && r14==false) {
			Console.Out.WriteLine("EqualsTree is ok");
		}
		else {
			Console.Out.WriteLine("EqualsTree is bad");
		}
		if ( r6==true && r7==false && r8==false && r9==true && r10==true ) {
			Console.Out.WriteLine("EqualsTreePartial is ok");
		}
		else {
			Console.Out.WriteLine("EqualsTreePartial is bad");
		}
		if ( r12==false && r16==true ) {
			Console.Out.WriteLine("EqualsList is ok");
		}
		else {
			Console.Out.WriteLine("Equalslist is bad");
		}
		if ( r13==true && r17==true ) {
			Console.Out.WriteLine("EqualsListPartial is ok");
		}
		else {
			Console.Out.WriteLine("EqualslistPartial is bad");
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
