using System;
using ASTFactory = antlr.ASTFactory;
using CommonAST  = antlr.CommonAST;
using Token      = antlr.Token;
using BaseAST    = antlr.BaseAST;
using AST        = antlr.collections.AST;

// wh: bug(?) in DotGNU 0.6 - "using antlr" will workaround the problem.
#if __CSCC__
using antlr;
#endif

/// <summary>Test the new heterogeneous token type to tree node mapping stuff.</summary>
public class TestASTFactory {
	private ASTFactory factory;
	
	public TestASTFactory()
	{
		factory = new ASTFactory();
		factory.setMaxNodeType(56);
	}

	public static void Main(string[] args) {
		TestASTFactory testHarness = new TestASTFactory();
		if ( !testHarness.testDefaultCreate() ) error("testDefaultCreate");
		else success("testDefaultCreate");
		if ( !testHarness.testSpecificHomoCreate() ) error("testSpecificHomoCreate");
		else success("testSpecificHomoCreate");
		if ( !testHarness.testDynamicHeteroCreate() ) error("testDynamicHeteroCreate");
		else success("testDynamicHeteroCreate");
		if ( !testHarness.testNodeDup() ) error("testNodeDup");
		else success("testNodeDup");
		if ( !testHarness.testHeteroTreeDup() ) error("testHeteroTreeDup");
		else success("testHeteroTreeDup");
	}

	public bool testDefaultCreate() {
		factory = new ASTFactory();
        AST t = factory.create();
		return checkNode(t, typeof(CommonAST), Token.INVALID_TYPE);
	}

	public bool testSpecificHomoCreate() {
		factory = new ASTFactory();
		factory.setASTNodeType("MyAST");
		AST t = factory.create();
		factory.setASTNodeType("antlr.CommonAST"); // put it back
		return checkNode(t, typeof(MyAST), Token.INVALID_TYPE);
	}

	public bool testDynamicHeteroCreate() {
		factory = new ASTFactory();
		factory.setMaxNodeType(55);
		factory.setTokenTypeASTNodeType(49,"ASTType49");
		AST t = factory.create(49);
		bool a = checkNode(t, typeof(ASTType49), 49);
		AST u = factory.create(55);
		bool b = checkNode(u, typeof(CommonAST), 55);
		AST v = factory.create(49,"","MyAST");
		bool c = checkNode(v, typeof(MyAST), 49);
		return a&&b&&c;
	}

	public bool testNodeDup() {
		factory = new ASTFactory();
		factory.setMaxNodeType(49);
		AST t = factory.create();
		bool a = t.Equals(factory.dup(t));
		bool b = !t.Equals(null);
		AST u = factory.create(49,"","ASTType49");
		bool c = checkNode(factory.dup(u),typeof(ASTType49), 49);
		bool d = u.Equals(factory.dup(u));
		return a&&b&&c&&d;
	}

	public bool testHeteroTreeDup() {
		factory = new ASTFactory();
		factory.setMaxNodeType(49);
		// create a tree and try to dup:
		// ( [type 1] [type 2] ( [type 49] [type 3 #2] ) [type 3] )
		AST x = factory.create(1,"[type 1]","MyAST"); // will be root
		AST y = factory.create(2,"[type 2]","MyAST");
		AST z = factory.create(3,"[type 3]","MyAST");
		AST sub = factory.create(49,"[type 49]","ASTType49");
		sub.addChild(factory.create(3,"[type 3 #2]","MyAST"));
		AST t = factory.make(new AST[] {x,y,sub,z});
		AST dup_t = factory.dupList(t);
        // check structure
		bool a = dup_t.EqualsList(t);
        // check types
        bool b = equalsNodeTypesList(t,dup_t);

		return a&&b;
	}

	protected bool checkNode(AST t, Type typ, int tokenType) {
		if ( t==null ) {
			return false;
		}
		if ( t.GetType()!=typ ) {
			return false;
		}
		if ( t.Type!=tokenType ) {
			return false;
		}
		return true;
	}

	/** Is t an exact structural match of this tree with the same node
	 *  types?  'self' is considered the start of a sibling list.
     */
    protected bool equalsNodeTypesList(AST self, AST t) {
		// Console.Out.WriteLine("self="+self+", t="+t);
		// Console.Out.WriteLine("self.class="+self.getClass()+", t.class="+t.getClass());
        AST sibling;

        // the empty tree is not a match of any non-null tree.
        if (t == null) {
            return false;
        }

        // Otherwise, start walking sibling lists.  First mismatch, return false.
        for (sibling = self;
			 sibling != null && t != null;
			 sibling = sibling.getNextSibling(), t = t.getNextSibling())
		{
			// Console.Out.WriteLine("sibling="+sibling+", t="+t);
            // as a quick optimization, check root types first.
            if ( sibling.GetType()!=t.GetType() ) {
                return false;
            }
            // if roots match, do full list match test on children.
            if (sibling.getFirstChild() != null) {
                if (!equalsNodeTypesList(sibling.getFirstChild(),
										 t.getFirstChild()))
				{
                    return false;
                }
            }
            // sibling has no kids, make sure t doesn't either
            else if (t.getFirstChild() != null) {
                return false;
            }
        }
        if (sibling == null && t == null) {
            return true;
        }
        // one sibling list has more than the other
        return false;
    }

	public static void error(string test) {
		Console.Out.WriteLine("Test "+test+" FAILED");
	}

	public static void success(string test) {
		Console.Out.WriteLine("Test "+test+" succeeded");
	}
}
