// $ANTLR 2.7.5 (20131102): "src/Boo.Lang.Parser/boo.g" -> "BooLexer.cs"$

namespace Boo.Lang.Parser
{
	// Generate header specific to lexer CSharp file
	using System;
	using Stream                          = System.IO.Stream;
	using TextReader                      = System.IO.TextReader;
	using Hashtable                       = System.Collections.Hashtable;
	using Comparer                        = System.Collections.Comparer;
	
	using TokenStreamException            = antlr.TokenStreamException;
	using TokenStreamIOException          = antlr.TokenStreamIOException;
	using TokenStreamRecognitionException = antlr.TokenStreamRecognitionException;
	using CharStreamException             = antlr.CharStreamException;
	using CharStreamIOException           = antlr.CharStreamIOException;
	using ANTLRException                  = antlr.ANTLRException;
	using CharScanner                     = antlr.CharScanner;
	using InputBuffer                     = antlr.InputBuffer;
	using ByteBuffer                      = antlr.ByteBuffer;
	using CharBuffer                      = antlr.CharBuffer;
	using Token                           = antlr.Token;
	using IToken                          = antlr.IToken;
	using CommonToken                     = antlr.CommonToken;
	using SemanticException               = antlr.SemanticException;
	using RecognitionException            = antlr.RecognitionException;
	using NoViableAltForCharException     = antlr.NoViableAltForCharException;
	using MismatchedCharException         = antlr.MismatchedCharException;
	using TokenStream                     = antlr.TokenStream;
	using LexerSharedInputState           = antlr.LexerSharedInputState;
	using BitSet                          = antlr.collections.impl.BitSet;
	
using Boo.Lang.Parser.Util;

	public 	class BooLexer : antlr.CharScanner	, TokenStream
	 {
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int INDENT = 4;
		public const int DEDENT = 5;
		public const int ELIST = 6;
		public const int DLIST = 7;
		public const int ESEPARATOR = 8;
		public const int EOL = 9;
		public const int ASSEMBLY_ATTRIBUTE_BEGIN = 10;
		public const int MODULE_ATTRIBUTE_BEGIN = 11;
		public const int ABSTRACT = 12;
		public const int AND = 13;
		public const int AS = 14;
		public const int BREAK = 15;
		public const int CONTINUE = 16;
		public const int CALLABLE = 17;
		public const int CAST = 18;
		public const int CHAR = 19;
		public const int CLASS = 20;
		public const int CONSTRUCTOR = 21;
		public const int DEF = 22;
		public const int DESTRUCTOR = 23;
		public const int DO = 24;
		public const int ELIF = 25;
		public const int ELSE = 26;
		public const int END = 27;
		public const int ENSURE = 28;
		public const int ENUM = 29;
		public const int EVENT = 30;
		public const int EXCEPT = 31;
		public const int FAILURE = 32;
		public const int FINAL = 33;
		public const int FROM = 34;
		public const int FOR = 35;
		public const int FALSE = 36;
		public const int GET = 37;
		public const int GOTO = 38;
		public const int IMPORT = 39;
		public const int INTERFACE = 40;
		public const int INTERNAL = 41;
		public const int IS = 42;
		public const int ISA = 43;
		public const int IF = 44;
		public const int IN = 45;
		public const int NAMESPACE = 46;
		public const int NEW = 47;
		public const int NOT = 48;
		public const int NULL = 49;
		public const int OF = 50;
		public const int OR = 51;
		public const int OVERRIDE = 52;
		public const int PASS = 53;
		public const int PARTIAL = 54;
		public const int PUBLIC = 55;
		public const int PROTECTED = 56;
		public const int PRIVATE = 57;
		public const int RAISE = 58;
		public const int REF = 59;
		public const int RETURN = 60;
		public const int SET = 61;
		public const int SELF = 62;
		public const int SUPER = 63;
		public const int STATIC = 64;
		public const int STRUCT = 65;
		public const int THEN = 66;
		public const int TRY = 67;
		public const int TRANSIENT = 68;
		public const int TRUE = 69;
		public const int TYPEOF = 70;
		public const int UNLESS = 71;
		public const int VIRTUAL = 72;
		public const int WHILE = 73;
		public const int YIELD = 74;
		public const int LET = 75;
		public const int WHERE = 76;
		public const int JOIN = 77;
		public const int ON = 78;
		public const int EQUALS = 79;
		public const int INTO = 80;
		public const int ORDERBY = 81;
		public const int ASCENDING = 82;
		public const int DESCENDING = 83;
		public const int SELECT = 84;
		public const int GROUP = 85;
		public const int BY = 86;
		public const int TRIPLE_QUOTED_STRING = 87;
		public const int EOS = 88;
		public const int LPAREN = 89;
		public const int RPAREN = 90;
		public const int DOUBLE_QUOTED_STRING = 91;
		public const int SINGLE_QUOTED_STRING = 92;
		public const int MULTIPLY = 93;
		public const int LBRACK = 94;
		public const int RBRACK = 95;
		public const int ASSIGN = 96;
		public const int COMMA = 97;
		public const int SPLICE_BEGIN = 98;
		public const int ID = 99;
		public const int DOT = 100;
		public const int COLON = 101;
		public const int NULLABLE_SUFFIX = 102;
		public const int EXPONENTIATION = 103;
		public const int BITWISE_OR = 104;
		public const int LBRACE = 105;
		public const int RBRACE = 106;
		public const int QQ_BEGIN = 107;
		public const int QQ_END = 108;
		public const int INPLACE_BITWISE_OR = 109;
		public const int INPLACE_EXCLUSIVE_OR = 110;
		public const int INPLACE_BITWISE_AND = 111;
		public const int INPLACE_SHIFT_LEFT = 112;
		public const int INPLACE_SHIFT_RIGHT = 113;
		public const int CMP_OPERATOR = 114;
		public const int GREATER_THAN = 115;
		public const int LESS_THAN = 116;
		public const int ADD = 117;
		public const int SUBTRACT = 118;
		public const int EXCLUSIVE_OR = 119;
		public const int DIVISION = 120;
		public const int MODULUS = 121;
		public const int BITWISE_AND = 122;
		public const int SHIFT_LEFT = 123;
		public const int SHIFT_RIGHT = 124;
		public const int LONG = 125;
		public const int INCREMENT = 126;
		public const int DECREMENT = 127;
		public const int ONES_COMPLEMENT = 128;
		public const int INT = 129;
		// "=" = 130
		public const int BACKTICK_QUOTED_STRING = 131;
		public const int RE_LITERAL = 132;
		public const int DOUBLE = 133;
		public const int FLOAT = 134;
		public const int TIMESPAN = 135;
		public const int ID_SUFFIX = 136;
		public const int LINE_CONTINUATION = 137;
		public const int INTERPOLATED_EXPRESSION = 138;
		public const int INTERPOLATED_REFERENCE = 139;
		public const int SL_COMMENT = 140;
		public const int ML_COMMENT = 141;
		public const int WS = 142;
		public const int X_RE_LITERAL = 143;
		public const int NEWLINE = 144;
		public const int DQS_ESC = 145;
		public const int SQS_ESC = 146;
		public const int SESC = 147;
		public const int RE_CHAR = 148;
		public const int X_RE_CHAR = 149;
		public const int RE_OPTIONS = 150;
		public const int RE_ESC = 151;
		public const int DIGIT_GROUP = 152;
		public const int REVERSE_DIGIT_GROUP = 153;
		public const int AT_SYMBOL = 154;
		public const int ID_LETTER = 155;
		public const int DIGIT = 156;
		public const int HEXDIGIT = 157;
		
		
	protected int _skipWhitespaceRegion = 0;
	
	TokenStreamRecorder _erecorder;
	
	antlr.TokenStreamSelector _selector;

	bool _preserveComments;

	internal void Initialize(antlr.TokenStreamSelector selector, int tabSize, antlr.TokenCreator tokenCreator)
	{
		setTabSize(tabSize);
		setTokenCreator(tokenCreator);
		
		_selector = selector;
		_erecorder = new TokenStreamRecorder(selector);
	}
	
	internal antlr.TokenStream CreateExpressionLexer()
	{
		BooExpressionLexer lexer = new BooExpressionLexer(getInputState());
		lexer.setTabSize(getTabSize());
		lexer.setTokenCreator(tokenCreator);

		// Apply the end-to-id token filter
		var selector = new antlr.TokenStreamSelector();		
		var filter = new EndTokenStreamFilter(lexer, END, ID);
		selector.select(filter);
		return selector;
	}

	internal static bool IsDigit(char ch)
	{
		return ch >= '0' && ch <= '9';
	}
	
	bool SkipWhitespace
	{
		get
		{
			return _skipWhitespaceRegion > 0;
		}
	}

	public bool PreserveComments
	{
		get { return _preserveComments; }
		set { _preserveComments = value; }
	}
	
	void ParseInterpolatedExpression(int tokenClose, int tokenOpen)
	{
		EnqueueESEPARATOR();
		if (0 == _erecorder.RecordUntil(CreateExpressionLexer(), tokenClose, tokenOpen))
			_erecorder.Dequeue();
		else
			EnqueueESEPARATOR();
		refresh();
	}
	
	void EnqueueESEPARATOR()
	{
		_erecorder.Enqueue(makeESEPARATOR());
	}

	void Enqueue(antlr.IToken token, string text)
	{
		token.setText(text);
		EnqueueInterpolatedToken(token);
	}
	
	void EnqueueInterpolatedToken(antlr.IToken token)
	{
		EnqueueESEPARATOR();
		_erecorder.Enqueue(token);
		EnqueueESEPARATOR();
	}
	
	antlr.IToken makeESEPARATOR()
	{
		return makeToken(ESEPARATOR);
	}

	internal void EnterSkipWhitespaceRegion()
	{
		++_skipWhitespaceRegion;
	}	

	internal void LeaveSkipWhitespaceRegion()
	{
		--_skipWhitespaceRegion;
	}
		public BooLexer(Stream ins) : this(new ByteBuffer(ins))
		{
		}
		
		public BooLexer(TextReader r) : this(new CharBuffer(r))
		{
		}
		
		public BooLexer(InputBuffer ib)		 : this(new LexerSharedInputState(ib))
		{
		}
		
		public BooLexer(LexerSharedInputState state) : base(state)
		{
			initialize();
		}
		private void initialize()
		{
			caseSensitiveLiterals = true;
			setCaseSensitive(true);
			literals = new Hashtable(100, (float) 0.4, null, Comparer.Default);
			literals.Add("public", 55);
			literals.Add("namespace", 46);
			literals.Add("break", 15);
			literals.Add("while", 73);
			literals.Add("new", 47);
			literals.Add("end", 27);
			literals.Add("orderby", 81);
			literals.Add("where", 76);
			literals.Add("then", 66);
			literals.Add("raise", 58);
			literals.Add("typeof", 70);
			literals.Add("select", 84);
			literals.Add("and", 13);
			literals.Add("failure", 32);
			literals.Add("not", 48);
			literals.Add("return", 60);
			literals.Add("pass", 53);
			literals.Add("from", 34);
			literals.Add("null", 49);
			literals.Add("def", 22);
			literals.Add("protected", 56);
			literals.Add("ref", 59);
			literals.Add("class", 20);
			literals.Add("do", 24);
			literals.Add("except", 31);
			literals.Add("event", 30);
			literals.Add("unless", 71);
			literals.Add("super", 63);
			literals.Add("set", 61);
			literals.Add("transient", 68);
			literals.Add("constructor", 21);
			literals.Add("interface", 40);
			literals.Add("join", 77);
			literals.Add("of", 50);
			literals.Add("is", 42);
			literals.Add("internal", 41);
			literals.Add("final", 33);
			literals.Add("yield", 74);
			literals.Add("or", 51);
			literals.Add("destructor", 23);
			literals.Add("if", 44);
			literals.Add("override", 52);
			literals.Add("as", 14);
			literals.Add("by", 86);
			literals.Add("try", 67);
			literals.Add("=", 130);
			literals.Add("goto", 38);
			literals.Add("enum", 29);
			literals.Add("isa", 43);
			literals.Add("for", 35);
			literals.Add("ascending", 82);
			literals.Add("char", 19);
			literals.Add("private", 57);
			literals.Add("descending", 83);
			literals.Add("equals", 79);
			literals.Add("false", 36);
			literals.Add("static", 64);
			literals.Add("abstract", 12);
			literals.Add("partial", 54);
			literals.Add("callable", 17);
			literals.Add("get", 37);
			literals.Add("continue", 16);
			literals.Add("on", 78);
			literals.Add("cast", 18);
			literals.Add("struct", 65);
			literals.Add("into", 80);
			literals.Add("else", 26);
			literals.Add("import", 39);
			literals.Add("elif", 25);
			literals.Add("in", 45);
			literals.Add("let", 75);
			literals.Add("self", 62);
			literals.Add("ensure", 28);
			literals.Add("true", 69);
			literals.Add("group", 85);
			literals.Add("virtual", 72);
		}
		
		override public IToken nextToken()			//throws TokenStreamException
		{
			IToken theRetToken = null;
tryAgain:
			for (;;)
			{
				IToken _token = null;
				int _ttype = Token.INVALID_TYPE;
				resetText();
				try     // for char stream error handling
				{
					try     // for lexical error handling
					{
						switch ( cached_LA1 )
						{
						case '\\':
						{
							mLINE_CONTINUATION(true);
							theRetToken = returnToken_;
							break;
						}
						case '#':
						{
							mSL_COMMENT(true);
							theRetToken = returnToken_;
							break;
						}
						case '0':  case '1':  case '2':  case '3':
						case '4':  case '5':  case '6':  case '7':
						case '8':  case '9':
						{
							mINT(true);
							theRetToken = returnToken_;
							break;
						}
						case '.':
						{
							mDOT(true);
							theRetToken = returnToken_;
							break;
						}
						case ':':
						{
							mCOLON(true);
							theRetToken = returnToken_;
							break;
						}
						case '&':
						{
							mBITWISE_AND(true);
							theRetToken = returnToken_;
							break;
						}
						case '^':
						{
							mEXCLUSIVE_OR(true);
							theRetToken = returnToken_;
							break;
						}
						case '(':
						{
							mLPAREN(true);
							theRetToken = returnToken_;
							break;
						}
						case ')':
						{
							mRPAREN(true);
							theRetToken = returnToken_;
							break;
						}
						case ']':
						{
							mRBRACK(true);
							theRetToken = returnToken_;
							break;
						}
						case '{':
						{
							mLBRACE(true);
							theRetToken = returnToken_;
							break;
						}
						case '}':
						{
							mRBRACE(true);
							theRetToken = returnToken_;
							break;
						}
						case '$':
						{
							mSPLICE_BEGIN(true);
							theRetToken = returnToken_;
							break;
						}
						case '%':
						{
							mMODULUS(true);
							theRetToken = returnToken_;
							break;
						}
						case '/':
						{
							mDIVISION(true);
							theRetToken = returnToken_;
							break;
						}
						case '~':
						{
							mONES_COMPLEMENT(true);
							theRetToken = returnToken_;
							break;
						}
						case '=':
						{
							mASSIGN(true);
							theRetToken = returnToken_;
							break;
						}
						case ',':
						{
							mCOMMA(true);
							theRetToken = returnToken_;
							break;
						}
						case '"':
						{
							mDOUBLE_QUOTED_STRING(true);
							theRetToken = returnToken_;
							break;
						}
						case '\'':
						{
							mSINGLE_QUOTED_STRING(true);
							theRetToken = returnToken_;
							break;
						}
						case '`':
						{
							mBACKTICK_QUOTED_STRING(true);
							theRetToken = returnToken_;
							break;
						}
						case '\t':  case '\n':  case '\u000c':  case '\r':
						case ' ':
						{
							mWS(true);
							theRetToken = returnToken_;
							break;
						}
						case ';':
						{
							mEOS(true);
							theRetToken = returnToken_;
							break;
						}
						case '?':
						{
							mNULLABLE_SUFFIX(true);
							theRetToken = returnToken_;
							break;
						}
						default:
							if ((cached_LA1=='<') && (cached_LA2=='<') && (LA(3)=='='))
							{
								mINPLACE_SHIFT_LEFT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='>') && (cached_LA2=='>') && (LA(3)=='=')) {
								mINPLACE_SHIFT_RIGHT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='[') && (cached_LA2=='|')) {
								mQQ_BEGIN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='|') && (cached_LA2==']')) {
								mQQ_END(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='+') && (cached_LA2=='+')) {
								mINCREMENT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='-') && (cached_LA2=='-')) {
								mDECREMENT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='*') && (cached_LA2=='*')) {
								mEXPONENTIATION(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='<') && (cached_LA2=='<') && (true)) {
								mSHIFT_LEFT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='>') && (cached_LA2=='>') && (true)) {
								mSHIFT_RIGHT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='!'||cached_LA1=='<'||cached_LA1=='>') && (cached_LA2=='='||cached_LA2=='~')) {
								mCMP_OPERATOR(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='@') && (cached_LA2=='/')) {
								mX_RE_LITERAL(true);
								theRetToken = returnToken_;
							}
							else if ((tokenSet_0_.member(cached_LA1)) && (true)) {
								mID(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='|') && (true)) {
								mBITWISE_OR(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='[') && (true)) {
								mLBRACK(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='+') && (true)) {
								mADD(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='-') && (true)) {
								mSUBTRACT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='*') && (true)) {
								mMULTIPLY(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='<') && (true)) {
								mLESS_THAN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='>') && (true)) {
								mGREATER_THAN(true);
								theRetToken = returnToken_;
							}
						else
						{
							if (cached_LA1==EOF_CHAR) { uponEOF(); returnToken_ = makeToken(Token.EOF_TYPE); }
				else {throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());}
						}
						break; }
						if ( null==returnToken_ ) goto tryAgain; // found SKIP token
						_ttype = returnToken_.Type;
						returnToken_.Type = _ttype;
						return returnToken_;
					}
					catch (RecognitionException e) {
							throw new TokenStreamRecognitionException(e);
					}
				}
				catch (CharStreamException cse) {
					if ( cse is CharStreamIOException ) {
						throw new TokenStreamIOException(((CharStreamIOException)cse).io);
					}
					else {
						throw new TokenStreamException(cse.Message);
					}
				}
			}
		}
		
	public void mID(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = ID;
		
		bool synPredMatched691 = false;
		if (((cached_LA1=='@') && (tokenSet_1_.member(cached_LA2)) && (true)))
		{
			int _m691 = mark();
			synPredMatched691 = true;
			inputState.guessing++;
			try {
				{
					mAT_SYMBOL(false);
					mID_LETTER(false);
				}
			}
			catch (RecognitionException)
			{
				synPredMatched691 = false;
			}
			rewind(_m691);
			inputState.guessing--;
		}
		if ( synPredMatched691 )
		{
			{
				mAT_SYMBOL(false);
				mID_SUFFIX(false);
			}
		}
		else if ((cached_LA1=='@') && (true) && (true)) {
			mAT_SYMBOL(false);
		}
		else if ((tokenSet_1_.member(cached_LA1))) {
			mID_SUFFIX(false);
		}
		else
		{
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
		}
		
		_ttype = testLiteralsTable(_ttype);
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mAT_SYMBOL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = AT_SYMBOL;
		
		match('@');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mID_LETTER(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = ID_LETTER;
		
		{
			switch ( cached_LA1 )
			{
			case '_':
			{
				match('_');
				break;
			}
			case 'a':  case 'b':  case 'c':  case 'd':
			case 'e':  case 'f':  case 'g':  case 'h':
			case 'i':  case 'j':  case 'k':  case 'l':
			case 'm':  case 'n':  case 'o':  case 'p':
			case 'q':  case 'r':  case 's':  case 't':
			case 'u':  case 'v':  case 'w':  case 'x':
			case 'y':  case 'z':
			{
				matchRange('a','z');
				break;
			}
			case 'A':  case 'B':  case 'C':  case 'D':
			case 'E':  case 'F':  case 'G':  case 'H':
			case 'I':  case 'J':  case 'K':  case 'L':
			case 'M':  case 'N':  case 'O':  case 'P':
			case 'Q':  case 'R':  case 'S':  case 'T':
			case 'U':  case 'V':  case 'W':  case 'X':
			case 'Y':  case 'Z':
			{
				matchRange('A','Z');
				break;
			}
			default:
				if ((((cached_LA1 >= '\u0080' && cached_LA1 <= '\ufffe')))&&(System.Char.IsLetter(LA(1))))
				{
					matchRange('\u0080','\uFFFE');
				}
			else
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			break; }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mID_SUFFIX(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = ID_SUFFIX;
		
		mID_LETTER(false);
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_1_.member(cached_LA1)) && (true) && (true))
				{
					mID_LETTER(false);
				}
				else if (((cached_LA1 >= '0' && cached_LA1 <= '9')) && (true) && (true)) {
					mDIGIT(false);
				}
				else
				{
					goto _loop695_breakloop;
				}
				
			}
_loop695_breakloop:			;
		}    // ( ... )*
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mDIGIT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DIGIT;
		
		matchRange('0','9');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLINE_CONTINUATION(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LINE_CONTINUATION;
		
		match('\\');
		{ // ( ... )+
			int _cnt700=0;
			for (;;)
			{
				switch ( cached_LA1 )
				{
				case '\n':  case '\r':
				{
					mNEWLINE(false);
					break;
				}
				case '\t':  case ' ':
				{
					{ // ( ... )+
						int _cnt699=0;
						for (;;)
						{
							if ((cached_LA1==' ') && (true) && (true))
							{
								match(' ');
							}
							else if ((cached_LA1=='\t') && (true) && (true)) {
								match('\t');
							}
							else
							{
								if (_cnt699 >= 1) { goto _loop699_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
							}
							
							_cnt699++;
						}
_loop699_breakloop:						;
					}    // ( ... )+
					break;
				}
				case '#':
				{
					mSL_COMMENT(false);
					break;
				}
				case '/':
				{
					mML_COMMENT(false);
					break;
				}
				default:
				{
					if (_cnt700 >= 1) { goto _loop700_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				break; }
				_cnt700++;
			}
_loop700_breakloop:			;
		}    // ( ... )+
		if (0==inputState.guessing)
		{
			_ttype = Token.SKIP;
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mNEWLINE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = NEWLINE;
		
		{
			switch ( cached_LA1 )
			{
			case '\n':
			{
				match('\n');
				break;
			}
			case '\r':
			{
				{
					match('\r');
					{
						if ((cached_LA1=='\n') && (true) && (true))
						{
							match('\n');
						}
						else {
						}
						
					}
				}
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (0==inputState.guessing)
		{
			newline();
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSL_COMMENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SL_COMMENT;
		
		match("#");
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_2_.member(cached_LA1)) && (true) && (true))
				{
					{
						match(tokenSet_2_);
					}
				}
				else
				{
					goto _loop824_breakloop;
				}
				
			}
_loop824_breakloop:			;
		}    // ( ... )*
		if (0==inputState.guessing)
		{
			
					if (!_preserveComments)
						_ttype = Token.SKIP;
				
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mML_COMMENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = ML_COMMENT;
		
		match("/*");
		{    // ( ... )*
			for (;;)
			{
				if (((cached_LA1=='*') && ((cached_LA2 >= '\u0003' && cached_LA2 <= '\ufffe')) && ((LA(3) >= '\u0003' && LA(3) <= '\ufffe')))&&( LA(2) != '/' ))
				{
					match('*');
				}
				else {
					bool synPredMatched828 = false;
					if (((cached_LA1=='/') && (cached_LA2=='*') && ((LA(3) >= '\u0003' && LA(3) <= '\ufffe'))))
					{
						int _m828 = mark();
						synPredMatched828 = true;
						inputState.guessing++;
						try {
							{
								match("/*");
							}
						}
						catch (RecognitionException)
						{
							synPredMatched828 = false;
						}
						rewind(_m828);
						inputState.guessing--;
					}
					if ( synPredMatched828 )
					{
						mML_COMMENT(false);
					}
					else if ((tokenSet_3_.member(cached_LA1)) && ((cached_LA2 >= '\u0003' && cached_LA2 <= '\ufffe')) && ((LA(3) >= '\u0003' && LA(3) <= '\ufffe'))) {
						{
							match(tokenSet_3_);
						}
					}
					else if ((cached_LA1=='\n'||cached_LA1=='\r')) {
						mNEWLINE(false);
					}
					else
					{
						goto _loop830_breakloop;
					}
					}
				}
_loop830_breakloop:				;
			}    // ( ... )*
			match("*/");
			if (0==inputState.guessing)
			{
				
						if (!_preserveComments)
							_ttype = Token.SKIP;
					
			}
			if (_createToken && (null == _token) && (_ttype != Token.SKIP))
			{
				_token = makeToken(_ttype);
				_token.setText(text.ToString(_begin, text.Length-_begin));
			}
			returnToken_ = _token;
		}
		
	public void mINT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = INT;
		
		if ((cached_LA1=='0') && (cached_LA2=='x'))
		{
			{
				match("0x");
				{ // ( ... )+
					int _cnt704=0;
					for (;;)
					{
						if ((tokenSet_4_.member(cached_LA1)))
						{
							mHEXDIGIT(false);
						}
						else
						{
							if (_cnt704 >= 1) { goto _loop704_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
						}
						
						_cnt704++;
					}
_loop704_breakloop:					;
				}    // ( ... )+
			}
			{
				if ((cached_LA1=='L'||cached_LA1=='l'))
				{
					{
						switch ( cached_LA1 )
						{
						case 'l':
						{
							match('l');
							break;
						}
						case 'L':
						{
							match('L');
							break;
						}
						default:
						{
							throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
						}
						 }
					}
					if (0==inputState.guessing)
					{
						_ttype = LONG;
					}
				}
				else {
				}
				
			}
		}
		else if (((cached_LA1 >= '0' && cached_LA1 <= '9')) && (true)) {
			mDIGIT_GROUP(false);
			{
				if ((cached_LA1=='E'||cached_LA1=='e'))
				{
					{
						switch ( cached_LA1 )
						{
						case 'e':
						{
							match('e');
							break;
						}
						case 'E':
						{
							match('E');
							break;
						}
						default:
						{
							throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
						}
						 }
					}
					{
						switch ( cached_LA1 )
						{
						case '+':
						{
							match('+');
							break;
						}
						case '-':
						{
							match('-');
							break;
						}
						case '0':  case '1':  case '2':  case '3':
						case '4':  case '5':  case '6':  case '7':
						case '8':  case '9':
						{
							break;
						}
						default:
						{
							throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
						}
						 }
					}
					mDIGIT_GROUP(false);
				}
				else {
				}
				
			}
			{
				switch ( cached_LA1 )
				{
				case 'L':  case 'l':
				{
					{
						switch ( cached_LA1 )
						{
						case 'l':
						{
							match('l');
							break;
						}
						case 'L':
						{
							match('L');
							break;
						}
						default:
						{
							throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
						}
						 }
					}
					if (0==inputState.guessing)
					{
						_ttype = LONG;
					}
					break;
				}
				case 'F':  case 'f':
				{
					{
						{
							switch ( cached_LA1 )
							{
							case 'f':
							{
								match('f');
								break;
							}
							case 'F':
							{
								match('F');
								break;
							}
							default:
							{
								throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
							}
							 }
						}
						if (0==inputState.guessing)
						{
							_ttype = FLOAT;
						}
					}
					break;
				}
				default:
					{
						{
							{
								if (((cached_LA1=='.'))&&(BooLexer.IsDigit(LA(2))))
								{
									{
										match('.');
										mREVERSE_DIGIT_GROUP(false);
										{
											if ((cached_LA1=='E'||cached_LA1=='e'))
											{
												{
													switch ( cached_LA1 )
													{
													case 'e':
													{
														match('e');
														break;
													}
													case 'E':
													{
														match('E');
														break;
													}
													default:
													{
														throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
													}
													 }
												}
												{
													switch ( cached_LA1 )
													{
													case '+':
													{
														match('+');
														break;
													}
													case '-':
													{
														match('-');
														break;
													}
													case '0':  case '1':  case '2':  case '3':
													case '4':  case '5':  case '6':  case '7':
													case '8':  case '9':
													{
														break;
													}
													default:
													{
														throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
													}
													 }
												}
												mDIGIT_GROUP(false);
											}
											else {
											}
											
										}
									}
									{
										if ((cached_LA1=='F'||cached_LA1=='f'))
										{
											{
												{
													switch ( cached_LA1 )
													{
													case 'f':
													{
														match('f');
														break;
													}
													case 'F':
													{
														match('F');
														break;
													}
													default:
													{
														throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
													}
													 }
												}
												if (0==inputState.guessing)
												{
													_ttype = FLOAT;
												}
											}
										}
										else {
											if (0==inputState.guessing)
											{
												_ttype = DOUBLE;
											}
										}
										
									}
								}
								else {
								}
								
							}
							{
								if ((cached_LA1=='d'||cached_LA1=='h'||cached_LA1=='m'||cached_LA1=='s'))
								{
									{
										switch ( cached_LA1 )
										{
										case 's':
										{
											match('s');
											break;
										}
										case 'h':
										{
											match('h');
											break;
										}
										case 'd':
										{
											match('d');
											break;
										}
										default:
											if ((cached_LA1=='m') && (cached_LA2=='s'))
											{
												match("ms");
											}
											else if ((cached_LA1=='m') && (true)) {
												match('m');
											}
										else
										{
											throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
										}
										break; }
									}
									if (0==inputState.guessing)
									{
										_ttype = TIMESPAN;
									}
								}
								else {
								}
								
							}
						}
					}
				break; }
			}
		}
		else
		{
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
		}
		
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mHEXDIGIT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = HEXDIGIT;
		
		{
			switch ( cached_LA1 )
			{
			case 'a':  case 'b':  case 'c':  case 'd':
			case 'e':  case 'f':
			{
				matchRange('a','f');
				break;
			}
			case 'A':  case 'B':  case 'C':  case 'D':
			case 'E':  case 'F':
			{
				matchRange('A','F');
				break;
			}
			case '0':  case '1':  case '2':  case '3':
			case '4':  case '5':  case '6':  case '7':
			case '8':  case '9':
			{
				matchRange('0','9');
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mDIGIT_GROUP(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DIGIT_GROUP;
		
		mDIGIT(false);
		{    // ( ... )*
			for (;;)
			{
				switch ( cached_LA1 )
				{
				case '_':
				{
					{
						int _saveIndex = 0;
						_saveIndex = text.Length;
						match('_');
						text.Length = _saveIndex;
						mDIGIT(false);
						mDIGIT(false);
						mDIGIT(false);
					}
					break;
				}
				case '0':  case '1':  case '2':  case '3':
				case '4':  case '5':  case '6':  case '7':
				case '8':  case '9':
				{
					mDIGIT(false);
					break;
				}
				default:
				{
					goto _loop876_breakloop;
				}
				 }
			}
_loop876_breakloop:			;
		}    // ( ... )*
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mREVERSE_DIGIT_GROUP(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = REVERSE_DIGIT_GROUP;
		
		{ // ( ... )+
			int _cnt880=0;
			for (;;)
			{
				if (((cached_LA1 >= '0' && cached_LA1 <= '9')) && ((cached_LA2 >= '0' && cached_LA2 <= '9')) && ((LA(3) >= '0' && LA(3) <= '9')))
				{
					mDIGIT(false);
					mDIGIT(false);
					mDIGIT(false);
					{
						if (((cached_LA1=='_'))&&(BooLexer.IsDigit(LA(2))))
						{
							int _saveIndex = 0;
							_saveIndex = text.Length;
							match('_');
							text.Length = _saveIndex;
						}
						else {
						}
						
					}
				}
				else if (((cached_LA1 >= '0' && cached_LA1 <= '9')) && (true) && (true)) {
					mDIGIT(false);
				}
				else
				{
					if (_cnt880 >= 1) { goto _loop880_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				
				_cnt880++;
			}
_loop880_breakloop:			;
		}    // ( ... )+
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mDOT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DOT;
		
		match('.');
		{
			if (((cached_LA1 >= '0' && cached_LA1 <= '9')))
			{
				mREVERSE_DIGIT_GROUP(false);
				{
					if ((cached_LA1=='E'||cached_LA1=='e'))
					{
						{
							switch ( cached_LA1 )
							{
							case 'e':
							{
								match('e');
								break;
							}
							case 'E':
							{
								match('E');
								break;
							}
							default:
							{
								throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
							}
							 }
						}
						{
							switch ( cached_LA1 )
							{
							case '+':
							{
								match('+');
								break;
							}
							case '-':
							{
								match('-');
								break;
							}
							case '0':  case '1':  case '2':  case '3':
							case '4':  case '5':  case '6':  case '7':
							case '8':  case '9':
							{
								break;
							}
							default:
							{
								throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
							}
							 }
						}
						mDIGIT_GROUP(false);
					}
					else {
					}
					
				}
				{
					switch ( cached_LA1 )
					{
					case 'F':  case 'f':
					{
						{
							{
								switch ( cached_LA1 )
								{
								case 'f':
								{
									match('f');
									break;
								}
								case 'F':
								{
									match('F');
									break;
								}
								default:
								{
									throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
								}
								 }
							}
							if (0==inputState.guessing)
							{
								_ttype = FLOAT;
							}
						}
						break;
					}
					case 'd':  case 'h':  case 'm':  case 's':
					{
						{
							{
								switch ( cached_LA1 )
								{
								case 's':
								{
									match('s');
									break;
								}
								case 'h':
								{
									match('h');
									break;
								}
								case 'd':
								{
									match('d');
									break;
								}
								default:
									if ((cached_LA1=='m') && (cached_LA2=='s'))
									{
										match("ms");
									}
									else if ((cached_LA1=='m') && (true)) {
										match('m');
									}
								else
								{
									throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
								}
								break; }
							}
							if (0==inputState.guessing)
							{
								_ttype = TIMESPAN;
							}
						}
						break;
					}
					default:
						{
							if (0==inputState.guessing)
							{
								_ttype = DOUBLE;
							}
						}
					break; }
				}
			}
			else {
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mCOLON(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = COLON;
		
		match(':');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mBITWISE_OR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = BITWISE_OR;
		
		match('|');
		{
			if ((cached_LA1=='='))
			{
				match('=');
				if (0==inputState.guessing)
				{
					_ttype = INPLACE_BITWISE_OR;
				}
			}
			else {
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mBITWISE_AND(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = BITWISE_AND;
		
		match('&');
		{
			if ((cached_LA1=='='))
			{
				match('=');
				if (0==inputState.guessing)
				{
					_ttype = INPLACE_BITWISE_AND;
				}
			}
			else {
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mEXCLUSIVE_OR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = EXCLUSIVE_OR;
		
		match('^');
		{
			if ((cached_LA1=='='))
			{
				match('=');
				if (0==inputState.guessing)
				{
					_ttype = INPLACE_EXCLUSIVE_OR;
				}
			}
			else {
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLPAREN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LPAREN;
		
		match('(');
		if (0==inputState.guessing)
		{
			EnterSkipWhitespaceRegion();
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mRPAREN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RPAREN;
		
		match(')');
		if (0==inputState.guessing)
		{
			LeaveSkipWhitespaceRegion();
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLBRACK(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LBRACK;
		
		match('[');
		if (0==inputState.guessing)
		{
			EnterSkipWhitespaceRegion();
		}
		{
			bool synPredMatched747 = false;
			if (((cached_LA1=='a'||cached_LA1=='m')))
			{
				int _m747 = mark();
				synPredMatched747 = true;
				inputState.guessing++;
				try {
					{
						switch ( cached_LA1 )
						{
						case 'm':
						{
							match("module:");
							break;
						}
						case 'a':
						{
							match("assembly:");
							break;
						}
						default:
						{
							throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
						}
						 }
					}
				}
				catch (RecognitionException)
				{
					synPredMatched747 = false;
				}
				rewind(_m747);
				inputState.guessing--;
			}
			if ( synPredMatched747 )
			{
				{
					switch ( cached_LA1 )
					{
					case 'm':
					{
						match("module:");
						if (0==inputState.guessing)
						{
							_ttype = MODULE_ATTRIBUTE_BEGIN;
						}
						break;
					}
					case 'a':
					{
						match("assembly:");
						if (0==inputState.guessing)
						{
							_ttype = ASSEMBLY_ATTRIBUTE_BEGIN;
						}
						break;
					}
					default:
					{
						throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
					}
					 }
				}
			}
			else {
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mRBRACK(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RBRACK;
		
		match(']');
		if (0==inputState.guessing)
		{
			LeaveSkipWhitespaceRegion();
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLBRACE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LBRACE;
		
		match('{');
		if (0==inputState.guessing)
		{
			EnterSkipWhitespaceRegion();
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mRBRACE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RBRACE;
		
		match('}');
		if (0==inputState.guessing)
		{
			LeaveSkipWhitespaceRegion();
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSPLICE_BEGIN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SPLICE_BEGIN;
		
		match("$");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mQQ_BEGIN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = QQ_BEGIN;
		
		match("[|");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mQQ_END(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = QQ_END;
		
		match("|]");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mINCREMENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = INCREMENT;
		
		match("++");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mDECREMENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DECREMENT;
		
		match("--");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mADD(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = ADD;
		
		{
			match('+');
		}
		{
			if ((cached_LA1=='='))
			{
				match('=');
				if (0==inputState.guessing)
				{
					_ttype = ASSIGN;
				}
			}
			else {
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSUBTRACT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SUBTRACT;
		
		{
			match('-');
		}
		{
			if ((cached_LA1=='='))
			{
				match('=');
				if (0==inputState.guessing)
				{
					_ttype = ASSIGN;
				}
			}
			else {
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mMODULUS(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = MODULUS;
		
		match('%');
		{
			if ((cached_LA1=='='))
			{
				match('=');
				if (0==inputState.guessing)
				{
					_ttype = ASSIGN;
				}
			}
			else {
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mMULTIPLY(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = MULTIPLY;
		
		match('*');
		{
			if ((cached_LA1=='='))
			{
				match('=');
				if (0==inputState.guessing)
				{
					_ttype = ASSIGN;
				}
			}
			else {
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mEXPONENTIATION(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = EXPONENTIATION;
		
		match("**");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mDIVISION(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DIVISION;
		
		bool synPredMatched770 = false;
		if (((cached_LA1=='/') && (cached_LA2=='*') && ((LA(3) >= '\u0003' && LA(3) <= '\ufffe'))))
		{
			int _m770 = mark();
			synPredMatched770 = true;
			inputState.guessing++;
			try {
				{
					match("/*");
				}
			}
			catch (RecognitionException)
			{
				synPredMatched770 = false;
			}
			rewind(_m770);
			inputState.guessing--;
		}
		if ( synPredMatched770 )
		{
			mML_COMMENT(false);
			if (0==inputState.guessing)
			{
				
						if (!_preserveComments)
							_ttype = Token.SKIP;
					
			}
		}
		else {
			bool synPredMatched772 = false;
			if (((cached_LA1=='/') && (tokenSet_5_.member(cached_LA2)) && (tokenSet_6_.member(LA(3)))))
			{
				int _m772 = mark();
				synPredMatched772 = true;
				inputState.guessing++;
				try {
					{
						mRE_LITERAL(false);
					}
				}
				catch (RecognitionException)
				{
					synPredMatched772 = false;
				}
				rewind(_m772);
				inputState.guessing--;
			}
			if ( synPredMatched772 )
			{
				mRE_LITERAL(false);
				if (0==inputState.guessing)
				{
					_ttype = RE_LITERAL;
				}
			}
			else if ((cached_LA1=='/') && (true) && (true)) {
				match('/');
				{
					switch ( cached_LA1 )
					{
					case '/':
					{
						{
							match('/');
							{    // ( ... )*
								for (;;)
								{
									if ((tokenSet_2_.member(cached_LA1)))
									{
										{
											match(tokenSet_2_);
										}
									}
									else
									{
										goto _loop777_breakloop;
									}
									
								}
_loop777_breakloop:								;
							}    // ( ... )*
							if (0==inputState.guessing)
							{
								
												if (_preserveComments)
													_ttype = SL_COMMENT;
												else
													_ttype = Token.SKIP;
											
							}
						}
						break;
					}
					case '=':
					{
						{
							match('=');
							if (0==inputState.guessing)
							{
								_ttype = ASSIGN;
							}
						}
						break;
					}
					default:
						{
						}
					break; }
				}
			}
			else
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			}
			if (_createToken && (null == _token) && (_ttype != Token.SKIP))
			{
				_token = makeToken(_ttype);
				_token.setText(text.ToString(_begin, text.Length-_begin));
			}
			returnToken_ = _token;
		}
		
	protected void mRE_LITERAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RE_LITERAL;
		
		match('/');
		{ // ( ... )+
			int _cnt858=0;
			for (;;)
			{
				if ((tokenSet_5_.member(cached_LA1)))
				{
					mRE_CHAR(false);
				}
				else
				{
					if (_cnt858 >= 1) { goto _loop858_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				
				_cnt858++;
			}
_loop858_breakloop:			;
		}    // ( ... )+
		match('/');
		{
			if ((tokenSet_1_.member(cached_LA1)))
			{
				mRE_OPTIONS(false);
			}
			else {
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLESS_THAN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LESS_THAN;
		
		match('<');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSHIFT_LEFT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SHIFT_LEFT;
		
		match("<<");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mINPLACE_SHIFT_LEFT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = INPLACE_SHIFT_LEFT;
		
		match("<<=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mGREATER_THAN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = GREATER_THAN;
		
		match('>');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSHIFT_RIGHT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SHIFT_RIGHT;
		
		match(">>");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mINPLACE_SHIFT_RIGHT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = INPLACE_SHIFT_RIGHT;
		
		match(">>=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mONES_COMPLEMENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = ONES_COMPLEMENT;
		
		match('~');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mCMP_OPERATOR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = CMP_OPERATOR;
		
		switch ( cached_LA1 )
		{
		case '<':
		{
			match("<=");
			break;
		}
		case '>':
		{
			match(">=");
			break;
		}
		default:
			if ((cached_LA1=='!') && (cached_LA2=='~'))
			{
				match("!~");
			}
			else if ((cached_LA1=='!') && (cached_LA2=='=')) {
				match("!=");
			}
		else
		{
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
		}
		break; }
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = ASSIGN;
		
		match('=');
		{
			if ((cached_LA1=='='||cached_LA1=='~'))
			{
				{
					switch ( cached_LA1 )
					{
					case '=':
					{
						match('=');
						break;
					}
					case '~':
					{
						match('~');
						break;
					}
					default:
					{
						throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
					}
					 }
				}
				if (0==inputState.guessing)
				{
					_ttype = CMP_OPERATOR;
				}
			}
			else {
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mCOMMA(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = COMMA;
		
		match(',');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mTRIPLE_QUOTED_STRING(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = TRIPLE_QUOTED_STRING;
		
		int _saveIndex = 0;
		_saveIndex = text.Length;
		match("\"\"");
		text.Length = _saveIndex;
		{    // ( ... )*
			for (;;)
			{
				// nongreedy exit test
				if ((cached_LA1=='"') && (cached_LA2=='"') && (LA(3)=='"')) goto _loop800_breakloop;
				bool synPredMatched794 = false;
				if (((cached_LA1=='$') && (cached_LA2=='('||cached_LA2=='{') && ((LA(3) >= '\u0003' && LA(3) <= '\ufffe'))))
				{
					int _m794 = mark();
					synPredMatched794 = true;
					inputState.guessing++;
					try {
						{
							if ((cached_LA1=='$') && (cached_LA2=='{'))
							{
								match("${");
							}
							else if ((cached_LA1=='$') && (cached_LA2=='(')) {
								match("$(");
							}
							else
							{
								throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
							}
							
						}
					}
					catch (RecognitionException)
					{
						synPredMatched794 = false;
					}
					rewind(_m794);
					inputState.guessing--;
				}
				if ( synPredMatched794 )
				{
					if (0==inputState.guessing)
					{
											
									Enqueue(makeToken(TRIPLE_QUOTED_STRING), text.ToString(_begin, text.Length-_begin));
									text.Length = _begin; text.Append("");
								
					}
					mINTERPOLATED_EXPRESSION(false);
				}
				else {
					bool synPredMatched796 = false;
					if (((cached_LA1=='$') && (tokenSet_0_.member(cached_LA2)) && ((LA(3) >= '\u0003' && LA(3) <= '\ufffe'))))
					{
						int _m796 = mark();
						synPredMatched796 = true;
						inputState.guessing++;
						try {
							{
								match('$');
								mID(false);
							}
						}
						catch (RecognitionException)
						{
							synPredMatched796 = false;
						}
						rewind(_m796);
						inputState.guessing--;
					}
					if ( synPredMatched796 )
					{
						if (0==inputState.guessing)
						{
							
										Enqueue(makeToken(TRIPLE_QUOTED_STRING), text.ToString(_begin, text.Length-_begin));
										text.Length = _begin; text.Append("");
									
						}
						mINTERPOLATED_REFERENCE(false);
					}
					else {
						bool synPredMatched798 = false;
						if (((cached_LA1=='\\') && (cached_LA2=='$') && ((LA(3) >= '\u0003' && LA(3) <= '\ufffe'))))
						{
							int _m798 = mark();
							synPredMatched798 = true;
							inputState.guessing++;
							try {
								{
									match("\\$");
								}
							}
							catch (RecognitionException)
							{
								synPredMatched798 = false;
							}
							rewind(_m798);
							inputState.guessing--;
						}
						if ( synPredMatched798 )
						{
							_saveIndex = text.Length;
							match('\\');
							text.Length = _saveIndex;
							match('$');
						}
						else if ((tokenSet_2_.member(cached_LA1)) && ((cached_LA2 >= '\u0003' && cached_LA2 <= '\ufffe')) && ((LA(3) >= '\u0003' && LA(3) <= '\ufffe'))) {
							{
								match(tokenSet_2_);
							}
						}
						else if ((cached_LA1=='\n'||cached_LA1=='\r')) {
							mNEWLINE(false);
						}
						else
						{
							goto _loop800_breakloop;
						}
						}}
					}
_loop800_breakloop:					;
				}    // ( ... )*
				_saveIndex = text.Length;
				match("\"\"\"");
				text.Length = _saveIndex;
				if (_createToken && (null == _token) && (_ttype != Token.SKIP))
				{
					_token = makeToken(_ttype);
					_token.setText(text.ToString(_begin, text.Length-_begin));
				}
				returnToken_ = _token;
			}
			
	protected void mINTERPOLATED_EXPRESSION(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = INTERPOLATED_EXPRESSION;
		
		if ((cached_LA1=='$') && (cached_LA2=='{'))
		{
			int _saveIndex = 0;
			_saveIndex = text.Length;
			match("${");
			text.Length = _saveIndex;
			if (0==inputState.guessing)
			{
				ParseInterpolatedExpression(RBRACE, LBRACE);
			}
		}
		else if ((cached_LA1=='$') && (cached_LA2=='(')) {
			int _saveIndex = 0;
			_saveIndex = text.Length;
			match("$(");
			text.Length = _saveIndex;
			if (0==inputState.guessing)
			{
				ParseInterpolatedExpression(RPAREN, LPAREN);
			}
		}
		else
		{
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
		}
		
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mINTERPOLATED_REFERENCE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = INTERPOLATED_REFERENCE;
		IToken id = null;
		
		int _saveIndex = 0;
		_saveIndex = text.Length;
		match("$");
		text.Length = _saveIndex;
		_saveIndex = text.Length;
		mID(true);
		text.Length = _saveIndex;
		id = returnToken_;
		if (0==inputState.guessing)
		{
			EnqueueInterpolatedToken(id);
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mDOUBLE_QUOTED_STRING(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DOUBLE_QUOTED_STRING;
		
		int _saveIndex = 0;
		_saveIndex = text.Length;
		match('"');
		text.Length = _saveIndex;
		{
			if (((cached_LA1=='"') && (cached_LA2=='"') && ((LA(3) >= '\u0003' && LA(3) <= '\ufffe')))&&(LA(1)=='"' && LA(2)=='"'))
			{
				mTRIPLE_QUOTED_STRING(false);
				if (0==inputState.guessing)
				{
					_ttype = TRIPLE_QUOTED_STRING;
				}
			}
			else if ((tokenSet_2_.member(cached_LA1)) && (true) && (true)) {
				{
					{    // ( ... )*
						for (;;)
						{
							bool synPredMatched806 = false;
							if (((cached_LA1=='$') && (cached_LA2=='('||cached_LA2=='{') && (tokenSet_2_.member(LA(3)))))
							{
								int _m806 = mark();
								synPredMatched806 = true;
								inputState.guessing++;
								try {
									{
										if ((cached_LA1=='$') && (cached_LA2=='{'))
										{
											match("${");
										}
										else if ((cached_LA1=='$') && (cached_LA2=='(')) {
											match("$(");
										}
										else
										{
											throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
										}
										
									}
								}
								catch (RecognitionException)
								{
									synPredMatched806 = false;
								}
								rewind(_m806);
								inputState.guessing--;
							}
							if ( synPredMatched806 )
							{
								if (0==inputState.guessing)
								{
														
														Enqueue(makeToken(DOUBLE_QUOTED_STRING), text.ToString(_begin, text.Length-_begin));
														text.Length = _begin; text.Append("");
													
								}
								mINTERPOLATED_EXPRESSION(false);
							}
							else {
								bool synPredMatched808 = false;
								if (((cached_LA1=='$') && (tokenSet_0_.member(cached_LA2)) && (tokenSet_2_.member(LA(3)))))
								{
									int _m808 = mark();
									synPredMatched808 = true;
									inputState.guessing++;
									try {
										{
											match('$');
											mID(false);
										}
									}
									catch (RecognitionException)
									{
										synPredMatched808 = false;
									}
									rewind(_m808);
									inputState.guessing--;
								}
								if ( synPredMatched808 )
								{
									if (0==inputState.guessing)
									{
										
															Enqueue(makeToken(DOUBLE_QUOTED_STRING), text.ToString(_begin, text.Length-_begin));
															text.Length = _begin; text.Append("");
														
									}
									mINTERPOLATED_REFERENCE(false);
								}
								else if ((tokenSet_7_.member(cached_LA1)) && (tokenSet_2_.member(cached_LA2)) && (true)) {
									{
										match(tokenSet_7_);
									}
								}
								else if ((cached_LA1=='\\')) {
									mDQS_ESC(false);
								}
								else
								{
									goto _loop810_breakloop;
								}
								}
							}
_loop810_breakloop:							;
						}    // ( ... )*
						_saveIndex = text.Length;
						match('"');
						text.Length = _saveIndex;
					}
				}
				else
				{
					throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
				}
				
			}
			if (0==inputState.guessing)
			{
				
						if (_erecorder.Count > 0)
						{
							Enqueue(makeToken(DOUBLE_QUOTED_STRING), text.ToString(_begin, text.Length-_begin));
							_ttype = ESEPARATOR;
							text.Length = _begin; text.Append("");			
							_selector.push(_erecorder);
						}
					
			}
			if (_createToken && (null == _token) && (_ttype != Token.SKIP))
			{
				_token = makeToken(_ttype);
				_token.setText(text.ToString(_begin, text.Length-_begin));
			}
			returnToken_ = _token;
		}
		
	protected void mDQS_ESC(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DQS_ESC;
		
		int _saveIndex = 0;
		_saveIndex = text.Length;
		match('\\');
		text.Length = _saveIndex;
		{
			switch ( cached_LA1 )
			{
			case '0':  case '\\':  case 'a':  case 'b':
			case 'f':  case 'n':  case 'r':  case 't':
			case 'u':
			{
				mSESC(false);
				break;
			}
			case '"':
			{
				match('"');
				break;
			}
			case '$':
			{
				match('$');
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSINGLE_QUOTED_STRING(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SINGLE_QUOTED_STRING;
		
		int _saveIndex = 0;
		_saveIndex = text.Length;
		match('\'');
		text.Length = _saveIndex;
		{    // ( ... )*
			for (;;)
			{
				if ((cached_LA1=='\\'))
				{
					mSQS_ESC(false);
				}
				else if ((tokenSet_8_.member(cached_LA1))) {
					{
						match(tokenSet_8_);
					}
				}
				else
				{
					goto _loop816_breakloop;
				}
				
			}
_loop816_breakloop:			;
		}    // ( ... )*
		_saveIndex = text.Length;
		match('\'');
		text.Length = _saveIndex;
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mSQS_ESC(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SQS_ESC;
		
		int _saveIndex = 0;
		_saveIndex = text.Length;
		match('\\');
		text.Length = _saveIndex;
		{
			switch ( cached_LA1 )
			{
			case '0':  case '\\':  case 'a':  case 'b':
			case 'f':  case 'n':  case 'r':  case 't':
			case 'u':
			{
				mSESC(false);
				break;
			}
			case '\'':
			{
				match('\'');
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mBACKTICK_QUOTED_STRING(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = BACKTICK_QUOTED_STRING;
		
		int _saveIndex = 0;
		_saveIndex = text.Length;
		match('`');
		text.Length = _saveIndex;
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_9_.member(cached_LA1)))
				{
					{
						match(tokenSet_9_);
					}
				}
				else if ((cached_LA1=='\n'||cached_LA1=='\r')) {
					mNEWLINE(false);
				}
				else
				{
					goto _loop820_breakloop;
				}
				
			}
_loop820_breakloop:			;
		}    // ( ... )*
		_saveIndex = text.Length;
		match('`');
		text.Length = _saveIndex;
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mWS(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = WS;
		
		{ // ( ... )+
			int _cnt833=0;
			for (;;)
			{
				switch ( cached_LA1 )
				{
				case ' ':
				{
					match(' ');
					break;
				}
				case '\t':
				{
					match('\t');
					break;
				}
				case '\u000c':
				{
					match('\f');
					break;
				}
				case '\n':  case '\r':
				{
					mNEWLINE(false);
					break;
				}
				default:
				{
					if (_cnt833 >= 1) { goto _loop833_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				break; }
				_cnt833++;
			}
_loop833_breakloop:			;
		}    // ( ... )+
		if (0==inputState.guessing)
		{
			
					if (SkipWhitespace)
					{
						_ttype = Token.SKIP;
					}
				
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mEOS(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = EOS;
		
		match(';');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mX_RE_LITERAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = X_RE_LITERAL;
		
		int _saveIndex = 0;
		_saveIndex = text.Length;
		match('@');
		text.Length = _saveIndex;
		match('/');
		{ // ( ... )+
			int _cnt837=0;
			for (;;)
			{
				if ((tokenSet_10_.member(cached_LA1)))
				{
					mX_RE_CHAR(false);
				}
				else
				{
					if (_cnt837 >= 1) { goto _loop837_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				
				_cnt837++;
			}
_loop837_breakloop:			;
		}    // ( ... )+
		match('/');
		if (0==inputState.guessing)
		{
			_ttype = RE_LITERAL;
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mX_RE_CHAR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = X_RE_CHAR;
		
		switch ( cached_LA1 )
		{
		case ' ':
		{
			match(' ');
			break;
		}
		case '\t':
		{
			match('\t');
			break;
		}
		default:
			if ((tokenSet_5_.member(cached_LA1)))
			{
				mRE_CHAR(false);
			}
		else
		{
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
		}
		break; }
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mSESC(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SESC;
		
		switch ( cached_LA1 )
		{
		case 'r':
		{
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('r');
				text.Length = _saveIndex;
				if (0==inputState.guessing)
				{
					text.Length = _begin; text.Append("\r");
				}
			}
			break;
		}
		case 'n':
		{
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('n');
				text.Length = _saveIndex;
				if (0==inputState.guessing)
				{
					text.Length = _begin; text.Append("\n");
				}
			}
			break;
		}
		case 't':
		{
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('t');
				text.Length = _saveIndex;
				if (0==inputState.guessing)
				{
					text.Length = _begin; text.Append("\t");
				}
			}
			break;
		}
		case 'a':
		{
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('a');
				text.Length = _saveIndex;
				if (0==inputState.guessing)
				{
					text.Length = _begin; text.Append("\a");
				}
			}
			break;
		}
		case 'b':
		{
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('b');
				text.Length = _saveIndex;
				if (0==inputState.guessing)
				{
					text.Length = _begin; text.Append("\b");
				}
			}
			break;
		}
		case 'f':
		{
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('f');
				text.Length = _saveIndex;
				if (0==inputState.guessing)
				{
					text.Length = _begin; text.Append("\f");
				}
			}
			break;
		}
		case '0':
		{
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('0');
				text.Length = _saveIndex;
				if (0==inputState.guessing)
				{
					text.Length = _begin; text.Append("\0");
				}
			}
			break;
		}
		case 'u':
		{
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('u');
				text.Length = _saveIndex;
				mHEXDIGIT(false);
				mHEXDIGIT(false);
				mHEXDIGIT(false);
				mHEXDIGIT(false);
				if (0==inputState.guessing)
				{
					
											char ch = (char)int.Parse(text.ToString(_begin, 4), System.Globalization.NumberStyles.HexNumber);
											text.Length = _begin;
											text.Append(ch);
										
				}
			}
			break;
		}
		case '\\':
		{
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('\\');
				text.Length = _saveIndex;
				if (0==inputState.guessing)
				{
					text.Length = _begin; text.Append("\\");
				}
			}
			break;
		}
		default:
		{
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
		}
		 }
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mRE_CHAR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RE_CHAR;
		
		if ((cached_LA1=='\\'))
		{
			mRE_ESC(false);
		}
		else if ((tokenSet_11_.member(cached_LA1))) {
			{
				match(tokenSet_11_);
			}
		}
		else
		{
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
		}
		
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mRE_OPTIONS(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RE_OPTIONS;
		
		{ // ( ... )+
			int _cnt865=0;
			for (;;)
			{
				if ((tokenSet_1_.member(cached_LA1)))
				{
					mID_LETTER(false);
				}
				else
				{
					if (_cnt865 >= 1) { goto _loop865_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				
				_cnt865++;
			}
_loop865_breakloop:			;
		}    // ( ... )+
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mRE_ESC(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RE_ESC;
		
		match('\\');
		{
			switch ( cached_LA1 )
			{
			case '+':
			{
				match('+');
				break;
			}
			case '/':
			{
				match('/');
				break;
			}
			case '(':
			{
				match('(');
				break;
			}
			case ')':
			{
				match(')');
				break;
			}
			case '|':
			{
				match('|');
				break;
			}
			case '.':
			{
				match('.');
				break;
			}
			case '*':
			{
				match('*');
				break;
			}
			case '?':
			{
				match('?');
				break;
			}
			case '$':
			{
				match('$');
				break;
			}
			case '^':
			{
				match('^');
				break;
			}
			case '[':
			{
				match('[');
				break;
			}
			case ']':
			{
				match(']');
				break;
			}
			case '{':
			{
				match('{');
				break;
			}
			case '}':
			{
				match('}');
				break;
			}
			case 'a':
			{
				match('a');
				break;
			}
			case 'b':
			{
				match('b');
				break;
			}
			case 'c':
			{
				{
					match('c');
					matchRange('A','Z');
				}
				break;
			}
			case 't':
			{
				match('t');
				break;
			}
			case 'r':
			{
				match('r');
				break;
			}
			case 'v':
			{
				match('v');
				break;
			}
			case 'f':
			{
				match('f');
				break;
			}
			case 'n':
			{
				match('n');
				break;
			}
			case 'e':
			{
				match('e');
				break;
			}
			case '0':  case '1':  case '2':  case '3':
			case '4':  case '5':  case '6':  case '7':
			case '8':  case '9':
			{
				{ // ( ... )+
					int _cnt870=0;
					for (;;)
					{
						if (((cached_LA1 >= '0' && cached_LA1 <= '9')) && (tokenSet_2_.member(cached_LA2)) && (true))
						{
							mDIGIT(false);
						}
						else
						{
							if (_cnt870 >= 1) { goto _loop870_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
						}
						
						_cnt870++;
					}
_loop870_breakloop:					;
				}    // ( ... )+
				break;
			}
			case 'x':
			{
				{
					match('x');
					mHEXDIGIT(false);
					mHEXDIGIT(false);
				}
				break;
			}
			case 'u':
			{
				{
					match('u');
					mHEXDIGIT(false);
					mHEXDIGIT(false);
					mHEXDIGIT(false);
					mHEXDIGIT(false);
				}
				break;
			}
			case '\\':
			{
				match('\\');
				break;
			}
			case 'w':
			{
				match('w');
				break;
			}
			case 'W':
			{
				match('W');
				break;
			}
			case 's':
			{
				match('s');
				break;
			}
			case 'S':
			{
				match('S');
				break;
			}
			case 'd':
			{
				match('d');
				break;
			}
			case 'D':
			{
				match('D');
				break;
			}
			case 'p':
			{
				match('p');
				break;
			}
			case 'P':
			{
				match('P');
				break;
			}
			case 'A':
			{
				match('A');
				break;
			}
			case 'z':
			{
				match('z');
				break;
			}
			case 'Z':
			{
				match('Z');
				break;
			}
			case 'g':
			{
				match('g');
				break;
			}
			case 'B':
			{
				match('B');
				break;
			}
			case 'k':
			{
				match('k');
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mNULLABLE_SUFFIX(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = NULLABLE_SUFFIX;
		
		match('?');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = new long[3072];
		data[0]=0L;
		data[1]=576460745995190271L;
		for (int i = 2; i<=1022; i++) { data[i]=-1L; }
		data[1023]=9223372036854775807L;
		for (int i = 1024; i<=3071; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = new long[3072];
		data[0]=0L;
		data[1]=576460745995190270L;
		for (int i = 2; i<=1022; i++) { data[i]=-1L; }
		data[1023]=9223372036854775807L;
		for (int i = 1024; i<=3071; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = new long[2048];
		data[0]=-9224L;
		for (int i = 1; i<=1022; i++) { data[i]=-1L; }
		data[1023]=9223372036854775807L;
		for (int i = 1024; i<=2047; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = new long[2048];
		data[0]=-4398046520328L;
		for (int i = 1; i<=1022; i++) { data[i]=-1L; }
		data[1023]=9223372036854775807L;
		for (int i = 1024; i<=2047; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = new long[1025];
		data[0]=287948901175001088L;
		data[1]=541165879422L;
		for (int i = 2; i<=1024; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = new long[2048];
		data[0]=-140741783332360L;
		for (int i = 1; i<=1022; i++) { data[i]=-1L; }
		data[1023]=9223372036854775807L;
		for (int i = 1024; i<=2047; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = new long[2048];
		data[0]=-4294977032L;
		for (int i = 1; i<=1022; i++) { data[i]=-1L; }
		data[1023]=9223372036854775807L;
		for (int i = 1024; i<=2047; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = new long[2048];
		data[0]=-17179878408L;
		data[1]=-268435457L;
		for (int i = 2; i<=1022; i++) { data[i]=-1L; }
		data[1023]=9223372036854775807L;
		for (int i = 1024; i<=2047; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	private static long[] mk_tokenSet_8_()
	{
		long[] data = new long[2048];
		data[0]=-549755823112L;
		data[1]=-268435457L;
		for (int i = 2; i<=1022; i++) { data[i]=-1L; }
		data[1023]=9223372036854775807L;
		for (int i = 1024; i<=2047; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
	private static long[] mk_tokenSet_9_()
	{
		long[] data = new long[2048];
		data[0]=-9224L;
		data[1]=-4294967297L;
		for (int i = 2; i<=1022; i++) { data[i]=-1L; }
		data[1023]=9223372036854775807L;
		for (int i = 1024; i<=2047; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
	private static long[] mk_tokenSet_10_()
	{
		long[] data = new long[2048];
		data[0]=-140737488364552L;
		for (int i = 1; i<=1022; i++) { data[i]=-1L; }
		data[1023]=9223372036854775807L;
		for (int i = 1024; i<=2047; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
	private static long[] mk_tokenSet_11_()
	{
		long[] data = new long[2048];
		data[0]=-140741783332360L;
		data[1]=-268435457L;
		for (int i = 2; i<=1022; i++) { data[i]=-1L; }
		data[1023]=9223372036854775807L;
		for (int i = 1024; i<=2047; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
	
}
}
