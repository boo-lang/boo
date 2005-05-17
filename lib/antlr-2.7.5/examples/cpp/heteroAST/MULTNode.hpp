#ifndef INC_MULTNode_hpp__
#define INC_MULTNode_hpp__

#include "BinaryOperatorAST.hpp"

/** A simple node to represent MULT operation */
class MULTNode : public BinaryOperatorAST {
public:
	MULTNode()
	{
	}
	static ANTLR_USE_NAMESPACE(antlr)RefAST factory( void )
	{
		ANTLR_USE_NAMESPACE(antlr)RefAST ret(new MULTNode());
		return ret;
	}

	/** Compute value of subtree; this is heterogeneous part :) */
	int value() const
	{
		return left()->value() * right()->value();
	}

	ANTLR_USE_NAMESPACE(std)string toString() const
	{
		return " *";
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
	/// Clone this instance
	ANTLR_USE_NAMESPACE(antlr)RefAST clone( void ) const
	{
		MULTNode* ast = new MULTNode(*this);
		return ANTLR_USE_NAMESPACE(antlr)RefAST(ast);
	}

};

typedef ANTLR_USE_NAMESPACE(antlr)ASTRefCount<MULTNode> RefMULTNode;

#endif //INC_MULTNode_hpp__
