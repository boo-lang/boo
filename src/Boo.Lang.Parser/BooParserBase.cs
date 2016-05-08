// $ANTLR 2.7.5 (20131102): "src/Boo.Lang.Parser/boo.g" -> "BooParserBase.cs"$

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

public delegate void ParserErrorHandler(antlr.RecognitionException x);

abstract

	public 	class BooParserBase : antlr.LLkParser
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
		
				
	protected System.Text.StringBuilder _sbuilder = new System.Text.StringBuilder();
	
	protected AttributeCollection _attributes = new AttributeCollection();
	
	protected TypeMemberModifiers _modifiers = TypeMemberModifiers.None;

	protected bool _inArray;
	
	protected bool _compact = false;
	
	int _inQuery = 0;

	private void EnterQuery()
	{
		++_inQuery;
	}
	
	private void LeaveQuery()
	{
		--_inQuery;
	}

	private bool InQuery
	{
		get
		{
			return _inQuery > 0;
		}
	}
	
	protected void ResetMemberData()
	{
		_modifiers = TypeMemberModifiers.None;
	}

	protected void AddAttributes(AttributeCollection target)
	{
		if (target != null) target.Extend(_attributes);
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
	
	protected bool IsValidClosureMacroArgument(int token)
	{
		if (!IsValidMacroArgument(token)) return false;
		return SUBTRACT != token;
	}
	
	private LexicalInfo ToLexicalInfo(IToken token)
	{
		return SourceLocationFactory.ToLexicalInfo(token);
	}
	
	private void SetEndSourceLocation(Node node, IToken token)
	{
		node.EndSourceLocation = SourceLocationFactory.ToSourceLocation(token);
	}
	
	private MemberReferenceExpression MemberReferenceForToken(Expression target, IToken memberName)
	{
		MemberReferenceExpression mre = new MemberReferenceExpression(ToLexicalInfo(memberName));
		mre.Target = target;
		mre.Name = memberName.getText();
		return mre;	
	}
	
	protected abstract void EmitIndexedPropertyDeprecationWarning(Property deprecated);
	
	protected abstract void EmitTransientKeywordDeprecationWarning(LexicalInfo location);
		
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
		
	protected Module  start(
		CompileUnit cu
	) //throws RecognitionException, TokenStreamException
{
		Module module;
		
		IToken  eof = null;
		
			module = new Module();		
			module.LexicalInfo = new LexicalInfo(getFilename(), 1, 1);
			
			cu.Modules.Add(module);
		
		
		try {      // for error handling
			parse_module(module);
			eof = LT(1);
			match(Token.EOF_TYPE);
			if (0==inputState.guessing)
			{
				SetEndSourceLocation(module, eof);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "start");
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
				if ((LA(1)==EOL||LA(1)==EOS) && (tokenSet_1_.member(LA(2))))
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
				if ((LA(1)==EOL||LA(1)==EOS) && (tokenSet_1_.member(LA(2))))
				{
					eos();
				}
				else if ((tokenSet_1_.member(LA(1))) && (tokenSet_3_.member(LA(2)))) {
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
				case DEDENT:
				case ESEPARATOR:
				case EOL:
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
				case PARTIAL:
				case PUBLIC:
				case PROTECTED:
				case PRIVATE:
				case RAISE:
				case RETURN:
				case SELF:
				case SUPER:
				case STATIC:
				case STRUCT:
				case THEN:
				case TRY:
				case TRANSIENT:
				case TRUE:
				case TYPEOF:
				case UNLESS:
				case VIRTUAL:
				case WHILE:
				case YIELD:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case EOS:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case COLON:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
					if ((LA(1)==FROM||LA(1)==IMPORT) && (tokenSet_4_.member(LA(2))))
					{
						import_directive(module);
					}
					else
					{
						goto _loop7_breakloop;
					}
					
				}
_loop7_breakloop:				;
			}    // ( ... )*
			{    // ( ... )*
				for (;;)
				{
					bool synPredMatched11 = false;
					if ((((tokenSet_4_.member(LA(1))) && (tokenSet_5_.member(LA(2))))&&(IsValidMacroArgument(LA(2)))))
					{
						int _m11 = mark();
						synPredMatched11 = true;
						inputState.guessing++;
						try {
							{
								macro_name();
								{
									if ((tokenSet_6_.member(LA(1))))
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
							synPredMatched11 = false;
						}
						rewind(_m11);
						inputState.guessing--;
					}
					if ( synPredMatched11 )
					{
						module_macro(module);
					}
					else if ((tokenSet_7_.member(LA(1))) && (tokenSet_8_.member(LA(2)))) {
						type_member(module.Members);
					}
					else
					{
						goto _loop12_breakloop;
					}
					
				}
_loop12_breakloop:				;
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
						goto _loop15_breakloop;
					}
					
				}
_loop15_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "parse_module");
				recover(ex,tokenSet_9_);
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
				int _cnt23=0;
				for (;;)
				{
					if ((LA(1)==EOL||LA(1)==EOS) && (tokenSet_10_.member(LA(2))))
					{
						{
							switch ( LA(1) )
							{
							case EOL:
							{
								match(EOL);
								break;
							}
							case EOS:
							{
								match(EOS);
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
						if (_cnt23 >= 1) { goto _loop23_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt23++;
				}
_loop23_breakloop:				;
			}    // ( ... )+
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "eos");
				recover(ex,tokenSet_10_);
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
				if ((LA(1)==TRIPLE_QUOTED_STRING) && (tokenSet_11_.member(LA(2))))
				{
					doc = LT(1);
					match(TRIPLE_QUOTED_STRING);
					if (0==inputState.guessing)
					{
						node.Documentation = DocStringFormatter.Format(doc.getText());
					}
					{
						if ((LA(1)==EOL||LA(1)==EOS) && (tokenSet_11_.member(LA(2))))
						{
							eos();
						}
						else if ((tokenSet_11_.member(LA(1))) && (tokenSet_12_.member(LA(2)))) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
				}
				else if ((tokenSet_11_.member(LA(1))) && (tokenSet_12_.member(LA(2)))) {
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
				reportError(ex, "docstring");
				recover(ex,tokenSet_11_);
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
				reportError(ex, "namespace_directive");
				recover(ex,tokenSet_13_);
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
					break;
				}
				case FROM:
				{
					node=import_directive_from_();
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
			eos();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "import_directive");
				recover(ex,tokenSet_13_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected antlr.IToken  macro_name() //throws RecognitionException, TokenStreamException
{
		antlr.IToken name;
		
		IToken  id = null;
		IToken  then = null;
		IToken  j = null;
		IToken  l = null;
		IToken  w = null;
		IToken  o = null;
		IToken  e = null;
		IToken  i = null;
		IToken  r = null;
		IToken  a = null;
		IToken  d = null;
		IToken  s = null;
		IToken  g = null;
		IToken  b = null;
		
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
					name = id;
				}
				break;
			}
			case THEN:
			{
				then = LT(1);
				match(THEN);
				if (0==inputState.guessing)
				{
					name = then;
				}
				break;
			}
			default:
				if ((((LA(1) >= LET && LA(1) <= BY)))&&(!InQuery))
				{
					{
						switch ( LA(1) )
						{
						case JOIN:
						{
							j = LT(1);
							match(JOIN);
							if (0==inputState.guessing)
							{
								name = j;
							}
							break;
						}
						case LET:
						{
							l = LT(1);
							match(LET);
							if (0==inputState.guessing)
							{
								name = l;
							}
							break;
						}
						case WHERE:
						{
							w = LT(1);
							match(WHERE);
							if (0==inputState.guessing)
							{
								name = w;
							}
							break;
						}
						case ON:
						{
							o = LT(1);
							match(ON);
							if (0==inputState.guessing)
							{
								name = o;
							}
							break;
						}
						case EQUALS:
						{
							e = LT(1);
							match(EQUALS);
							if (0==inputState.guessing)
							{
								name = e;
							}
							break;
						}
						case INTO:
						{
							i = LT(1);
							match(INTO);
							if (0==inputState.guessing)
							{
								name = i;
							}
							break;
						}
						case ORDERBY:
						{
							r = LT(1);
							match(ORDERBY);
							if (0==inputState.guessing)
							{
								name = r;
							}
							break;
						}
						case ASCENDING:
						{
							a = LT(1);
							match(ASCENDING);
							if (0==inputState.guessing)
							{
								name = a;
							}
							break;
						}
						case DESCENDING:
						{
							d = LT(1);
							match(DESCENDING);
							if (0==inputState.guessing)
							{
								name = d;
							}
							break;
						}
						case SELECT:
						{
							s = LT(1);
							match(SELECT);
							if (0==inputState.guessing)
							{
								name = s;
							}
							break;
						}
						case GROUP:
						{
							g = LT(1);
							match(GROUP);
							if (0==inputState.guessing)
							{
								name = g;
							}
							break;
						}
						case BY:
						{
							b = LT(1);
							match(BY);
							if (0==inputState.guessing)
							{
								name = b;
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
				reportError(ex, "macro_name");
				recover(ex,tokenSet_14_);
			}
			else
			{
				throw ex;
			}
		}
		return name;
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
				if ((LA(1)==FOR) && (tokenSet_4_.member(LA(2))))
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
							if ((LA(1)==FOR) && (tokenSet_4_.member(LA(2))))
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
								goto _loop434_breakloop;
							}
							
						}
_loop434_breakloop:						;
					}    // ( ... )*
				}
				else if ((tokenSet_15_.member(LA(1))) && (tokenSet_16_.member(LA(2)))) {
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
				reportError(ex, "expression");
				recover(ex,tokenSet_15_);
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
		
		
			MacroStatement s = null;
		
		
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
				reportError(ex, "module_macro");
				recover(ex,tokenSet_17_);
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
				reportError(ex, "type_member");
				recover(ex,tokenSet_17_);
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
				switch ( LA(1) )
				{
				case EOL:
				case EOS:
				{
					eos();
					break;
				}
				case EOF:
				case DEDENT:
				case ESEPARATOR:
				case ASSEMBLY_ATTRIBUTE_BEGIN:
				case MODULE_ATTRIBUTE_BEGIN:
				case BREAK:
				case CONTINUE:
				case CAST:
				case CHAR:
				case DEF:
				case FROM:
				case FOR:
				case FALSE:
				case GOTO:
				case IF:
				case NULL:
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
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case COLON:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
						stmt(container.Globals.Statements);
					}
					else
					{
						goto _loop191_breakloop;
					}
					
				}
_loop191_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "globals");
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
				reportError(ex, "assembly_attribute");
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
				reportError(ex, "module_attribute");
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
		
		
				returnValue = null;
				MacroStatement macro = new MacroStatement();
				StatementModifier modifier = null;
				antlr.IToken id = null;
			
		
		try {      // for error handling
			id=macro_name();
			expression_list(macro.Arguments);
			{
				if ((LA(1)==COLON) && (LA(2)==INDENT||LA(2)==EOL||LA(2)==EOS))
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
							case EOL:
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
				reportError(ex, "macro_stmt");
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
		
			Expression ns = null;
			IToken id = null;
			IToken alias = null;
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
						case THEN:
						case LET:
						case WHERE:
						case JOIN:
						case ON:
						case EQUALS:
						case INTO:
						case ORDERBY:
						case ASCENDING:
						case DESCENDING:
						case SELECT:
						case GROUP:
						case BY:
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
				case EOL:
				case AS:
				case EOS:
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
			{
				switch ( LA(1) )
				{
				case AS:
				{
					match(AS);
					alias=macro_name();
					if (0==inputState.guessing)
					{
						
									returnValue.Alias = new ReferenceExpression(ToLexicalInfo(alias));
									returnValue.Alias.Name = alias.getText();
								
					}
					break;
				}
				case EOL:
				case EOS:
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
				reportError(ex, "import_directive_");
				recover(ex,tokenSet_24_);
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
				if ((LA(1)==MULTIPLY) && (LA(2)==EOL||LA(2)==EOS))
				{
					match(MULTIPLY);
					if (0==inputState.guessing)
					{
						returnValue.Expression = ns;
					}
				}
				else if ((tokenSet_25_.member(LA(1))) && (tokenSet_26_.member(LA(2)))) {
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
				reportError(ex, "import_directive_from_");
				recover(ex,tokenSet_20_);
			}
			else
			{
				throw ex;
			}
		}
		return returnValue;
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
				reportError(ex, "identifier_expression");
				recover(ex,tokenSet_27_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	protected IToken  identifier() //throws RecognitionException, TokenStreamException
{
		IToken value;
		
		
				value = null; _sbuilder.Length = 0;
				IToken id1 = null;
				IToken id2 = null;
			
		
		try {      // for error handling
			id1=macro_name();
			if (0==inputState.guessing)
			{
									
						if (id1 != null) _sbuilder.Append(id1.getText());
						value = id1;
					
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==DOT) && (tokenSet_28_.member(LA(2))))
					{
						match(DOT);
						id2=member();
						if (0==inputState.guessing)
						{
							
										_sbuilder.Append('.');
										if (id2 != null) _sbuilder.Append(id2.getText());
									
						}
					}
					else
					{
						goto _loop688_breakloop;
					}
					
				}
_loop688_breakloop:				;
			}    // ( ... )*
			if (0==inputState.guessing)
			{
				if (value != null) value.setText(_sbuilder.ToString());
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "identifier");
				recover(ex,tokenSet_29_);
			}
			else
			{
				throw ex;
			}
		}
		return value;
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
				case EOL:
				case AS:
				case FROM:
				case EOS:
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
				reportError(ex, "namespace_expression");
				recover(ex,tokenSet_30_);
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
				case FROM:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
									if (e != null) ec.Add(e);
								}
							}
							else
							{
								goto _loop676_breakloop;
							}
							
						}
_loop676_breakloop:						;
					}    // ( ... )*
					break;
				}
				case EOL:
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
				case RPAREN:
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
				reportError(ex, "expression_list");
				recover(ex,tokenSet_31_);
			}
			else
			{
				throw ex;
			}
		}
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
							case THEN:
							case TRANSIENT:
							case LET:
							case WHERE:
							case JOIN:
							case ON:
							case EQUALS:
							case INTO:
							case ORDERBY:
							case ASCENDING:
							case DESCENDING:
							case SELECT:
							case GROUP:
							case BY:
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
											goto _loop55_breakloop;
										}
										
									}
_loop55_breakloop:									;
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
							case EOL:
							case EOS:
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
							case PARTIAL:
							case PUBLIC:
							case PROTECTED:
							case PRIVATE:
							case REF:
							case SET:
							case SELF:
							case STATIC:
							case STRUCT:
							case THEN:
							case TRANSIENT:
							case VIRTUAL:
							case LET:
							case WHERE:
							case JOIN:
							case ON:
							case EQUALS:
							case INTO:
							case ORDERBY:
							case ASCENDING:
							case DESCENDING:
							case SELECT:
							case GROUP:
							case BY:
							case MULTIPLY:
							case LBRACK:
							case SPLICE_BEGIN:
							case ID:
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
						goto _loop57_breakloop;
					}
					
				}
_loop57_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "attributes");
				recover(ex,tokenSet_32_);
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
					if ((tokenSet_33_.member(LA(1))))
					{
						type_member_modifier();
					}
					else
					{
						goto _loop200_breakloop;
					}
					
				}
_loop200_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "modifiers");
				recover(ex,tokenSet_34_);
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
				reportError(ex, "type_definition");
				recover(ex,tokenSet_23_);
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
		IToken  spliceBegin = null;
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
				Expression nameSplice = null;
				TypeMember typeMember = null;
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
				case THEN:
				case YIELD:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case SPLICE_BEGIN:
				case ID:
				{
					{
						{
							{
								if ((tokenSet_4_.member(LA(1))) && (LA(2)==DOT))
								{
									emi=explicit_member_info();
								}
								else if ((tokenSet_35_.member(LA(1))) && (tokenSet_36_.member(LA(2)))) {
								}
								else
								{
									throw new NoViableAltException(LT(1), getFilename());
								}
								
							}
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
								case THEN:
								case YIELD:
								case LET:
								case WHERE:
								case JOIN:
								case ON:
								case EQUALS:
								case INTO:
								case ORDERBY:
								case ASCENDING:
								case DESCENDING:
								case SELECT:
								case GROUP:
								case BY:
								case ID:
								{
									id=member();
									break;
								}
								case SPLICE_BEGIN:
								{
									spliceBegin = LT(1);
									match(SPLICE_BEGIN);
									nameSplice=atom();
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
							
											IToken token = id ?? spliceBegin;
											if (emi != null) {
												m = new Method(emi.LexicalInfo);
											} else {
												m = new Method(ToLexicalInfo(token));
											}
											m.Name = token.getText();
											m.ExplicitInfo  = emi;
											
											if (nameSplice != null) {
												typeMember = new SpliceTypeMember(m, nameSplice);
											} else {
												typeMember = m;
											}
										
						}
					}
					break;
				}
				case CONSTRUCTOR:
				{
					c = LT(1);
					match(CONSTRUCTOR);
					if (0==inputState.guessing)
					{
						typeMember = m = new Constructor(ToLexicalInfo(c));
					}
					break;
				}
				case DESTRUCTOR:
				{
					d = LT(1);
					match(DESTRUCTOR);
					if (0==inputState.guessing)
					{
						typeMember = m = new Destructor(ToLexicalInfo(d));
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
						case THEN:
						case LET:
						case WHERE:
						case JOIN:
						case ON:
						case EQUALS:
						case INTO:
						case ORDERBY:
						case ASCENDING:
						case DESCENDING:
						case SELECT:
						case GROUP:
						case BY:
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
				
						container.Add(typeMember);
						m.EndSourceLocation = body.EndSourceLocation;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "method");
				recover(ex,tokenSet_23_);
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
		
		IToken  begin = null;
		
				TypeDefinition td = null;
				TypeReferenceCollection baseTypes = null;
				TypeMemberCollection members = null;
				GenericParameterDeclarationCollection genericParameters = null;
				Expression nameSplice = null;
				IToken id = null;
			
		
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
			{
				switch ( LA(1) )
				{
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case ID:
				{
					id=macro_name();
					break;
				}
				case SPLICE_BEGIN:
				{
					begin = LT(1);
					match(SPLICE_BEGIN);
					nameSplice=atom();
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
						
						IToken token = id ?? begin;
						td.LexicalInfo = ToLexicalInfo(token);
						td.Name = token.getText();
						td.Modifiers = _modifiers;
						AddAttributes(td.Attributes);
						baseTypes = td.BaseTypes;
						members = td.Members;
						genericParameters = td.GenericParameters;
						
						if (id != null) {
							container.Add(td);
						} else {
							container.Add(new SpliceTypeMember(td, nameSplice));
						}
					
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
						case THEN:
						case LET:
						case WHERE:
						case JOIN:
						case ON:
						case EQUALS:
						case INTO:
						case ORDERBY:
						case ASCENDING:
						case DESCENDING:
						case SELECT:
						case GROUP:
						case BY:
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
				case EOL:
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
				case PARTIAL:
				case PUBLIC:
				case PROTECTED:
				case PRIVATE:
				case SELF:
				case STATIC:
				case STRUCT:
				case THEN:
				case TRANSIENT:
				case VIRTUAL:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case EOS:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				{
					{
						{
							switch ( LA(1) )
							{
							case EOL:
							case EOS:
							{
								eos();
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
							case PARTIAL:
							case PUBLIC:
							case PROTECTED:
							case PRIVATE:
							case SELF:
							case STATIC:
							case STRUCT:
							case THEN:
							case TRANSIENT:
							case VIRTUAL:
							case LET:
							case WHERE:
							case JOIN:
							case ON:
							case EQUALS:
							case INTO:
							case ORDERBY:
							case ASCENDING:
							case DESCENDING:
							case SELECT:
							case GROUP:
							case BY:
							case LBRACK:
							case SPLICE_BEGIN:
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
						{ // ( ... )+
							int _cnt76=0;
							for (;;)
							{
								bool synPredMatched75 = false;
								if (((LA(1)==SPLICE_BEGIN) && (tokenSet_36_.member(LA(2)))))
								{
									int _m75 = mark();
									synPredMatched75 = true;
									inputState.guessing++;
									try {
										{
											splice_expression();
											eos();
										}
									}
									catch (RecognitionException)
									{
										synPredMatched75 = false;
									}
									rewind(_m75);
									inputState.guessing--;
								}
								if ( synPredMatched75 )
								{
									splice_type_definition_body(members);
								}
								else if ((tokenSet_37_.member(LA(1))) && (tokenSet_38_.member(LA(2)))) {
									type_definition_member(members);
								}
								else
								{
									if (_cnt76 >= 1) { goto _loop76_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
								}
								
								_cnt76++;
							}
_loop76_breakloop:							;
						}    // ( ... )+
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			end(td);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "class_definition");
				recover(ex,tokenSet_23_);
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
		
		IToken  begin = null;
		
				InterfaceDefinition itf = null;
				TypeMemberCollection members = null;
				GenericParameterDeclarationCollection genericParameters = null;
				Expression nameSplice = null;
				IToken id = null;
			
		
		try {      // for error handling
			match(INTERFACE);
			{
				switch ( LA(1) )
				{
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case ID:
				{
					id=macro_name();
					break;
				}
				case SPLICE_BEGIN:
				{
					{
						begin = LT(1);
						match(SPLICE_BEGIN);
						nameSplice=atom();
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
				
						IToken token = id ?? begin;
						itf = new InterfaceDefinition(ToLexicalInfo(token));
						itf.Name = token.getText();
						itf.Modifiers = _modifiers;
						AddAttributes(itf.Attributes);
						if (id != null) {
							container.Add(itf);
						} else {
							container.Add(new SpliceTypeMember(itf, nameSplice));
						}
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
						case THEN:
						case LET:
						case WHERE:
						case JOIN:
						case ON:
						case EQUALS:
						case INTO:
						case ORDERBY:
						case ASCENDING:
						case DESCENDING:
						case SELECT:
						case GROUP:
						case BY:
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
				case EVENT:
				case SELF:
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case LBRACK:
				case ID:
				{
					{ // ( ... )+
						int _cnt90=0;
						for (;;)
						{
							if ((tokenSet_39_.member(LA(1))))
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
									case THEN:
									case LET:
									case WHERE:
									case JOIN:
									case ON:
									case EQUALS:
									case INTO:
									case ORDERBY:
									case ASCENDING:
									case DESCENDING:
									case SELECT:
									case GROUP:
									case BY:
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
								if (_cnt90 >= 1) { goto _loop90_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
							}
							
							_cnt90++;
						}
_loop90_breakloop:						;
					}    // ( ... )+
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			end(itf);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "interface_definition");
				recover(ex,tokenSet_23_);
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
		
		
				EnumDefinition ed = null;
				TypeMemberCollection members = null;
				IToken id = null;
			
		
		try {      // for error handling
			match(ENUM);
			id=macro_name();
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
							members = ed.Members;
						
			}
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
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				{
					{ // ( ... )+
						int _cnt48=0;
						for (;;)
						{
							switch ( LA(1) )
							{
							case THEN:
							case LET:
							case WHERE:
							case JOIN:
							case ON:
							case EQUALS:
							case INTO:
							case ORDERBY:
							case ASCENDING:
							case DESCENDING:
							case SELECT:
							case GROUP:
							case BY:
							case LBRACK:
							case ID:
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
_loop48_breakloop:						;
					}    // ( ... )+
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			end(ed);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "enum_definition");
				recover(ex,tokenSet_23_);
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
		
		
				CallableDefinition cd = null;
				TypeReference returnType = null;
				GenericParameterDeclarationCollection genericParameters = null;
				IToken id = null;
			
		
		try {      // for error handling
			match(CALLABLE);
			id=macro_name();
			if (0==inputState.guessing)
			{
				
						cd = new CallableDefinition(ToLexicalInfo(id));
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
						case THEN:
						case LET:
						case WHERE:
						case JOIN:
						case ON:
						case EQUALS:
						case INTO:
						case ORDERBY:
						case ASCENDING:
						case DESCENDING:
						case SELECT:
						case GROUP:
						case BY:
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
				case EOL:
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
				reportError(ex, "callable_definition");
				recover(ex,tokenSet_23_);
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
						goto _loop231_breakloop;
					}
					
				}
_loop231_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "generic_parameter_declaration_list");
				recover(ex,tokenSet_40_);
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
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
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
								goto _loop208_breakloop;
							}
							
						}
_loop208_breakloop:						;
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
				reportError(ex, "parameter_declaration_list");
				recover(ex,tokenSet_41_);
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
					bool synPredMatched249 = false;
					if (((LA(1)==CALLABLE) && (LA(2)==LPAREN)))
					{
						int _m249 = mark();
						synPredMatched249 = true;
						inputState.guessing++;
						try {
							{
								match(CALLABLE);
								match(LPAREN);
							}
						}
						catch (RecognitionException)
						{
							synPredMatched249 = false;
						}
						rewind(_m249);
						inputState.guessing--;
					}
					if ( synPredMatched249 )
					{
						{
							tr=callable_type_reference();
						}
					}
					else if ((tokenSet_42_.member(LA(1))) && (tokenSet_43_.member(LA(2)))) {
						{
							id=type_name();
							{
								if ((LA(1)==LBRACK) && (tokenSet_44_.member(LA(2))))
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
											case THEN:
											case LET:
											case WHERE:
											case JOIN:
											case ON:
											case EQUALS:
											case INTO:
											case ORDERBY:
											case ASCENDING:
											case DESCENDING:
											case SELECT:
											case GROUP:
											case BY:
											case LPAREN:
											case MULTIPLY:
											case SPLICE_BEGIN:
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
																goto _loop258_breakloop;
															}
															
														}
_loop258_breakloop:														;
													}    // ( ... )*
													match(RBRACK);
												}
												break;
											}
											case CALLABLE:
											case CHAR:
											case THEN:
											case LET:
											case WHERE:
											case JOIN:
											case ON:
											case EQUALS:
											case INTO:
											case ORDERBY:
											case ASCENDING:
											case DESCENDING:
											case SELECT:
											case GROUP:
											case BY:
											case LPAREN:
											case SPLICE_BEGIN:
											case ID:
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
								else if ((LA(1)==OF) && (tokenSet_45_.member(LA(2)))) {
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
								else if ((tokenSet_43_.member(LA(1))) && (tokenSet_16_.member(LA(2)))) {
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
								if ((LA(1)==NULLABLE_SUFFIX) && (tokenSet_43_.member(LA(2))))
								{
									match(NULLABLE_SUFFIX);
									if (0==inputState.guessing)
									{
										
														GenericTypeReference ntr = new GenericTypeReference(tr.LexicalInfo, "System.Nullable");
														ntr.GenericArguments.Add(tr);
														tr = ntr;
													
									}
								}
								else if ((tokenSet_43_.member(LA(1))) && (tokenSet_16_.member(LA(2)))) {
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
					if ((LA(1)==MULTIPLY) && (tokenSet_43_.member(LA(2))))
					{
						match(MULTIPLY);
						if (0==inputState.guessing)
						{
							tr = CodeFactory.EnumerableTypeReferenceFor(tr);
						}
					}
					else if ((LA(1)==EXPONENTIATION) && (tokenSet_43_.member(LA(2)))) {
						match(EXPONENTIATION);
						if (0==inputState.guessing)
						{
							tr = CodeFactory.EnumerableTypeReferenceFor(CodeFactory.EnumerableTypeReferenceFor(tr));
						}
					}
					else
					{
						goto _loop264_breakloop;
					}
					
				}
_loop264_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "type_reference");
				recover(ex,tokenSet_43_);
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
				switch ( LA(1) )
				{
				case EOL:
				case EOS:
				{
					eos();
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
				reportError(ex, "begin_with_doc");
				recover(ex,tokenSet_46_);
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
		
			
				EnumMember em = null;
				Expression initializer = null;
				IToken id = null;
			
		
		try {      // for error handling
			attributes();
			id=macro_name();
			{
				switch ( LA(1) )
				{
				case ASSIGN:
				{
					match(ASSIGN);
					initializer=simple_initializer();
					break;
				}
				case EOL:
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
				reportError(ex, "enum_member");
				recover(ex,tokenSet_47_);
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
				reportError(ex, "splice_type_definition_body");
				recover(ex,tokenSet_48_);
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
			match(DEDENT);
			if (0==inputState.guessing)
			{
				SetEndSourceLocation(node, t);
			}
			{
				if ((LA(1)==EOL||LA(1)==EOS) && (tokenSet_49_.member(LA(2))))
				{
					eos();
				}
				else if ((tokenSet_49_.member(LA(1))) && (tokenSet_50_.member(LA(2)))) {
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
				reportError(ex, "end");
				recover(ex,tokenSet_49_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public Expression  simple_initializer() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		
			e = null;
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case ESEPARATOR:
			case CAST:
			case CHAR:
			case FROM:
			case FALSE:
			case NOT:
			case NULL:
			case SELF:
			case SUPER:
			case THEN:
			case TRUE:
			case TYPEOF:
			case LET:
			case WHERE:
			case JOIN:
			case ON:
			case EQUALS:
			case INTO:
			case ORDERBY:
			case ASCENDING:
			case DESCENDING:
			case SELECT:
			case GROUP:
			case BY:
			case TRIPLE_QUOTED_STRING:
			case LPAREN:
			case DOUBLE_QUOTED_STRING:
			case SINGLE_QUOTED_STRING:
			case MULTIPLY:
			case LBRACK:
			case COMMA:
			case SPLICE_BEGIN:
			case ID:
			case DOT:
			case LBRACE:
			case QQ_BEGIN:
			case SUBTRACT:
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
			case DEF:
			case DO:
			case COLON:
			{
				e=callable_expression();
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
				reportError(ex, "simple_initializer");
				recover(ex,tokenSet_23_);
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
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
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
				reportError(ex, "attribute");
				recover(ex,tokenSet_51_);
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
				case FROM:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
								goto _loop680_breakloop;
							}
							
						}
_loop680_breakloop:						;
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
				reportError(ex, "argument_list");
				recover(ex,tokenSet_52_);
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
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
					bool synPredMatched554 = false;
					if (((LA(1)==CHAR) && (LA(2)==LPAREN)))
					{
						int _m554 = mark();
						synPredMatched554 = true;
						inputState.guessing++;
						try {
							{
								match(CHAR);
								match(LPAREN);
							}
						}
						catch (RecognitionException)
						{
							synPredMatched554 = false;
						}
						rewind(_m554);
						inputState.guessing--;
					}
					if ( synPredMatched554 )
					{
						e=char_literal();
					}
					else if ((tokenSet_53_.member(LA(1))) && (tokenSet_43_.member(LA(2)))) {
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
				reportError(ex, "atom");
				recover(ex,tokenSet_43_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
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
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case LPAREN:
				case SPLICE_BEGIN:
				case ID:
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
								goto _loop94_breakloop;
							}
							
						}
_loop94_breakloop:						;
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
				reportError(ex, "base_types");
				recover(ex,tokenSet_54_);
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
				
						e = new SpliceExpression(ToLexicalInfo(begin), e);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "splice_expression");
				recover(ex,tokenSet_43_);
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
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case SPLICE_BEGIN:
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
				reportError(ex, "type_definition_member");
				recover(ex,tokenSet_55_);
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
		
				Event e = null;
				TypeReference tr = null;
				IToken id = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(EVENT);
			id=macro_name();
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
				reportError(ex, "event_declaration");
				recover(ex,tokenSet_55_);
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
		
		IToken  begin1 = null;
		IToken  s = null;
		IToken  lparen = null;
		IToken  begin2 = null;
		
			IToken id = null;
			IToken id2 = null;
			TypeMember tm = null;
			TypeReference tr = null;
			Property p = null;
			Field field = null;
			ExplicitMemberInfo emi = null;
			Expression initializer = null;
			ParameterDeclarationCollection parameters = null;
			Expression nameSplice = null;
		
		
		try {      // for error handling
			{
				bool synPredMatched154 = false;
				if (((tokenSet_56_.member(LA(1))) && (tokenSet_57_.member(LA(2)))))
				{
					int _m154 = mark();
					synPredMatched154 = true;
					inputState.guessing++;
					try {
						{
							property_header();
						}
					}
					catch (RecognitionException)
					{
						synPredMatched154 = false;
					}
					rewind(_m154);
					inputState.guessing--;
				}
				if ( synPredMatched154 )
				{
					{
						{
							if ((tokenSet_4_.member(LA(1))) && (LA(2)==DOT))
							{
								emi=explicit_member_info();
							}
							else if ((tokenSet_56_.member(LA(1))) && (tokenSet_57_.member(LA(2)))) {
							}
							else
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							
						}
						{
							switch ( LA(1) )
							{
							case THEN:
							case LET:
							case WHERE:
							case JOIN:
							case ON:
							case EQUALS:
							case INTO:
							case ORDERBY:
							case ASCENDING:
							case DESCENDING:
							case SELECT:
							case GROUP:
							case BY:
							case ID:
							{
								id=macro_name();
								break;
							}
							case SPLICE_BEGIN:
							{
								begin1 = LT(1);
								match(SPLICE_BEGIN);
								nameSplice=atom();
								if (0==inputState.guessing)
								{
									id=begin1;
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
													p = new Property(ToLexicalInfo(id));
												p.Name = id.getText();
												p.ExplicitInfo = emi;
												AddAttributes(p.Attributes);
												parameters = p.Parameters;
												tm = p;
											
							}
							{
								switch ( LA(1) )
								{
								case LPAREN:
								{
									{
										lparen = LT(1);
										match(LPAREN);
										parameter_declaration_list(parameters);
										match(RPAREN);
										if (0==inputState.guessing)
										{
											EmitIndexedPropertyDeprecationWarning(p);
										}
									}
									break;
								}
								case LBRACK:
								{
									{
										match(LBRACK);
										parameter_declaration_list(parameters);
										match(RBRACK);
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
												p.Modifiers = _modifiers;
											
							}
							begin_with_doc(p);
							{ // ( ... )+
								int _cnt164=0;
								for (;;)
								{
									if ((tokenSet_58_.member(LA(1))))
									{
										property_accessor(p);
									}
									else
									{
										if (_cnt164 >= 1) { goto _loop164_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
									}
									
									_cnt164++;
								}
_loop164_breakloop:								;
							}    // ( ... )+
							end(p);
						}
					}
				}
				else {
					bool synPredMatched167 = false;
					if (((tokenSet_4_.member(LA(1))) && (tokenSet_5_.member(LA(2)))))
					{
						int _m167 = mark();
						synPredMatched167 = true;
						inputState.guessing++;
						try {
							{
								macro_name();
								expression_list(null);
								{
									switch ( LA(1) )
									{
									case EOL:
									case EOS:
									{
										eos();
										break;
									}
									case COLON:
									{
										begin_with_doc(null);
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
							synPredMatched167 = false;
						}
						rewind(_m167);
						inputState.guessing--;
					}
					if ( synPredMatched167 )
					{
						tm=member_macro();
					}
					else if ((tokenSet_59_.member(LA(1))) && (tokenSet_60_.member(LA(2)))) {
						{
							{
								switch ( LA(1) )
								{
								case THEN:
								case LET:
								case WHERE:
								case JOIN:
								case ON:
								case EQUALS:
								case INTO:
								case ORDERBY:
								case ASCENDING:
								case DESCENDING:
								case SELECT:
								case GROUP:
								case BY:
								case ID:
								{
									id2=macro_name();
									break;
								}
								case SPLICE_BEGIN:
								{
									begin2 = LT(1);
									match(SPLICE_BEGIN);
									nameSplice=atom();
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
								
											IToken token = id2 ?? begin2;
											field = new Field(ToLexicalInfo(token));
											field.Name = token.getText();
											field.Modifiers = _modifiers;
											AddAttributes(field.Attributes);
											tm = field;
										
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
									case EOL:
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
									case EOL:
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
					}
					else
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					}
				}
				if (0==inputState.guessing)
				{
					
						if (null != nameSplice) {
							tm = new SpliceTypeMember(tm, nameSplice);
						}
						container.Add(tm);
					
				}
			}
			catch (RecognitionException ex)
			{
				if (0 == inputState.guessing)
				{
					reportError(ex, "field_or_property");
					recover(ex,tokenSet_55_);
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
		
		IToken  begin = null;
		
				Method m = null;
				TypeReference rt = null;
				Expression nameSplice = null;
				IToken id = null;
			
		
		try {      // for error handling
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
				case THEN:
				case YIELD:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case ID:
				{
					id=member();
					break;
				}
				case SPLICE_BEGIN:
				{
					{
						begin = LT(1);
						match(SPLICE_BEGIN);
						nameSplice=atom();
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
				
						IToken token = id ?? begin;
						m = new Method(ToLexicalInfo(token));
						m.Name = token.getText();
						AddAttributes(m.Attributes);
						if (nameSplice != null) {
							container.Add(new SpliceTypeMember(m, nameSplice));
						} else {
							container.Add(m);
						}
					
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
							case THEN:
							case LET:
							case WHERE:
							case JOIN:
							case ON:
							case EQUALS:
							case INTO:
							case ORDERBY:
							case ASCENDING:
							case DESCENDING:
							case SELECT:
							case GROUP:
							case BY:
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
				case EOL:
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
				case EOL:
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
						{
							switch ( LA(1) )
							{
							case EOL:
							case EOS:
							{
								eos();
								break;
							}
							case DEDENT:
							case DEF:
							case EVENT:
							case SELF:
							case THEN:
							case LET:
							case WHERE:
							case JOIN:
							case ON:
							case EQUALS:
							case INTO:
							case ORDERBY:
							case ASCENDING:
							case DESCENDING:
							case SELECT:
							case GROUP:
							case BY:
							case LBRACK:
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
				reportError(ex, "interface_method");
				recover(ex,tokenSet_61_);
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
		
		IToken  s = null;
		
				IToken id = null;
		Property p = null;
		TypeReference tr = null;
		ParameterDeclarationCollection parameters = null;
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case ID:
				{
					id=macro_name();
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
				
				p = new Property(ToLexicalInfo(id));
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
				int _cnt114=0;
				for (;;)
				{
					if ((LA(1)==GET||LA(1)==SET||LA(1)==LBRACK))
					{
						interface_property_accessor(p);
					}
					else
					{
						if (_cnt114 >= 1) { goto _loop114_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt114++;
				}
_loop114_breakloop:				;
			}    // ( ... )+
			end(p);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "interface_property");
				recover(ex,tokenSet_61_);
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
		
		IToken  set = null;
		IToken  get = null;
		IToken  t1 = null;
		IToken  t2 = null;
		IToken  t3 = null;
		IToken  ev = null;
		IToken  r = null;
		IToken  y = null;
		
				name = null;
				IToken id = null;
			
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case THEN:
			case LET:
			case WHERE:
			case JOIN:
			case ON:
			case EQUALS:
			case INTO:
			case ORDERBY:
			case ASCENDING:
			case DESCENDING:
			case SELECT:
			case GROUP:
			case BY:
			case ID:
			{
				id=macro_name();
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
			case YIELD:
			{
				y = LT(1);
				match(YIELD);
				if (0==inputState.guessing)
				{
					name=y;
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
				reportError(ex, "member");
				recover(ex,tokenSet_29_);
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
		
		
				GenericParameterDeclaration gpd = null;
				IToken id = null;
			
		
		try {      // for error handling
			id=macro_name();
			if (0==inputState.guessing)
			{
				
						gpd = new GenericParameterDeclaration(ToLexicalInfo(id));
						gpd.Name = id.getText();
						c.Add(gpd);
					
			}
			{
				if ((LA(1)==LPAREN) && (tokenSet_62_.member(LA(2))))
				{
					match(LPAREN);
					generic_parameter_constraints(gpd);
					match(RPAREN);
				}
				else if ((LA(1)==LPAREN||LA(1)==RBRACK||LA(1)==COMMA) && (tokenSet_63_.member(LA(2)))) {
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
				reportError(ex, "generic_parameter_declaration");
				recover(ex,tokenSet_64_);
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
			match(PASS);
			eos();
			end(node);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "empty_block");
				recover(ex,tokenSet_65_);
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
				case EOL:
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
				reportError(ex, "interface_property_accessor");
				recover(ex,tokenSet_66_);
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
			match(INDENT);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "begin");
				recover(ex,tokenSet_67_);
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
		
		
				emi = null; _sbuilder.Length = 0;
				IToken id = null;
				IToken id2 = null;
			
		
		try {      // for error handling
			{
				{
					{
						id=macro_name();
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
							if ((tokenSet_4_.member(LA(1))) && (LA(2)==DOT))
							{
								{
									id2=macro_name();
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
								goto _loop130_breakloop;
							}
							
						}
_loop130_breakloop:						;
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
				reportError(ex, "explicit_member_info");
				recover(ex,tokenSet_68_);
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
			match(COLON);
			{
				switch ( LA(1) )
				{
				case EOL:
				case EOS:
				{
					eos();
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
			begin = LT(1);
			match(INDENT);
			if (0==inputState.guessing)
			{
				
						block.LexicalInfo = ToLexicalInfo(begin);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "begin_block_with_doc");
				recover(ex,tokenSet_69_);
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
				case EOL:
				case EOS:
				{
					eos();
					break;
				}
				case ESEPARATOR:
				case BREAK:
				case CONTINUE:
				case CAST:
				case CHAR:
				case DEF:
				case FROM:
				case FOR:
				case FALSE:
				case GOTO:
				case IF:
				case NULL:
				case PASS:
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
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case COLON:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
				case ESEPARATOR:
				case BREAK:
				case CONTINUE:
				case CAST:
				case CHAR:
				case DEF:
				case FROM:
				case FOR:
				case FALSE:
				case GOTO:
				case IF:
				case NULL:
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
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case COLON:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
					{ // ( ... )+
						int _cnt197=0;
						for (;;)
						{
							if ((tokenSet_18_.member(LA(1))))
							{
								stmt(container);
							}
							else
							{
								if (_cnt197 >= 1) { goto _loop197_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
							}
							
							_cnt197++;
						}
_loop197_breakloop:						;
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
				reportError(ex, "block");
				recover(ex,tokenSet_70_);
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
					case SPLICE_BEGIN:
					{
						{
							match(SPLICE_BEGIN);
							atom();
						}
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
							goto _loop145_breakloop;
						}
						
					}
_loop145_breakloop:					;
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
				case AS:
				case LPAREN:
				case COLON:
				{
					{
						{
							switch ( LA(1) )
							{
							case LPAREN:
							{
								match(LPAREN);
								parameter_declaration_list(null);
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
						attributes();
						modifiers();
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
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "property_header");
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
				if (((LA(1)==GET))&&( null != p && null == p.Getter ))
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
				else if (((LA(1)==SET))&&( null != p && null == p.Setter )) {
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
			{
				switch ( LA(1) )
				{
				case EOL:
				case EOS:
				{
					eos();
					break;
				}
				case COLON:
				{
					compound_stmt(body);
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
				reportError(ex, "property_accessor");
				recover(ex,tokenSet_71_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected TypeMember  member_macro() //throws RecognitionException, TokenStreamException
{
		TypeMember result;
		
		
			MacroStatement s = null;
			result = null;
		
		
		try {      // for error handling
			s=macro_stmt();
			if (0==inputState.guessing)
			{
				
						result = new StatementTypeMember(s);
						result.Modifiers = _modifiers;
						AddAttributes(result.Attributes);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "member_macro");
				recover(ex,tokenSet_55_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expression  declaration_initializer() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		
			e = null;
		
		
		try {      // for error handling
			bool synPredMatched178 = false;
			if (((tokenSet_36_.member(LA(1))) && (tokenSet_72_.member(LA(2)))))
			{
				int _m178 = mark();
				synPredMatched178 = true;
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
					synPredMatched178 = false;
				}
				rewind(_m178);
				inputState.guessing--;
			}
			if ( synPredMatched178 )
			{
				{
					e=slicing_expression();
					e=method_invocation_block(e);
				}
			}
			else if ((tokenSet_73_.member(LA(1))) && (tokenSet_74_.member(LA(2)))) {
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
				reportError(ex, "declaration_initializer");
				recover(ex,tokenSet_23_);
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
		IToken  begin = null;
		IToken  lparen = null;
		
				e = null;
				SlicingExpression se = null;
				MethodInvocationExpression mce = null;
				IToken memberName = null;
				TypeReference genericArgument = null;
				TypeReferenceCollection genericArguments = null;
				Expression nameSplice = null;
				Expression initializer = null;
				UnaryExpression ue = null;		
			
		
		try {      // for error handling
			e=safe_atom();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==LBRACK) && (tokenSet_75_.member(LA(2))))
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
											
																	GenericReferenceExpression gre = new GenericReferenceExpression(ToLexicalInfo(lbrack));
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
								case FROM:
								case FALSE:
								case NOT:
								case NULL:
								case SELF:
								case SUPER:
								case THEN:
								case TRUE:
								case TYPEOF:
								case LET:
								case WHERE:
								case JOIN:
								case ON:
								case EQUALS:
								case INTO:
								case ORDERBY:
								case ASCENDING:
								case DESCENDING:
								case SELECT:
								case GROUP:
								case BY:
								case TRIPLE_QUOTED_STRING:
								case LPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case MULTIPLY:
								case LBRACK:
								case SPLICE_BEGIN:
								case ID:
								case DOT:
								case COLON:
								case LBRACE:
								case QQ_BEGIN:
								case SUBTRACT:
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
												goto _loop614_breakloop;
											}
											
										}
_loop614_breakloop:										;
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
							{
								switch ( LA(1) )
								{
								case NULLABLE_SUFFIX:
								{
									match(NULLABLE_SUFFIX);
									if (0==inputState.guessing)
									{
										
															ue = new UnaryExpression(e.LexicalInfo);
															ue.Operator = UnaryOperatorType.SafeAccess;
															ue.Operand = e;
															e = ue;
														
									}
									break;
								}
								case EOF:
								case DEDENT:
								case ESEPARATOR:
								case EOL:
								case ASSEMBLY_ATTRIBUTE_BEGIN:
								case MODULE_ATTRIBUTE_BEGIN:
								case ABSTRACT:
								case AND:
								case AS:
								case BREAK:
								case CONTINUE:
								case CALLABLE:
								case CAST:
								case CHAR:
								case CLASS:
								case DEF:
								case DO:
								case ELSE:
								case ENUM:
								case EVENT:
								case FINAL:
								case FROM:
								case FOR:
								case FALSE:
								case GOTO:
								case INTERFACE:
								case INTERNAL:
								case IS:
								case ISA:
								case IF:
								case IN:
								case NEW:
								case NOT:
								case NULL:
								case OF:
								case OR:
								case OVERRIDE:
								case PARTIAL:
								case PUBLIC:
								case PROTECTED:
								case PRIVATE:
								case RAISE:
								case RETURN:
								case SELF:
								case SUPER:
								case STATIC:
								case STRUCT:
								case THEN:
								case TRY:
								case TRANSIENT:
								case TRUE:
								case TYPEOF:
								case UNLESS:
								case VIRTUAL:
								case WHILE:
								case YIELD:
								case LET:
								case WHERE:
								case JOIN:
								case ON:
								case EQUALS:
								case INTO:
								case ORDERBY:
								case ASCENDING:
								case DESCENDING:
								case SELECT:
								case GROUP:
								case BY:
								case TRIPLE_QUOTED_STRING:
								case EOS:
								case LPAREN:
								case RPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case MULTIPLY:
								case LBRACK:
								case RBRACK:
								case ASSIGN:
								case COMMA:
								case SPLICE_BEGIN:
								case ID:
								case DOT:
								case COLON:
								case EXPONENTIATION:
								case BITWISE_OR:
								case LBRACE:
								case RBRACE:
								case QQ_BEGIN:
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
								case SUBTRACT:
								case EXCLUSIVE_OR:
								case DIVISION:
								case MODULUS:
								case BITWISE_AND:
								case SHIFT_LEFT:
								case SHIFT_RIGHT:
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
						}
					}
					else if ((LA(1)==OF)) {
						{
							oft = LT(1);
							match(OF);
							genericArgument=type_reference();
							if (0==inputState.guessing)
							{
								
												GenericReferenceExpression gre = new GenericReferenceExpression(ToLexicalInfo(oft));
												gre.Target = e;
												e = gre;
												gre.GenericArguments.Add(genericArgument);
											
							}
						}
					}
					else if ((LA(1)==DOT) && (tokenSet_35_.member(LA(2)))) {
						{
							match(DOT);
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
								case THEN:
								case YIELD:
								case LET:
								case WHERE:
								case JOIN:
								case ON:
								case EQUALS:
								case INTO:
								case ORDERBY:
								case ASCENDING:
								case DESCENDING:
								case SELECT:
								case GROUP:
								case BY:
								case ID:
								{
									{
										memberName=member();
										if (0==inputState.guessing)
										{
											
																	e = MemberReferenceForToken(e, memberName);
																
										}
									}
									break;
								}
								case SPLICE_BEGIN:
								{
									{
										begin = LT(1);
										match(SPLICE_BEGIN);
										nameSplice=atom();
										if (0==inputState.guessing)
										{
											
																	e = new SpliceMemberReferenceExpression(
																				ToLexicalInfo(begin),
																				e,
																				nameSplice);
																
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
							{
								switch ( LA(1) )
								{
								case NULLABLE_SUFFIX:
								{
									match(NULLABLE_SUFFIX);
									if (0==inputState.guessing)
									{
										
															ue = new UnaryExpression(e.LexicalInfo);
															ue.Operator = UnaryOperatorType.SafeAccess;
															ue.Operand = e;
															e = ue;
														
									}
									break;
								}
								case EOF:
								case DEDENT:
								case ESEPARATOR:
								case EOL:
								case ASSEMBLY_ATTRIBUTE_BEGIN:
								case MODULE_ATTRIBUTE_BEGIN:
								case ABSTRACT:
								case AND:
								case AS:
								case BREAK:
								case CONTINUE:
								case CALLABLE:
								case CAST:
								case CHAR:
								case CLASS:
								case DEF:
								case DO:
								case ELSE:
								case ENUM:
								case EVENT:
								case FINAL:
								case FROM:
								case FOR:
								case FALSE:
								case GOTO:
								case INTERFACE:
								case INTERNAL:
								case IS:
								case ISA:
								case IF:
								case IN:
								case NEW:
								case NOT:
								case NULL:
								case OF:
								case OR:
								case OVERRIDE:
								case PARTIAL:
								case PUBLIC:
								case PROTECTED:
								case PRIVATE:
								case RAISE:
								case RETURN:
								case SELF:
								case SUPER:
								case STATIC:
								case STRUCT:
								case THEN:
								case TRY:
								case TRANSIENT:
								case TRUE:
								case TYPEOF:
								case UNLESS:
								case VIRTUAL:
								case WHILE:
								case YIELD:
								case LET:
								case WHERE:
								case JOIN:
								case ON:
								case EQUALS:
								case INTO:
								case ORDERBY:
								case ASCENDING:
								case DESCENDING:
								case SELECT:
								case GROUP:
								case BY:
								case TRIPLE_QUOTED_STRING:
								case EOS:
								case LPAREN:
								case RPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case MULTIPLY:
								case LBRACK:
								case RBRACK:
								case ASSIGN:
								case COMMA:
								case SPLICE_BEGIN:
								case ID:
								case DOT:
								case COLON:
								case EXPONENTIATION:
								case BITWISE_OR:
								case LBRACE:
								case RBRACE:
								case QQ_BEGIN:
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
								case SUBTRACT:
								case EXCLUSIVE_OR:
								case DIVISION:
								case MODULUS:
								case BITWISE_AND:
								case SHIFT_LEFT:
								case SHIFT_RIGHT:
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
						}
					}
					else if ((LA(1)==LPAREN) && (tokenSet_76_.member(LA(2)))) {
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
								case CAST:
								case CHAR:
								case FROM:
								case FALSE:
								case NOT:
								case NULL:
								case SELF:
								case SUPER:
								case THEN:
								case TRUE:
								case TYPEOF:
								case LET:
								case WHERE:
								case JOIN:
								case ON:
								case EQUALS:
								case INTO:
								case ORDERBY:
								case ASCENDING:
								case DESCENDING:
								case SELECT:
								case GROUP:
								case BY:
								case TRIPLE_QUOTED_STRING:
								case LPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case MULTIPLY:
								case LBRACK:
								case SPLICE_BEGIN:
								case ID:
								case DOT:
								case LBRACE:
								case QQ_BEGIN:
								case SUBTRACT:
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
												goto _loop625_breakloop;
											}
											
										}
_loop625_breakloop:										;
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
								case NULLABLE_SUFFIX:
								{
									match(NULLABLE_SUFFIX);
									if (0==inputState.guessing)
									{
										
															ue = new UnaryExpression(e.LexicalInfo);
															ue.Operator = UnaryOperatorType.SafeAccess;
															ue.Operand = e;
															e = ue;
														
									}
									break;
								}
								case EOF:
								case DEDENT:
								case ESEPARATOR:
								case EOL:
								case ASSEMBLY_ATTRIBUTE_BEGIN:
								case MODULE_ATTRIBUTE_BEGIN:
								case ABSTRACT:
								case AND:
								case AS:
								case BREAK:
								case CONTINUE:
								case CALLABLE:
								case CAST:
								case CHAR:
								case CLASS:
								case DEF:
								case DO:
								case ELSE:
								case ENUM:
								case EVENT:
								case FINAL:
								case FROM:
								case FOR:
								case FALSE:
								case GOTO:
								case INTERFACE:
								case INTERNAL:
								case IS:
								case ISA:
								case IF:
								case IN:
								case NEW:
								case NOT:
								case NULL:
								case OF:
								case OR:
								case OVERRIDE:
								case PARTIAL:
								case PUBLIC:
								case PROTECTED:
								case PRIVATE:
								case RAISE:
								case RETURN:
								case SELF:
								case SUPER:
								case STATIC:
								case STRUCT:
								case THEN:
								case TRY:
								case TRANSIENT:
								case TRUE:
								case TYPEOF:
								case UNLESS:
								case VIRTUAL:
								case WHILE:
								case YIELD:
								case LET:
								case WHERE:
								case JOIN:
								case ON:
								case EQUALS:
								case INTO:
								case ORDERBY:
								case ASCENDING:
								case DESCENDING:
								case SELECT:
								case GROUP:
								case BY:
								case TRIPLE_QUOTED_STRING:
								case EOS:
								case LPAREN:
								case RPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case MULTIPLY:
								case LBRACK:
								case RBRACK:
								case ASSIGN:
								case COMMA:
								case SPLICE_BEGIN:
								case ID:
								case DOT:
								case COLON:
								case EXPONENTIATION:
								case BITWISE_OR:
								case LBRACE:
								case RBRACE:
								case QQ_BEGIN:
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
								case SUBTRACT:
								case EXCLUSIVE_OR:
								case DIVISION:
								case MODULUS:
								case BITWISE_AND:
								case SHIFT_LEFT:
								case SHIFT_RIGHT:
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
							{
								if ((LA(1)==LBRACE) && (tokenSet_77_.member(LA(2))))
								{
									{
										bool synPredMatched630 = false;
										if (((LA(1)==LBRACE) && (tokenSet_77_.member(LA(2)))))
										{
											int _m630 = mark();
											synPredMatched630 = true;
											inputState.guessing++;
											try {
												{
													hash_literal_test();
												}
											}
											catch (RecognitionException)
											{
												synPredMatched630 = false;
											}
											rewind(_m630);
											inputState.guessing--;
										}
										if ( synPredMatched630 )
										{
											initializer=hash_literal();
										}
										else if ((LA(1)==LBRACE) && (tokenSet_77_.member(LA(2)))) {
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
								}
								else if ((tokenSet_78_.member(LA(1))) && (tokenSet_16_.member(LA(2)))) {
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
						goto _loop631_breakloop;
					}
					
				}
_loop631_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "slicing_expression");
				recover(ex,tokenSet_15_);
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
				
						mi = e as MethodInvocationExpression ?? new MethodInvocationExpression(e.LexicalInfo, e);
						mi.Arguments.Add(block);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "method_invocation_block");
				recover(ex,tokenSet_23_);
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
						e = new ArrayLiteralExpression(ToLexicalInfo(c));
					}
				}
				break;
			}
			case ESEPARATOR:
			case CAST:
			case CHAR:
			case FROM:
			case FALSE:
			case NOT:
			case NULL:
			case SELF:
			case SUPER:
			case THEN:
			case TRUE:
			case TYPEOF:
			case LET:
			case WHERE:
			case JOIN:
			case ON:
			case EQUALS:
			case INTO:
			case ORDERBY:
			case ASCENDING:
			case DESCENDING:
			case SELECT:
			case GROUP:
			case BY:
			case TRIPLE_QUOTED_STRING:
			case LPAREN:
			case DOUBLE_QUOTED_STRING:
			case SINGLE_QUOTED_STRING:
			case MULTIPLY:
			case LBRACK:
			case SPLICE_BEGIN:
			case ID:
			case DOT:
			case LBRACE:
			case QQ_BEGIN:
			case SUBTRACT:
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
								if ((tokenSet_6_.member(LA(1))) && (tokenSet_79_.member(LA(2))))
								{
									e=expression();
									if (0==inputState.guessing)
									{
										tle.Items.Add(e);
									}
									{    // ( ... )*
										for (;;)
										{
											if ((LA(1)==COMMA) && (tokenSet_6_.member(LA(2))))
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
												goto _loop429_breakloop;
											}
											
										}
_loop429_breakloop:										;
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
										case DEDENT:
										case ESEPARATOR:
										case EOL:
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
										case DO:
										case ENUM:
										case EVENT:
										case FINAL:
										case FROM:
										case FOR:
										case FALSE:
										case GOTO:
										case INTERFACE:
										case INTERNAL:
										case IF:
										case NEW:
										case NULL:
										case OVERRIDE:
										case PARTIAL:
										case PUBLIC:
										case PROTECTED:
										case PRIVATE:
										case RAISE:
										case RETURN:
										case SELF:
										case SUPER:
										case STATIC:
										case STRUCT:
										case THEN:
										case TRY:
										case TRANSIENT:
										case TRUE:
										case TYPEOF:
										case UNLESS:
										case VIRTUAL:
										case WHILE:
										case YIELD:
										case LET:
										case WHERE:
										case JOIN:
										case ON:
										case EQUALS:
										case INTO:
										case ORDERBY:
										case ASCENDING:
										case DESCENDING:
										case SELECT:
										case GROUP:
										case BY:
										case TRIPLE_QUOTED_STRING:
										case EOS:
										case LPAREN:
										case RPAREN:
										case DOUBLE_QUOTED_STRING:
										case SINGLE_QUOTED_STRING:
										case MULTIPLY:
										case LBRACK:
										case SPLICE_BEGIN:
										case ID:
										case DOT:
										case COLON:
										case LBRACE:
										case RBRACE:
										case QQ_BEGIN:
										case QQ_END:
										case SUBTRACT:
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
								}
								else if ((tokenSet_80_.member(LA(1))) && (tokenSet_16_.member(LA(2)))) {
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
							break;
						}
						case EOF:
						case DEDENT:
						case ESEPARATOR:
						case EOL:
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
						case DO:
						case ENUM:
						case EVENT:
						case FINAL:
						case FROM:
						case FOR:
						case FALSE:
						case GOTO:
						case INTERFACE:
						case INTERNAL:
						case IF:
						case NEW:
						case NULL:
						case OVERRIDE:
						case PARTIAL:
						case PUBLIC:
						case PROTECTED:
						case PRIVATE:
						case RAISE:
						case RETURN:
						case SELF:
						case SUPER:
						case STATIC:
						case STRUCT:
						case THEN:
						case TRY:
						case TRANSIENT:
						case TRUE:
						case TYPEOF:
						case UNLESS:
						case VIRTUAL:
						case WHILE:
						case YIELD:
						case LET:
						case WHERE:
						case JOIN:
						case ON:
						case EQUALS:
						case INTO:
						case ORDERBY:
						case ASCENDING:
						case DESCENDING:
						case SELECT:
						case GROUP:
						case BY:
						case TRIPLE_QUOTED_STRING:
						case EOS:
						case LPAREN:
						case RPAREN:
						case DOUBLE_QUOTED_STRING:
						case SINGLE_QUOTED_STRING:
						case MULTIPLY:
						case LBRACK:
						case SPLICE_BEGIN:
						case ID:
						case DOT:
						case COLON:
						case LBRACE:
						case RBRACE:
						case QQ_BEGIN:
						case QQ_END:
						case SUBTRACT:
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
				reportError(ex, "array_or_expression");
				recover(ex,tokenSet_80_);
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
						
									e = cbe = new BlockExpression(ToLexicalInfo(anchor));
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
				reportError(ex, "callable_expression");
				recover(ex,tokenSet_23_);
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
			if ((LA(1)==COLON) && (tokenSet_81_.member(LA(2))))
			{
				single_line_block(b);
			}
			else if ((LA(1)==COLON) && (LA(2)==INDENT)) {
				{
					match(COLON);
					begin = LT(1);
					match(INDENT);
					if (0==inputState.guessing)
					{
						
									b.LexicalInfo = ToLexicalInfo(begin);
									statements = b.Statements;
								
					}
					block(statements);
					end(b);
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
				reportError(ex, "compound_stmt");
				recover(ex,tokenSet_49_);
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
				case DEF:
				{
					s=nested_function();
					break;
				}
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
					bool synPredMatched309 = false;
					if ((((tokenSet_4_.member(LA(1))) && (tokenSet_5_.member(LA(2))))&&(IsValidMacroArgument(LA(2)))))
					{
						int _m309 = mark();
						synPredMatched309 = true;
						inputState.guessing++;
						try {
							{
								macro_name();
								{
									if ((tokenSet_6_.member(LA(1))))
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
							synPredMatched309 = false;
						}
						rewind(_m309);
						inputState.guessing--;
					}
					if ( synPredMatched309 )
					{
						s=macro_stmt();
					}
					else {
						bool synPredMatched313 = false;
						if (((tokenSet_36_.member(LA(1))) && (tokenSet_82_.member(LA(2)))))
						{
							int _m313 = mark();
							synPredMatched313 = true;
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
								synPredMatched313 = false;
							}
							rewind(_m313);
							inputState.guessing--;
						}
						if ( synPredMatched313 )
						{
							s=assignment_or_method_invocation_with_block_stmt();
						}
						else {
							bool synPredMatched315 = false;
							if (((tokenSet_4_.member(LA(1))) && (LA(2)==AS||LA(2)==COMMA)))
							{
								int _m315 = mark();
								synPredMatched315 = true;
								inputState.guessing++;
								try {
									{
										declaration();
										match(COMMA);
									}
								}
								catch (RecognitionException)
								{
									synPredMatched315 = false;
								}
								rewind(_m315);
								inputState.guessing--;
							}
							if ( synPredMatched315 )
							{
								s=unpack_stmt();
							}
							else if ((tokenSet_4_.member(LA(1))) && (LA(2)==AS)) {
								s=declaration_stmt();
							}
							else if ((tokenSet_83_.member(LA(1))) && (tokenSet_84_.member(LA(2)))) {
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
										case FROM:
										case FALSE:
										case NULL:
										case SELF:
										case SUPER:
										case THEN:
										case TRUE:
										case TYPEOF:
										case LET:
										case WHERE:
										case JOIN:
										case ON:
										case EQUALS:
										case INTO:
										case ORDERBY:
										case ASCENDING:
										case DESCENDING:
										case SELECT:
										case GROUP:
										case BY:
										case TRIPLE_QUOTED_STRING:
										case LPAREN:
										case DOUBLE_QUOTED_STRING:
										case SINGLE_QUOTED_STRING:
										case MULTIPLY:
										case LBRACK:
										case SPLICE_BEGIN:
										case ID:
										case DOT:
										case LBRACE:
										case QQ_BEGIN:
										case SUBTRACT:
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
												if (s != null) s.Modifier = m;
											}
											break;
										}
										case EOL:
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
						
								if (s != null && container != null)
								{
									container.Add(s);
								}
							
					}
				}
				catch (RecognitionException ex)
				{
					if (0 == inputState.guessing)
					{
						reportError(ex, "stmt");
						recover(ex,tokenSet_85_);
					}
					else
					{
						throw ex;
					}
				}
			}
			
	protected void type_member_modifier() //throws RecognitionException, TokenStreamException
{
		
		IToken  t = null;
		
		
		
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
				t = LT(1);
				match(TRANSIENT);
				if (0==inputState.guessing)
				{
					
							_modifiers |= TypeMemberModifiers.Transient;
							EmitTransientKeywordDeprecationWarning(ToLexicalInfo(t));
						
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
				reportError(ex, "type_member_modifier");
				recover(ex,tokenSet_86_);
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
				reportError(ex, "parameter_modifier");
				recover(ex,tokenSet_45_);
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
		
		IToken  begin1 = null;
		IToken  begin2 = null;
				
				IToken id = null;
				TypeReference tr = null;
				ParameterModifiers pm = ParameterModifiers.None;
				variableArguments = false;
				Expression nameSplice = null;
			
		
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
						{
							switch ( LA(1) )
							{
							case THEN:
							case LET:
							case WHERE:
							case JOIN:
							case ON:
							case EQUALS:
							case INTO:
							case ORDERBY:
							case ASCENDING:
							case DESCENDING:
							case SELECT:
							case GROUP:
							case BY:
							case ID:
							{
								id=macro_name();
								break;
							}
							case SPLICE_BEGIN:
							{
								begin1 = LT(1);
								match(SPLICE_BEGIN);
								nameSplice=atom();
								if (0==inputState.guessing)
								{
									id = begin1;
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
					}
					break;
				}
				case REF:
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case SPLICE_BEGIN:
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
							case THEN:
							case LET:
							case WHERE:
							case JOIN:
							case ON:
							case EQUALS:
							case INTO:
							case ORDERBY:
							case ASCENDING:
							case DESCENDING:
							case SELECT:
							case GROUP:
							case BY:
							case SPLICE_BEGIN:
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
							switch ( LA(1) )
							{
							case THEN:
							case LET:
							case WHERE:
							case JOIN:
							case ON:
							case EQUALS:
							case INTO:
							case ORDERBY:
							case ASCENDING:
							case DESCENDING:
							case SELECT:
							case GROUP:
							case BY:
							case ID:
							{
								id=macro_name();
								break;
							}
							case SPLICE_BEGIN:
							{
								begin2 = LT(1);
								match(SPLICE_BEGIN);
								nameSplice=atom();
								if (0==inputState.guessing)
								{
									id = begin2;
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
						
						c.Add(
							nameSplice != null
							? new SpliceParameterDeclaration(pd, nameSplice)
							: pd);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "parameter_declaration");
				recover(ex,tokenSet_87_);
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
				reportError(ex, "array_type_reference");
				recover(ex,tokenSet_43_);
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
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case LPAREN:
				case MULTIPLY:
				case SPLICE_BEGIN:
				case ID:
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
								goto _loop222_breakloop;
							}
							
						}
_loop222_breakloop:						;
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
				reportError(ex, "callable_parameter_declaration_list");
				recover(ex,tokenSet_52_);
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
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case LPAREN:
				case SPLICE_BEGIN:
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
							case CALLABLE:
							case CHAR:
							case THEN:
							case LET:
							case WHERE:
							case JOIN:
							case ON:
							case EQUALS:
							case INTO:
							case ORDERBY:
							case ASCENDING:
							case DESCENDING:
							case SELECT:
							case GROUP:
							case BY:
							case LPAREN:
							case SPLICE_BEGIN:
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
				reportError(ex, "callable_parameter_declaration");
				recover(ex,tokenSet_88_);
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
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case LPAREN:
				case SPLICE_BEGIN:
				case ID:
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
				reportError(ex, "generic_parameter_constraints");
				recover(ex,tokenSet_52_);
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
				if ((LA(1)==AS) && (tokenSet_45_.member(LA(2))))
				{
					match(AS);
					tr=type_reference();
					if (0==inputState.guessing)
					{
						
								ctr.ReturnType = tr; 
								
					}
				}
				else if ((tokenSet_43_.member(LA(1))) && (tokenSet_16_.member(LA(2)))) {
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
				reportError(ex, "callable_type_reference");
				recover(ex,tokenSet_43_);
			}
			else
			{
				throw ex;
			}
		}
		return ctr;
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
				reportError(ex, "integer_literal");
				recover(ex,tokenSet_43_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
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
						goto _loop244_breakloop;
					}
					
				}
_loop244_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "type_reference_list");
				recover(ex,tokenSet_40_);
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
				
						tr = new SpliceTypeReference(ToLexicalInfo(begin), e);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "splice_type_reference");
				recover(ex,tokenSet_43_);
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
			case THEN:
			case LET:
			case WHERE:
			case JOIN:
			case ON:
			case EQUALS:
			case INTO:
			case ORDERBY:
			case ASCENDING:
			case DESCENDING:
			case SELECT:
			case GROUP:
			case BY:
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
				reportError(ex, "type_name");
				recover(ex,tokenSet_43_);
			}
			else
			{
				throw ex;
			}
		}
		return id;
	}
	
	protected void single_line_block(
		Block b
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  eolToken = null;
		
			StatementCollection statements = b != null ? b.Statements : null;
			IToken lastEOL = null;
		
		
		try {      // for error handling
			match(COLON);
			{
				switch ( LA(1) )
				{
				case PASS:
				{
					match(PASS);
					break;
				}
				case ESEPARATOR:
				case BREAK:
				case CONTINUE:
				case CAST:
				case CHAR:
				case FROM:
				case FALSE:
				case GOTO:
				case NULL:
				case RAISE:
				case RETURN:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case YIELD:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case COLON:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
						simple_stmt(statements);
						{    // ( ... )*
							for (;;)
							{
								if ((LA(1)==EOS))
								{
									match(EOS);
									{
										switch ( LA(1) )
										{
										case ESEPARATOR:
										case BREAK:
										case CONTINUE:
										case CAST:
										case CHAR:
										case FROM:
										case FALSE:
										case GOTO:
										case NULL:
										case RAISE:
										case RETURN:
										case SELF:
										case SUPER:
										case THEN:
										case TRUE:
										case TYPEOF:
										case YIELD:
										case LET:
										case WHERE:
										case JOIN:
										case ON:
										case EQUALS:
										case INTO:
										case ORDERBY:
										case ASCENDING:
										case DESCENDING:
										case SELECT:
										case GROUP:
										case BY:
										case TRIPLE_QUOTED_STRING:
										case LPAREN:
										case DOUBLE_QUOTED_STRING:
										case SINGLE_QUOTED_STRING:
										case MULTIPLY:
										case LBRACK:
										case SPLICE_BEGIN:
										case ID:
										case DOT:
										case COLON:
										case LBRACE:
										case QQ_BEGIN:
										case SUBTRACT:
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
											simple_stmt(statements);
											break;
										}
										case EOL:
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
								}
								else
								{
									goto _loop280_breakloop;
								}
								
							}
_loop280_breakloop:							;
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
			{ // ( ... )+
				int _cnt282=0;
				for (;;)
				{
					if ((LA(1)==EOL) && (tokenSet_49_.member(LA(2))))
					{
						eolToken = LT(1);
						match(EOL);
						if (0==inputState.guessing)
						{
							lastEOL = eolToken;
						}
					}
					else
					{
						if (_cnt282 >= 1) { goto _loop282_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt282++;
				}
_loop282_breakloop:				;
			}    // ( ... )+
			if (0==inputState.guessing)
			{
				SetEndSourceLocation(b, lastEOL);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "single_line_block");
				recover(ex,tokenSet_49_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void simple_stmt(
		StatementCollection container
	) //throws RecognitionException, TokenStreamException
{
		
		
				Statement s = null;
				_compact = true;
			
		
		try {      // for error handling
			{
				if (((tokenSet_4_.member(LA(1))) && (tokenSet_25_.member(LA(2))))&&(IsValidMacroArgument(LA(2))))
				{
					s=closure_macro_stmt();
				}
				else {
					bool synPredMatched322 = false;
					if (((tokenSet_36_.member(LA(1))) && (tokenSet_89_.member(LA(2)))))
					{
						int _m322 = mark();
						synPredMatched322 = true;
						inputState.guessing++;
						try {
							{
								slicing_expression();
								match(ASSIGN);
							}
						}
						catch (RecognitionException)
						{
							synPredMatched322 = false;
						}
						rewind(_m322);
						inputState.guessing--;
					}
					if ( synPredMatched322 )
					{
						s=assignment_or_method_invocation();
					}
					else if ((LA(1)==RETURN)) {
						s=return_expression_stmt();
					}
					else {
						bool synPredMatched324 = false;
						if (((tokenSet_4_.member(LA(1))) && (LA(2)==AS||LA(2)==COMMA)))
						{
							int _m324 = mark();
							synPredMatched324 = true;
							inputState.guessing++;
							try {
								{
									declaration();
									match(COMMA);
								}
							}
							catch (RecognitionException)
							{
								synPredMatched324 = false;
							}
							rewind(_m324);
							inputState.guessing--;
						}
						if ( synPredMatched324 )
						{
							s=unpack();
						}
						else if ((tokenSet_4_.member(LA(1))) && (LA(2)==AS)) {
							s=declaration_stmt();
						}
						else if ((tokenSet_83_.member(LA(1))) && (tokenSet_90_.member(LA(2)))) {
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
									case FROM:
									case FALSE:
									case NULL:
									case SELF:
									case SUPER:
									case THEN:
									case TRUE:
									case TYPEOF:
									case LET:
									case WHERE:
									case JOIN:
									case ON:
									case EQUALS:
									case INTO:
									case ORDERBY:
									case ASCENDING:
									case DESCENDING:
									case SELECT:
									case GROUP:
									case BY:
									case TRIPLE_QUOTED_STRING:
									case LPAREN:
									case DOUBLE_QUOTED_STRING:
									case SINGLE_QUOTED_STRING:
									case MULTIPLY:
									case LBRACK:
									case SPLICE_BEGIN:
									case ID:
									case DOT:
									case LBRACE:
									case QQ_BEGIN:
									case SUBTRACT:
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
							}
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						}}
					}
					if (0==inputState.guessing)
					{
						
								if (null != s)
								{
									container.Add(s);
								}
							
					}
					if (0==inputState.guessing)
					{
						_compact=false;
					}
				}
				catch (RecognitionException ex)
				{
					if (0 == inputState.guessing)
					{
						reportError(ex, "simple_stmt");
						recover(ex,tokenSet_20_);
					}
					else
					{
						throw ex;
					}
				}
			}
			
	protected MacroStatement  closure_macro_stmt() //throws RecognitionException, TokenStreamException
{
		MacroStatement returnValue;
		
		
				returnValue = null;
				antlr.IToken id = null;
				MacroStatement macro = new MacroStatement();
			
		
		try {      // for error handling
			id=macro_name();
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
				reportError(ex, "closure_macro_stmt");
				recover(ex,tokenSet_91_);
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
				case EOL:
				case EOS:
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
				case EVENT:
				case FINAL:
				case FROM:
				case FOR:
				case FALSE:
				case GOTO:
				case INTERFACE:
				case INTERNAL:
				case IF:
				case NEW:
				case NULL:
				case OVERRIDE:
				case PASS:
				case PARTIAL:
				case PUBLIC:
				case PROTECTED:
				case PRIVATE:
				case RAISE:
				case RETURN:
				case SELF:
				case SUPER:
				case STATIC:
				case STRUCT:
				case THEN:
				case TRY:
				case TRANSIENT:
				case TRUE:
				case TYPEOF:
				case UNLESS:
				case VIRTUAL:
				case WHILE:
				case YIELD:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case COLON:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
				case EVENT:
				case FINAL:
				case FROM:
				case FOR:
				case FALSE:
				case GOTO:
				case INTERFACE:
				case INTERNAL:
				case IF:
				case NEW:
				case NULL:
				case OVERRIDE:
				case PARTIAL:
				case PUBLIC:
				case PROTECTED:
				case PRIVATE:
				case RAISE:
				case RETURN:
				case SELF:
				case SUPER:
				case STATIC:
				case STRUCT:
				case THEN:
				case TRY:
				case TRANSIENT:
				case TRUE:
				case TYPEOF:
				case UNLESS:
				case VIRTUAL:
				case WHILE:
				case YIELD:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case COLON:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
					{ // ( ... )+
						int _cnt289=0;
						for (;;)
						{
							if ((tokenSet_18_.member(LA(1))) && (tokenSet_92_.member(LA(2))))
							{
								stmt(container);
							}
							else if ((tokenSet_37_.member(LA(1))) && (tokenSet_38_.member(LA(2)))) {
								type_member_stmt(container);
							}
							else
							{
								if (_cnt289 >= 1) { goto _loop289_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
							}
							
							_cnt289++;
						}
_loop289_breakloop:						;
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
				reportError(ex, "macro_block");
				recover(ex,tokenSet_70_);
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
			type_definition_member(members);
			if (0==inputState.guessing)
			{
				
						foreach (var member in members)
							container.Add(new TypeMemberStatement(member));
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "type_member_stmt");
				recover(ex,tokenSet_55_);
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
			if ((LA(1)==COLON) && (tokenSet_81_.member(LA(2))))
			{
				single_line_block(b);
			}
			else if ((LA(1)==COLON) && (LA(2)==INDENT)) {
				{
					match(COLON);
					begin = LT(1);
					match(INDENT);
					if (0==inputState.guessing)
					{
						
									b.LexicalInfo = ToLexicalInfo(begin);
									statements = b.Statements;
								
					}
					macro_block(statements);
					end(b);
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
				reportError(ex, "macro_compound_stmt");
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
				
						m = new StatementModifier(ToLexicalInfo(t));
						m.Type = type;
						m.Condition = e;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "stmt_modifier");
				recover(ex,tokenSet_15_);
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
		
				stmt = null;
				IToken label = null;
			
		
		try {      // for error handling
			token = LT(1);
			match(GOTO);
			label=macro_name();
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
				reportError(ex, "goto_stmt");
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
		
				stmt = null;
				IToken label = null;
			
		
		try {      // for error handling
			token = LT(1);
			match(COLON);
			label=macro_name();
			if (0==inputState.guessing)
			{
				
						stmt = new LabelStatement(ToLexicalInfo(token), label.getText());
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "label_stmt");
				recover(ex,tokenSet_22_);
			}
			else
			{
				throw ex;
			}
		}
		return stmt;
	}
	
	protected Statement  nested_function() //throws RecognitionException, TokenStreamException
{
		Statement stmt;
		
		IToken  def = null;
		
			stmt = null;
			BlockExpression be = null;
			Block body = null;
			TypeReference rt = null;
			IToken id = null;
		
		
		try {      // for error handling
			def = LT(1);
			match(DEF);
			id=macro_name();
			if (0==inputState.guessing)
			{
				
						be = new BlockExpression(ToLexicalInfo(id));
						body = be.Body;
					
			}
			{
				switch ( LA(1) )
				{
				case LPAREN:
				{
					match(LPAREN);
					parameter_declaration_list(be.Parameters);
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
								be.ReturnType = rt;
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
			if (0==inputState.guessing)
			{
				
						string name = id.getText();
						stmt = new DeclarationStatement(
									ToLexicalInfo(def),
									new Declaration(
										ToLexicalInfo(id),
										name),
									be);
						be[BlockExpression.ClosureNameAnnotation] = name;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "nested_function");
				recover(ex,tokenSet_85_);
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
			{
				switch ( LA(1) )
				{
				case OR:
				{
					or = LT(1);
					match(OR);
					if (0==inputState.guessing)
					{
						fs.OrBlock = new Block(ToLexicalInfo(or));
					}
					compound_stmt(fs.OrBlock);
					break;
				}
				case EOF:
				case DEDENT:
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
				case EVENT:
				case FINAL:
				case FROM:
				case FOR:
				case FALSE:
				case GOTO:
				case INTERFACE:
				case INTERNAL:
				case IF:
				case NEW:
				case NULL:
				case OVERRIDE:
				case PARTIAL:
				case PUBLIC:
				case PROTECTED:
				case PRIVATE:
				case RAISE:
				case RETURN:
				case SELF:
				case SUPER:
				case STATIC:
				case STRUCT:
				case THEN:
				case TRY:
				case TRANSIENT:
				case TRUE:
				case TYPEOF:
				case UNLESS:
				case VIRTUAL:
				case WHILE:
				case YIELD:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case COLON:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
			{
				if ((LA(1)==THEN) && (LA(2)==COLON))
				{
					et = LT(1);
					match(THEN);
					if (0==inputState.guessing)
					{
						fs.ThenBlock = new Block(ToLexicalInfo(et));
					}
					compound_stmt(fs.ThenBlock);
				}
				else if ((tokenSet_85_.member(LA(1))) && (tokenSet_50_.member(LA(2)))) {
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
				reportError(ex, "for_stmt");
				recover(ex,tokenSet_85_);
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
			{
				switch ( LA(1) )
				{
				case OR:
				{
					or = LT(1);
					match(OR);
					if (0==inputState.guessing)
					{
						ws.OrBlock = new Block(ToLexicalInfo(or));
					}
					compound_stmt(ws.OrBlock);
					break;
				}
				case EOF:
				case DEDENT:
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
				case EVENT:
				case FINAL:
				case FROM:
				case FOR:
				case FALSE:
				case GOTO:
				case INTERFACE:
				case INTERNAL:
				case IF:
				case NEW:
				case NULL:
				case OVERRIDE:
				case PARTIAL:
				case PUBLIC:
				case PROTECTED:
				case PRIVATE:
				case RAISE:
				case RETURN:
				case SELF:
				case SUPER:
				case STATIC:
				case STRUCT:
				case THEN:
				case TRY:
				case TRANSIENT:
				case TRUE:
				case TYPEOF:
				case UNLESS:
				case VIRTUAL:
				case WHILE:
				case YIELD:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case COLON:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
			{
				if ((LA(1)==THEN) && (LA(2)==COLON))
				{
					et = LT(1);
					match(THEN);
					if (0==inputState.guessing)
					{
						ws.ThenBlock = new Block(ToLexicalInfo(et));
					}
					compound_stmt(ws.ThenBlock);
				}
				else if ((tokenSet_85_.member(LA(1))) && (tokenSet_50_.member(LA(2)))) {
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
				reportError(ex, "while_stmt");
				recover(ex,tokenSet_85_);
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
						goto _loop412_breakloop;
					}
					
				}
_loop412_breakloop:				;
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
				case DEDENT:
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
				case EVENT:
				case FINAL:
				case FROM:
				case FOR:
				case FALSE:
				case GOTO:
				case INTERFACE:
				case INTERNAL:
				case IF:
				case NEW:
				case NULL:
				case OVERRIDE:
				case PARTIAL:
				case PUBLIC:
				case PROTECTED:
				case PRIVATE:
				case RAISE:
				case RETURN:
				case SELF:
				case SUPER:
				case STATIC:
				case STRUCT:
				case THEN:
				case TRY:
				case TRANSIENT:
				case TRUE:
				case TYPEOF:
				case UNLESS:
				case VIRTUAL:
				case WHILE:
				case YIELD:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case COLON:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "if_stmt");
				recover(ex,tokenSet_85_);
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
				reportError(ex, "unless_stmt");
				recover(ex,tokenSet_85_);
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
						goto _loop368_breakloop;
					}
					
				}
_loop368_breakloop:				;
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
						eblock = new Block(ToLexicalInfo(ftoken));
					}
					compound_stmt(eblock);
					if (0==inputState.guessing)
					{
						s.FailureBlock = eblock;
					}
					break;
				}
				case EOF:
				case DEDENT:
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
				case ENSURE:
				case ENUM:
				case EVENT:
				case FINAL:
				case FROM:
				case FOR:
				case FALSE:
				case GOTO:
				case INTERFACE:
				case INTERNAL:
				case IF:
				case NEW:
				case NULL:
				case OVERRIDE:
				case PARTIAL:
				case PUBLIC:
				case PROTECTED:
				case PRIVATE:
				case RAISE:
				case RETURN:
				case SELF:
				case SUPER:
				case STATIC:
				case STRUCT:
				case THEN:
				case TRY:
				case TRANSIENT:
				case TRUE:
				case TYPEOF:
				case UNLESS:
				case VIRTUAL:
				case WHILE:
				case YIELD:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case COLON:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
				case DEDENT:
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
				case EVENT:
				case FINAL:
				case FROM:
				case FOR:
				case FALSE:
				case GOTO:
				case INTERFACE:
				case INTERNAL:
				case IF:
				case NEW:
				case NULL:
				case OVERRIDE:
				case PARTIAL:
				case PUBLIC:
				case PROTECTED:
				case PRIVATE:
				case RAISE:
				case RETURN:
				case SELF:
				case SUPER:
				case STATIC:
				case STRUCT:
				case THEN:
				case TRY:
				case TRANSIENT:
				case TRUE:
				case TYPEOF:
				case UNLESS:
				case VIRTUAL:
				case WHILE:
				case YIELD:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case COLON:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "try_stmt");
				recover(ex,tokenSet_85_);
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
								if (((tokenSet_73_.member(LA(1))) && (tokenSet_93_.member(LA(2))))&&(_compact))
								{
									rhs=array_or_expression();
								}
								else {
									bool synPredMatched491 = false;
									if (((LA(1)==DEF||LA(1)==DO||LA(1)==COLON)))
									{
										int _m491 = mark();
										synPredMatched491 = true;
										inputState.guessing++;
										try {
											{
												switch ( LA(1) )
												{
												case COLON:
												{
													match(COLON);
													break;
												}
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
											synPredMatched491 = false;
										}
										rewind(_m491);
										inputState.guessing--;
									}
									if ( synPredMatched491 )
									{
										rhs=callable_expression();
									}
									else if ((tokenSet_73_.member(LA(1))) && (tokenSet_94_.member(LA(2)))) {
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
												case EOL:
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
									else
									{
										throw new NoViableAltException(LT(1), getFilename());
									}
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
					reportError(ex, "assignment_or_method_invocation_with_block_stmt");
					recover(ex,tokenSet_85_);
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
				case FROM:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case COMMA:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
							case EOL:
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
										case EOL:
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
				case EOL:
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
							case EOL:
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
				
						s = new ReturnStatement(ToLexicalInfo(r));
						s.Modifier = modifier;
						s.Expression = e;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "return_stmt");
				recover(ex,tokenSet_85_);
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
		
		
				d = null;
				TypeReference tr = null;
				IToken id = null;
			
		
		try {      // for error handling
			id=macro_name();
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
				reportError(ex, "declaration");
				recover(ex,tokenSet_95_);
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
				case EOL:
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
				reportError(ex, "unpack_stmt");
				recover(ex,tokenSet_85_);
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
		
		
				s = null;
				TypeReference tr = null;
				Expression initializer = null;
				StatementModifier m = null;
				IToken id = null;
			
		
		try {      // for error handling
			id=macro_name();
			match(AS);
			tr=type_reference();
			{
				switch ( LA(1) )
				{
				case ASSIGN:
				{
					{
						match(ASSIGN);
						{
							if (((tokenSet_96_.member(LA(1))) && (tokenSet_97_.member(LA(2))))&&(_compact))
							{
								initializer=simple_initializer();
							}
							else if ((tokenSet_96_.member(LA(1))) && (tokenSet_98_.member(LA(2)))) {
								initializer=declaration_initializer();
							}
							else
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							
						}
					}
					break;
				}
				case EOL:
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
				{
					{
						if (!(!_compact))
						  throw new SemanticException("!_compact");
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
							case EOL:
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
				reportError(ex, "declaration_stmt");
				recover(ex,tokenSet_23_);
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
				case FROM:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case COMMA:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
				case EOL:
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
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
				reportError(ex, "yield_stmt");
				recover(ex,tokenSet_91_);
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
				reportError(ex, "break_stmt");
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
				s = new ContinueStatement(ToLexicalInfo(c));
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "continue_stmt");
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
				case FROM:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
				case EOL:
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
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
				reportError(ex, "raise_stmt");
				recover(ex,tokenSet_91_);
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
				reportError(ex, "expression_stmt");
				recover(ex,tokenSet_22_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
	}
	
	protected Statement  assignment_or_method_invocation() //throws RecognitionException, TokenStreamException
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
				op = LT(1);
				match(ASSIGN);
				if (0==inputState.guessing)
				{
					token = op; binaryOperator = OperatorParser.ParseAssignment(op.getText());
				}
				rhs=array_or_expression();
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
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "assignment_or_method_invocation");
				recover(ex,tokenSet_20_);
			}
			else
			{
				throw ex;
			}
		}
		return stmt;
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
				case FROM:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case COMMA:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
				case EOL:
				case IF:
				case UNLESS:
				case WHILE:
				case EOS:
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
			{
				if (((LA(1)==IF||LA(1)==UNLESS||LA(1)==WHILE))&&(!_compact))
				{
					modifier=stmt_modifier();
				}
				else if ((tokenSet_99_.member(LA(1)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
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
				reportError(ex, "return_expression_stmt");
				recover(ex,tokenSet_99_);
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
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
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
				reportError(ex, "unpack");
				recover(ex,tokenSet_91_);
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
						if ((LA(1)==OR) && (tokenSet_6_.member(LA(2))))
						{
							ot = LT(1);
							match(OR);
							r=boolean_term();
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
							goto _loop440_breakloop;
						}
						
					}
_loop440_breakloop:					;
				}    // ( ... )*
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "boolean_expression");
				recover(ex,tokenSet_15_);
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
			case FROM:
			case FALSE:
			case NOT:
			case NULL:
			case SELF:
			case SUPER:
			case THEN:
			case TRUE:
			case TYPEOF:
			case LET:
			case WHERE:
			case JOIN:
			case ON:
			case EQUALS:
			case INTO:
			case ORDERBY:
			case ASCENDING:
			case DESCENDING:
			case SELECT:
			case GROUP:
			case BY:
			case TRIPLE_QUOTED_STRING:
			case LPAREN:
			case DOUBLE_QUOTED_STRING:
			case SINGLE_QUOTED_STRING:
			case MULTIPLY:
			case LBRACK:
			case COMMA:
			case SPLICE_BEGIN:
			case ID:
			case DOT:
			case LBRACE:
			case QQ_BEGIN:
			case SUBTRACT:
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
				reportError(ex, "callable_or_expression");
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
						goto _loop336_breakloop;
					}
					
				}
_loop336_breakloop:				;
			}    // ( ... )*
			match(BITWISE_OR);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "closure_parameters_test");
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
				case FROM:
				case FALSE:
				case NOT:
				case NULL:
				case RAISE:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case YIELD:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case COMMA:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
								bool synPredMatched342 = false;
								if (((tokenSet_4_.member(LA(1))) && (LA(2)==AS||LA(2)==COMMA)))
								{
									int _m342 = mark();
									synPredMatched342 = true;
									inputState.guessing++;
									try {
										{
											declaration();
											match(COMMA);
										}
									}
									catch (RecognitionException)
									{
										synPredMatched342 = false;
									}
									rewind(_m342);
									inputState.guessing--;
								}
								if ( synPredMatched342 )
								{
									stmt=unpack();
								}
								else if (((tokenSet_4_.member(LA(1))) && (tokenSet_100_.member(LA(2))))&&(IsValidClosureMacroArgument(LA(2)))) {
									stmt=closure_macro_stmt();
								}
								else if ((tokenSet_73_.member(LA(1))) && (tokenSet_101_.member(LA(2)))) {
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
							case EOL:
							case EOS:
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
				reportError(ex, "internal_closure_stmt");
				recover(ex,tokenSet_99_);
			}
			else
			{
				throw ex;
			}
		}
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
				reportError(ex, "closure_expression_stmt");
				recover(ex,tokenSet_91_);
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
				
							e = cbe = new BlockExpression(ToLexicalInfo(anchorBegin));
							cbe.Annotate("inline");
							parameters = cbe.Parameters;
							body = cbe.Body;
						
			}
			{
				bool synPredMatched348 = false;
				if (((tokenSet_102_.member(LA(1))) && (tokenSet_103_.member(LA(2)))))
				{
					int _m348 = mark();
					synPredMatched348 = true;
					inputState.guessing++;
					try {
						{
							closure_parameters_test();
						}
					}
					catch (RecognitionException)
					{
						synPredMatched348 = false;
					}
					rewind(_m348);
					inputState.guessing--;
				}
				if ( synPredMatched348 )
				{
					{
						parameter_declaration_list(parameters);
						match(BITWISE_OR);
					}
				}
				else if ((tokenSet_104_.member(LA(1))) && (tokenSet_105_.member(LA(2)))) {
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
						if ((LA(1)==EOL||LA(1)==EOS))
						{
							eos();
							{
								switch ( LA(1) )
								{
								case ESEPARATOR:
								case CAST:
								case CHAR:
								case FROM:
								case FALSE:
								case NOT:
								case NULL:
								case RAISE:
								case RETURN:
								case SELF:
								case SUPER:
								case THEN:
								case TRUE:
								case TYPEOF:
								case YIELD:
								case LET:
								case WHERE:
								case JOIN:
								case ON:
								case EQUALS:
								case INTO:
								case ORDERBY:
								case ASCENDING:
								case DESCENDING:
								case SELECT:
								case GROUP:
								case BY:
								case TRIPLE_QUOTED_STRING:
								case LPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case MULTIPLY:
								case LBRACK:
								case COMMA:
								case SPLICE_BEGIN:
								case ID:
								case DOT:
								case LBRACE:
								case QQ_BEGIN:
								case SUBTRACT:
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
								case EOL:
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
							goto _loop353_breakloop;
						}
						
					}
_loop353_breakloop:					;
				}    // ( ... )*
			}
			anchorEnd = LT(1);
			match(RBRACE);
			if (0==inputState.guessing)
			{
				
						body.EndSourceLocation = SourceLocationFactory.ToEndSourceLocation(anchorEnd);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "closure_expression");
				recover(ex,tokenSet_43_);
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
		IToken  u = null;
		
				ExceptionHandler eh = null;		
				TypeReference tr = null;
				Expression e = null;
				IToken x = null;
			
		
		try {      // for error handling
			c = LT(1);
			match(EXCEPT);
			{
				switch ( LA(1) )
				{
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case ID:
				{
					x=macro_name();
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
					e=boolean_expression();
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
						
						eh.Declaration = new Declaration();
						eh.Declaration.Type = tr;
						
						if (x != null)
						{
							eh.Declaration.LexicalInfo = ToLexicalInfo(x);
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
								UnaryExpression not = new UnaryExpression(ToLexicalInfo(u));
								not.Operator = UnaryOperatorType.LogicalNot;
								not.Operand = e;
								e = not;
							}
							eh.FilterCondition = e;
							eh.Flags |= ExceptionHandlerFlags.Filter;
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
				reportError(ex, "exception_handler");
				recover(ex,tokenSet_106_);
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
				if ((tokenSet_107_.member(LA(1))) && (tokenSet_108_.member(LA(2))))
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
						
									BinaryExpression be = new BinaryExpression(ToLexicalInfo(token));
									be.Operator = binaryOperator;
									be.Left = e;
									be.Right = r;
									e = be;
								
					}
				}
				else if ((tokenSet_15_.member(LA(1))) && (tokenSet_16_.member(LA(2)))) {
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
				reportError(ex, "assignment_expression");
				recover(ex,tokenSet_15_);
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
						goto _loop420_breakloop;
					}
					
				}
_loop420_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "declaration_list");
				recover(ex,tokenSet_109_);
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
				if ((LA(1)==IF||LA(1)==UNLESS||LA(1)==WHILE) && (tokenSet_6_.member(LA(2))))
				{
					filter=stmt_modifier();
					if (0==inputState.guessing)
					{
						ge.Filter = filter;
					}
				}
				else if ((tokenSet_15_.member(LA(1))) && (tokenSet_16_.member(LA(2)))) {
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
				reportError(ex, "generator_expression_body");
				recover(ex,tokenSet_15_);
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
					if ((LA(1)==AND) && (tokenSet_6_.member(LA(2))))
					{
						at = LT(1);
						match(AND);
						r=not_expression();
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
						goto _loop443_breakloop;
					}
					
				}
_loop443_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "boolean_term");
				recover(ex,tokenSet_15_);
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
				case FROM:
				case FALSE:
				case NULL:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
				reportError(ex, "not_expression");
				recover(ex,tokenSet_15_);
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
		
			e = null;
		
		
		try {      // for error handling
			begin = LT(1);
			match(QQ_BEGIN);
			if (0==inputState.guessing)
			{
				e = new QuasiquoteExpression(ToLexicalInfo(begin));
			}
			{
				switch ( LA(1) )
				{
				case INDENT:
				{
					{
						match(INDENT);
						ast_literal_block(e);
						match(DEDENT);
						{
							switch ( LA(1) )
							{
							case EOL:
							case EOS:
							{
								eos();
								break;
							}
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
					break;
				}
				case ESEPARATOR:
				case CAST:
				case CHAR:
				case FROM:
				case FALSE:
				case IMPORT:
				case NOT:
				case NULL:
				case RAISE:
				case RETURN:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case YIELD:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case COMMA:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
					ast_literal_closure(e);
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			end = LT(1);
			match(QQ_END);
			if (0==inputState.guessing)
			{
				SetEndSourceLocation(e, end);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "ast_literal_expression");
				recover(ex,tokenSet_43_);
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
			bool synPredMatched452 = false;
			if (((tokenSet_110_.member(LA(1))) && (tokenSet_111_.member(LA(2)))))
			{
				int _m452 = mark();
				synPredMatched452 = true;
				inputState.guessing++;
				try {
					{
						ast_literal_module_prediction();
					}
				}
				catch (RecognitionException)
				{
					synPredMatched452 = false;
				}
				rewind(_m452);
				inputState.guessing--;
			}
			if ( synPredMatched452 )
			{
				{
					ast_literal_module(e);
				}
			}
			else {
				bool synPredMatched462 = false;
				if (((tokenSet_37_.member(LA(1))) && (tokenSet_38_.member(LA(2)))))
				{
					int _m462 = mark();
					synPredMatched462 = true;
					inputState.guessing++;
					try {
						{
							attributes();
							{
								if ((tokenSet_33_.member(LA(1))) && (true))
								{
									type_member_modifier();
								}
								else if ((tokenSet_112_.member(LA(1))) && (true)) {
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
											case SPLICE_BEGIN:
											case ID:
											{
												{
													{
														switch ( LA(1) )
														{
														case ID:
														{
															match(ID);
															break;
														}
														case SPLICE_BEGIN:
														{
															splice_expression();
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
						synPredMatched462 = false;
					}
					rewind(_m462);
					inputState.guessing--;
				}
				if ( synPredMatched462 )
				{
					{
						{ // ( ... )+
							int _cnt465=0;
							for (;;)
							{
								if ((tokenSet_37_.member(LA(1))))
								{
									type_definition_member(collection);
								}
								else
								{
									if (_cnt465 >= 1) { goto _loop465_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
								}
								
								_cnt465++;
							}
_loop465_breakloop:							;
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
				else if ((tokenSet_18_.member(LA(1))) && (tokenSet_92_.member(LA(2)))) {
					{ // ( ... )+
						int _cnt467=0;
						for (;;)
						{
							if ((tokenSet_18_.member(LA(1))))
							{
								stmt(statements);
							}
							else
							{
								if (_cnt467 >= 1) { goto _loop467_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
							}
							
							_cnt467++;
						}
_loop467_breakloop:						;
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
					reportError(ex, "ast_literal_block");
					recover(ex,tokenSet_70_);
				}
				else
				{
					throw ex;
				}
			}
		}
		
	public void ast_literal_closure(
		QuasiquoteExpression e
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  c = null;
		
			Block block = null;
			Node node = null;
		
		
		try {      // for error handling
			bool synPredMatched474 = false;
			if (((tokenSet_6_.member(LA(1))) && (tokenSet_113_.member(LA(2)))))
			{
				int _m474 = mark();
				synPredMatched474 = true;
				inputState.guessing++;
				try {
					{
						expression();
						{
							switch ( LA(1) )
							{
							case COLON:
							{
								match(COLON);
								break;
							}
							case QQ_END:
							{
								match(QQ_END);
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
					synPredMatched474 = false;
				}
				rewind(_m474);
				inputState.guessing--;
			}
			if ( synPredMatched474 )
			{
				{
					node=expression();
					if (0==inputState.guessing)
					{
						e.Node = node;
					}
					{
						switch ( LA(1) )
						{
						case COLON:
						{
							c = LT(1);
							match(COLON);
							node=expression();
							if (0==inputState.guessing)
							{
								
												e.Node = new ExpressionPair(ToLexicalInfo(c), (Expression)e.Node, (Expression)node);
											
							}
							break;
						}
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
			}
			else if ((LA(1)==IMPORT)) {
				{
					node=import_directive_();
					if (0==inputState.guessing)
					{
						
									e.Node = node;
								
					}
				}
			}
			else if ((tokenSet_104_.member(LA(1))) && (tokenSet_101_.member(LA(2)))) {
				{
					if (0==inputState.guessing)
					{
						block = new Block();
					}
					internal_closure_stmt(block);
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==EOL||LA(1)==EOS))
							{
								eos();
								{
									switch ( LA(1) )
									{
									case ESEPARATOR:
									case CAST:
									case CHAR:
									case FROM:
									case FALSE:
									case NOT:
									case NULL:
									case RAISE:
									case RETURN:
									case SELF:
									case SUPER:
									case THEN:
									case TRUE:
									case TYPEOF:
									case YIELD:
									case LET:
									case WHERE:
									case JOIN:
									case ON:
									case EQUALS:
									case INTO:
									case ORDERBY:
									case ASCENDING:
									case DESCENDING:
									case SELECT:
									case GROUP:
									case BY:
									case TRIPLE_QUOTED_STRING:
									case LPAREN:
									case DOUBLE_QUOTED_STRING:
									case SINGLE_QUOTED_STRING:
									case MULTIPLY:
									case LBRACK:
									case COMMA:
									case SPLICE_BEGIN:
									case ID:
									case DOT:
									case LBRACE:
									case QQ_BEGIN:
									case SUBTRACT:
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
										internal_closure_stmt(block);
										break;
									}
									case EOL:
									case EOS:
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
							else
							{
								goto _loop481_breakloop;
							}
							
						}
_loop481_breakloop:						;
					}    // ( ... )*
					if (0==inputState.guessing)
					{
						
									e.Node = block;
									if (block.Statements.Count == 1)
									{
										e.Node = block.Statements[0];
									}
								
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
				reportError(ex, "ast_literal_closure");
				recover(ex,tokenSet_114_);
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
				reportError(ex, "ast_literal_module");
				recover(ex,tokenSet_70_);
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
				case EOL:
				case EOS:
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
				reportError(ex, "ast_literal_module_prediction");
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
					if ((tokenSet_115_.member(LA(1))) && (tokenSet_116_.member(LA(2))))
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
											else if ((LA(1)==IS) && (tokenSet_108_.member(LA(2)))) {
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
							
									BinaryExpression be = new BinaryExpression(ToLexicalInfo(token));
									be.Operator = op;
									be.Left = e;
									be.Right = r;
									e = be;
								
						}
					}
					else
					{
						goto _loop524_breakloop;
					}
					
				}
_loop524_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "conditional_expression");
				recover(ex,tokenSet_15_);
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
					if ((tokenSet_117_.member(LA(1))) && (tokenSet_108_.member(LA(2))))
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
						goto _loop528_breakloop;
					}
					
				}
_loop528_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "sum");
				recover(ex,tokenSet_15_);
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
					if ((tokenSet_118_.member(LA(1))) && (tokenSet_108_.member(LA(2))))
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
						goto _loop532_breakloop;
					}
					
				}
_loop532_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "term");
				recover(ex,tokenSet_15_);
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
					if ((LA(1)==SHIFT_LEFT||LA(1)==SHIFT_RIGHT) && (tokenSet_108_.member(LA(2))))
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
						goto _loop536_breakloop;
					}
					
				}
_loop536_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "factor");
				recover(ex,tokenSet_15_);
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
				if ((LA(1)==AS) && (tokenSet_45_.member(LA(2))))
				{
					t = LT(1);
					match(AS);
					tr=type_reference();
					if (0==inputState.guessing)
					{
						e = new TryCastExpression(ToLexicalInfo(t)) { Target = e, Type = tr };
					}
				}
				else if ((LA(1)==CAST) && (tokenSet_45_.member(LA(2)))) {
					c = LT(1);
					match(CAST);
					tr=type_reference();
					if (0==inputState.guessing)
					{
						e = new CastExpression(ToLexicalInfo(c)) { Target = e, Type = tr };
					}
				}
				else if ((tokenSet_15_.member(LA(1))) && (tokenSet_16_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==EXPONENTIATION) && (tokenSet_108_.member(LA(2))))
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
						goto _loop540_breakloop;
					}
					
				}
_loop540_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "exponentiation");
				recover(ex,tokenSet_15_);
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
				bool synPredMatched544 = false;
				if (((LA(1)==SUBTRACT||LA(1)==LONG||LA(1)==INT) && (tokenSet_15_.member(LA(2)))))
				{
					int _m544 = mark();
					synPredMatched544 = true;
					inputState.guessing++;
					try {
						{
							match(SUBTRACT);
							match(LONG);
						}
					}
					catch (RecognitionException)
					{
						synPredMatched544 = false;
					}
					rewind(_m544);
					inputState.guessing--;
				}
				if ( synPredMatched544 )
				{
					{
						e=integer_literal();
					}
				}
				else if ((tokenSet_119_.member(LA(1))) && (tokenSet_108_.member(LA(2)))) {
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
				else if ((tokenSet_36_.member(LA(1))) && (tokenSet_120_.member(LA(2)))) {
					{
						e=slicing_expression();
						{
							if ((LA(1)==INCREMENT) && (tokenSet_15_.member(LA(2))))
							{
								postinc = LT(1);
								match(INCREMENT);
								if (0==inputState.guessing)
								{
									op = postinc; uOperator = UnaryOperatorType.PostIncrement;
								}
							}
							else if ((LA(1)==DECREMENT) && (tokenSet_15_.member(LA(2)))) {
								postdec = LT(1);
								match(DECREMENT);
								if (0==inputState.guessing)
								{
									op = postdec; uOperator = UnaryOperatorType.PostDecrement;
								}
							}
							else if ((tokenSet_15_.member(LA(1))) && (tokenSet_16_.member(LA(2)))) {
							}
							else
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							
						}
					}
				}
				else if ((LA(1)==FROM)) {
					{
						e=query_expression();
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
				reportError(ex, "unary_expression");
				recover(ex,tokenSet_15_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected QueryExpression  query_expression() //throws RecognitionException, TokenStreamException
{
		QueryExpression e;
		
		
			e = null;
			FromClauseExpression f = null;
			EnterQuery();
		
		
		try {      // for error handling
			f=from_clause();
			if (0==inputState.guessing)
			{
				
						e = new QueryExpression(f.LexicalInfo);
						e.Clauses.Add(f);
					
			}
			query_body(e);
			if (0==inputState.guessing)
			{
				
						LeaveQuery();
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "query_expression");
				recover(ex,tokenSet_15_);
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
					if ((LA(1)==SUBTRACT||LA(1)==LONG||LA(1)==INT) && (tokenSet_43_.member(LA(2))))
					{
						e=integer_literal();
					}
					else {
						bool synPredMatched636 = false;
						if (((LA(1)==LBRACE) && (tokenSet_77_.member(LA(2)))))
						{
							int _m636 = mark();
							synPredMatched636 = true;
							inputState.guessing++;
							try {
								{
									hash_literal_test();
								}
							}
							catch (RecognitionException)
							{
								synPredMatched636 = false;
							}
							rewind(_m636);
							inputState.guessing--;
						}
						if ( synPredMatched636 )
						{
							e=hash_literal();
						}
						else if ((LA(1)==LBRACE) && (tokenSet_121_.member(LA(2)))) {
							e=closure_expression();
						}
						else if ((LA(1)==SUBTRACT||LA(1)==DOUBLE||LA(1)==FLOAT) && (tokenSet_43_.member(LA(2)))) {
							e=double_literal();
						}
						else if ((LA(1)==SUBTRACT||LA(1)==TIMESPAN) && (tokenSet_43_.member(LA(2)))) {
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
					reportError(ex, "literal");
					recover(ex,tokenSet_43_);
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
						
									e = new CharLiteralExpression(ToLexicalInfo(t), t.getText());
								
					}
					break;
				}
				case INT:
				{
					i = LT(1);
					match(INT);
					if (0==inputState.guessing)
					{
						
									e = new CharLiteralExpression(ToLexicalInfo(i), (char) PrimitiveParser.ParseInt(i));
								
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
				reportError(ex, "char_literal");
				recover(ex,tokenSet_43_);
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
		
		IToken  ch = null;
		
			e = null;
			IToken t = null;
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case THEN:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case ID:
				{
					t=macro_name();
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
				
						e = new ReferenceExpression(ToLexicalInfo(t), t.getText());
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "reference_expression");
				recover(ex,tokenSet_43_);
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
			bool synPredMatched565 = false;
			if (((LA(1)==LPAREN) && (LA(2)==OF)))
			{
				int _m565 = mark();
				synPredMatched565 = true;
				inputState.guessing++;
				try {
					{
						match(LPAREN);
						match(OF);
					}
				}
				catch (RecognitionException)
				{
					synPredMatched565 = false;
				}
				rewind(_m565);
				inputState.guessing--;
			}
			if ( synPredMatched565 )
			{
				e=typed_array();
			}
			else if ((LA(1)==LPAREN) && (tokenSet_73_.member(LA(2)))) {
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
								
												ConditionalExpression ce = new ConditionalExpression(ToLexicalInfo(lparen));
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
				reportError(ex, "paren_expression");
				recover(ex,tokenSet_43_);
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
				
						e = new CastExpression(ToLexicalInfo(t), target, tr);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "cast_expression");
				recover(ex,tokenSet_43_);
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
				reportError(ex, "typeof_expression");
				recover(ex,tokenSet_43_);
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
				reportError(ex, "omitted_member_expression");
				recover(ex,tokenSet_43_);
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
				
						e = tle = new ArrayLiteralExpression(ToLexicalInfo(t));
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
				case FROM:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
								if ((LA(1)==COMMA) && (tokenSet_6_.member(LA(2))))
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
									goto _loop595_breakloop;
								}
								
							}
_loop595_breakloop:							;
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
				reportError(ex, "typed_array");
				recover(ex,tokenSet_43_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
	}
	
	protected FromClauseExpression  from_clause() //throws RecognitionException, TokenStreamException
{
		FromClauseExpression f;
		
		IToken  fr = null;
		
			f = null;
			Declaration ident = null;
			Expression enumerable = null;
		
		
		try {      // for error handling
			fr = LT(1);
			match(FROM);
			ident=declaration();
			match(IN);
			enumerable=expression();
			if (0==inputState.guessing)
			{
				
						f = new FromClauseExpression(ToLexicalInfo(fr));
						f.Identifier = ident;
						f.Container = enumerable;
						f.DeclaredType = (ident.Type != null);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "from_clause");
				recover(ex,tokenSet_122_);
			}
			else
			{
				throw ex;
			}
		}
		return f;
	}
	
	protected void query_body(
		QueryExpression q
	) //throws RecognitionException, TokenStreamException
{
		
		
			var clauses = q.Clauses;
			QueryEndingExpression e = null;
			QueryContinuationExpression c = null;
		
		
		try {      // for error handling
			query_body_clause(clauses);
			{
				switch ( LA(1) )
				{
				case SELECT:
				{
					e=select_clause();
					break;
				}
				case GROUP:
				{
					e=group_clause();
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{
				if ((LA(1)==INTO) && (tokenSet_4_.member(LA(2))))
				{
					c=query_continuation();
				}
				else if ((tokenSet_15_.member(LA(1))) && (tokenSet_16_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			if (0==inputState.guessing)
			{
				
						q.Ending = e;
						q.Cont = c;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "query_body");
				recover(ex,tokenSet_15_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void query_body_clause(
		ExpressionCollection c
	) //throws RecognitionException, TokenStreamException
{
		
		
			QueryClauseExpression next = null;
		
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_123_.member(LA(1))))
					{
						{
							switch ( LA(1) )
							{
							case FROM:
							{
								next=from_clause();
								break;
							}
							case LET:
							{
								next=let_clause();
								break;
							}
							case WHERE:
							{
								next=where_clause();
								break;
							}
							case JOIN:
							{
								next=join_clause();
								break;
							}
							case ORDERBY:
							{
								next=orderby_clause();
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
							
										c.Add(next);
									
						}
					}
					else
					{
						goto _loop576_breakloop;
					}
					
				}
_loop576_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "query_body_clause");
				recover(ex,tokenSet_124_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected SelectClauseExpression  select_clause() //throws RecognitionException, TokenStreamException
{
		SelectClauseExpression s;
		
		IToken  sel = null;
		
			s = null;
			Expression baseExpr = null;
		
		
		try {      // for error handling
			sel = LT(1);
			match(SELECT);
			baseExpr=expression();
			if (0==inputState.guessing)
			{
				
						s = new SelectClauseExpression(ToLexicalInfo(sel));
						s.BaseExpr = baseExpr;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "select_clause");
				recover(ex,tokenSet_15_);
			}
			else
			{
				throw ex;
			}
		}
		return s;
	}
	
	protected GroupClauseExpression  group_clause() //throws RecognitionException, TokenStreamException
{
		GroupClauseExpression g;
		
		IToken  gr = null;
		
			g = null;
			Expression baseExpr = null;
			Expression criterion = null;
		
		
		try {      // for error handling
			gr = LT(1);
			match(GROUP);
			baseExpr=expression();
			match(BY);
			criterion=expression();
			if (0==inputState.guessing)
			{
				
						g = new GroupClauseExpression(ToLexicalInfo(gr));
						g.BaseExpr = baseExpr;
						g.Criterion = criterion;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "group_clause");
				recover(ex,tokenSet_15_);
			}
			else
			{
				throw ex;
			}
		}
		return g;
	}
	
	protected QueryContinuationExpression  query_continuation() //throws RecognitionException, TokenStreamException
{
		QueryContinuationExpression q;
		
		IToken  qb = null;
		
			q = null;
			QueryExpression body = null;
			IToken id = null;
		
		
		try {      // for error handling
			qb = LT(1);
			match(INTO);
			id=macro_name();
			if (0==inputState.guessing)
			{
				
						q = new QueryContinuationExpression(ToLexicalInfo(qb));
						q.Ident = id.getText();
						q.Body = new QueryExpression();
					
			}
			query_body(q.Body);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "query_continuation");
				recover(ex,tokenSet_15_);
			}
			else
			{
				throw ex;
			}
		}
		return q;
	}
	
	protected LetClauseExpression  let_clause() //throws RecognitionException, TokenStreamException
{
		LetClauseExpression l;
		
		IToken  le = null;
		
			l = null;
			Expression identifier = null;
			Expression expr = null;
			IToken ident = null;
		
		
		try {      // for error handling
			le = LT(1);
			match(LET);
			ident=macro_name();
			match(130);
			expr=expression();
			if (0==inputState.guessing)
			{
				
						l = new LetClauseExpression(ToLexicalInfo(le));
						l.Identifier = identifier;
						l.Value = expr;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "let_clause");
				recover(ex,tokenSet_122_);
			}
			else
			{
				throw ex;
			}
		}
		return l;
	}
	
	protected WhereClauseExpression  where_clause() //throws RecognitionException, TokenStreamException
{
		WhereClauseExpression w;
		
		IToken  wh = null;
		
			w = null;
			Expression where = null;
		
		
		try {      // for error handling
			wh = LT(1);
			match(WHERE);
			where=boolean_expression();
			if (0==inputState.guessing)
			{
				
						w = new WhereClauseExpression(ToLexicalInfo(wh));
						w.Cond = where;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "where_clause");
				recover(ex,tokenSet_122_);
			}
			else
			{
				throw ex;
			}
		}
		return w;
	}
	
	protected JoinClauseExpression  join_clause() //throws RecognitionException, TokenStreamException
{
		JoinClauseExpression j;
		
		IToken  jo = null;
		
			j = null;
			Declaration ident = null;
			Expression enumerable = null;
			Expression onExprL = null;
			Expression onExprR = null;
			ReferenceExpression intoExpr = null;
		
		
		try {      // for error handling
			jo = LT(1);
			match(JOIN);
			ident=declaration();
			match(IN);
			enumerable=expression();
			match(ON);
			onExprL=expression();
			match(EQUALS);
			onExprR=expression();
			{
				switch ( LA(1) )
				{
				case INTO:
				{
					match(INTO);
					intoExpr=identifier_expression();
					break;
				}
				case FROM:
				case LET:
				case WHERE:
				case JOIN:
				case ORDERBY:
				case SELECT:
				case GROUP:
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
				
						j = new JoinClauseExpression(ToLexicalInfo(jo));
						j.Identifier = ident;
						j.Container = enumerable;
						j.DeclaredType = (ident.Type != null);
						j.Left = onExprL;
						j.Right = onExprR;
						j.Into = intoExpr;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "join_clause");
				recover(ex,tokenSet_122_);
			}
			else
			{
				throw ex;
			}
		}
		return j;
	}
	
	protected OrderByClauseExpression  orderby_clause() //throws RecognitionException, TokenStreamException
{
		OrderByClauseExpression o;
		
		IToken  ob = null;
		
			o = null;
			OrderingExpressionCollection ord = null;
		
		
		try {      // for error handling
			ob = LT(1);
			match(ORDERBY);
			ord=orderings();
			if (0==inputState.guessing)
			{
				
						o = new OrderByClauseExpression(ToLexicalInfo(ob));
						o.Orderings = ord;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "orderby_clause");
				recover(ex,tokenSet_122_);
			}
			else
			{
				throw ex;
			}
		}
		return o;
	}
	
	protected OrderingExpressionCollection  orderings() //throws RecognitionException, TokenStreamException
{
		OrderingExpressionCollection l;
		
		
			l = null;
			OrderingExpression oe = null;
		
		
		try {      // for error handling
			oe=ordering();
			if (0==inputState.guessing)
			{
				
						l = new OrderingExpressionCollection();
						l.Add(oe);
					
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						oe=ordering();
						if (0==inputState.guessing)
						{
							l.Add(oe);
						}
					}
					else
					{
						goto _loop584_breakloop;
					}
					
				}
_loop584_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "orderings");
				recover(ex,tokenSet_122_);
			}
			else
			{
				throw ex;
			}
		}
		return l;
	}
	
	protected OrderingExpression  ordering() //throws RecognitionException, TokenStreamException
{
		OrderingExpression o;
		
		
			o = null;
			Expression baseExpr = null;
			bool desc = false;
		
		
		try {      // for error handling
			baseExpr=expression();
			{
				switch ( LA(1) )
				{
				case ASCENDING:
				{
					match(ASCENDING);
					break;
				}
				case DESCENDING:
				{
					{
						match(DESCENDING);
					}
					if (0==inputState.guessing)
					{
						desc = true;
					}
					break;
				}
				case FROM:
				case LET:
				case WHERE:
				case JOIN:
				case ORDERBY:
				case SELECT:
				case GROUP:
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
				
						o = new OrderingExpression(baseExpr.LexicalInfo);
						o.BaseExpr = baseExpr;
						o.Descending = desc;
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "ordering");
				recover(ex,tokenSet_125_);
			}
			else
			{
				throw ex;
			}
		}
		return o;
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
							case FROM:
							case FALSE:
							case NOT:
							case NULL:
							case SELF:
							case SUPER:
							case THEN:
							case TRUE:
							case TYPEOF:
							case LET:
							case WHERE:
							case JOIN:
							case ON:
							case EQUALS:
							case INTO:
							case ORDERBY:
							case ASCENDING:
							case DESCENDING:
							case SELECT:
							case GROUP:
							case BY:
							case TRIPLE_QUOTED_STRING:
							case LPAREN:
							case DOUBLE_QUOTED_STRING:
							case SINGLE_QUOTED_STRING:
							case MULTIPLY:
							case LBRACK:
							case SPLICE_BEGIN:
							case ID:
							case DOT:
							case LBRACE:
							case QQ_BEGIN:
							case SUBTRACT:
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
				case FROM:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
								case FROM:
								case FALSE:
								case NOT:
								case NULL:
								case SELF:
								case SUPER:
								case THEN:
								case TRUE:
								case TYPEOF:
								case LET:
								case WHERE:
								case JOIN:
								case ON:
								case EQUALS:
								case INTO:
								case ORDERBY:
								case ASCENDING:
								case DESCENDING:
								case SELECT:
								case GROUP:
								case BY:
								case TRIPLE_QUOTED_STRING:
								case LPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case MULTIPLY:
								case LBRACK:
								case SPLICE_BEGIN:
								case ID:
								case DOT:
								case LBRACE:
								case QQ_BEGIN:
								case SUBTRACT:
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
				reportError(ex, "slice");
				recover(ex,tokenSet_51_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected Expression  safe_atom() //throws RecognitionException, TokenStreamException
{
		Expression e;
		
		
				e = null;
				UnaryExpression ue = null;
			
		
		try {      // for error handling
			e=atom();
			{
				switch ( LA(1) )
				{
				case NULLABLE_SUFFIX:
				{
					match(NULLABLE_SUFFIX);
					if (0==inputState.guessing)
					{
						
									ue = new UnaryExpression(e.LexicalInfo);
									ue.Operator = UnaryOperatorType.SafeAccess;
									ue.Operand = e;
									e = ue;
								
					}
					break;
				}
				case EOF:
				case DEDENT:
				case ESEPARATOR:
				case EOL:
				case ASSEMBLY_ATTRIBUTE_BEGIN:
				case MODULE_ATTRIBUTE_BEGIN:
				case ABSTRACT:
				case AND:
				case AS:
				case BREAK:
				case CONTINUE:
				case CALLABLE:
				case CAST:
				case CHAR:
				case CLASS:
				case DEF:
				case DO:
				case ELSE:
				case ENUM:
				case EVENT:
				case FINAL:
				case FROM:
				case FOR:
				case FALSE:
				case GOTO:
				case INTERFACE:
				case INTERNAL:
				case IS:
				case ISA:
				case IF:
				case IN:
				case NEW:
				case NOT:
				case NULL:
				case OF:
				case OR:
				case OVERRIDE:
				case PARTIAL:
				case PUBLIC:
				case PROTECTED:
				case PRIVATE:
				case RAISE:
				case RETURN:
				case SELF:
				case SUPER:
				case STATIC:
				case STRUCT:
				case THEN:
				case TRY:
				case TRANSIENT:
				case TRUE:
				case TYPEOF:
				case UNLESS:
				case VIRTUAL:
				case WHILE:
				case YIELD:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case EOS:
				case LPAREN:
				case RPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case RBRACK:
				case ASSIGN:
				case COMMA:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case COLON:
				case EXPONENTIATION:
				case BITWISE_OR:
				case LBRACE:
				case RBRACE:
				case QQ_BEGIN:
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
				case SUBTRACT:
				case EXCLUSIVE_OR:
				case DIVISION:
				case MODULUS:
				case BITWISE_AND:
				case SHIFT_LEFT:
				case SHIFT_RIGHT:
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
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "safe_atom");
				recover(ex,tokenSet_78_);
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
		
				
				Expression value = null;
				ExpressionPair pair = null;
			
		
		try {      // for error handling
			bool synPredMatched683 = false;
			if (((tokenSet_6_.member(LA(1))) && (tokenSet_126_.member(LA(2)))))
			{
				int _m683 = mark();
				synPredMatched683 = true;
				inputState.guessing++;
				try {
					{
						expression_pair();
					}
				}
				catch (RecognitionException)
				{
					synPredMatched683 = false;
				}
				rewind(_m683);
				inputState.guessing--;
			}
			if ( synPredMatched683 )
			{
				{
					pair=expression_pair();
					if (0==inputState.guessing)
					{
						if (pair != null) node.NamedArguments.Add(pair);
					}
				}
			}
			else if ((tokenSet_6_.member(LA(1))) && (tokenSet_127_.member(LA(2)))) {
				{
					value=expression();
					if (0==inputState.guessing)
					{
						if (value != null) node.Arguments.Add(value);
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
				reportError(ex, "argument");
				recover(ex,tokenSet_88_);
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
				case FROM:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
				reportError(ex, "hash_literal_test");
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
				dle = new HashLiteralExpression(ToLexicalInfo(lbrace));
			}
			{
				switch ( LA(1) )
				{
				case ESEPARATOR:
				case CAST:
				case CHAR:
				case FROM:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
							if ((LA(1)==COMMA) && (tokenSet_6_.member(LA(2))))
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
								goto _loop665_breakloop;
							}
							
						}
_loop665_breakloop:						;
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
				reportError(ex, "hash_literal");
				recover(ex,tokenSet_43_);
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
				reportError(ex, "list_initializer");
				recover(ex,tokenSet_78_);
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
				case FROM:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case LET:
				case WHERE:
				case JOIN:
				case ON:
				case EQUALS:
				case INTO:
				case ORDERBY:
				case ASCENDING:
				case DESCENDING:
				case SELECT:
				case GROUP:
				case BY:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case MULTIPLY:
				case LBRACK:
				case SPLICE_BEGIN:
				case ID:
				case DOT:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
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
								if ((LA(1)==COMMA) && (tokenSet_6_.member(LA(2))))
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
									goto _loop657_breakloop;
								}
								
							}
_loop657_breakloop:							;
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
				reportError(ex, "list_items");
				recover(ex,tokenSet_128_);
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
					
							e = new StringLiteralExpression(ToLexicalInfo(dqs), dqs.getText());
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
					
							e = new StringLiteralExpression(ToLexicalInfo(sqs), sqs.getText());
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
					
							e = new StringLiteralExpression(ToLexicalInfo(tqs), tqs.getText());
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
				reportError(ex, "string_literal");
				recover(ex,tokenSet_43_);
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
				reportError(ex, "list_literal");
				recover(ex,tokenSet_43_);
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
				re = new RELiteralExpression(ToLexicalInfo(value), value.getText());
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "re_literal");
				recover(ex,tokenSet_43_);
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
				reportError(ex, "bool_literal");
				recover(ex,tokenSet_43_);
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
				reportError(ex, "null_literal");
				recover(ex,tokenSet_43_);
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
				reportError(ex, "self_literal");
				recover(ex,tokenSet_43_);
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
				reportError(ex, "super_literal");
				recover(ex,tokenSet_43_);
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
							rle = new DoubleLiteralExpression(ToLexicalInfo(value), PrimitiveParser.ParseDouble(value, val));
						
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
							rle = new DoubleLiteralExpression(ToLexicalInfo(single), PrimitiveParser.ParseDouble(single, val, true), true);
						
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
				reportError(ex, "double_literal");
				recover(ex,tokenSet_43_);
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
						tsle = new TimeSpanLiteralExpression(ToLexicalInfo(value), PrimitiveParser.ParseTimeSpan(value, val)); 
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex, "timespan_literal");
				recover(ex,tokenSet_43_);
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
		
		IToken  firstseparator = null;
		IToken  startsep = null;
		IToken  format_sep = null;
		IToken  endsep = null;
		IToken  lastseparator = null;
		
			e = null;
			Expression param = null;
			LexicalInfo info = null;
			IToken formatString = null;
		
		
		try {      // for error handling
			{
				if ((LA(1)==ESEPARATOR) && (LA(2)==ESEPARATOR))
				{
					firstseparator = LT(1);
					match(ESEPARATOR);
				}
				else if ((LA(1)==ESEPARATOR) && (tokenSet_6_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			{ // ( ... )+
				int _cnt650=0;
				for (;;)
				{
					if ((LA(1)==ESEPARATOR) && (tokenSet_6_.member(LA(2))))
					{
						startsep = LT(1);
						match(ESEPARATOR);
						if (0==inputState.guessing)
						{
							
										if (info == null)
										{
											info = ToLexicalInfo(startsep);
											e = new ExpressionInterpolationExpression(info);
										}
									
						}
						param=expression();
						{
							switch ( LA(1) )
							{
							case THEN:
							case LET:
							case WHERE:
							case JOIN:
							case ON:
							case EQUALS:
							case INTO:
							case ORDERBY:
							case ASCENDING:
							case DESCENDING:
							case SELECT:
							case GROUP:
							case BY:
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
									case THEN:
									case LET:
									case WHERE:
									case JOIN:
									case ON:
									case EQUALS:
									case INTO:
									case ORDERBY:
									case ASCENDING:
									case DESCENDING:
									case SELECT:
									case GROUP:
									case BY:
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
								formatString=macro_name();
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
						endsep = LT(1);
						match(ESEPARATOR);
					}
					else
					{
						if (_cnt650 >= 1) { goto _loop650_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt650++;
				}
_loop650_breakloop:				;
			}    // ( ... )+
			{
				if ((LA(1)==ESEPARATOR) && (tokenSet_43_.member(LA(2))))
				{
					lastseparator = LT(1);
					match(ESEPARATOR);
				}
				else if ((tokenSet_43_.member(LA(1))) && (tokenSet_16_.member(LA(2)))) {
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
				reportError(ex, "expression_interpolation");
				recover(ex,tokenSet_43_);
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
				reportError(ex, "expression_pair");
				recover(ex,tokenSet_129_);
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
		@"""INDENT""",
		@"""DEDENT""",
		@"""ELIST""",
		@"""DLIST""",
		@"""ESEPARATOR""",
		@"""EOL""",
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
		@"""pass""",
		@"""partial""",
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
		@"""while""",
		@"""yield""",
		@"""let""",
		@"""where""",
		@"""join""",
		@"""on""",
		@"""equals""",
		@"""into""",
		@"""orderby""",
		@"""ascending""",
		@"""descending""",
		@"""select""",
		@"""group""",
		@"""by""",
		@"""TRIPLE_QUOTED_STRING""",
		@"""EOS""",
		@"""LPAREN""",
		@"""RPAREN""",
		@"""DOUBLE_QUOTED_STRING""",
		@"""SINGLE_QUOTED_STRING""",
		@"""MULTIPLY""",
		@"""LBRACK""",
		@"""RBRACK""",
		@"""ASSIGN""",
		@"""COMMA""",
		@"""SPLICE_BEGIN""",
		@"""ID""",
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
		@"""SUBTRACT""",
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
		@"""=""",
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
		@"""NEWLINE""",
		@"""DQS_ESC""",
		@"""SQS_ESC""",
		@"""SESC""",
		@"""RE_CHAR""",
		@"""X_RE_CHAR""",
		@"""RE_OPTIONS""",
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
		long[] data = { -2895018659466469598L, -2287817355809521665L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { -11259006014202062L, -67108865L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { -11329374758379726L, -67108865L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { 0L, 34368124932L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { -4610823915411078400L, -2287817355809522972L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = { -4610841507597123328L, -2287817493265254300L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = { 274863622425022464L, 1073742099L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	private static long[] mk_tokenSet_8_()
	{
		long[] data = { 3157167522465320960L, 53695478039L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
	private static long[] mk_tokenSet_9_()
	{
		long[] data = { 34L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
	private static long[] mk_tokenSet_10_()
	{
		long[] data = { -1174278579953870L, -2287795356987031553L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
	private static long[] mk_tokenSet_11_()
	{
		long[] data = { -2895018658392727758L, -2287817355809521665L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
	private static long[] mk_tokenSet_12_()
	{
		long[] data = { -134226126L, -67108865L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
	private static long[] mk_tokenSet_13_()
	{
		long[] data = { -2895089028210647262L, -2287817355809521665L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_13_ = new BitSet(mk_tokenSet_13_());
	private static long[] mk_tokenSet_14_()
	{
		long[] data = { -2891381473844134110L, -1L, 255L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_14_ = new BitSet(mk_tokenSet_14_());
	private static long[] mk_tokenSet_15_()
	{
		long[] data = { -2892507923506790622L, -274877906945L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_15_ = new BitSet(mk_tokenSet_15_());
	private static long[] mk_tokenSet_16_()
	{
		long[] data = { -70368878395598L, -1L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_16_ = new BitSet(mk_tokenSet_16_());
	private static long[] mk_tokenSet_17_()
	{
		long[] data = { -2895089577966461150L, -2287817355809521665L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_17_ = new BitSet(mk_tokenSet_17_());
	private static long[] mk_tokenSet_18_()
	{
		long[] data = { -3169953200387292928L, -2287817355826299156L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_18_ = new BitSet(mk_tokenSet_18_());
	private static long[] mk_tokenSet_19_()
	{
		long[] data = { 3106L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_19_ = new BitSet(mk_tokenSet_19_());
	private static long[] mk_tokenSet_20_()
	{
		long[] data = { 512L, 16777216L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_20_ = new BitSet(mk_tokenSet_20_());
	private static long[] mk_tokenSet_21_()
	{
		long[] data = { -3160963627682529008L, -2287817355826299804L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_21_ = new BitSet(mk_tokenSet_21_());
	private static long[] mk_tokenSet_22_()
	{
		long[] data = { 17592186044928L, 16777856L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_22_ = new BitSet(mk_tokenSet_22_());
	private static long[] mk_tokenSet_23_()
	{
		long[] data = { -2895089576892719326L, -2287817355809521665L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_23_ = new BitSet(mk_tokenSet_23_());
	private static long[] mk_tokenSet_24_()
	{
		long[] data = { 512L, 17592202821632L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_24_ = new BitSet(mk_tokenSet_24_());
	private static long[] mk_tokenSet_25_()
	{
		long[] data = { -4610841507597122816L, -2287817493248477084L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_25_ = new BitSet(mk_tokenSet_25_());
	private static long[] mk_tokenSet_26_()
	{
		long[] data = { -9077574971949262L, -17592253153281L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_26_ = new BitSet(mk_tokenSet_26_());
	private static long[] mk_tokenSet_27_()
	{
		long[] data = { 566935699968L, 17592239667200L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_27_ = new BitSet(mk_tokenSet_27_());
	private static long[] mk_tokenSet_28_()
	{
		long[] data = { 2990392490109960192L, 34368124932L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_28_ = new BitSet(mk_tokenSet_28_());
	private static long[] mk_tokenSet_29_()
	{
		long[] data = { -2891381473844134110L, -1L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_29_ = new BitSet(mk_tokenSet_29_());
	private static long[] mk_tokenSet_30_()
	{
		long[] data = { 17179886080L, 17592202821632L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_30_ = new BitSet(mk_tokenSet_30_());
	private static long[] mk_tokenSet_31_()
	{
		long[] data = { 17592186044928L, 22127755395712L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_31_ = new BitSet(mk_tokenSet_31_());
	private static long[] mk_tokenSet_32_()
	{
		long[] data = { 7768853540882239488L, 189523818775L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_32_ = new BitSet(mk_tokenSet_32_());
	private static long[] mk_tokenSet_33_()
	{
		long[] data = { 274862522371149824L, 273L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_33_ = new BitSet(mk_tokenSet_33_());
	private static long[] mk_tokenSet_34_()
	{
		long[] data = { 6917530266207649792L, 51547994118L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_34_ = new BitSet(mk_tokenSet_34_());
	private static long[] mk_tokenSet_35_()
	{
		long[] data = { 2990392490109960192L, 51547994116L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_35_ = new BitSet(mk_tokenSet_35_());
	private static long[] mk_tokenSet_36_()
	{
		long[] data = { -4611122999753703168L, 2323868524625262692L, 250L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_36_ = new BitSet(mk_tokenSet_36_());
	private static long[] mk_tokenSet_37_()
	{
		long[] data = { 4886549641926152192L, 52621736215L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_37_ = new BitSet(mk_tokenSet_37_());
	private static long[] mk_tokenSet_38_()
	{
		long[] data = { -1453656392945741056L, -2287817349367071753L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_38_ = new BitSet(mk_tokenSet_38_());
	private static long[] mk_tokenSet_39_()
	{
		long[] data = { 4611686019505324032L, 35441866756L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_39_ = new BitSet(mk_tokenSet_39_());
	private static long[] mk_tokenSet_40_()
	{
		long[] data = { 0L, 2147483648L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_40_ = new BitSet(mk_tokenSet_40_());
	private static long[] mk_tokenSet_41_()
	{
		long[] data = { 0L, 1101726220288L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_41_ = new BitSet(mk_tokenSet_41_());
	private static long[] mk_tokenSet_42_()
	{
		long[] data = { 655360L, 34368124932L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_42_ = new BitSet(mk_tokenSet_42_());
	private static long[] mk_tokenSet_43_()
	{
		long[] data = { -2891382023599947998L, -1L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_43_ = new BitSet(mk_tokenSet_43_());
	private static long[] mk_tokenSet_44_()
	{
		long[] data = { 1125899907497984L, 52118419460L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_44_ = new BitSet(mk_tokenSet_44_());
	private static long[] mk_tokenSet_45_()
	{
		long[] data = { 655360L, 51581548548L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_45_ = new BitSet(mk_tokenSet_45_());
	private static long[] mk_tokenSet_46_()
	{
		long[] data = { -580239230985334016L, -2287817355809521665L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_46_ = new BitSet(mk_tokenSet_46_());
	private static long[] mk_tokenSet_47_()
	{
		long[] data = { 32L, 52621735940L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_47_ = new BitSet(mk_tokenSet_47_());
	private static long[] mk_tokenSet_48_()
	{
		long[] data = { 4886549641926152224L, 52621736215L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_48_ = new BitSet(mk_tokenSet_48_());
	private static long[] mk_tokenSet_49_()
	{
		long[] data = { -586994623614836958L, -2287817355809521665L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_49_ = new BitSet(mk_tokenSet_49_());
	private static long[] mk_tokenSet_50_()
	{
		long[] data = { -9077568133144782L, -67108865L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_50_ = new BitSet(mk_tokenSet_50_());
	private static long[] mk_tokenSet_51_()
	{
		long[] data = { 0L, 10737418240L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_51_ = new BitSet(mk_tokenSet_51_());
	private static long[] mk_tokenSet_52_()
	{
		long[] data = { 0L, 67108864L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_52_ = new BitSet(mk_tokenSet_52_());
	private static long[] mk_tokenSet_53_()
	{
		long[] data = { 524288L, 34368124932L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_53_ = new BitSet(mk_tokenSet_53_());
	private static long[] mk_tokenSet_54_()
	{
		long[] data = { 0L, 137438953472L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_54_ = new BitSet(mk_tokenSet_54_());
	private static long[] mk_tokenSet_55_()
	{
		long[] data = { -2895089576892722912L, -2287817355826298881L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_55_ = new BitSet(mk_tokenSet_55_());
	private static long[] mk_tokenSet_56_()
	{
		long[] data = { 4611686018427387904L, 51547994116L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_56_ = new BitSet(mk_tokenSet_56_());
	private static long[] mk_tokenSet_57_()
	{
		long[] data = { -4611122999753686784L, 2323868662064216164L, 250L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_57_ = new BitSet(mk_tokenSet_57_());
	private static long[] mk_tokenSet_58_()
	{
		long[] data = { 2580705669023797248L, 1073742097L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_58_ = new BitSet(mk_tokenSet_58_());
	private static long[] mk_tokenSet_59_()
	{
		long[] data = { 0L, 51547994116L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_59_ = new BitSet(mk_tokenSet_59_());
	private static long[] mk_tokenSet_60_()
	{
		long[] data = { -4611122999753686272L, 2323868528937007204L, 250L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_60_ = new BitSet(mk_tokenSet_60_());
	private static long[] mk_tokenSet_61_()
	{
		long[] data = { 4611686019505324064L, 35441866756L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_61_ = new BitSet(mk_tokenSet_61_());
	private static long[] mk_tokenSet_62_()
	{
		long[] data = { 3801088L, 51581548550L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_62_ = new BitSet(mk_tokenSet_62_());
	private static long[] mk_tokenSet_63_()
	{
		long[] data = { 576460752303423488L, 190698223620L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_63_ = new BitSet(mk_tokenSet_63_());
	private static long[] mk_tokenSet_64_()
	{
		long[] data = { 0L, 10770972672L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_64_ = new BitSet(mk_tokenSet_64_());
	private static long[] mk_tokenSet_65_()
	{
		long[] data = { 6917529166157972000L, 35458643972L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_65_ = new BitSet(mk_tokenSet_65_());
	private static long[] mk_tokenSet_66_()
	{
		long[] data = { 2305843146652647456L, 1073741824L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_66_ = new BitSet(mk_tokenSet_66_());
	private static long[] mk_tokenSet_67_()
	{
		long[] data = { 9007199254740992L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_67_ = new BitSet(mk_tokenSet_67_());
	private static long[] mk_tokenSet_68_()
	{
		long[] data = { 7602078508537348096L, 51547994116L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_68_ = new BitSet(mk_tokenSet_68_());
	private static long[] mk_tokenSet_69_()
	{
		long[] data = { -3160946001132551424L, -2287817355809521940L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_69_ = new BitSet(mk_tokenSet_69_());
	private static long[] mk_tokenSet_70_()
	{
		long[] data = { 32L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_70_ = new BitSet(mk_tokenSet_70_());
	private static long[] mk_tokenSet_71_()
	{
		long[] data = { 2580705669023797280L, 1073742097L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_71_ = new BitSet(mk_tokenSet_71_());
	private static long[] mk_tokenSet_72_()
	{
		long[] data = { -178170687044976368L, -2287811572652835740L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_72_ = new BitSet(mk_tokenSet_72_());
	private static long[] mk_tokenSet_73_()
	{
		long[] data = { -4610841507597123328L, -2287817484675319708L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_73_ = new BitSet(mk_tokenSet_73_());
	private static long[] mk_tokenSet_74_()
	{
		long[] data = { -175870474380877040L, -17729692107676L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_74_ = new BitSet(mk_tokenSet_74_());
	private static long[] mk_tokenSet_75_()
	{
		long[] data = { -4609715607690280704L, -2287817355826300828L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_75_ = new BitSet(mk_tokenSet_75_());
	private static long[] mk_tokenSet_76_()
	{
		long[] data = { -4610841507597123328L, -2287817493198145436L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_76_ = new BitSet(mk_tokenSet_76_());
	private static long[] mk_tokenSet_77_()
	{
		long[] data = { -4610841507597123328L, -2287813095218743196L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_77_ = new BitSet(mk_tokenSet_77_());
	private static long[] mk_tokenSet_78_()
	{
		long[] data = { -2891382023599947998L, -274877906945L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_78_ = new BitSet(mk_tokenSet_78_());
	private static long[] mk_tokenSet_79_()
	{
		long[] data = { -9077574955172046L, -1L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_79_ = new BitSet(mk_tokenSet_79_());
	private static long[] mk_tokenSet_80_()
	{
		long[] data = { -2895089576875942110L, -2287795365509857281L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_80_ = new BitSet(mk_tokenSet_80_());
	private static long[] mk_tokenSet_81_()
	{
		long[] data = { -3160963627682529024L, -2287817355826299804L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_81_ = new BitSet(mk_tokenSet_81_());
	private static long[] mk_tokenSet_82_()
	{
		long[] data = { -178170687044976368L, -2287811568357868444L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_82_ = new BitSet(mk_tokenSet_82_());
	private static long[] mk_tokenSet_83_()
	{
		long[] data = { -4322892331544116992L, -2287817355826299804L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_83_ = new BitSet(mk_tokenSet_83_());
	private static long[] mk_tokenSet_84_()
	{
		long[] data = { -178104716368264432L, -17729692107036L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_84_ = new BitSet(mk_tokenSet_84_());
	private static long[] mk_tokenSet_85_()
	{
		long[] data = { -2895089576892719838L, -2287817355826298881L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_85_ = new BitSet(mk_tokenSet_85_());
	private static long[] mk_tokenSet_86_()
	{
		long[] data = { 7192392788578799616L, 51547994391L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_86_ = new BitSet(mk_tokenSet_86_());
	private static long[] mk_tokenSet_87_()
	{
		long[] data = { 0L, 1110316154880L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_87_ = new BitSet(mk_tokenSet_87_());
	private static long[] mk_tokenSet_88_()
	{
		long[] data = { 0L, 8657043456L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_88_ = new BitSet(mk_tokenSet_88_());
	private static long[] mk_tokenSet_89_()
	{
		long[] data = { -178170687065947888L, -2287811705796821916L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_89_ = new BitSet(mk_tokenSet_89_());
	private static long[] mk_tokenSet_90_()
	{
		long[] data = { -178122308554308848L, -17729692107676L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_90_ = new BitSet(mk_tokenSet_90_());
	private static long[] mk_tokenSet_91_()
	{
		long[] data = { 17592186044928L, 21990249333376L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_91_ = new BitSet(mk_tokenSet_91_());
	private static long[] mk_tokenSet_92_()
	{
		long[] data = { -178104716347292912L, -17592253153564L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_92_ = new BitSet(mk_tokenSet_92_());
	private static long[] mk_tokenSet_93_()
	{
		long[] data = { -9077574971949774L, -17592269930497L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_93_ = new BitSet(mk_tokenSet_93_());
	private static long[] mk_tokenSet_94_()
	{
		long[] data = { -175852882173861104L, -17592253153564L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_94_ = new BitSet(mk_tokenSet_94_());
	private static long[] mk_tokenSet_95_()
	{
		long[] data = { 35184372088832L, 12884901888L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_95_ = new BitSet(mk_tokenSet_95_());
	private static long[] mk_tokenSet_96_()
	{
		long[] data = { -4610841507576151808L, -2287817347236366236L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_96_ = new BitSet(mk_tokenSet_96_());
	private static long[] mk_tokenSet_97_()
	{
		long[] data = { -70375717208270L, -17592253153281L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_97_ = new BitSet(mk_tokenSet_97_());
	private static long[] mk_tokenSet_98_()
	{
		long[] data = { -166863000227159280L, -17592253154204L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_98_ = new BitSet(mk_tokenSet_98_());
	private static long[] mk_tokenSet_99_()
	{
		long[] data = { 512L, 21990249332736L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_99_ = new BitSet(mk_tokenSet_99_());
	private static long[] mk_tokenSet_100_()
	{
		long[] data = { -4610823915411078400L, -2287795503015920924L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_100_ = new BitSet(mk_tokenSet_100_());
	private static long[] mk_tokenSet_101_()
	{
		long[] data = { -175852882194832624L, -137506062620L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_101_ = new BitSet(mk_tokenSet_101_());
	private static long[] mk_tokenSet_102_()
	{
		long[] data = { 576460752303423488L, 1152670234628L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_102_ = new BitSet(mk_tokenSet_102_());
	private static long[] mk_tokenSet_103_()
	{
		long[] data = { -3169689626838548224L, -2287816383016207244L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_103_ = new BitSet(mk_tokenSet_103_());
	private static long[] mk_tokenSet_104_()
	{
		long[] data = { -3169689626838564608L, -2287817484675318684L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_104_ = new BitSet(mk_tokenSet_104_());
	private static long[] mk_tokenSet_105_()
	{
		long[] data = { -175852882194832624L, -17729692107036L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_105_ = new BitSet(mk_tokenSet_105_());
	private static long[] mk_tokenSet_106_()
	{
		long[] data = { -2895089570181833438L, -2287817355826298881L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_106_ = new BitSet(mk_tokenSet_106_());
	private static long[] mk_tokenSet_107_()
	{
		long[] data = { 0L, 1090719829721088L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_107_ = new BitSet(mk_tokenSet_107_());
	private static long[] mk_tokenSet_108_()
	{
		long[] data = { -4611122982573833984L, -2287817493265254300L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_108_ = new BitSet(mk_tokenSet_108_());
	private static long[] mk_tokenSet_109_()
	{
		long[] data = { 35184372088832L, 4294967296L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_109_ = new BitSet(mk_tokenSet_109_());
	private static long[] mk_tokenSet_110_()
	{
		long[] data = { -2895018659466469600L, -2287817355809521665L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_110_ = new BitSet(mk_tokenSet_110_());
	private static long[] mk_tokenSet_111_()
	{
		long[] data = { -11259006014202064L, -67108865L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_111_ = new BitSet(mk_tokenSet_111_());
	private static long[] mk_tokenSet_112_()
	{
		long[] data = { 274863623498764288L, 51539607827L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_112_ = new BitSet(mk_tokenSet_112_());
	private static long[] mk_tokenSet_113_()
	{
		long[] data = { -175870474380877552L, -83887004L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_113_ = new BitSet(mk_tokenSet_113_());
	private static long[] mk_tokenSet_114_()
	{
		long[] data = { 0L, 17592186044416L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_114_ = new BitSet(mk_tokenSet_114_());
	private static long[] mk_tokenSet_115_()
	{
		long[] data = { 329853488332800L, 7881299347898368L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_115_ = new BitSet(mk_tokenSet_115_());
	private static long[] mk_tokenSet_116_()
	{
		long[] data = { -4610806323224903424L, -2287817493265254300L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_116_ = new BitSet(mk_tokenSet_116_());
	private static long[] mk_tokenSet_117_()
	{
		long[] data = { 0L, 63051494294814720L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_117_ = new BitSet(mk_tokenSet_117_());
	private static long[] mk_tokenSet_118_()
	{
		long[] data = { 0L, 504403158802366464L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_118_ = new BitSet(mk_tokenSet_118_());
	private static long[] mk_tokenSet_119_()
	{
		long[] data = { 0L, -4593671619381035008L, 1L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_119_ = new BitSet(mk_tokenSet_119_());
	private static long[] mk_tokenSet_120_()
	{
		long[] data = { -9077574888063182L, -1L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_120_ = new BitSet(mk_tokenSet_120_());
	private static long[] mk_tokenSet_121_()
	{
		long[] data = { -2593228874535141120L, -2287816385163690908L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_121_ = new BitSet(mk_tokenSet_121_());
	private static long[] mk_tokenSet_122_()
	{
		long[] data = { 17179869184L, 3291136L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_122_ = new BitSet(mk_tokenSet_122_());
	private static long[] mk_tokenSet_123_()
	{
		long[] data = { 17179869184L, 145408L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_123_ = new BitSet(mk_tokenSet_123_());
	private static long[] mk_tokenSet_124_()
	{
		long[] data = { 0L, 3145728L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_124_ = new BitSet(mk_tokenSet_124_());
	private static long[] mk_tokenSet_125_()
	{
		long[] data = { 17179869184L, 8593225728L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_125_ = new BitSet(mk_tokenSet_125_());
	private static long[] mk_tokenSet_126_()
	{
		long[] data = { -175870474380877552L, -17592269931420L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_126_ = new BitSet(mk_tokenSet_126_());
	private static long[] mk_tokenSet_127_()
	{
		long[] data = { -175870474380877552L, -17729641776028L, 251L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_127_ = new BitSet(mk_tokenSet_127_());
	private static long[] mk_tokenSet_128_()
	{
		long[] data = { 0L, 4400193994752L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_128_ = new BitSet(mk_tokenSet_128_());
	private static long[] mk_tokenSet_129_()
	{
		long[] data = { 0L, 4406703554560L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_129_ = new BitSet(mk_tokenSet_129_());
	
}
}
