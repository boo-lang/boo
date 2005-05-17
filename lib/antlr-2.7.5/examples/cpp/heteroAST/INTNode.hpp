#ifndef INC_INTNode_hpp__
#define INC_INTNode_hpp__

#include <cstdlib>
#include <sstream>
#include "antlr/Token.hpp"
#include "antlr/String.hpp"
#include "CalcAST.hpp"

/** A simple node to represent an INT */
class INTNode : public CalcAST {
protected:
	int v;

public:
	INTNode() : v(0)
	{
	}
	static ANTLR_USE_NAMESPACE(antlr)RefAST factory( void )
	{
		ANTLR_USE_NAMESPACE(antlr)RefAST ret(new INTNode());
		return ret;
	}

	/** Compute value of subtree; this is heterogeneous part :) */
	int value() const
	{
		return v;
	}

	ANTLR_USE_NAMESPACE(std)string toString() const
	{
		ANTLR_USE_NAMESPACE(std)ostringstream ss;
		ss << v;
		return ss.str();
	}

	void initialize(int t, const ANTLR_USE_NAMESPACE(std)string& txt)
	{
		CalcAST::initialize(t, txt);
		v = atoi(txt.c_str());
	}
	void initialize(ANTLR_USE_NAMESPACE(antlr)RefAST t)
	{
		CalcAST::initialize(t);
	}
	void initialize(ANTLR_USE_NAMESPACE(antlr)RefToken tok)
	{
		CalcAST::initialize(tok);
		v = atoi(tok->getText().c_str());
	}
#ifdef ANTLR_SUPPORT_XML
	virtual void initialize( ANTLR_USE_NAMESPACE(std)istream& in )
	{
		CalcAST::initialize(in);
	}
#endif
	ANTLR_USE_NAMESPACE(antlr)RefAST clone( void ) const
	{
		INTNode* ast = new INTNode(*this);
		return ANTLR_USE_NAMESPACE(antlr)RefAST(ast);
	}
};

typedef ANTLR_USE_NAMESPACE(antlr)ASTRefCount<INTNode> RefINTNode;

#endif //INC_INTNode_hpp__
