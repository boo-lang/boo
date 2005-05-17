#ifndef INC_PLUSNode_hpp__
#define INC_PLUSNode_hpp__

#include "BinaryOperatorAST.hpp"

/** A simple node to represent PLUS operation */
class PLUSNode : public BinaryOperatorAST {
public:
	PLUSNode()
	{
	}
	static ANTLR_USE_NAMESPACE(antlr)RefAST factory( void )
	{
		ANTLR_USE_NAMESPACE(antlr)RefAST ret(new PLUSNode());
		return ret;
	}

	/** Compute value of subtree; this is heterogeneous part :) */
	int value() const
	{
		return left()->value() + right()->value();
	}

	ANTLR_USE_NAMESPACE(std)string toString() const
	{
		return " +";
	}
	void initialize(int t, const ANTLR_USE_NAMESPACE(std)string& txt)
	{
		CalcAST::initialize(t, txt);
	}
	void initialize(ANTLR_USE_NAMESPACE(antlr)RefAST t)
	{
		CalcAST::initialize(t);
	}
	void initialize(ANTLR_USE_NAMESPACE(antlr)RefToken tok)
	{
		CalcAST::initialize(tok);
	}
#ifdef ANTLR_SUPPORT_XML
	virtual void initialize( ANTLR_USE_NAMESPACE(std)istream& in )
	{
		CalcAST::initialize(in);
	}
#endif
	ANTLR_USE_NAMESPACE(antlr)RefAST clone( void ) const
	{
		PLUSNode* ast = new PLUSNode(*this);
		return ANTLR_USE_NAMESPACE(antlr)RefAST(ast);
	}
};

#if (_MSC_VER == 1300 )
extern template class ANTLR_USE_NAMESPACE(antlr)ASTRefCount<PLUSNode>;
#endif

typedef ANTLR_USE_NAMESPACE(antlr)ASTRefCount<PLUSNode> RefPLUSNode;

#endif //INC_PLUSNode_hpp__
