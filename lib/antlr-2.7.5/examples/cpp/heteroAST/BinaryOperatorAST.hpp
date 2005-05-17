#ifndef INC_BinaryOperatorAST_hpp__
#define INC_BinaryOperatorAST_hpp__

#include "antlr/AST.hpp"
#include "CalcAST.hpp"

class BinaryOperatorAST : public CalcAST {
public:
	RefCalcAST left() const
	{
		return RefCalcAST(getFirstChild());
	}

	RefCalcAST right() const
	{
		RefCalcAST t = left();
		if ( !t ) return t;
		return RefCalcAST(t->getNextSibling());
	}
};

typedef ANTLR_USE_NAMESPACE(antlr)ASTRefCount<BinaryOperatorAST> RefBinaryOperatorAST;

#endif //INC_BinaryOperatorAST_hpp__
