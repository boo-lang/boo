// $ANTLR 2.7.5 (20050517): "PreProcessorExpressions.g" -> "PreProcessorExpressionEvaluator.boo"$

namespace Boo.Lang.Useful.IO.Impl
// Generate header specific to the tree-parser Boo file
import System

import antlr.TreeParser as TreeParser
import antlr.Token as Token
import antlr.IToken as IToken
import antlr.collections.AST as AST
import antlr.RecognitionException as RecognitionException
import antlr.ANTLRException as ANTLRException
import antlr.NoViableAltException as NoViableAltException
import antlr.MismatchedTokenException as MismatchedTokenException
import antlr.SemanticException as SemanticException
import antlr.collections.impl.BitSet as BitSet
import antlr.ASTPair as ASTPair
import antlr.ASTFactory as ASTFactory
import antlr.collections.impl.ASTArray as ASTArray


class PreProcessorExpressionEvaluator(antlr.TreeParser):
	public static final EOF = 1
	public static final NULL_TREE_LOOKAHEAD = 3
	public static final OR = 4
	public static final AND = 5
	public static final ID = 6
	public static final NOT = 7
	public static final LPAREN = 8
	public static final RPAREN = 9
	public static final WS = 10
	public static final COMMENT = 11
	public static final ID_START = 12
	public static final ID_PART = 13
	public static final LETTER = 14
	public static final DIGIT = 15
	
	
	[property(SymbolTable)]
	_symbolTable as System.Collections.IDictionary
	def constructor():
		tokenNames = tokenNames_
	
	public def expr(_t as AST) as bool : //throws RecognitionException
		value as bool 
		
		expr_AST_in as AST = cast(AST, _t)
		id as AST = null
		a as bool
		b as bool
		value = false
		
		try:     // for error handling
			if _t is null:
				_t = ASTNULL
			_givenValue  = _t.Type
			if ((_givenValue == OR)): // 1831
				__t28 as AST  = _t
				tmp1_AST_in as AST = _t
				match(_t,OR)
				_t = _t.getFirstChild()
				a=expr(_t)
				_t = retTree_
				b=expr(_t)
				_t = retTree_
				_t = __t28
				_t = _t.getNextSibling()
				value = a or b; 
			elif ((_givenValue == AND)): // 1831
				__t29 as AST  = _t
				tmp2_AST_in as AST = _t
				match(_t,AND)
				_t = _t.getFirstChild()
				a=expr(_t)
				_t = retTree_
				b=expr(_t)
				_t = retTree_
				_t = __t29
				_t = _t.getNextSibling()
				value = a and b; 
			elif ((_givenValue == NOT)): // 1831
				__t30 as AST  = _t
				tmp3_AST_in as AST = _t
				match(_t,NOT)
				_t = _t.getFirstChild()
				a=expr(_t)
				_t = retTree_
				_t = __t30
				_t = _t.getNextSibling()
				value = not a; 
			elif ((_givenValue == ID)): // 1831
				id = _t
				match(_t,ID)
				_t = _t.getNextSibling()
				value = id.getText() in _symbolTable; 
			else: // line 1969
					raise NoViableAltException(_t)
		except ex as RecognitionException:
			reportError(ex)
			if _t is not null:
				_t = _t.getNextSibling()
		retTree_ = _t
		return value
	
	
	public static final tokenNames_ = (
		'<0>',
		'EOF',
		'<2>',
		'NULL_TREE_LOOKAHEAD',
		'OR',
		'AND',
		'ID',
		'NOT',
		'LPAREN',
		'RPAREN',
		'WS',
		'COMMENT',
		'ID_START',
		'ID_PART',
		'LETTER',
		'DIGIT',
	)
	

