// $ANTLR 2.7.5 (20050517): "PreProcessorExpressions.g" -> "PreProcessorExpressionParser.boo"$

namespace Boo.Lang.Useful.IO.Impl
// Generate the header common to all output files.
import System

import antlr.TokenBuffer as TokenBuffer
import antlr.TokenStreamException as TokenStreamException
import antlr.TokenStreamIOException as TokenStreamIOException
import antlr.ANTLRException as ANTLRException
import antlr.LLkParser as LLkParser
import antlr.Token as Token
import antlr.IToken as IToken
import antlr.TokenStream as TokenStream
import antlr.RecognitionException as RecognitionException
import antlr.NoViableAltException as NoViableAltException
import antlr.MismatchedTokenException as MismatchedTokenException
import antlr.SemanticException as SemanticException
import antlr.ParserSharedInputState as ParserSharedInputState
import antlr.collections.impl.BitSet as BitSet
import antlr.collections.AST as AST
import antlr.ASTPair as ASTPair
import antlr.ASTFactory as ASTFactory
import antlr.collections.impl.ASTArray as ASTArray

class PreProcessorExpressionParser(antlr.LLkParser):
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
	
	
	protected def initialize():
		tokenNames = tokenNames_
		initializeFactory()
	
	
	protected def constructor(tokenBuf as TokenBuffer, k as int):
		super(tokenBuf, k)
		initialize()
	
	def constructor(tokenBuf as TokenBuffer):
		self(tokenBuf, 1)
	
	protected def constructor(lexer as TokenStream, k as int):
		super(lexer, k)
		initialize()
	
	public def constructor(lexer as TokenStream):
		self(lexer, 1)
	
	public def constructor(state as ParserSharedInputState):
		super(state, 1)
		initialize()
	
	public def expr() as void: //throws RecognitionException, TokenStreamException
		
		returnAST = null
		currentAST as ASTPair = ASTPair.GetInstance()
		expr_AST as AST
		
		try:     // for error handling
			mexpr()
			astFactory.addASTChild(currentAST, returnAST)
			while true:
				if ((LA(1)==OR)):
					tmp4_AST as AST = null
					tmp4_AST = astFactory.create(LT(1))
					astFactory.makeASTRoot(currentAST, tmp4_AST)
					match(OR)
					mexpr()
					astFactory.addASTChild(currentAST, returnAST)
				else:
					goto _loop3_breakloop
			:_loop3_breakloop
			expr_AST = currentAST.root
		except ex as RecognitionException:
			reportError(ex)
			recover(ex,tokenSet_0_)
		returnAST = expr_AST
		ASTPair.PutInstance(currentAST)
	
	public def mexpr() as void: //throws RecognitionException, TokenStreamException
		
		returnAST = null
		currentAST as ASTPair = ASTPair.GetInstance()
		mexpr_AST as AST
		
		try:     // for error handling
			atom()
			astFactory.addASTChild(currentAST, returnAST)
			while true:
				if ((LA(1)==AND)):
					tmp5_AST as AST = null
					tmp5_AST = astFactory.create(LT(1))
					astFactory.makeASTRoot(currentAST, tmp5_AST)
					match(AND)
					atom()
					astFactory.addASTChild(currentAST, returnAST)
				else:
					goto _loop6_breakloop
			:_loop6_breakloop
			mexpr_AST = currentAST.root
		except ex as RecognitionException:
			reportError(ex)
			recover(ex,tokenSet_1_)
		returnAST = mexpr_AST
		ASTPair.PutInstance(currentAST)
	
	public def atom() as void: //throws RecognitionException, TokenStreamException
		
		returnAST = null
		currentAST as ASTPair = ASTPair.GetInstance()
		atom_AST as AST
		
		try:     // for error handling
			_givenValue  = LA(1)
			if ((_givenValue == ID)): // 1831
				tmp6_AST as AST = null
				tmp6_AST = astFactory.create(LT(1))
				astFactory.addASTChild(currentAST, tmp6_AST)
				match(ID)
				atom_AST = currentAST.root
			elif ((_givenValue == NOT)): // 1831
				tmp7_AST as AST = null
				tmp7_AST = astFactory.create(LT(1))
				astFactory.makeASTRoot(currentAST, tmp7_AST)
				match(NOT)
				tmp8_AST as AST = null
				tmp8_AST = astFactory.create(LT(1))
				astFactory.addASTChild(currentAST, tmp8_AST)
				match(ID)
				atom_AST = currentAST.root
			elif ((_givenValue == LPAREN)): // 1831
				paren_expr()
				astFactory.addASTChild(currentAST, returnAST)
				atom_AST = currentAST.root
			else: // line 1969
					raise NoViableAltException(LT(1), getFilename())
		except ex as RecognitionException:
			reportError(ex)
			recover(ex,tokenSet_2_)
		returnAST = atom_AST
		ASTPair.PutInstance(currentAST)
	
	public def paren_expr() as void: //throws RecognitionException, TokenStreamException
		
		returnAST = null
		currentAST as ASTPair = ASTPair.GetInstance()
		paren_expr_AST as AST
		
		try:     // for error handling
			match(LPAREN)
			expr()
			astFactory.addASTChild(currentAST, returnAST)
			match(RPAREN)
			paren_expr_AST = currentAST.root
		except ex as RecognitionException:
			reportError(ex)
			recover(ex,tokenSet_2_)
		returnAST = paren_expr_AST
		ASTPair.PutInstance(currentAST)
	
	private def initializeFactory():
		if (astFactory is null):
			astFactory = ASTFactory()
		initializeASTFactory(astFactory)
	static def initializeASTFactory(factory as ASTFactory):
		factory.setMaxNodeType(15)
	
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
	
	private static def mk_tokenSet_0_() as (long):
		data = (512L, 0L, )
		return data
	public static final tokenSet_0_ = BitSet(mk_tokenSet_0_())
	private static def mk_tokenSet_1_() as (long):
		data = (528L, 0L, )
		return data
	public static final tokenSet_1_ = BitSet(mk_tokenSet_1_())
	private static def mk_tokenSet_2_() as (long):
		data = (560L, 0L, )
		return data
	public static final tokenSet_2_ = BitSet(mk_tokenSet_2_())
	
