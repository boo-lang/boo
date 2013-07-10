// $ANTLR 2.7.5 (20050517): "src/Boo.Lang.Parser/wsaboo.g" -> "WSABooParserBase.cs"$

namespace Boo.Lang.Parser
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
using AstAttribute=Boo.Lang.Compiler.Ast.Attribute;
using Boo.Lang.Parser.Util;
using System.Globalization;

	public 	class WSABooParserBase : antlr.LLkParser
	{
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int ELIST = 4;
		public const int DLIST = 5;
		public const int ESEPARATOR = 6;
		public const int ASSEMBLY_ATTRIBUTE_BEGIN = 7;
		public const int MODULE_ATTRIBUTE_BEGIN = 8;
		public const int ABSTRACT = 9;
		public const int AND = 10;
		public const int AS = 11;
		public const int BREAK = 12;
		public const int CONTINUE = 13;
		public const int CALLABLE = 14;
		public const int CAST = 15;
		public const int CHAR = 16;
		public const int CLASS = 17;
		public const int CONSTRUCTOR = 18;
		public const int DEF = 19;
		public const int DESTRUCTOR = 20;
		public const int DO = 21;
		public const int ELIF = 22;
		public const int ELSE = 23;
		public const int END = 24;
		public const int ENSURE = 25;
		public const int ENUM = 26;
		public const int EVENT = 27;
		public const int EXCEPT = 28;
		public const int FAILURE = 29;
		public const int FINAL = 30;
		public const int FROM = 31;
		public const int FOR = 32;
		public const int FALSE = 33;
		public const int GET = 34;
		public const int GOTO = 35;
		public const int IMPORT = 36;
		public const int INTERFACE = 37;
		public const int INTERNAL = 38;
		public const int IS = 39;
		public const int ISA = 40;
		public const int IF = 41;
		public const int IN = 42;
		public const int NAMESPACE = 43;
		public const int NEW = 44;
		public const int NOT = 45;
		public const int NULL = 46;
		public const int OF = 47;
		public const int OR = 48;
		public const int OVERRIDE = 49;
		public const int PUBLIC = 50;
		public const int PROTECTED = 51;
		public const int PRIVATE = 52;
		public const int RAISE = 53;
		public const int REF = 54;
		public const int RETURN = 55;
		public const int SET = 56;
		public const int SELF = 57;
		public const int SUPER = 58;
		public const int STATIC = 59;
		public const int STRUCT = 60;
		public const int THEN = 61;
		public const int TRY = 62;
		public const int TRANSIENT = 63;
		public const int TRUE = 64;
		public const int TYPEOF = 65;
		public const int UNLESS = 66;
		public const int VIRTUAL = 67;
		public const int PARTIAL = 68;
		public const int WHILE = 69;
		public const int YIELD = 70;
		public const int ID = 71;
		public const int TRIPLE_QUOTED_STRING = 72;
		public const int EOS = 73;
		public const int NEWLINE = 74;
		public const int LPAREN = 75;
		public const int RPAREN = 76;
		public const int DOUBLE_QUOTED_STRING = 77;
		public const int SINGLE_QUOTED_STRING = 78;
		public const int MULTIPLY = 79;
		public const int LBRACK = 80;
		public const int RBRACK = 81;
		public const int ASSIGN = 82;
		public const int SUBTRACT = 83;
		public const int COMMA = 84;
		public const int SPLICE_BEGIN = 85;
		public const int DOT = 86;
		public const int COLON = 87;
		public const int NULLABLE_SUFFIX = 88;
		public const int EXPONENTIATION = 89;
		public const int BITWISE_OR = 90;
		public const int LBRACE = 91;
		public const int RBRACE = 92;
		public const int QQ_BEGIN = 93;
		public const int QQ_END = 94;
		public const int INPLACE_BITWISE_OR = 95;
		public const int INPLACE_EXCLUSIVE_OR = 96;
		public const int INPLACE_BITWISE_AND = 97;
		public const int INPLACE_SHIFT_LEFT = 98;
		public const int INPLACE_SHIFT_RIGHT = 99;
		public const int CMP_OPERATOR = 100;
		public const int GREATER_THAN = 101;
		public const int LESS_THAN = 102;
		public const int ADD = 103;
		public const int EXCLUSIVE_OR = 104;
		public const int DIVISION = 105;
		public const int MODULUS = 106;
		public const int BITWISE_AND = 107;
		public const int SHIFT_LEFT = 108;
		public const int SHIFT_RIGHT = 109;
		public const int LONG = 110;
		public const int INCREMENT = 111;
		public const int DECREMENT = 112;
		public const int ONES_COMPLEMENT = 113;
		public const int INT = 114;
		public const int BACKTICK_QUOTED_STRING = 115;
		public const int RE_LITERAL = 116;
		public const int DOUBLE = 117;
		public const int FLOAT = 118;
		public const int TIMESPAN = 119;
		public const int ID_SUFFIX = 120;
		public const int LINE_CONTINUATION = 121;
		public const int INTERPOLATED_EXPRESSION = 122;
		public const int INTERPOLATED_REFERENCE = 123;
		public const int SL_COMMENT = 124;
		public const int ML_COMMENT = 125;
		public const int WS = 126;
		public const int X_RE_LITERAL = 127;
		public const int DQS_ESC = 128;
		public const int SQS_ESC = 129;
		public const int SESC = 130;
		public const int RE_CHAR = 131;
		public const int X_RE_CHAR = 132;
		public const int RE_ESC = 133;
		public const int DIGIT_GROUP = 134;
		public const int REVERSE_DIGIT_GROUP = 135;
		public const int AT_SYMBOL = 136;
		public const int ID_LETTER = 137;
		public const int DIGIT = 138;
		public const int HEXDIGIT = 139;
		
				
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
	
	static bool IsMethodInvocationExpression(Expression e)
	{
		return NodeType.MethodInvocationExpression == e.NodeType;
	}

	protected bool IsValidMacroArgument(int token)
	{
		return LPAREN != token && LBRACK != token && DOT != token && MULTIPLY != token;
	}
	
	private LexicalInfo ToLexicalInfo(IToken token)
	{
		return SourceLocationFactory.ToLexicalInfo(token);
	}
	
	private MemberReferenceExpression MemberReferenceForToken(Expression target, IToken memberName)
	{
		MemberReferenceExpression mre = new MemberReferenceExpression(ToLexicalInfo(memberName));
		mre.Target = target;
		mre.Name = memberName.getText();
		return mre;	
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
			parse_module(module);
			{
				if ((LA(1)==EOF) && (LA(2)==EOF))
				{
					match(Token.EOF_TYPE);
				}
				else if ((LA(1)==EOF) && (LA(2)==EOF)) {
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
				recover(ex,tokenSet_0_);
			}
			else
			{
				throw ex;
			}
		}
		return module;
	}
	
	protected void parse_module(
		Module module
	) //throws RecognitionException, TokenStreamException
{
		
		
		
		
		try {      // for error handling
			{
				if ((LA(1)==EOF||LA(1)==EOS||LA(1)==NEWLINE) && (tokenSet_1_.member(LA(2))))
				{
					eos();
				}
				else if ((tokenSet_1_.member(LA(1))) && (tokenSet_2_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			docstring(module);
			{
				if ((LA(1)==EOF||LA(1)==EOS||LA(1)==NEWLINE) && (tokenSet_1_.member(LA(2))))
				{
					eos();
				}
				else if ((tokenSet_1_.member(LA(1))) && (tokenSet_2_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
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
				case ASSEMBLY_ATTRIBUTE_BEGIN:
				case MODULE_ATTRIBUTE_BEGIN:
				case ABSTRACT:
				case BREAK:
				case CONTINUE:
				case CALLABLE:
				case CAST:
				case CHAR:
				case CLASS:
				case DEF:
				case ENUM:
				case FINAL:
				case FROM:
				case FOR:
				case FALSE:
				case GOTO:
				case IMPORT:
				case INTERFACE:
				case INTERNAL:
				case IF:
				case NEW:
				case NULL:
				case OVERRIDE:
				case PUBLIC:
				case PROTECTED:
				case PRIVATE:
				case RAISE:
				case RETURN:
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
				case PARTIAL:
				case WHILE:
				case YIELD:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case EOS:
				case NEWLINE:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case LBRACE:
				case QQ_BEGIN:
				case QQ_END:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
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
					if ((LA(1)==FROM||LA(1)==IMPORT))
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
					bool synPredMatched12 = false;
					if ((((LA(1)==ID) && (tokenSet_3_.member(LA(2))))&&(IsValidMacroArgument(LA(2)))))
					{
						int _m12 = mark();
						synPredMatched12 = true;
						inputState.guessing++;
						try {
							{
								match(ID);
								{
									if ((tokenSet_4_.member(LA(1))))
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
							synPredMatched12 = false;
						}
						rewind(_m12);
						inputState.guessing--;
					}
					if ( synPredMatched12 )
					{
						module_macro(module);
					}
					else if ((tokenSet_5_.member(LA(1))) && (tokenSet_6_.member(LA(2)))) {
						type_member(module.Members);
					}
					else
					{
						goto _loop13_breakloop;
					}
					
				}
_loop13_breakloop:				;
			}    // ( ... )*
			globals(module);
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==ASSEMBLY_ATTRIBUTE_BEGIN||LA(1)==MODULE_ATTRIBUTE_BEGIN))
					{
						{
							switch ( LA(1) )
							{
							case ASSEMBLY_ATTRIBUTE_BEGIN:
							{
								assembly_attribute(module);
								break;
							}
							case MODULE_ATTRIBUTE_BEGIN:
							{
								module_attribute(module);
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
					else
					{
						goto _loop16_breakloop;
					}
					
				}
_loop16_breakloop:				;
			}    // ( ... )*
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
	
	protected void eos() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case EOF:
			{
				match(Token.EOF_TYPE);
				break;
			}
			case EOS:
			case NEWLINE:
			{
				{ // ( ... )+
					int _cnt24=0;
					for (;;)
					{
						if ((LA(1)==EOS||LA(1)==NEWLINE) && (tokenSet_8_.member(LA(2))))
						{
							{
								switch ( LA(1) )
								{
								case EOS:
								{
									match(EOS);
									break;
								}
								case NEWLINE:
								{
									match(NEWLINE);
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
							if (_cnt24 >= 1) { goto _loop24_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
						}
						
						_cnt24++;
					}
_loop24_breakloop:					;
				}    // ( ... )+
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
				recover(ex,tokenSet_8_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void docstring(
		Node node
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  doc = null;
		
		try {      // for error handling
			{
				if ((LA(1)==TRIPLE_QUOTED_STRING) && (tokenSet_9_.member(LA(2))))
				{
					doc = LT(1);
					match(TRIPLE_QUOTED_STRING);
					if (0==inputState.guessing)
					{
						node.Documentation = DocStringFormatter.Format(doc.getText());
					}
					{
						if ((LA(1)==EOF||LA(1)==EOS||LA(1)==NEWLINE) && (tokenSet_9_.member(LA(2))))
						{
							eos();
						}
						else if ((tokenSet_9_.member(LA(1))) && (tokenSet_10_.member(LA(2)))) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
				}
				else if ((tokenSet_9_.member(LA(1))) && (tokenSet_10_.member(LA(2)))) {
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
				recover(ex,tokenSet_9_);
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
				
						p = new NamespaceDeclaration(SourceLocationFactory.ToLexicalInfo(t));
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
				recover(ex,tokenSet_11_);
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
		
		
			Import node = null;
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case IMPORT:
				{
					node=import_directive_();
					eos();
					break;
				}
				case FROM:
				{
					node=import_directive_from_();
					eos();
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
				
						if (node != null) container.Imports.Add(node);
					
			}
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
				switch ( LA(1) )
				{
				case FOR:
				{
					f = LT(1);
					match(FOR);
					if (0==inputState.guessing)
					{
						
									ge = new GeneratorExpression(SourceLocationFactory.ToLexicalInfo(f));
									ge.Expression = e;
									e = ge;
								
					}
					generator_expression_body(ge);
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==FOR))
							{
								f2 = LT(1);
								match(FOR);
								if (0==inputState.guessing)
								{
									
													if (null == mge)
													{
														mge = new ExtendedGeneratorExpression(SourceLocationFactory.ToLexicalInfo(f));
														mge.Items.Add(ge);
														e = mge;
													}
													
													ge = new GeneratorExpression(SourceLocationFactory.ToLexicalInfo(f2));
													mge.Items.Add(ge);
												
								}
								generator_expression_body(ge);
							}
							else
							{
								goto _loop385_breakloop;
							}
							
						}
_loop385_breakloop:						;
					}    // ( ... )*
					break;
				}
				case EOF:
				case ESEPARATOR:
				case DEF:
				case DO:
				case IF:
				case UNLESS:
				case WHILE:
				case ID:
				case EOS:
				case NEWLINE:
				case RPAREN:
				case RBRACK:
				case COMMA:
				case COLON:
				case RBRACE:
				case QQ_END:
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
				recover(ex,tokenSet_12_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected void module_macro(
		Module module
	) //throws RecognitionException, TokenStreamException
{
		
		
			Statement s = null;
		
		
		try {      // for error handling
			s=macro_stmt();
			if (0==inputState.guessing)
			{
				module.Globals.Add(s);
			}
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
				recover(ex,tokenSet_14_);
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
			{
				if ((LA(1)==EOF||LA(1)==EOS||LA(1)==NEWLINE) && (tokenSet_15_.member(LA(2))))
				{
					eos();
				}
				else if ((tokenSet_16_.member(LA(1))) && (tokenSet_17_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_18_.member(LA(1))))
					{
						stmt(container.Globals.Statements);
					}
					else
					{
						goto _loop167_breakloop;
					}
					
				}
_loop167_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_19_);
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
		
		
				AstAttribute attr = null;
			
		
		try {      // for error handling
			match(ASSEMBLY_ATTRIBUTE_BEGIN);
			attr=attribute();
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
				recover(ex,tokenSet_20_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void module_attribute(
		Module module
	) //throws RecognitionException, TokenStreamException
{
		
		
				AstAttribute attr = null;
			
		
		try {      // for error handling
			match(MODULE_ATTRIBUTE_BEGIN);
			attr=attribute();
			match(RBRACK);
			if (0==inputState.guessing)
			{
				module.Attributes.Add(attr);
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
				if ((LA(1)==COLON) && (tokenSet_21_.member(LA(2))))
				{
					{
						begin_with_doc(macro);
						macro_block(macro.Body.Statements);
						end(macro.Body);
						if (0==inputState.guessing)
						{
							macro.Annotate("compound" );
						}
					}
				}
				else if ((LA(1)==COLON) && (tokenSet_21_.member(LA(2)))) {
					macro_compound_stmt(macro.Body);
					if (0==inputState.guessing)
					{
						macro.Annotate("compound");
					}
				}
				else if ((tokenSet_22_.member(LA(1)))) {
					{
						{
							switch ( LA(1) )
							{
							case EOF:
							case EOS:
							case NEWLINE:
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
						docstring(macro);
					}
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
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
				recover(ex,tokenSet_23_);
			}
			else
			{
				throw ex;
			}
		}
		return returnValue;
	}
	
	protected Import  import_directive_() //throws RecognitionException, TokenStreamException
{
		Import returnValue;
		
		IToken  imp = null;
		IToken  dqs = null;
		IToken  sqs = null;
		IToken  alias = null;
		
			Expression ns = null;
			IToken id = null;
			returnValue = null;
		
		
		try {      // for error handling
			imp = LT(1);
			match(IMPORT);
			ns=namespace_expression();
			if (0==inputState.guessing)
			{
				if (ns != null) returnValue = new Import(ToLexicalInfo(imp), ns);
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
						
									returnValue.AssemblyReference = new ReferenceExpression(ToLexicalInfo(id), id.getText());
								
					}
					break;
				}
				case EOF:
				case AS:
				case EOS:
				case NEWLINE:
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
						
									returnValue.Alias = new ReferenceExpression(ToLexicalInfo(alias));
									returnValue.Alias.Name = alias.getText();
								
					}
					break;
				}
				case EOF:
				case EOS:
				case NEWLINE:
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
		return returnValue;
	}
	
	protected Import  import_directive_from_() //throws RecognitionException, TokenStreamException
{
		Import returnValue;
		
		IToken  from = null;
		
			Expression ns = null;
			ExpressionCollection names = null;
			returnValue = null;
		
		
		try {      // for error handling
			from = LT(1);
			match(FROM);
			ns=identifier_expression();
			match(IMPORT);
			if (0==inputState.guessing)
			{
				
						var mie = new MethodInvocationExpression(ns);
						names = mie.Arguments;
						returnValue = new Import(ToLexicalInfo(from), mie);
					
			}
			{
				if ((LA(1)==MULTIPLY) && (LA(2)==EOF||LA(2)==EOS||LA(2)==NEWLINE))
				{
					match(MULTIPLY);
					if (0==inputState.guessing)
					{
						returnValue.Expression = ns;
					}
				}
				else if ((tokenSet_24_.member(LA(1))) && (tokenSet_25_.member(LA(2)))) {
					expression_list(names);
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
		return returnValue;
	}
	
	protected Expression  namespace_expression() //throws RecognitionException, TokenStreamException
{
		Expression result;
		
		
			result = null;
			ExpressionCollection names = null;
		
		
		try {      // for error handling
			result=identifier_expression();
			{
				switch ( LA(1) )
				{
				case LPAREN:
				{
					match(LPAREN);
					if (0==inputState.guessing)
					{
						
									var mie = new MethodInvocationExpression(result);
									names = mie.Arguments;
									result = mie;
								
					}
					expression_list(names);
					match(RPAREN);
					break;
				}
				case EOF:
				case AS:
				case FROM:
				case EOS:
				case NEWLINE:
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
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	protected ReferenceExpression  identifier_expression() //throws RecognitionException, TokenStreamException
{
		ReferenceExpression result;
		
		
			result = null;
			IToken id = null;
		
		
		try {      // for error handling
			id=identifier();
			if (0==inputState.guessing)
			{
				if (id != null) result = new ReferenceExpression(ToLexicalInfo(id), id.getText());
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
		return result;
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
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case SPLICE_BEGIN:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
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
								goto _loop586_breakloop;
							}
							
						}
_loop586_breakloop:						;
					}    // ( ... )*
					break;
				}
				case EOF:
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
				case NEWLINE:
				case RPAREN:
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
				recover(ex,tokenSet_28_);
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
					if ((LA(1)==DOT) && (tokenSet_29_.member(LA(2))))
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
						goto _loop598_breakloop;
					}
					
				}
_loop598_breakloop:				;
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
				recover(ex,tokenSet_30_);
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
		
		
		AstAttribute attr = null;
		
		
		try {      // for error handling
			if (0==inputState.guessing)
			{
				_attributes.Clear();
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==LBRACK))
					{
						match(LBRACK);
						{
							switch ( LA(1) )
							{
							case TRANSIENT:
							case ID:
							{
								attr=attribute();
								if (0==inputState.guessing)
								{
									if (attr != null) _attributes.Add(attr);
								}
								{    // ( ... )*
									for (;;)
									{
										if ((LA(1)==COMMA))
										{
											match(COMMA);
											attr=attribute();
											if (0==inputState.guessing)
											{
												if (attr != null) _attributes.Add(attr);
											}
										}
										else
										{
											goto _loop56_breakloop;
										}
										
									}
_loop56_breakloop:									;
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
						{
							switch ( LA(1) )
							{
							case EOF:
							case EOS:
							case NEWLINE:
							{
								eos();
								break;
							}
							case ABSTRACT:
							case AS:
							case CALLABLE:
							case CLASS:
							case DEF:
							case ENUM:
							case EVENT:
							case FINAL:
							case GET:
							case INTERFACE:
							case INTERNAL:
							case NEW:
							case OVERRIDE:
							case PUBLIC:
							case PROTECTED:
							case PRIVATE:
							case REF:
							case SET:
							case SELF:
							case STATIC:
							case STRUCT:
							case TRANSIENT:
							case VIRTUAL:
							case PARTIAL:
							case ID:
							case MULTIPLY:
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
					}
					else
					{
						goto _loop58_breakloop;
					}
					
				}
_loop58_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_31_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void modifiers() //throws RecognitionException, TokenStreamException
{
		
		
		
		
		try {      // for error handling
			if (0==inputState.guessing)
			{
				_modifiers = TypeMemberModifiers.None;
			}
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_32_.member(LA(1))))
					{
						type_member_modifier();
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
				recover(ex,tokenSet_33_);
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
				recover(ex,tokenSet_34_);
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
		IToken  c = null;
		IToken  d = null;
		
				Method m = null;
				TypeReference rt = null;
				TypeReference it = null;
				ExplicitMemberInfo emi = null;
				ParameterDeclarationCollection parameters = null;
				GenericParameterDeclarationCollection genericParameters = null;
				Block body = null;
				StatementCollection statements = null;
				IToken id = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(DEF);
			{
				switch ( LA(1) )
				{
				case EVENT:
				case GET:
				case INTERNAL:
				case PUBLIC:
				case PROTECTED:
				case REF:
				case SET:
				case ID:
				{
					{
						if ((LA(1)==ID) && (LA(2)==DOT))
						{
							emi=explicit_member_info();
						}
						else if ((tokenSet_29_.member(LA(1))) && (LA(2)==LPAREN||LA(2)==LBRACK)) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
					id=member();
					if (0==inputState.guessing)
					{
						
									if (emi != null)
									{
										m = new Method(emi.LexicalInfo);
									}
									else
									{
										m = new Method(SourceLocationFactory.ToLexicalInfo(id));
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
						m = new Constructor(SourceLocationFactory.ToLexicalInfo(c));
					}
					break;
				}
				case DESTRUCTOR:
				{
					d = LT(1);
					match(DESTRUCTOR);
					if (0==inputState.guessing)
					{
						m = new Destructor(SourceLocationFactory.ToLexicalInfo(d));
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
						genericParameters = m.GenericParameters;
						body = m.Body;
						statements = body.Statements;
					
			}
			{
				switch ( LA(1) )
				{
				case LBRACK:
				{
					match(LBRACK);
					{
						switch ( LA(1) )
						{
						case OF:
						{
							match(OF);
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
					generic_parameter_declaration_list(genericParameters);
					match(RBRACK);
					break;
				}
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
			match(LPAREN);
			parameter_declaration_list(parameters);
			match(RPAREN);
			attributes();
			if (0==inputState.guessing)
			{
				AddAttributes(m.ReturnTypeAttributes);
			}
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
				recover(ex,tokenSet_34_);
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
				GenericParameterDeclarationCollection genericParameters = null;
			
		
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
						
						td.LexicalInfo = SourceLocationFactory.ToLexicalInfo(id);
						td.Name = id.getText();
						td.Modifiers = _modifiers;
						AddAttributes(td.Attributes);
						container.Add(td);
						baseTypes = td.BaseTypes;
						members = td.Members;
						genericParameters = td.GenericParameters;
					
			}
			{
				switch ( LA(1) )
				{
				case LBRACK:
				{
					match(LBRACK);
					{
						switch ( LA(1) )
						{
						case OF:
						{
							match(OF);
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
					generic_parameter_declaration_list(genericParameters);
					match(RBRACK);
					break;
				}
				case LPAREN:
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
			{    // ( ... )*
				for (;;)
				{
					switch ( LA(1) )
					{
					case SPLICE_BEGIN:
					{
						splice_type_definition_body(members);
						break;
					}
					case ABSTRACT:
					case CALLABLE:
					case CLASS:
					case DEF:
					case ENUM:
					case EVENT:
					case FINAL:
					case INTERFACE:
					case INTERNAL:
					case NEW:
					case OVERRIDE:
					case PUBLIC:
					case PROTECTED:
					case PRIVATE:
					case SELF:
					case STATIC:
					case STRUCT:
					case TRANSIENT:
					case VIRTUAL:
					case PARTIAL:
					case ID:
					case LBRACK:
					{
						type_definition_member(members);
						break;
					}
					default:
					{
						goto _loop72_breakloop;
					}
					 }
				}
_loop72_breakloop:				;
			}    // ( ... )*
			end(td);
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
	
	protected void interface_definition(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		
				InterfaceDefinition itf = null;
				TypeMemberCollection members = null;
				GenericParameterDeclarationCollection genericParameters = null;
			
		
		try {      // for error handling
			match(INTERFACE);
			id = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
				
						itf = new InterfaceDefinition(SourceLocationFactory.ToLexicalInfo(id));
						itf.Name = id.getText();
						itf.Modifiers = _modifiers;
						AddAttributes(itf.Attributes);
						container.Add(itf);
						members = itf.Members;
						genericParameters = itf.GenericParameters;
					
			}
			{
				switch ( LA(1) )
				{
				case LBRACK:
				{
					match(LBRACK);
					{
						switch ( LA(1) )
						{
						case OF:
						{
							match(OF);
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
					generic_parameter_declaration_list(genericParameters);
					match(RBRACK);
					break;
				}
				case LPAREN:
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
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_35_.member(LA(1))))
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
							case SELF:
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
						goto _loop82_breakloop;
					}
					
				}
_loop82_breakloop:				;
			}    // ( ... )*
			end(itf);
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
	
	protected void enum_definition(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		
				EnumDefinition ed = null;
				TypeMemberCollection members = null;
			
		
		try {      // for error handling
			match(ENUM);
			id = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
				ed = new EnumDefinition(SourceLocationFactory.ToLexicalInfo(id));
			}
			begin_with_doc(ed);
			if (0==inputState.guessing)
			{
				
						ed.Name = id.getText();
						ed.Modifiers = _modifiers;
						AddAttributes(ed.Attributes);
						container.Add(ed);
						members = ed.Members;
					
			}
			{
				{ // ( ... )+
					int _cnt48=0;
					for (;;)
					{
						switch ( LA(1) )
						{
						case ID:
						case LBRACK:
						{
							enum_member(members);
							break;
						}
						case SPLICE_BEGIN:
						{
							splice_type_definition_body(members);
							break;
						}
						default:
						{
							if (_cnt48 >= 1) { goto _loop48_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
						}
						break; }
						_cnt48++;
					}
_loop48_breakloop:					;
				}    // ( ... )+
			}
			end(ed);
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
	
	protected void callable_definition(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		
				CallableDefinition cd = null;
				TypeReference returnType = null;
				GenericParameterDeclarationCollection genericParameters = null;
			
		
		try {      // for error handling
			match(CALLABLE);
			id = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
				
						cd = new CallableDefinition(SourceLocationFactory.ToLexicalInfo(id));
						cd.Name = id.getText();
						cd.Modifiers = _modifiers;
						AddAttributes(cd.Attributes);
						container.Add(cd);
						genericParameters = cd.GenericParameters;
					
			}
			{
				switch ( LA(1) )
				{
				case LBRACK:
				{
					match(LBRACK);
					{
						switch ( LA(1) )
						{
						case OF:
						{
							match(OF);
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
					generic_parameter_declaration_list(genericParameters);
					match(RBRACK);
					break;
				}
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
				case EOF:
				case EOS:
				case NEWLINE:
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
				recover(ex,tokenSet_34_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void generic_parameter_declaration_list(
		GenericParameterDeclarationCollection c
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			generic_parameter_declaration(c);
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						generic_parameter_declaration(c);
					}
					else
					{
						goto _loop203_breakloop;
					}
					
				}
_loop203_breakloop:				;
			}    // ( ... )*
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
				case MULTIPLY:
				case LBRACK:
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
								goto _loop182_breakloop;
							}
							
						}
_loop182_breakloop:						;
					}    // ( ... )*
					break;
				}
				case RPAREN:
				case RBRACK:
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
				c.HasParamArray = variableArguments;
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
	}
	
	protected TypeReference  type_reference() //throws RecognitionException, TokenStreamException
{
		TypeReference tr;
		
		
				tr = null;
				IToken id = null;
				TypeReferenceCollection arguments = null;
				GenericTypeDefinitionReference gtdr = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case SPLICE_BEGIN:
				{
					tr=splice_type_reference();
					break;
				}
				case LPAREN:
				{
					tr=array_type_reference();
					break;
				}
				default:
					bool synPredMatched221 = false;
					if (((LA(1)==CALLABLE) && (LA(2)==LPAREN)))
					{
						int _m221 = mark();
						synPredMatched221 = true;
						inputState.guessing++;
						try {
							{
								match(CALLABLE);
								match(LPAREN);
							}
						}
						catch (RecognitionException)
						{
							synPredMatched221 = false;
						}
						rewind(_m221);
						inputState.guessing--;
					}
					if ( synPredMatched221 )
					{
						{
							tr=callable_type_reference();
						}
					}
					else if ((LA(1)==CALLABLE||LA(1)==CHAR||LA(1)==ID) && (tokenSet_38_.member(LA(2)))) {
						{
							id=type_name();
							{
								if ((LA(1)==LBRACK) && (tokenSet_39_.member(LA(2))))
								{
									{
										match(LBRACK);
										{
											switch ( LA(1) )
											{
											case OF:
											{
												match(OF);
												break;
											}
											case CALLABLE:
											case CHAR:
											case ID:
											case LPAREN:
											case MULTIPLY:
											case SPLICE_BEGIN:
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
											case MULTIPLY:
											{
												{
													match(MULTIPLY);
													if (0==inputState.guessing)
													{
														
																					gtdr = new GenericTypeDefinitionReference(ToLexicalInfo(id));
																					gtdr.Name = id.getText();
																					gtdr.GenericPlaceholders = 1;
																					tr = gtdr;										
																				
													}
													{    // ( ... )*
														for (;;)
														{
															if ((LA(1)==COMMA))
															{
																match(COMMA);
																match(MULTIPLY);
																if (0==inputState.guessing)
																{
																	
																									gtdr.GenericPlaceholders++;
																								
																}
															}
															else
															{
																goto _loop230_breakloop;
															}
															
														}
_loop230_breakloop:														;
													}    // ( ... )*
													match(RBRACK);
												}
												break;
											}
											case CALLABLE:
											case CHAR:
											case ID:
											case LPAREN:
											case SPLICE_BEGIN:
											{
												{
													if (0==inputState.guessing)
													{
														
																					GenericTypeReference gtr = new GenericTypeReference(ToLexicalInfo(id), id.getText());
																					arguments = gtr.GenericArguments;
																					tr = gtr;
																				
													}
													type_reference_list(arguments);
													match(RBRACK);
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
								else if ((LA(1)==OF) && (LA(2)==MULTIPLY)) {
									{
										match(OF);
										match(MULTIPLY);
										if (0==inputState.guessing)
										{
											
																gtdr = new GenericTypeDefinitionReference(ToLexicalInfo(id));
																gtdr.Name = id.getText();
																gtdr.GenericPlaceholders = 1;
																tr = gtdr;
															
										}
									}
								}
								else if ((LA(1)==OF) && (tokenSet_40_.member(LA(2)))) {
									{
										match(OF);
										tr=type_reference();
										if (0==inputState.guessing)
										{
											
																GenericTypeReference gtr = new GenericTypeReference(ToLexicalInfo(id), id.getText());
																gtr.GenericArguments.Add(tr);
																tr = gtr;
															
										}
									}
								}
								else if ((tokenSet_38_.member(LA(1))) && (tokenSet_41_.member(LA(2)))) {
									if (0==inputState.guessing)
									{
										
														SimpleTypeReference str = new SimpleTypeReference(ToLexicalInfo(id));
														str.Name = id.getText();
														tr = str;
													
									}
								}
								else
								{
									throw new NoViableAltException(LT(1), getFilename());
								}
								
							}
							{
								if ((LA(1)==NULLABLE_SUFFIX) && (tokenSet_38_.member(LA(2))))
								{
									match(NULLABLE_SUFFIX);
									if (0==inputState.guessing)
									{
										
														GenericTypeReference ntr = new GenericTypeReference(tr.LexicalInfo, "System.Nullable");
														ntr.GenericArguments.Add(tr);
														tr = ntr;
													
									}
								}
								else if ((tokenSet_38_.member(LA(1))) && (tokenSet_41_.member(LA(2)))) {
								}
								else
								{
									throw new NoViableAltException(LT(1), getFilename());
								}
								
							}
						}
					}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				break; }
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==MULTIPLY) && (tokenSet_38_.member(LA(2))))
					{
						match(MULTIPLY);
						if (0==inputState.guessing)
						{
							tr = CodeFactory.EnumerableTypeReferenceFor(tr);
						}
					}
					else if ((LA(1)==EXPONENTIATION) && (tokenSet_38_.member(LA(2)))) {
						match(EXPONENTIATION);
						if (0==inputState.guessing)
						{
							tr = CodeFactory.EnumerableTypeReferenceFor(CodeFactory.EnumerableTypeReferenceFor(tr));
						}
					}
					else
					{
						goto _loop236_breakloop;
					}
					
				}
_loop236_breakloop:				;
			}    // ( ... )*
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
		return tr;
	}
	
	protected void begin_with_doc(
		Node node
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(COLON);
			{
				if ((LA(1)==EOF||LA(1)==EOS||LA(1)==NEWLINE) && (tokenSet_42_.member(LA(2))))
				{
					eos();
					docstring(node);
				}
				else if ((tokenSet_42_.member(LA(1))) && (tokenSet_43_.member(LA(2)))) {
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
				recover(ex,tokenSet_42_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void enum_member(
		TypeMemberCollection container
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
						if ((LA(1)==SUBTRACT) && (LA(2)==SUBTRACT||LA(2)==LONG||LA(2)==INT))
						{
							match(SUBTRACT);
							if (0==inputState.guessing)
							{
								negative = true;
							}
						}
						else if ((LA(1)==SUBTRACT||LA(1)==LONG||LA(1)==INT) && (tokenSet_44_.member(LA(2)))) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
					initializer=integer_literal();
					break;
				}
				case EOF:
				case EOS:
				case NEWLINE:
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
				
						em = new EnumMember(SourceLocationFactory.ToLexicalInfo(id));
						em.Name = id.getText();
						em.Initializer = initializer;
						if (negative && null != initializer)
						{
							initializer.Value *= -1;
						}
						AddAttributes(em.Attributes);
						container.Add(em);
					
			}
			eos();
			docstring(em);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_45_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void splice_type_definition_body(
		TypeMemberCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  begin = null;
		
			Expression e = null;
		
		
		try {      // for error handling
			begin = LT(1);
			match(SPLICE_BEGIN);
			e=atom();
			eos();
			if (0==inputState.guessing)
			{
				
						container.Add(new SpliceTypeDefinitionBody(e));
					
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
				node.EndSourceLocation = SourceLocationFactory.ToSourceLocation(t);
			}
			{
				if ((LA(1)==EOF||LA(1)==EOS||LA(1)==NEWLINE) && (tokenSet_47_.member(LA(2))))
				{
					eos();
				}
				else if ((tokenSet_47_.member(LA(1))) && (tokenSet_10_.member(LA(2)))) {
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
				recover(ex,tokenSet_47_);
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
		
		IToken  sign = null;
		IToken  i = null;
		IToken  l = null;
		
				string number = null;
				e = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case SUBTRACT:
				{
					sign = LT(1);
					match(SUBTRACT);
					break;
				}
				case LONG:
				case INT:
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
				case INT:
				{
					i = LT(1);
					match(INT);
					if (0==inputState.guessing)
					{
						
									number = sign != null ? sign.getText() + i.getText() : i.getText();
									e = PrimitiveParser.ParseIntegerLiteralExpression(i, number, false);
								
					}
					break;
				}
				case LONG:
				{
					l = LT(1);
					match(LONG);
					if (0==inputState.guessing)
					{
						
									number = sign != null ? sign.getText() + l.getText() : l.getText();
									e = PrimitiveParser.ParseIntegerLiteralExpression(l, number, true);
								
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
				recover(ex,tokenSet_38_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected AstAttribute  attribute() //throws RecognitionException, TokenStreamException
{
		AstAttribute attr;
		
		IToken  t = null;
				
				antlr.IToken id = null;
		attr = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case ID:
				{
					id=identifier();
					break;
				}
				case TRANSIENT:
				{
					t = LT(1);
					match(TRANSIENT);
					if (0==inputState.guessing)
					{
						id=t;
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
				
						attr = new AstAttribute(ToLexicalInfo(id), id.getText());
					
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
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_48_);
			}
			else
			{
				throw ex;
			}
		}
		return attr;
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
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case SPLICE_BEGIN:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
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
								goto _loop590_breakloop;
							}
							
						}
_loop590_breakloop:						;
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
				recover(ex,tokenSet_49_);
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
				case SPLICE_BEGIN:
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
								goto _loop86_breakloop;
							}
							
						}
_loop86_breakloop:						;
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
				recover(ex,tokenSet_50_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected Expression  splice_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  begin = null;
		
			e = null;
		
		
		try {      // for error handling
			begin = LT(1);
			match(SPLICE_BEGIN);
			e=atom();
			if (0==inputState.guessing)
			{
				
						e = new SpliceExpression(SourceLocationFactory.ToLexicalInfo(begin), e);
					
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
		return e;
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
				case SELF:
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
				recover(ex,tokenSet_51_);
			}
			else
			{
				throw ex;
			}
		}
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
				case FALSE:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case LBRACK:
				case SUBTRACT:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INT:
				case BACKTICK_QUOTED_STRING:
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
				case SPLICE_BEGIN:
				{
					e=splice_expression();
					break;
				}
				case DOT:
				{
					e=omitted_member_expression();
					break;
				}
				default:
					bool synPredMatched492 = false;
					if (((LA(1)==CHAR) && (LA(2)==LPAREN)))
					{
						int _m492 = mark();
						synPredMatched492 = true;
						inputState.guessing++;
						try {
							{
								match(CHAR);
								match(LPAREN);
							}
						}
						catch (RecognitionException)
						{
							synPredMatched492 = false;
						}
						rewind(_m492);
						inputState.guessing--;
					}
					if ( synPredMatched492 )
					{
						e=char_literal();
					}
					else if ((LA(1)==CHAR||LA(1)==ID) && (tokenSet_38_.member(LA(2)))) {
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
				recover(ex,tokenSet_38_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
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
				
						e = new Event(SourceLocationFactory.ToLexicalInfo(id), id.getText(), tr);
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
				recover(ex,tokenSet_51_);
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
		
		IToken  id1 = null;
		IToken  s = null;
		IToken  id2 = null;
		
				IToken id = null;
				TypeMember tm = null;
				TypeReference tr = null;
				Property p = null;
				Field field = null;
				ExplicitMemberInfo emi = null;
				Expression initializer = null;
				ParameterDeclarationCollection parameters = null;
			
		
		try {      // for error handling
			bool synPredMatched137 = false;
			if (((LA(1)==SELF||LA(1)==ID) && (tokenSet_52_.member(LA(2)))))
			{
				int _m137 = mark();
				synPredMatched137 = true;
				inputState.guessing++;
				try {
					{
						property_header();
					}
				}
				catch (RecognitionException)
				{
					synPredMatched137 = false;
				}
				rewind(_m137);
				inputState.guessing--;
			}
			if ( synPredMatched137 )
			{
				{
					{
						if ((LA(1)==ID) && (LA(2)==DOT))
						{
							emi=explicit_member_info();
						}
						else if ((LA(1)==SELF||LA(1)==ID) && (tokenSet_53_.member(LA(2)))) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
					{
						switch ( LA(1) )
						{
						case ID:
						{
							id1 = LT(1);
							match(ID);
							if (0==inputState.guessing)
							{
								id=id1;
							}
							break;
						}
						case SELF:
						{
							s = LT(1);
							match(SELF);
							if (0==inputState.guessing)
							{
								id=s;
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
						if (0==inputState.guessing)
						{
							
											if (emi != null)
												p = new Property(emi.LexicalInfo);
											else
												p = new Property(SourceLocationFactory.ToLexicalInfo(id));
											p.Name = id.getText();
											p.ExplicitInfo = emi;
											AddAttributes(p.Attributes);
											parameters = p.Parameters;
										
						}
						{
							switch ( LA(1) )
							{
							case LPAREN:
							case LBRACK:
							{
								{
									switch ( LA(1) )
									{
									case LBRACK:
									{
										match(LBRACK);
										break;
									}
									case LPAREN:
									{
										match(LPAREN);
										break;
									}
									default:
									{
										throw new NoViableAltException(LT(1), getFilename());
									}
									 }
								}
								parameter_declaration_list(parameters);
								{
									switch ( LA(1) )
									{
									case RBRACK:
									{
										match(RBRACK);
										break;
									}
									case RPAREN:
									{
										match(RPAREN);
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
							int _cnt147=0;
							for (;;)
							{
								if ((tokenSet_54_.member(LA(1))))
								{
									property_accessor(p);
								}
								else
								{
									if (_cnt147 >= 1) { goto _loop147_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
								}
								
								_cnt147++;
							}
_loop147_breakloop:							;
						}    // ( ... )+
						end(p);
					}
				}
				if (0==inputState.guessing)
				{
					container.Add(tm);
				}
			}
			else if ((LA(1)==ID) && (tokenSet_55_.member(LA(2)))) {
				{
					id2 = LT(1);
					match(ID);
					if (0==inputState.guessing)
					{
						
									tm = field = new Field(SourceLocationFactory.ToLexicalInfo(id2));
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
							case EOF:
							case EOS:
							case NEWLINE:
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
							case EOF:
							case EOS:
							case NEWLINE:
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
				recover(ex,tokenSet_51_);
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
		
		
				Method m = null;
				TypeReference rt = null;
				IToken id = null;
			
		
		try {      // for error handling
			match(DEF);
			id=member();
			if (0==inputState.guessing)
			{
				
						m = new Method(SourceLocationFactory.ToLexicalInfo(id));
						m.Name = id.getText();
						AddAttributes(m.Attributes);
						container.Add(m);
					
			}
			{
				switch ( LA(1) )
				{
				case LBRACK:
				{
					{
						match(LBRACK);
						{
							switch ( LA(1) )
							{
							case OF:
							{
								match(OF);
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
						generic_parameter_declaration_list(m.GenericParameters);
						match(RBRACK);
					}
					break;
				}
				case OF:
				{
					{
						match(OF);
						generic_parameter_declaration(m.GenericParameters);
					}
					break;
				}
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
				case EOF:
				case EOS:
				case NEWLINE:
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
				case EOF:
				case EOS:
				case NEWLINE:
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
						{
							switch ( LA(1) )
							{
							case EOF:
							case EOS:
							case NEWLINE:
							{
								eos();
								break;
							}
							case DEF:
							case END:
							case EVENT:
							case SELF:
							case ID:
							case LBRACK:
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
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_56_);
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
		
		IToken  id1 = null;
		IToken  s = null;
		
				IToken id = null;
		Property p = null;
		TypeReference tr = null;
		ParameterDeclarationCollection parameters = null;
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case ID:
				{
					id1 = LT(1);
					match(ID);
					if (0==inputState.guessing)
					{
						id=id1;
					}
					break;
				}
				case SELF:
				{
					s = LT(1);
					match(SELF);
					if (0==inputState.guessing)
					{
						id=s;
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
				
				p = new Property(SourceLocationFactory.ToLexicalInfo(id));
				p.Name = id.getText();
				AddAttributes(p.Attributes);
				container.Add(p);
				parameters = p.Parameters;
				
			}
			{
				switch ( LA(1) )
				{
				case LPAREN:
				case LBRACK:
				{
					{
						switch ( LA(1) )
						{
						case LBRACK:
						{
							match(LBRACK);
							break;
						}
						case LPAREN:
						{
							match(LPAREN);
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					parameter_declaration_list(parameters);
					{
						switch ( LA(1) )
						{
						case RBRACK:
						{
							match(RBRACK);
							break;
						}
						case RPAREN:
						{
							match(RPAREN);
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
				
			}
			begin_with_doc(p);
			{ // ( ... )+
				int _cnt104=0;
				for (;;)
				{
					if ((LA(1)==GET||LA(1)==SET||LA(1)==LBRACK))
					{
						interface_property_accessor(p);
					}
					else
					{
						if (_cnt104 >= 1) { goto _loop104_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt104++;
				}
_loop104_breakloop:				;
			}    // ( ... )+
			end(p);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_56_);
			}
			else
			{
				throw ex;
			}
		}
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
		IToken  ev = null;
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
			case EVENT:
			{
				ev = LT(1);
				match(EVENT);
				if (0==inputState.guessing)
				{
					name=ev;
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
				recover(ex,tokenSet_30_);
			}
			else
			{
				throw ex;
			}
		}
		return name;
	}
	
	protected void generic_parameter_declaration(
		GenericParameterDeclarationCollection c
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		
				GenericParameterDeclaration gpd = null;
			
		
		try {      // for error handling
			id = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
				
						gpd = new GenericParameterDeclaration(SourceLocationFactory.ToLexicalInfo(id));
						gpd.Name = id.getText();
						c.Add(gpd);
					
			}
			{
				if ((LA(1)==LPAREN) && (tokenSet_57_.member(LA(2))))
				{
					match(LPAREN);
					generic_parameter_constraints(gpd);
					match(RPAREN);
				}
				else if ((LA(1)==LPAREN||LA(1)==RBRACK||LA(1)==COMMA) && (tokenSet_58_.member(LA(2)))) {
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
				recover(ex,tokenSet_59_);
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
				recover(ex,tokenSet_60_);
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
							m = p.Getter = new Method(SourceLocationFactory.ToLexicalInfo(gt)); m.Name = "get";
						}
					}
				}
				else if (((LA(1)==SET))&&( null == p.Setter )) {
					{
						st = LT(1);
						match(SET);
						if (0==inputState.guessing)
						{
							m = p.Setter = new Method(SourceLocationFactory.ToLexicalInfo(st)); m.Name = "set";
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
				case EOF:
				case EOS:
				case NEWLINE:
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
				recover(ex,tokenSet_61_);
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
				recover(ex,tokenSet_62_);
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
						
										emi = new ExplicitMemberInfo(SourceLocationFactory.ToLexicalInfo(id));
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
								goto _loop120_breakloop;
							}
							
						}
_loop120_breakloop:						;
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
				recover(ex,tokenSet_63_);
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
				if ((LA(1)==EOF||LA(1)==EOS||LA(1)==NEWLINE) && (tokenSet_64_.member(LA(2))))
				{
					eos();
					docstring(node);
				}
				else if ((tokenSet_64_.member(LA(1))) && (tokenSet_65_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			if (0==inputState.guessing)
			{
				
						block.LexicalInfo = SourceLocationFactory.ToLexicalInfo(begin);
					
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
	}
	
	protected void block(
		StatementCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case EOF:
				case EOS:
				case NEWLINE:
				{
					eos();
					break;
				}
				case ESEPARATOR:
				case BREAK:
				case CONTINUE:
				case CAST:
				case CHAR:
				case ELIF:
				case ELSE:
				case END:
				case ENSURE:
				case EXCEPT:
				case FAILURE:
				case FOR:
				case FALSE:
				case GOTO:
				case IF:
				case NULL:
				case OR:
				case RAISE:
				case RETURN:
				case SELF:
				case SUPER:
				case THEN:
				case TRY:
				case TRUE:
				case TYPEOF:
				case UNLESS:
				case WHILE:
				case YIELD:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
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
					if ((tokenSet_18_.member(LA(1))))
					{
						stmt(container);
					}
					else
					{
						goto _loop171_breakloop;
					}
					
				}
_loop171_breakloop:				;
			}    // ( ... )*
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
	}
	
	protected void property_header() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{
				{
					switch ( LA(1) )
					{
					case ID:
					{
						match(ID);
						break;
					}
					case SELF:
					{
						match(SELF);
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
						if ((LA(1)==DOT))
						{
							match(DOT);
							match(ID);
						}
						else
						{
							goto _loop131_breakloop;
						}
						
					}
_loop131_breakloop:					;
				}    // ( ... )*
			}
			{
				switch ( LA(1) )
				{
				case LBRACK:
				{
					match(LBRACK);
					break;
				}
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
				recover(ex,tokenSet_0_);
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
							
											p.Getter = m = new Method(SourceLocationFactory.ToLexicalInfo(gt));		
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
							
											p.Setter = m = new Method(SourceLocationFactory.ToLexicalInfo(st));
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
				recover(ex,tokenSet_67_);
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
			bool synPredMatched156 = false;
			if (((tokenSet_68_.member(LA(1))) && (tokenSet_69_.member(LA(2)))))
			{
				int _m156 = mark();
				synPredMatched156 = true;
				inputState.guessing++;
				try {
					{
						slicing_expression();
						{
							switch ( LA(1) )
							{
							case COLON:
							{
								match(COLON);
								break;
							}
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
					synPredMatched156 = false;
				}
				rewind(_m156);
				inputState.guessing--;
			}
			if ( synPredMatched156 )
			{
				{
					e=slicing_expression();
					e=method_invocation_block(e);
				}
			}
			else if ((tokenSet_70_.member(LA(1))) && (tokenSet_25_.member(LA(2)))) {
				{
					e=array_or_expression();
					eos();
				}
			}
			else if ((LA(1)==DEF||LA(1)==DO||LA(1)==COLON)) {
				{
					e=callable_expression();
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
				recover(ex,tokenSet_71_);
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
		IToken  oft = null;
		IToken  lparen = null;
		
				e = null;
				SlicingExpression se = null;
				MethodInvocationExpression mce = null;
				TypeReference genericArgument = null;
				TypeReferenceCollection genericArguments = null;
				Expression initializer = null;
			
		
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
							{
								switch ( LA(1) )
								{
								case OF:
								{
									{
										match(OF);
										if (0==inputState.guessing)
										{
											
																	GenericReferenceExpression gre = new GenericReferenceExpression(SourceLocationFactory.ToLexicalInfo(lbrack));
																	gre.Target = e;
																	e = gre;
																	genericArguments = gre.GenericArguments;
																
										}
										type_reference_list(genericArguments);
									}
									break;
								}
								case ESEPARATOR:
								case CAST:
								case CHAR:
								case FALSE:
								case NOT:
								case NULL:
								case SELF:
								case SUPER:
								case TRUE:
								case TYPEOF:
								case ID:
								case TRIPLE_QUOTED_STRING:
								case LPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case MULTIPLY:
								case LBRACK:
								case SUBTRACT:
								case SPLICE_BEGIN:
								case DOT:
								case COLON:
								case LBRACE:
								case QQ_BEGIN:
								case LONG:
								case INCREMENT:
								case DECREMENT:
								case ONES_COMPLEMENT:
								case INT:
								case BACKTICK_QUOTED_STRING:
								case RE_LITERAL:
								case DOUBLE:
								case FLOAT:
								case TIMESPAN:
								{
									if (0==inputState.guessing)
									{
										
															se = new SlicingExpression(SourceLocationFactory.ToLexicalInfo(lbrack));				
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
												goto _loop530_breakloop;
											}
											
										}
_loop530_breakloop:										;
									}    // ( ... )*
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
						break;
					}
					case OF:
					{
						{
							oft = LT(1);
							match(OF);
							genericArgument=type_reference();
							if (0==inputState.guessing)
							{
								
												GenericReferenceExpression gre = new GenericReferenceExpression(SourceLocationFactory.ToLexicalInfo(oft));
												gre.Target = e;
												e = gre;
												gre.GenericArguments.Add(genericArgument);
											
							}
						}
						break;
					}
					case DOT:
					{
						{
							match(DOT);
							{    // ( ... )*
								for (;;)
								{
									if ((LA(1)==NEWLINE))
									{
										match(NEWLINE);
									}
									else
									{
										goto _loop534_breakloop;
									}
									
								}
_loop534_breakloop:								;
							}    // ( ... )*
							e=member_reference_expression(e);
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
								
													mce = new MethodInvocationExpression(SourceLocationFactory.ToLexicalInfo(lparen));
													mce.Target = e;
													e = mce;
												
							}
							{
								switch ( LA(1) )
								{
								case ESEPARATOR:
								case CAST:
								case CHAR:
								case FALSE:
								case NOT:
								case NULL:
								case SELF:
								case SUPER:
								case TRUE:
								case TYPEOF:
								case ID:
								case TRIPLE_QUOTED_STRING:
								case LPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case MULTIPLY:
								case LBRACK:
								case SUBTRACT:
								case SPLICE_BEGIN:
								case DOT:
								case LBRACE:
								case QQ_BEGIN:
								case LONG:
								case INCREMENT:
								case DECREMENT:
								case ONES_COMPLEMENT:
								case INT:
								case BACKTICK_QUOTED_STRING:
								case RE_LITERAL:
								case DOUBLE:
								case FLOAT:
								case TIMESPAN:
								{
									argument(mce);
									{    // ( ... )*
										for (;;)
										{
											if ((LA(1)==COMMA))
											{
												match(COMMA);
												argument(mce);
											}
											else
											{
												goto _loop538_breakloop;
											}
											
										}
_loop538_breakloop:										;
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
							{
								switch ( LA(1) )
								{
								case LBRACE:
								{
									{
										bool synPredMatched542 = false;
										if (((LA(1)==LBRACE) && (tokenSet_72_.member(LA(2)))))
										{
											int _m542 = mark();
											synPredMatched542 = true;
											inputState.guessing++;
											try {
												{
													hash_literal_test();
												}
											}
											catch (RecognitionException)
											{
												synPredMatched542 = false;
											}
											rewind(_m542);
											inputState.guessing--;
										}
										if ( synPredMatched542 )
										{
											initializer=hash_literal();
										}
										else if ((LA(1)==LBRACE) && (tokenSet_72_.member(LA(2)))) {
											initializer=list_initializer();
										}
										else
										{
											throw new NoViableAltException(LT(1), getFilename());
										}
										
									}
									if (0==inputState.guessing)
									{
										e = new CollectionInitializationExpression(e, initializer);
									}
									break;
								}
								case EOF:
								case ESEPARATOR:
								case AND:
								case AS:
								case CAST:
								case DEF:
								case DO:
								case ELSE:
								case FOR:
								case IS:
								case ISA:
								case IF:
								case IN:
								case NOT:
								case OF:
								case OR:
								case UNLESS:
								case WHILE:
								case ID:
								case EOS:
								case NEWLINE:
								case LPAREN:
								case RPAREN:
								case MULTIPLY:
								case LBRACK:
								case RBRACK:
								case ASSIGN:
								case SUBTRACT:
								case COMMA:
								case DOT:
								case COLON:
								case EXPONENTIATION:
								case BITWISE_OR:
								case RBRACE:
								case QQ_END:
								case INPLACE_BITWISE_OR:
								case INPLACE_EXCLUSIVE_OR:
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
								case INCREMENT:
								case DECREMENT:
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
						goto _loop543_breakloop;
					}
					 }
				}
_loop543_breakloop:				;
			}    // ( ... )*
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
		return e;
	}
	
	protected MethodInvocationExpression  method_invocation_block(
		Expression e
	) //throws RecognitionException, TokenStreamException
{
		MethodInvocationExpression mi;
		
		
			Expression block = null;
			mi = null;
		
		
		try {      // for error handling
			block=callable_expression();
			if (0==inputState.guessing)
			{
				
						mi = e as MethodInvocationExpression;
						if (null == mi) 
						{
							mi = new MethodInvocationExpression(e.LexicalInfo, e);
						}
						mi.Arguments.Add(block);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_71_);
			}
			else
			{
				throw ex;
			}
		}
		return mi;
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
						e = new ArrayLiteralExpression(SourceLocationFactory.ToLexicalInfo(c));
					}
				}
				break;
			}
			case ESEPARATOR:
			case CAST:
			case CHAR:
			case FALSE:
			case NOT:
			case NULL:
			case SELF:
			case SUPER:
			case TRUE:
			case TYPEOF:
			case ID:
			case TRIPLE_QUOTED_STRING:
			case LPAREN:
			case DOUBLE_QUOTED_STRING:
			case SINGLE_QUOTED_STRING:
			case MULTIPLY:
			case LBRACK:
			case SUBTRACT:
			case SPLICE_BEGIN:
			case DOT:
			case LBRACE:
			case QQ_BEGIN:
			case LONG:
			case INCREMENT:
			case DECREMENT:
			case ONES_COMPLEMENT:
			case INT:
			case BACKTICK_QUOTED_STRING:
			case RE_LITERAL:
			case DOUBLE:
			case FLOAT:
			case TIMESPAN:
			{
				{
					e=expression();
					{
						switch ( LA(1) )
						{
						case COMMA:
						{
							t = LT(1);
							match(COMMA);
							if (0==inputState.guessing)
							{
													
													tle = new ArrayLiteralExpression(e.LexicalInfo);
													tle.Items.Add(e);		
												
							}
							{
								switch ( LA(1) )
								{
								case ESEPARATOR:
								case CAST:
								case CHAR:
								case FALSE:
								case NOT:
								case NULL:
								case SELF:
								case SUPER:
								case TRUE:
								case TYPEOF:
								case ID:
								case TRIPLE_QUOTED_STRING:
								case LPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case MULTIPLY:
								case LBRACK:
								case SUBTRACT:
								case SPLICE_BEGIN:
								case DOT:
								case LBRACE:
								case QQ_BEGIN:
								case LONG:
								case INCREMENT:
								case DECREMENT:
								case ONES_COMPLEMENT:
								case INT:
								case BACKTICK_QUOTED_STRING:
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
											if ((LA(1)==COMMA) && (tokenSet_4_.member(LA(2))))
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
												goto _loop380_breakloop;
											}
											
										}
_loop380_breakloop:										;
									}    // ( ... )*
									{
										switch ( LA(1) )
										{
										case COMMA:
										{
											match(COMMA);
											break;
										}
										case EOF:
										case DEF:
										case DO:
										case IF:
										case UNLESS:
										case WHILE:
										case EOS:
										case NEWLINE:
										case RPAREN:
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
									break;
								}
								case EOF:
								case DEF:
								case DO:
								case IF:
								case UNLESS:
								case WHILE:
								case EOS:
								case NEWLINE:
								case RPAREN:
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
							if (0==inputState.guessing)
							{
								
													e = tle;
												
							}
							break;
						}
						case EOF:
						case DEF:
						case DO:
						case IF:
						case UNLESS:
						case WHILE:
						case EOS:
						case NEWLINE:
						case RPAREN:
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
				recover(ex,tokenSet_74_);
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
			Block body = null;
			BlockExpression cbe = null;
			TypeReference rt = null;
			IToken anchor = null;
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case COLON:
			{
				{
					if (0==inputState.guessing)
					{
						body = new Block();
					}
					compound_stmt(body);
					if (0==inputState.guessing)
					{
						e = new BlockExpression(body.LexicalInfo, body);
					}
				}
				break;
			}
			case DEF:
			case DO:
			{
				{
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
						
									e = cbe = new BlockExpression(SourceLocationFactory.ToLexicalInfo(anchor));
									body = cbe.Body;
								
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
					compound_stmt(body);
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
				recover(ex,tokenSet_71_);
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
				
							b.LexicalInfo = SourceLocationFactory.ToLexicalInfo(begin);
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
				recover(ex,tokenSet_75_);
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
				case RETURN:
				{
					s=return_stmt();
					break;
				}
				default:
					bool synPredMatched265 = false;
					if (((tokenSet_76_.member(LA(1))) && (tokenSet_77_.member(LA(2)))))
					{
						int _m265 = mark();
						synPredMatched265 = true;
						inputState.guessing++;
						try {
							{
								atom();
								{ // ( ... )+
									int _cnt264=0;
									for (;;)
									{
										if ((LA(1)==NEWLINE))
										{
											match(NEWLINE);
										}
										else
										{
											if (_cnt264 >= 1) { goto _loop264_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
										}
										
										_cnt264++;
									}
_loop264_breakloop:									;
								}    // ( ... )+
								match(DOT);
							}
						}
						catch (RecognitionException)
						{
							synPredMatched265 = false;
						}
						rewind(_m265);
						inputState.guessing--;
					}
					if ( synPredMatched265 )
					{
						{
							s=expression_stmt();
							eos();
						}
					}
					else {
						bool synPredMatched269 = false;
						if ((((LA(1)==ID) && (tokenSet_3_.member(LA(2))))&&(IsValidMacroArgument(LA(2)))))
						{
							int _m269 = mark();
							synPredMatched269 = true;
							inputState.guessing++;
							try {
								{
									match(ID);
									{
										if ((tokenSet_4_.member(LA(1))))
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
								synPredMatched269 = false;
							}
							rewind(_m269);
							inputState.guessing--;
						}
						if ( synPredMatched269 )
						{
							s=macro_stmt();
						}
						else {
							bool synPredMatched273 = false;
							if (((tokenSet_68_.member(LA(1))) && (tokenSet_78_.member(LA(2)))))
							{
								int _m273 = mark();
								synPredMatched273 = true;
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
											case COLON:
											{
												{
													switch ( LA(1) )
													{
													case COLON:
													{
														match(COLON);
														break;
													}
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
									synPredMatched273 = false;
								}
								rewind(_m273);
								inputState.guessing--;
							}
							if ( synPredMatched273 )
							{
								s=assignment_or_method_invocation_with_block_stmt();
							}
							else {
								bool synPredMatched275 = false;
								if (((LA(1)==ID) && (LA(2)==AS||LA(2)==COMMA)))
								{
									int _m275 = mark();
									synPredMatched275 = true;
									inputState.guessing++;
									try {
										{
											declaration();
											match(COMMA);
										}
									}
									catch (RecognitionException)
									{
										synPredMatched275 = false;
									}
									rewind(_m275);
									inputState.guessing--;
								}
								if ( synPredMatched275 )
								{
									s=unpack_stmt();
								}
								else if ((LA(1)==ID) && (LA(2)==AS)) {
									s=declaration_stmt();
								}
								else if ((tokenSet_79_.member(LA(1))) && (tokenSet_77_.member(LA(2)))) {
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
											case ESEPARATOR:
											case CAST:
											case CHAR:
											case FALSE:
											case NULL:
											case SELF:
											case SUPER:
											case TRUE:
											case TYPEOF:
											case ID:
											case TRIPLE_QUOTED_STRING:
											case LPAREN:
											case DOUBLE_QUOTED_STRING:
											case SINGLE_QUOTED_STRING:
											case MULTIPLY:
											case LBRACK:
											case SUBTRACT:
											case SPLICE_BEGIN:
											case DOT:
											case LBRACE:
											case QQ_BEGIN:
											case LONG:
											case INCREMENT:
											case DECREMENT:
											case ONES_COMPLEMENT:
											case INT:
											case BACKTICK_QUOTED_STRING:
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
											case EOF:
											case EOS:
											case NEWLINE:
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
							}}}break; }
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
							recover(ex,tokenSet_80_);
						}
						else
						{
							throw ex;
						}
					}
				}
				
	protected void type_member_modifier() //throws RecognitionException, TokenStreamException
{
		
		
		
		
		try {      // for error handling
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
			case NEW:
			{
				match(NEW);
				if (0==inputState.guessing)
				{
					_modifiers |= TypeMemberModifiers.New;
				}
				break;
			}
			case PARTIAL:
			{
				match(PARTIAL);
				if (0==inputState.guessing)
				{
					_modifiers |= TypeMemberModifiers.Partial;
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
				recover(ex,tokenSet_81_);
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
				recover(ex,tokenSet_40_);
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
							case RBRACK:
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
							case RBRACK:
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
				
						ParameterDeclaration pd = new ParameterDeclaration(SourceLocationFactory.ToLexicalInfo(id));
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
				recover(ex,tokenSet_82_);
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
				
						atr = new ArrayTypeReference(SourceLocationFactory.ToLexicalInfo(lparen));
					
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
				recover(ex,tokenSet_38_);
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
		
		
			bool varArgs = false;
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case CALLABLE:
				case CHAR:
				case REF:
				case ID:
				case LPAREN:
				case MULTIPLY:
				case SPLICE_BEGIN:
				{
					varArgs=callable_parameter_declaration(c);
					{    // ( ... )*
						for (;;)
						{
							if (((LA(1)==COMMA))&&(!varArgs))
							{
								{
									match(COMMA);
									varArgs=callable_parameter_declaration(c);
								}
							}
							else
							{
								goto _loop194_breakloop;
							}
							
						}
_loop194_breakloop:						;
					}    // ( ... )*
					if (0==inputState.guessing)
					{
						c.HasParamArray = varArgs;
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
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_49_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected bool  callable_parameter_declaration(
		ParameterDeclarationCollection c
	) //throws RecognitionException, TokenStreamException
{
		bool varArgs;
		
				
				TypeReference tr = null;
				ParameterModifiers pm = ParameterModifiers.None;
				varArgs = false;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case MULTIPLY:
				{
					{
						match(MULTIPLY);
						if (0==inputState.guessing)
						{
							varArgs=true;
						}
						tr=type_reference();
					}
					break;
				}
				case CALLABLE:
				case CHAR:
				case REF:
				case ID:
				case LPAREN:
				case SPLICE_BEGIN:
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
							case CALLABLE:
							case CHAR:
							case ID:
							case LPAREN:
							case SPLICE_BEGIN:
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
				recover(ex,tokenSet_83_);
			}
			else
			{
				throw ex;
			}
		}
		return varArgs;
	}
	
	protected void generic_parameter_constraints(
		GenericParameterDeclaration gpd
	) //throws RecognitionException, TokenStreamException
{
		
		
				TypeReference tr = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case CLASS:
				{
					match(CLASS);
					if (0==inputState.guessing)
					{
						
									gpd.Constraints |= GenericParameterConstraints.ReferenceType;
								
					}
					break;
				}
				case STRUCT:
				{
					match(STRUCT);
					if (0==inputState.guessing)
					{
						
									gpd.Constraints |= GenericParameterConstraints.ValueType;
								
					}
					break;
				}
				case CONSTRUCTOR:
				{
					match(CONSTRUCTOR);
					if (0==inputState.guessing)
					{
						
									gpd.Constraints |= GenericParameterConstraints.Constructable;
								
					}
					break;
				}
				case CALLABLE:
				case CHAR:
				case ID:
				case LPAREN:
				case SPLICE_BEGIN:
				{
					tr=type_reference();
					if (0==inputState.guessing)
					{
						
									gpd.BaseTypes.Add(tr);
								
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
				case COMMA:
				{
					match(COMMA);
					generic_parameter_constraints(gpd);
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
				recover(ex,tokenSet_49_);
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
				
						ctr = new CallableTypeReference(SourceLocationFactory.ToLexicalInfo(c));
						parameters = ctr.Parameters;
					
			}
			callable_parameter_declaration_list(parameters);
			match(RPAREN);
			{
				if ((LA(1)==AS) && (tokenSet_40_.member(LA(2))))
				{
					match(AS);
					tr=type_reference();
					if (0==inputState.guessing)
					{
						
								ctr.ReturnType = tr; 
								
					}
				}
				else if ((tokenSet_38_.member(LA(1))) && (tokenSet_41_.member(LA(2)))) {
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
				recover(ex,tokenSet_38_);
			}
			else
			{
				throw ex;
			}
		}
		return ctr;
	}
	
	protected void type_reference_list(
		TypeReferenceCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		
				TypeReference tr = null;
			
		
		try {      // for error handling
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
						goto _loop216_breakloop;
					}
					
				}
_loop216_breakloop:				;
			}    // ( ... )*
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
	
	protected SpliceTypeReference  splice_type_reference() //throws RecognitionException, TokenStreamException
{
		SpliceTypeReference tr;
		
		IToken  begin = null;
		
			tr = null;
			Expression e = null;
		
		
		try {      // for error handling
			begin = LT(1);
			match(SPLICE_BEGIN);
			e=atom();
			if (0==inputState.guessing)
			{
				
						tr = new SpliceTypeReference(SourceLocationFactory.ToLexicalInfo(begin), e);
					
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
		return tr;
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
				recover(ex,tokenSet_38_);
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
						macro.LexicalInfo = SourceLocationFactory.ToLexicalInfo(id);		
						returnValue = macro;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_84_);
			}
			else
			{
				throw ex;
			}
		}
		return returnValue;
	}
	
	protected void macro_block(
		StatementCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case EOF:
				case EOS:
				case NEWLINE:
				{
					eos();
					break;
				}
				case ESEPARATOR:
				case ABSTRACT:
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
				case GOTO:
				case INTERFACE:
				case INTERNAL:
				case IF:
				case NEW:
				case NULL:
				case OVERRIDE:
				case PUBLIC:
				case PROTECTED:
				case PRIVATE:
				case RAISE:
				case RETURN:
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
				case PARTIAL:
				case WHILE:
				case YIELD:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
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
			{ // ( ... )+
				int _cnt250=0;
				for (;;)
				{
					if ((tokenSet_18_.member(LA(1))) && (tokenSet_85_.member(LA(2))))
					{
						stmt(container);
					}
					else if ((tokenSet_5_.member(LA(1))) && (tokenSet_6_.member(LA(2)))) {
						type_member_stmt(container);
					}
					else
					{
						if (_cnt250 >= 1) { goto _loop250_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt250++;
				}
_loop250_breakloop:				;
			}    // ( ... )+
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_86_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void type_member_stmt(
		StatementCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		
			TypeMemberCollection members = new TypeMemberCollection();
		
		
		try {      // for error handling
			type_member(members);
			if (0==inputState.guessing)
			{
				
						container.Add(new TypeMemberStatement(members[0]));
					
			}
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
	}
	
	protected void macro_compound_stmt(
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
			macro_block(statements);
			end(b);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_23_);
			}
			else
			{
				throw ex;
			}
		}
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
				StatementModifierType type = StatementModifierType.None;
			
		
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
				
						m = new StatementModifier(SourceLocationFactory.ToLexicalInfo(t));
						m.Type = type;
						m.Condition = e;
					
			}
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
				
						stmt = new GotoStatement(SourceLocationFactory.ToLexicalInfo(token),
									new ReferenceExpression(SourceLocationFactory.ToLexicalInfo(label), label.getText()));
					
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
				
						stmt = new LabelStatement(SourceLocationFactory.ToLexicalInfo(token), label.getText());
					
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
		return stmt;
	}
	
	protected ForStatement  for_stmt() //throws RecognitionException, TokenStreamException
{
		ForStatement fs;
		
		IToken  f = null;
		IToken  or = null;
		IToken  et = null;
		
				fs = null;
				Expression iterator = null;
				DeclarationCollection declarations = null;
				Block body = null;
				Block lastBlock = null;
			
		
		try {      // for error handling
			f = LT(1);
			match(FOR);
			if (0==inputState.guessing)
			{
				
						fs = new ForStatement(SourceLocationFactory.ToLexicalInfo(f));
						declarations = fs.Declarations;
						lastBlock = body = fs.Block;
					
			}
			declaration_list(declarations);
			match(IN);
			iterator=array_or_expression();
			if (0==inputState.guessing)
			{
				fs.Iterator = iterator;
			}
			begin();
			block(body.Statements);
			{
				switch ( LA(1) )
				{
				case OR:
				{
					or = LT(1);
					match(OR);
					if (0==inputState.guessing)
					{
						lastBlock = fs.OrBlock = new Block(SourceLocationFactory.ToLexicalInfo(or));
					}
					begin();
					block(fs.OrBlock.Statements);
					break;
				}
				case END:
				case THEN:
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
				case THEN:
				{
					et = LT(1);
					match(THEN);
					if (0==inputState.guessing)
					{
						lastBlock = fs.ThenBlock = new Block(SourceLocationFactory.ToLexicalInfo(et));
					}
					begin();
					block(fs.ThenBlock.Statements);
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
			end(lastBlock);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_80_);
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
		IToken  or = null;
		IToken  et = null;
		
				ws = null;
				Expression e = null;
				Block lastBlock = null;
			
		
		try {      // for error handling
			w = LT(1);
			match(WHILE);
			e=expression();
			if (0==inputState.guessing)
			{
				
						ws = new WhileStatement(SourceLocationFactory.ToLexicalInfo(w));
						ws.Condition = e;
						lastBlock = ws.Block;
					
			}
			begin();
			block(ws.Block.Statements);
			{
				switch ( LA(1) )
				{
				case OR:
				{
					or = LT(1);
					match(OR);
					if (0==inputState.guessing)
					{
						lastBlock = ws.OrBlock = new Block(SourceLocationFactory.ToLexicalInfo(or));
					}
					begin();
					block(ws.OrBlock.Statements);
					break;
				}
				case END:
				case THEN:
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
				case THEN:
				{
					et = LT(1);
					match(THEN);
					if (0==inputState.guessing)
					{
						lastBlock = ws.ThenBlock = new Block(SourceLocationFactory.ToLexicalInfo(et));
					}
					begin();
					block(ws.ThenBlock.Statements);
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
			end(lastBlock);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_80_);
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
				Block lastBlock = null;
			
		
		try {      // for error handling
			it = LT(1);
			match(IF);
			e=expression();
			if (0==inputState.guessing)
			{
				
						returnValue = s = new IfStatement(SourceLocationFactory.ToLexicalInfo(it));
						s.Condition = e;
						lastBlock = s.TrueBlock = new Block();
					
			}
			begin();
			block(s.TrueBlock.Statements);
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
										
										IfStatement elif = new IfStatement(SourceLocationFactory.ToLexicalInfo(ei));
										lastBlock = elif.TrueBlock = new Block();
										elif.Condition = e;
										
										s.FalseBlock.Add(elif);
										s = elif;
									
						}
						begin();
						block(s.TrueBlock.Statements);
					}
					else
					{
						goto _loop363_breakloop;
					}
					
				}
_loop363_breakloop:				;
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
						lastBlock = s.FalseBlock = new Block(SourceLocationFactory.ToLexicalInfo(et));
					}
					begin();
					block(s.FalseBlock.Statements);
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
			end(lastBlock);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_80_);
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
				
						us = new UnlessStatement(SourceLocationFactory.ToLexicalInfo(u));
						us.Condition = condition;
					
			}
			compound_stmt(us.Block);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_80_);
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
		IToken  ftoken = null;
		IToken  etoken = null;
		
				s = null;		
				Block eblock = null;
				Block lastBlock = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(TRY);
			if (0==inputState.guessing)
			{
				s = new TryStatement(SourceLocationFactory.ToLexicalInfo(t));
			}
			begin();
			if (0==inputState.guessing)
			{
				s.ProtectedBlock = new Block();
			}
			block(s.ProtectedBlock.Statements);
			if (0==inputState.guessing)
			{
				lastBlock = s.ProtectedBlock;
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==EXCEPT))
					{
						lastBlock=exception_handler(s);
					}
					else
					{
						goto _loop320_breakloop;
					}
					
				}
_loop320_breakloop:				;
			}    // ( ... )*
			{
				switch ( LA(1) )
				{
				case FAILURE:
				{
					ftoken = LT(1);
					match(FAILURE);
					if (0==inputState.guessing)
					{
						eblock = new Block(SourceLocationFactory.ToLexicalInfo(ftoken));
					}
					begin();
					block(eblock.Statements);
					if (0==inputState.guessing)
					{
						s.FailureBlock = lastBlock = eblock;
					}
					break;
				}
				case END:
				case ENSURE:
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
						eblock = new Block(SourceLocationFactory.ToLexicalInfo(etoken));
					}
					begin();
					block(eblock.Statements);
					if (0==inputState.guessing)
					{
						s.EnsureBlock = lastBlock = eblock;
					}
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
			end(lastBlock);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_80_);
			}
			else
			{
				throw ex;
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
				recover(ex,tokenSet_22_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
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
				case COLON:
				{
					{
						lhs=method_invocation_block(lhs);
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
								token = op; binaryOperator = OperatorParser.ParseAssignment(op.getText());
							}
							{
								switch ( LA(1) )
								{
								case DEF:
								case DO:
								case COLON:
								{
									rhs=callable_expression();
									break;
								}
								case ESEPARATOR:
								case CAST:
								case CHAR:
								case FALSE:
								case NOT:
								case NULL:
								case SELF:
								case SUPER:
								case TRUE:
								case TYPEOF:
								case ID:
								case TRIPLE_QUOTED_STRING:
								case LPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case MULTIPLY:
								case LBRACK:
								case SUBTRACT:
								case COMMA:
								case SPLICE_BEGIN:
								case DOT:
								case LBRACE:
								case QQ_BEGIN:
								case LONG:
								case INCREMENT:
								case DECREMENT:
								case ONES_COMPLEMENT:
								case INT:
								case BACKTICK_QUOTED_STRING:
								case RE_LITERAL:
								case DOUBLE:
								case FLOAT:
								case TIMESPAN:
								{
									{
										rhs=array_or_expression();
										{
											switch ( LA(1) )
											{
											case DEF:
											case DO:
											case COLON:
											{
												rhs=method_invocation_block(rhs);
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
											case EOF:
											case EOS:
											case NEWLINE:
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
									break;
								}
								default:
								{
									throw new NoViableAltException(LT(1), getFilename());
								}
								 }
							}
						}
						if (0==inputState.guessing)
						{
							
											stmt = new ExpressionStatement(
													new BinaryExpression(SourceLocationFactory.ToLexicalInfo(token),
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
				recover(ex,tokenSet_80_);
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
				case ESEPARATOR:
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case COMMA:
				case SPLICE_BEGIN:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					{
						e=array_or_expression();
						{
							switch ( LA(1) )
							{
							case DEF:
							case DO:
							case COLON:
							{
								e=method_invocation_block(e);
								break;
							}
							case EOF:
							case IF:
							case UNLESS:
							case WHILE:
							case EOS:
							case NEWLINE:
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
										case EOF:
										case EOS:
										case NEWLINE:
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
					break;
				}
				case DEF:
				case DO:
				case COLON:
				{
					{
						e=callable_expression();
					}
					break;
				}
				case EOF:
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
				case NEWLINE:
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
							case EOF:
							case EOS:
							case NEWLINE:
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
				
						s = new ReturnStatement(SourceLocationFactory.ToLexicalInfo(r));
						s.Modifier = modifier;
						s.Expression = e;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_80_);
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
				
						d = new Declaration(SourceLocationFactory.ToLexicalInfo(id));
						d.Name = id.getText();
						d.Type = tr;
					
			}
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
				case EOF:
				case EOS:
				case NEWLINE:
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
				recover(ex,tokenSet_80_);
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
				case EOF:
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
				case NEWLINE:
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
							case EOF:
							case EOS:
							case NEWLINE:
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
				
						Declaration d = new Declaration(SourceLocationFactory.ToLexicalInfo(id));
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
				recover(ex,tokenSet_80_);
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
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case COMMA:
				case SPLICE_BEGIN:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					e=array_or_expression();
					break;
				}
				case EOF:
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
				case NEWLINE:
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
				
						s = new YieldStatement(SourceLocationFactory.ToLexicalInfo(yt));
						s.Expression = e;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_84_);
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
				s = new BreakStatement(SourceLocationFactory.ToLexicalInfo(b));
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
				s = new ContinueStatement(SourceLocationFactory.ToLexicalInfo(c));
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
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case SPLICE_BEGIN:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					e=expression();
					break;
				}
				case EOF:
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
				case NEWLINE:
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
				
						s = new RaiseStatement(SourceLocationFactory.ToLexicalInfo(t));
						s.Exception = e;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_84_);
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
						if ((LA(1)==OR))
						{
							ot = LT(1);
							match(OR);
							r=boolean_term();
							if (0==inputState.guessing)
							{
								
												BinaryExpression be = new BinaryExpression(SourceLocationFactory.ToLexicalInfo(ot));
												be.Operator = BinaryOperatorType.Or;
												be.Left = e;
												be.Right = r;
												e = be;
											
							}
						}
						else
						{
							goto _loop391_breakloop;
						}
						
					}
_loop391_breakloop:					;
				}    // ( ... )*
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
	
	protected Expression  callable_or_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		
				e = null;
			
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case DEF:
			case DO:
			case COLON:
			{
				e=callable_expression();
				break;
			}
			case ESEPARATOR:
			case CAST:
			case CHAR:
			case FALSE:
			case NOT:
			case NULL:
			case SELF:
			case SUPER:
			case TRUE:
			case TYPEOF:
			case ID:
			case TRIPLE_QUOTED_STRING:
			case LPAREN:
			case DOUBLE_QUOTED_STRING:
			case SINGLE_QUOTED_STRING:
			case MULTIPLY:
			case LBRACK:
			case SUBTRACT:
			case COMMA:
			case SPLICE_BEGIN:
			case DOT:
			case LBRACE:
			case QQ_BEGIN:
			case LONG:
			case INCREMENT:
			case DECREMENT:
			case ONES_COMPLEMENT:
			case INT:
			case BACKTICK_QUOTED_STRING:
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
				recover(ex,tokenSet_0_);
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
						goto _loop288_breakloop;
					}
					
				}
_loop288_breakloop:				;
			}    // ( ... )*
			match(BITWISE_OR);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_0_);
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
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case RAISE:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case YIELD:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case COMMA:
				case SPLICE_BEGIN:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
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
								bool synPredMatched294 = false;
								if (((LA(1)==ID) && (LA(2)==AS||LA(2)==COMMA)))
								{
									int _m294 = mark();
									synPredMatched294 = true;
									inputState.guessing++;
									try {
										{
											declaration();
											match(COMMA);
										}
									}
									catch (RecognitionException)
									{
										synPredMatched294 = false;
									}
									rewind(_m294);
									inputState.guessing--;
								}
								if ( synPredMatched294 )
								{
									stmt=unpack();
								}
								else if (((LA(1)==ID) && (tokenSet_91_.member(LA(2))))&&(IsValidMacroArgument(LA(2)))) {
									stmt=closure_macro_stmt();
								}
								else if ((tokenSet_70_.member(LA(1))) && (tokenSet_25_.member(LA(2)))) {
									stmt=closure_expression_stmt();
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
							case EOF:
							case EOS:
							case NEWLINE:
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
				recover(ex,tokenSet_92_);
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
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case COMMA:
				case SPLICE_BEGIN:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					e=array_or_expression();
					break;
				}
				case EOF:
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
				case NEWLINE:
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
				case EOF:
				case EOS:
				case NEWLINE:
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
				
						s = new ReturnStatement(SourceLocationFactory.ToLexicalInfo(r));
						s.Modifier = modifier;
						s.Expression = e;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_92_);
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
						s.LexicalInfo = SourceLocationFactory.ToLexicalInfo(t);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_84_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
	}
	
	protected Statement  closure_expression_stmt() //throws RecognitionException, TokenStreamException
{
		Statement s;
		
		
			s = null;
			Expression e = null;
		
		
		try {      // for error handling
			e=array_or_expression();
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
				recover(ex,tokenSet_84_);
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
				BlockExpression cbe = null;
				ParameterDeclarationCollection parameters = null;
				Block body = null;
			
		
		try {      // for error handling
			anchorBegin = LT(1);
			match(LBRACE);
			if (0==inputState.guessing)
			{
				
							e = cbe = new BlockExpression(SourceLocationFactory.ToLexicalInfo(anchorBegin));
							cbe.Annotate("inline");
							parameters = cbe.Parameters;
							body = cbe.Body;
						
			}
			{
				bool synPredMatched300 = false;
				if (((tokenSet_93_.member(LA(1))) && (tokenSet_94_.member(LA(2)))))
				{
					int _m300 = mark();
					synPredMatched300 = true;
					inputState.guessing++;
					try {
						{
							closure_parameters_test();
						}
					}
					catch (RecognitionException)
					{
						synPredMatched300 = false;
					}
					rewind(_m300);
					inputState.guessing--;
				}
				if ( synPredMatched300 )
				{
					{
						parameter_declaration_list(parameters);
						match(BITWISE_OR);
					}
				}
				else if ((tokenSet_95_.member(LA(1))) && (tokenSet_25_.member(LA(2)))) {
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
						if ((LA(1)==EOF||LA(1)==EOS||LA(1)==NEWLINE))
						{
							eos();
							{
								switch ( LA(1) )
								{
								case ESEPARATOR:
								case CAST:
								case CHAR:
								case FALSE:
								case NOT:
								case NULL:
								case RAISE:
								case RETURN:
								case SELF:
								case SUPER:
								case TRUE:
								case TYPEOF:
								case YIELD:
								case ID:
								case TRIPLE_QUOTED_STRING:
								case LPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case MULTIPLY:
								case LBRACK:
								case SUBTRACT:
								case COMMA:
								case SPLICE_BEGIN:
								case DOT:
								case LBRACE:
								case QQ_BEGIN:
								case LONG:
								case INCREMENT:
								case DECREMENT:
								case ONES_COMPLEMENT:
								case INT:
								case BACKTICK_QUOTED_STRING:
								case RE_LITERAL:
								case DOUBLE:
								case FLOAT:
								case TIMESPAN:
								{
									internal_closure_stmt(body);
									break;
								}
								case EOF:
								case EOS:
								case NEWLINE:
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
							goto _loop305_breakloop;
						}
						
					}
_loop305_breakloop:					;
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
				recover(ex,tokenSet_38_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected Block  exception_handler(
		TryStatement t
	) //throws RecognitionException, TokenStreamException
{
		Block lastBlock;
		
		IToken  c = null;
		IToken  x = null;
		IToken  u = null;
		
				ExceptionHandler eh = null;		
				TypeReference tr = null;
				Expression e = null;
				lastBlock = null;
			
		
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
					break;
				}
				case AS:
				case IF:
				case UNLESS:
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
				case IF:
				case UNLESS:
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
				case IF:
				case UNLESS:
				{
					{
						switch ( LA(1) )
						{
						case IF:
						{
							match(IF);
							break;
						}
						case UNLESS:
						{
							u = LT(1);
							match(UNLESS);
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					e=expression();
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
			if (0==inputState.guessing)
			{
				
						eh = new ExceptionHandler(SourceLocationFactory.ToLexicalInfo(c));
						
						eh.Declaration = new Declaration();
						eh.Declaration.Type = tr;
						
						if (x != null)
						{
							eh.Declaration.LexicalInfo = SourceLocationFactory.ToLexicalInfo(x);
							eh.Declaration.Name = x.getText();		
						}
						else
						{
							eh.Declaration.Name = null;
							eh.Flags |= ExceptionHandlerFlags.Anonymous;
						}
						if (tr != null)
						{
							eh.Declaration.LexicalInfo = tr.LexicalInfo;
						}
						else if (x != null)
						{
							eh.Declaration.LexicalInfo = eh.LexicalInfo;
						}
						if(tr == null)
						{
							eh.Flags |= ExceptionHandlerFlags.Untyped;
						}
						if (e != null)
						{
							if(u != null)
							{
								UnaryExpression not = new UnaryExpression(SourceLocationFactory.ToLexicalInfo(u));
								not.Operator = UnaryOperatorType.LogicalNot;
								not.Operand = e;
								e = not;
							}
							eh.FilterCondition = e;
							eh.Flags |= ExceptionHandlerFlags.Filter;
						}
						eh.Block = new Block(SourceLocationFactory.ToLexicalInfo(c));
					
			}
			block(eh.Block.Statements);
			if (0==inputState.guessing)
			{
				
						lastBlock = eh.Block;
						t.ExceptionHandlers.Add(eh);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_96_);
			}
			else
			{
				throw ex;
			}
		}
		return lastBlock;
	}
	
	protected Expression  assignment_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  op = null;
		IToken  ipbo = null;
		IToken  ipxo = null;
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
				switch ( LA(1) )
				{
				case ASSIGN:
				case INPLACE_BITWISE_OR:
				case INPLACE_EXCLUSIVE_OR:
				case INPLACE_BITWISE_AND:
				case INPLACE_SHIFT_LEFT:
				case INPLACE_SHIFT_RIGHT:
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
														binaryOperator = OperatorParser.ParseAssignment(op.getText());
													
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
						case INPLACE_EXCLUSIVE_OR:
						{
							{
								ipxo = LT(1);
								match(INPLACE_EXCLUSIVE_OR);
								if (0==inputState.guessing)
								{
									
														token = ipxo;
														binaryOperator = BinaryOperatorType.InPlaceExclusiveOr;
													
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
						
									BinaryExpression be = new BinaryExpression(SourceLocationFactory.ToLexicalInfo(token));
									be.Operator = binaryOperator;
									be.Left = e;
									be.Right = r;
									e = be;
								
					}
					break;
				}
				case EOF:
				case ESEPARATOR:
				case AND:
				case DEF:
				case DO:
				case ELSE:
				case FOR:
				case IF:
				case OR:
				case UNLESS:
				case WHILE:
				case ID:
				case EOS:
				case NEWLINE:
				case RPAREN:
				case RBRACK:
				case COMMA:
				case COLON:
				case RBRACE:
				case QQ_END:
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
				recover(ex,tokenSet_97_);
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
						goto _loop371_breakloop;
					}
					
				}
_loop371_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_98_);
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
				if ((LA(1)==IF||LA(1)==UNLESS||LA(1)==WHILE) && (tokenSet_4_.member(LA(2))))
				{
					filter=stmt_modifier();
					if (0==inputState.guessing)
					{
						ge.Filter = filter;
					}
				}
				else if ((tokenSet_88_.member(LA(1))) && (tokenSet_99_.member(LA(2)))) {
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
				recover(ex,tokenSet_88_);
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
					if ((LA(1)==AND))
					{
						at = LT(1);
						match(AND);
						r=not_expression();
						if (0==inputState.guessing)
						{
							
										BinaryExpression be = new BinaryExpression(SourceLocationFactory.ToLexicalInfo(at));
										be.Operator = BinaryOperatorType.And;
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
				recover(ex,tokenSet_100_);
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
				case CAST:
				case CHAR:
				case FALSE:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case SPLICE_BEGIN:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
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
							UnaryExpression ue = new UnaryExpression(SourceLocationFactory.ToLexicalInfo(nt));
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
				recover(ex,tokenSet_97_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	public QuasiquoteExpression  ast_literal_expression() //throws RecognitionException, TokenStreamException
{
		QuasiquoteExpression e;
		
		IToken  begin = null;
		IToken  end = null;
		
			Node node = null;
			e = null;
		
		
		try {      // for error handling
			begin = LT(1);
			match(QQ_BEGIN);
			if (0==inputState.guessing)
			{
				e = new QuasiquoteExpression(SourceLocationFactory.ToLexicalInfo(begin));
			}
			{
				bool synPredMatched399 = false;
				if (((tokenSet_4_.member(LA(1))) && (tokenSet_25_.member(LA(2)))))
				{
					int _m399 = mark();
					synPredMatched399 = true;
					inputState.guessing++;
					try {
						{
							expression();
							match(QQ_END);
						}
					}
					catch (RecognitionException)
					{
						synPredMatched399 = false;
					}
					rewind(_m399);
					inputState.guessing--;
				}
				if ( synPredMatched399 )
				{
					node=expression();
					if (0==inputState.guessing)
					{
						e.Node = node;
					}
				}
				else if ((tokenSet_101_.member(LA(1))) && (tokenSet_2_.member(LA(2)))) {
					{
						{
							if ((LA(1)==EOF||LA(1)==EOS||LA(1)==NEWLINE) && (tokenSet_101_.member(LA(2))))
							{
								eos();
							}
							else if ((tokenSet_101_.member(LA(1))) && (tokenSet_2_.member(LA(2)))) {
							}
							else
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							
						}
						ast_literal_block(e);
					}
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			end = LT(1);
			match(QQ_END);
			if (0==inputState.guessing)
			{
				e.EndSourceLocation = SourceLocationFactory.ToSourceLocation(end);
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
		return e;
	}
	
	public void ast_literal_block(
		QuasiquoteExpression e
	) //throws RecognitionException, TokenStreamException
{
		
		
			// TODO: either cache or construct these objects on demand
			TypeMemberCollection collection = new TypeMemberCollection();
			Block b = new Block();
			StatementCollection statements = b.Statements;
			Node node = null;
		
		
		try {      // for error handling
			bool synPredMatched405 = false;
			if (((tokenSet_1_.member(LA(1))) && (tokenSet_2_.member(LA(2)))))
			{
				int _m405 = mark();
				synPredMatched405 = true;
				inputState.guessing++;
				try {
					{
						ast_literal_module_prediction();
					}
				}
				catch (RecognitionException)
				{
					synPredMatched405 = false;
				}
				rewind(_m405);
				inputState.guessing--;
			}
			if ( synPredMatched405 )
			{
				{
					ast_literal_module(e);
				}
			}
			else {
				bool synPredMatched414 = false;
				if (((tokenSet_102_.member(LA(1))) && (tokenSet_103_.member(LA(2)))))
				{
					int _m414 = mark();
					synPredMatched414 = true;
					inputState.guessing++;
					try {
						{
							attributes();
							{
								if ((tokenSet_32_.member(LA(1))) && (true))
								{
									type_member_modifier();
								}
								else if ((tokenSet_104_.member(LA(1))) && (true)) {
									{
										modifiers();
										{
											switch ( LA(1) )
											{
											case CLASS:
											{
												match(CLASS);
												break;
											}
											case ENUM:
											{
												match(ENUM);
												break;
											}
											case STRUCT:
											{
												match(STRUCT);
												break;
											}
											case INTERFACE:
											{
												match(INTERFACE);
												break;
											}
											case EVENT:
											{
												match(EVENT);
												break;
											}
											case DEF:
											{
												match(DEF);
												break;
											}
											case CALLABLE:
											{
												match(CALLABLE);
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
													begin_with_doc(null);
													{
														switch ( LA(1) )
														{
														case GET:
														{
															match(GET);
															break;
														}
														case SET:
														{
															match(SET);
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
								}
								else
								{
									throw new NoViableAltException(LT(1), getFilename());
								}
								
							}
						}
					}
					catch (RecognitionException)
					{
						synPredMatched414 = false;
					}
					rewind(_m414);
					inputState.guessing--;
				}
				if ( synPredMatched414 )
				{
					{
						{ // ( ... )+
							int _cnt417=0;
							for (;;)
							{
								if ((tokenSet_102_.member(LA(1))))
								{
									type_definition_member(collection);
								}
								else
								{
									if (_cnt417 >= 1) { goto _loop417_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
								}
								
								_cnt417++;
							}
_loop417_breakloop:							;
						}    // ( ... )+
						if (0==inputState.guessing)
						{
							
										if (collection.Count == 1) {
											e.Node = collection[0];
										} else {
											Module m = CodeFactory.NewQuasiquoteModule(e.LexicalInfo);
											m.Members = collection;
											e.Node = m;
										}
									
						}
					}
				}
				else if ((tokenSet_18_.member(LA(1))) && (tokenSet_85_.member(LA(2)))) {
					{ // ( ... )+
						int _cnt419=0;
						for (;;)
						{
							if ((tokenSet_18_.member(LA(1))))
							{
								stmt(statements);
							}
							else
							{
								if (_cnt419 >= 1) { goto _loop419_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
							}
							
							_cnt419++;
						}
_loop419_breakloop:						;
					}    // ( ... )+
					if (0==inputState.guessing)
					{
						e.Node = b.Statements.Count > 1 ? b : b.Statements[0];
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
					recover(ex,tokenSet_105_);
				}
				else
				{
					throw ex;
				}
			}
		}
		
	public void ast_literal_module(
		QuasiquoteExpression e
	) //throws RecognitionException, TokenStreamException
{
		
		
			var m = CodeFactory.NewQuasiquoteModule(e.LexicalInfo);
			e.Node = m;
		
		
		try {      // for error handling
			parse_module(m);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_105_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void ast_literal_module_prediction() //throws RecognitionException, TokenStreamException
{
		
		
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case EOF:
				case EOS:
				case NEWLINE:
				{
					eos();
					break;
				}
				case IMPORT:
				case NAMESPACE:
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
				case NAMESPACE:
				{
					match(NAMESPACE);
					break;
				}
				case IMPORT:
				{
					match(IMPORT);
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
				recover(ex,tokenSet_0_);
			}
			else
			{
				throw ex;
			}
		}
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
					if ((tokenSet_106_.member(LA(1))))
					{
						{
							switch ( LA(1) )
							{
							case IS:
							case IN:
							case NOT:
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
													op = OperatorParser.ParseComparison(t.getText()); token = t;
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
											else if ((LA(1)==IS) && (tokenSet_76_.member(LA(2)))) {
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
							
									BinaryExpression be = new BinaryExpression(SourceLocationFactory.ToLexicalInfo(token));
									be.Operator = op;
									be.Left = e;
									be.Right = r;
									e = be;
								
						}
					}
					else
					{
						goto _loop463_breakloop;
					}
					
				}
_loop463_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_107_);
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
					if ((tokenSet_108_.member(LA(1))))
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
							
										BinaryExpression be = new BinaryExpression(SourceLocationFactory.ToLexicalInfo(op));
										be.Operator = bOperator;
										be.Left = e;
										be.Right = r;
										e = be;
									
						}
					}
					else
					{
						goto _loop467_breakloop;
					}
					
				}
_loop467_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_109_);
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
					if ((tokenSet_110_.member(LA(1))))
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
							
										BinaryExpression be = new BinaryExpression(SourceLocationFactory.ToLexicalInfo(token));
										be.Operator = op;
										be.Left = e;
										be.Right = r;
										e = be;
									
						}
					}
					else
					{
						goto _loop471_breakloop;
					}
					
				}
_loop471_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_111_);
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
							
										BinaryExpression be = new BinaryExpression(SourceLocationFactory.ToLexicalInfo(token));
										be.Operator = op;
										be.Left = e;
										be.Right = r;
										e = be;
									
						}
					}
					else
					{
						goto _loop475_breakloop;
					}
					
				}
_loop475_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_112_);
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
		IToken  c = null;
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
						e = new TryCastExpression(ToLexicalInfo(t)) { Target = e, Type = tr };
					}
					break;
				}
				case CAST:
				{
					c = LT(1);
					match(CAST);
					tr=type_reference();
					if (0==inputState.guessing)
					{
						e = new CastExpression(ToLexicalInfo(c)) { Target = e, Type = tr };
					}
					break;
				}
				case EOF:
				case ESEPARATOR:
				case AND:
				case DEF:
				case DO:
				case ELSE:
				case FOR:
				case IS:
				case ISA:
				case IF:
				case IN:
				case NOT:
				case OR:
				case UNLESS:
				case WHILE:
				case ID:
				case EOS:
				case NEWLINE:
				case RPAREN:
				case MULTIPLY:
				case RBRACK:
				case ASSIGN:
				case SUBTRACT:
				case COMMA:
				case COLON:
				case EXPONENTIATION:
				case BITWISE_OR:
				case RBRACE:
				case QQ_END:
				case INPLACE_BITWISE_OR:
				case INPLACE_EXCLUSIVE_OR:
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
					if ((LA(1)==EXPONENTIATION) && (tokenSet_76_.member(LA(2))))
					{
						token = LT(1);
						match(EXPONENTIATION);
						r=exponentiation();
						if (0==inputState.guessing)
						{
							
										e = new BinaryExpression(ToLexicalInfo(token)) { Operator = BinaryOperatorType.Exponentiation, Left = e, Right = r };
									
						}
					}
					else
					{
						goto _loop479_breakloop;
					}
					
				}
_loop479_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_113_);
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
		IToken  explode = null;
		IToken  postinc = null;
		IToken  postdec = null;
		
					e = null;
					IToken op = null;
					UnaryOperatorType uOperator = UnaryOperatorType.None;
			
		
		try {      // for error handling
			{
				bool synPredMatched483 = false;
				if (((LA(1)==SUBTRACT||LA(1)==LONG||LA(1)==INT) && (tokenSet_114_.member(LA(2)))))
				{
					int _m483 = mark();
					synPredMatched483 = true;
					inputState.guessing++;
					try {
						{
							match(SUBTRACT);
							match(LONG);
						}
					}
					catch (RecognitionException)
					{
						synPredMatched483 = false;
					}
					rewind(_m483);
					inputState.guessing--;
				}
				if ( synPredMatched483 )
				{
					{
						e=integer_literal();
					}
				}
				else if ((tokenSet_115_.member(LA(1))) && (tokenSet_76_.member(LA(2)))) {
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
							case MULTIPLY:
							{
								explode = LT(1);
								match(MULTIPLY);
								if (0==inputState.guessing)
								{
									op = explode; uOperator = UnaryOperatorType.Explode;
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
				}
				else if ((tokenSet_68_.member(LA(1))) && (tokenSet_116_.member(LA(2)))) {
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
							case CAST:
							case DEF:
							case DO:
							case ELSE:
							case FOR:
							case IS:
							case ISA:
							case IF:
							case IN:
							case NOT:
							case OR:
							case UNLESS:
							case WHILE:
							case ID:
							case EOS:
							case NEWLINE:
							case RPAREN:
							case MULTIPLY:
							case RBRACK:
							case ASSIGN:
							case SUBTRACT:
							case COMMA:
							case COLON:
							case EXPONENTIATION:
							case BITWISE_OR:
							case RBRACE:
							case QQ_END:
							case INPLACE_BITWISE_OR:
							case INPLACE_EXCLUSIVE_OR:
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
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
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
				recover(ex,tokenSet_117_);
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
				case ESEPARATOR:
				case TRIPLE_QUOTED_STRING:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case BACKTICK_QUOTED_STRING:
				{
					e=string_literal();
					break;
				}
				case LBRACK:
				{
					e=list_literal();
					break;
				}
				case QQ_BEGIN:
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
				default:
					if ((LA(1)==SUBTRACT||LA(1)==LONG||LA(1)==INT) && (tokenSet_118_.member(LA(2))))
					{
						e=integer_literal();
					}
					else {
						bool synPredMatched548 = false;
						if (((LA(1)==LBRACE) && (tokenSet_72_.member(LA(2)))))
						{
							int _m548 = mark();
							synPredMatched548 = true;
							inputState.guessing++;
							try {
								{
									hash_literal_test();
								}
							}
							catch (RecognitionException)
							{
								synPredMatched548 = false;
							}
							rewind(_m548);
							inputState.guessing--;
						}
						if ( synPredMatched548 )
						{
							e=hash_literal();
						}
						else if ((LA(1)==LBRACE) && (tokenSet_119_.member(LA(2)))) {
							e=closure_expression();
						}
						else if ((LA(1)==SUBTRACT||LA(1)==DOUBLE||LA(1)==FLOAT) && (tokenSet_120_.member(LA(2)))) {
							e=double_literal();
						}
						else if ((LA(1)==SUBTRACT||LA(1)==TIMESPAN) && (tokenSet_121_.member(LA(2)))) {
							e=timespan_literal();
						}
					else
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					}break; }
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
			return e;
		}
		
	protected Expression  char_literal() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  charToken = null;
		IToken  t = null;
		IToken  i = null;
		
			e = null;
		
		
		try {      // for error handling
			charToken = LT(1);
			match(CHAR);
			match(LPAREN);
			{
				switch ( LA(1) )
				{
				case SINGLE_QUOTED_STRING:
				{
					t = LT(1);
					match(SINGLE_QUOTED_STRING);
					if (0==inputState.guessing)
					{
						
									e = new CharLiteralExpression(SourceLocationFactory.ToLexicalInfo(t), t.getText());
								
					}
					break;
				}
				case INT:
				{
					i = LT(1);
					match(INT);
					if (0==inputState.guessing)
					{
						
									e = new CharLiteralExpression(SourceLocationFactory.ToLexicalInfo(i), (char) PrimitiveParser.ParseInt(i));
								
					}
					break;
				}
				case RPAREN:
				{
					if (0==inputState.guessing)
					{
						
									e = new MethodInvocationExpression(
												ToLexicalInfo(charToken),
												new ReferenceExpression(ToLexicalInfo(charToken), charToken.getText()));
								
					}
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
				recover(ex,tokenSet_38_);
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
				
						e = new ReferenceExpression(SourceLocationFactory.ToLexicalInfo(t));
						e.Name = t.getText();
					
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
		return e;
	}
	
	protected Expression  paren_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  lparen = null;
		
			e = null;
			Expression condition = null;
			Expression falseValue = null;
		
		
		try {      // for error handling
			bool synPredMatched503 = false;
			if (((LA(1)==LPAREN) && (LA(2)==OF)))
			{
				int _m503 = mark();
				synPredMatched503 = true;
				inputState.guessing++;
				try {
					{
						match(LPAREN);
						match(OF);
					}
				}
				catch (RecognitionException)
				{
					synPredMatched503 = false;
				}
				rewind(_m503);
				inputState.guessing--;
			}
			if ( synPredMatched503 )
			{
				e=typed_array();
			}
			else if ((LA(1)==LPAREN) && (tokenSet_70_.member(LA(2)))) {
				{
					lparen = LT(1);
					match(LPAREN);
					e=array_or_expression();
					{
						switch ( LA(1) )
						{
						case IF:
						{
							match(IF);
							condition=boolean_expression();
							match(ELSE);
							falseValue=array_or_expression();
							if (0==inputState.guessing)
							{
								
												ConditionalExpression ce = new ConditionalExpression(SourceLocationFactory.ToLexicalInfo(lparen));
												ce.Condition = condition;
												ce.TrueValue = e;
												ce.FalseValue = falseValue;
												
												e = ce;
											
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
				recover(ex,tokenSet_38_);
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
				
						e = new CastExpression(SourceLocationFactory.ToLexicalInfo(t), target, tr);
					
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
				
						e = new TypeofExpression(SourceLocationFactory.ToLexicalInfo(t), tr);
					
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
		return e;
	}
	
	protected Expression  omitted_member_expression() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  dot = null;
		
			e = null;
			IToken memberName = null;
		
		
		try {      // for error handling
			dot = LT(1);
			match(DOT);
			memberName=member();
			if (0==inputState.guessing)
			{
				
						e = MemberReferenceForToken(new OmittedExpression(ToLexicalInfo(dot)), memberName);
					
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
		return e;
	}
	
	protected Expression  typed_array() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		IToken  t = null;
		
				e = null;
				ArrayLiteralExpression tle = null;
				TypeReference tr = null;
				Expression item = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(LPAREN);
			match(OF);
			tr=type_reference();
			match(COLON);
			if (0==inputState.guessing)
			{
				
						e = tle = new ArrayLiteralExpression(SourceLocationFactory.ToLexicalInfo(t));
						tle.Type = new ArrayTypeReference(tr.LexicalInfo, tr);
					
			}
			{
				switch ( LA(1) )
				{
				case COMMA:
				{
					match(COMMA);
					break;
				}
				case ESEPARATOR:
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case SPLICE_BEGIN:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					{
						item=expression();
						if (0==inputState.guessing)
						{
							tle.Items.Add(item);
						}
						{    // ( ... )*
							for (;;)
							{
								if ((LA(1)==COMMA) && (tokenSet_4_.member(LA(2))))
								{
									match(COMMA);
									item=expression();
									if (0==inputState.guessing)
									{
										tle.Items.Add(item);
									}
								}
								else
								{
									goto _loop510_breakloop;
								}
								
							}
_loop510_breakloop:							;
						}    // ( ... )*
						{
							switch ( LA(1) )
							{
							case COMMA:
							{
								match(COMMA);
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
				recover(ex,tokenSet_38_);
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
				case COLON:
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
				recover(ex,tokenSet_0_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
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
							case CAST:
							case CHAR:
							case FALSE:
							case NOT:
							case NULL:
							case SELF:
							case SUPER:
							case TRUE:
							case TYPEOF:
							case ID:
							case TRIPLE_QUOTED_STRING:
							case LPAREN:
							case DOUBLE_QUOTED_STRING:
							case SINGLE_QUOTED_STRING:
							case MULTIPLY:
							case LBRACK:
							case SUBTRACT:
							case SPLICE_BEGIN:
							case DOT:
							case LBRACE:
							case QQ_BEGIN:
							case LONG:
							case INCREMENT:
							case DECREMENT:
							case ONES_COMPLEMENT:
							case INT:
							case BACKTICK_QUOTED_STRING:
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
							case RBRACK:
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
					}
					break;
				}
				case ESEPARATOR:
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case SPLICE_BEGIN:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
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
								case CAST:
								case CHAR:
								case FALSE:
								case NOT:
								case NULL:
								case SELF:
								case SUPER:
								case TRUE:
								case TYPEOF:
								case ID:
								case TRIPLE_QUOTED_STRING:
								case LPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case MULTIPLY:
								case LBRACK:
								case SUBTRACT:
								case SPLICE_BEGIN:
								case DOT:
								case LBRACE:
								case QQ_BEGIN:
								case LONG:
								case INCREMENT:
								case DECREMENT:
								case ONES_COMPLEMENT:
								case INT:
								case BACKTICK_QUOTED_STRING:
								case RE_LITERAL:
								case DOUBLE:
								case FLOAT:
								case TIMESPAN:
								{
									end=expression();
									break;
								}
								case RBRACK:
								case COMMA:
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
							break;
						}
						case RBRACK:
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
				recover(ex,tokenSet_48_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected Expression  member_reference_expression(
		Expression target
	) //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		
				e = null;
				IToken memberName = null;
			
		
		try {      // for error handling
			memberName=member();
			if (0==inputState.guessing)
			{
				
						e = MemberReferenceForToken(target, memberName);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_122_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected void argument(
		INodeWithArguments node
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		IToken  colon = null;
				
				Expression value = null;
			
		
		try {      // for error handling
			bool synPredMatched593 = false;
			if (((LA(1)==ID) && (LA(2)==COLON)))
			{
				int _m593 = mark();
				synPredMatched593 = true;
				inputState.guessing++;
				try {
					{
						match(ID);
						match(COLON);
					}
				}
				catch (RecognitionException)
				{
					synPredMatched593 = false;
				}
				rewind(_m593);
				inputState.guessing--;
			}
			if ( synPredMatched593 )
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
											SourceLocationFactory.ToLexicalInfo(colon),
											new ReferenceExpression(SourceLocationFactory.ToLexicalInfo(id), id.getText()),
											value));
								
					}
				}
			}
			else if ((tokenSet_4_.member(LA(1))) && (tokenSet_123_.member(LA(2)))) {
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
				recover(ex,tokenSet_83_);
			}
			else
			{
				throw ex;
			}
		}
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
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case SPLICE_BEGIN:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
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
				recover(ex,tokenSet_0_);
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
				dle = new HashLiteralExpression(SourceLocationFactory.ToLexicalInfo(lbrace));
			}
			{
				switch ( LA(1) )
				{
				case ESEPARATOR:
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case SPLICE_BEGIN:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
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
							if ((LA(1)==COMMA) && (tokenSet_4_.member(LA(2))))
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
								goto _loop575_breakloop;
							}
							
						}
_loop575_breakloop:						;
					}    // ( ... )*
					{
						switch ( LA(1) )
						{
						case COMMA:
						{
							match(COMMA);
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
				recover(ex,tokenSet_38_);
			}
			else
			{
				throw ex;
			}
		}
		return dle;
	}
	
	protected ListLiteralExpression  list_initializer() //throws RecognitionException, TokenStreamException
{
		ListLiteralExpression e;
		
		IToken  lbrace = null;
		
				e = null;
				ExpressionCollection items = null;
			
		
		try {      // for error handling
			lbrace = LT(1);
			match(LBRACE);
			if (0==inputState.guessing)
			{
				
						e = new ListLiteralExpression(ToLexicalInfo(lbrace));
						items = e.Items;
					
			}
			list_items(items);
			match(RBRACE);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_122_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected void list_items(
		ExpressionCollection items
	) //throws RecognitionException, TokenStreamException
{
		
		
				Expression item = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case ESEPARATOR:
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case TRUE:
				case TYPEOF:
				case ID:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SUBTRACT:
				case SPLICE_BEGIN:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case BACKTICK_QUOTED_STRING:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					item=expression();
					if (0==inputState.guessing)
					{
						items.Add(item);
					}
					{
						{    // ( ... )*
							for (;;)
							{
								if ((LA(1)==COMMA) && (tokenSet_4_.member(LA(2))))
								{
									match(COMMA);
									item=expression();
									if (0==inputState.guessing)
									{
										items.Add(item);
									}
								}
								else
								{
									goto _loop567_breakloop;
								}
								
							}
_loop567_breakloop:							;
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
					break;
				}
				case RBRACK:
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
				recover(ex,tokenSet_124_);
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
		IToken  bqs = null;
		
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
					
							e = new StringLiteralExpression(SourceLocationFactory.ToLexicalInfo(dqs), dqs.getText());
							e.Annotate("quote", "\"");
						
				}
				break;
			}
			case SINGLE_QUOTED_STRING:
			{
				sqs = LT(1);
				match(SINGLE_QUOTED_STRING);
				if (0==inputState.guessing)
				{
					
							e = new StringLiteralExpression(SourceLocationFactory.ToLexicalInfo(sqs), sqs.getText());
							e.Annotate("quote", "'");
						
				}
				break;
			}
			case TRIPLE_QUOTED_STRING:
			{
				tqs = LT(1);
				match(TRIPLE_QUOTED_STRING);
				if (0==inputState.guessing)
				{
					
							e = new StringLiteralExpression(SourceLocationFactory.ToLexicalInfo(tqs), tqs.getText());
							e.Annotate("quote", "\"\"\"");
						
				}
				break;
			}
			case BACKTICK_QUOTED_STRING:
			{
				bqs = LT(1);
				match(BACKTICK_QUOTED_STRING);
				if (0==inputState.guessing)
				{
					
							e = new StringLiteralExpression(ToLexicalInfo(bqs), bqs.getText());
							e.Annotate("quote", "`");
						
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
				recover(ex,tokenSet_38_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected ListLiteralExpression  list_literal() //throws RecognitionException, TokenStreamException
{
		ListLiteralExpression e;
		
		IToken  lbrack = null;
		
				e = null;
				ExpressionCollection items = null;
			
		
		try {      // for error handling
			lbrack = LT(1);
			match(LBRACK);
			if (0==inputState.guessing)
			{
				
						e = new ListLiteralExpression(ToLexicalInfo(lbrack));
						items = e.Items;
					
			}
			list_items(items);
			match(RBRACK);
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
		return e;
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
				re = new RELiteralExpression(SourceLocationFactory.ToLexicalInfo(value), value.getText());
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
					
							e = new BoolLiteralExpression(SourceLocationFactory.ToLexicalInfo(t));
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
					
							e = new BoolLiteralExpression(SourceLocationFactory.ToLexicalInfo(f));
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
				recover(ex,tokenSet_38_);
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
				e = new NullLiteralExpression(SourceLocationFactory.ToLexicalInfo(t));
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
				e = new SelfLiteralExpression(SourceLocationFactory.ToLexicalInfo(t));
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
				e = new SuperLiteralExpression(SourceLocationFactory.ToLexicalInfo(t));
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
		return e;
	}
	
	protected DoubleLiteralExpression  double_literal() //throws RecognitionException, TokenStreamException
{
		DoubleLiteralExpression rle;
		
		IToken  neg = null;
		IToken  value = null;
		IToken  single = null;
		
				rle = null;
				string val;
			
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case SUBTRACT:
			case DOUBLE:
			{
				{
					switch ( LA(1) )
					{
					case SUBTRACT:
					{
						neg = LT(1);
						match(SUBTRACT);
						break;
					}
					case DOUBLE:
					{
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				value = LT(1);
				match(DOUBLE);
				if (0==inputState.guessing)
				{
					
							val = value.getText();
							if (neg != null) val = neg.getText() + val;
							rle = new DoubleLiteralExpression(SourceLocationFactory.ToLexicalInfo(value), PrimitiveParser.ParseDouble(value, val));
						
				}
				break;
			}
			case FLOAT:
			{
				single = LT(1);
				match(FLOAT);
				if (0==inputState.guessing)
				{
					
							val = single.getText();
							val = val.Substring(0, val.Length-1);
							if (neg != null) val = neg.getText() + val;
							rle = new DoubleLiteralExpression(SourceLocationFactory.ToLexicalInfo(single), PrimitiveParser.ParseDouble(single, val, true), true);
						
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
				recover(ex,tokenSet_38_);
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
		
		IToken  neg = null;
		IToken  value = null;
		tsle = null;
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case SUBTRACT:
				{
					neg = LT(1);
					match(SUBTRACT);
					break;
				}
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
			value = LT(1);
			match(TIMESPAN);
			if (0==inputState.guessing)
			{
				
						string val = value.getText();
						if (neg != null) val = neg.getText() + val;
						tsle = new TimeSpanLiteralExpression(SourceLocationFactory.ToLexicalInfo(value), PrimitiveParser.ParseTimeSpan(value, val)); 
					
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
		return tsle;
	}
	
	protected ExpressionInterpolationExpression  expression_interpolation() //throws RecognitionException, TokenStreamException
{
		ExpressionInterpolationExpression e;
		
		IToken  separator = null;
		IToken  format_sep = null;
		IToken  formatString = null;
		
				e = null;
				Expression param = null;	
			
		
		try {      // for error handling
			separator = LT(1);
			match(ESEPARATOR);
			if (0==inputState.guessing)
			{
				
						LexicalInfo info = SourceLocationFactory.ToLexicalInfo(separator);
						e = new ExpressionInterpolationExpression(info);		
					
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==ESEPARATOR) && (tokenSet_4_.member(LA(2))))
					{
						match(ESEPARATOR);
						param=expression();
						{
							switch ( LA(1) )
							{
							case ID:
							case COLON:
							{
								{
									switch ( LA(1) )
									{
									case COLON:
									{
										format_sep = LT(1);
										match(COLON);
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
								formatString = LT(1);
								match(ID);
								break;
							}
							case ESEPARATOR:
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
							
										if (null != param)
										{
											e.Expressions.Add(param);
											if (null != formatString)
												param.Annotate("formatString", formatString.getText());
										}
									
						}
						match(ESEPARATOR);
					}
					else
					{
						goto _loop561_breakloop;
					}
					
				}
_loop561_breakloop:				;
			}    // ( ... )*
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
				ep = new ExpressionPair(SourceLocationFactory.ToLexicalInfo(t), key, value);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_125_);
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
		@"""ASSEMBLY_ATTRIBUTE_BEGIN""",
		@"""MODULE_ATTRIBUTE_BEGIN""",
		@"""abstract""",
		@"""and""",
		@"""as""",
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
		@"""goto""",
		@"""import""",
		@"""interface""",
		@"""internal""",
		@"""is""",
		@"""isa""",
		@"""if""",
		@"""in""",
		@"""namespace""",
		@"""new""",
		@"""not""",
		@"""null""",
		@"""of""",
		@"""or""",
		@"""override""",
		@"""public""",
		@"""protected""",
		@"""private""",
		@"""raise""",
		@"""ref""",
		@"""return""",
		@"""set""",
		@"""self""",
		@"""super""",
		@"""static""",
		@"""struct""",
		@"""then""",
		@"""try""",
		@"""transient""",
		@"""true""",
		@"""typeof""",
		@"""unless""",
		@"""virtual""",
		@"""partial""",
		@"""while""",
		@"""yield""",
		@"""ID""",
		@"""TRIPLE_QUOTED_STRING""",
		@"""EOS""",
		@"""NEWLINE""",
		@"""LPAREN""",
		@"""RPAREN""",
		@"""DOUBLE_QUOTED_STRING""",
		@"""SINGLE_QUOTED_STRING""",
		@"""MULTIPLY""",
		@"""LBRACK""",
		@"""RBRACK""",
		@"""ASSIGN""",
		@"""SUBTRACT""",
		@"""COMMA""",
		@"""SPLICE_BEGIN""",
		@"""DOT""",
		@"""COLON""",
		@"""NULLABLE_SUFFIX""",
		@"""EXPONENTIATION""",
		@"""BITWISE_OR""",
		@"""LBRACE""",
		@"""RBRACE""",
		@"""QQ_BEGIN""",
		@"""QQ_END""",
		@"""INPLACE_BITWISE_OR""",
		@"""INPLACE_EXCLUSIVE_OR""",
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
		@"""LONG""",
		@"""INCREMENT""",
		@"""DECREMENT""",
		@"""ONES_COMPLEMENT""",
		@"""INT""",
		@"""BACKTICK_QUOTED_STRING""",
		@"""RE_LITERAL""",
		@"""DOUBLE""",
		@"""FLOAT""",
		@"""TIMESPAN""",
		@"""ID_SUFFIX""",
		@"""LINE_CONTINUATION""",
		@"""INTERPOLATED_EXPRESSION""",
		@"""INTERPOLATED_REFERENCE""",
		@"""SL_COMMENT""",
		@"""ML_COMMENT""",
		@"""WS""",
		@"""X_RE_LITERAL""",
		@"""DQS_ESC""",
		@"""SQS_ESC""",
		@"""SESC""",
		@"""RE_CHAR""",
		@"""X_RE_CHAR""",
		@"""RE_ESC""",
		@"""DIGIT_GROUP""",
		@"""REVERSE_DIGIT_GROUP""",
		@"""AT_SYMBOL""",
		@"""ID_LETTER""",
		@"""DIGIT""",
		@"""HEXDIGIT"""
	};
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = { 2L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = { -2396378464097930302L, 71987227053912063L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { -2305843010073526334L, 72057594037927935L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { 432453324957122626L, 71987225980170151L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { 432451125933867072L, 71987225971779971L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { -7485527524998757888L, 65560L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = { -7395455515135950336L, 131224L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = { 2L, 1073741824L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	private static long[] mk_tokenSet_8_()
	{
		long[] data = { -146784805717054L, 71987227323396095L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
	private static long[] mk_tokenSet_9_()
	{
		long[] data = { -18196367687289918L, 71987227053912063L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
	private static long[] mk_tokenSet_10_()
	{
		long[] data = { -62L, 72057594037927935L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
	private static long[] mk_tokenSet_11_()
	{
		long[] data = { -2396387260190952510L, 71987227053912063L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
	private static long[] mk_tokenSet_12_()
	{
		long[] data = { 2199025877058L, 1351751332L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
	private static long[] mk_tokenSet_13_()
	{
		long[] data = { -2396387331057912894L, 71987227053912063L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_13_ = new BitSet(mk_tokenSet_13_());
	private static long[] mk_tokenSet_14_()
	{
		long[] data = { -2396387331041135678L, 71987227053912063L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_14_ = new BitSet(mk_tokenSet_14_());
	private static long[] mk_tokenSet_15_()
	{
		long[] data = { 5089140193940844994L, 71987227053912039L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_15_ = new BitSet(mk_tokenSet_15_());
	private static long[] mk_tokenSet_16_()
	{
		long[] data = { 5089140193940844994L, 71987227053910503L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_16_ = new BitSet(mk_tokenSet_16_());
	private static long[] mk_tokenSet_17_()
	{
		long[] data = { -2305843010074837054L, 72057594037927935L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_17_ = new BitSet(mk_tokenSet_17_());
	private static long[] mk_tokenSet_18_()
	{
		long[] data = { 5089140193940844608L, 71987225980168679L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_18_ = new BitSet(mk_tokenSet_18_());
	private static long[] mk_tokenSet_19_()
	{
		long[] data = { 386L, 1073741824L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_19_ = new BitSet(mk_tokenSet_19_());
	private static long[] mk_tokenSet_20_()
	{
		long[] data = { 2L, 1536L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_20_ = new BitSet(mk_tokenSet_20_());
	private static long[] mk_tokenSet_21_()
	{
		long[] data = { -2396387331057913278L, 71987225980170239L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_21_ = new BitSet(mk_tokenSet_21_());
	private static long[] mk_tokenSet_22_()
	{
		long[] data = { 2199023255554L, 1572L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_22_ = new BitSet(mk_tokenSet_22_());
	private static long[] mk_tokenSet_23_()
	{
		long[] data = { -90262845999287358L, 71987227053912063L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_23_ = new BitSet(mk_tokenSet_23_());
	private static long[] mk_tokenSet_24_()
	{
		long[] data = { 432451125933867074L, 71987225971781507L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_24_ = new BitSet(mk_tokenSet_24_());
	private static long[] mk_tokenSet_25_()
	{
		long[] data = { -2305843010085322814L, 72057594021146623L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_25_ = new BitSet(mk_tokenSet_25_());
	private static long[] mk_tokenSet_26_()
	{
		long[] data = { 2147485698L, 1536L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_26_ = new BitSet(mk_tokenSet_26_());
	private static long[] mk_tokenSet_27_()
	{
		long[] data = { 70866962434L, 3584L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_27_ = new BitSet(mk_tokenSet_27_());
	private static long[] mk_tokenSet_28_()
	{
		long[] data = { 2199023255554L, 276829732L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_28_ = new BitSet(mk_tokenSet_28_());
	private static long[] mk_tokenSet_29_()
	{
		long[] data = { 93449984459931648L, 128L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_29_ = new BitSet(mk_tokenSet_29_());
	private static long[] mk_tokenSet_30_()
	{
		long[] data = { 465718347336770L, 492580536032932L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_30_ = new BitSet(mk_tokenSet_30_());
	private static long[] mk_tokenSet_31_()
	{
		long[] data = { -7251340327061403136L, 8421528L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_31_ = new BitSet(mk_tokenSet_31_());
	private static long[] mk_tokenSet_32_()
	{
		long[] data = { -8638449167112338944L, 24L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_32_ = new BitSet(mk_tokenSet_32_());
	private static long[] mk_tokenSet_33_()
	{
		long[] data = { 1369094441541451776L, 128L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_33_ = new BitSet(mk_tokenSet_33_());
	private static long[] mk_tokenSet_34_()
	{
		long[] data = { -2396387330906917950L, 71987227053912063L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_34_ = new BitSet(mk_tokenSet_34_());
	private static long[] mk_tokenSet_35_()
	{
		long[] data = { 144115188210597888L, 65664L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_35_ = new BitSet(mk_tokenSet_35_());
	private static long[] mk_tokenSet_36_()
	{
		long[] data = { 0L, 131072L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_36_ = new BitSet(mk_tokenSet_36_());
	private static long[] mk_tokenSet_37_()
	{
		long[] data = { 0L, 67244032L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_37_ = new BitSet(mk_tokenSet_37_());
	private static long[] mk_tokenSet_38_()
	{
		long[] data = { 465647480376386L, 492580536032932L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_38_ = new BitSet(mk_tokenSet_38_());
	private static long[] mk_tokenSet_39_()
	{
		long[] data = { 140737488437248L, 2132096L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_39_ = new BitSet(mk_tokenSet_39_());
	private static long[] mk_tokenSet_40_()
	{
		long[] data = { 81920L, 2099328L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_40_ = new BitSet(mk_tokenSet_40_());
	private static long[] mk_tokenSet_41_()
	{
		long[] data = { -8796094070846L, 72057594037927935L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_41_ = new BitSet(mk_tokenSet_41_());
	private static long[] mk_tokenSet_42_()
	{
		long[] data = { -2324329719689121214L, 71987225980170239L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_42_ = new BitSet(mk_tokenSet_42_());
	private static long[] mk_tokenSet_43_()
	{
		long[] data = { -2306124485041849406L, 72057594021146623L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_43_ = new BitSet(mk_tokenSet_43_());
	private static long[] mk_tokenSet_44_()
	{
		long[] data = { 2L, 1196268651021824L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_44_ = new BitSet(mk_tokenSet_44_());
	private static long[] mk_tokenSet_45_()
	{
		long[] data = { 16777216L, 2162816L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_45_ = new BitSet(mk_tokenSet_45_());
	private static long[] mk_tokenSet_46_()
	{
		long[] data = { -7341412336771907072L, 2162840L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_46_ = new BitSet(mk_tokenSet_46_());
	private static long[] mk_tokenSet_47_()
	{
		long[] data = { -18205234647272510L, 71987227053912063L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_47_ = new BitSet(mk_tokenSet_47_());
	private static long[] mk_tokenSet_48_()
	{
		long[] data = { 0L, 1179648L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_48_ = new BitSet(mk_tokenSet_48_());
	private static long[] mk_tokenSet_49_()
	{
		long[] data = { 0L, 4096L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_49_ = new BitSet(mk_tokenSet_49_());
	private static long[] mk_tokenSet_50_()
	{
		long[] data = { 0L, 8388608L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_50_ = new BitSet(mk_tokenSet_50_());
	private static long[] mk_tokenSet_51_()
	{
		long[] data = { -7341412336771907072L, 1075904664L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_51_ = new BitSet(mk_tokenSet_51_());
	private static long[] mk_tokenSet_52_()
	{
		long[] data = { 2048L, 12650496L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_52_ = new BitSet(mk_tokenSet_52_());
	private static long[] mk_tokenSet_53_()
	{
		long[] data = { 2048L, 8456192L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_53_ = new BitSet(mk_tokenSet_53_());
	private static long[] mk_tokenSet_54_()
	{
		long[] data = { -8566391555894541824L, 65560L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_54_ = new BitSet(mk_tokenSet_54_());
	private static long[] mk_tokenSet_55_()
	{
		long[] data = { 2050L, 263680L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_55_ = new BitSet(mk_tokenSet_55_());
	private static long[] mk_tokenSet_56_()
	{
		long[] data = { 144115188227375104L, 65664L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_56_ = new BitSet(mk_tokenSet_56_());
	private static long[] mk_tokenSet_57_()
	{
		long[] data = { 1152921504607322112L, 2099328L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_57_ = new BitSet(mk_tokenSet_57_());
	private static long[] mk_tokenSet_58_()
	{
		long[] data = { 18014398509481984L, 8493184L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_58_ = new BitSet(mk_tokenSet_58_());
	private static long[] mk_tokenSet_59_()
	{
		long[] data = { 0L, 1181696L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_59_ = new BitSet(mk_tokenSet_59_());
	private static long[] mk_tokenSet_60_()
	{
		long[] data = { 216172799445172226L, 67200L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_60_ = new BitSet(mk_tokenSet_60_());
	private static long[] mk_tokenSet_61_()
	{
		long[] data = { 72057611234574336L, 65536L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_61_ = new BitSet(mk_tokenSet_61_());
	private static long[] mk_tokenSet_62_()
	{
		long[] data = { 7395264678999470146L, 71987225980170215L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_62_ = new BitSet(mk_tokenSet_62_());
	private static long[] mk_tokenSet_63_()
	{
		long[] data = { 237565172535787520L, 128L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_63_ = new BitSet(mk_tokenSet_63_());
	private static long[] mk_tokenSet_64_()
	{
		long[] data = { 5089140193957621826L, 71987225980170215L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_64_ = new BitSet(mk_tokenSet_64_());
	private static long[] mk_tokenSet_65_()
	{
		long[] data = { -2306124485043160126L, 72057594021146623L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_65_ = new BitSet(mk_tokenSet_65_());
	private static long[] mk_tokenSet_66_()
	{
		long[] data = { 2306124485058625536L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_66_ = new BitSet(mk_tokenSet_66_());
	private static long[] mk_tokenSet_67_()
	{
		long[] data = { -8566391555877764608L, 65560L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_67_ = new BitSet(mk_tokenSet_67_());
	private static long[] mk_tokenSet_68_()
	{
		long[] data = { 432415941561778240L, 71002063553259907L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_68_ = new BitSet(mk_tokenSet_68_());
	private static long[] mk_tokenSet_69_()
	{
		long[] data = { -2306130532373892158L, 71987227390636031L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_69_ = new BitSet(mk_tokenSet_69_());
	private static long[] mk_tokenSet_70_()
	{
		long[] data = { 432451125933867072L, 71987225972828547L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_70_ = new BitSet(mk_tokenSet_70_());
	private static long[] mk_tokenSet_71_()
	{
		long[] data = { -90262845865069630L, 71987227053910527L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_71_ = new BitSet(mk_tokenSet_71_());
	private static long[] mk_tokenSet_72_()
	{
		long[] data = { 432451125933867072L, 71987226240215427L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_72_ = new BitSet(mk_tokenSet_72_());
	private static long[] mk_tokenSet_73_()
	{
		long[] data = { 324909992021058L, 492580514993828L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_73_ = new BitSet(mk_tokenSet_73_());
	private static long[] mk_tokenSet_74_()
	{
		long[] data = { 2199025876994L, 276829732L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_74_ = new BitSet(mk_tokenSet_74_());
	private static long[] mk_tokenSet_75_()
	{
		long[] data = { -18205234647272510L, 71987227053910527L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_75_ = new BitSet(mk_tokenSet_75_());
	private static long[] mk_tokenSet_76_()
	{
		long[] data = { 432415941561778240L, 71987225971779971L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_76_ = new BitSet(mk_tokenSet_76_());
	private static long[] mk_tokenSet_77_()
	{
		long[] data = { -2306124485062034494L, 72057594021146623L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_77_ = new BitSet(mk_tokenSet_77_());
	private static long[] mk_tokenSet_78_()
	{
		long[] data = { -2306130532373892158L, 71987227390898175L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_78_ = new BitSet(mk_tokenSet_78_());
	private static long[] mk_tokenSet_79_()
	{
		long[] data = { 441423175176269888L, 71987225980168643L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_79_ = new BitSet(mk_tokenSet_79_());
	private static long[] mk_tokenSet_80_()
	{
		long[] data = { -90262845999287358L, 71987227053910527L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_80_ = new BitSet(mk_tokenSet_80_());
	private static long[] mk_tokenSet_81_()
	{
		long[] data = { -7269354725570887168L, 152L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_81_ = new BitSet(mk_tokenSet_81_());
	private static long[] mk_tokenSet_82_()
	{
		long[] data = { 0L, 68292608L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_82_ = new BitSet(mk_tokenSet_82_());
	private static long[] mk_tokenSet_83_()
	{
		long[] data = { 0L, 1052672L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_83_ = new BitSet(mk_tokenSet_83_());
	private static long[] mk_tokenSet_84_()
	{
		long[] data = { 2199023255554L, 268437028L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_84_ = new BitSet(mk_tokenSet_84_());
	private static long[] mk_tokenSet_85_()
	{
		long[] data = { -2306124485059937342L, 72057594021146623L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_85_ = new BitSet(mk_tokenSet_85_());
	private static long[] mk_tokenSet_86_()
	{
		long[] data = { 16777216L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_86_ = new BitSet(mk_tokenSet_86_());
	private static long[] mk_tokenSet_87_()
	{
		long[] data = { -2396387331041136064L, 71987225980168703L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_87_ = new BitSet(mk_tokenSet_87_());
	private static long[] mk_tokenSet_88_()
	{
		long[] data = { 2203320844354L, 1351751332L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_88_ = new BitSet(mk_tokenSet_88_());
	private static long[] mk_tokenSet_89_()
	{
		long[] data = { 4398046511104L, 1310720L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_89_ = new BitSet(mk_tokenSet_89_());
	private static long[] mk_tokenSet_90_()
	{
		long[] data = { 2203329232962L, 1351751332L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_90_ = new BitSet(mk_tokenSet_90_());
	private static long[] mk_tokenSet_91_()
	{
		long[] data = { 432453324957122626L, 71987226240216999L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_91_ = new BitSet(mk_tokenSet_91_());
	private static long[] mk_tokenSet_92_()
	{
		long[] data = { 2L, 268436992L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_92_ = new BitSet(mk_tokenSet_92_());
	private static long[] mk_tokenSet_93_()
	{
		long[] data = { 18014398509481984L, 67207296L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_93_ = new BitSet(mk_tokenSet_93_());
	private static long[] mk_tokenSet_94_()
	{
		long[] data = { -8745884914647201728L, 71987226040068547L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_94_ = new BitSet(mk_tokenSet_94_());
	private static long[] mk_tokenSet_95_()
	{
		long[] data = { 477487122207572032L, 71987225972828611L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_95_ = new BitSet(mk_tokenSet_95_());
	private static long[] mk_tokenSet_96_()
	{
		long[] data = { 855638016L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_96_ = new BitSet(mk_tokenSet_96_());
	private static long[] mk_tokenSet_97_()
	{
		long[] data = { 283678305944642L, 1351751332L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_97_ = new BitSet(mk_tokenSet_97_());
	private static long[] mk_tokenSet_98_()
	{
		long[] data = { 4398046511104L, 262144L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_98_ = new BitSet(mk_tokenSet_98_());
	private static long[] mk_tokenSet_99_()
	{
		long[] data = { -90080805821612094L, 72057594037927935L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_99_ = new BitSet(mk_tokenSet_99_());
	private static long[] mk_tokenSet_100_()
	{
		long[] data = { 283678305943618L, 1351751332L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_100_ = new BitSet(mk_tokenSet_100_());
	private static long[] mk_tokenSet_101_()
	{
		long[] data = { -2396378463963712574L, 71987227053912063L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_101_ = new BitSet(mk_tokenSet_101_());
	private static long[] mk_tokenSet_102_()
	{
		long[] data = { -7341412336788684288L, 65688L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_102_ = new BitSet(mk_tokenSet_102_());
	private static long[] mk_tokenSet_103_()
	{
		long[] data = { -7251340327060092414L, 13045400L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_103_ = new BitSet(mk_tokenSet_103_());
	private static long[] mk_tokenSet_104_()
	{
		long[] data = { -7485527524864540160L, 152L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_104_ = new BitSet(mk_tokenSet_104_());
	private static long[] mk_tokenSet_105_()
	{
		long[] data = { 0L, 1073741824L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_105_ = new BitSet(mk_tokenSet_105_());
	private static long[] mk_tokenSet_106_()
	{
		long[] data = { 41231686041600L, 481036337152L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_106_ = new BitSet(mk_tokenSet_106_());
	private static long[] mk_tokenSet_107_()
	{
		long[] data = { 283678305944642L, 67924006564L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_107_ = new BitSet(mk_tokenSet_107_());
	private static long[] mk_tokenSet_108_()
	{
		long[] data = { 0L, 1649335074816L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_108_ = new BitSet(mk_tokenSet_108_());
	private static long[] mk_tokenSet_109_()
	{
		long[] data = { 324909991986242L, 548960343716L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_109_ = new BitSet(mk_tokenSet_109_());
	private static long[] mk_tokenSet_110_()
	{
		long[] data = { 0L, 15393162821632L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_110_ = new BitSet(mk_tokenSet_110_());
	private static long[] mk_tokenSet_111_()
	{
		long[] data = { 324909991986242L, 2198295418532L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_111_ = new BitSet(mk_tokenSet_111_());
	private static long[] mk_tokenSet_112_()
	{
		long[] data = { 324909991986242L, 17591458240164L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_112_ = new BitSet(mk_tokenSet_112_());
	private static long[] mk_tokenSet_113_()
	{
		long[] data = { 324909991986242L, 70368049927844L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_113_ = new BitSet(mk_tokenSet_113_());
	private static long[] mk_tokenSet_114_()
	{
		long[] data = { 324909992021058L, 1266636700948132L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_114_ = new BitSet(mk_tokenSet_114_());
	private static long[] mk_tokenSet_115_()
	{
		long[] data = { 0L, 985162419044352L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_115_ = new BitSet(mk_tokenSet_115_());
	private static long[] mk_tokenSet_116_()
	{
		long[] data = { -2305843010074837054L, 72057594021150719L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_116_ = new BitSet(mk_tokenSet_116_());
	private static long[] mk_tokenSet_117_()
	{
		long[] data = { 324909992021058L, 70368049927844L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_117_ = new BitSet(mk_tokenSet_117_());
	private static long[] mk_tokenSet_118_()
	{
		long[] data = { 465647480376386L, 1688849187053220L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_118_ = new BitSet(mk_tokenSet_118_());
	private static long[] mk_tokenSet_119_()
	{
		long[] data = { 495501520717054016L, 71987226039937475L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_119_ = new BitSet(mk_tokenSet_119_());
	private static long[] mk_tokenSet_120_()
	{
		long[] data = { 465647480376386L, 9499779790773924L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_120_ = new BitSet(mk_tokenSet_120_());
	private static long[] mk_tokenSet_121_()
	{
		long[] data = { 465647480376386L, 36521377554996900L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_121_ = new BitSet(mk_tokenSet_121_());
	private static long[] mk_tokenSet_122_()
	{
		long[] data = { 465647480376386L, 492580519255716L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_122_ = new BitSet(mk_tokenSet_122_());
	private static long[] mk_tokenSet_123_()
	{
		long[] data = { -2305843010085322814L, 72057594021150719L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_123_ = new BitSet(mk_tokenSet_123_());
	private static long[] mk_tokenSet_124_()
	{
		long[] data = { 0L, 268566528L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_124_ = new BitSet(mk_tokenSet_124_());
	private static long[] mk_tokenSet_125_()
	{
		long[] data = { 0L, 269484032L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_125_ = new BitSet(mk_tokenSet_125_());
	
}
}
