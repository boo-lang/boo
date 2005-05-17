#ifndef INC_CalcAST_hpp__
#define INC_CalcAST_hpp__

#include "antlr/CommonAST.hpp"

class CalcAST : public ANTLR_USE_NAMESPACE(antlr)CommonAST {
public:
	virtual int value() const=0;
	void initialize(int t, const ANTLR_USE_NAMESPACE(std)string& txt)
	{
		CommonAST::initialize(t, txt);
	}
	void initialize(ANTLR_USE_NAMESPACE(antlr)RefAST t)
	{
		CommonAST::initialize(t);
	}
	void initialize(ANTLR_USE_NAMESPACE(antlr)RefToken tok)
	{
		CommonAST::initialize(tok);
	}
#ifdef ANTLR_SUPPORT_XML
	virtual void initialize( ANTLR_USE_NAMESPACE(std)istream& in )
	{
		CommonAST::initialize(in);
	}
#endif	
};

typedef ANTLR_USE_NAMESPACE(antlr)ASTRefCount<CalcAST> RefCalcAST;

#endif //INC_CalcAST_hpp__
