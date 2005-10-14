// $ANTLR 2.7.5 (20050517): "src/WSABoo.Parser/wsaboo.g" -> "WSABooParserBase.cs"$

namespace WSABoo.Parser
{
	// Generate the header common to all output files.
	using System;
	
	using TokenBuffer              = antlr.TokenBuffer;
	using TokenStreamException     = antlr.TokenStreamException;
	using TokenStreamIOException   = antlr.TokenStreamIOException;
	using ANTLRException           = antlr.ANTLRException;
	using LLkParser = antlr.LLkParser;
	using Token                    = antlr.Token;
	using IToken                   = antlr.IToken;
	using TokenStream              = antlr.TokenStream;
	using RecognitionException     = antlr.RecognitionException;
	using NoViableAltException     = antlr.NoViableAltException;
	using MismatchedTokenException = antlr.MismatchedTokenException;
	using SemanticException        = antlr.SemanticException;
	using ParserSharedInputState   = antlr.ParserSharedInputState;
	using BitSet                   = antlr.collections.impl.BitSet;
	
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Parser;
using Boo.Lang.Parser.Util;
using System.Globalization;

	public 	class WSABooParserBase : antlr.LLkParser
	{
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int ELIST = 4;
		public const int DLIST = 5;
		public const int ESEPARATOR = 6;
		public const int ABSTRACT = 7;
		public const int AND = 8;
		public const int AS = 9;
		public const int AST = 10;
		public const int BREAK = 11;
		public const int CONTINUE = 12;
		public const int CALLABLE = 13;
		public const int CAST = 14;
		public const int CHAR = 15;
		public const int CLASS = 16;
		public const int CONSTRUCTOR = 17;
		public const int DEF = 18;
		public const int DESTRUCTOR = 19;
		public const int DO = 20;
		public const int ELIF = 21;
		public const int ELSE = 22;
		public const int END = 23;
		public const int ENSURE = 24;
		public const int ENUM = 25;
		public const int EVENT = 26;
		public const int EXCEPT = 27;
		public const int FAILURE = 28;
		public const int FINAL = 29;
		public const int FROM = 30;
		public const int FOR = 31;
		public const int FALSE = 32;
		public const int GET = 33;
		public const int GIVEN = 34;
		public const int GOTO = 35;
		public const int IMPORT = 36;
		public const int INTERFACE = 37;
		public const int INTERNAL = 38;
		public const int IS = 39;
		public const int ISA = 40;
		public const int IF = 41;
		public const int IN = 42;
		public const int NOT = 43;
		public const int NULL = 44;
		public const int OR = 45;
		public const int OTHERWISE = 46;
		public const int OVERRIDE = 47;
		public const int NAMESPACE = 48;
		public const int PUBLIC = 49;
		public const int PROTECTED = 50;
		public const int PRIVATE = 51;
		public const int RAISE = 52;
		public const int REF = 53;
		public const int RETURN = 54;
		public const int RETRY = 55;
		public const int SET = 56;
		public const int SELF = 57;
		public const int SUPER = 58;
		public const int STATIC = 59;
		public const int SUCCESS = 60;
		public const int STRUCT = 61;
		public const int TRY = 62;
		public const int TRANSIENT = 63;
		public const int TRUE = 64;
		public const int TYPEOF = 65;
		public const int UNLESS = 66;
		public const int VIRTUAL = 67;
		public const int WHEN = 68;
		public const int WHILE = 69;
		public const int YIELD = 70;
		public const int EOS = 71;
		public const int TRIPLE_QUOTED_STRING = 72;
		public const int DOUBLE_QUOTED_STRING = 73;
		public const int SINGLE_QUOTED_STRING = 74;
		public const int ID = 75;
		public const int LPAREN = 76;
		public const int RPAREN = 77;
		public const int ASSIGN = 78;
		public const int SUBTRACT = 79;
		public const int LBRACK = 80;
		public const int COMMA = 81;
		public const int RBRACK = 82;
		public const int ASSEMBLY_ATTRIBUTE_BEGIN = 83;
		public const int DOT = 84;
		public const int COLON = 85;
		public const int MULTIPLY = 86;
		public const int BITWISE_OR = 87;
		public const int LBRACE = 88;
		public const int RBRACE = 89;
		public const int INPLACE_BITWISE_OR = 90;
		public const int INPLACE_BITWISE_AND = 91;
		public const int INPLACE_SHIFT_LEFT = 92;
		public const int INPLACE_SHIFT_RIGHT = 93;
		public const int CMP_OPERATOR = 94;
		public const int GREATER_THAN = 95;
		public const int LESS_THAN = 96;
		public const int ADD = 97;
		public const int EXCLUSIVE_OR = 98;
		public const int DIVISION = 99;
		public const int MODULUS = 100;
		public const int BITWISE_AND = 101;
		public const int SHIFT_LEFT = 102;
		public const int SHIFT_RIGHT = 103;
		public const int EXPONENTIATION = 104;
		public const int INCREMENT = 105;
		public const int DECREMENT = 106;
		public const int ONES_COMPLEMENT = 107;
		public const int INT = 108;
		public const int LONG = 109;
		public const int RE_LITERAL = 110;
		public const int DOUBLE = 111;
		public const int FLOAT = 112;
		public const int TIMESPAN = 113;
		public const int LINE_CONTINUATION = 114;
		public const int SL_COMMENT = 115;
		public const int ML_COMMENT = 116;
		public const int WS = 117;
		public const int X_RE_LITERAL = 118;
		public const int NEWLINE = 119;
		public const int ESCAPED_EXPRESSION = 120;
		public const int DQS_ESC = 121;
		public const int SQS_ESC = 122;
		public const int SESC = 123;
		public const int RE_CHAR = 124;
		public const int X_RE_CHAR = 125;
		public const int RE_ESC = 126;
		public const int ID_LETTER = 127;
		public const int DIGIT = 128;
		public const int HEXDIGIT = 129;
		
				
	protected System.Text.StringBuilder _sbuilder = new System.Text.StringBuilder();
	
	protected AttributeCollection _attributes = new AttributeCollection();
	
	protected TypeMemberModifiers _modifiers = TypeMemberModifiers.None;

	protected bool _inArray;
	
	protected void ResetMemberData()
	{
		_modifiers = TypeMemberModifiers.None;
	}

	protected void AddAttributes(AttributeCollection target)
	{
		target.Extend(_attributes);
		_attributes.Clear();
	}

	protected LexicalInfo ToLexicalInfo(antlr.IToken token)
	{
		return new LexicalInfo(token.getFilename(),
								token.getLine(),
								token.getColumn());
	}
	
	protected SourceLocation ToSourceLocation(antlr.IToken token)
	{
		return new SourceLocation(token.getLine(), token.getColumn()+token.getText().Length-1);
	}

	protected BinaryOperatorType ParseCmpOperator(string op)
	{
		switch (op)
		{		
			case "<=": return BinaryOperatorType.LessThanOrEqual;		
			case ">=": return BinaryOperatorType.GreaterThanOrEqual;
			case "==": return BinaryOperatorType.Equality;
			case "!=": return BinaryOperatorType.Inequality;
			case "=~": return BinaryOperatorType.Match;
			case "!~": return BinaryOperatorType.NotMatch;
		}
		throw new ArgumentException("op");
	}

	protected BinaryOperatorType ParseAssignOperator(string op)
	{
		switch (op)
		{
			case "=": return BinaryOperatorType.Assign;
			case "+=": return BinaryOperatorType.InPlaceAddition;
			case "-=": return BinaryOperatorType.InPlaceSubtraction;
			case "/=": return BinaryOperatorType.InPlaceDivision;
			case "*=": return BinaryOperatorType.InPlaceMultiply;
			case "^=": return BinaryOperatorType.InPlaceExclusiveOr;
		}
		throw new ArgumentException(op, "op");
	}
	
	static bool IsMethodInvocationExpression(Expression e)
	{
		return NodeType.MethodInvocationExpression == e.NodeType;
	}

	protected TimeSpan ParseTimeSpan(string text)
	{
		if (text.EndsWith("ms"))
		{
			return TimeSpan.FromMilliseconds(ParseDouble(text.Substring(0, text.Length-2)));
		}
	
		char last = text[text.Length-1];		
		double value = ParseDouble(text.Substring(0, text.Length-1));
		switch (last)
		{
			case 's':
			{
				return TimeSpan.FromSeconds(value);
			}
	
			case 'h':
			{
				return TimeSpan.FromHours(value);
			}
			
			case 'm':
			{
				return TimeSpan.FromMinutes(value);
			}			
		}
		return TimeSpan.FromDays(value); 
	}

	// every new line is transformed to '\n'
	// trailing and leading newlines are removed
	protected string MassageDocString(string s)
	{			
		if (s.Length != 0)
		{						
			s = s.Replace("\r\n", "\n");
			
			int length = s.Length;
			int startIndex = 0;			
			if ('\n' == s[0])
			{			
				// assumes '\n'
				startIndex++;
				length--;
			}						
			if ('\n' == s[s.Length-1])
			{
				length--;
			}
			
			if (length > 0)
			{
				return s.Substring(startIndex, length);
			}
			return string.Empty;
		}
		return s;
	}

	protected bool IsValidMacroArgument(int token)
	{
		return LPAREN != token && LBRACK != token;
	}
	
	static double ParseDouble(string s)
	{
		return double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
	}
	
	protected IntegerLiteralExpression ParseIntegerLiteralExpression(
		antlr.IToken token, string s, bool isLong)
	{
		const string HEX_PREFIX = "0x";
		
		long value;
		
		if (s.StartsWith(HEX_PREFIX))
		{
			value = long.Parse(
				s.Substring(HEX_PREFIX.Length), NumberStyles.AllowHexSpecifier, 
					CultureInfo.InvariantCulture);
		}
		else
		{
			value = long.Parse(s, NumberStyles.Integer | NumberStyles.AllowExponent,
					CultureInfo.InvariantCulture);
		}
		return new IntegerLiteralExpression(ToLexicalInfo(token), value, isLong);
	}
	
		
		protected void initialize()
		{
			tokenNames = tokenNames_;
		}
		
		
		protected WSABooParserBase(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
		{
			initialize();
		}
		
		public WSABooParserBase(TokenBuffer tokenBuf) : this(tokenBuf,2)
		{
		}
		
		protected WSABooParserBase(TokenStream lexer, int k) : base(lexer,k)
		{
			initialize();
		}
		
		public WSABooParserBase(TokenStream lexer) : this(lexer,2)
		{
		}
		
		public WSABooParserBase(ParserSharedInputState state) : base(state,2)
		{
			initialize();
		}
		
	protected Module  start(
		CompileUnit cu
	) //throws RecognitionException, TokenStreamException
{
		Module module;
		
		
				module = new Module();		
				module.LexicalInfo = new LexicalInfo(getFilename(), 1, 1);
				
				cu.Modules.Add(module);
			
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==EOS) && (tokenSet_0_.member(LA(2))))
					{
						match(EOS);
					}
					else
					{
						goto _loop3_breakloop;
					}
					
				}
_loop3_breakloop:				;
			}    // ( ... )*
			docstring(module);
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==EOS) && (tokenSet_0_.member(LA(2))))
					{
						match(EOS);
					}
					else
					{
						goto _loop5_breakloop;
					}
					
				}
_loop5_breakloop:				;
			}    // ( ... )*
			{
				switch ( LA(1) )
				{
				case NAMESPACE:
				{
					namespace_directive(module);
					break;
				}
				case EOF:
				case ESEPARATOR:
				case ABSTRACT:
				case AST:
				case BREAK:
				case CONTINUE:
				case CALLABLE:
				case CAST:
				case CHAR:
				case CLASS:
				case DEF:
				case ENUM:
				case FINAL:
				case FOR:
				case FALSE:
				case GIVEN:
				case GOTO:
				case IMPORT:
				case INTERFACE:
				case INTERNAL:
				case IF:
				case NULL:
				case OVERRIDE:
				case PUBLIC:
				case PROTECTED:
				case PRIVATE:
				case RAISE:
				case RETURN:
				case RETRY:
				case SELF:
				case SUPER:
				case STATIC:
				case STRUCT:
				case TRY:
				case TRANSIENT:
				case TRUE:
				case TYPEOF:
				case UNLESS:
				case VIRTUAL:
				case WHILE:
				case YIELD:
				case EOS:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case SUBTRACT:
				case LBRACK:
				case ASSEMBLY_ATTRIBUTE_BEGIN:
				case COLON:
				case LBRACE:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==IMPORT))
					{
						import_directive(module);
					}
					else
					{
						goto _loop8_breakloop;
					}
					
				}
_loop8_breakloop:				;
			}    // ( ... )*
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_1_.member(LA(1))) && (tokenSet_2_.member(LA(2))))
					{
						type_member(module.Members);
					}
					else
					{
						goto _loop10_breakloop;
					}
					
				}
_loop10_breakloop:				;
			}    // ( ... )*
			globals(module);
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==ASSEMBLY_ATTRIBUTE_BEGIN))
					{
						assembly_attribute(module);
						eos();
					}
					else
					{
						goto _loop12_breakloop;
					}
					
				}
_loop12_breakloop:				;
			}    // ( ... )*
			match(Token.EOF_TYPE);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_3_);
			}
			else
			{
				throw ex;
			}
		}
		return module;
	}
	
	protected void docstring(
		Node node
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  doc = null;
		
		try {      // for error handling
			{
				if ((LA(1)==TRIPLE_QUOTED_STRING) && (tokenSet_4_.member(LA(2))))
				{
					doc = LT(1);
					match(TRIPLE_QUOTED_STRING);
					if (0==inputState.guessing)
					{
						node.Documentation = MassageDocString(doc.getText());
					}
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==EOS) && (tokenSet_4_.member(LA(2))))
							{
								match(EOS);
							}
							else
							{
								goto _loop16_breakloop;
							}
							
						}
_loop16_breakloop:						;
					}    // ( ... )*
				}
				else if ((tokenSet_4_.member(LA(1))) && (tokenSet_5_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_4_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void namespace_directive(
		Module container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  t = null;
		
				IToken id;
				NamespaceDeclaration p = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(NAMESPACE);
			id=identifier();
			if (0==inputState.guessing)
			{
				
						p = new NamespaceDeclaration(ToLexicalInfo(t));
						p.Name = id.getText();
						container.Namespace = p; 
					
			}
			eos();
			docstring(p);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_6_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void import_directive(
		Module container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  dqs = null;
		IToken  sqs = null;
		IToken  alias = null;
		
				IToken id;
				Import usingNode = null;
			
		
		try {      // for error handling
			match(IMPORT);
			id=identifier();
			if (0==inputState.guessing)
			{
				
						usingNode = new Import(ToLexicalInfo(id));
						usingNode.Namespace = id.getText();
						container.Imports.Add(usingNode);
					
			}
			{
				switch ( LA(1) )
				{
				case FROM:
				{
					match(FROM);
					{
						switch ( LA(1) )
						{
						case ID:
						{
							id=identifier();
							break;
						}
						case DOUBLE_QUOTED_STRING:
						{
							dqs = LT(1);
							match(DOUBLE_QUOTED_STRING);
							if (0==inputState.guessing)
							{
								id=dqs;
							}
							break;
						}
						case SINGLE_QUOTED_STRING:
						{
							sqs = LT(1);
							match(SINGLE_QUOTED_STRING);
							if (0==inputState.guessing)
							{
								id=sqs;
							}
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					if (0==inputState.guessing)
					{
						
									usingNode.AssemblyReference = new ReferenceExpression(ToLexicalInfo(id));
									usingNode.AssemblyReference.Name = id.getText();
								
					}
					break;
				}
				case AS:
				case EOS:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{
				switch ( LA(1) )
				{
				case AS:
				{
					match(AS);
					alias = LT(1);
					match(ID);
					if (0==inputState.guessing)
					{
						
									usingNode.Alias = new ReferenceExpression(ToLexicalInfo(alias));
									usingNode.Alias.Name = alias.getText();
								
					}
					break;
				}
				case EOS:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			eos();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_6_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void type_member(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			attributes();
			modifiers();
			{
				switch ( LA(1) )
				{
				case CALLABLE:
				case CLASS:
				case ENUM:
				case INTERFACE:
				case STRUCT:
				{
					type_definition(container);
					break;
				}
				case DEF:
				{
					method(container);
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_7_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void globals(
		Module container
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==EOS))
					{
						match(EOS);
					}
					else
					{
						goto _loop136_breakloop;
					}
					
				}
_loop136_breakloop:				;
			}    // ( ... )*
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_8_.member(LA(1))))
					{
						stmt(container.Globals.Statements);
					}
					else
					{
						goto _loop138_breakloop;
					}
					
				}
_loop138_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_9_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void assembly_attribute(
		Module module
	) //throws RecognitionException, TokenStreamException
{
		
		
				antlr.IToken id = null;
				Boo.Lang.Compiler.Ast.Attribute attr = null;
			
		
		try {      // for error handling
			match(ASSEMBLY_ATTRIBUTE_BEGIN);
			id=identifier();
			if (0==inputState.guessing)
			{
				attr = new Boo.Lang.Compiler.Ast.Attribute(ToLexicalInfo(id), id.getText());
			}
			{
				switch ( LA(1) )
				{
				case LPAREN:
				{
					match(LPAREN);
					argument_list(attr);
					match(RPAREN);
					break;
				}
				case RBRACK:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(RBRACK);
			if (0==inputState.guessing)
			{
				module.AssemblyAttributes.Add(attr);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_10_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void eos() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{ // ( ... )+
				int _cnt19=0;
				for (;;)
				{
					if ((LA(1)==EOS) && (tokenSet_11_.member(LA(2))))
					{
						match(EOS);
					}
					else
					{
						if (_cnt19 >= 1) { goto _loop19_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt19++;
				}
_loop19_breakloop:				;
			}    // ( ... )+
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_11_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected IToken  identifier() //throws RecognitionException, TokenStreamException
{
		IToken value;
		
		IToken  id1 = null;
		
				value = null; _sbuilder.Length = 0;
				IToken id2 = null;
			
		
		try {      // for error handling
			id1 = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
									
						_sbuilder.Append(id1.getText());
						value = id1;
					
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==DOT))
					{
						match(DOT);
						id2=member();
						if (0==inputState.guessing)
						{
							_sbuilder.Append('.'); _sbuilder.Append(id2.getText());
						}
					}
					else
					{
						goto _loop494_breakloop;
					}
					
				}
_loop494_breakloop:				;
			}    // ( ... )*
			if (0==inputState.guessing)
			{
				value.setText(_sbuilder.ToString());
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_12_);
			}
			else
			{
				throw ex;
			}
		}
		return value;
	}
	
	protected void attributes() //throws RecognitionException, TokenStreamException
{
		
		
				_attributes.Clear();
			
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==LBRACK))
					{
						match(LBRACK);
						{
							switch ( LA(1) )
							{
							case ID:
							{
								attribute();
								{    // ( ... )*
									for (;;)
									{
										if ((LA(1)==COMMA))
										{
											match(COMMA);
											attribute();
										}
										else
										{
											goto _loop42_breakloop;
										}
										
									}
_loop42_breakloop:									;
								}    // ( ... )*
								break;
							}
							case RBRACK:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						match(RBRACK);
						{    // ( ... )*
							for (;;)
							{
								if ((LA(1)==EOS))
								{
									match(EOS);
								}
								else
								{
									goto _loop44_breakloop;
								}
								
							}
_loop44_breakloop:							;
						}    // ( ... )*
					}
					else
					{
						goto _loop45_breakloop;
					}
					
				}
_loop45_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_13_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void modifiers() //throws RecognitionException, TokenStreamException
{
		
		
			_modifiers = TypeMemberModifiers.None;
		
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					switch ( LA(1) )
					{
					case STATIC:
					{
						match(STATIC);
						if (0==inputState.guessing)
						{
							_modifiers |= TypeMemberModifiers.Static;
						}
						break;
					}
					case PUBLIC:
					{
						match(PUBLIC);
						if (0==inputState.guessing)
						{
							_modifiers |= TypeMemberModifiers.Public;
						}
						break;
					}
					case PROTECTED:
					{
						match(PROTECTED);
						if (0==inputState.guessing)
						{
							_modifiers |= TypeMemberModifiers.Protected;
						}
						break;
					}
					case PRIVATE:
					{
						match(PRIVATE);
						if (0==inputState.guessing)
						{
							_modifiers |= TypeMemberModifiers.Private;
						}
						break;
					}
					case INTERNAL:
					{
						match(INTERNAL);
						if (0==inputState.guessing)
						{
							_modifiers |= TypeMemberModifiers.Internal;
						}
						break;
					}
					case FINAL:
					{
						match(FINAL);
						if (0==inputState.guessing)
						{
							_modifiers |= TypeMemberModifiers.Final;
						}
						break;
					}
					case TRANSIENT:
					{
						match(TRANSIENT);
						if (0==inputState.guessing)
						{
							_modifiers |= TypeMemberModifiers.Transient;
						}
						break;
					}
					case OVERRIDE:
					{
						match(OVERRIDE);
						if (0==inputState.guessing)
						{
							_modifiers |= TypeMemberModifiers.Override;
						}
						break;
					}
					case ABSTRACT:
					{
						match(ABSTRACT);
						if (0==inputState.guessing)
						{
							_modifiers |= TypeMemberModifiers.Abstract;
						}
						break;
					}
					case VIRTUAL:
					{
						match(VIRTUAL);
						if (0==inputState.guessing)
						{
							_modifiers |= TypeMemberModifiers.Virtual;
						}
						break;
					}
					default:
					{
						goto _loop146_breakloop;
					}
					 }
				}
_loop146_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_14_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void type_definition(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case CLASS:
				case STRUCT:
				{
					class_definition(container);
					break;
				}
				case INTERFACE:
				{
					interface_definition(container);
					break;
				}
				case ENUM:
				{
					enum_definition(container);
					break;
				}
				case CALLABLE:
				{
					callable_definition(container);
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_15_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void method(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  t = null;
		IToken  id = null;
		IToken  c = null;
		IToken  d = null;
		
				Method m = null;
				TypeReference rt = null;
				TypeReference it = null;
				ExplicitMemberInfo emi = null;
				ParameterDeclarationCollection parameters = null;
				Block body = null;
				StatementCollection statements = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(DEF);
			{
				switch ( LA(1) )
				{
				case ID:
				{
					{
						if ((LA(1)==ID) && (LA(2)==DOT))
						{
							emi=explicit_member_info();
						}
						else if ((LA(1)==ID) && (LA(2)==LPAREN)) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
					id = LT(1);
					match(ID);
					if (0==inputState.guessing)
					{
						
									if (emi != null)
									{
										m = new Method(emi.LexicalInfo);
									}
									else
									{
										m = new Method(ToLexicalInfo(id));
									}
									m.Name = id.getText();
									m.ExplicitInfo  = emi;
								
					}
					break;
				}
				case CONSTRUCTOR:
				{
					c = LT(1);
					match(CONSTRUCTOR);
					if (0==inputState.guessing)
					{
						m = new Constructor(ToLexicalInfo(c));
					}
					break;
				}
				case DESTRUCTOR:
				{
					d = LT(1);
					match(DESTRUCTOR);
					if (0==inputState.guessing)
					{
						m = new Destructor(ToLexicalInfo(d));
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						m.Modifiers = _modifiers;
						AddAttributes(m.Attributes);
						parameters = m.Parameters;
						body = m.Body;
						statements = body.Statements;
					
			}
			match(LPAREN);
			parameter_declaration_list(parameters);
			match(RPAREN);
			{
				switch ( LA(1) )
				{
				case AS:
				{
					match(AS);
					rt=type_reference();
					if (0==inputState.guessing)
					{
						m.ReturnType = rt;
					}
					break;
				}
				case LBRACK:
				case COLON:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			attributes();
			if (0==inputState.guessing)
			{
				AddAttributes(m.ReturnTypeAttributes);
			}
			begin_block_with_doc(m, body);
			block(statements);
			end(body);
			if (0==inputState.guessing)
			{
				
						container.Add(m);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_15_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void class_definition(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		
				TypeDefinition td = null;
				TypeReferenceCollection baseTypes = null;
				TypeMemberCollection members = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case CLASS:
				{
					match(CLASS);
					if (0==inputState.guessing)
					{
						td = new ClassDefinition();
					}
					break;
				}
				case STRUCT:
				{
					match(STRUCT);
					if (0==inputState.guessing)
					{
						td = new StructDefinition();
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			id = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
						
						td.LexicalInfo = ToLexicalInfo(id);
						td.Name = id.getText();
						td.Modifiers = _modifiers;
						AddAttributes(td.Attributes);
						container.Add(td);
						baseTypes = td.BaseTypes;
						members = td.Members;
					
			}
			{
				switch ( LA(1) )
				{
				case LPAREN:
				{
					base_types(baseTypes);
					break;
				}
				case COLON:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			begin_with_doc(td);
			{
				{    // ( ... )*
					for (;;)
					{
						if ((LA(1)==EOS))
						{
							match(EOS);
						}
						else
						{
							goto _loop55_breakloop;
						}
						
					}
_loop55_breakloop:					;
				}    // ( ... )*
				{ // ( ... )+
					int _cnt57=0;
					for (;;)
					{
						if ((tokenSet_16_.member(LA(1))))
						{
							type_definition_member(members);
						}
						else
						{
							if (_cnt57 >= 1) { goto _loop57_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
						}
						
						_cnt57++;
					}
_loop57_breakloop:					;
				}    // ( ... )+
			}
			end(td);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_15_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void interface_definition(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		
				InterfaceDefinition itf = null;
				TypeMemberCollection members = null;
			
		
		try {      // for error handling
			match(INTERFACE);
			id = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
				
						itf = new InterfaceDefinition(ToLexicalInfo(id));
						itf.Name = id.getText();
						itf.Modifiers = _modifiers;
						AddAttributes(itf.Attributes);
						container.Add(itf);
						members = itf.Members;
					
			}
			{
				switch ( LA(1) )
				{
				case LPAREN:
				{
					base_types(itf.BaseTypes);
					break;
				}
				case COLON:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			begin_with_doc(itf);
			{ // ( ... )+
				int _cnt64=0;
				for (;;)
				{
					if ((tokenSet_17_.member(LA(1))))
					{
						attributes();
						{
							switch ( LA(1) )
							{
							case DEF:
							{
								interface_method(members);
								break;
							}
							case EVENT:
							{
								event_declaration(members);
								break;
							}
							case ID:
							{
								interface_property(members);
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
					}
					else
					{
						if (_cnt64 >= 1) { goto _loop64_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt64++;
				}
_loop64_breakloop:				;
			}    // ( ... )+
			end(itf);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_15_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void enum_definition(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		
				EnumDefinition ed = null;
			
		
		try {      // for error handling
			match(ENUM);
			id = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
				ed = new EnumDefinition(ToLexicalInfo(id));
			}
			begin_with_doc(ed);
			if (0==inputState.guessing)
			{
				
						ed.Name = id.getText();
						ed.Modifiers = _modifiers;
						AddAttributes(ed.Attributes);
						container.Add(ed);
					
			}
			{
				{ // ( ... )+
					int _cnt34=0;
					for (;;)
					{
						if ((LA(1)==ID||LA(1)==LBRACK))
						{
							enum_member(ed);
						}
						else
						{
							if (_cnt34 >= 1) { goto _loop34_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
						}
						
						_cnt34++;
					}
_loop34_breakloop:					;
				}    // ( ... )+
			}
			end(ed);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_15_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void callable_definition(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		
				CallableDefinition cd = null;
				TypeReference returnType = null;
			
		
		try {      // for error handling
			match(CALLABLE);
			id = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
				
						cd = new CallableDefinition(ToLexicalInfo(id));
						cd.Name = id.getText();
						cd.Modifiers = _modifiers;
						AddAttributes(cd.Attributes);
						container.Add(cd);
					
			}
			match(LPAREN);
			parameter_declaration_list(cd.Parameters);
			match(RPAREN);
			{
				switch ( LA(1) )
				{
				case AS:
				{
					match(AS);
					returnType=type_reference();
					if (0==inputState.guessing)
					{
						cd.ReturnType=returnType;
					}
					break;
				}
				case EOS:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			eos();
			docstring(cd);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_15_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void parameter_declaration_list(
		ParameterDeclarationCollection c
	) //throws RecognitionException, TokenStreamException
{
		
		
				bool variableArguments = false;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case REF:
				case ID:
				case LBRACK:
				case MULTIPLY:
				{
					variableArguments=parameter_declaration(c);
					{    // ( ... )*
						for (;;)
						{
							if (((LA(1)==COMMA))&&(!variableArguments))
							{
								{
									match(COMMA);
									variableArguments=parameter_declaration(c);
								}
							}
							else
							{
								goto _loop153_breakloop;
							}
							
						}
_loop153_breakloop:						;
					}    // ( ... )*
					break;
				}
				case RPAREN:
				case BITWISE_OR:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				c.VariableNumber = variableArguments;
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_18_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected TypeReference  type_reference() //throws RecognitionException, TokenStreamException
{
		TypeReference tr;
		
		
				tr=null;
				IToken id = null;
			
		
		try {      // for error handling
			if ((LA(1)==LPAREN))
			{
				tr=array_type_reference();
			}
			else {
				bool synPredMatched176 = false;
				if (((LA(1)==CALLABLE) && (LA(2)==LPAREN)))
				{
					int _m176 = mark();
					synPredMatched176 = true;
					inputState.guessing++;
					try {
						{
							match(CALLABLE);
							match(LPAREN);
						}
					}
					catch (RecognitionException)
					{
						synPredMatched176 = false;
					}
					rewind(_m176);
					inputState.guessing--;
				}
				if ( synPredMatched176 )
				{
					{
						tr=callable_type_reference();
					}
				}
				else if ((LA(1)==CALLABLE||LA(1)==CHAR||LA(1)==ID) && (tokenSet_19_.member(LA(2)))) {
					{
						id=type_name();
						if (0==inputState.guessing)
						{
							
										SimpleTypeReference str = new SimpleTypeReference(ToLexicalInfo(id));
										str.Name = id.getText();
										tr = str;
									
						}
					}
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				}
			}
			catch (RecognitionException ex)
			{
				if (0 == inputState.guessing)
				{
					reportError(ex);
					recover(ex,tokenSet_20_);
				}
				else
				{
					throw ex;
				}
			}
			return tr;
		}
		
	protected void begin_with_doc(
		Node node
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(COLON);
			{
				if ((LA(1)==EOS) && (tokenSet_21_.member(LA(2))))
				{
					match(EOS);
					docstring(node);
				}
				else if ((tokenSet_22_.member(LA(1))) && (tokenSet_23_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_22_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void enum_member(
		EnumDefinition container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
			
				EnumMember em = null;	
				IntegerLiteralExpression initializer = null;
				bool negative = false;		
			
		
		try {      // for error handling
			attributes();
			id = LT(1);
			match(ID);
			{
				switch ( LA(1) )
				{
				case ASSIGN:
				{
					match(ASSIGN);
					{
						switch ( LA(1) )
						{
						case SUBTRACT:
						{
							match(SUBTRACT);
							if (0==inputState.guessing)
							{
								negative = true;
							}
							break;
						}
						case INT:
						case LONG:
						{
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					initializer=integer_literal();
					break;
				}
				case EOS:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						em = new EnumMember(ToLexicalInfo(id));
						em.Name = id.getText();
						em.Initializer = initializer;
						if (negative && null != initializer)
						{
							initializer.Value *= -1;
						}
						AddAttributes(em.Attributes);
						container.Members.Add(em);
					
			}
			eos();
			docstring(em);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_24_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void end(
		Node node
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  t = null;
		
		try {      // for error handling
			t = LT(1);
			match(END);
			if (0==inputState.guessing)
			{
				node.EndSourceLocation = ToSourceLocation(t);
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==EOS) && (tokenSet_25_.member(LA(2))))
					{
						match(EOS);
					}
					else
					{
						goto _loop187_breakloop;
					}
					
				}
_loop187_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_25_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected IntegerLiteralExpression  integer_literal() //throws RecognitionException, TokenStreamException
{
		IntegerLiteralExpression e;
		
		IToken  i = null;
		IToken  l = null;
		e = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case INT:
			{
				i = LT(1);
				match(INT);
				if (0==inputState.guessing)
				{
					
							e = ParseIntegerLiteralExpression(i, i.getText(), false);
						
				}
				break;
			}
			case LONG:
			{
				l = LT(1);
				match(LONG);
				if (0==inputState.guessing)
				{
					
							string s = l.getText();
							s = s.Substring(0, s.Length-1);
							e = ParseIntegerLiteralExpression(l, s, true);
						
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected void attribute() //throws RecognitionException, TokenStreamException
{
		
				
				antlr.IToken id = null;
				Boo.Lang.Compiler.Ast.Attribute attr = null;
			
		
		try {      // for error handling
			id=identifier();
			if (0==inputState.guessing)
			{
				
						attr = new Boo.Lang.Compiler.Ast.Attribute(ToLexicalInfo(id), id.getText());
						_attributes.Add(attr);
					
			}
			{
				switch ( LA(1) )
				{
				case LPAREN:
				{
					match(LPAREN);
					argument_list(attr);
					match(RPAREN);
					break;
				}
				case COMMA:
				case RBRACK:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_27_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void argument_list(
		INodeWithArguments node
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case ESEPARATOR:
				case AST:
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case SUBTRACT:
				case LBRACK:
				case LBRACE:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					argument(node);
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==COMMA))
							{
								match(COMMA);
								argument(node);
							}
							else
							{
								goto _loop486_breakloop;
							}
							
						}
_loop486_breakloop:						;
					}    // ( ... )*
					break;
				}
				case RPAREN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_28_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void base_types(
		TypeReferenceCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		
				TypeReference tr = null;
			
		
		try {      // for error handling
			match(LPAREN);
			{
				switch ( LA(1) )
				{
				case CALLABLE:
				case CHAR:
				case ID:
				case LPAREN:
				{
					tr=type_reference();
					if (0==inputState.guessing)
					{
						container.Add(tr);
					}
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==COMMA))
							{
								match(COMMA);
								tr=type_reference();
								if (0==inputState.guessing)
								{
									container.Add(tr);
								}
							}
							else
							{
								goto _loop68_breakloop;
							}
							
						}
_loop68_breakloop:						;
					}    // ( ... )*
					break;
				}
				case RPAREN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_29_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void type_definition_member(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		
		
		
		try {      // for error handling
			attributes();
			modifiers();
			{
				switch ( LA(1) )
				{
				case DEF:
				{
					method(container);
					break;
				}
				case EVENT:
				{
					event_declaration(container);
					break;
				}
				case ID:
				{
					field_or_property(container);
					break;
				}
				case CALLABLE:
				case CLASS:
				case ENUM:
				case INTERFACE:
				case STRUCT:
				{
					type_definition(container);
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_30_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void event_declaration(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  t = null;
		IToken  id = null;
		
				Event e = null;
				TypeReference tr = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(EVENT);
			id = LT(1);
			match(ID);
			match(AS);
			tr=type_reference();
			eos();
			if (0==inputState.guessing)
			{
				
						e = new Event(ToLexicalInfo(id), id.getText(), tr);
						e.Modifiers = _modifiers;
						AddAttributes(e.Attributes);
						container.Add(e);
					
			}
			docstring(e);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_30_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void field_or_property(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		IToken  id2 = null;
		
				TypeMember tm = null;
				TypeReference tr = null;
				Property p = null;
				Field field = null;
				ExplicitMemberInfo emi = null;
				Expression initializer = null;
				ParameterDeclarationCollection parameters = null;
			
		
		try {      // for error handling
			bool synPredMatched109 = false;
			if (((LA(1)==ID) && (tokenSet_31_.member(LA(2)))))
			{
				int _m109 = mark();
				synPredMatched109 = true;
				inputState.guessing++;
				try {
					{
						property_header();
					}
				}
				catch (RecognitionException)
				{
					synPredMatched109 = false;
				}
				rewind(_m109);
				inputState.guessing--;
			}
			if ( synPredMatched109 )
			{
				{
					{
						if ((LA(1)==ID) && (LA(2)==DOT))
						{
							emi=explicit_member_info();
						}
						else if ((LA(1)==ID) && (LA(2)==AS||LA(2)==LPAREN||LA(2)==COLON)) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
					id = LT(1);
					match(ID);
					{
						if (0==inputState.guessing)
						{
							
											if (emi != null)
												p = new Property(emi.LexicalInfo);
											else
												p = new Property(ToLexicalInfo(id));
											p.Name = id.getText();
											p.ExplicitInfo = emi;
											AddAttributes(p.Attributes);
											parameters = p.Parameters;
										
						}
						{
							switch ( LA(1) )
							{
							case LPAREN:
							{
								match(LPAREN);
								parameter_declaration_list(parameters);
								match(RPAREN);
								break;
							}
							case AS:
							case COLON:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						{
							switch ( LA(1) )
							{
							case AS:
							{
								match(AS);
								tr=type_reference();
								break;
							}
							case COLON:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						if (0==inputState.guessing)
						{
														
											p.Type = tr;
											tm = p;
											tm.Modifiers = _modifiers;
										
						}
						begin_with_doc(p);
						{ // ( ... )+
							int _cnt116=0;
							for (;;)
							{
								if ((tokenSet_32_.member(LA(1))))
								{
									property_accessor(p);
								}
								else
								{
									if (_cnt116 >= 1) { goto _loop116_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
								}
								
								_cnt116++;
							}
_loop116_breakloop:							;
						}    // ( ... )+
						end(p);
					}
				}
				if (0==inputState.guessing)
				{
					container.Add(tm);
				}
			}
			else if ((LA(1)==ID) && (LA(2)==AS||LA(2)==EOS||LA(2)==ASSIGN)) {
				{
					id2 = LT(1);
					match(ID);
					if (0==inputState.guessing)
					{
						
									tm = field = new Field(ToLexicalInfo(id2));
									field.Name = id2.getText();
									field.Modifiers = _modifiers;
									AddAttributes(field.Attributes);
								
					}
					{
						{
							switch ( LA(1) )
							{
							case AS:
							{
								match(AS);
								tr=type_reference();
								if (0==inputState.guessing)
								{
									field.Type = tr;
								}
								break;
							}
							case EOS:
							case ASSIGN:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						{
							switch ( LA(1) )
							{
							case ASSIGN:
							{
								{
									match(ASSIGN);
									initializer=declaration_initializer();
									if (0==inputState.guessing)
									{
										field.Initializer = initializer;	
									}
								}
								break;
							}
							case EOS:
							{
								eos();
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						docstring(field);
					}
				}
				if (0==inputState.guessing)
				{
					container.Add(tm);
				}
			}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_30_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void interface_method(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		
				Method m = null;
				TypeReference rt = null;
			
		
		try {      // for error handling
			match(DEF);
			id = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
				
						m = new Method(ToLexicalInfo(id));
						m.Name = id.getText();
						AddAttributes(m.Attributes);
						container.Add(m);
					
			}
			match(LPAREN);
			parameter_declaration_list(m.Parameters);
			match(RPAREN);
			{
				switch ( LA(1) )
				{
				case AS:
				{
					match(AS);
					rt=type_reference();
					if (0==inputState.guessing)
					{
						m.ReturnType=rt;
					}
					break;
				}
				case EOS:
				case COLON:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{
				switch ( LA(1) )
				{
				case EOS:
				{
					{
						eos();
						docstring(m);
					}
					break;
				}
				case COLON:
				{
					{
						empty_block(m);
						{    // ( ... )*
							for (;;)
							{
								if ((LA(1)==EOS))
								{
									match(EOS);
								}
								else
								{
									goto _loop75_breakloop;
								}
								
							}
_loop75_breakloop:							;
						}    // ( ... )*
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_33_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void interface_property(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		
				Property p = null;
				TypeReference tr = null;
			
		
		try {      // for error handling
			id = LT(1);
			match(ID);
			{
				switch ( LA(1) )
				{
				case AS:
				{
					match(AS);
					tr=type_reference();
					break;
				}
				case COLON:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						p = new Property(ToLexicalInfo(id));
						p.Name = id.getText();
						p.Type = tr;
						AddAttributes(p.Attributes);
						container.Add(p);
					
			}
			begin_with_doc(p);
			{ // ( ... )+
				int _cnt79=0;
				for (;;)
				{
					if ((LA(1)==GET||LA(1)==SET||LA(1)==LBRACK))
					{
						interface_property_accessor(p);
					}
					else
					{
						if (_cnt79 >= 1) { goto _loop79_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt79++;
				}
_loop79_breakloop:				;
			}    // ( ... )+
			end(p);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_33_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void empty_block(
		Node node
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			begin();
			eos();
			end(node);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_34_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void interface_property_accessor(
		Property p
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  gt = null;
		IToken  st = null;
		
				Method m = null;
			
		
		try {      // for error handling
			attributes();
			{
				if (((LA(1)==GET))&&( null == p.Getter ))
				{
					{
						gt = LT(1);
						match(GET);
						if (0==inputState.guessing)
						{
							m = p.Getter = new Method(ToLexicalInfo(gt)); m.Name = "get";
						}
					}
				}
				else if (((LA(1)==SET))&&( null == p.Setter )) {
					{
						st = LT(1);
						match(SET);
						if (0==inputState.guessing)
						{
							m = p.Setter = new Method(ToLexicalInfo(st)); m.Name = "set";
						}
					}
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			{
				switch ( LA(1) )
				{
				case EOS:
				{
					eos();
					break;
				}
				case COLON:
				{
					empty_block(m);
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						AddAttributes(m.Attributes);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_35_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void begin() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(COLON);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_36_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected ExplicitMemberInfo  explicit_member_info() //throws RecognitionException, TokenStreamException
{
		ExplicitMemberInfo emi;
		
		IToken  id = null;
		IToken  id2 = null;
		
				emi = null; _sbuilder.Length = 0;
			
		
		try {      // for error handling
			{
				{
					{
						id = LT(1);
						match(ID);
						match(DOT);
					}
					if (0==inputState.guessing)
					{
						
										emi = new ExplicitMemberInfo(ToLexicalInfo(id));
										_sbuilder.Append(id.getText());
									
					}
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==ID) && (LA(2)==DOT))
							{
								{
									id2 = LT(1);
									match(ID);
									match(DOT);
								}
								if (0==inputState.guessing)
								{
									
														_sbuilder.Append('.');
														_sbuilder.Append(id2.getText());
													
								}
							}
							else
							{
								goto _loop95_breakloop;
							}
							
						}
_loop95_breakloop:						;
					}    // ( ... )*
				}
			}
			if (0==inputState.guessing)
			{
				
						if (emi != null)
						{
							emi.InterfaceType = new SimpleTypeReference(emi.LexicalInfo);
							emi.InterfaceType.Name = _sbuilder.ToString();
						}
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_37_);
			}
			else
			{
				throw ex;
			}
		}
		return emi;
	}
	
	protected void begin_block_with_doc(
		Node node, Block block
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  begin = null;
		
		try {      // for error handling
			begin = LT(1);
			match(COLON);
			{
				if ((LA(1)==EOS) && (tokenSet_38_.member(LA(2))))
				{
					match(EOS);
					docstring(node);
				}
				else if ((tokenSet_38_.member(LA(1))) && (tokenSet_39_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			if (0==inputState.guessing)
			{
				
						block.LexicalInfo = ToLexicalInfo(begin);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_38_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void block(
		StatementCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==EOS))
					{
						match(EOS);
					}
					else
					{
						goto _loop141_breakloop;
					}
					
				}
_loop141_breakloop:				;
			}    // ( ... )*
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_8_.member(LA(1))))
					{
						stmt(container);
					}
					else
					{
						goto _loop143_breakloop;
					}
					
				}
_loop143_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_40_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void property_header() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{
				match(ID);
				{    // ( ... )*
					for (;;)
					{
						if ((LA(1)==DOT))
						{
							match(DOT);
							match(ID);
						}
						else
						{
							goto _loop103_breakloop;
						}
						
					}
_loop103_breakloop:					;
				}    // ( ... )*
			}
			{
				switch ( LA(1) )
				{
				case LPAREN:
				{
					match(LPAREN);
					break;
				}
				case AS:
				case COLON:
				{
					{
						{
							switch ( LA(1) )
							{
							case AS:
							{
								match(AS);
								type_reference();
								break;
							}
							case COLON:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						match(COLON);
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_3_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void property_accessor(
		Property p
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  gt = null;
		IToken  st = null;
				
				Method m = null;
				Block body = null;
			
		
		try {      // for error handling
			attributes();
			modifiers();
			{
				if (((LA(1)==GET))&&( null == p.Getter ))
				{
					{
						gt = LT(1);
						match(GET);
						if (0==inputState.guessing)
						{
							
											p.Getter = m = new Method(ToLexicalInfo(gt));		
											m.Name = "get";
										
						}
					}
				}
				else if (((LA(1)==SET))&&( null == p.Setter )) {
					{
						st = LT(1);
						match(SET);
						if (0==inputState.guessing)
						{
							
											p.Setter = m = new Method(ToLexicalInfo(st));
											m.Name = "set";
										
						}
					}
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			if (0==inputState.guessing)
			{
				
						AddAttributes(m.Attributes);
						m.Modifiers = _modifiers;
						body = m.Body;
					
			}
			compound_stmt(body);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_41_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public Expression  declaration_initializer() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		
			e = null;
		
		
		try {      // for error handling
			bool synPredMatched125 = false;
			if (((tokenSet_42_.member(LA(1))) && (tokenSet_43_.member(LA(2)))))
			{
				int _m125 = mark();
				synPredMatched125 = true;
				inputState.guessing++;
				try {
					{
						slicing_expression();
						{
							switch ( LA(1) )
							{
							case DO:
							{
								match(DO);
								break;
							}
							case DEF:
							{
								match(DEF);
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
					}
				}
				catch (RecognitionException)
				{
					synPredMatched125 = false;
				}
				rewind(_m125);
				inputState.guessing--;
			}
			if ( synPredMatched125 )
			{
				{
					e=slicing_expression();
					method_invocation_block(e);
				}
			}
			else if ((tokenSet_44_.member(LA(1))) && (tokenSet_45_.member(LA(2)))) {
				{
					e=array_or_expression();
					eos();
				}
			}
			else if ((LA(1)==DEF||LA(1)==DO)) {
				{
					e=callable_expression();
				}
			}
			else if ((LA(1)==AST) && (LA(2)==COLON||LA(2)==LBRACE)) {
				{
					e=ast_literal();
				}
			}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_46_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  slicing_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  lbrack = null;
		IToken  lparen = null;
		
				e = null;
				SlicingExpression se = null;
				MethodInvocationExpression mce = null;
				IToken memberName = null;
			
		
		try {      // for error handling
			e=atom();
			{    // ( ... )*
				for (;;)
				{
					switch ( LA(1) )
					{
					case LBRACK:
					{
						{
							lbrack = LT(1);
							match(LBRACK);
							if (0==inputState.guessing)
							{
								
												se = new SlicingExpression(ToLexicalInfo(lbrack));				
												se.Target = e;
												e = se;
											
							}
							slice(se);
							{    // ( ... )*
								for (;;)
								{
									if ((LA(1)==COMMA))
									{
										match(COMMA);
										slice(se);
									}
									else
									{
										goto _loop439_breakloop;
									}
									
								}
_loop439_breakloop:								;
							}    // ( ... )*
							match(RBRACK);
						}
						break;
					}
					case DOT:
					{
						{
							match(DOT);
							memberName=member();
							if (0==inputState.guessing)
							{
								
													MemberReferenceExpression mre = new MemberReferenceExpression(ToLexicalInfo(memberName));
													mre.Target = e;
													mre.Name = memberName.getText();
													e = mre;
												
							}
						}
						break;
					}
					case LPAREN:
					{
						{
							lparen = LT(1);
							match(LPAREN);
							if (0==inputState.guessing)
							{
								
													mce = new MethodInvocationExpression(ToLexicalInfo(lparen));
													mce.Target = e;
													e = mce;
												
							}
							{
								switch ( LA(1) )
								{
								case ESEPARATOR:
								case AST:
								case CAST:
								case CHAR:
								case FALSE:
								case NOT:
								case NULL:
								case SELF:
								case SUPER:
								case TRUE:
								case TYPEOF:
								case TRIPLE_QUOTED_STRING:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case ID:
								case LPAREN:
								case SUBTRACT:
								case LBRACK:
								case MULTIPLY:
								case LBRACE:
								case INCREMENT:
								case DECREMENT:
								case ONES_COMPLEMENT:
								case INT:
								case LONG:
								case RE_LITERAL:
								case DOUBLE:
								case FLOAT:
								case TIMESPAN:
								{
									method_invocation_argument(mce);
									{    // ( ... )*
										for (;;)
										{
											if ((LA(1)==COMMA))
											{
												match(COMMA);
												method_invocation_argument(mce);
											}
											else
											{
												goto _loop444_breakloop;
											}
											
										}
_loop444_breakloop:										;
									}    // ( ... )*
									break;
								}
								case RPAREN:
								{
									break;
								}
								default:
								{
									throw new NoViableAltException(LT(1), getFilename());
								}
								 }
							}
							match(RPAREN);
						}
						break;
					}
					default:
					{
						goto _loop445_breakloop;
					}
					 }
				}
_loop445_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_47_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected void method_invocation_block(
		Expression mi
	) //throws RecognitionException, TokenStreamException
{
		
		
				Expression block = null;
			
		
		try {      // for error handling
			if (!(IsMethodInvocationExpression(mi)))
			  throw new SemanticException("IsMethodInvocationExpression(mi)");
			block=callable_expression();
			if (0==inputState.guessing)
			{
				
						((MethodInvocationExpression)mi).Arguments.Add(block);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_46_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected Expression  array_or_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  c = null;
		IToken  t = null;
		
				e = null;
				ArrayLiteralExpression tle = null;
			
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case COMMA:
			{
				{
					c = LT(1);
					match(COMMA);
					if (0==inputState.guessing)
					{
						e = new ArrayLiteralExpression(ToLexicalInfo(c));
					}
				}
				break;
			}
			case ESEPARATOR:
			case AST:
			case CAST:
			case CHAR:
			case FALSE:
			case NOT:
			case NULL:
			case SELF:
			case SUPER:
			case TRUE:
			case TYPEOF:
			case TRIPLE_QUOTED_STRING:
			case DOUBLE_QUOTED_STRING:
			case SINGLE_QUOTED_STRING:
			case ID:
			case LPAREN:
			case SUBTRACT:
			case LBRACK:
			case LBRACE:
			case INCREMENT:
			case DECREMENT:
			case ONES_COMPLEMENT:
			case INT:
			case LONG:
			case RE_LITERAL:
			case DOUBLE:
			case FLOAT:
			case TIMESPAN:
			{
				{
					e=expression();
					{
						if ((LA(1)==COMMA) && (tokenSet_48_.member(LA(2))))
						{
							t = LT(1);
							match(COMMA);
							if (0==inputState.guessing)
							{
													
													tle = new ArrayLiteralExpression(e.LexicalInfo);
													tle.Items.Add(e);		
												
							}
							{
								if ((tokenSet_49_.member(LA(1))) && (tokenSet_50_.member(LA(2))))
								{
									e=expression();
									if (0==inputState.guessing)
									{
										tle.Items.Add(e);
									}
									{    // ( ... )*
										for (;;)
										{
											if ((LA(1)==COMMA) && (tokenSet_49_.member(LA(2))))
											{
												match(COMMA);
												e=expression();
												if (0==inputState.guessing)
												{
													tle.Items.Add(e);
												}
											}
											else
											{
												goto _loop304_breakloop;
											}
											
										}
_loop304_breakloop:										;
									}    // ( ... )*
									{
										if ((LA(1)==COMMA) && (tokenSet_51_.member(LA(2))))
										{
											match(COMMA);
										}
										else if ((tokenSet_51_.member(LA(1))) && (tokenSet_52_.member(LA(2)))) {
										}
										else
										{
											throw new NoViableAltException(LT(1), getFilename());
										}
										
									}
								}
								else if ((tokenSet_51_.member(LA(1))) && (tokenSet_52_.member(LA(2)))) {
								}
								else
								{
									throw new NoViableAltException(LT(1), getFilename());
								}
								
							}
							if (0==inputState.guessing)
							{
								
													e = tle;
												
							}
						}
						else if ((tokenSet_51_.member(LA(1))) && (tokenSet_52_.member(LA(2)))) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_51_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  callable_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  doAnchor = null;
		IToken  defAnchor = null;
		
				e = null;
				CallableBlockExpression cbe = null;
				TypeReference rt = null;
				IToken anchor = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case DO:
				{
					{
						doAnchor = LT(1);
						match(DO);
						if (0==inputState.guessing)
						{
							anchor = doAnchor;
						}
					}
					break;
				}
				case DEF:
				{
					{
						defAnchor = LT(1);
						match(DEF);
						if (0==inputState.guessing)
						{
							anchor = defAnchor;
						}
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						e = cbe = new CallableBlockExpression(ToLexicalInfo(anchor));
					
			}
			{
				switch ( LA(1) )
				{
				case LPAREN:
				{
					match(LPAREN);
					parameter_declaration_list(cbe.Parameters);
					match(RPAREN);
					{
						switch ( LA(1) )
						{
						case AS:
						{
							match(AS);
							rt=type_reference();
							if (0==inputState.guessing)
							{
								cbe.ReturnType = rt;
							}
							break;
						}
						case COLON:
						{
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					break;
				}
				case COLON:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			compound_stmt(cbe.Body);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_46_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	public AstLiteralExpression  ast_literal() //throws RecognitionException, TokenStreamException
{
		AstLiteralExpression e;
		
		IToken  t = null;
		
			e = null;
		
		
		try {      // for error handling
			t = LT(1);
			match(AST);
			if (0==inputState.guessing)
			{
				e = new AstLiteralExpression(ToLexicalInfo(t));
			}
			{
				switch ( LA(1) )
				{
				case COLON:
				{
					ast_literal_block(e);
					break;
				}
				case LBRACE:
				{
					ast_literal_closure(e);
					eos();
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_46_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected void compound_stmt(
		Block b
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  begin = null;
		
			StatementCollection statements = null;
		
		
		try {      // for error handling
			begin = LT(1);
			match(COLON);
			if (0==inputState.guessing)
			{
				
							b.LexicalInfo = ToLexicalInfo(begin);
							statements = b.Statements;
						
			}
			block(statements);
			end(b);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_53_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void stmt(
		StatementCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		
				Statement s = null;
				StatementModifier m = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case FOR:
				{
					s=for_stmt();
					break;
				}
				case WHILE:
				{
					s=while_stmt();
					break;
				}
				case IF:
				{
					s=if_stmt();
					break;
				}
				case UNLESS:
				{
					s=unless_stmt();
					break;
				}
				case TRY:
				{
					s=try_stmt();
					break;
				}
				case GIVEN:
				{
					s=given_stmt();
					break;
				}
				case RETURN:
				{
					s=return_stmt();
					break;
				}
				default:
					bool synPredMatched198 = false;
					if ((((LA(1)==ID) && (tokenSet_54_.member(LA(2))))&&(IsValidMacroArgument(LA(2)))))
					{
						int _m198 = mark();
						synPredMatched198 = true;
						inputState.guessing++;
						try {
							{
								match(ID);
								{
									if ((tokenSet_49_.member(LA(1))))
									{
										expression();
									}
									else {
									}
									
								}
							}
						}
						catch (RecognitionException)
						{
							synPredMatched198 = false;
						}
						rewind(_m198);
						inputState.guessing--;
					}
					if ( synPredMatched198 )
					{
						s=macro_stmt();
					}
					else {
						bool synPredMatched202 = false;
						if (((tokenSet_42_.member(LA(1))) && (tokenSet_55_.member(LA(2)))))
						{
							int _m202 = mark();
							synPredMatched202 = true;
							inputState.guessing++;
							try {
								{
									slicing_expression();
									{
										switch ( LA(1) )
										{
										case ASSIGN:
										{
											match(ASSIGN);
											break;
										}
										case DEF:
										case DO:
										{
											{
												switch ( LA(1) )
												{
												case DO:
												{
													match(DO);
													break;
												}
												case DEF:
												{
													match(DEF);
													break;
												}
												default:
												{
													throw new NoViableAltException(LT(1), getFilename());
												}
												 }
											}
											break;
										}
										default:
										{
											throw new NoViableAltException(LT(1), getFilename());
										}
										 }
									}
								}
							}
							catch (RecognitionException)
							{
								synPredMatched202 = false;
							}
							rewind(_m202);
							inputState.guessing--;
						}
						if ( synPredMatched202 )
						{
							s=assignment_or_method_invocation_with_block_stmt();
						}
						else {
							bool synPredMatched204 = false;
							if (((LA(1)==ID) && (LA(2)==AS||LA(2)==COMMA)))
							{
								int _m204 = mark();
								synPredMatched204 = true;
								inputState.guessing++;
								try {
									{
										declaration();
										match(COMMA);
									}
								}
								catch (RecognitionException)
								{
									synPredMatched204 = false;
								}
								rewind(_m204);
								inputState.guessing--;
							}
							if ( synPredMatched204 )
							{
								s=unpack_stmt();
							}
							else if ((LA(1)==ID) && (LA(2)==AS)) {
								s=declaration_stmt();
							}
							else if ((tokenSet_56_.member(LA(1))) && (tokenSet_57_.member(LA(2)))) {
								{
									{
										switch ( LA(1) )
										{
										case GOTO:
										{
											s=goto_stmt();
											break;
										}
										case COLON:
										{
											s=label_stmt();
											break;
										}
										case YIELD:
										{
											s=yield_stmt();
											break;
										}
										case BREAK:
										{
											s=break_stmt();
											break;
										}
										case CONTINUE:
										{
											s=continue_stmt();
											break;
										}
										case RAISE:
										{
											s=raise_stmt();
											break;
										}
										case RETRY:
										{
											s=retry_stmt();
											break;
										}
										case ESEPARATOR:
										case AST:
										case CAST:
										case CHAR:
										case FALSE:
										case NULL:
										case SELF:
										case SUPER:
										case TRUE:
										case TYPEOF:
										case TRIPLE_QUOTED_STRING:
										case DOUBLE_QUOTED_STRING:
										case SINGLE_QUOTED_STRING:
										case ID:
										case LPAREN:
										case SUBTRACT:
										case LBRACK:
										case LBRACE:
										case INCREMENT:
										case DECREMENT:
										case ONES_COMPLEMENT:
										case INT:
										case LONG:
										case RE_LITERAL:
										case DOUBLE:
										case FLOAT:
										case TIMESPAN:
										{
											s=expression_stmt();
											break;
										}
										default:
										{
											throw new NoViableAltException(LT(1), getFilename());
										}
										 }
									}
									{
										switch ( LA(1) )
										{
										case IF:
										case UNLESS:
										case WHILE:
										{
											m=stmt_modifier();
											if (0==inputState.guessing)
											{
												s.Modifier = m;
											}
											break;
										}
										case EOS:
										{
											break;
										}
										default:
										{
											throw new NoViableAltException(LT(1), getFilename());
										}
										 }
									}
									eos();
								}
							}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						}}break; }
					}
					if (0==inputState.guessing)
					{
						
								if (null != s)
								{
									container.Add(s);
								}
							
					}
				}
				catch (RecognitionException ex)
				{
					if (0 == inputState.guessing)
					{
						reportError(ex);
						recover(ex,tokenSet_58_);
					}
					else
					{
						throw ex;
					}
				}
			}
			
	protected ParameterModifiers  parameter_modifier() //throws RecognitionException, TokenStreamException
{
		ParameterModifiers pm;
		
		
				pm = ParameterModifiers.None;
			
		
		try {      // for error handling
			{
				match(REF);
				if (0==inputState.guessing)
				{
					pm = ParameterModifiers.Ref;
				}
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_59_);
			}
			else
			{
				throw ex;
			}
		}
		return pm;
	}
	
	protected bool  parameter_declaration(
		ParameterDeclarationCollection c
	) //throws RecognitionException, TokenStreamException
{
		bool variableArguments;
		
		IToken  id1 = null;
		IToken  id2 = null;
				
				IToken id = null;
				TypeReference tr = null;
				ParameterModifiers pm = ParameterModifiers.None;
				variableArguments = false;
			
		
		try {      // for error handling
			attributes();
			{
				switch ( LA(1) )
				{
				case MULTIPLY:
				{
					{
						match(MULTIPLY);
						if (0==inputState.guessing)
						{
							variableArguments=true;
						}
						id1 = LT(1);
						match(ID);
						{
							switch ( LA(1) )
							{
							case AS:
							{
								match(AS);
								tr=array_type_reference();
								break;
							}
							case RPAREN:
							case COMMA:
							case BITWISE_OR:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						if (0==inputState.guessing)
						{
							id = id1;
						}
					}
					break;
				}
				case REF:
				case ID:
				{
					{
						{
							switch ( LA(1) )
							{
							case REF:
							{
								pm=parameter_modifier();
								break;
							}
							case ID:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						id2 = LT(1);
						match(ID);
						{
							switch ( LA(1) )
							{
							case AS:
							{
								match(AS);
								tr=type_reference();
								break;
							}
							case RPAREN:
							case COMMA:
							case BITWISE_OR:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						if (0==inputState.guessing)
						{
							id = id2;
						}
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						ParameterDeclaration pd = new ParameterDeclaration(ToLexicalInfo(id));
						pd.Name = id.getText();
						pd.Type = tr;
						pd.Modifiers = pm;
						AddAttributes(pd.Attributes);
						c.Add(pd);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_60_);
			}
			else
			{
				throw ex;
			}
		}
		return variableArguments;
	}
	
	protected ArrayTypeReference  array_type_reference() //throws RecognitionException, TokenStreamException
{
		ArrayTypeReference atr;
		
		IToken  lparen = null;
		IToken  rparen = null;
		
				TypeReference tr = null;
				atr = null;
				IntegerLiteralExpression rank = null;
			
		
		try {      // for error handling
			lparen = LT(1);
			match(LPAREN);
			if (0==inputState.guessing)
			{
				
						atr = new ArrayTypeReference(ToLexicalInfo(lparen));
					
			}
			{
				tr=type_reference();
				if (0==inputState.guessing)
				{
					atr.ElementType = tr;
				}
				{
					switch ( LA(1) )
					{
					case COMMA:
					{
						match(COMMA);
						rank=integer_literal();
						if (0==inputState.guessing)
						{
							atr.Rank = rank;
						}
						break;
					}
					case RPAREN:
					{
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
			}
			rparen = LT(1);
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_20_);
			}
			else
			{
				throw ex;
			}
		}
		return atr;
	}
	
	protected void callable_parameter_declaration_list(
		ParameterDeclarationCollection c
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case CALLABLE:
				case CHAR:
				case REF:
				case ID:
				case LPAREN:
				{
					callable_parameter_declaration(c);
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==COMMA))
							{
								match(COMMA);
								callable_parameter_declaration(c);
							}
							else
							{
								goto _loop164_breakloop;
							}
							
						}
_loop164_breakloop:						;
					}    // ( ... )*
					break;
				}
				case RPAREN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_28_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void callable_parameter_declaration(
		ParameterDeclarationCollection c
	) //throws RecognitionException, TokenStreamException
{
		
				
				TypeReference tr = null;
				ParameterModifiers pm = ParameterModifiers.None;
			
		
		try {      // for error handling
			{
				{
					switch ( LA(1) )
					{
					case REF:
					{
						pm=parameter_modifier();
						break;
					}
					case CALLABLE:
					case CHAR:
					case ID:
					case LPAREN:
					{
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				{
					tr=type_reference();
				}
			}
			if (0==inputState.guessing)
			{
				
						ParameterDeclaration pd = new ParameterDeclaration(tr.LexicalInfo);
						pd.Name = "arg" + c.Count;
						pd.Type = tr;
						pd.Modifiers = pm;
						c.Add(pd);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_61_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected CallableTypeReference  callable_type_reference() //throws RecognitionException, TokenStreamException
{
		CallableTypeReference ctr;
		
		IToken  c = null;
		
				ctr = null;
				TypeReference tr = null;
				ParameterDeclarationCollection parameters = null;
			
		
		try {      // for error handling
			c = LT(1);
			match(CALLABLE);
			match(LPAREN);
			if (0==inputState.guessing)
			{
				
						ctr = new CallableTypeReference(ToLexicalInfo(c));
						parameters = ctr.Parameters;
					
			}
			callable_parameter_declaration_list(parameters);
			match(RPAREN);
			{
				switch ( LA(1) )
				{
				case AS:
				{
					match(AS);
					tr=type_reference();
					if (0==inputState.guessing)
					{
						
								ctr.ReturnType = tr; 
								
					}
					break;
				}
				case EOF:
				case ESEPARATOR:
				case AND:
				case DEF:
				case DO:
				case FOR:
				case IS:
				case ISA:
				case IF:
				case IN:
				case NOT:
				case OR:
				case UNLESS:
				case WHILE:
				case EOS:
				case RPAREN:
				case ASSIGN:
				case SUBTRACT:
				case LBRACK:
				case COMMA:
				case RBRACK:
				case COLON:
				case MULTIPLY:
				case BITWISE_OR:
				case RBRACE:
				case INPLACE_BITWISE_OR:
				case INPLACE_BITWISE_AND:
				case INPLACE_SHIFT_LEFT:
				case INPLACE_SHIFT_RIGHT:
				case CMP_OPERATOR:
				case GREATER_THAN:
				case LESS_THAN:
				case ADD:
				case EXCLUSIVE_OR:
				case DIVISION:
				case MODULUS:
				case BITWISE_AND:
				case SHIFT_LEFT:
				case SHIFT_RIGHT:
				case EXPONENTIATION:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_20_);
			}
			else
			{
				throw ex;
			}
		}
		return ctr;
	}
	
	protected IToken  type_name() //throws RecognitionException, TokenStreamException
{
		IToken id;
		
		IToken  c = null;
		IToken  ch = null;
		
				id = null;
			
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case ID:
			{
				id=identifier();
				break;
			}
			case CALLABLE:
			{
				c = LT(1);
				match(CALLABLE);
				if (0==inputState.guessing)
				{
					id=c;
				}
				break;
			}
			case CHAR:
			{
				ch = LT(1);
				match(CHAR);
				if (0==inputState.guessing)
				{
					id=ch;
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_20_);
			}
			else
			{
				throw ex;
			}
		}
		return id;
	}
	
	protected MacroStatement  closure_macro_stmt() //throws RecognitionException, TokenStreamException
{
		MacroStatement returnValue;
		
		IToken  id = null;
		
				returnValue = null;
				MacroStatement macro = new MacroStatement();
			
		
		try {      // for error handling
			id = LT(1);
			match(ID);
			expression_list(macro.Arguments);
			if (0==inputState.guessing)
			{
				
						macro.Name = id.getText();
						macro.LexicalInfo = ToLexicalInfo(id);		
						returnValue = macro;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_62_);
			}
			else
			{
				throw ex;
			}
		}
		return returnValue;
	}
	
	protected void expression_list(
		ExpressionCollection ec
	) //throws RecognitionException, TokenStreamException
{
		
		
				Expression e = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case ESEPARATOR:
				case AST:
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case SUBTRACT:
				case LBRACK:
				case LBRACE:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					e=expression();
					if (0==inputState.guessing)
					{
						ec.Add(e);
					}
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==COMMA))
							{
								match(COMMA);
								e=expression();
								if (0==inputState.guessing)
								{
									ec.Add(e);
								}
							}
							else
							{
								goto _loop482_breakloop;
							}
							
						}
_loop482_breakloop:						;
					}    // ( ... )*
					break;
				}
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
				case COLON:
				case RBRACE:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_63_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected MacroStatement  macro_stmt() //throws RecognitionException, TokenStreamException
{
		MacroStatement returnValue;
		
		IToken  id = null;
		
				returnValue = null;
				MacroStatement macro = new MacroStatement();
				StatementModifier modifier = null;
			
		
		try {      // for error handling
			id = LT(1);
			match(ID);
			expression_list(macro.Arguments);
			{
				switch ( LA(1) )
				{
				case COLON:
				{
					compound_stmt(macro.Block);
					break;
				}
				case EOS:
				{
					eos();
					break;
				}
				case IF:
				case UNLESS:
				case WHILE:
				{
					modifier=stmt_modifier();
					eos();
					if (0==inputState.guessing)
					{
						macro.Modifier = modifier;
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						macro.Name = id.getText();
						macro.LexicalInfo = ToLexicalInfo(id);
						
						returnValue = macro;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_58_);
			}
			else
			{
				throw ex;
			}
		}
		return returnValue;
	}
	
	protected StatementModifier  stmt_modifier() //throws RecognitionException, TokenStreamException
{
		StatementModifier m;
		
		IToken  i = null;
		IToken  u = null;
		IToken  w = null;
		
				m = null;
				Expression e = null;
				IToken t = null;
				StatementModifierType type = StatementModifierType.Uninitialized;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case IF:
				{
					i = LT(1);
					match(IF);
					if (0==inputState.guessing)
					{
						t = i; type = StatementModifierType.If;
					}
					break;
				}
				case UNLESS:
				{
					u = LT(1);
					match(UNLESS);
					if (0==inputState.guessing)
					{
						t = u; type = StatementModifierType.Unless;
					}
					break;
				}
				case WHILE:
				{
					w = LT(1);
					match(WHILE);
					if (0==inputState.guessing)
					{
						t = w; type = StatementModifierType.While;
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			e=boolean_expression();
			if (0==inputState.guessing)
			{
				
						m = new StatementModifier(ToLexicalInfo(t));
						m.Type = type;
						m.Condition = e;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_51_);
			}
			else
			{
				throw ex;
			}
		}
		return m;
	}
	
	protected GotoStatement  goto_stmt() //throws RecognitionException, TokenStreamException
{
		GotoStatement stmt;
		
		IToken  token = null;
		IToken  label = null;
		
				stmt = null;
			
		
		try {      // for error handling
			token = LT(1);
			match(GOTO);
			label = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
				
						stmt = new GotoStatement(ToLexicalInfo(token),
									new ReferenceExpression(ToLexicalInfo(label), label.getText()));
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_64_);
			}
			else
			{
				throw ex;
			}
		}
		return stmt;
	}
	
	protected LabelStatement  label_stmt() //throws RecognitionException, TokenStreamException
{
		LabelStatement stmt;
		
		IToken  token = null;
		IToken  label = null;
		
				stmt = null;
			
		
		try {      // for error handling
			token = LT(1);
			match(COLON);
			label = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
				
						stmt = new LabelStatement(ToLexicalInfo(token), label.getText());
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_64_);
			}
			else
			{
				throw ex;
			}
		}
		return stmt;
	}
	
	protected ForStatement  for_stmt() //throws RecognitionException, TokenStreamException
{
		ForStatement fs;
		
		IToken  f = null;
		
				fs = null;
				Expression iterator = null;
				DeclarationCollection declarations = null;
				Block body = null;
			
		
		try {      // for error handling
			f = LT(1);
			match(FOR);
			if (0==inputState.guessing)
			{
				
						fs = new ForStatement(ToLexicalInfo(f));
						declarations = fs.Declarations;
						body = fs.Block;
					
			}
			declaration_list(declarations);
			match(IN);
			iterator=array_or_expression();
			if (0==inputState.guessing)
			{
				fs.Iterator = iterator;
			}
			compound_stmt(body);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_58_);
			}
			else
			{
				throw ex;
			}
		}
		return fs;
	}
	
	protected WhileStatement  while_stmt() //throws RecognitionException, TokenStreamException
{
		WhileStatement ws;
		
		IToken  w = null;
		
				ws = null;
				Expression e = null;
			
		
		try {      // for error handling
			w = LT(1);
			match(WHILE);
			e=expression();
			if (0==inputState.guessing)
			{
				
						ws = new WhileStatement(ToLexicalInfo(w));
						ws.Condition = e;
					
			}
			compound_stmt(ws.Block);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_58_);
			}
			else
			{
				throw ex;
			}
		}
		return ws;
	}
	
	protected IfStatement  if_stmt() //throws RecognitionException, TokenStreamException
{
		IfStatement returnValue;
		
		IToken  it = null;
		IToken  ei = null;
		IToken  et = null;
		
				returnValue = null;
				
				IfStatement s = null;
				Expression e = null;
			
		
		try {      // for error handling
			it = LT(1);
			match(IF);
			e=expression();
			if (0==inputState.guessing)
			{
				
						returnValue = s = new IfStatement(ToLexicalInfo(it));
						s.Condition = e;
						s.TrueBlock = new Block();
					
			}
			compound_stmt(s.TrueBlock);
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==ELIF))
					{
						ei = LT(1);
						match(ELIF);
						e=expression();
						if (0==inputState.guessing)
						{
							
										s.FalseBlock = new Block();
										
										IfStatement elif = new IfStatement(ToLexicalInfo(ei));
										elif.TrueBlock = new Block();
										elif.Condition = e;
										
										s.FalseBlock.Add(elif);
										s = elif;
									
						}
						compound_stmt(s.TrueBlock);
					}
					else
					{
						goto _loop287_breakloop;
					}
					
				}
_loop287_breakloop:				;
			}    // ( ... )*
			{
				switch ( LA(1) )
				{
				case ELSE:
				{
					et = LT(1);
					match(ELSE);
					if (0==inputState.guessing)
					{
						s.FalseBlock = new Block(ToLexicalInfo(et));
					}
					compound_stmt(s.FalseBlock);
					break;
				}
				case EOF:
				case ESEPARATOR:
				case AST:
				case BREAK:
				case CONTINUE:
				case CAST:
				case CHAR:
				case END:
				case FOR:
				case FALSE:
				case GIVEN:
				case GOTO:
				case IF:
				case NULL:
				case RAISE:
				case RETURN:
				case RETRY:
				case SELF:
				case SUPER:
				case TRY:
				case TRUE:
				case TYPEOF:
				case UNLESS:
				case WHILE:
				case YIELD:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case SUBTRACT:
				case LBRACK:
				case ASSEMBLY_ATTRIBUTE_BEGIN:
				case COLON:
				case LBRACE:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_58_);
			}
			else
			{
				throw ex;
			}
		}
		return returnValue;
	}
	
	protected UnlessStatement  unless_stmt() //throws RecognitionException, TokenStreamException
{
		UnlessStatement us;
		
		IToken  u = null;
		
				us = null;
				Expression condition = null;
			
		
		try {      // for error handling
			u = LT(1);
			match(UNLESS);
			condition=expression();
			if (0==inputState.guessing)
			{
				
						us = new UnlessStatement(ToLexicalInfo(u));
						us.Condition = condition;
					
			}
			compound_stmt(us.Block);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_58_);
			}
			else
			{
				throw ex;
			}
		}
		return us;
	}
	
	protected TryStatement  try_stmt() //throws RecognitionException, TokenStreamException
{
		TryStatement s;
		
		IToken  t = null;
		IToken  stoken = null;
		IToken  etoken = null;
		
				s = null;		
				Block sblock = null;
				Block eblock = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(TRY);
			if (0==inputState.guessing)
			{
				s = new TryStatement(ToLexicalInfo(t));
			}
			compound_stmt(s.ProtectedBlock);
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==EXCEPT))
					{
						exception_handler(s);
					}
					else
					{
						goto _loop243_breakloop;
					}
					
				}
_loop243_breakloop:				;
			}    // ( ... )*
			{
				switch ( LA(1) )
				{
				case SUCCESS:
				{
					stoken = LT(1);
					match(SUCCESS);
					if (0==inputState.guessing)
					{
						sblock = new Block(ToLexicalInfo(stoken));
					}
					compound_stmt(sblock);
					if (0==inputState.guessing)
					{
						s.SuccessBlock = sblock;
					}
					break;
				}
				case EOF:
				case ESEPARATOR:
				case AST:
				case BREAK:
				case CONTINUE:
				case CAST:
				case CHAR:
				case END:
				case ENSURE:
				case FOR:
				case FALSE:
				case GIVEN:
				case GOTO:
				case IF:
				case NULL:
				case RAISE:
				case RETURN:
				case RETRY:
				case SELF:
				case SUPER:
				case TRY:
				case TRUE:
				case TYPEOF:
				case UNLESS:
				case WHILE:
				case YIELD:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case SUBTRACT:
				case LBRACK:
				case ASSEMBLY_ATTRIBUTE_BEGIN:
				case COLON:
				case LBRACE:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{
				switch ( LA(1) )
				{
				case ENSURE:
				{
					etoken = LT(1);
					match(ENSURE);
					if (0==inputState.guessing)
					{
						eblock = new Block(ToLexicalInfo(etoken));
					}
					compound_stmt(eblock);
					if (0==inputState.guessing)
					{
						s.EnsureBlock = eblock;
					}
					break;
				}
				case EOF:
				case ESEPARATOR:
				case AST:
				case BREAK:
				case CONTINUE:
				case CAST:
				case CHAR:
				case END:
				case FOR:
				case FALSE:
				case GIVEN:
				case GOTO:
				case IF:
				case NULL:
				case RAISE:
				case RETURN:
				case RETRY:
				case SELF:
				case SUPER:
				case TRY:
				case TRUE:
				case TYPEOF:
				case UNLESS:
				case WHILE:
				case YIELD:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case SUBTRACT:
				case LBRACK:
				case ASSEMBLY_ATTRIBUTE_BEGIN:
				case COLON:
				case LBRACE:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_58_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
	}
	
	protected GivenStatement  given_stmt() //throws RecognitionException, TokenStreamException
{
		GivenStatement gs;
		
		IToken  given = null;
		IToken  when = null;
		IToken  otherwise = null;
		
				gs = null;		
				Expression e = null;
				WhenClause wc = null;
			
		
		try {      // for error handling
			given = LT(1);
			match(GIVEN);
			e=expression();
			if (0==inputState.guessing)
			{
				
						gs = new GivenStatement(ToLexicalInfo(given));
						gs.Expression = e;
					
			}
			begin();
			{ // ( ... )+
				int _cnt283=0;
				for (;;)
				{
					if ((LA(1)==WHEN))
					{
						when = LT(1);
						match(WHEN);
						e=array_or_expression();
						if (0==inputState.guessing)
						{
							
											wc = new WhenClause(ToLexicalInfo(when));
											wc.Condition = e;
											gs.WhenClauses.Add(wc);
										
						}
						compound_stmt(wc.Block);
					}
					else
					{
						if (_cnt283 >= 1) { goto _loop283_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt283++;
				}
_loop283_breakloop:				;
			}    // ( ... )+
			{
				switch ( LA(1) )
				{
				case OTHERWISE:
				{
					otherwise = LT(1);
					match(OTHERWISE);
					if (0==inputState.guessing)
					{
						
										gs.OtherwiseBlock = new Block(ToLexicalInfo(otherwise));
									
					}
					compound_stmt(gs.OtherwiseBlock);
					break;
				}
				case END:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			end(gs);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_58_);
			}
			else
			{
				throw ex;
			}
		}
		return gs;
	}
	
	protected Expression  expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  f = null;
		IToken  f2 = null;
		
				e = null;
				
				ExtendedGeneratorExpression mge = null;
				GeneratorExpression ge = null;
			
		
		try {      // for error handling
			e=boolean_expression();
			{
				if ((LA(1)==FOR) && (LA(2)==ID))
				{
					f = LT(1);
					match(FOR);
					if (0==inputState.guessing)
					{
						
									ge = new GeneratorExpression(ToLexicalInfo(f));
									ge.Expression = e;
									e = ge;
								
					}
					generator_expression_body(ge);
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==FOR) && (LA(2)==ID))
							{
								f2 = LT(1);
								match(FOR);
								if (0==inputState.guessing)
								{
									
													if (null == mge)
													{
														mge = new ExtendedGeneratorExpression(ToLexicalInfo(f));
														mge.Items.Add(ge);
														e = mge;
													}
													
													ge = new GeneratorExpression(ToLexicalInfo(f2));
													mge.Items.Add(ge);
												
								}
								generator_expression_body(ge);
							}
							else
							{
								goto _loop309_breakloop;
							}
							
						}
_loop309_breakloop:						;
					}    // ( ... )*
				}
				else if ((tokenSet_51_.member(LA(1))) && (tokenSet_52_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_51_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Statement  assignment_or_method_invocation_with_block_stmt() //throws RecognitionException, TokenStreamException
{
		Statement stmt;
		
		IToken  op = null;
		
				stmt = null;
				Expression lhs = null;
				Expression rhs = null;
				StatementModifier modifier = null;
				BinaryOperatorType binaryOperator = BinaryOperatorType.None;
				IToken token = null;
			
		
		try {      // for error handling
			lhs=slicing_expression();
			{
				switch ( LA(1) )
				{
				case DEF:
				case DO:
				{
					{
						method_invocation_block(lhs);
						if (0==inputState.guessing)
						{
							stmt = new ExpressionStatement(lhs);
						}
					}
					break;
				}
				case ASSIGN:
				{
					{
						{
							op = LT(1);
							match(ASSIGN);
							if (0==inputState.guessing)
							{
								token = op; binaryOperator = ParseAssignOperator(op.getText());
							}
							{
								bool synPredMatched354 = false;
								if (((LA(1)==DEF||LA(1)==DO)))
								{
									int _m354 = mark();
									synPredMatched354 = true;
									inputState.guessing++;
									try {
										{
											switch ( LA(1) )
											{
											case DEF:
											{
												match(DEF);
												break;
											}
											case DO:
											{
												match(DO);
												break;
											}
											default:
											{
												throw new NoViableAltException(LT(1), getFilename());
											}
											 }
										}
									}
									catch (RecognitionException)
									{
										synPredMatched354 = false;
									}
									rewind(_m354);
									inputState.guessing--;
								}
								if ( synPredMatched354 )
								{
									rhs=callable_expression();
								}
								else if ((tokenSet_44_.member(LA(1))) && (tokenSet_65_.member(LA(2)))) {
									{
										rhs=array_or_expression();
										{
											switch ( LA(1) )
											{
											case DEF:
											case DO:
											{
												method_invocation_block(rhs);
												break;
											}
											case IF:
											case UNLESS:
											case WHILE:
											{
												{
													modifier=stmt_modifier();
													eos();
												}
												break;
											}
											case EOS:
											{
												eos();
												break;
											}
											default:
											{
												throw new NoViableAltException(LT(1), getFilename());
											}
											 }
										}
									}
								}
								else if ((LA(1)==AST) && (LA(2)==COLON||LA(2)==LBRACE)) {
									rhs=ast_literal();
								}
								else
								{
									throw new NoViableAltException(LT(1), getFilename());
								}
								
							}
						}
						if (0==inputState.guessing)
						{
							
											stmt = new ExpressionStatement(
													new BinaryExpression(ToLexicalInfo(token),
														binaryOperator,
														lhs, rhs));
											stmt.Modifier = modifier;
										
						}
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_58_);
			}
			else
			{
				throw ex;
			}
		}
		return stmt;
	}
	
	protected ReturnStatement  return_stmt() //throws RecognitionException, TokenStreamException
{
		ReturnStatement s;
		
		IToken  r = null;
		
				s = null;
				Expression e = null;
				StatementModifier modifier = null;
			
		
		try {      // for error handling
			r = LT(1);
			match(RETURN);
			{
				switch ( LA(1) )
				{
				case DEF:
				case DO:
				{
					{
						e=callable_expression();
					}
					break;
				}
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
				{
					{
						{
							switch ( LA(1) )
							{
							case IF:
							case UNLESS:
							case WHILE:
							{
								modifier=stmt_modifier();
								break;
							}
							case EOS:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						eos();
					}
					break;
				}
				default:
					if ((tokenSet_44_.member(LA(1))) && (tokenSet_65_.member(LA(2))))
					{
						{
							e=array_or_expression();
							{
								switch ( LA(1) )
								{
								case DEF:
								case DO:
								{
									method_invocation_block(e);
									break;
								}
								case IF:
								case UNLESS:
								case WHILE:
								case EOS:
								{
									{
										{
											switch ( LA(1) )
											{
											case IF:
											case UNLESS:
											case WHILE:
											{
												modifier=stmt_modifier();
												break;
											}
											case EOS:
											{
												break;
											}
											default:
											{
												throw new NoViableAltException(LT(1), getFilename());
											}
											 }
										}
										eos();
									}
									break;
								}
								default:
								{
									throw new NoViableAltException(LT(1), getFilename());
								}
								 }
							}
						}
					}
					else {
						bool synPredMatched270 = false;
						if (((LA(1)==AST) && (LA(2)==COLON||LA(2)==LBRACE)))
						{
							int _m270 = mark();
							synPredMatched270 = true;
							inputState.guessing++;
							try {
								{
									match(AST);
								}
							}
							catch (RecognitionException)
							{
								synPredMatched270 = false;
							}
							rewind(_m270);
							inputState.guessing--;
						}
						if ( synPredMatched270 )
						{
							{
								e=ast_literal();
							}
						}
					else
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					}break; }
				}
				if (0==inputState.guessing)
				{
					
							s = new ReturnStatement(ToLexicalInfo(r));
							s.Modifier = modifier;
							s.Expression = e;
						
				}
			}
			catch (RecognitionException ex)
			{
				if (0 == inputState.guessing)
				{
					reportError(ex);
					recover(ex,tokenSet_58_);
				}
				else
				{
					throw ex;
				}
			}
			return s;
		}
		
	protected Declaration  declaration() //throws RecognitionException, TokenStreamException
{
		Declaration d;
		
		IToken  id = null;
		
				d = null;
				TypeReference tr = null;
			
		
		try {      // for error handling
			id = LT(1);
			match(ID);
			{
				switch ( LA(1) )
				{
				case AS:
				{
					match(AS);
					tr=type_reference();
					break;
				}
				case IN:
				case ASSIGN:
				case COMMA:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						d = new Declaration(ToLexicalInfo(id));
						d.Name = id.getText();
						d.Type = tr;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_66_);
			}
			else
			{
				throw ex;
			}
		}
		return d;
	}
	
	protected UnpackStatement  unpack_stmt() //throws RecognitionException, TokenStreamException
{
		UnpackStatement s;
		
		
				s = null;
				StatementModifier m = null;
			
		
		try {      // for error handling
			s=unpack();
			{
				switch ( LA(1) )
				{
				case IF:
				case UNLESS:
				case WHILE:
				{
					m=stmt_modifier();
					break;
				}
				case EOS:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			eos();
			if (0==inputState.guessing)
			{
				
						s.Modifier = m;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_58_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
	}
	
	protected DeclarationStatement  declaration_stmt() //throws RecognitionException, TokenStreamException
{
		DeclarationStatement s;
		
		IToken  id = null;
		
				s = null;
				TypeReference tr = null;
				Expression initializer = null;
				StatementModifier m = null;
			
		
		try {      // for error handling
			id = LT(1);
			match(ID);
			match(AS);
			tr=type_reference();
			{
				switch ( LA(1) )
				{
				case ASSIGN:
				{
					{
						match(ASSIGN);
						initializer=declaration_initializer();
					}
					break;
				}
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
				{
					{
						{
							switch ( LA(1) )
							{
							case IF:
							case UNLESS:
							case WHILE:
							{
								m=stmt_modifier();
								break;
							}
							case EOS:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						eos();
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						Declaration d = new Declaration(ToLexicalInfo(id));
						d.Name = id.getText();
						d.Type = tr;
						
						s = new DeclarationStatement(d.LexicalInfo);
						s.Declaration = d;
						s.Initializer = initializer;
						s.Modifier = m;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_58_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
	}
	
	protected YieldStatement  yield_stmt() //throws RecognitionException, TokenStreamException
{
		YieldStatement s;
		
		IToken  yt = null;
		
				s = null;
				Expression e = null;
			
		
		try {      // for error handling
			yt = LT(1);
			match(YIELD);
			{
				switch ( LA(1) )
				{
				case ESEPARATOR:
				case AST:
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case SUBTRACT:
				case LBRACK:
				case COMMA:
				case LBRACE:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					e=array_or_expression();
					break;
				}
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
				case RBRACE:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						s = new YieldStatement(ToLexicalInfo(yt));
						s.Expression = e;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_62_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
	}
	
	protected BreakStatement  break_stmt() //throws RecognitionException, TokenStreamException
{
		BreakStatement s;
		
		IToken  b = null;
		s = null;
		
		try {      // for error handling
			b = LT(1);
			match(BREAK);
			if (0==inputState.guessing)
			{
				s = new BreakStatement(ToLexicalInfo(b));
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_64_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
	}
	
	protected Statement  continue_stmt() //throws RecognitionException, TokenStreamException
{
		Statement s;
		
		IToken  c = null;
		s = null;
		
		try {      // for error handling
			c = LT(1);
			match(CONTINUE);
			if (0==inputState.guessing)
			{
				s = new ContinueStatement(ToLexicalInfo(c));
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_64_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
	}
	
	protected RaiseStatement  raise_stmt() //throws RecognitionException, TokenStreamException
{
		RaiseStatement s;
		
		IToken  t = null;
		
				s = null;
				Expression e = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(RAISE);
			{
				switch ( LA(1) )
				{
				case ESEPARATOR:
				case AST:
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case SUBTRACT:
				case LBRACK:
				case LBRACE:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					e=expression();
					break;
				}
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
				case RBRACE:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						s = new RaiseStatement(ToLexicalInfo(t));
						s.Exception = e;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_62_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
	}
	
	protected RetryStatement  retry_stmt() //throws RecognitionException, TokenStreamException
{
		RetryStatement rs;
		
		IToken  t = null;
		
				rs = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(RETRY);
			if (0==inputState.guessing)
			{
				rs = new RetryStatement(ToLexicalInfo(t));
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_64_);
			}
			else
			{
				throw ex;
			}
		}
		return rs;
	}
	
	protected ExpressionStatement  expression_stmt() //throws RecognitionException, TokenStreamException
{
		ExpressionStatement s;
		
		
				s = null;
				Expression e = null;
			
		
		try {      // for error handling
			e=assignment_expression();
			if (0==inputState.guessing)
			{
				
						s = new ExpressionStatement(e);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_62_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
	}
	
	protected Expression  boolean_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  ot = null;
		
				e = null;
				Expression r = null;
			
		
		try {      // for error handling
			{
				e=boolean_term();
				{    // ( ... )*
					for (;;)
					{
						if ((LA(1)==OR) && (tokenSet_49_.member(LA(2))))
						{
							ot = LT(1);
							match(OR);
							r=expression();
							if (0==inputState.guessing)
							{
								
												BinaryExpression be = new BinaryExpression(ToLexicalInfo(ot));
												be.Operator = BinaryOperatorType.Or;
												be.Left = e;
												be.Right = r;
												e = be;
											
							}
						}
						else
						{
							goto _loop315_breakloop;
						}
						
					}
_loop315_breakloop:					;
				}    // ( ... )*
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_51_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  callable_or_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		
				e = null;
			
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case DEF:
			case DO:
			{
				e=callable_expression();
				break;
			}
			case ESEPARATOR:
			case AST:
			case CAST:
			case CHAR:
			case FALSE:
			case NOT:
			case NULL:
			case SELF:
			case SUPER:
			case TRUE:
			case TYPEOF:
			case TRIPLE_QUOTED_STRING:
			case DOUBLE_QUOTED_STRING:
			case SINGLE_QUOTED_STRING:
			case ID:
			case LPAREN:
			case SUBTRACT:
			case LBRACK:
			case COMMA:
			case LBRACE:
			case INCREMENT:
			case DECREMENT:
			case ONES_COMPLEMENT:
			case INT:
			case LONG:
			case RE_LITERAL:
			case DOUBLE:
			case FLOAT:
			case TIMESPAN:
			{
				e=array_or_expression();
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_3_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected void closure_parameters_test() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case REF:
				{
					parameter_modifier();
					break;
				}
				case ID:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{
				match(ID);
				{
					switch ( LA(1) )
					{
					case AS:
					{
						match(AS);
						type_reference();
						break;
					}
					case COMMA:
					case BITWISE_OR:
					{
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						match(ID);
						{
							switch ( LA(1) )
							{
							case AS:
							{
								match(AS);
								type_reference();
								break;
							}
							case COMMA:
							case BITWISE_OR:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
					}
					else
					{
						goto _loop217_breakloop;
					}
					
				}
_loop217_breakloop:				;
			}    // ( ... )*
			match(BITWISE_OR);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_3_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void internal_closure_stmt(
		Block block
	) //throws RecognitionException, TokenStreamException
{
		
		
				Statement stmt = null;
				StatementModifier modifier = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case RETURN:
				{
					stmt=return_expression_stmt();
					break;
				}
				case ESEPARATOR:
				case AST:
				case CAST:
				case CHAR:
				case FALSE:
				case NULL:
				case RAISE:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case YIELD:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case SUBTRACT:
				case LBRACK:
				case LBRACE:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					{
						{
							switch ( LA(1) )
							{
							case RAISE:
							{
								stmt=raise_stmt();
								break;
							}
							case YIELD:
							{
								stmt=yield_stmt();
								break;
							}
							default:
								bool synPredMatched223 = false;
								if (((LA(1)==ID) && (LA(2)==AS||LA(2)==COMMA)))
								{
									int _m223 = mark();
									synPredMatched223 = true;
									inputState.guessing++;
									try {
										{
											declaration();
											match(COMMA);
										}
									}
									catch (RecognitionException)
									{
										synPredMatched223 = false;
									}
									rewind(_m223);
									inputState.guessing--;
								}
								if ( synPredMatched223 )
								{
									stmt=unpack();
								}
								else if (((LA(1)==ID) && (tokenSet_67_.member(LA(2))))&&(IsValidMacroArgument(LA(2)))) {
									stmt=closure_macro_stmt();
								}
								else if ((tokenSet_68_.member(LA(1))) && (tokenSet_57_.member(LA(2)))) {
									stmt=expression_stmt();
								}
							else
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							break; }
						}
						{
							switch ( LA(1) )
							{
							case IF:
							case UNLESS:
							case WHILE:
							{
								modifier=stmt_modifier();
								if (0==inputState.guessing)
								{
									stmt.Modifier = modifier;
								}
								break;
							}
							case EOS:
							case RBRACE:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						if (null != stmt)
						{
							block.Add(stmt);
						}
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_69_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected ReturnStatement  return_expression_stmt() //throws RecognitionException, TokenStreamException
{
		ReturnStatement s;
		
		IToken  r = null;
		
				s = null;
				Expression e = null;
				StatementModifier modifier = null;
			
		
		try {      // for error handling
			r = LT(1);
			match(RETURN);
			{
				switch ( LA(1) )
				{
				case ESEPARATOR:
				case AST:
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case SUBTRACT:
				case LBRACK:
				case COMMA:
				case LBRACE:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					e=array_or_expression();
					break;
				}
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
				case RBRACE:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{
				switch ( LA(1) )
				{
				case IF:
				case UNLESS:
				case WHILE:
				{
					modifier=stmt_modifier();
					break;
				}
				case EOS:
				case RBRACE:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						s = new ReturnStatement(ToLexicalInfo(r));
						s.Modifier = modifier;
						s.Expression = e;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_69_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
	}
	
	protected UnpackStatement  unpack() //throws RecognitionException, TokenStreamException
{
		UnpackStatement s;
		
		IToken  t = null;
		
			Declaration d = null;
			s = new UnpackStatement();
			Expression e = null;
		
		
		try {      // for error handling
			d=declaration();
			match(COMMA);
			if (0==inputState.guessing)
			{
				s.Declarations.Add(d);
			}
			{
				switch ( LA(1) )
				{
				case ID:
				{
					declaration_list(s.Declarations);
					break;
				}
				case ASSIGN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			t = LT(1);
			match(ASSIGN);
			e=array_or_expression();
			if (0==inputState.guessing)
			{
				
						s.Expression = e;
						s.LexicalInfo = ToLexicalInfo(t);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_62_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
	}
	
	protected Expression  closure_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  anchorBegin = null;
		IToken  anchorEnd = null;
		
				e = null;
				CallableBlockExpression cbe = null;
				ParameterDeclarationCollection parameters = null;
				Block body = null;
			
		
		try {      // for error handling
			anchorBegin = LT(1);
			match(LBRACE);
			if (0==inputState.guessing)
			{
				
							e = cbe = new CallableBlockExpression(ToLexicalInfo(anchorBegin));
							parameters = cbe.Parameters;
							body = cbe.Body;
						
			}
			{
				bool synPredMatched228 = false;
				if (((tokenSet_70_.member(LA(1))) && (tokenSet_71_.member(LA(2)))))
				{
					int _m228 = mark();
					synPredMatched228 = true;
					inputState.guessing++;
					try {
						{
							closure_parameters_test();
						}
					}
					catch (RecognitionException)
					{
						synPredMatched228 = false;
					}
					rewind(_m228);
					inputState.guessing--;
				}
				if ( synPredMatched228 )
				{
					{
						parameter_declaration_list(parameters);
						match(BITWISE_OR);
					}
				}
				else if ((tokenSet_72_.member(LA(1))) && (tokenSet_57_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			{
				internal_closure_stmt(body);
				{    // ( ... )*
					for (;;)
					{
						if ((LA(1)==EOS))
						{
							eos();
							{
								switch ( LA(1) )
								{
								case ESEPARATOR:
								case AST:
								case CAST:
								case CHAR:
								case FALSE:
								case NULL:
								case RAISE:
								case RETURN:
								case SELF:
								case SUPER:
								case TRUE:
								case TYPEOF:
								case YIELD:
								case TRIPLE_QUOTED_STRING:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case ID:
								case LPAREN:
								case SUBTRACT:
								case LBRACK:
								case LBRACE:
								case INCREMENT:
								case DECREMENT:
								case ONES_COMPLEMENT:
								case INT:
								case LONG:
								case RE_LITERAL:
								case DOUBLE:
								case FLOAT:
								case TIMESPAN:
								{
									internal_closure_stmt(body);
									break;
								}
								case EOS:
								case RBRACE:
								{
									break;
								}
								default:
								{
									throw new NoViableAltException(LT(1), getFilename());
								}
								 }
							}
						}
						else
						{
							goto _loop233_breakloop;
						}
						
					}
_loop233_breakloop:					;
				}    // ( ... )*
			}
			anchorEnd = LT(1);
			match(RBRACE);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected void exception_handler(
		TryStatement t
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  c = null;
		IToken  x = null;
		
				ExceptionHandler eh = null;		
				TypeReference tr = null;
			
		
		try {      // for error handling
			c = LT(1);
			match(EXCEPT);
			{
				switch ( LA(1) )
				{
				case ID:
				{
					x = LT(1);
					match(ID);
					{
						switch ( LA(1) )
						{
						case AS:
						{
							match(AS);
							tr=type_reference();
							break;
						}
						case COLON:
						{
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					break;
				}
				case COLON:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						eh = new ExceptionHandler(ToLexicalInfo(c));
						
						if (x != null)
						{
							eh.Declaration = new Declaration(ToLexicalInfo(x));
							eh.Declaration.Name = x.getText();		
							eh.Declaration.Type = tr;
						}
					
			}
			compound_stmt(eh.Block);
			if (0==inputState.guessing)
			{
				
						t.ExceptionHandlers.Add(eh);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_73_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected Expression  assignment_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  op = null;
		IToken  ipbo = null;
		IToken  ipba = null;
		IToken  ipsl = null;
		IToken  ipsr = null;
		
				e = null;
				Expression r=null;
				IToken token = null;
				BinaryOperatorType binaryOperator = BinaryOperatorType.None;
			
		
		try {      // for error handling
			e=conditional_expression();
			{
				if ((tokenSet_74_.member(LA(1))) && (tokenSet_68_.member(LA(2))))
				{
					{
						switch ( LA(1) )
						{
						case ASSIGN:
						{
							{
								op = LT(1);
								match(ASSIGN);
								if (0==inputState.guessing)
								{
									
														token = op;
														binaryOperator = ParseAssignOperator(op.getText());
													
								}
							}
							break;
						}
						case INPLACE_BITWISE_OR:
						{
							{
								ipbo = LT(1);
								match(INPLACE_BITWISE_OR);
								if (0==inputState.guessing)
								{
									
														token = ipbo;
														binaryOperator = BinaryOperatorType.InPlaceBitwiseOr;
													
								}
							}
							break;
						}
						case INPLACE_BITWISE_AND:
						{
							{
								ipba = LT(1);
								match(INPLACE_BITWISE_AND);
								if (0==inputState.guessing)
								{
									
														token = ipba;
														binaryOperator = BinaryOperatorType.InPlaceBitwiseAnd;
													
								}
							}
							break;
						}
						case INPLACE_SHIFT_LEFT:
						{
							{
								ipsl = LT(1);
								match(INPLACE_SHIFT_LEFT);
								if (0==inputState.guessing)
								{
									
														token = ipsl;
														binaryOperator = BinaryOperatorType.InPlaceShiftLeft;
													
								}
							}
							break;
						}
						case INPLACE_SHIFT_RIGHT:
						{
							{
								ipsr = LT(1);
								match(INPLACE_SHIFT_RIGHT);
								if (0==inputState.guessing)
								{
									
														token = ipsr;
														binaryOperator = BinaryOperatorType.InPlaceShiftRight;
													
								}
							}
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					r=assignment_expression();
					if (0==inputState.guessing)
					{
						
									BinaryExpression be = new BinaryExpression(ToLexicalInfo(token));
									be.Operator = binaryOperator;
									be.Left = e;
									be.Right = r;
									e = be;
								
					}
				}
				else if ((tokenSet_51_.member(LA(1))) && (tokenSet_52_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_51_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected void declaration_list(
		DeclarationCollection dc
	) //throws RecognitionException, TokenStreamException
{
		
		
				Declaration d = null;
			
		
		try {      // for error handling
			d=declaration();
			if (0==inputState.guessing)
			{
				dc.Add(d);
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						d=declaration();
						if (0==inputState.guessing)
						{
							dc.Add(d);
						}
					}
					else
					{
						goto _loop295_breakloop;
					}
					
				}
_loop295_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_75_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void generator_expression_body(
		GeneratorExpression ge
	) //throws RecognitionException, TokenStreamException
{
		
		
			StatementModifier filter = null;
			Expression iterator = null;
			DeclarationCollection declarations = null == ge ? null : ge.Declarations;
		
		
		try {      // for error handling
			declaration_list(declarations);
			match(IN);
			iterator=boolean_expression();
			if (0==inputState.guessing)
			{
				ge.Iterator = iterator;
			}
			{
				if ((LA(1)==IF||LA(1)==UNLESS||LA(1)==WHILE) && (tokenSet_49_.member(LA(2))))
				{
					filter=stmt_modifier();
					if (0==inputState.guessing)
					{
						ge.Filter = filter;
					}
				}
				else if ((tokenSet_51_.member(LA(1))) && (tokenSet_52_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_51_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected Expression  boolean_term() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  at = null;
		
				e = null;
				Expression r = null;
			
		
		try {      // for error handling
			e=not_expression();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==AND) && (tokenSet_49_.member(LA(2))))
					{
						at = LT(1);
						match(AND);
						r=expression();
						if (0==inputState.guessing)
						{
							
										BinaryExpression be = new BinaryExpression(ToLexicalInfo(at));
										be.Operator = BinaryOperatorType.And;
										be.Left = e;
										be.Right = r; 
										e = be;
									
						}
					}
					else
					{
						goto _loop318_breakloop;
					}
					
				}
_loop318_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_51_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  not_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  nt = null;
		
				e = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case NOT:
				{
					{
						nt = LT(1);
						match(NOT);
						e=not_expression();
					}
					break;
				}
				case ESEPARATOR:
				case AST:
				case CAST:
				case CHAR:
				case FALSE:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case SUBTRACT:
				case LBRACK:
				case LBRACE:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					e=assignment_expression();
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						if (nt != null)
						{
							UnaryExpression ue = new UnaryExpression(ToLexicalInfo(nt));
							ue.Operator = UnaryOperatorType.LogicalNot;
							ue.Operand = e;
							e = ue;
						}
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_51_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	public AstLiteralExpression  ast_literal_expression() //throws RecognitionException, TokenStreamException
{
		AstLiteralExpression e;
		
		IToken  t = null;
		
			e = null;
		
		
		try {      // for error handling
			t = LT(1);
			match(AST);
			if (0==inputState.guessing)
			{
				e = new AstLiteralExpression(ToLexicalInfo(t));
			}
			ast_literal_closure(e);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	public void ast_literal_closure(
		AstLiteralExpression e
	) //throws RecognitionException, TokenStreamException
{
		
		
			Node node = null;
		
		
		try {      // for error handling
			match(LBRACE);
			{
				bool synPredMatched338 = false;
				if (((tokenSet_49_.member(LA(1))) && (tokenSet_76_.member(LA(2)))))
				{
					int _m338 = mark();
					synPredMatched338 = true;
					inputState.guessing++;
					try {
						{
							expression();
						}
					}
					catch (RecognitionException)
					{
						synPredMatched338 = false;
					}
					rewind(_m338);
					inputState.guessing--;
				}
				if ( synPredMatched338 )
				{
					node=expression();
					if (0==inputState.guessing)
					{
						e.Node = node;
					}
				}
				else if ((tokenSet_77_.member(LA(1))) && (tokenSet_78_.member(LA(2)))) {
					node=ast_literal_closure_stmt();
					if (0==inputState.guessing)
					{
						e.Node = node;
					}
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			match(RBRACE);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void ast_literal_block(
		AstLiteralExpression e
	) //throws RecognitionException, TokenStreamException
{
		
		
			// TODO: either cache or construct these objects on demand
			TypeMemberCollection collection = new TypeMemberCollection();
			Block block = new Block();
			StatementCollection statements = block.Statements;
			Node node = null;
		
		
		try {      // for error handling
			begin();
			{
				bool synPredMatched330 = false;
				if (((tokenSet_16_.member(LA(1))) && (tokenSet_79_.member(LA(2)))))
				{
					int _m330 = mark();
					synPredMatched330 = true;
					inputState.guessing++;
					try {
						{
							type_definition_member_prediction();
						}
					}
					catch (RecognitionException)
					{
						synPredMatched330 = false;
					}
					rewind(_m330);
					inputState.guessing--;
				}
				if ( synPredMatched330 )
				{
					{
						type_definition_member(collection);
						if (0==inputState.guessing)
						{
							e.Node = collection[0];
						}
					}
				}
				else if ((tokenSet_8_.member(LA(1))) && (tokenSet_80_.member(LA(2)))) {
					{
						{ // ( ... )+
							int _cnt334=0;
							for (;;)
							{
								if ((tokenSet_8_.member(LA(1))))
								{
									stmt(statements);
								}
								else
								{
									if (_cnt334 >= 1) { goto _loop334_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
								}
								
								_cnt334++;
							}
_loop334_breakloop:							;
						}    // ( ... )+
						if (0==inputState.guessing)
						{
							e.Node = block.Statements.Count > 1 ? block : block.Statements[0];
						}
					}
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			end(e);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_46_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void type_definition_member_prediction() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			attributes();
			modifiers();
			{
				switch ( LA(1) )
				{
				case CLASS:
				{
					match(CLASS);
					break;
				}
				case INTERFACE:
				{
					match(INTERFACE);
					break;
				}
				case STRUCT:
				{
					match(STRUCT);
					break;
				}
				case DEF:
				{
					match(DEF);
					break;
				}
				case EVENT:
				{
					match(EVENT);
					break;
				}
				case ID:
				{
					{
						match(ID);
						{
							switch ( LA(1) )
							{
							case AS:
							{
								match(AS);
								break;
							}
							case ASSIGN:
							{
								match(ASSIGN);
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_3_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public Statement  ast_literal_closure_stmt() //throws RecognitionException, TokenStreamException
{
		Statement s;
		
		
			s = null;
			StatementModifier modifier;
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case RETURN:
			{
				s=return_expression_stmt();
				break;
			}
			case RAISE:
			case YIELD:
			case ID:
			{
				{
					{
						switch ( LA(1) )
						{
						case RAISE:
						{
							s=raise_stmt();
							break;
						}
						case YIELD:
						{
							s=yield_stmt();
							break;
						}
						default:
							bool synPredMatched343 = false;
							if (((LA(1)==ID) && (LA(2)==AS||LA(2)==COMMA)))
							{
								int _m343 = mark();
								synPredMatched343 = true;
								inputState.guessing++;
								try {
									{
										declaration();
										match(COMMA);
									}
								}
								catch (RecognitionException)
								{
									synPredMatched343 = false;
								}
								rewind(_m343);
								inputState.guessing--;
							}
							if ( synPredMatched343 )
							{
								s=unpack();
							}
							else if (((LA(1)==ID) && (tokenSet_81_.member(LA(2))))&&(IsValidMacroArgument(LA(2)))) {
								s=closure_macro_stmt();
							}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						break; }
					}
					{
						switch ( LA(1) )
						{
						case IF:
						case UNLESS:
						case WHILE:
						{
							modifier=stmt_modifier();
							if (0==inputState.guessing)
							{
								s.Modifier = modifier;
							}
							break;
						}
						case RBRACE:
						{
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_82_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
	}
	
	protected Expression  conditional_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  t = null;
		IToken  tgt = null;
		IToken  tlt = null;
		IToken  tnot = null;
		IToken  tis = null;
		IToken  tnint = null;
		IToken  tin = null;
		IToken  tisa = null;
		
				e = null;		
				Expression r = null;
				BinaryOperatorType op = BinaryOperatorType.None;
				IToken token = null;
				TypeReference tr = null;
			
		
		try {      // for error handling
			e=sum();
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_83_.member(LA(1))) && (tokenSet_84_.member(LA(2))))
					{
						{
							switch ( LA(1) )
							{
							case IS:
							case CMP_OPERATOR:
							case GREATER_THAN:
							case LESS_THAN:
							{
								{
									{
										switch ( LA(1) )
										{
										case CMP_OPERATOR:
										{
											{
												t = LT(1);
												match(CMP_OPERATOR);
												if (0==inputState.guessing)
												{
													op = ParseCmpOperator(t.getText()); token = t;
												}
											}
											break;
										}
										case GREATER_THAN:
										{
											{
												tgt = LT(1);
												match(GREATER_THAN);
												if (0==inputState.guessing)
												{
													op = BinaryOperatorType.GreaterThan; token = tgt;
												}
											}
											break;
										}
										case LESS_THAN:
										{
											{
												tlt = LT(1);
												match(LESS_THAN);
												if (0==inputState.guessing)
												{
													op = BinaryOperatorType.LessThan; token = tlt;
												}
											}
											break;
										}
										default:
											if ((LA(1)==IS) && (LA(2)==NOT))
											{
												{
													tnot = LT(1);
													match(IS);
													match(NOT);
													if (0==inputState.guessing)
													{
														op = BinaryOperatorType.ReferenceInequality; token = tnot;
													}
												}
											}
											else if ((LA(1)==IS) && (tokenSet_68_.member(LA(2)))) {
												{
													tis = LT(1);
													match(IS);
													if (0==inputState.guessing)
													{
														op = BinaryOperatorType.ReferenceEquality; token = tis;
													}
												}
											}
										else
										{
											throw new NoViableAltException(LT(1), getFilename());
										}
										break; }
									}
									r=sum();
								}
								break;
							}
							case IN:
							case NOT:
							{
								{
									{
										switch ( LA(1) )
										{
										case NOT:
										{
											{
												tnint = LT(1);
												match(NOT);
												match(IN);
												if (0==inputState.guessing)
												{
													op = BinaryOperatorType.NotMember; token = tnint;
												}
											}
											break;
										}
										case IN:
										{
											{
												tin = LT(1);
												match(IN);
												if (0==inputState.guessing)
												{
													op = BinaryOperatorType.Member; token = tin;
												}
											}
											break;
										}
										default:
										{
											throw new NoViableAltException(LT(1), getFilename());
										}
										 }
									}
									r=array_or_expression();
								}
								break;
							}
							case ISA:
							{
								{
									tisa = LT(1);
									match(ISA);
									tr=type_reference();
									if (0==inputState.guessing)
									{
										
													op = BinaryOperatorType.TypeTest;
													token = tisa;
													r = new TypeofExpression(tr.LexicalInfo, tr);
												
									}
								}
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						if (0==inputState.guessing)
						{
							
									BinaryExpression be = new BinaryExpression(ToLexicalInfo(token));
									be.Operator = op;
									be.Left = e;
									be.Right = r;
									e = be;
								
						}
					}
					else
					{
						goto _loop386_breakloop;
					}
					
				}
_loop386_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_51_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  sum() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  add = null;
		IToken  sub = null;
		IToken  bitor = null;
		IToken  eo = null;
		
				e = null;
				Expression r = null;
				IToken op = null;
				BinaryOperatorType bOperator = BinaryOperatorType.None;
			
		
		try {      // for error handling
			e=term();
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_85_.member(LA(1))))
					{
						{
							switch ( LA(1) )
							{
							case ADD:
							{
								add = LT(1);
								match(ADD);
								if (0==inputState.guessing)
								{
									op=add; bOperator = BinaryOperatorType.Addition;
								}
								break;
							}
							case SUBTRACT:
							{
								sub = LT(1);
								match(SUBTRACT);
								if (0==inputState.guessing)
								{
									op=sub; bOperator = BinaryOperatorType.Subtraction;
								}
								break;
							}
							case BITWISE_OR:
							{
								bitor = LT(1);
								match(BITWISE_OR);
								if (0==inputState.guessing)
								{
									op=bitor; bOperator = BinaryOperatorType.BitwiseOr;
								}
								break;
							}
							case EXCLUSIVE_OR:
							{
								eo = LT(1);
								match(EXCLUSIVE_OR);
								if (0==inputState.guessing)
								{
									op=eo; bOperator = BinaryOperatorType.ExclusiveOr;
								}
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						r=term();
						if (0==inputState.guessing)
						{
							
										BinaryExpression be = new BinaryExpression(ToLexicalInfo(op));
										be.Operator = bOperator;
										be.Left = e;
										be.Right = r;
										e = be;
									
						}
					}
					else
					{
						goto _loop390_breakloop;
					}
					
				}
_loop390_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_51_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  term() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  m = null;
		IToken  d = null;
		IToken  md = null;
		IToken  ba = null;
		
				e = null;
				Expression r = null;
				IToken token = null;
				BinaryOperatorType op = BinaryOperatorType.None; 
			
		
		try {      // for error handling
			e=factor();
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_86_.member(LA(1))))
					{
						{
							switch ( LA(1) )
							{
							case MULTIPLY:
							{
								m = LT(1);
								match(MULTIPLY);
								if (0==inputState.guessing)
								{
									op=BinaryOperatorType.Multiply; token=m;
								}
								break;
							}
							case DIVISION:
							{
								d = LT(1);
								match(DIVISION);
								if (0==inputState.guessing)
								{
									op=BinaryOperatorType.Division; token=d;
								}
								break;
							}
							case MODULUS:
							{
								md = LT(1);
								match(MODULUS);
								if (0==inputState.guessing)
								{
									op=BinaryOperatorType.Modulus; token=md;
								}
								break;
							}
							case BITWISE_AND:
							{
								ba = LT(1);
								match(BITWISE_AND);
								if (0==inputState.guessing)
								{
									op=BinaryOperatorType.BitwiseAnd; token=ba;
								}
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						r=factor();
						if (0==inputState.guessing)
						{
							
										BinaryExpression be = new BinaryExpression(ToLexicalInfo(token));
										be.Operator = op;
										be.Left = e;
										be.Right = r;
										e = be;
									
						}
					}
					else
					{
						goto _loop394_breakloop;
					}
					
				}
_loop394_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_87_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  factor() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  shl = null;
		IToken  shr = null;
		
				e = null;
				Expression r = null;
				IToken token = null;
				BinaryOperatorType op = BinaryOperatorType.None;
			
		
		try {      // for error handling
			e=exponentiation();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==SHIFT_LEFT||LA(1)==SHIFT_RIGHT))
					{
						{
							switch ( LA(1) )
							{
							case SHIFT_LEFT:
							{
								shl = LT(1);
								match(SHIFT_LEFT);
								if (0==inputState.guessing)
								{
									op=BinaryOperatorType.ShiftLeft; token = shl;
								}
								break;
							}
							case SHIFT_RIGHT:
							{
								shr = LT(1);
								match(SHIFT_RIGHT);
								if (0==inputState.guessing)
								{
									op=BinaryOperatorType.ShiftRight; token = shr;
								}
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						r=exponentiation();
						if (0==inputState.guessing)
						{
							
										BinaryExpression be = new BinaryExpression(ToLexicalInfo(token));
										be.Operator = op;
										be.Left = e;
										be.Right = r;
										e = be;
									
						}
					}
					else
					{
						goto _loop398_breakloop;
					}
					
				}
_loop398_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_88_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  exponentiation() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  t = null;
		IToken  token = null;
		
				e = null;
				Expression r = null;
				TypeReference tr = null;
			
		
		try {      // for error handling
			e=unary_expression();
			{
				switch ( LA(1) )
				{
				case AS:
				{
					t = LT(1);
					match(AS);
					tr=type_reference();
					if (0==inputState.guessing)
					{
						
									AsExpression ae = new AsExpression(ToLexicalInfo(t));
									ae.Target = e;
									ae.Type = tr;
									e = ae; 
								
					}
					break;
				}
				case EOF:
				case ESEPARATOR:
				case AND:
				case DEF:
				case DO:
				case FOR:
				case IS:
				case ISA:
				case IF:
				case IN:
				case NOT:
				case OR:
				case UNLESS:
				case WHILE:
				case EOS:
				case RPAREN:
				case ASSIGN:
				case SUBTRACT:
				case COMMA:
				case RBRACK:
				case COLON:
				case MULTIPLY:
				case BITWISE_OR:
				case RBRACE:
				case INPLACE_BITWISE_OR:
				case INPLACE_BITWISE_AND:
				case INPLACE_SHIFT_LEFT:
				case INPLACE_SHIFT_RIGHT:
				case CMP_OPERATOR:
				case GREATER_THAN:
				case LESS_THAN:
				case ADD:
				case EXCLUSIVE_OR:
				case DIVISION:
				case MODULUS:
				case BITWISE_AND:
				case SHIFT_LEFT:
				case SHIFT_RIGHT:
				case EXPONENTIATION:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==EXPONENTIATION) && (tokenSet_68_.member(LA(2))))
					{
						token = LT(1);
						match(EXPONENTIATION);
						r=exponentiation();
						if (0==inputState.guessing)
						{
							
										BinaryExpression be = new BinaryExpression(ToLexicalInfo(token));
										be.Operator = BinaryOperatorType.Exponentiation;
										be.Left = e;
										be.Right = r;
										e = be;
									
						}
					}
					else
					{
						goto _loop402_breakloop;
					}
					
				}
_loop402_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_89_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  unary_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  sub = null;
		IToken  inc = null;
		IToken  dec = null;
		IToken  oc = null;
		IToken  postinc = null;
		IToken  postdec = null;
		
					e = null;
					IToken op = null;
					UnaryOperatorType uOperator = UnaryOperatorType.None;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case SUBTRACT:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				{
					{
						{
							switch ( LA(1) )
							{
							case SUBTRACT:
							{
								sub = LT(1);
								match(SUBTRACT);
								if (0==inputState.guessing)
								{
									op = sub; uOperator = UnaryOperatorType.UnaryNegation;
								}
								break;
							}
							case INCREMENT:
							{
								inc = LT(1);
								match(INCREMENT);
								if (0==inputState.guessing)
								{
									op = inc; uOperator = UnaryOperatorType.Increment;
								}
								break;
							}
							case DECREMENT:
							{
								dec = LT(1);
								match(DECREMENT);
								if (0==inputState.guessing)
								{
									op = dec; uOperator = UnaryOperatorType.Decrement;
								}
								break;
							}
							case ONES_COMPLEMENT:
							{
								oc = LT(1);
								match(ONES_COMPLEMENT);
								if (0==inputState.guessing)
								{
									op = oc; uOperator = UnaryOperatorType.OnesComplement;
								}
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						e=unary_expression();
					}
					break;
				}
				case ESEPARATOR:
				case AST:
				case CAST:
				case CHAR:
				case FALSE:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case LBRACK:
				case LBRACE:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					{
						e=slicing_expression();
						{
							switch ( LA(1) )
							{
							case INCREMENT:
							{
								postinc = LT(1);
								match(INCREMENT);
								if (0==inputState.guessing)
								{
									op = postinc; uOperator = UnaryOperatorType.PostIncrement;
								}
								break;
							}
							case DECREMENT:
							{
								postdec = LT(1);
								match(DECREMENT);
								if (0==inputState.guessing)
								{
									op = postdec; uOperator = UnaryOperatorType.PostDecrement;
								}
								break;
							}
							case EOF:
							case ESEPARATOR:
							case AND:
							case AS:
							case DEF:
							case DO:
							case FOR:
							case IS:
							case ISA:
							case IF:
							case IN:
							case NOT:
							case OR:
							case UNLESS:
							case WHILE:
							case EOS:
							case RPAREN:
							case ASSIGN:
							case SUBTRACT:
							case COMMA:
							case RBRACK:
							case COLON:
							case MULTIPLY:
							case BITWISE_OR:
							case RBRACE:
							case INPLACE_BITWISE_OR:
							case INPLACE_BITWISE_AND:
							case INPLACE_SHIFT_LEFT:
							case INPLACE_SHIFT_RIGHT:
							case CMP_OPERATOR:
							case GREATER_THAN:
							case LESS_THAN:
							case ADD:
							case EXCLUSIVE_OR:
							case DIVISION:
							case MODULUS:
							case BITWISE_AND:
							case SHIFT_LEFT:
							case SHIFT_RIGHT:
							case EXPONENTIATION:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						if (null != op)
						{
							UnaryExpression ue = new UnaryExpression(ToLexicalInfo(op));
							ue.Operator = uOperator;
							ue.Operand = e;
							e = ue; 
						}
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_90_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  atom() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		
				e = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case ESEPARATOR:
				case AST:
				case FALSE:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case LBRACK:
				case LBRACE:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					e=literal();
					break;
				}
				case LPAREN:
				{
					e=paren_expression();
					break;
				}
				case CAST:
				{
					e=cast_expression();
					break;
				}
				case TYPEOF:
				{
					e=typeof_expression();
					break;
				}
				default:
					bool synPredMatched412 = false;
					if (((LA(1)==CHAR) && (LA(2)==LPAREN)))
					{
						int _m412 = mark();
						synPredMatched412 = true;
						inputState.guessing++;
						try {
							{
								match(CHAR);
								match(LPAREN);
							}
						}
						catch (RecognitionException)
						{
							synPredMatched412 = false;
						}
						rewind(_m412);
						inputState.guessing--;
					}
					if ( synPredMatched412 )
					{
						e=char_literal();
					}
					else if ((LA(1)==CHAR||LA(1)==ID) && (tokenSet_26_.member(LA(2)))) {
						e=reference_expression();
					}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				break; }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  literal() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		
				e = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case INT:
				case LONG:
				{
					e=integer_literal();
					break;
				}
				case ESEPARATOR:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				{
					e=string_literal();
					break;
				}
				case LBRACK:
				{
					e=list_literal();
					break;
				}
				case AST:
				{
					e=ast_literal_expression();
					break;
				}
				case RE_LITERAL:
				{
					e=re_literal();
					break;
				}
				case FALSE:
				case TRUE:
				{
					e=bool_literal();
					break;
				}
				case NULL:
				{
					e=null_literal();
					break;
				}
				case SELF:
				{
					e=self_literal();
					break;
				}
				case SUPER:
				{
					e=super_literal();
					break;
				}
				case DOUBLE:
				case FLOAT:
				{
					e=double_literal();
					break;
				}
				case TIMESPAN:
				{
					e=timespan_literal();
					break;
				}
				default:
					bool synPredMatched451 = false;
					if (((LA(1)==LBRACE) && (tokenSet_91_.member(LA(2)))))
					{
						int _m451 = mark();
						synPredMatched451 = true;
						inputState.guessing++;
						try {
							{
								hash_literal_test();
							}
						}
						catch (RecognitionException)
						{
							synPredMatched451 = false;
						}
						rewind(_m451);
						inputState.guessing--;
					}
					if ( synPredMatched451 )
					{
						e=hash_literal();
					}
					else if ((LA(1)==LBRACE) && (tokenSet_92_.member(LA(2)))) {
						e=closure_expression();
					}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				break; }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  char_literal() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  t = null;
		
			e = null;
		
		
		try {      // for error handling
			match(CHAR);
			match(LPAREN);
			t = LT(1);
			match(SINGLE_QUOTED_STRING);
			match(RPAREN);
			if (0==inputState.guessing)
			{
				
						e = new CharLiteralExpression(ToLexicalInfo(t), t.getText());
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected ReferenceExpression  reference_expression() //throws RecognitionException, TokenStreamException
{
		ReferenceExpression e;
		
		IToken  id = null;
		IToken  ch = null;
		
			e = null;
			IToken t = null;
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case ID:
				{
					id = LT(1);
					match(ID);
					if (0==inputState.guessing)
					{
						t = id;
					}
					break;
				}
				case CHAR:
				{
					ch = LT(1);
					match(CHAR);
					if (0==inputState.guessing)
					{
						t = ch;
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						e = new ReferenceExpression(ToLexicalInfo(t));
						e.Name = t.getText();
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  paren_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		e = null;
		
		try {      // for error handling
			match(LPAREN);
			e=array_or_expression();
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  cast_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  t = null;
		
				e = null;
				TypeReference tr = null;
				Expression target = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(CAST);
			match(LPAREN);
			tr=type_reference();
			match(COMMA);
			target=expression();
			match(RPAREN);
			if (0==inputState.guessing)
			{
				
						e = new CastExpression(ToLexicalInfo(t), tr, target);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  typeof_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  t = null;
		
				e = null;
				TypeReference tr = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(TYPEOF);
			match(LPAREN);
			tr=type_reference();
			match(RPAREN);
			if (0==inputState.guessing)
			{
				
						e = new TypeofExpression(ToLexicalInfo(t), tr);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  array() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  t = null;
		
				ArrayLiteralExpression tle = null;
				e = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(LPAREN);
			e=expression();
			{
				switch ( LA(1) )
				{
				case COMMA:
				{
					match(COMMA);
					if (0==inputState.guessing)
					{
						
									tle = new ArrayLiteralExpression(ToLexicalInfo(t));
									tle.Items.Add(e);
								
					}
					{
						switch ( LA(1) )
						{
						case ESEPARATOR:
						case AST:
						case CAST:
						case CHAR:
						case FALSE:
						case NOT:
						case NULL:
						case SELF:
						case SUPER:
						case TRUE:
						case TYPEOF:
						case TRIPLE_QUOTED_STRING:
						case DOUBLE_QUOTED_STRING:
						case SINGLE_QUOTED_STRING:
						case ID:
						case LPAREN:
						case SUBTRACT:
						case LBRACK:
						case LBRACE:
						case INCREMENT:
						case DECREMENT:
						case ONES_COMPLEMENT:
						case INT:
						case LONG:
						case RE_LITERAL:
						case DOUBLE:
						case FLOAT:
						case TIMESPAN:
						{
							e=expression();
							if (0==inputState.guessing)
							{
								tle.Items.Add(e);
							}
							{    // ( ... )*
								for (;;)
								{
									if ((LA(1)==COMMA))
									{
										match(COMMA);
										e=expression();
										if (0==inputState.guessing)
										{
											tle.Items.Add(e);
										}
									}
									else
									{
										goto _loop423_breakloop;
									}
									
								}
_loop423_breakloop:								;
							}    // ( ... )*
							break;
						}
						case RPAREN:
						{
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					if (0==inputState.guessing)
					{
						e = tle;
					}
					break;
				}
				case RPAREN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_3_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Statement  method_invocation_with_block() //throws RecognitionException, TokenStreamException
{
		Statement s;
		
		
				s = null;
				MethodInvocationExpression mie = null;
				Expression block = null;
			
		
		try {      // for error handling
			match(RPAREN);
			{
				switch ( LA(1) )
				{
				case DEF:
				case DO:
				{
					block=callable_expression();
					if (0==inputState.guessing)
					{
						
									mie.Arguments.Add(block);
								
					}
					break;
				}
				case EOF:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_3_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
	}
	
	protected IToken  member() //throws RecognitionException, TokenStreamException
{
		IToken name;
		
		IToken  id = null;
		IToken  set = null;
		IToken  get = null;
		IToken  t1 = null;
		IToken  t2 = null;
		IToken  t3 = null;
		IToken  r = null;
		
				name = null;
			
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case ID:
			{
				id = LT(1);
				match(ID);
				if (0==inputState.guessing)
				{
					name=id;
				}
				break;
			}
			case SET:
			{
				set = LT(1);
				match(SET);
				if (0==inputState.guessing)
				{
					name=set;
				}
				break;
			}
			case GET:
			{
				get = LT(1);
				match(GET);
				if (0==inputState.guessing)
				{
					name=get;
				}
				break;
			}
			case INTERNAL:
			{
				t1 = LT(1);
				match(INTERNAL);
				if (0==inputState.guessing)
				{
					name=t1;
				}
				break;
			}
			case PUBLIC:
			{
				t2 = LT(1);
				match(PUBLIC);
				if (0==inputState.guessing)
				{
					name=t2;
				}
				break;
			}
			case PROTECTED:
			{
				t3 = LT(1);
				match(PROTECTED);
				if (0==inputState.guessing)
				{
					name=t3;
				}
				break;
			}
			case REF:
			{
				r = LT(1);
				match(REF);
				if (0==inputState.guessing)
				{
					name=r;
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_93_);
			}
			else
			{
				throw ex;
			}
		}
		return name;
	}
	
	protected void slice(
		SlicingExpression se
	) //throws RecognitionException, TokenStreamException
{
		
		
				Expression begin = null;
				Expression end = null;
				Expression step = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case COLON:
				{
					{
						match(COLON);
						if (0==inputState.guessing)
						{
							begin = OmittedExpression.Default;
						}
						{
							switch ( LA(1) )
							{
							case ESEPARATOR:
							case AST:
							case CAST:
							case CHAR:
							case FALSE:
							case NOT:
							case NULL:
							case SELF:
							case SUPER:
							case TRUE:
							case TYPEOF:
							case TRIPLE_QUOTED_STRING:
							case DOUBLE_QUOTED_STRING:
							case SINGLE_QUOTED_STRING:
							case ID:
							case LPAREN:
							case SUBTRACT:
							case LBRACK:
							case LBRACE:
							case INCREMENT:
							case DECREMENT:
							case ONES_COMPLEMENT:
							case INT:
							case LONG:
							case RE_LITERAL:
							case DOUBLE:
							case FLOAT:
							case TIMESPAN:
							{
								end=expression();
								break;
							}
							case COLON:
							{
								{
									match(COLON);
									if (0==inputState.guessing)
									{
										end = OmittedExpression.Default;
									}
									step=expression();
								}
								break;
							}
							case COMMA:
							case RBRACK:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
					}
					break;
				}
				case ESEPARATOR:
				case AST:
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case SUBTRACT:
				case LBRACK:
				case LBRACE:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					begin=expression();
					{
						switch ( LA(1) )
						{
						case COLON:
						{
							match(COLON);
							{
								switch ( LA(1) )
								{
								case ESEPARATOR:
								case AST:
								case CAST:
								case CHAR:
								case FALSE:
								case NOT:
								case NULL:
								case SELF:
								case SUPER:
								case TRUE:
								case TYPEOF:
								case TRIPLE_QUOTED_STRING:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case ID:
								case LPAREN:
								case SUBTRACT:
								case LBRACK:
								case LBRACE:
								case INCREMENT:
								case DECREMENT:
								case ONES_COMPLEMENT:
								case INT:
								case LONG:
								case RE_LITERAL:
								case DOUBLE:
								case FLOAT:
								case TIMESPAN:
								{
									end=expression();
									break;
								}
								case COMMA:
								case RBRACK:
								case COLON:
								{
									if (0==inputState.guessing)
									{
										end = OmittedExpression.Default;
									}
									break;
								}
								default:
								{
									throw new NoViableAltException(LT(1), getFilename());
								}
								 }
							}
							{
								switch ( LA(1) )
								{
								case COLON:
								{
									match(COLON);
									step=expression();
									break;
								}
								case COMMA:
								case RBRACK:
								{
									break;
								}
								default:
								{
									throw new NoViableAltException(LT(1), getFilename());
								}
								 }
							}
							break;
						}
						case COMMA:
						case RBRACK:
						{
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
					
						se.Indices.Add(new Slice(begin, end, step));
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_27_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void method_invocation_argument(
		MethodInvocationExpression mie
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  t = null;
		
				Expression arg = null;
			
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case MULTIPLY:
			{
				{
					t = LT(1);
					match(MULTIPLY);
					arg=expression();
					if (0==inputState.guessing)
					{
						
									if (null != arg)
									{
										mie.Arguments.Add(
											new UnaryExpression(
												ToLexicalInfo(t),
												UnaryOperatorType.Explode,
												arg));
									}
								
					}
				}
				break;
			}
			case ESEPARATOR:
			case AST:
			case CAST:
			case CHAR:
			case FALSE:
			case NOT:
			case NULL:
			case SELF:
			case SUPER:
			case TRUE:
			case TYPEOF:
			case TRIPLE_QUOTED_STRING:
			case DOUBLE_QUOTED_STRING:
			case SINGLE_QUOTED_STRING:
			case ID:
			case LPAREN:
			case SUBTRACT:
			case LBRACK:
			case LBRACE:
			case INCREMENT:
			case DECREMENT:
			case ONES_COMPLEMENT:
			case INT:
			case LONG:
			case RE_LITERAL:
			case DOUBLE:
			case FLOAT:
			case TIMESPAN:
			{
				argument(mie);
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_61_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void argument(
		INodeWithArguments node
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		IToken  colon = null;
				
				Expression value = null;
			
		
		try {      // for error handling
			bool synPredMatched489 = false;
			if (((LA(1)==ID) && (LA(2)==COLON)))
			{
				int _m489 = mark();
				synPredMatched489 = true;
				inputState.guessing++;
				try {
					{
						match(ID);
						match(COLON);
					}
				}
				catch (RecognitionException)
				{
					synPredMatched489 = false;
				}
				rewind(_m489);
				inputState.guessing--;
			}
			if ( synPredMatched489 )
			{
				{
					id = LT(1);
					match(ID);
					colon = LT(1);
					match(COLON);
					value=expression();
					if (0==inputState.guessing)
					{
						
									node.NamedArguments.Add(
										new ExpressionPair(
											ToLexicalInfo(colon),
											new ReferenceExpression(ToLexicalInfo(id), id.getText()),
											value));
								
					}
				}
			}
			else if ((tokenSet_49_.member(LA(1))) && (tokenSet_94_.member(LA(2)))) {
				{
					value=expression();
					if (0==inputState.guessing)
					{
						if (null != value) { node.Arguments.Add(value); }
					}
				}
			}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_61_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected Expression  string_literal() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  dqs = null;
		IToken  sqs = null;
		IToken  tqs = null;
		
				e = null;
			
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case ESEPARATOR:
			{
				e=expression_interpolation();
				break;
			}
			case DOUBLE_QUOTED_STRING:
			{
				dqs = LT(1);
				match(DOUBLE_QUOTED_STRING);
				if (0==inputState.guessing)
				{
					
							e = new StringLiteralExpression(ToLexicalInfo(dqs), dqs.getText());
						
				}
				break;
			}
			case SINGLE_QUOTED_STRING:
			{
				sqs = LT(1);
				match(SINGLE_QUOTED_STRING);
				if (0==inputState.guessing)
				{
					
							e = new StringLiteralExpression(ToLexicalInfo(sqs), sqs.getText());
						
				}
				break;
			}
			case TRIPLE_QUOTED_STRING:
			{
				tqs = LT(1);
				match(TRIPLE_QUOTED_STRING);
				if (0==inputState.guessing)
				{
					
							e = new StringLiteralExpression(ToLexicalInfo(tqs), tqs.getText());
						
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Expression  list_literal() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  lbrack = null;
		
				e = null;
				ListLiteralExpression lle = null;
				Expression item = null;
			
		
		try {      // for error handling
			lbrack = LT(1);
			match(LBRACK);
			{
				switch ( LA(1) )
				{
				case ESEPARATOR:
				case AST:
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case SUBTRACT:
				case LBRACK:
				case LBRACE:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					{
						item=expression();
						{
							if (0==inputState.guessing)
							{
								
													e = lle = new ListLiteralExpression(ToLexicalInfo(lbrack));
													lle.Items.Add(item);
												
							}
							{    // ( ... )*
								for (;;)
								{
									if ((LA(1)==COMMA) && (tokenSet_49_.member(LA(2))))
									{
										match(COMMA);
										item=expression();
										if (0==inputState.guessing)
										{
											lle.Items.Add(item);
										}
									}
									else
									{
										goto _loop466_breakloop;
									}
									
								}
_loop466_breakloop:								;
							}    // ( ... )*
						}
						{
							switch ( LA(1) )
							{
							case COMMA:
							{
								match(COMMA);
								break;
							}
							case RBRACK:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
					}
					break;
				}
				case RBRACK:
				{
					if (0==inputState.guessing)
					{
						e = new ListLiteralExpression(ToLexicalInfo(lbrack));
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(RBRACK);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected void hash_literal_test() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(LBRACE);
			{
				switch ( LA(1) )
				{
				case RBRACE:
				{
					match(RBRACE);
					break;
				}
				case ESEPARATOR:
				case AST:
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case SUBTRACT:
				case LBRACK:
				case LBRACE:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					{
						expression();
						match(COLON);
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_3_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected HashLiteralExpression  hash_literal() //throws RecognitionException, TokenStreamException
{
		HashLiteralExpression dle;
		
		IToken  lbrace = null;
		
				dle = null;
				ExpressionPair pair = null;
			
		
		try {      // for error handling
			lbrace = LT(1);
			match(LBRACE);
			if (0==inputState.guessing)
			{
				dle = new HashLiteralExpression(ToLexicalInfo(lbrace));
			}
			{
				switch ( LA(1) )
				{
				case ESEPARATOR:
				case AST:
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LPAREN:
				case SUBTRACT:
				case LBRACK:
				case LBRACE:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case LONG:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					pair=expression_pair();
					if (0==inputState.guessing)
					{
						dle.Items.Add(pair);
					}
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==COMMA))
							{
								match(COMMA);
								pair=expression_pair();
								if (0==inputState.guessing)
								{
									dle.Items.Add(pair);
								}
							}
							else
							{
								goto _loop474_breakloop;
							}
							
						}
_loop474_breakloop:						;
					}    // ( ... )*
					break;
				}
				case RBRACE:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(RBRACE);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return dle;
	}
	
	protected RELiteralExpression  re_literal() //throws RecognitionException, TokenStreamException
{
		RELiteralExpression re;
		
		IToken  value = null;
		re = null;
		
		try {      // for error handling
			value = LT(1);
			match(RE_LITERAL);
			if (0==inputState.guessing)
			{
				re = new RELiteralExpression(ToLexicalInfo(value), value.getText());
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return re;
	}
	
	protected BoolLiteralExpression  bool_literal() //throws RecognitionException, TokenStreamException
{
		BoolLiteralExpression e;
		
		IToken  t = null;
		IToken  f = null;
		e = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case TRUE:
			{
				t = LT(1);
				match(TRUE);
				if (0==inputState.guessing)
				{
					
							e = new BoolLiteralExpression(ToLexicalInfo(t));
							e.Value = true;
						
				}
				break;
			}
			case FALSE:
			{
				f = LT(1);
				match(FALSE);
				if (0==inputState.guessing)
				{
					
							e = new BoolLiteralExpression(ToLexicalInfo(f));
							e.Value = false;
						
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected NullLiteralExpression  null_literal() //throws RecognitionException, TokenStreamException
{
		NullLiteralExpression e;
		
		IToken  t = null;
		e = null;
		
		try {      // for error handling
			t = LT(1);
			match(NULL);
			if (0==inputState.guessing)
			{
				e = new NullLiteralExpression(ToLexicalInfo(t));
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected SelfLiteralExpression  self_literal() //throws RecognitionException, TokenStreamException
{
		SelfLiteralExpression e;
		
		IToken  t = null;
		e = null;
		
		try {      // for error handling
			t = LT(1);
			match(SELF);
			if (0==inputState.guessing)
			{
				e = new SelfLiteralExpression(ToLexicalInfo(t));
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected SuperLiteralExpression  super_literal() //throws RecognitionException, TokenStreamException
{
		SuperLiteralExpression e;
		
		IToken  t = null;
		e = null;
		
		try {      // for error handling
			t = LT(1);
			match(SUPER);
			if (0==inputState.guessing)
			{
				e = new SuperLiteralExpression(ToLexicalInfo(t));
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected DoubleLiteralExpression  double_literal() //throws RecognitionException, TokenStreamException
{
		DoubleLiteralExpression rle;
		
		IToken  value = null;
		IToken  single = null;
		rle = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case DOUBLE:
			{
				value = LT(1);
				match(DOUBLE);
				if (0==inputState.guessing)
				{
					
							rle = new DoubleLiteralExpression(ToLexicalInfo(value), ParseDouble(value.getText())); 
						
				}
				break;
			}
			case FLOAT:
			{
				single = LT(1);
				match(FLOAT);
				if (0==inputState.guessing)
				{
					
							string s = single.getText();
							s = s.Substring(0, s.Length-1);
							double val = float.Parse(s, CultureInfo.InvariantCulture);
							rle = new DoubleLiteralExpression(ToLexicalInfo(single), val, true);
						
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return rle;
	}
	
	protected TimeSpanLiteralExpression  timespan_literal() //throws RecognitionException, TokenStreamException
{
		TimeSpanLiteralExpression tsle;
		
		IToken  value = null;
		tsle = null;
		
		try {      // for error handling
			value = LT(1);
			match(TIMESPAN);
			if (0==inputState.guessing)
			{
				tsle = new TimeSpanLiteralExpression(ToLexicalInfo(value), ParseTimeSpan(value.getText()));
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return tsle;
	}
	
	protected ExpressionInterpolationExpression  expression_interpolation() //throws RecognitionException, TokenStreamException
{
		ExpressionInterpolationExpression e;
		
		IToken  separator = null;
		
				e = null;
				Expression param = null;	
			
		
		try {      // for error handling
			separator = LT(1);
			match(ESEPARATOR);
			if (0==inputState.guessing)
			{
				
						LexicalInfo info = ToLexicalInfo(separator);
						e = new ExpressionInterpolationExpression(info);		
					
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==ESEPARATOR) && (tokenSet_49_.member(LA(2))))
					{
						match(ESEPARATOR);
						param=expression();
						if (0==inputState.guessing)
						{
							if (null != param) { e.Expressions.Add(param); }
						}
						match(ESEPARATOR);
					}
					else
					{
						goto _loop460_breakloop;
					}
					
				}
_loop460_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected ExpressionPair  expression_pair() //throws RecognitionException, TokenStreamException
{
		ExpressionPair ep;
		
		IToken  t = null;
		
				ep = null;
				Expression key = null;
				Expression value = null;
			
		
		try {      // for error handling
			key=expression();
			t = LT(1);
			match(COLON);
			value=expression();
			if (0==inputState.guessing)
			{
				ep = new ExpressionPair(ToLexicalInfo(t), key, value);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_95_);
			}
			else
			{
				throw ex;
			}
		}
		return ep;
	}
	
	private void initializeFactory()
	{
	}
	
	public static readonly string[] tokenNames_ = new string[] {
		@"""<0>""",
		@"""EOF""",
		@"""<2>""",
		@"""NULL_TREE_LOOKAHEAD""",
		@"""ELIST""",
		@"""DLIST""",
		@"""ESEPARATOR""",
		@"""abstract""",
		@"""and""",
		@"""as""",
		@"""ast""",
		@"""break""",
		@"""continue""",
		@"""callable""",
		@"""cast""",
		@"""char""",
		@"""class""",
		@"""constructor""",
		@"""def""",
		@"""destructor""",
		@"""do""",
		@"""elif""",
		@"""else""",
		@"""end""",
		@"""ensure""",
		@"""enum""",
		@"""event""",
		@"""except""",
		@"""failure""",
		@"""final""",
		@"""from""",
		@"""for""",
		@"""false""",
		@"""get""",
		@"""given""",
		@"""goto""",
		@"""import""",
		@"""interface""",
		@"""internal""",
		@"""is""",
		@"""isa""",
		@"""if""",
		@"""in""",
		@"""not""",
		@"""null""",
		@"""or""",
		@"""otherwise""",
		@"""override""",
		@"""namespace""",
		@"""public""",
		@"""protected""",
		@"""private""",
		@"""raise""",
		@"""ref""",
		@"""return""",
		@"""retry""",
		@"""set""",
		@"""self""",
		@"""super""",
		@"""static""",
		@"""success""",
		@"""struct""",
		@"""try""",
		@"""transient""",
		@"""true""",
		@"""typeof""",
		@"""unless""",
		@"""virtual""",
		@"""when""",
		@"""while""",
		@"""yield""",
		@"""EOS""",
		@"""TRIPLE_QUOTED_STRING""",
		@"""DOUBLE_QUOTED_STRING""",
		@"""SINGLE_QUOTED_STRING""",
		@"""ID""",
		@"""LPAREN""",
		@"""RPAREN""",
		@"""ASSIGN""",
		@"""SUBTRACT""",
		@"""LBRACK""",
		@"""COMMA""",
		@"""RBRACK""",
		@"""ASSEMBLY_ATTRIBUTE_BEGIN""",
		@"""DOT""",
		@"""COLON""",
		@"""MULTIPLY""",
		@"""BITWISE_OR""",
		@"""LBRACE""",
		@"""RBRACE""",
		@"""INPLACE_BITWISE_OR""",
		@"""INPLACE_BITWISE_AND""",
		@"""INPLACE_SHIFT_LEFT""",
		@"""INPLACE_SHIFT_RIGHT""",
		@"""CMP_OPERATOR""",
		@"""GREATER_THAN""",
		@"""LESS_THAN""",
		@"""ADD""",
		@"""EXCLUSIVE_OR""",
		@"""DIVISION""",
		@"""MODULUS""",
		@"""BITWISE_AND""",
		@"""SHIFT_LEFT""",
		@"""SHIFT_RIGHT""",
		@"""EXPONENTIATION""",
		@"""INCREMENT""",
		@"""DECREMENT""",
		@"""ONES_COMPLEMENT""",
		@"""INT""",
		@"""LONG""",
		@"""RE_LITERAL""",
		@"""DOUBLE""",
		@"""FLOAT""",
		@"""TIMESPAN""",
		@"""LINE_CONTINUATION""",
		@"""SL_COMMENT""",
		@"""ML_COMMENT""",
		@"""WS""",
		@"""X_RE_LITERAL""",
		@"""NEWLINE""",
		@"""ESCAPED_EXPRESSION""",
		@"""DQS_ESC""",
		@"""SQS_ESC""",
		@"""SESC""",
		@"""RE_CHAR""",
		@"""X_RE_CHAR""",
		@"""RE_ESC""",
		@"""ID_LETTER""",
		@"""DIGIT""",
		@"""HEXDIGIT"""
	};
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = { -1234106704589357886L, 1123700903092207L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = { -6336986475287732096L, 65544L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { -6336986475287076736L, 264200L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { 2L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { -1162049101885997886L, 1123700903092207L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { -1153027059222577470L, 1125899906834415L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = { -1234388179566068542L, 1123700903092207L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = { -1234388248285545278L, 1123700903092207L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	private static long[] mk_tokenSet_8_()
	{
		long[] data = { 5102598227002186816L, 1123700902567783L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
	private static long[] mk_tokenSet_9_()
	{
		long[] data = { 2L, 524288L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
	private static long[] mk_tokenSet_10_()
	{
		long[] data = { 0L, 128L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
	private static long[] mk_tokenSet_11_()
	{
		long[] data = { -1162330576862708542L, 1123700936646639L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
	private static long[] mk_tokenSet_12_()
	{
		long[] data = { 52230024856386L, 2199004901540L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
	private static long[] mk_tokenSet_13_()
	{
		long[] data = { -6255921673338019712L, 6293512L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_13_ = new BitSet(mk_tokenSet_13_());
	private static long[] mk_tokenSet_14_()
	{
		long[] data = { 2377900749381509120L, 2048L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_14_ = new BitSet(mk_tokenSet_14_());
	private static long[] mk_tokenSet_15_()
	{
		long[] data = { -1234388248210047806L, 1123700903092207L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_15_ = new BitSet(mk_tokenSet_15_());
	private static long[] mk_tokenSet_16_()
	{
		long[] data = { -6336986475220623232L, 67592L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_16_ = new BitSet(mk_tokenSet_16_());
	private static long[] mk_tokenSet_17_()
	{
		long[] data = { 67371008L, 67584L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_17_ = new BitSet(mk_tokenSet_17_());
	private static long[] mk_tokenSet_18_()
	{
		long[] data = { 0L, 8396800L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_18_ = new BitSet(mk_tokenSet_18_());
	private static long[] mk_tokenSet_19_()
	{
		long[] data = { 52228951114050L, 2199005946020L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_19_ = new BitSet(mk_tokenSet_19_());
	private static long[] mk_tokenSet_20_()
	{
		long[] data = { 52228951114050L, 2199004897444L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_20_ = new BitSet(mk_tokenSet_20_());
	private static long[] mk_tokenSet_21_()
	{
		long[] data = { -6264928872592760704L, 67976L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_21_ = new BitSet(mk_tokenSet_21_());
	private static long[] mk_tokenSet_22_()
	{
		long[] data = { -6264928872592760704L, 67720L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_22_ = new BitSet(mk_tokenSet_22_());
	private static long[] mk_tokenSet_23_()
	{
		long[] data = { -6264928872592104832L, 3496072L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_23_ = new BitSet(mk_tokenSet_23_());
	private static long[] mk_tokenSet_24_()
	{
		long[] data = { 8388608L, 67584L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_24_ = new BitSet(mk_tokenSet_24_());
	private static long[] mk_tokenSet_25_()
	{
		long[] data = { -9338772073874238L, 1123700903092223L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_25_ = new BitSet(mk_tokenSet_25_());
	private static long[] mk_tokenSet_26_()
	{
		long[] data = { 52228951114562L, 8796075716772L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_26_ = new BitSet(mk_tokenSet_26_());
	private static long[] mk_tokenSet_27_()
	{
		long[] data = { 0L, 393216L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_27_ = new BitSet(mk_tokenSet_27_());
	private static long[] mk_tokenSet_28_()
	{
		long[] data = { 0L, 8192L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_28_ = new BitSet(mk_tokenSet_28_());
	private static long[] mk_tokenSet_29_()
	{
		long[] data = { 0L, 2097152L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_29_ = new BitSet(mk_tokenSet_29_());
	private static long[] mk_tokenSet_30_()
	{
		long[] data = { -6336986475212234624L, 67592L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_30_ = new BitSet(mk_tokenSet_30_());
	private static long[] mk_tokenSet_31_()
	{
		long[] data = { 512L, 3149824L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_31_ = new BitSet(mk_tokenSet_31_());
	private static long[] mk_tokenSet_32_()
	{
		long[] data = { -8570772019346407296L, 65544L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_32_ = new BitSet(mk_tokenSet_32_());
	private static long[] mk_tokenSet_33_()
	{
		long[] data = { 75759616L, 67584L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_33_ = new BitSet(mk_tokenSet_33_());
	private static long[] mk_tokenSet_34_()
	{
		long[] data = { 72057602703622144L, 67712L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_34_ = new BitSet(mk_tokenSet_34_());
	private static long[] mk_tokenSet_35_()
	{
		long[] data = { 72057602636251136L, 65536L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_35_ = new BitSet(mk_tokenSet_35_());
	private static long[] mk_tokenSet_36_()
	{
		long[] data = { -1234388248218436416L, 1123700902567935L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_36_ = new BitSet(mk_tokenSet_36_());
	private static long[] mk_tokenSet_37_()
	{
		long[] data = { 0L, 2048L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_37_ = new BitSet(mk_tokenSet_37_());
	private static long[] mk_tokenSet_38_()
	{
		long[] data = { 5102598227010575424L, 1123700902567911L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_38_ = new BitSet(mk_tokenSet_38_());
	private static long[] mk_tokenSet_39_()
	{
		long[] data = { -1225366205547282750L, 1125899906834415L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_39_ = new BitSet(mk_tokenSet_39_());
	private static long[] mk_tokenSet_40_()
	{
		long[] data = { 8388608L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_40_ = new BitSet(mk_tokenSet_40_());
	private static long[] mk_tokenSet_41_()
	{
		long[] data = { -8570772019338018688L, 65544L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_41_ = new BitSet(mk_tokenSet_41_());
	private static long[] mk_tokenSet_42_()
	{
		long[] data = { 432363160708629568L, 1108307737648899L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_42_ = new BitSet(mk_tokenSet_42_());
	private static long[] mk_tokenSet_43_()
	{
		long[] data = { 463897154194555968L, 1123700948049731L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_43_ = new BitSet(mk_tokenSet_43_());
	private static long[] mk_tokenSet_44_()
	{
		long[] data = { 432371956801651776L, 1123700900601603L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_44_ = new BitSet(mk_tokenSet_44_());
	private static long[] mk_tokenSet_45_()
	{
		long[] data = { 463938388026771264L, 1125899904212931L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_45_ = new BitSet(mk_tokenSet_45_());
	private static long[] mk_tokenSet_46_()
	{
		long[] data = { -1234388248210047806L, 1123700903092079L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_46_ = new BitSet(mk_tokenSet_46_());
	private static long[] mk_tokenSet_47_()
	{
		long[] data = { 52228951114562L, 8796074598564L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_47_ = new BitSet(mk_tokenSet_47_());
	private static long[] mk_tokenSet_48_()
	{
		long[] data = { 432415389659743554L, 1123709459365799L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_48_ = new BitSet(mk_tokenSet_48_());
	private static long[] mk_tokenSet_49_()
	{
		long[] data = { 432371956801651776L, 1123700900470531L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_49_ = new BitSet(mk_tokenSet_49_());
	private static long[] mk_tokenSet_50_()
	{
		long[] data = { 463940587051337538L, 1125899906318311L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_50_ = new BitSet(mk_tokenSet_50_());
	private static long[] mk_tokenSet_51_()
	{
		long[] data = { 52228951114050L, 8558895268L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_51_ = new BitSet(mk_tokenSet_51_());
	private static long[] mk_tokenSet_52_()
	{
		long[] data = { -1234338220429934654L, 1125899906842623L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_52_ = new BitSet(mk_tokenSet_52_());
	private static long[] mk_tokenSet_53_()
	{
		long[] data = { -9338772073874238L, 1123700903092095L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_53_ = new BitSet(mk_tokenSet_53_());
	private static long[] mk_tokenSet_54_()
	{
		long[] data = { 432374155824907328L, 1123700902567847L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_54_ = new BitSet(mk_tokenSet_54_());
	private static long[] mk_tokenSet_55_()
	{
		long[] data = { 463897154194555968L, 1123700948066115L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_55_ = new BitSet(mk_tokenSet_55_());
	private static long[] mk_tokenSet_56_()
	{
		long[] data = { 472895591714708544L, 1123700902567747L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_56_ = new BitSet(mk_tokenSet_56_());
	private static long[] mk_tokenSet_57_()
	{
		long[] data = { 463905400530454080L, 1125899904212967L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_57_ = new BitSet(mk_tokenSet_57_());
	private static long[] mk_tokenSet_58_()
	{
		long[] data = { 5102598227010575426L, 1123700903092071L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_58_ = new BitSet(mk_tokenSet_58_());
	private static long[] mk_tokenSet_59_()
	{
		long[] data = { 40960L, 6144L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_59_ = new BitSet(mk_tokenSet_59_());
	private static long[] mk_tokenSet_60_()
	{
		long[] data = { 0L, 8527872L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_60_ = new BitSet(mk_tokenSet_60_());
	private static long[] mk_tokenSet_61_()
	{
		long[] data = { 0L, 139264L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_61_ = new BitSet(mk_tokenSet_61_());
	private static long[] mk_tokenSet_62_()
	{
		long[] data = { 2199023255552L, 33554596L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_62_ = new BitSet(mk_tokenSet_62_());
	private static long[] mk_tokenSet_63_()
	{
		long[] data = { 2199023255552L, 35651748L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_63_ = new BitSet(mk_tokenSet_63_());
	private static long[] mk_tokenSet_64_()
	{
		long[] data = { 2199023255552L, 164L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_64_ = new BitSet(mk_tokenSet_64_());
	private static long[] mk_tokenSet_65_()
	{
		long[] data = { 463940587051337536L, 1125899904212967L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_65_ = new BitSet(mk_tokenSet_65_());
	private static long[] mk_tokenSet_66_()
	{
		long[] data = { 4398046511104L, 147456L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_66_ = new BitSet(mk_tokenSet_66_());
	private static long[] mk_tokenSet_67_()
	{
		long[] data = { 432374155824907328L, 1123700934025127L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_67_ = new BitSet(mk_tokenSet_67_());
	private static long[] mk_tokenSet_68_()
	{
		long[] data = { 432363160708629568L, 1123700900470531L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_68_ = new BitSet(mk_tokenSet_68_());
	private static long[] mk_tokenSet_69_()
	{
		long[] data = { 0L, 33554560L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_69_ = new BitSet(mk_tokenSet_69_());
	private static long[] mk_tokenSet_70_()
	{
		long[] data = { 9007199254740992L, 12650496L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_70_ = new BitSet(mk_tokenSet_70_());
	private static long[] mk_tokenSet_71_()
	{
		long[] data = { 454881158845482560L, 1123700909252419L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_71_ = new BitSet(mk_tokenSet_71_());
	private static long[] mk_tokenSet_72_()
	{
		long[] data = { 454881158845482048L, 1123700900470595L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_72_ = new BitSet(mk_tokenSet_72_());
	private static long[] mk_tokenSet_73_()
	{
		long[] data = { 6255519731768417346L, 1123700903092071L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_73_ = new BitSet(mk_tokenSet_73_());
	private static long[] mk_tokenSet_74_()
	{
		long[] data = { 0L, 1006649344L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_74_ = new BitSet(mk_tokenSet_74_());
	private static long[] mk_tokenSet_75_()
	{
		long[] data = { 4398046511104L, 16384L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_75_ = new BitSet(mk_tokenSet_75_());
	private static long[] mk_tokenSet_76_()
	{
		long[] data = { 463938388026771264L, 1125899904212803L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_76_ = new BitSet(mk_tokenSet_76_());
	private static long[] mk_tokenSet_77_()
	{
		long[] data = { 22517998136852480L, 2112L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_77_ = new BitSet(mk_tokenSet_77_());
	private static long[] mk_tokenSet_78_()
	{
		long[] data = { 432374155824907840L, 1123700934156071L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_78_ = new BitSet(mk_tokenSet_78_());
	private static long[] mk_tokenSet_79_()
	{
		long[] data = { -6336986475219967360L, 3430536L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_79_ = new BitSet(mk_tokenSet_79_());
	private static long[] mk_tokenSet_80_()
	{
		long[] data = { 463905400531764800L, 1125899906310119L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_80_ = new BitSet(mk_tokenSet_80_());
	private static long[] mk_tokenSet_81_()
	{
		long[] data = { 432374155824907328L, 1123700934024999L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_81_ = new BitSet(mk_tokenSet_81_());
	private static long[] mk_tokenSet_82_()
	{
		long[] data = { 0L, 33554432L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_82_ = new BitSet(mk_tokenSet_82_());
	private static long[] mk_tokenSet_83_()
	{
		long[] data = { 14843406974976L, 7516192768L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_83_ = new BitSet(mk_tokenSet_83_());
	private static long[] mk_tokenSet_84_()
	{
		long[] data = { 432376354848171072L, 1123700900601603L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_84_ = new BitSet(mk_tokenSet_84_());
	private static long[] mk_tokenSet_85_()
	{
		long[] data = { 0L, 25778225152L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_85_ = new BitSet(mk_tokenSet_85_());
	private static long[] mk_tokenSet_86_()
	{
		long[] data = { 0L, 240522362880L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_86_ = new BitSet(mk_tokenSet_86_());
	private static long[] mk_tokenSet_87_()
	{
		long[] data = { 52228951114050L, 34337120420L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_87_ = new BitSet(mk_tokenSet_87_());
	private static long[] mk_tokenSet_88_()
	{
		long[] data = { 52228951114050L, 274859483300L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_88_ = new BitSet(mk_tokenSet_88_());
	private static long[] mk_tokenSet_89_()
	{
		long[] data = { 52228951114050L, 2199004831908L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_89_ = new BitSet(mk_tokenSet_89_());
	private static long[] mk_tokenSet_90_()
	{
		long[] data = { 52228951114562L, 2199004831908L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_90_ = new BitSet(mk_tokenSet_90_());
	private static long[] mk_tokenSet_91_()
	{
		long[] data = { 432371956801651776L, 1123700934024963L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_91_ = new BitSet(mk_tokenSet_91_());
	private static long[] mk_tokenSet_92_()
	{
		long[] data = { 463888358100223040L, 1123700913053507L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_92_ = new BitSet(mk_tokenSet_92_());
	private static long[] mk_tokenSet_93_()
	{
		long[] data = { 52230024856386L, 8796075716772L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_93_ = new BitSet(mk_tokenSet_93_());
	private static long[] mk_tokenSet_94_()
	{
		long[] data = { 463938388026771264L, 1125899904220995L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_94_ = new BitSet(mk_tokenSet_94_());
	private static long[] mk_tokenSet_95_()
	{
		long[] data = { 0L, 33685504L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_95_ = new BitSet(mk_tokenSet_95_());
	
}
}
