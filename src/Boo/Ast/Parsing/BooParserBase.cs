// $ANTLR 2.7.2: "boo.g" -> "BooParserBase.cs"$

namespace Boo.Ast.Parsing
{
	// Generate the header common to all output files.
	using System;
	
	using TokenBuffer              = antlr.TokenBuffer;
	using TokenStreamException     = antlr.TokenStreamException;
	using TokenStreamIOException   = antlr.TokenStreamIOException;
	using ANTLRException           = antlr.ANTLRException;
	using LLkParser = antlr.LLkParser;
	using Token                    = antlr.Token;
	using TokenStream              = antlr.TokenStream;
	using RecognitionException     = antlr.RecognitionException;
	using NoViableAltException     = antlr.NoViableAltException;
	using MismatchedTokenException = antlr.MismatchedTokenException;
	using SemanticException        = antlr.SemanticException;
	using ParserSharedInputState   = antlr.ParserSharedInputState;
	using BitSet                   = antlr.collections.impl.BitSet;
	
using Boo.Ast;
using Boo.Ast.Parsing.Util;

public delegate void ParserErrorHandler(antlr.RecognitionException x);

	public 	class BooParserBase : antlr.LLkParser
	{
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int TIMESPAN = 4;
		public const int ESEPARATOR = 5;
		public const int INDENT = 6;
		public const int DEDENT = 7;
		public const int COMPILATION_UNIT = 8;
		public const int PARAMETERS = 9;
		public const int PARAMETER = 10;
		public const int ELIST = 11;
		public const int DLIST = 12;
		public const int TYPE = 13;
		public const int CALL = 14;
		public const int STMT = 15;
		public const int BLOCK = 16;
		public const int FIELD = 17;
		public const int MODIFIERS = 18;
		public const int MODULE = 19;
		public const int LITERAL = 20;
		public const int LIST_LITERAL = 21;
		public const int UNPACKING = 22;
		public const int AND = 23;
		public const int AS = 24;
		public const int BREAK = 25;
		public const int CONTINUE = 26;
		public const int CATCH = 27;
		public const int CLASS = 28;
		public const int CONSTRUCTOR = 29;
		public const int DEF = 30;
		public const int ELSE = 31;
		public const int ENSURE = 32;
		public const int ENUM = 33;
		public const int FAILURE = 34;
		public const int FINAL = 35;
		public const int FROM = 36;
		public const int FOR = 37;
		public const int FALSE = 38;
		public const int GET = 39;
		public const int GIVEN = 40;
		public const int INTERFACE = 41;
		public const int INTERNAL = 42;
		public const int IS = 43;
		public const int IF = 44;
		public const int IN = 45;
		public const int KINDOF = 46;
		public const int NOT = 47;
		public const int NULL = 48;
		public const int OR = 49;
		public const int OTHERWISE = 50;
		public const int PASS = 51;
		public const int PACKAGE = 52;
		public const int PROPERTY = 53;
		public const int PUBLIC = 54;
		public const int PROTECTED = 55;
		public const int PRIVATE = 56;
		public const int RAISE = 57;
		public const int RETURN = 58;
		public const int RETRY = 59;
		public const int SET = 60;
		public const int SELF = 61;
		public const int SUPER = 62;
		public const int STATIC = 63;
		public const int SUCCESS = 64;
		public const int TRY = 65;
		public const int TRANSIENT = 66;
		public const int TRUE = 67;
		public const int UNLESS = 68;
		public const int USING = 69;
		public const int UNTIL = 70;
		public const int VOID = 71;
		public const int WHEN = 72;
		public const int WHILE = 73;
		public const int YIELD = 74;
		public const int EOS = 75;
		public const int TRIPLE_QUOTED_STRING = 76;
		public const int ID = 77;
		public const int ASSIGN = 78;
		public const int LBRACK = 79;
		public const int COMMA = 80;
		public const int RBRACK = 81;
		public const int LPAREN = 82;
		public const int RPAREN = 83;
		public const int COLON = 84;
		public const int QMARK = 85;
		public const int SUM_OPERATOR = 86;
		public const int MULT_OPERATOR = 87;
		public const int INCREMENT = 88;
		public const int DECREMENT = 89;
		public const int CMP_OPERATOR = 90;
		public const int DOT = 91;
		public const int INT = 92;
		public const int DOUBLE_QUOTED_STRING = 93;
		public const int SINGLE_QUOTED_STRING = 94;
		public const int LBRACE = 95;
		public const int RBRACE = 96;
		public const int RE_LITERAL = 97;
		public const int SL_COMMENT = 98;
		public const int WS = 99;
		public const int ESCAPED_EXPRESSION = 100;
		public const int DQS_ESC = 101;
		public const int SQS_ESC = 102;
		public const int SESC = 103;
		public const int RE_CHAR = 104;
		public const int RE_ESC = 105;
		public const int ID_LETTER = 106;
		public const int DIGIT = 107;
		
				
	static string NewLine = Environment.NewLine;
	
	static int NewLineLength = NewLine.Length;
	
	protected System.Text.StringBuilder _sbuilder = new System.Text.StringBuilder();
	
	protected AttributeCollection _attributes = new AttributeCollection();
	
	protected TypeMemberModifiers _modifiers = TypeMemberModifiers.None;	
	
	protected void ResetMemberData()
	{
		_modifiers = TypeMemberModifiers.None;
		_attributes.Clear();
	}

	protected BinaryOperatorType ParseCmpOperator(string op)
	{
		switch (op)
		{
			case "<": return BinaryOperatorType.LessThan;
			case ">": return BinaryOperatorType.GreaterThan;
			case "==": return BinaryOperatorType.Equality;
			case "!=": return BinaryOperatorType.Inequality;
			case "=~": return BinaryOperatorType.Match;
			case "kindof": return BinaryOperatorType.TypeTest;
		}
		throw new ArgumentException("op");
	}

	protected BinaryOperatorType ParseSumOperator(string op)
	{
		switch (op)
		{
			case "+": return BinaryOperatorType.Add;
			case "-": return BinaryOperatorType.Subtract;
		}
		throw new ArgumentException("op");
	}

	protected BinaryOperatorType ParseMultOperator(string op)
	{
		switch (op)
		{
			case "*": return BinaryOperatorType.Multiply;
			case "/": return BinaryOperatorType.Divide;
			case "%": return BinaryOperatorType.Modulus;
		}
		throw new ArgumentException("op");
	}

	protected BinaryOperatorType ParseAssignOperator(string op)
	{
		switch (op)
		{
			case "=": return BinaryOperatorType.Assign;
			case "+=": return BinaryOperatorType.InPlaceAdd;
			case "-=": return BinaryOperatorType.InPlaceSubtract;
			case "/=": return BinaryOperatorType.InPlaceDivide;
			case "*=": return BinaryOperatorType.InPlaceMultiply;
		}
		throw new ArgumentException(op, "op");
	}

	protected UnaryOperatorType ParseUnaryOperator(string op)
	{
		switch (op)
		{
			case "++": return UnaryOperatorType.Increment;
			case "--": return UnaryOperatorType.Decrement;
			case "-": return UnaryOperatorType.ArithmeticNegate;
		}
		throw new ArgumentException("op");
	}

	// removes trailing and ending new lines
	protected string MassageDocString(string s)
	{
		int startIndex = 0;
		int length = s.Length;
		if (s.StartsWith(NewLine))
		{			
			startIndex += NewLineLength;
			length -= NewLineLength;
		}
		if (s.EndsWith(NewLine))
		{
			length -= NewLineLength;
		}
		return s.Substring(startIndex, length);
	}
		
		protected void initialize()
		{
			tokenNames = tokenNames_;
		}
		
		
		protected BooParserBase(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
		{
			initialize();
		}
		
		public BooParserBase(TokenBuffer tokenBuf) : this(tokenBuf,2)
		{
		}
		
		protected BooParserBase(TokenStream lexer, int k) : base(lexer,k)
		{
			initialize();
		}
		
		public BooParserBase(TokenStream lexer) : this(lexer,2)
		{
		}
		
		public BooParserBase(ParserSharedInputState state) : base(state,2)
		{
			initialize();
		}
		
	protected Module  start() //throws RecognitionException, TokenStreamException
{
		Module module;
		
		
				module = new Module();		
				module.LexicalInfo = new LexicalInfo(getFilename());
			
		
		try {      // for error handling
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
						goto _loop3_breakloop;
					}
					
				}
_loop3_breakloop:				;
			}    // ( ... )*
			{
				switch ( LA(1) )
				{
				case PACKAGE:
				{
					package_directive(module);
					break;
				}
				case EOF:
				case TIMESPAN:
				case BREAK:
				case CLASS:
				case DEF:
				case ENUM:
				case FINAL:
				case FOR:
				case FALSE:
				case GIVEN:
				case INTERFACE:
				case INTERNAL:
				case IF:
				case NOT:
				case NULL:
				case PUBLIC:
				case PROTECTED:
				case PRIVATE:
				case RAISE:
				case RETURN:
				case RETRY:
				case SELF:
				case SUPER:
				case STATIC:
				case TRY:
				case TRANSIENT:
				case TRUE:
				case USING:
				case WHILE:
				case YIELD:
				case EOS:
				case TRIPLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case LPAREN:
				case SUM_OPERATOR:
				case INCREMENT:
				case DECREMENT:
				case INT:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case LBRACE:
				case RE_LITERAL:
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
					if ((LA(1)==USING))
					{
						using_directive(module);
					}
					else
					{
						goto _loop6_breakloop;
					}
					
				}
_loop6_breakloop:				;
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
						goto _loop8_breakloop;
					}
					
				}
_loop8_breakloop:				;
			}    // ( ... )*
			globals(module);
			match(Token.EOF_TYPE);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_3_);
			}
			else
			{
				throw;
			}
		}
		return module;
	}
	
	protected void docstring(
		Node node
	) //throws RecognitionException, TokenStreamException
{
		
		Token  doc = null;
		
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
								goto _loop12_breakloop;
							}
							
						}
_loop12_breakloop:						;
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
				consume();
				consumeUntil(tokenSet_4_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void package_directive(
		Module container
	) //throws RecognitionException, TokenStreamException
{
		
		Token  t = null;
		
				Token id;
				Package p = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(PACKAGE);
			id=identifier();
			if (0==inputState.guessing)
			{
				
						p = new Package(t);
						p.Name = id.getText();
						container.Package = p; 
					
			}
			eos();
			docstring(p);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_6_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void using_directive(
		Module container
	) //throws RecognitionException, TokenStreamException
{
		
		Token  t = null;
		Token  alias = null;
		
				Token id;
				Using usingNode = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(USING);
			id=identifier();
			if (0==inputState.guessing)
			{
				
						usingNode = new Using(t);
						usingNode.Namespace = id.getText();
						container.Using.Add(usingNode);
					
			}
			{
				switch ( LA(1) )
				{
				case FROM:
				{
					match(FROM);
					id=identifier();
					if (0==inputState.guessing)
					{
						
									usingNode.AssemblyReference = new ReferenceExpression(id);
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
						
									usingNode.Alias = new ReferenceExpression(alias);
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
				consume();
				consumeUntil(tokenSet_6_);
			}
			else
			{
				throw;
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
				case CLASS:
				case ENUM:
				case INTERFACE:
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
				consume();
				consumeUntil(tokenSet_7_);
			}
			else
			{
				throw;
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
						goto _loop91_breakloop;
					}
					
				}
_loop91_breakloop:				;
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
						goto _loop93_breakloop;
					}
					
				}
_loop93_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_3_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void eos() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{ // ( ... )+
			int _cnt15=0;
			for (;;)
			{
				if ((LA(1)==EOS) && (tokenSet_9_.member(LA(2))))
				{
					match(EOS);
				}
				else
				{
					if (_cnt15 >= 1) { goto _loop15_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
				}
				
				_cnt15++;
			}
_loop15_breakloop:			;
			}    // ( ... )+
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_9_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected Token  identifier() //throws RecognitionException, TokenStreamException
{
		Token value;
		
		Token  id1 = null;
		Token  id2 = null;
		
				value = null; _sbuilder.Length = 0;
			
		
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
					if ((LA(1)==DOT) && (LA(2)==ID))
					{
						match(DOT);
						id2 = LT(1);
						match(ID);
						if (0==inputState.guessing)
						{
							_sbuilder.Append('.'); _sbuilder.Append(id2.getText());
						}
					}
					else
					{
						goto _loop255_breakloop;
					}
					
				}
_loop255_breakloop:				;
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
				consume();
				consumeUntil(tokenSet_10_);
			}
			else
			{
				throw;
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
											goto _loop34_breakloop;
										}
										
									}
_loop34_breakloop:									;
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
									goto _loop36_breakloop;
								}
								
							}
_loop36_breakloop:							;
						}    // ( ... )*
					}
					else
					{
						goto _loop37_breakloop;
					}
					
				}
_loop37_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_11_);
			}
			else
			{
				throw;
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
					default:
					{
						goto _loop103_breakloop;
					}
					 }
				}
_loop103_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_12_);
			}
			else
			{
				throw;
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
				consume();
				consumeUntil(tokenSet_13_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void method(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		Token  t = null;
		Token  id = null;
		Token  c = null;
		
				Method m = null;
				TypeReference rt = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(DEF);
			{
				switch ( LA(1) )
				{
				case ID:
				{
					id = LT(1);
					match(ID);
					if (0==inputState.guessing)
					{
						m = new Method(t); m.Name = id.getText();
					}
					break;
				}
				case CONSTRUCTOR:
				{
					c = LT(1);
					match(CONSTRUCTOR);
					if (0==inputState.guessing)
					{
						m = new Constructor(t); m.Name = c.getText();
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
						m.Attributes.Add(_attributes);
					
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
				m.ReturnTypeAttributes.Add(_attributes);
			}
			begin_with_doc(m);
			block(m.Body.Statements);
			end();
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
				consume();
				consumeUntil(tokenSet_13_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void class_definition(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		Token  c = null;
		Token  id = null;
		
				ClassDefinition cd = null;
			
		
		try {      // for error handling
			c = LT(1);
			match(CLASS);
			id = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
				
						cd = new ClassDefinition(c);
						cd.Name = id.getText();
						cd.Modifiers = _modifiers;
						cd.Attributes.Add(_attributes);
						container.Add(cd);
					
			}
			{
				switch ( LA(1) )
				{
				case LPAREN:
				{
					base_types(cd.BaseTypes);
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
			begin_with_doc(cd);
			{
				switch ( LA(1) )
				{
				case PASS:
				{
					{
						match(PASS);
						eos();
					}
					break;
				}
				case CLASS:
				case DEF:
				case ENUM:
				case FINAL:
				case INTERFACE:
				case INTERNAL:
				case PUBLIC:
				case PROTECTED:
				case PRIVATE:
				case STATIC:
				case TRANSIENT:
				case EOS:
				case ID:
				case LBRACK:
				{
					{ // ( ... )+
					int _cnt48=0;
					for (;;)
					{
						if ((tokenSet_14_.member(LA(1))))
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
										goto _loop46_breakloop;
									}
									
								}
_loop46_breakloop:								;
							}    // ( ... )*
							attributes();
							modifiers();
							{
								switch ( LA(1) )
								{
								case DEF:
								{
									method(cd.Members);
									break;
								}
								case ID:
								{
									field_or_property(cd.Members);
									break;
								}
								case CLASS:
								case ENUM:
								case INTERFACE:
								{
									type_definition(cd.Members);
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
							if (_cnt48 >= 1) { goto _loop48_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
						}
						
						_cnt48++;
					}
_loop48_breakloop:					;
					}    // ( ... )+
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			end();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_13_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void interface_definition(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		Token  it = null;
		Token  id = null;
		
				InterfaceDefinition itf = null;
			
		
		try {      // for error handling
			it = LT(1);
			match(INTERFACE);
			id = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
				
						itf = new InterfaceDefinition(it);
						itf.Name = id.getText();
						itf.Modifiers = _modifiers;
						itf.Attributes.Add(_attributes);
						container.Add(itf);
					
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
			begin();
			{
				switch ( LA(1) )
				{
				case PASS:
				{
					{
						match(PASS);
						eos();
					}
					break;
				}
				case DEF:
				case ID:
				case LBRACK:
				{
					{ // ( ... )+
					int _cnt55=0;
					for (;;)
					{
						if ((LA(1)==DEF||LA(1)==ID||LA(1)==LBRACK))
						{
							attributes();
							{
								switch ( LA(1) )
								{
								case DEF:
								{
									interface_method(itf.Members);
									break;
								}
								case ID:
								{
									interface_property(itf.Members);
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
							if (_cnt55 >= 1) { goto _loop55_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
						}
						
						_cnt55++;
					}
_loop55_breakloop:					;
					}    // ( ... )+
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			end();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_13_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void enum_definition(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		Token  id = null;
		
				EnumDefinition ed = null;
			
		
		try {      // for error handling
			match(ENUM);
			id = LT(1);
			match(ID);
			begin();
			if (0==inputState.guessing)
			{
				
						ed = new EnumDefinition(id);
						ed.Name = id.getText();
						ed.Modifiers = _modifiers;
						ed.Attributes.Add(_attributes);
						container.Add(ed);
					
			}
			{
				{ // ( ... )+
				int _cnt27=0;
				for (;;)
				{
					if ((LA(1)==ID||LA(1)==LBRACK))
					{
						enum_member(ed);
					}
					else
					{
						if (_cnt27 >= 1) { goto _loop27_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt27++;
				}
_loop27_breakloop:				;
				}    // ( ... )+
			}
			end();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_13_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void begin() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(COLON);
			match(INDENT);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_15_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void enum_member(
		EnumDefinition container
	) //throws RecognitionException, TokenStreamException
{
		
		Token  id = null;
				
				IntegerLiteralExpression initializer = null;		
			
		
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
				
						EnumMember em = new EnumMember(id);
						em.Name = id.getText();
						em.Initializer = initializer;
						em.Attributes.Add(_attributes);
						container.Members.Add(em);
					
			}
			eos();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_16_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void end() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(DEDENT);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_17_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected IntegerLiteralExpression  integer_literal() //throws RecognitionException, TokenStreamException
{
		IntegerLiteralExpression e;
		
		Token  i = null;
		e = null;
		
		try {      // for error handling
			i = LT(1);
			match(INT);
			if (0==inputState.guessing)
			{
				
						e = new IntegerLiteralExpression(i);
						e.Value = i.getText();
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected void attribute() //throws RecognitionException, TokenStreamException
{
		
				
				antlr.Token id = null;
				Boo.Ast.Attribute attr = null;
			
		
		try {      // for error handling
			id=identifier();
			if (0==inputState.guessing)
			{
				
						attr = new Boo.Ast.Attribute(id);
						attr.Name = id.getText();
						_attributes.Add(attr);
					
			}
			{
				switch ( LA(1) )
				{
				case LPAREN:
				{
					match(LPAREN);
					parameter_list(attr);
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
				consume();
				consumeUntil(tokenSet_19_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void parameter_list(
		INodeWithArguments node
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case TIMESPAN:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TRIPLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case LPAREN:
				case SUM_OPERATOR:
				case INCREMENT:
				case DECREMENT:
				case INT:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case LBRACE:
				case RE_LITERAL:
				{
					parameter(node);
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==COMMA))
							{
								match(COMMA);
								parameter(node);
							}
							else
							{
								goto _loop249_breakloop;
							}
							
						}
_loop249_breakloop:						;
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
				consume();
				consumeUntil(tokenSet_20_);
			}
			else
			{
				throw;
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
						goto _loop58_breakloop;
					}
					
				}
_loop58_breakloop:				;
			}    // ( ... )*
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_21_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void begin_with_doc(
		Node node
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(COLON);
			{
				switch ( LA(1) )
				{
				case EOS:
				{
					match(EOS);
					docstring(node);
					break;
				}
				case INDENT:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(INDENT);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_22_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void field_or_property(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		Token  id = null;
		
				TypeMember tm = null;
				TypeReference tr = null;
				Property p = null;
			
		
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
					eos();
					if (0==inputState.guessing)
					{
						
									Field field = new Field(id);
									field.Type = tr;
									tm = field;
									tm.Name = id.getText();
									tm.Modifiers = _modifiers;
									tm.Attributes.Add(_attributes);
								
					}
					docstring(tm);
					break;
				}
				case COLON:
				{
					if (0==inputState.guessing)
					{
						
									p = new Property(id);			
									p.Type = tr;
									tm = p;
									tm.Name = id.getText();
									tm.Modifiers = _modifiers;
									tm.Attributes.Add(_attributes);
								
					}
					begin_with_doc(p);
					{ // ( ... )+
					int _cnt84=0;
					for (;;)
					{
						if ((tokenSet_23_.member(LA(1))))
						{
							property_accessor(p);
						}
						else
						{
							if (_cnt84 >= 1) { goto _loop84_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
						}
						
						_cnt84++;
					}
_loop84_breakloop:					;
					}    // ( ... )+
					end();
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
				container.Add(tm);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_24_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void interface_method(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		Token  t = null;
		Token  id = null;
		
				Method m = null;
				TypeReference rt = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(DEF);
			id = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
				
						m = new Method(t);
						m.Name = id.getText();
						m.Attributes.Add(_attributes);
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
					eos();
					break;
				}
				case COLON:
				{
					{
						empty_block();
						{    // ( ... )*
							for (;;)
							{
								if ((LA(1)==EOS))
								{
									match(EOS);
								}
								else
								{
									goto _loop64_breakloop;
								}
								
							}
_loop64_breakloop:							;
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
				consume();
				consumeUntil(tokenSet_25_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void interface_property(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		Token  id = null;
		
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
				
						p = new Property(id);
						p.Name = id.getText();
						p.Type = tr;
						p.Attributes.Add(_attributes);
						container.Add(p);
					
			}
			begin();
			{ // ( ... )+
			int _cnt68=0;
			for (;;)
			{
				if ((LA(1)==GET||LA(1)==SET||LA(1)==LBRACK))
				{
					interface_property_accessor(p);
				}
				else
				{
					if (_cnt68 >= 1) { goto _loop68_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
				}
				
				_cnt68++;
			}
_loop68_breakloop:			;
			}    // ( ... )+
			end();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==EOS))
					{
						match(EOS);
					}
					else
					{
						goto _loop70_breakloop;
					}
					
				}
_loop70_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_25_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected TypeReference  type_reference() //throws RecognitionException, TokenStreamException
{
		TypeReference tr;
		
		
				tr=null;
				Token id = null;
			
		
		try {      // for error handling
			id=identifier();
			if (0==inputState.guessing)
			{
				
						tr = new TypeReference(id);
						tr.Name = id.getText();
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_26_);
			}
			else
			{
				throw;
			}
		}
		return tr;
	}
	
	protected void parameter_declaration_list(
		ParameterDeclarationCollection c
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case ID:
				case LBRACK:
				{
					parameter_declaration(c);
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==COMMA))
							{
								match(COMMA);
								parameter_declaration(c);
							}
							else
							{
								goto _loop107_breakloop;
							}
							
						}
_loop107_breakloop:						;
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
				consume();
				consumeUntil(tokenSet_20_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void empty_block() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			begin();
			match(PASS);
			eos();
			end();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_27_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void interface_property_accessor(
		Property p
	) //throws RecognitionException, TokenStreamException
{
		
		Token  gt = null;
		Token  st = null;
		
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
							m = p.Getter = new Method(gt); m.Name = "get";
						}
					}
				}
				else if (((LA(1)==SET))&&( null == p.Setter )) {
					{
						st = LT(1);
						match(SET);
						if (0==inputState.guessing)
						{
							m = p.Setter = new Method(st); m.Name = "set";
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
					empty_block();
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
				
						m.Attributes.Add(_attributes);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_28_);
			}
			else
			{
				throw;
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
						goto _loop96_breakloop;
					}
					
				}
_loop96_breakloop:				;
			}    // ( ... )*
			{
				switch ( LA(1) )
				{
				case PASS:
				{
					{
						match(PASS);
						eos();
					}
					break;
				}
				case TIMESPAN:
				case BREAK:
				case FOR:
				case FALSE:
				case GIVEN:
				case IF:
				case NOT:
				case NULL:
				case RAISE:
				case RETURN:
				case RETRY:
				case SELF:
				case SUPER:
				case TRY:
				case TRUE:
				case WHILE:
				case YIELD:
				case TRIPLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case LPAREN:
				case SUM_OPERATOR:
				case INCREMENT:
				case DECREMENT:
				case INT:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case LBRACE:
				case RE_LITERAL:
				{
					{ // ( ... )+
					int _cnt100=0;
					for (;;)
					{
						if ((tokenSet_8_.member(LA(1))))
						{
							stmt(container);
						}
						else
						{
							if (_cnt100 >= 1) { goto _loop100_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
						}
						
						_cnt100++;
					}
_loop100_breakloop:					;
					}    // ( ... )+
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
				consume();
				consumeUntil(tokenSet_29_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void property_accessor(
		Property p
	) //throws RecognitionException, TokenStreamException
{
		
		Token  gt = null;
		Token  st = null;
				
				Method m = null;
			
		
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
							
											p.Getter = m = new Method(gt);		
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
							
											p.Setter = m = new Method(st);
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
				
						m.Attributes.Add(_attributes);
						m.Modifiers = _modifiers;
					
			}
			compound_stmt(m.Body.Statements);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_30_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void compound_stmt(
		StatementCollection c
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			begin();
			block(c);
			end();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==EOS))
					{
						match(EOS);
					}
					else
					{
						goto _loop117_breakloop;
					}
					
				}
_loop117_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_31_);
			}
			else
			{
				throw;
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
				case TIMESPAN:
				case BREAK:
				case FALSE:
				case NOT:
				case NULL:
				case RAISE:
				case RETURN:
				case RETRY:
				case SELF:
				case SUPER:
				case TRUE:
				case YIELD:
				case TRIPLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case LPAREN:
				case SUM_OPERATOR:
				case INCREMENT:
				case DECREMENT:
				case INT:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case LBRACE:
				case RE_LITERAL:
				{
					{
						{
							switch ( LA(1) )
							{
							case RETURN:
							{
								s=return_stmt();
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
							default:
								bool synPredMatched123 = false;
								if (((LA(1)==ID) && (LA(2)==AS||LA(2)==ASSIGN||LA(2)==COMMA)))
								{
									int _m123 = mark();
									synPredMatched123 = true;
									inputState.guessing++;
									try {
										{
											declaration();
											match(COMMA);
										}
									}
									catch (RecognitionException)
									{
										synPredMatched123 = false;
									}
									rewind(_m123);
									inputState.guessing--;
								}
								if ( synPredMatched123 )
								{
									s=unpack_stmt();
								}
								else if ((LA(1)==ID) && (LA(2)==AS)) {
									s=declaration_stmt();
								}
								else if ((tokenSet_32_.member(LA(1))) && (tokenSet_33_.member(LA(2)))) {
									s=expression_stmt();
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
							case UNTIL:
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
				container.Add(s);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_34_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void parameter_declaration(
		ParameterDeclarationCollection c
	) //throws RecognitionException, TokenStreamException
{
		
		Token  id = null;
				
				TypeReference tr = null;
			
		
		try {      // for error handling
			attributes();
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
				case COMMA:
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
				
						ParameterDeclaration pd = new ParameterDeclaration(id);
						pd.Name = id.getText();
						pd.Type = tr;
						pd.Attributes.Add(_attributes);
						c.Add(pd);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_35_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected ForStatement  for_stmt() //throws RecognitionException, TokenStreamException
{
		ForStatement fs;
		
		Token  f = null;
		
				fs = null;
				Expression iterator = null;
			
		
		try {      // for error handling
			f = LT(1);
			match(FOR);
			if (0==inputState.guessing)
			{
				fs = new ForStatement(f);
			}
			declaration_list(fs.Declarations);
			match(IN);
			iterator=tuple_or_expression();
			if (0==inputState.guessing)
			{
				fs.Iterator = iterator;
			}
			compound_stmt(fs.Statements);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_34_);
			}
			else
			{
				throw;
			}
		}
		return fs;
	}
	
	protected WhileStatement  while_stmt() //throws RecognitionException, TokenStreamException
{
		WhileStatement ws;
		
		Token  w = null;
		
				ws = null;
				Expression e = null;
			
		
		try {      // for error handling
			w = LT(1);
			match(WHILE);
			e=expression();
			if (0==inputState.guessing)
			{
				
						ws = new WhileStatement(w);
						ws.Condition = e;
					
			}
			compound_stmt(ws.Statements);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_34_);
			}
			else
			{
				throw;
			}
		}
		return ws;
	}
	
	protected IfStatement  if_stmt() //throws RecognitionException, TokenStreamException
{
		IfStatement s;
		
		Token  it = null;
		Token  et = null;
		
				s = null;
				Expression e = null;
			
		
		try {      // for error handling
			it = LT(1);
			match(IF);
			e=expression();
			if (0==inputState.guessing)
			{
				
						s = new IfStatement(it);
						s.Expression = e;
						s.TrueBlock = new Block(it);
					
			}
			compound_stmt(s.TrueBlock.Statements);
			{
				switch ( LA(1) )
				{
				case ELSE:
				{
					et = LT(1);
					match(ELSE);
					if (0==inputState.guessing)
					{
						s.FalseBlock = new Block(et);
					}
					compound_stmt(s.FalseBlock.Statements);
					break;
				}
				case EOF:
				case TIMESPAN:
				case DEDENT:
				case BREAK:
				case FOR:
				case FALSE:
				case GIVEN:
				case IF:
				case NOT:
				case NULL:
				case RAISE:
				case RETURN:
				case RETRY:
				case SELF:
				case SUPER:
				case TRY:
				case TRUE:
				case WHILE:
				case YIELD:
				case TRIPLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case LPAREN:
				case SUM_OPERATOR:
				case INCREMENT:
				case DECREMENT:
				case INT:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case LBRACE:
				case RE_LITERAL:
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
				consume();
				consumeUntil(tokenSet_34_);
			}
			else
			{
				throw;
			}
		}
		return s;
	}
	
	protected TryStatement  try_stmt() //throws RecognitionException, TokenStreamException
{
		TryStatement s;
		
		Token  t = null;
		Token  stoken = null;
		Token  etoken = null;
		
				s = null;
				ExceptionHandler eh = null;
				Block sblock = null;
				Block eblock = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(TRY);
			if (0==inputState.guessing)
			{
				s = new TryStatement(t);
			}
			compound_stmt(s.ProtectedBlock.Statements);
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==CATCH))
					{
						eh=exception_handler();
						if (0==inputState.guessing)
						{
							s.ExceptionHandlers.Add(eh);
						}
					}
					else
					{
						goto _loop130_breakloop;
					}
					
				}
_loop130_breakloop:				;
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
						sblock = new Block(stoken);
					}
					compound_stmt(sblock.Statements);
					if (0==inputState.guessing)
					{
						s.SuccessBlock = sblock;
					}
					break;
				}
				case EOF:
				case TIMESPAN:
				case DEDENT:
				case BREAK:
				case ENSURE:
				case FOR:
				case FALSE:
				case GIVEN:
				case IF:
				case NOT:
				case NULL:
				case RAISE:
				case RETURN:
				case RETRY:
				case SELF:
				case SUPER:
				case TRY:
				case TRUE:
				case WHILE:
				case YIELD:
				case TRIPLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case LPAREN:
				case SUM_OPERATOR:
				case INCREMENT:
				case DECREMENT:
				case INT:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case LBRACE:
				case RE_LITERAL:
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
						eblock = new Block(etoken);
					}
					compound_stmt(eblock.Statements);
					if (0==inputState.guessing)
					{
						s.EnsureBlock = eblock;
					}
					break;
				}
				case EOF:
				case TIMESPAN:
				case DEDENT:
				case BREAK:
				case FOR:
				case FALSE:
				case GIVEN:
				case IF:
				case NOT:
				case NULL:
				case RAISE:
				case RETURN:
				case RETRY:
				case SELF:
				case SUPER:
				case TRY:
				case TRUE:
				case WHILE:
				case YIELD:
				case TRIPLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case LPAREN:
				case SUM_OPERATOR:
				case INCREMENT:
				case DECREMENT:
				case INT:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case LBRACE:
				case RE_LITERAL:
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
				consume();
				consumeUntil(tokenSet_34_);
			}
			else
			{
				throw;
			}
		}
		return s;
	}
	
	protected GivenStatement  given_stmt() //throws RecognitionException, TokenStreamException
{
		GivenStatement gs;
		
		Token  given = null;
		Token  when = null;
		Token  otherwise = null;
		
				gs = null;		
				Expression e = null;
				WhenClause wc = null;
			
		
		try {      // for error handling
			given = LT(1);
			match(GIVEN);
			e=expression();
			if (0==inputState.guessing)
			{
				
						gs = new GivenStatement(given);
						gs.Expression = e;
					
			}
			begin();
			{ // ( ... )+
			int _cnt148=0;
			for (;;)
			{
				if ((LA(1)==WHEN))
				{
					when = LT(1);
					match(WHEN);
					e=tuple_or_expression();
					if (0==inputState.guessing)
					{
						
										wc = new WhenClause(when);
										wc.Condition = e;
										gs.WhenClauses.Add(wc);
									
					}
					compound_stmt(wc.Statements);
				}
				else
				{
					if (_cnt148 >= 1) { goto _loop148_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
				}
				
				_cnt148++;
			}
_loop148_breakloop:			;
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
						
										gs.OtherwiseBlock = new Block(otherwise);
									
					}
					compound_stmt(gs.OtherwiseBlock.Statements);
					break;
				}
				case DEDENT:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			end();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_34_);
			}
			else
			{
				throw;
			}
		}
		return gs;
	}
	
	protected ReturnStatement  return_stmt() //throws RecognitionException, TokenStreamException
{
		ReturnStatement s;
		
		Token  r = null;
		
				s = null;
				Expression e = null;
			
		
		try {      // for error handling
			r = LT(1);
			match(RETURN);
			{
				switch ( LA(1) )
				{
				case TIMESPAN:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TRIPLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case COMMA:
				case LPAREN:
				case SUM_OPERATOR:
				case INCREMENT:
				case DECREMENT:
				case INT:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case LBRACE:
				case RE_LITERAL:
				{
					e=tuple_or_expression();
					break;
				}
				case IF:
				case UNLESS:
				case UNTIL:
				case WHILE:
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
				
						s = new ReturnStatement(r);
						s.Expression = e;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_36_);
			}
			else
			{
				throw;
			}
		}
		return s;
	}
	
	protected YieldStatement  yield_stmt() //throws RecognitionException, TokenStreamException
{
		YieldStatement s;
		
		Token  yt = null;
		
				s = null;
				Expression e = null;
			
		
		try {      // for error handling
			yt = LT(1);
			match(YIELD);
			e=tuple_or_expression();
			if (0==inputState.guessing)
			{
				
						s = new YieldStatement(yt);
						s.Expression = e;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_36_);
			}
			else
			{
				throw;
			}
		}
		return s;
	}
	
	protected BreakStatement  break_stmt() //throws RecognitionException, TokenStreamException
{
		BreakStatement s;
		
		Token  b = null;
		s = null;
		
		try {      // for error handling
			b = LT(1);
			match(BREAK);
			if (0==inputState.guessing)
			{
				s = new BreakStatement(b);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_36_);
			}
			else
			{
				throw;
			}
		}
		return s;
	}
	
	protected RaiseStatement  raise_stmt() //throws RecognitionException, TokenStreamException
{
		RaiseStatement s;
		
		Token  t = null;
		
				s = null;
				Expression e = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(RAISE);
			e=expression();
			if (0==inputState.guessing)
			{
				
						s = new RaiseStatement(t);
						s.Exception = e;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_36_);
			}
			else
			{
				throw;
			}
		}
		return s;
	}
	
	protected RetryStatement  retry_stmt() //throws RecognitionException, TokenStreamException
{
		RetryStatement rs;
		
		Token  t = null;
		
				rs = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(RETRY);
			if (0==inputState.guessing)
			{
				rs = new RetryStatement(t);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_36_);
			}
			else
			{
				throw;
			}
		}
		return rs;
	}
	
	protected Declaration  declaration() //throws RecognitionException, TokenStreamException
{
		Declaration d;
		
		Token  id = null;
		
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
				
						d = new Declaration(id);
						d.Name = id.getText();
						d.Type = tr;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_37_);
			}
			else
			{
				throw;
			}
		}
		return d;
	}
	
	protected UnpackStatement  unpack_stmt() //throws RecognitionException, TokenStreamException
{
		UnpackStatement s;
		
		Token  t = null;
		
				s = new UnpackStatement();
				Expression e = null;
			
		
		try {      // for error handling
			declaration_list(s.Declarations);
			t = LT(1);
			match(ASSIGN);
			e=tuple_or_expression();
			if (0==inputState.guessing)
			{
				
						s.Expression = e;
						s.LexicalInfo = new LexicalInfo(t);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_36_);
			}
			else
			{
				throw;
			}
		}
		return s;
	}
	
	protected DeclarationStatement  declaration_stmt() //throws RecognitionException, TokenStreamException
{
		DeclarationStatement s;
		
		Token  id = null;
		
				s = null;
				TypeReference tr = null;
				Expression initializer = null;
			
		
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
					match(ASSIGN);
					initializer=tuple_or_expression();
					break;
				}
				case IF:
				case UNLESS:
				case UNTIL:
				case WHILE:
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
				
						Declaration d = new Declaration(id);
						d.Name = id.getText();
						d.Type = tr;
						
						s = new DeclarationStatement(d);
						s.Declaration = d;
						s.Initializer = initializer;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_36_);
			}
			else
			{
				throw;
			}
		}
		return s;
	}
	
	protected ExpressionStatement  expression_stmt() //throws RecognitionException, TokenStreamException
{
		ExpressionStatement s;
		
		
				s = null;
				Expression e = null;
			
		
		try {      // for error handling
			e=expression();
			if (0==inputState.guessing)
			{
				
						s = new ExpressionStatement(e);
						s.Expression = e;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_36_);
			}
			else
			{
				throw;
			}
		}
		return s;
	}
	
	protected StatementModifier  stmt_modifier() //throws RecognitionException, TokenStreamException
{
		StatementModifier m;
		
		Token  i = null;
		Token  u = null;
		Token  w = null;
		Token  ut = null;
		
				m = null;
				Expression e = null;
				Token t = null;
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
				case UNTIL:
				{
					ut = LT(1);
					match(UNTIL);
					if (0==inputState.guessing)
					{
						t = ut; type = StatementModifierType.Until;
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			e=expression();
			if (0==inputState.guessing)
			{
				
						m = new StatementModifier(t);
						m.Type = type;
						m.Condition = e;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_38_);
			}
			else
			{
				throw;
			}
		}
		return m;
	}
	
	protected Expression  expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		Token  t = null;
		
				e = null;
				TypeReference tr = null;
			
		
		try {      // for error handling
			e=boolean_expression();
			{
				if ((LA(1)==AS) && (LA(2)==ID))
				{
					t = LT(1);
					match(AS);
					tr=type_reference();
					if (0==inputState.guessing)
					{
						
									AsExpression ae = new AsExpression(t);
									ae.Target = e;
									ae.Type = tr;
									e = ae; 
								
					}
				}
				else if ((tokenSet_18_.member(LA(1))) && (tokenSet_39_.member(LA(2)))) {
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
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected ExceptionHandler  exception_handler() //throws RecognitionException, TokenStreamException
{
		ExceptionHandler eh;
		
		Token  c = null;
		Token  x = null;
		
				eh = null;		
				TypeReference tr = null;
			
		
		try {      // for error handling
			c = LT(1);
			match(CATCH);
			tr=type_reference();
			{
				switch ( LA(1) )
				{
				case ID:
				{
					x = LT(1);
					match(ID);
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
				
						eh = new ExceptionHandler(c);
						if (null != x)
						{
							eh.Declaration = new Declaration(x);
							eh.Declaration.Name = x.getText();
						}
						else
						{
							eh.Declaration = new Declaration(tr);
						}
						eh.Declaration.Type = tr;
					
			}
			compound_stmt(eh.Statements);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_40_);
			}
			else
			{
				throw;
			}
		}
		return eh;
	}
	
	protected Expression  tuple_or_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		Token  c = null;
		Token  t = null;
		
				e = null;
				TupleLiteralExpression tle = null;
			
		
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
						e = new TupleLiteralExpression(c);
					}
				}
				break;
			}
			case TIMESPAN:
			case FALSE:
			case NOT:
			case NULL:
			case SELF:
			case SUPER:
			case TRUE:
			case TRIPLE_QUOTED_STRING:
			case ID:
			case LBRACK:
			case LPAREN:
			case SUM_OPERATOR:
			case INCREMENT:
			case DECREMENT:
			case INT:
			case DOUBLE_QUOTED_STRING:
			case SINGLE_QUOTED_STRING:
			case LBRACE:
			case RE_LITERAL:
			{
				{
					e=expression();
					{
						if ((LA(1)==COMMA) && (tokenSet_41_.member(LA(2))))
						{
							t = LT(1);
							match(COMMA);
							if (0==inputState.guessing)
							{
								
												tle = new TupleLiteralExpression(e);
												tle.Items.Add(e);		
											
							}
							{
								if ((tokenSet_32_.member(LA(1))) && (tokenSet_41_.member(LA(2))))
								{
									e=expression();
									if (0==inputState.guessing)
									{
										tle.Items.Add(e);
									}
									{    // ( ... )*
										for (;;)
										{
											if ((LA(1)==COMMA) && (tokenSet_32_.member(LA(2))))
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
												goto _loop164_breakloop;
											}
											
										}
_loop164_breakloop:										;
									}    // ( ... )*
								}
								else if ((tokenSet_18_.member(LA(1))) && (tokenSet_39_.member(LA(2)))) {
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
						else if ((tokenSet_18_.member(LA(1))) && (tokenSet_39_.member(LA(2)))) {
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
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected Statement  continue_stmt() //throws RecognitionException, TokenStreamException
{
		Statement s;
		
		Token  c = null;
		s = null;
		
		try {      // for error handling
			c = LT(1);
			match(CONTINUE);
			if (0==inputState.guessing)
			{
				s = new ContinueStatement(c);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_3_);
			}
			else
			{
				throw;
			}
		}
		return s;
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
						goto _loop155_breakloop;
					}
					
				}
_loop155_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_42_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected Expression  boolean_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		Token  nt = null;
		Token  ot = null;
		
				e = null;
				Expression r = null;
			
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case NOT:
			{
				{
					nt = LT(1);
					match(NOT);
					e=boolean_expression();
					if (0==inputState.guessing)
					{
						
									UnaryExpression ue = new UnaryExpression(nt);
									ue.Operator = UnaryOperatorType.Not;
									ue.Operand = e;
									e = ue;
								
					}
				}
				break;
			}
			case TIMESPAN:
			case FALSE:
			case NULL:
			case SELF:
			case SUPER:
			case TRUE:
			case TRIPLE_QUOTED_STRING:
			case ID:
			case LBRACK:
			case LPAREN:
			case SUM_OPERATOR:
			case INCREMENT:
			case DECREMENT:
			case INT:
			case DOUBLE_QUOTED_STRING:
			case SINGLE_QUOTED_STRING:
			case LBRACE:
			case RE_LITERAL:
			{
				{
					e=boolean_term();
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==OR) && (tokenSet_43_.member(LA(2))))
							{
								ot = LT(1);
								match(OR);
								r=boolean_term();
								if (0==inputState.guessing)
								{
									
													BinaryExpression be = new BinaryExpression(ot);
													be.Operator = BinaryOperatorType.Or;
													be.Left = e;
													be.Right = r;
													e = be;
												
								}
							}
							else
							{
								goto _loop171_breakloop;
							}
							
						}
_loop171_breakloop:						;
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
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected Expression  boolean_term() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		Token  at = null;
		
				e = null;
				Expression r = null;
			
		
		try {      // for error handling
			e=ternary_expression();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==AND) && (tokenSet_43_.member(LA(2))))
					{
						at = LT(1);
						match(AND);
						r=ternary_expression();
						if (0==inputState.guessing)
						{
							
										BinaryExpression be = new BinaryExpression(at);
										be.Operator = BinaryOperatorType.And;
										be.Left = e;
										be.Right = r; 
										e = be;
									
						}
					}
					else
					{
						goto _loop174_breakloop;
					}
					
				}
_loop174_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected Expression  ternary_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		Token  t = null;
		
				e = null;			
				Expression te = null;
				Expression fe = null;
			
		
		try {      // for error handling
			e=assignment_expression();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==QMARK) && (tokenSet_43_.member(LA(2))))
					{
						t = LT(1);
						match(QMARK);
						te=ternary_expression();
						match(COLON);
						fe=assignment_expression();
						if (0==inputState.guessing)
						{
							
										TernaryExpression finalExpression = new TernaryExpression(t);
										finalExpression.Condition = e;
										finalExpression.TrueExpression = te;
										finalExpression.FalseExpression = fe;
										e = finalExpression;
									
						}
					}
					else
					{
						goto _loop177_breakloop;
					}
					
				}
_loop177_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected Expression  assignment_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		Token  op = null;
		
				e = null;
				Expression r=null;
			
		
		try {      // for error handling
			e=conditional_expression();
			{
				if ((LA(1)==ASSIGN) && (tokenSet_44_.member(LA(2))))
				{
					op = LT(1);
					match(ASSIGN);
					r=tuple_or_expression();
					if (0==inputState.guessing)
					{
						
									BinaryExpression be = new BinaryExpression(op);
									be.Operator = ParseAssignOperator(op.getText());
									be.Left = e;
									be.Right = r;
									e = be; 
								
					}
				}
				else if ((tokenSet_18_.member(LA(1))) && (tokenSet_39_.member(LA(2)))) {
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
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected Expression  conditional_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		
				e = null;
				Token op = null;		
				Expression r = null;
			
		
		try {      // for error handling
			e=sum();
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_45_.member(LA(1))) && (tokenSet_43_.member(LA(2))))
					{
						op=cmp_operator();
						r=sum();
						if (0==inputState.guessing)
						{
							
										BinaryExpression be = new BinaryExpression(op);
										be.Operator = ParseCmpOperator(op.getText());
										be.Left = e;
										be.Right = r;
										e = be;			
									
						}
					}
					else
					{
						goto _loop182_breakloop;
					}
					
				}
_loop182_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected Expression  sum() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		Token  op = null;
		
				e = null;
				Expression r = null;
			
		
		try {      // for error handling
			e=term();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==SUM_OPERATOR) && (tokenSet_43_.member(LA(2))))
					{
						op = LT(1);
						match(SUM_OPERATOR);
						r=term();
						if (0==inputState.guessing)
						{
							
										BinaryExpression be = new BinaryExpression(op);
										be.Operator = ParseSumOperator(op.getText());
										be.Left = e;
										be.Right = r;
										e = be;
									
						}
					}
					else
					{
						goto _loop185_breakloop;
					}
					
				}
_loop185_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected Token  cmp_operator() //throws RecognitionException, TokenStreamException
{
		Token t;
		
		Token  co = null;
		Token  isop = null;
		Token  kt = null;
		Token  it = null;
		t = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case CMP_OPERATOR:
			{
				co = LT(1);
				match(CMP_OPERATOR);
				if (0==inputState.guessing)
				{
					t = co;
				}
				break;
			}
			case IS:
			{
				isop = LT(1);
				match(IS);
				if (0==inputState.guessing)
				{
					t = isop;
				}
				break;
			}
			case KINDOF:
			{
				kt = LT(1);
				match(KINDOF);
				if (0==inputState.guessing)
				{
					t = kt;
				}
				break;
			}
			case IN:
			{
				it = LT(1);
				match(IN);
				if (0==inputState.guessing)
				{
					t = it;
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
				consume();
				consumeUntil(tokenSet_43_);
			}
			else
			{
				throw;
			}
		}
		return t;
	}
	
	protected Expression  term() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		Token  op = null;
		
				e = null;
				Expression r = null;
			
		
		try {      // for error handling
			e=unary_expression();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==MULT_OPERATOR) && (tokenSet_43_.member(LA(2))))
					{
						op = LT(1);
						match(MULT_OPERATOR);
						r=unary_expression();
						if (0==inputState.guessing)
						{
							
										BinaryExpression be = new BinaryExpression(op);
										be.Operator = ParseMultOperator(op.getText());
										be.Left = e;
										be.Right = r;
										e = be;
									
						}
					}
					else
					{
						goto _loop188_breakloop;
					}
					
				}
_loop188_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected Expression  unary_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		Token  sum_op = null;
		Token  inc = null;
		Token  dec = null;
		
					e = null;
					Token op = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case SUM_OPERATOR:
				{
					sum_op = LT(1);
					match(SUM_OPERATOR);
					if (0==inputState.guessing)
					{
						op = sum_op;
					}
					break;
				}
				case INCREMENT:
				{
					inc = LT(1);
					match(INCREMENT);
					if (0==inputState.guessing)
					{
						op = inc;
					}
					break;
				}
				case DECREMENT:
				{
					dec = LT(1);
					match(DECREMENT);
					if (0==inputState.guessing)
					{
						op = dec;
					}
					break;
				}
				case TIMESPAN:
				case FALSE:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TRIPLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case LPAREN:
				case INT:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case LBRACE:
				case RE_LITERAL:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			e=slicing_expression();
			if (0==inputState.guessing)
			{
				
						if (null != op)
						{
							UnaryExpression ue = new UnaryExpression(op);
							ue.Operator = ParseUnaryOperator(op.getText());
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
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected Expression  slicing_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		Token  lbrack = null;
		Token  id = null;
		Token  lparen = null;
		
				e = null;
				Expression begin = null;
				Expression end = null;
				Expression step = null;		
				MethodInvocationExpression mce = null;
			
		
		try {      // for error handling
			e=atom();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==LBRACK) && (tokenSet_46_.member(LA(2))))
					{
						{
							lbrack = LT(1);
							match(LBRACK);
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
											case TIMESPAN:
											case FALSE:
											case NOT:
											case NULL:
											case SELF:
											case SUPER:
											case TRUE:
											case TRIPLE_QUOTED_STRING:
											case ID:
											case LBRACK:
											case LPAREN:
											case SUM_OPERATOR:
											case INCREMENT:
											case DECREMENT:
											case INT:
											case DOUBLE_QUOTED_STRING:
											case SINGLE_QUOTED_STRING:
											case LBRACE:
											case RE_LITERAL:
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
								case TIMESPAN:
								case FALSE:
								case NOT:
								case NULL:
								case SELF:
								case SUPER:
								case TRUE:
								case TRIPLE_QUOTED_STRING:
								case ID:
								case LBRACK:
								case LPAREN:
								case SUM_OPERATOR:
								case INCREMENT:
								case DECREMENT:
								case INT:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case LBRACE:
								case RE_LITERAL:
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
												case TIMESPAN:
												case FALSE:
												case NOT:
												case NULL:
												case SELF:
												case SUPER:
												case TRUE:
												case TRIPLE_QUOTED_STRING:
												case ID:
												case LBRACK:
												case LPAREN:
												case SUM_OPERATOR:
												case INCREMENT:
												case DECREMENT:
												case INT:
												case DOUBLE_QUOTED_STRING:
												case SINGLE_QUOTED_STRING:
												case LBRACE:
												case RE_LITERAL:
												{
													end=expression();
													break;
												}
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
								
												SlicingExpression se = new SlicingExpression(lbrack);
												se.Target = e;
												se.Begin = begin;
												se.End = end;
												se.Step = step;
												e = se;
											
							}
							match(RBRACK);
						}
					}
					else if ((LA(1)==DOT) && (LA(2)==ID)) {
						{
							match(DOT);
							id = LT(1);
							match(ID);
							if (0==inputState.guessing)
							{
								
													MemberReferenceExpression mre = new MemberReferenceExpression(id);
													mre.Target = e;
													mre.Name = id.getText();
													e = mre;
												
							}
						}
					}
					else if ((LA(1)==LPAREN) && (tokenSet_47_.member(LA(2)))) {
						{
							lparen = LT(1);
							match(LPAREN);
							if (0==inputState.guessing)
							{
								
													mce = new MethodInvocationExpression(lparen);
													mce.Target = e;
													e = mce;
												
							}
							parameter_list(mce);
							match(RPAREN);
						}
					}
					else
					{
						goto _loop213_breakloop;
					}
					
				}
_loop213_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
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
				case TIMESPAN:
				case FALSE:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TRIPLE_QUOTED_STRING:
				case LBRACK:
				case INT:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case LBRACE:
				case RE_LITERAL:
				{
					e=literal();
					break;
				}
				case ID:
				{
					e=reference_expression();
					break;
				}
				case LPAREN:
				{
					e=paren_expression();
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
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
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
				{
					e=integer_literal();
					break;
				}
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
				case LBRACE:
				{
					e=hash_literal();
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
				case TIMESPAN:
				{
					e=timespan_literal();
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
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected ReferenceExpression  reference_expression() //throws RecognitionException, TokenStreamException
{
		ReferenceExpression e;
		
		Token  id = null;
		e = null;
		
		try {      // for error handling
			id = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
				
						e = new ReferenceExpression(id);
						e.Name = id.getText();
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
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
			e=tuple_or_expression();
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected Expression  tuple() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		Token  t = null;
		
				TupleLiteralExpression tle = null;
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
						
									tle = new TupleLiteralExpression(t);
									tle.Items.Add(e);
								
					}
					{
						switch ( LA(1) )
						{
						case TIMESPAN:
						case FALSE:
						case NOT:
						case NULL:
						case SELF:
						case SUPER:
						case TRUE:
						case TRIPLE_QUOTED_STRING:
						case ID:
						case LBRACK:
						case LPAREN:
						case SUM_OPERATOR:
						case INCREMENT:
						case DECREMENT:
						case INT:
						case DOUBLE_QUOTED_STRING:
						case SINGLE_QUOTED_STRING:
						case LBRACE:
						case RE_LITERAL:
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
										goto _loop200_breakloop;
									}
									
								}
_loop200_breakloop:								;
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
				consume();
				consumeUntil(tokenSet_3_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected Expression  string_literal() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		Token  dqs = null;
		Token  sqs = null;
		Token  tqs = null;
		
				e = null;
			
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case SINGLE_QUOTED_STRING:
			{
				sqs = LT(1);
				match(SINGLE_QUOTED_STRING);
				if (0==inputState.guessing)
				{
					
							e = new StringLiteralExpression(sqs, sqs.getText());
						
				}
				break;
			}
			case TRIPLE_QUOTED_STRING:
			{
				tqs = LT(1);
				match(TRIPLE_QUOTED_STRING);
				if (0==inputState.guessing)
				{
					
							e = new StringLiteralExpression(tqs, tqs.getText());
						
				}
				break;
			}
			default:
				bool synPredMatched223 = false;
				if (((LA(1)==DOUBLE_QUOTED_STRING) && (tokenSet_18_.member(LA(2)))))
				{
					int _m223 = mark();
					synPredMatched223 = true;
					inputState.guessing++;
					try {
						{
							match(DOUBLE_QUOTED_STRING);
							match(ESEPARATOR);
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
					e=string_formatting();
				}
				else if ((LA(1)==DOUBLE_QUOTED_STRING) && (tokenSet_18_.member(LA(2)))) {
					dqs = LT(1);
					match(DOUBLE_QUOTED_STRING);
					if (0==inputState.guessing)
					{
						
								e = new StringLiteralExpression(dqs, dqs.getText());
							
					}
				}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			break; }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected Expression  list_literal() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		Token  lbrack = null;
		Token  f = null;
		
				e = null;
				ListLiteralExpression lle = null;
				ListDisplayExpression lde = null;
				Expression item = null;
				Expression iterator = null;
				StatementModifier filter = null;
			
		
		try {      // for error handling
			lbrack = LT(1);
			match(LBRACK);
			{
				switch ( LA(1) )
				{
				case TIMESPAN:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TRIPLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case LPAREN:
				case SUM_OPERATOR:
				case INCREMENT:
				case DECREMENT:
				case INT:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case LBRACE:
				case RE_LITERAL:
				{
					{
						item=expression();
						{
							switch ( LA(1) )
							{
							case FOR:
							{
								{
									f = LT(1);
									match(FOR);
									if (0==inputState.guessing)
									{
										
																lde = new ListDisplayExpression(f);
																lde.Expression = item;
															
									}
									declaration_list(lde.Declarations);
									match(IN);
									iterator=expression();
									if (0==inputState.guessing)
									{
										lde.Iterator = iterator;
									}
									{
										switch ( LA(1) )
										{
										case IF:
										case UNLESS:
										case UNTIL:
										case WHILE:
										{
											filter=stmt_modifier();
											if (0==inputState.guessing)
											{
												lde.Filter = filter;
											}
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
									if (0==inputState.guessing)
									{
										e = lde;
									}
								}
								break;
							}
							case COMMA:
							case RBRACK:
							{
								if (0==inputState.guessing)
								{
									
														e = lle = new ListLiteralExpression(lbrack);
														lle.Items.Add(item);
													
								}
								{    // ( ... )*
									for (;;)
									{
										if ((LA(1)==COMMA))
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
											goto _loop234_breakloop;
										}
										
									}
_loop234_breakloop:									;
								}    // ( ... )*
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
						e = new ListLiteralExpression(lbrack);
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
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected HashLiteralExpression  hash_literal() //throws RecognitionException, TokenStreamException
{
		HashLiteralExpression dle;
		
		Token  lbrace = null;
		
				dle = null;
				ExpressionPair pair = null;
			
		
		try {      // for error handling
			lbrace = LT(1);
			match(LBRACE);
			if (0==inputState.guessing)
			{
				dle = new HashLiteralExpression(lbrace);
			}
			{
				switch ( LA(1) )
				{
				case TIMESPAN:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TRIPLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case LPAREN:
				case SUM_OPERATOR:
				case INCREMENT:
				case DECREMENT:
				case INT:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case LBRACE:
				case RE_LITERAL:
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
								goto _loop238_breakloop;
							}
							
						}
_loop238_breakloop:						;
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
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return dle;
	}
	
	protected RELiteralExpression  re_literal() //throws RecognitionException, TokenStreamException
{
		RELiteralExpression re;
		
		Token  value = null;
		re = null;
		
		try {      // for error handling
			value = LT(1);
			match(RE_LITERAL);
			if (0==inputState.guessing)
			{
				re = new RELiteralExpression(value, value.getText());
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return re;
	}
	
	protected BoolLiteralExpression  bool_literal() //throws RecognitionException, TokenStreamException
{
		BoolLiteralExpression e;
		
		Token  t = null;
		Token  f = null;
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
					
							e = new BoolLiteralExpression(t);
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
					
							e = new BoolLiteralExpression(f);
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
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected NullLiteralExpression  null_literal() //throws RecognitionException, TokenStreamException
{
		NullLiteralExpression e;
		
		Token  t = null;
		e = null;
		
		try {      // for error handling
			t = LT(1);
			match(NULL);
			if (0==inputState.guessing)
			{
				e = new NullLiteralExpression(t);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected SelfLiteralExpression  self_literal() //throws RecognitionException, TokenStreamException
{
		SelfLiteralExpression e;
		
		Token  t = null;
		e = null;
		
		try {      // for error handling
			t = LT(1);
			match(SELF);
			if (0==inputState.guessing)
			{
				e = new SelfLiteralExpression(t);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected SuperLiteralExpression  super_literal() //throws RecognitionException, TokenStreamException
{
		SuperLiteralExpression e;
		
		Token  t = null;
		e = null;
		
		try {      // for error handling
			t = LT(1);
			match(SUPER);
			if (0==inputState.guessing)
			{
				e = new SuperLiteralExpression(t);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected TimeSpanLiteralExpression  timespan_literal() //throws RecognitionException, TokenStreamException
{
		TimeSpanLiteralExpression tsle;
		
		Token  value = null;
		tsle = null;
		
		try {      // for error handling
			value = LT(1);
			match(TIMESPAN);
			if (0==inputState.guessing)
			{
				tsle = new TimeSpanLiteralExpression(value, value.getText());
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return tsle;
	}
	
	protected StringFormattingExpression  string_formatting() //throws RecognitionException, TokenStreamException
{
		StringFormattingExpression e;
		
		Token  dqs = null;
		
				e = null;
				Expression param = null;
			
		
		try {      // for error handling
			dqs = LT(1);
			match(DOUBLE_QUOTED_STRING);
			if (0==inputState.guessing)
			{
				
						e = new StringFormattingExpression(dqs);
						e.Template = dqs.getText();
					
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==ESEPARATOR) && (tokenSet_32_.member(LA(2))))
					{
						match(ESEPARATOR);
						param=expression();
						if (0==inputState.guessing)
						{
							e.Arguments.Add(param);
						}
					}
					else
					{
						goto _loop226_breakloop;
					}
					
				}
_loop226_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_18_);
			}
			else
			{
				throw;
			}
		}
		return e;
	}
	
	protected ExpressionPair  expression_pair() //throws RecognitionException, TokenStreamException
{
		ExpressionPair ep;
		
		Token  t = null;
		
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
				ep = new ExpressionPair(t, key, value);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				consume();
				consumeUntil(tokenSet_48_);
			}
			else
			{
				throw;
			}
		}
		return ep;
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
				case TIMESPAN:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TRIPLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case LPAREN:
				case SUM_OPERATOR:
				case INCREMENT:
				case DECREMENT:
				case INT:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case LBRACE:
				case RE_LITERAL:
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
								goto _loop245_breakloop;
							}
							
						}
_loop245_breakloop:						;
					}    // ( ... )*
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
				consume();
				consumeUntil(tokenSet_3_);
			}
			else
			{
				throw;
			}
		}
	}
	
	protected void parameter(
		INodeWithArguments node
	) //throws RecognitionException, TokenStreamException
{
		
		Token  colon = null;
		
				Expression e = null;
				Expression value = null;
			
		
		try {      // for error handling
			e=expression();
			{
				switch ( LA(1) )
				{
				case COLON:
				{
					{
						colon = LT(1);
						match(COLON);
						value=expression();
						if (0==inputState.guessing)
						{
							node.NamedArguments.Add(new ExpressionPair(colon, e, value));
						}
					}
					break;
				}
				case COMMA:
				case RPAREN:
				{
					if (0==inputState.guessing)
					{
						node.Arguments.Add(e);
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
				consume();
				consumeUntil(tokenSet_35_);
			}
			else
			{
				throw;
			}
		}
	}
	
	private void initializeFactory()
	{
	}
	
	public static readonly string[] tokenNames_ = new string[] {
		@"""<0>""",
		@"""EOF""",
		@"""<2>""",
		@"""NULL_TREE_LOOKAHEAD""",
		@"""TIMESPAN""",
		@"""ESEPARATOR""",
		@"""INDENT""",
		@"""DEDENT""",
		@"""COMPILATION_UNIT""",
		@"""PARAMETERS""",
		@"""PARAMETER""",
		@"""ELIST""",
		@"""DLIST""",
		@"""TYPE""",
		@"""CALL""",
		@"""STMT""",
		@"""BLOCK""",
		@"""FIELD""",
		@"""MODIFIERS""",
		@"""MODULE""",
		@"""LITERAL""",
		@"""LIST_LITERAL""",
		@"""UNPACKING""",
		@"""and""",
		@"""as""",
		@"""break""",
		@"""continue""",
		@"""catch""",
		@"""class""",
		@"""constructor""",
		@"""def""",
		@"""else""",
		@"""ensure""",
		@"""enum""",
		@"""failure""",
		@"""final""",
		@"""from""",
		@"""for""",
		@"""false""",
		@"""get""",
		@"""given""",
		@"""interface""",
		@"""internal""",
		@"""is""",
		@"""if""",
		@"""in""",
		@"""kindof""",
		@"""not""",
		@"""null""",
		@"""or""",
		@"""otherwise""",
		@"""pass""",
		@"""package""",
		@"""property""",
		@"""public""",
		@"""protected""",
		@"""private""",
		@"""raise""",
		@"""return""",
		@"""retry""",
		@"""set""",
		@"""self""",
		@"""super""",
		@"""static""",
		@"""success""",
		@"""try""",
		@"""transient""",
		@"""true""",
		@"""unless""",
		@"""using""",
		@"""until""",
		@"""void""",
		@"""when""",
		@"""while""",
		@"""yield""",
		@"""EOS""",
		@"""TRIPLE_QUOTED_STRING""",
		@"""ID""",
		@"""ASSIGN""",
		@"""LBRACK""",
		@"""COMMA""",
		@"""RBRACK""",
		@"""LPAREN""",
		@"""RPAREN""",
		@"""COLON""",
		@"""QMARK""",
		@"""SUM_OPERATOR""",
		@"""MULT_OPERATOR""",
		@"""INCREMENT""",
		@"""DECREMENT""",
		@"""CMP_OPERATOR""",
		@"""DOT""",
		@"""INT""",
		@"""DOUBLE_QUOTED_STRING""",
		@"""SINGLE_QUOTED_STRING""",
		@"""LBRACE""",
		@"""RBRACE""",
		@"""RE_LITERAL""",
		@"""SL_COMMENT""",
		@"""WS""",
		@"""ESCAPED_EXPRESSION""",
		@"""DQS_ESC""",
		@"""SQS_ESC""",
		@"""SESC""",
		@"""RE_CHAR""",
		@"""RE_ESC""",
		@"""ID_LETTER""",
		@"""DIGIT"""
	};
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = { -1165984345614188526L, 12671303214L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = { -9097264605926785024L, 32772L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { -9097264605389914112L, 139268L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { 2L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { -1165984345614188334L, 12671303214L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { -10133191713095502L, 17179344510L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = { -1170487945241559022L, 12671303214L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = { -1170487945241559022L, 12671303182L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	private static long[] mk_tokenSet_8_()
	{
		long[] data = { 7926776660685226000L, 12671301130L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
	private static long[] mk_tokenSet_9_()
	{
		long[] data = { -17565890878898030L, 12671303214L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
	private static long[] mk_tokenSet_10_()
	{
		long[] data = { 695097532350498L, 4513065552L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
	private static long[] mk_tokenSet_11_()
	{
		long[] data = { -7944342551564124160L, 1056772L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
	private static long[] mk_tokenSet_12_()
	{
		long[] data = { 1152924263318028288L, 8192L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
	private static long[] mk_tokenSet_13_()
	{
		long[] data = { -1170487945241558894L, 12671303182L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_13_ = new BitSet(mk_tokenSet_13_());
	private static long[] mk_tokenSet_14_()
	{
		long[] data = { -9097264605926785024L, 43012L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_14_ = new BitSet(mk_tokenSet_14_());
	private static long[] mk_tokenSet_15_()
	{
		long[] data = { 9081950515935313936L, 12671303434L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_15_ = new BitSet(mk_tokenSet_15_());
	private static long[] mk_tokenSet_16_()
	{
		long[] data = { 128L, 40960L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_16_ = new BitSet(mk_tokenSet_16_());
	private static long[] mk_tokenSet_17_()
	{
		long[] data = { -16439984395386734L, 12671303439L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_17_ = new BitSet(mk_tokenSet_17_());
	private static long[] mk_tokenSet_18_()
	{
		long[] data = { 695028812873762L, 4513057360L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_18_ = new BitSet(mk_tokenSet_18_());
	private static long[] mk_tokenSet_19_()
	{
		long[] data = { 0L, 196608L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_19_ = new BitSet(mk_tokenSet_19_());
	private static long[] mk_tokenSet_20_()
	{
		long[] data = { 0L, 524288L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_20_ = new BitSet(mk_tokenSet_20_());
	private static long[] mk_tokenSet_21_()
	{
		long[] data = { 0L, 1048576L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_21_ = new BitSet(mk_tokenSet_21_());
	private static long[] mk_tokenSet_22_()
	{
		long[] data = { -15314091065212912L, 12671303182L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_22_ = new BitSet(mk_tokenSet_22_());
	private static long[] mk_tokenSet_23_()
	{
		long[] data = { -7944344760519491584L, 32772L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_23_ = new BitSet(mk_tokenSet_23_());
	private static long[] mk_tokenSet_24_()
	{
		long[] data = { -9097264605926784896L, 43012L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_24_ = new BitSet(mk_tokenSet_24_());
	private static long[] mk_tokenSet_25_()
	{
		long[] data = { 1073741952L, 40960L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_25_ = new BitSet(mk_tokenSet_25_());
	private static long[] mk_tokenSet_26_()
	{
		long[] data = { 695028812873762L, 4513065552L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_26_ = new BitSet(mk_tokenSet_26_());
	private static long[] mk_tokenSet_27_()
	{
		long[] data = { 1152922055436402816L, 43008L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_27_ = new BitSet(mk_tokenSet_27_());
	private static long[] mk_tokenSet_28_()
	{
		long[] data = { 1152922054362660992L, 32768L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_28_ = new BitSet(mk_tokenSet_28_());
	private static long[] mk_tokenSet_29_()
	{
		long[] data = { 128L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_29_ = new BitSet(mk_tokenSet_29_());
	private static long[] mk_tokenSet_30_()
	{
		long[] data = { -7944344760519491456L, 32772L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_30_ = new BitSet(mk_tokenSet_30_());
	private static long[] mk_tokenSet_31_()
	{
		long[] data = { -16442193350754158L, 12671301391L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_31_ = new BitSet(mk_tokenSet_31_());
	private static long[] mk_tokenSet_32_()
	{
		long[] data = { 6917951514984054800L, 12671299592L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_32_ = new BitSet(mk_tokenSet_32_());
	private static long[] mk_tokenSet_33_()
	{
		long[] data = { 6918646406357975088L, 17178294872L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_33_ = new BitSet(mk_tokenSet_33_());
	private static long[] mk_tokenSet_34_()
	{
		long[] data = { 7926776660685226130L, 12671301130L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_34_ = new BitSet(mk_tokenSet_34_());
	private static long[] mk_tokenSet_35_()
	{
		long[] data = { 0L, 589824L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_35_ = new BitSet(mk_tokenSet_35_());
	private static long[] mk_tokenSet_36_()
	{
		long[] data = { 17592186044416L, 2640L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_36_ = new BitSet(mk_tokenSet_36_());
	private static long[] mk_tokenSet_37_()
	{
		long[] data = { 35184372088832L, 81920L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_37_ = new BitSet(mk_tokenSet_37_());
	private static long[] mk_tokenSet_38_()
	{
		long[] data = { 0L, 133120L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_38_ = new BitSet(mk_tokenSet_38_());
	private static long[] mk_tokenSet_39_()
	{
		long[] data = { 7927453959873102066L, 17179868762L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_39_ = new BitSet(mk_tokenSet_39_());
	private static long[] mk_tokenSet_40_()
	{
		long[] data = { 7926776665114411154L, 12671301131L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_40_ = new BitSet(mk_tokenSet_40_());
	private static long[] mk_tokenSet_41_()
	{
		long[] data = { 6918646543796928562L, 17179867736L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_41_ = new BitSet(mk_tokenSet_41_());
	private static long[] mk_tokenSet_42_()
	{
		long[] data = { 35184372088832L, 16384L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_42_ = new BitSet(mk_tokenSet_42_());
	private static long[] mk_tokenSet_43_()
	{
		long[] data = { 6917810777495699472L, 12671299592L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_43_ = new BitSet(mk_tokenSet_43_());
	private static long[] mk_tokenSet_44_()
	{
		long[] data = { 6917951514984054800L, 12671365128L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_44_ = new BitSet(mk_tokenSet_44_());
	private static long[] mk_tokenSet_45_()
	{
		long[] data = { 114349209288704L, 67108864L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_45_ = new BitSet(mk_tokenSet_45_());
	private static long[] mk_tokenSet_46_()
	{
		long[] data = { 6917951514984054800L, 12672348168L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_46_ = new BitSet(mk_tokenSet_46_());
	private static long[] mk_tokenSet_47_()
	{
		long[] data = { 6917951514984054800L, 12671823880L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_47_ = new BitSet(mk_tokenSet_47_());
	private static long[] mk_tokenSet_48_()
	{
		long[] data = { 0L, 4295032832L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_48_ = new BitSet(mk_tokenSet_48_());
	
}
}
