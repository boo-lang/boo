header
{
#include <iostream>
#include <antlr/CharScanner.hpp>
}

options
{
	language=Cpp;
}

{ // into cpp file...
	ANTLR_USING_NAMESPACE(std);
	ANTLR_USING_NAMESPACE(antlr);

	void SupportTest::main(void)
	{
		try
		{
			bool r1,r2,r3,r4,r5,r6,r7,r8,r9,r10,r11,r12,r13,r14,r15,r16,r17;

			// define "astFactory" so translation of #(...) works
			// do some dirty tricks to get an initialized astFactory.
			ASTFactory ast_factory;
			ParserSharedInputState is(new ParserInputState(0));
			SupportTest dummy(is);
			dummy.initializeASTFactory(ast_factory);
			dummy.setASTFactory(&ast_factory);
			// dirty trick to get to our member variable and get the right
			// codegen...
			ASTFactory *astFactory = dummy.getASTFactory();

			RefAST t = #([ASSIGN,"="], [ID,"a"], [INT,"1"]); // build "a=1" tree
			cout << "t is " << t->toStringList() << endl;
			RefAST u = #([ASSIGN,"="], [ID,"b"]); // build "b=?" tree
			cout << "u is " << u->toStringList() << endl;
			RefAST v = #([ASSIGN,"="], [INT,"4"]); // build "4=?" tree
			cout << "v is " << v->toStringList() << endl;
			RefAST w = #[ASSIGN,"="]; // build "=" tree
			cout << "w is " << w->toStringList() << endl;
			cout << endl;

			cout << "t.equalsTree(t) is " << (r1=t->equalsTree(t)) << endl;
			cout << "t.equalsTree(u) is " << (r2=t->equalsTree(u)) << endl;
			cout << "t.equalsTree(v) is " << (r3=t->equalsTree(v)) << endl;
			cout << "t.equalsTree(w) is " << (r4=t->equalsTree(w)) << endl;
			cout << "t.equalsTree(null) is " << (r5=t->equalsTree(nullAST)) << endl;
			cout << endl;

			cout << "t.equalsTreePartial(t) is " << (r6=t->equalsTreePartial(t)) << endl;
			cout << "t.equalsTreePartial(u) is " << (r7=t->equalsTreePartial(u)) << endl;
			cout << "t.equalsTreePartial(v) is " << (r8=t->equalsTreePartial(v)) << endl;
			cout << "t.equalsTreePartial(w) is " << (r9=t->equalsTreePartial(w)) << endl;
			cout << "t.equalsTreePartial(null) is " << (r10=t->equalsTreePartial(nullAST)) << endl;
			cout << endl;

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
			RefAST a = #(nullAST,
							 ([A,"A"],
							  ([B,"B"], [C,"C"], ([A,"A"],[B,"B"])),
							  ([A,"A"],[B,"B"]),
							  ([F,"F"], #([A,"A"], [B,"B"])),
							  ([A,"A"], #([A,"A"], [B,"B"]))),
							 [J,"J"]);
			cout << "a is "<<a->toStringList()<<"\n" << endl;
			cout << "              A---------------------J" << endl;
			cout << "              |" << endl;
			cout << "              B-----A-----F----A" << endl;
			cout << "              |     |     |    |" << endl;
			cout << "              C--A  B     A    A" << endl;
			cout << "                 |        |    |" << endl;
			cout << "                 B        B    B\n" << endl;

			RefAST x = a->getFirstChild()->getNextSibling();
			cout << "x is second sibling of upperleftmost A: "<<x->toStringList() << endl;
			RefAST y = a->getFirstChild()->getNextSibling()->getFirstChild();
			cout << "y is child B of x: "<<y->toStringList() << endl;
			cout << "x.equalsTree(#(A B)) is "<<(r11=x->equalsTree(#([A,"A"],[B,"B"]))) << endl;
			cout << "x.equalsList(#(A B)) is "<<(r12=x->equalsList(#([A,"A"],[B,"B"]))) << endl;
			cout << "x.equalsListPartial(#(A B)) is "<<(r13=x->equalsListPartial(#([A,"A"],[B,"B"]))) << endl;
			cout << "a.equalsTree(#(A B)) is "<<(r14=a->equalsTree(#([A,"A"],[B,"B"]))) << endl;
			cout << "a.equalsTreePartial(#(A B)) is "<<(r15=a->equalsTreePartial(#([A,"A"],[B,"B"]))) << endl;
			cout << "y.equalsList(#[B]) is "<<(r16=y->equalsList(#[B,"B"])) << endl;
			cout << "y.equalsListPartial(#[B]) is "<<(r17=y->equalsList(#[B,"B"])) << endl;

			vector<RefAST> _enum;
			cout << "\na.findAllPartial(#(A B)):" << endl;
			_enum = a->findAllPartial(#([A,"A"],[B,"B"]));
			{for (vector<RefAST>::const_iterator i=_enum.begin();i!=_enum.end();i++) {
				cout << (*i)->toStringList() << endl;
			}
			}

			cout << "\na.findAllPartial(#[A])):" << endl;
			_enum = a->findAllPartial(#[A,"A"]);
			{for (vector<RefAST>::const_iterator i=_enum.begin();i!=_enum.end();i++) {
				cout << (*i)->toStringList() << endl;
			}
			}

			cout << "\na.findAll(#(A B)):" << endl;
			_enum = a->findAll(#([A,"A"],[B,"B"]));
			{for (vector<RefAST>::const_iterator i=_enum.begin();i!=_enum.end();i++) {
				cout << (*i)->toStringList() << endl;
			}
			}

			// check results
			cout << "\nTest results:" << endl;
			if ( r1==true && r2==false && r3==false && r4==false &&
				  r5==false && r11==true && r14==false)
			{
				cout << "equalsTree is ok" << endl;
			}
			else
			{
				cout << "equalsTree is bad" << endl;
			}
			if ( r6==true && r7==false && r8==false && r9==true && r10==true )
			{
				cout << "equalsTreePartial is ok" << endl;
			}
			else
			{
				cout << "equalsTreePartial is bad" << endl;
			}
			if ( r12==false && r16==true )
			{
				cout << "equalsList is ok" << endl;
			}
			else
			{
				cout << "equalslist is bad" << endl;
			}
			if ( r13==true && r17==true )
			{
				cout << "equalsListPartial is ok" << endl;
			}
			else
			{
				cout << "equalslistPartial is bad" << endl;
			}
		}
		catch( ... ) {
			cout << "Exception caught";
		}
	}
}
class SupportTest extends Parser;
options
{
	buildAST = true;
	genHashLines = false;
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
public:
	static void main();
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
