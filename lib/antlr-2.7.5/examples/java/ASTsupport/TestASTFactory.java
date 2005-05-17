
import antlr.ASTFactory;
import antlr.CommonAST;
import antlr.Token;
import antlr.BaseAST;
import antlr.collections.AST;

/** Test the new heterogeneous token type to tree node mapping stuff.
 *  I'm not using jUnit because I don't want the dependency on that
 *  package for the moment.
 */
public class TestASTFactory {
	private ASTFactory factory=new ASTFactory();

	public static void main(String[] args) {
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

	public boolean testDefaultCreate() {
        AST t = factory.create();
		return checkNode(t, CommonAST.class, Token.INVALID_TYPE);
	}

	public boolean testSpecificHomoCreate() {
		factory.setASTNodeType("MyAST");
		AST t = factory.create();
		factory.setASTNodeType("antlr.CommonAST"); // put it back
		return checkNode(t, MyAST.class, Token.INVALID_TYPE);
	}

	public boolean testDynamicHeteroCreate() {
		factory.setTokenTypeASTNodeType(49,"ASTType49");
		AST t = factory.create(49);
		boolean a = checkNode(t, ASTType49.class, 49);
		AST u = factory.create(55);
		boolean b = checkNode(u, CommonAST.class, 55);
		AST v = factory.create(49,"","MyAST");
		boolean c = checkNode(v, MyAST.class, 49);
		factory.setTokenTypeASTNodeType(49,null); // undo setting
		return a&&b&&c;
	}

	public boolean testNodeDup() {
		AST t = factory.create();
		boolean a = t.equals(factory.dup(t));
		boolean b = !t.equals(null);
		AST u = factory.create(49,"","ASTType49");
		boolean c = checkNode(factory.dup(u),ASTType49.class, 49);
		boolean d = u.equals(factory.dup(u));
		return a&&b&&c&&d;
	}

	public boolean testHeteroTreeDup() {
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
		boolean a = dup_t.equalsList(t);
        // check types
        boolean b = equalsNodeTypesList(t,dup_t);

		return a&&b;
	}

	protected boolean checkNode(AST t, Class c, int tokenType) {
		if ( t==null ) {
			return false;
		}
		if ( t.getClass()!=c ) {
			return false;
		}
		if ( t.getType()!=tokenType ) {
			return false;
		}
		return true;
	}

	/** Is t an exact structural match of this tree with the same node
	 *  types?  'self' is considered the start of a sibling list.
     */
    protected boolean equalsNodeTypesList(AST self, AST t) {
		// System.out.println("self="+self+", t="+t);
		// System.out.println("self.class="+self.getClass()+", t.class="+t.getClass());
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
			// System.out.println("sibling="+sibling+", t="+t);
            // as a quick optimization, check root types first.
            if ( sibling.getClass()!=t.getClass() ) {
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

	public static void error(String test) {
		System.out.println("Test "+test+" FAILED");
	}

	public static void success(String test) {
		System.out.println("Test "+test+" succeeded");
	}
}
