// $ANTLR 2.7.5 (20050517): "PreProcessorExpressions.g" -> "PreProcessorExpressionLexer.boo"$

namespace Boo.Lang.Useful.IO.Impl
// Generate header specific to lexer Boo file
import System
import System.IO.Stream as Stream
import System.IO.TextReader as TextReader
import System.Collections.Hashtable as Hashtable
import System.Collections.Comparer as Comparer

import antlr.TokenStreamException as TokenStreamException
import antlr.TokenStreamIOException as TokenStreamIOException
import antlr.TokenStreamRecognitionException as TokenStreamRecognitionException
import antlr.CharStreamException as CharStreamException
import antlr.CharStreamIOException as CharStreamIOException
import antlr.ANTLRException as ANTLRException
import antlr.CharScanner as CharScanner
import antlr.InputBuffer as InputBuffer
import antlr.ByteBuffer as ByteBuffer
import antlr.CharBuffer as CharBuffer
import antlr.Token as Token
import antlr.IToken as IToken
import antlr.CommonToken as CommonToken
import antlr.SemanticException as SemanticException
import antlr.RecognitionException as RecognitionException
import antlr.NoViableAltForCharException as NoViableAltForCharException
import antlr.MismatchedCharException as MismatchedCharException
import antlr.TokenStream as TokenStream
import antlr.LexerSharedInputState as LexerSharedInputState
import antlr.collections.impl.BitSet as BitSet

class PreProcessorExpressionLexer(antlr.CharScanner, TokenStream):
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
	
	def constructor(ins as Stream):
		self(ByteBuffer(ins))
	
	def constructor(r as TextReader):
		self(CharBuffer(r))
	
	def constructor(ib as InputBuffer):
		self(LexerSharedInputState(ib))
	
	def constructor(state as LexerSharedInputState):
		super(state)
		initialize()
	
	private def initialize():
		caseSensitiveLiterals = true
		setCaseSensitive(true)
		literals = Hashtable(100, 0.4, null, Comparer.Default)
	
	override def nextToken() as IToken:
		theRetToken as IToken
		:tryAgain
		while true:
			_token as IToken = null
			_ttype = Token.INVALID_TYPE
			resetText()
			try:     // for char stream error handling
				try:    // for lexical error handling
					_givenValue  = cached_LA1
					if ((_givenValue == char('\t'))
						 or (_givenValue ==char('\n'))
						 or (_givenValue ==char('\r'))
						 or (_givenValue ==char(' '))
					): // 1827
						mWS(true)
						theRetToken = returnToken_
					elif ((_givenValue == char('&'))): // 1831
						mAND(true)
						theRetToken = returnToken_
					elif ((_givenValue == char('|'))): // 1831
						mOR(true)
						theRetToken = returnToken_
					elif ((_givenValue == char('!'))): // 1831
						mNOT(true)
						theRetToken = returnToken_
					elif ((_givenValue == char('('))): // 1831
						mLPAREN(true)
						theRetToken = returnToken_
					elif ((_givenValue == char(')'))): // 1831
						mRPAREN(true)
						theRetToken = returnToken_
					elif ((_givenValue == char('A'))
						 or (_givenValue ==char('B'))
						 or (_givenValue ==char('C'))
						 or (_givenValue ==char('D'))
						 or (_givenValue ==char('E'))
						 or (_givenValue ==char('F'))
						 or (_givenValue ==char('G'))
						 or (_givenValue ==char('H'))
						 or (_givenValue ==char('I'))
						 or (_givenValue ==char('J'))
						 or (_givenValue ==char('K'))
						 or (_givenValue ==char('L'))
						 or (_givenValue ==char('M'))
						 or (_givenValue ==char('N'))
						 or (_givenValue ==char('O'))
						 or (_givenValue ==char('P'))
						 or (_givenValue ==char('Q'))
						 or (_givenValue ==char('R'))
						 or (_givenValue ==char('S'))
						 or (_givenValue ==char('T'))
						 or (_givenValue ==char('U'))
						 or (_givenValue ==char('V'))
						 or (_givenValue ==char('W'))
						 or (_givenValue ==char('X'))
						 or (_givenValue ==char('Y'))
						 or (_givenValue ==char('Z'))
						 or (_givenValue ==char('_'))
						 or (_givenValue ==char('a'))
						 or (_givenValue ==char('b'))
						 or (_givenValue ==char('c'))
						 or (_givenValue ==char('d'))
						 or (_givenValue ==char('e'))
						 or (_givenValue ==char('f'))
						 or (_givenValue ==char('g'))
						 or (_givenValue ==char('h'))
						 or (_givenValue ==char('i'))
						 or (_givenValue ==char('j'))
						 or (_givenValue ==char('k'))
						 or (_givenValue ==char('l'))
						 or (_givenValue ==char('m'))
						 or (_givenValue ==char('n'))
						 or (_givenValue ==char('o'))
						 or (_givenValue ==char('p'))
						 or (_givenValue ==char('q'))
						 or (_givenValue ==char('r'))
						 or (_givenValue ==char('s'))
						 or (_givenValue ==char('t'))
						 or (_givenValue ==char('u'))
						 or (_givenValue ==char('v'))
						 or (_givenValue ==char('w'))
						 or (_givenValue ==char('x'))
						 or (_givenValue ==char('y'))
						 or (_givenValue ==char('z'))
					): // 1827
						mID(true)
						theRetToken = returnToken_
					elif ((_givenValue == char('/'))): // 1831
						mCOMMENT(true)
						theRetToken = returnToken_
					else: // line 1969
							if cached_LA1 == EOF_CHAR:
								uponEOF(); returnToken_ = makeToken(Token.EOF_TYPE)
							else:
								raise NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn())
					goto tryAgain if returnToken_ is null // found SKIP token
					_ttype = returnToken_.Type
					_ttype = testLiteralsTable(_ttype)
					returnToken_.Type = _ttype
					return returnToken_
				except e as RecognitionException:
						raise TokenStreamRecognitionException(e)
			except cse as CharStreamException:
				if cse isa CharStreamIOException:
					raise TokenStreamIOException(cast(CharStreamIOException, cse).io)
				else:
					raise TokenStreamException(cse.Message)
	
	public def mWS(_createToken as bool) as void: //throws RecognitionException, CharStreamException, TokenStreamException
		_ttype as int; _token as IToken; _begin = text.Length;
		_ttype = WS
		
		_givenValue  = cached_LA1
		if ((_givenValue == char(' '))): // 1831
			match(' ')
		elif ((_givenValue == char('\t'))): // 1831
			match('\t')
		elif ((_givenValue == char('\n'))): // 1831
			match('\n')
		elif ((_givenValue == char('\r'))): // 1831
			match('\r')
		else: // line 1969
				raise NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn())
		_ttype = Token.SKIP; 
		if (_createToken and (_token is null) and (_ttype != Token.SKIP)):
			_token = makeToken(_ttype)
			_token.setText(text.ToString(_begin, text.Length-_begin))
		returnToken_ = _token
	
	public def mAND(_createToken as bool) as void: //throws RecognitionException, CharStreamException, TokenStreamException
		_ttype as int; _token as IToken; _begin = text.Length;
		_ttype = AND
		
		match("&&")
		if (_createToken and (_token is null) and (_ttype != Token.SKIP)):
			_token = makeToken(_ttype)
			_token.setText(text.ToString(_begin, text.Length-_begin))
		returnToken_ = _token
	
	public def mOR(_createToken as bool) as void: //throws RecognitionException, CharStreamException, TokenStreamException
		_ttype as int; _token as IToken; _begin = text.Length;
		_ttype = OR
		
		match("||")
		if (_createToken and (_token is null) and (_ttype != Token.SKIP)):
			_token = makeToken(_ttype)
			_token.setText(text.ToString(_begin, text.Length-_begin))
		returnToken_ = _token
	
	public def mNOT(_createToken as bool) as void: //throws RecognitionException, CharStreamException, TokenStreamException
		_ttype as int; _token as IToken; _begin = text.Length;
		_ttype = NOT
		
		match('!')
		if (_createToken and (_token is null) and (_ttype != Token.SKIP)):
			_token = makeToken(_ttype)
			_token.setText(text.ToString(_begin, text.Length-_begin))
		returnToken_ = _token
	
	public def mLPAREN(_createToken as bool) as void: //throws RecognitionException, CharStreamException, TokenStreamException
		_ttype as int; _token as IToken; _begin = text.Length;
		_ttype = LPAREN
		
		match('(')
		if (_createToken and (_token is null) and (_ttype != Token.SKIP)):
			_token = makeToken(_ttype)
			_token.setText(text.ToString(_begin, text.Length-_begin))
		returnToken_ = _token
	
	public def mRPAREN(_createToken as bool) as void: //throws RecognitionException, CharStreamException, TokenStreamException
		_ttype as int; _token as IToken; _begin = text.Length;
		_ttype = RPAREN
		
		match(')')
		if (_createToken and (_token is null) and (_ttype != Token.SKIP)):
			_token = makeToken(_ttype)
			_token.setText(text.ToString(_begin, text.Length-_begin))
		returnToken_ = _token
	
	public def mID(_createToken as bool) as void: //throws RecognitionException, CharStreamException, TokenStreamException
		_ttype as int; _token as IToken; _begin = text.Length;
		_ttype = ID
		
		mID_START(false)
		while true:
			if ((tokenSet_0_.member(cast(int, cached_LA1)))):
				mID_PART(false)
			else:
				goto _loop18_breakloop
		:_loop18_breakloop
		if (_createToken and (_token is null) and (_ttype != Token.SKIP)):
			_token = makeToken(_ttype)
			_token.setText(text.ToString(_begin, text.Length-_begin))
		returnToken_ = _token
	
	protected def mID_START(_createToken as bool) as void: //throws RecognitionException, CharStreamException, TokenStreamException
		_ttype as int; _token as IToken; _begin = text.Length;
		_ttype = ID_START
		
		_givenValue  = cached_LA1
		if ((_givenValue == char('_'))): // 1831
			match('_')
		elif ((_givenValue == char('A'))
			 or (_givenValue ==char('B'))
			 or (_givenValue ==char('C'))
			 or (_givenValue ==char('D'))
			 or (_givenValue ==char('E'))
			 or (_givenValue ==char('F'))
			 or (_givenValue ==char('G'))
			 or (_givenValue ==char('H'))
			 or (_givenValue ==char('I'))
			 or (_givenValue ==char('J'))
			 or (_givenValue ==char('K'))
			 or (_givenValue ==char('L'))
			 or (_givenValue ==char('M'))
			 or (_givenValue ==char('N'))
			 or (_givenValue ==char('O'))
			 or (_givenValue ==char('P'))
			 or (_givenValue ==char('Q'))
			 or (_givenValue ==char('R'))
			 or (_givenValue ==char('S'))
			 or (_givenValue ==char('T'))
			 or (_givenValue ==char('U'))
			 or (_givenValue ==char('V'))
			 or (_givenValue ==char('W'))
			 or (_givenValue ==char('X'))
			 or (_givenValue ==char('Y'))
			 or (_givenValue ==char('Z'))
			 or (_givenValue ==char('a'))
			 or (_givenValue ==char('b'))
			 or (_givenValue ==char('c'))
			 or (_givenValue ==char('d'))
			 or (_givenValue ==char('e'))
			 or (_givenValue ==char('f'))
			 or (_givenValue ==char('g'))
			 or (_givenValue ==char('h'))
			 or (_givenValue ==char('i'))
			 or (_givenValue ==char('j'))
			 or (_givenValue ==char('k'))
			 or (_givenValue ==char('l'))
			 or (_givenValue ==char('m'))
			 or (_givenValue ==char('n'))
			 or (_givenValue ==char('o'))
			 or (_givenValue ==char('p'))
			 or (_givenValue ==char('q'))
			 or (_givenValue ==char('r'))
			 or (_givenValue ==char('s'))
			 or (_givenValue ==char('t'))
			 or (_givenValue ==char('u'))
			 or (_givenValue ==char('v'))
			 or (_givenValue ==char('w'))
			 or (_givenValue ==char('x'))
			 or (_givenValue ==char('y'))
			 or (_givenValue ==char('z'))
		): // 1827
			mLETTER(false)
		else: // line 1969
				raise NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn())
		if (_createToken and (_token is null) and (_ttype != Token.SKIP)):
			_token = makeToken(_ttype)
			_token.setText(text.ToString(_begin, text.Length-_begin))
		returnToken_ = _token
	
	protected def mID_PART(_createToken as bool) as void: //throws RecognitionException, CharStreamException, TokenStreamException
		_ttype as int; _token as IToken; _begin = text.Length;
		_ttype = ID_PART
		
		_givenValue  = cached_LA1
		if ((_givenValue == char('A'))
			 or (_givenValue ==char('B'))
			 or (_givenValue ==char('C'))
			 or (_givenValue ==char('D'))
			 or (_givenValue ==char('E'))
			 or (_givenValue ==char('F'))
			 or (_givenValue ==char('G'))
			 or (_givenValue ==char('H'))
			 or (_givenValue ==char('I'))
			 or (_givenValue ==char('J'))
			 or (_givenValue ==char('K'))
			 or (_givenValue ==char('L'))
			 or (_givenValue ==char('M'))
			 or (_givenValue ==char('N'))
			 or (_givenValue ==char('O'))
			 or (_givenValue ==char('P'))
			 or (_givenValue ==char('Q'))
			 or (_givenValue ==char('R'))
			 or (_givenValue ==char('S'))
			 or (_givenValue ==char('T'))
			 or (_givenValue ==char('U'))
			 or (_givenValue ==char('V'))
			 or (_givenValue ==char('W'))
			 or (_givenValue ==char('X'))
			 or (_givenValue ==char('Y'))
			 or (_givenValue ==char('Z'))
			 or (_givenValue ==char('_'))
			 or (_givenValue ==char('a'))
			 or (_givenValue ==char('b'))
			 or (_givenValue ==char('c'))
			 or (_givenValue ==char('d'))
			 or (_givenValue ==char('e'))
			 or (_givenValue ==char('f'))
			 or (_givenValue ==char('g'))
			 or (_givenValue ==char('h'))
			 or (_givenValue ==char('i'))
			 or (_givenValue ==char('j'))
			 or (_givenValue ==char('k'))
			 or (_givenValue ==char('l'))
			 or (_givenValue ==char('m'))
			 or (_givenValue ==char('n'))
			 or (_givenValue ==char('o'))
			 or (_givenValue ==char('p'))
			 or (_givenValue ==char('q'))
			 or (_givenValue ==char('r'))
			 or (_givenValue ==char('s'))
			 or (_givenValue ==char('t'))
			 or (_givenValue ==char('u'))
			 or (_givenValue ==char('v'))
			 or (_givenValue ==char('w'))
			 or (_givenValue ==char('x'))
			 or (_givenValue ==char('y'))
			 or (_givenValue ==char('z'))
		): // 1827
			mID_START(false)
		elif ((_givenValue == char('0'))
			 or (_givenValue ==char('1'))
			 or (_givenValue ==char('2'))
			 or (_givenValue ==char('3'))
			 or (_givenValue ==char('4'))
			 or (_givenValue ==char('5'))
			 or (_givenValue ==char('6'))
			 or (_givenValue ==char('7'))
			 or (_givenValue ==char('8'))
			 or (_givenValue ==char('9'))
		): // 1827
			mDIGIT(false)
		else: // line 1969
				raise NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn())
		if (_createToken and (_token is null) and (_ttype != Token.SKIP)):
			_token = makeToken(_ttype)
			_token.setText(text.ToString(_begin, text.Length-_begin))
		returnToken_ = _token
	
	public def mCOMMENT(_createToken as bool) as void: //throws RecognitionException, CharStreamException, TokenStreamException
		_ttype as int; _token as IToken; _begin = text.Length;
		_ttype = COMMENT
		
		match("//")
		while true:
			if ((tokenSet_1_.member(cast(int, cached_LA1)))):
				match(tokenSet_1_)
			else:
				goto _loop22_breakloop
		:_loop22_breakloop
		_ttype = Token.SKIP; 
		if (_createToken and (_token is null) and (_ttype != Token.SKIP)):
			_token = makeToken(_ttype)
			_token.setText(text.ToString(_begin, text.Length-_begin))
		returnToken_ = _token
	
	protected def mLETTER(_createToken as bool) as void: //throws RecognitionException, CharStreamException, TokenStreamException
		_ttype as int; _token as IToken; _begin = text.Length;
		_ttype = LETTER
		
		_givenValue  = cached_LA1
		if ((_givenValue == char('a'))
			 or (_givenValue ==char('b'))
			 or (_givenValue ==char('c'))
			 or (_givenValue ==char('d'))
			 or (_givenValue ==char('e'))
			 or (_givenValue ==char('f'))
			 or (_givenValue ==char('g'))
			 or (_givenValue ==char('h'))
			 or (_givenValue ==char('i'))
			 or (_givenValue ==char('j'))
			 or (_givenValue ==char('k'))
			 or (_givenValue ==char('l'))
			 or (_givenValue ==char('m'))
			 or (_givenValue ==char('n'))
			 or (_givenValue ==char('o'))
			 or (_givenValue ==char('p'))
			 or (_givenValue ==char('q'))
			 or (_givenValue ==char('r'))
			 or (_givenValue ==char('s'))
			 or (_givenValue ==char('t'))
			 or (_givenValue ==char('u'))
			 or (_givenValue ==char('v'))
			 or (_givenValue ==char('w'))
			 or (_givenValue ==char('x'))
			 or (_givenValue ==char('y'))
			 or (_givenValue ==char('z'))
		): // 1827
			matchRange(char('a'),char('z'))
		elif ((_givenValue == char('A'))
			 or (_givenValue ==char('B'))
			 or (_givenValue ==char('C'))
			 or (_givenValue ==char('D'))
			 or (_givenValue ==char('E'))
			 or (_givenValue ==char('F'))
			 or (_givenValue ==char('G'))
			 or (_givenValue ==char('H'))
			 or (_givenValue ==char('I'))
			 or (_givenValue ==char('J'))
			 or (_givenValue ==char('K'))
			 or (_givenValue ==char('L'))
			 or (_givenValue ==char('M'))
			 or (_givenValue ==char('N'))
			 or (_givenValue ==char('O'))
			 or (_givenValue ==char('P'))
			 or (_givenValue ==char('Q'))
			 or (_givenValue ==char('R'))
			 or (_givenValue ==char('S'))
			 or (_givenValue ==char('T'))
			 or (_givenValue ==char('U'))
			 or (_givenValue ==char('V'))
			 or (_givenValue ==char('W'))
			 or (_givenValue ==char('X'))
			 or (_givenValue ==char('Y'))
			 or (_givenValue ==char('Z'))
		): // 1827
			matchRange(char('A'),char('Z'))
		else: // line 1969
				raise NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn())
		if (_createToken and (_token is null) and (_ttype != Token.SKIP)):
			_token = makeToken(_ttype)
			_token.setText(text.ToString(_begin, text.Length-_begin))
		returnToken_ = _token
	
	protected def mDIGIT(_createToken as bool) as void: //throws RecognitionException, CharStreamException, TokenStreamException
		_ttype as int; _token as IToken; _begin = text.Length;
		_ttype = DIGIT
		
		matchRange(char('0'),char('9'))
		if (_createToken and (_token is null) and (_ttype != Token.SKIP)):
			_token = makeToken(_ttype)
			_token.setText(text.ToString(_begin, text.Length-_begin))
		returnToken_ = _token
	
	
	private static def mk_tokenSet_0_() as (long):
		data = (287948901175001088L, 576460745995190270L, 0L, 0L, )
		return data
	public static final tokenSet_0_ = BitSet(mk_tokenSet_0_())
	private static def mk_tokenSet_1_() as (long):
		data = (-9217L, -1L, 0L, 0L, )
		return data
	public static final tokenSet_1_ = BitSet(mk_tokenSet_1_())
	
