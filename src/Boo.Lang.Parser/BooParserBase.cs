// $ANTLR 2.7.5 (20050517): "src/Boo.Lang.Parser/boo.g" -> "BooParserBase.cs"$

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
		public const int ABSTRACT = 10;
		public const int AND = 11;
		public const int AS = 12;
		public const int BREAK = 13;
		public const int CONTINUE = 14;
		public const int CALLABLE = 15;
		public const int CAST = 16;
		public const int CHAR = 17;
		public const int CLASS = 18;
		public const int CONSTRUCTOR = 19;
		public const int DEF = 20;
		public const int DESTRUCTOR = 21;
		public const int DO = 22;
		public const int ELIF = 23;
		public const int ELSE = 24;
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
		public const int PASS = 50;
		public const int PARTIAL = 51;
		public const int PUBLIC = 52;
		public const int PROTECTED = 53;
		public const int PRIVATE = 54;
		public const int RAISE = 55;
		public const int REF = 56;
		public const int RETURN = 57;
		public const int SET = 58;
		public const int SELF = 59;
		public const int SUPER = 60;
		public const int STATIC = 61;
		public const int STRUCT = 62;
		public const int THEN = 63;
		public const int TRY = 64;
		public const int TRANSIENT = 65;
		public const int TRUE = 66;
		public const int TYPEOF = 67;
		public const int UNLESS = 68;
		public const int VIRTUAL = 69;
		public const int WHILE = 70;
		public const int YIELD = 71;
		public const int TRIPLE_QUOTED_STRING = 72;
		public const int EOS = 73;
		public const int LPAREN = 74;
		public const int RPAREN = 75;
		public const int DOUBLE_QUOTED_STRING = 76;
		public const int SINGLE_QUOTED_STRING = 77;
		public const int ID = 78;
		public const int LBRACK = 79;
		public const int RBRACK = 80;
		public const int ASSIGN = 81;
		public const int COMMA = 82;
		public const int ASSEMBLY_ATTRIBUTE_BEGIN = 83;
		public const int SPLICE_BEGIN = 84;
		public const int DOT = 85;
		public const int COLON = 86;
		public const int MULTIPLY = 87;
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
		public const int SUBTRACT = 104;
		public const int EXCLUSIVE_OR = 105;
		public const int DIVISION = 106;
		public const int MODULUS = 107;
		public const int BITWISE_AND = 108;
		public const int SHIFT_LEFT = 109;
		public const int SHIFT_RIGHT = 110;
		public const int LONG = 111;
		public const int INCREMENT = 112;
		public const int DECREMENT = 113;
		public const int ONES_COMPLEMENT = 114;
		public const int INT = 115;
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
		public const int NEWLINE = 128;
		public const int DQS_ESC = 129;
		public const int SQS_ESC = 130;
		public const int SESC = 131;
		public const int RE_CHAR = 132;
		public const int X_RE_CHAR = 133;
		public const int RE_OPTIONS = 134;
		public const int RE_ESC = 135;
		public const int DIGIT_GROUP = 136;
		public const int REVERSE_DIGIT_GROUP = 137;
		public const int AT_SYMBOL = 138;
		public const int ID_LETTER = 139;
		public const int DIGIT = 140;
		public const int HEXDIGIT = 141;
		
				
	protected System.Text.StringBuilder _sbuilder = new System.Text.StringBuilder();
	
	protected AttributeCollection _attributes = new AttributeCollection();
	
	protected TypeMemberModifiers _modifiers = TypeMemberModifiers.None;

	protected bool _inArray;
	
	protected bool _compact = false;
	
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
				case TRIPLE_QUOTED_STRING:
				case EOS:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case ASSEMBLY_ATTRIBUTE_BEGIN:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
						goto _loop7_breakloop;
					}
					
				}
_loop7_breakloop:				;
			}    // ( ... )*
			{    // ( ... )*
				for (;;)
				{
					bool synPredMatched11 = false;
					if ((((LA(1)==THEN||LA(1)==ID) && (tokenSet_4_.member(LA(2))))&&(IsValidMacroArgument(LA(2)))))
					{
						int _m11 = mark();
						synPredMatched11 = true;
						inputState.guessing++;
						try {
							{
								macro_name();
								{
									if ((tokenSet_5_.member(LA(1))))
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
					else if ((tokenSet_6_.member(LA(1))) && (tokenSet_7_.member(LA(2)))) {
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
					if ((LA(1)==ASSEMBLY_ATTRIBUTE_BEGIN))
					{
						assembly_attribute(module);
						eos();
					}
					else
					{
						goto _loop14_breakloop;
					}
					
				}
_loop14_breakloop:				;
			}    // ( ... )*
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
	
	protected void eos() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{ // ( ... )+
				int _cnt22=0;
				for (;;)
				{
					if ((LA(1)==EOL||LA(1)==EOS) && (tokenSet_9_.member(LA(2))))
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
						if (_cnt22 >= 1) { goto _loop22_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt22++;
				}
_loop22_breakloop:				;
			}    // ( ... )+
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
	
	protected void docstring(
		Node node
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  doc = null;
		
		try {      // for error handling
			{
				if ((LA(1)==TRIPLE_QUOTED_STRING) && (tokenSet_10_.member(LA(2))))
				{
					doc = LT(1);
					match(TRIPLE_QUOTED_STRING);
					if (0==inputState.guessing)
					{
						node.Documentation = DocStringFormatter.Format(doc.getText());
					}
					{
						if ((LA(1)==EOL||LA(1)==EOS) && (tokenSet_10_.member(LA(2))))
						{
							eos();
						}
						else if ((tokenSet_10_.member(LA(1))) && (tokenSet_11_.member(LA(2)))) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
				}
				else if ((tokenSet_10_.member(LA(1))) && (tokenSet_11_.member(LA(2)))) {
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
				recover(ex,tokenSet_10_);
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
				recover(ex,tokenSet_12_);
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
			node=import_directive_();
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
				reportError(ex);
				recover(ex,tokenSet_12_);
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
				recover(ex,tokenSet_13_);
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
								goto _loop429_breakloop;
							}
							
						}
_loop429_breakloop:						;
					}    // ( ... )*
				}
				else if ((tokenSet_14_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
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
				recover(ex,tokenSet_14_);
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
				reportError(ex);
				recover(ex,tokenSet_16_);
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
				recover(ex,tokenSet_16_);
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
				case BREAK:
				case CONTINUE:
				case CAST:
				case CHAR:
				case DEF:
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
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case ASSEMBLY_ATTRIBUTE_BEGIN:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
					if ((tokenSet_17_.member(LA(1))))
					{
						stmt(container.Globals.Statements);
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
				recover(ex,tokenSet_18_);
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
				recover(ex,tokenSet_19_);
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
				else if ((LA(1)==COLON) && (tokenSet_20_.member(LA(2)))) {
					macro_compound_stmt(macro.Body);
					if (0==inputState.guessing)
					{
						macro.Annotate("compound");
					}
				}
				else if ((tokenSet_21_.member(LA(1)))) {
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
				reportError(ex);
				recover(ex,tokenSet_22_);
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
						case THEN:
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
					alias = LT(1);
					match(ID);
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
				recover(ex,tokenSet_24_);
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
					if ((LA(1)==DOT) && (tokenSet_25_.member(LA(2))))
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
						goto _loop654_breakloop;
					}
					
				}
_loop654_breakloop:				;
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
				reportError(ex);
				recover(ex,tokenSet_13_);
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
				case THEN:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
				case DOT:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
								goto _loop642_breakloop;
							}
							
						}
_loop642_breakloop:						;
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
				reportError(ex);
				recover(ex,tokenSet_27_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void attributes() //throws RecognitionException, TokenStreamException
{
		
		
		
		
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
											goto _loop51_breakloop;
										}
										
									}
_loop51_breakloop:									;
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
							case ID:
							case LBRACK:
							case SPLICE_BEGIN:
							case COLON:
							case MULTIPLY:
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
						goto _loop53_breakloop;
					}
					
				}
_loop53_breakloop:				;
			}    // ( ... )*
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
					if ((tokenSet_29_.member(LA(1))))
					{
						type_member_modifier();
					}
					else
					{
						goto _loop196_breakloop;
					}
					
				}
_loop196_breakloop:				;
			}    // ( ... )*
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
				recover(ex,tokenSet_22_);
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
				case ID:
				case SPLICE_BEGIN:
				{
					{
						{
							{
								if ((LA(1)==ID) && (LA(2)==DOT))
								{
									emi=explicit_member_info();
								}
								else if ((tokenSet_31_.member(LA(1))) && (tokenSet_32_.member(LA(2)))) {
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
				reportError(ex);
				recover(ex,tokenSet_22_);
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
		IToken  begin = null;
		
				TypeDefinition td = null;
				TypeReferenceCollection baseTypes = null;
				TypeMemberCollection members = null;
				GenericParameterDeclarationCollection genericParameters = null;
				Expression nameSplice = null;
			
		
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
				case ID:
				{
					id = LT(1);
					match(ID);
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
				case EOS:
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
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
							case ID:
							case LBRACK:
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
						{ // ( ... )+
							int _cnt72=0;
							for (;;)
							{
								bool synPredMatched71 = false;
								if (((LA(1)==SPLICE_BEGIN) && (tokenSet_32_.member(LA(2)))))
								{
									int _m71 = mark();
									synPredMatched71 = true;
									inputState.guessing++;
									try {
										{
											splice_expression();
											eos();
										}
									}
									catch (RecognitionException)
									{
										synPredMatched71 = false;
									}
									rewind(_m71);
									inputState.guessing--;
								}
								if ( synPredMatched71 )
								{
									splice_type_definition_body(members);
								}
								else if ((tokenSet_33_.member(LA(1))) && (tokenSet_34_.member(LA(2)))) {
									type_definition_member(members);
								}
								else
								{
									if (_cnt72 >= 1) { goto _loop72_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
								}
								
								_cnt72++;
							}
_loop72_breakloop:							;
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
				reportError(ex);
				recover(ex,tokenSet_22_);
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
		IToken  begin = null;
		
				InterfaceDefinition itf = null;
				TypeMemberCollection members = null;
				GenericParameterDeclarationCollection genericParameters = null;
				Expression nameSplice = null;
			
		
		try {      // for error handling
			match(INTERFACE);
			{
				switch ( LA(1) )
				{
				case ID:
				{
					id = LT(1);
					match(ID);
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
				case ID:
				case LBRACK:
				{
					{ // ( ... )+
						int _cnt86=0;
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
								if (_cnt86 >= 1) { goto _loop86_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
							}
							
							_cnt86++;
						}
_loop86_breakloop:						;
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
				reportError(ex);
				recover(ex,tokenSet_22_);
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
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
				{
					{ // ( ... )+
						int _cnt44=0;
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
								if (_cnt44 >= 1) { goto _loop44_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
							}
							break; }
							_cnt44++;
						}
_loop44_breakloop:						;
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
				reportError(ex);
				recover(ex,tokenSet_22_);
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
				reportError(ex);
				recover(ex,tokenSet_22_);
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
						goto _loop227_breakloop;
					}
					
				}
_loop227_breakloop:				;
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
				case LBRACK:
				case SPLICE_BEGIN:
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
								goto _loop204_breakloop;
							}
							
						}
_loop204_breakloop:						;
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
					bool synPredMatched245 = false;
					if (((LA(1)==CALLABLE) && (LA(2)==LPAREN)))
					{
						int _m245 = mark();
						synPredMatched245 = true;
						inputState.guessing++;
						try {
							{
								match(CALLABLE);
								match(LPAREN);
							}
						}
						catch (RecognitionException)
						{
							synPredMatched245 = false;
						}
						rewind(_m245);
						inputState.guessing--;
					}
					if ( synPredMatched245 )
					{
						{
							tr=callable_type_reference();
						}
					}
					else if ((tokenSet_38_.member(LA(1))) && (tokenSet_39_.member(LA(2)))) {
						{
							id=type_name();
							{
								if ((LA(1)==LBRACK) && (tokenSet_40_.member(LA(2))))
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
											case LPAREN:
											case ID:
											case SPLICE_BEGIN:
											case MULTIPLY:
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
																goto _loop254_breakloop;
															}
															
														}
_loop254_breakloop:														;
													}    // ( ... )*
													match(RBRACK);
												}
												break;
											}
											case CALLABLE:
											case CHAR:
											case THEN:
											case LPAREN:
											case ID:
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
								else if ((LA(1)==OF) && (tokenSet_41_.member(LA(2)))) {
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
								else if ((tokenSet_39_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
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
								if ((LA(1)==NULLABLE_SUFFIX) && (tokenSet_39_.member(LA(2))))
								{
									match(NULLABLE_SUFFIX);
									if (0==inputState.guessing)
									{
										
														GenericTypeReference ntr = new GenericTypeReference(tr.LexicalInfo, "System.Nullable");
														ntr.GenericArguments.Add(tr);
														tr = ntr;
													
									}
								}
								else if ((tokenSet_39_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
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
					if ((LA(1)==MULTIPLY) && (tokenSet_39_.member(LA(2))))
					{
						match(MULTIPLY);
						if (0==inputState.guessing)
						{
							tr = CodeFactory.EnumerableTypeReferenceFor(tr);
						}
					}
					else if ((LA(1)==EXPONENTIATION) && (tokenSet_39_.member(LA(2)))) {
						match(EXPONENTIATION);
						if (0==inputState.guessing)
						{
							tr = CodeFactory.EnumerableTypeReferenceFor(CodeFactory.EnumerableTypeReferenceFor(tr));
						}
					}
					else
					{
						goto _loop260_breakloop;
					}
					
				}
_loop260_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_39_);
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
				Expression initializer = null;
			
		
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
				reportError(ex);
				recover(ex,tokenSet_43_);
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
				recover(ex,tokenSet_44_);
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
				if ((LA(1)==EOL||LA(1)==EOS) && (tokenSet_45_.member(LA(2))))
				{
					eos();
				}
				else if ((tokenSet_45_.member(LA(1))) && (tokenSet_46_.member(LA(2)))) {
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
				recover(ex,tokenSet_45_);
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
			case FALSE:
			case NOT:
			case NULL:
			case SELF:
			case SUPER:
			case THEN:
			case TRUE:
			case TYPEOF:
			case TRIPLE_QUOTED_STRING:
			case LPAREN:
			case DOUBLE_QUOTED_STRING:
			case SINGLE_QUOTED_STRING:
			case ID:
			case LBRACK:
			case COMMA:
			case SPLICE_BEGIN:
			case DOT:
			case MULTIPLY:
			case LBRACE:
			case QQ_BEGIN:
			case SUBTRACT:
			case LONG:
			case INCREMENT:
			case DECREMENT:
			case ONES_COMPLEMENT:
			case INT:
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
				reportError(ex);
				recover(ex,tokenSet_22_);
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
		
		IToken  t = null;
				
				antlr.IToken id = null;
				Boo.Lang.Compiler.Ast.Attribute attr = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case THEN:
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
				recover(ex,tokenSet_47_);
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
				case CAST:
				case CHAR:
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
				case DOT:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
								goto _loop646_breakloop;
							}
							
						}
_loop646_breakloop:						;
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
				recover(ex,tokenSet_48_);
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
					bool synPredMatched548 = false;
					if (((LA(1)==CHAR) && (LA(2)==LPAREN)))
					{
						int _m548 = mark();
						synPredMatched548 = true;
						inputState.guessing++;
						try {
							{
								match(CHAR);
								match(LPAREN);
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
						e=char_literal();
					}
					else if ((LA(1)==CHAR||LA(1)==THEN||LA(1)==ID) && (tokenSet_39_.member(LA(2)))) {
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
				recover(ex,tokenSet_39_);
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
				case LPAREN:
				case ID:
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
								goto _loop90_breakloop;
							}
							
						}
_loop90_breakloop:						;
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
				recover(ex,tokenSet_49_);
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
				reportError(ex);
				recover(ex,tokenSet_39_);
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
				case ID:
				case SPLICE_BEGIN:
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
				recover(ex,tokenSet_50_);
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
				recover(ex,tokenSet_50_);
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
		IToken  begin1 = null;
		IToken  s = null;
		IToken  lparen = null;
		IToken  id2 = null;
		IToken  begin2 = null;
		
			IToken id = null;
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
				bool synPredMatched150 = false;
				if (((LA(1)==SELF||LA(1)==ID||LA(1)==SPLICE_BEGIN) && (tokenSet_51_.member(LA(2)))))
				{
					int _m150 = mark();
					synPredMatched150 = true;
					inputState.guessing++;
					try {
						{
							property_header();
						}
					}
					catch (RecognitionException)
					{
						synPredMatched150 = false;
					}
					rewind(_m150);
					inputState.guessing--;
				}
				if ( synPredMatched150 )
				{
					{
						{
							if ((LA(1)==ID) && (LA(2)==DOT))
							{
								emi=explicit_member_info();
							}
							else if ((LA(1)==SELF||LA(1)==ID||LA(1)==SPLICE_BEGIN) && (tokenSet_51_.member(LA(2)))) {
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
								int _cnt160=0;
								for (;;)
								{
									if ((tokenSet_52_.member(LA(1))))
									{
										property_accessor(p);
									}
									else
									{
										if (_cnt160 >= 1) { goto _loop160_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
									}
									
									_cnt160++;
								}
_loop160_breakloop:								;
							}    // ( ... )+
							end(p);
						}
					}
				}
				else {
					bool synPredMatched163 = false;
					if (((LA(1)==THEN||LA(1)==ID) && (tokenSet_4_.member(LA(2)))))
					{
						int _m163 = mark();
						synPredMatched163 = true;
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
							synPredMatched163 = false;
						}
						rewind(_m163);
						inputState.guessing--;
					}
					if ( synPredMatched163 )
					{
						tm=member_macro();
					}
					else if ((LA(1)==ID||LA(1)==SPLICE_BEGIN) && (tokenSet_53_.member(LA(2)))) {
						{
							{
								switch ( LA(1) )
								{
								case ID:
								{
									id2 = LT(1);
									match(ID);
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
					reportError(ex);
					recover(ex,tokenSet_50_);
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
				recover(ex,tokenSet_54_);
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
				int _cnt110=0;
				for (;;)
				{
					if ((LA(1)==GET||LA(1)==SET||LA(1)==LBRACK))
					{
						interface_property_accessor(p);
					}
					else
					{
						if (_cnt110 >= 1) { goto _loop110_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt110++;
				}
_loop110_breakloop:				;
			}    // ( ... )+
			end(p);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_54_);
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
				recover(ex,tokenSet_13_);
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
				
						gpd = new GenericParameterDeclaration(ToLexicalInfo(id));
						gpd.Name = id.getText();
						c.Add(gpd);
					
			}
			{
				if ((LA(1)==LPAREN) && (tokenSet_55_.member(LA(2))))
				{
					match(LPAREN);
					generic_parameter_constraints(gpd);
					match(RPAREN);
				}
				else if ((LA(1)==LPAREN||LA(1)==RBRACK||LA(1)==COMMA) && (tokenSet_56_.member(LA(2)))) {
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
				recover(ex,tokenSet_57_);
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
				reportError(ex);
				recover(ex,tokenSet_58_);
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
				reportError(ex);
				recover(ex,tokenSet_59_);
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
				reportError(ex);
				recover(ex,tokenSet_60_);
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
								goto _loop126_breakloop;
							}
							
						}
_loop126_breakloop:						;
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
				recover(ex,tokenSet_61_);
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
				reportError(ex);
				recover(ex,tokenSet_62_);
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
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					{ // ( ... )+
						int _cnt193=0;
						for (;;)
						{
							if ((tokenSet_17_.member(LA(1))))
							{
								stmt(container);
							}
							else
							{
								if (_cnt193 >= 1) { goto _loop193_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
							}
							
							_cnt193++;
						}
_loop193_breakloop:						;
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
				recover(ex,tokenSet_63_);
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
							goto _loop141_breakloop;
						}
						
					}
_loop141_breakloop:					;
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
				reportError(ex);
				recover(ex,tokenSet_64_);
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
				reportError(ex);
				recover(ex,tokenSet_50_);
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
			bool synPredMatched174 = false;
			if (((tokenSet_32_.member(LA(1))) && (tokenSet_65_.member(LA(2)))))
			{
				int _m174 = mark();
				synPredMatched174 = true;
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
					synPredMatched174 = false;
				}
				rewind(_m174);
				inputState.guessing--;
			}
			if ( synPredMatched174 )
			{
				{
					e=slicing_expression();
					e=method_invocation_block(e);
				}
			}
			else if ((tokenSet_66_.member(LA(1))) && (tokenSet_67_.member(LA(2)))) {
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
				recover(ex,tokenSet_22_);
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
			
		
		try {      // for error handling
			e=atom();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==LBRACK) && (tokenSet_68_.member(LA(2))))
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
								case FALSE:
								case NOT:
								case NULL:
								case SELF:
								case SUPER:
								case THEN:
								case TRUE:
								case TYPEOF:
								case TRIPLE_QUOTED_STRING:
								case LPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case ID:
								case LBRACK:
								case SPLICE_BEGIN:
								case DOT:
								case COLON:
								case MULTIPLY:
								case LBRACE:
								case QQ_BEGIN:
								case SUBTRACT:
								case LONG:
								case INCREMENT:
								case DECREMENT:
								case ONES_COMPLEMENT:
								case INT:
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
												goto _loop583_breakloop;
											}
											
										}
_loop583_breakloop:										;
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
					else if ((LA(1)==DOT) && (tokenSet_31_.member(LA(2)))) {
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
						}
					}
					else if ((LA(1)==LPAREN) && (tokenSet_69_.member(LA(2)))) {
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
								case FALSE:
								case NOT:
								case NULL:
								case SELF:
								case SUPER:
								case THEN:
								case TRUE:
								case TYPEOF:
								case TRIPLE_QUOTED_STRING:
								case LPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case ID:
								case LBRACK:
								case SPLICE_BEGIN:
								case DOT:
								case MULTIPLY:
								case LBRACE:
								case QQ_BEGIN:
								case SUBTRACT:
								case LONG:
								case INCREMENT:
								case DECREMENT:
								case ONES_COMPLEMENT:
								case INT:
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
												goto _loop592_breakloop;
											}
											
										}
_loop592_breakloop:										;
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
								if ((LA(1)==LBRACE) && (tokenSet_70_.member(LA(2))))
								{
									{
										bool synPredMatched596 = false;
										if (((LA(1)==LBRACE) && (tokenSet_70_.member(LA(2)))))
										{
											int _m596 = mark();
											synPredMatched596 = true;
											inputState.guessing++;
											try {
												{
													hash_literal_test();
												}
											}
											catch (RecognitionException)
											{
												synPredMatched596 = false;
											}
											rewind(_m596);
											inputState.guessing--;
										}
										if ( synPredMatched596 )
										{
											initializer=hash_literal();
										}
										else if ((LA(1)==LBRACE) && (tokenSet_70_.member(LA(2)))) {
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
								else if ((tokenSet_71_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
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
						goto _loop597_breakloop;
					}
					
				}
_loop597_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_72_);
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
				reportError(ex);
				recover(ex,tokenSet_22_);
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
			case FALSE:
			case NOT:
			case NULL:
			case SELF:
			case SUPER:
			case THEN:
			case TRUE:
			case TYPEOF:
			case TRIPLE_QUOTED_STRING:
			case LPAREN:
			case DOUBLE_QUOTED_STRING:
			case SINGLE_QUOTED_STRING:
			case ID:
			case LBRACK:
			case SPLICE_BEGIN:
			case DOT:
			case MULTIPLY:
			case LBRACE:
			case QQ_BEGIN:
			case SUBTRACT:
			case LONG:
			case INCREMENT:
			case DECREMENT:
			case ONES_COMPLEMENT:
			case INT:
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
								if ((tokenSet_5_.member(LA(1))) && (tokenSet_73_.member(LA(2))))
								{
									e=expression();
									if (0==inputState.guessing)
									{
										tle.Items.Add(e);
									}
									{    // ( ... )*
										for (;;)
										{
											if ((LA(1)==COMMA) && (tokenSet_5_.member(LA(2))))
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
												goto _loop424_breakloop;
											}
											
										}
_loop424_breakloop:										;
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
										case TRIPLE_QUOTED_STRING:
										case EOS:
										case LPAREN:
										case RPAREN:
										case DOUBLE_QUOTED_STRING:
										case SINGLE_QUOTED_STRING:
										case ID:
										case LBRACK:
										case ASSEMBLY_ATTRIBUTE_BEGIN:
										case SPLICE_BEGIN:
										case DOT:
										case COLON:
										case MULTIPLY:
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
								else if ((tokenSet_74_.member(LA(1))) && (tokenSet_75_.member(LA(2)))) {
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
						case TRIPLE_QUOTED_STRING:
						case EOS:
						case LPAREN:
						case RPAREN:
						case DOUBLE_QUOTED_STRING:
						case SINGLE_QUOTED_STRING:
						case ID:
						case LBRACK:
						case ASSEMBLY_ATTRIBUTE_BEGIN:
						case SPLICE_BEGIN:
						case DOT:
						case COLON:
						case MULTIPLY:
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
				reportError(ex);
				recover(ex,tokenSet_22_);
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
			if ((LA(1)==COLON) && (tokenSet_76_.member(LA(2))))
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
				reportError(ex);
				recover(ex,tokenSet_45_);
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
					bool synPredMatched304 = false;
					if ((((LA(1)==THEN||LA(1)==ID) && (tokenSet_4_.member(LA(2))))&&(IsValidMacroArgument(LA(2)))))
					{
						int _m304 = mark();
						synPredMatched304 = true;
						inputState.guessing++;
						try {
							{
								macro_name();
								{
									if ((tokenSet_5_.member(LA(1))))
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
							synPredMatched304 = false;
						}
						rewind(_m304);
						inputState.guessing--;
					}
					if ( synPredMatched304 )
					{
						s=macro_stmt();
					}
					else {
						bool synPredMatched308 = false;
						if (((tokenSet_32_.member(LA(1))) && (tokenSet_77_.member(LA(2)))))
						{
							int _m308 = mark();
							synPredMatched308 = true;
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
								synPredMatched308 = false;
							}
							rewind(_m308);
							inputState.guessing--;
						}
						if ( synPredMatched308 )
						{
							s=assignment_or_method_invocation_with_block_stmt();
						}
						else {
							bool synPredMatched310 = false;
							if (((LA(1)==ID) && (LA(2)==AS||LA(2)==COMMA)))
							{
								int _m310 = mark();
								synPredMatched310 = true;
								inputState.guessing++;
								try {
									{
										declaration();
										match(COMMA);
									}
								}
								catch (RecognitionException)
								{
									synPredMatched310 = false;
								}
								rewind(_m310);
								inputState.guessing--;
							}
							if ( synPredMatched310 )
							{
								s=unpack_stmt();
							}
							else if ((LA(1)==ID) && (LA(2)==AS)) {
								s=declaration_stmt();
							}
							else if ((tokenSet_78_.member(LA(1))) && (tokenSet_79_.member(LA(2)))) {
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
										case THEN:
										case TRUE:
										case TYPEOF:
										case TRIPLE_QUOTED_STRING:
										case LPAREN:
										case DOUBLE_QUOTED_STRING:
										case SINGLE_QUOTED_STRING:
										case ID:
										case LBRACK:
										case SPLICE_BEGIN:
										case DOT:
										case MULTIPLY:
										case LBRACE:
										case QQ_BEGIN:
										case SUBTRACT:
										case LONG:
										case INCREMENT:
										case DECREMENT:
										case ONES_COMPLEMENT:
										case INT:
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
				recover(ex,tokenSet_41_);
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
		IToken  begin1 = null;
		IToken  id2 = null;
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
							case ID:
							{
								id1 = LT(1);
								match(ID);
								if (0==inputState.guessing)
								{
									id = id1;
								}
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
				case ID:
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
							case ID:
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
							case ID:
							{
								id2 = LT(1);
								match(ID);
								if (0==inputState.guessing)
								{
									id = id2;
								}
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
				recover(ex,tokenSet_39_);
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
				case LPAREN:
				case ID:
				case SPLICE_BEGIN:
				case MULTIPLY:
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
								goto _loop218_breakloop;
							}
							
						}
_loop218_breakloop:						;
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
				recover(ex,tokenSet_48_);
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
				case LPAREN:
				case ID:
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
							case THEN:
							case LPAREN:
							case ID:
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
				case THEN:
				case LPAREN:
				case ID:
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
				recover(ex,tokenSet_48_);
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
				if ((LA(1)==AS) && (tokenSet_41_.member(LA(2))))
				{
					match(AS);
					tr=type_reference();
					if (0==inputState.guessing)
					{
						
								ctr.ReturnType = tr; 
								
					}
				}
				else if ((tokenSet_39_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
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
				recover(ex,tokenSet_39_);
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
				reportError(ex);
				recover(ex,tokenSet_39_);
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
						goto _loop240_breakloop;
					}
					
				}
_loop240_breakloop:				;
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
				
						tr = new SpliceTypeReference(ToLexicalInfo(begin), e);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_39_);
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
				recover(ex,tokenSet_39_);
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
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
										case TRIPLE_QUOTED_STRING:
										case LPAREN:
										case DOUBLE_QUOTED_STRING:
										case SINGLE_QUOTED_STRING:
										case ID:
										case LBRACK:
										case SPLICE_BEGIN:
										case DOT:
										case COLON:
										case MULTIPLY:
										case LBRACE:
										case QQ_BEGIN:
										case SUBTRACT:
										case LONG:
										case INCREMENT:
										case DECREMENT:
										case ONES_COMPLEMENT:
										case INT:
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
									goto _loop276_breakloop;
								}
								
							}
_loop276_breakloop:							;
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
				int _cnt278=0;
				for (;;)
				{
					if ((LA(1)==EOL) && (tokenSet_45_.member(LA(2))))
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
						if (_cnt278 >= 1) { goto _loop278_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt278++;
				}
_loop278_breakloop:				;
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
				reportError(ex);
				recover(ex,tokenSet_45_);
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
				if (((LA(1)==THEN||LA(1)==ID) && (tokenSet_84_.member(LA(2))))&&(IsValidMacroArgument(LA(2))))
				{
					s=closure_macro_stmt();
				}
				else {
					bool synPredMatched317 = false;
					if (((tokenSet_32_.member(LA(1))) && (tokenSet_85_.member(LA(2)))))
					{
						int _m317 = mark();
						synPredMatched317 = true;
						inputState.guessing++;
						try {
							{
								slicing_expression();
								match(ASSIGN);
							}
						}
						catch (RecognitionException)
						{
							synPredMatched317 = false;
						}
						rewind(_m317);
						inputState.guessing--;
					}
					if ( synPredMatched317 )
					{
						s=assignment_or_method_invocation();
					}
					else if ((LA(1)==RETURN)) {
						s=return_expression_stmt();
					}
					else {
						bool synPredMatched319 = false;
						if (((LA(1)==ID) && (LA(2)==AS||LA(2)==COMMA)))
						{
							int _m319 = mark();
							synPredMatched319 = true;
							inputState.guessing++;
							try {
								{
									declaration();
									match(COMMA);
								}
							}
							catch (RecognitionException)
							{
								synPredMatched319 = false;
							}
							rewind(_m319);
							inputState.guessing--;
						}
						if ( synPredMatched319 )
						{
							s=unpack();
						}
						else if ((LA(1)==ID) && (LA(2)==AS)) {
							s=declaration_stmt();
						}
						else if ((tokenSet_78_.member(LA(1))) && (tokenSet_86_.member(LA(2)))) {
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
									case THEN:
									case TRUE:
									case TYPEOF:
									case TRIPLE_QUOTED_STRING:
									case LPAREN:
									case DOUBLE_QUOTED_STRING:
									case SINGLE_QUOTED_STRING:
									case ID:
									case LBRACK:
									case SPLICE_BEGIN:
									case DOT:
									case MULTIPLY:
									case LBRACE:
									case QQ_BEGIN:
									case SUBTRACT:
									case LONG:
									case INCREMENT:
									case DECREMENT:
									case ONES_COMPLEMENT:
									case INT:
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
						reportError(ex);
						recover(ex,tokenSet_19_);
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
				reportError(ex);
				recover(ex,tokenSet_87_);
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
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
				case RE_LITERAL:
				case DOUBLE:
				case FLOAT:
				case TIMESPAN:
				{
					{ // ( ... )+
						int _cnt285=0;
						for (;;)
						{
							if ((tokenSet_17_.member(LA(1))) && (tokenSet_88_.member(LA(2))))
							{
								stmt(container);
							}
							else if ((tokenSet_33_.member(LA(1))) && (tokenSet_34_.member(LA(2)))) {
								type_member_stmt(container);
							}
							else
							{
								if (_cnt285 >= 1) { goto _loop285_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
							}
							
							_cnt285++;
						}
_loop285_breakloop:						;
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
				recover(ex,tokenSet_63_);
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
				reportError(ex);
				recover(ex,tokenSet_50_);
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
			if ((LA(1)==COLON) && (tokenSet_76_.member(LA(2))))
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
				reportError(ex);
				recover(ex,tokenSet_22_);
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
				reportError(ex);
				recover(ex,tokenSet_14_);
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
				recover(ex,tokenSet_21_);
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
				recover(ex,tokenSet_21_);
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
		IToken  id = null;
		
			stmt = null;
			BlockExpression be = null;
			Block body = null;
			TypeReference rt = null;
		
		
		try {      // for error handling
			def = LT(1);
			match(DEF);
			id = LT(1);
			match(ID);
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
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case ASSEMBLY_ATTRIBUTE_BEGIN:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
				else if ((tokenSet_80_.member(LA(1))) && (tokenSet_46_.member(LA(2)))) {
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
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case ASSEMBLY_ATTRIBUTE_BEGIN:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
				else if ((tokenSet_80_.member(LA(1))) && (tokenSet_46_.member(LA(2)))) {
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
						goto _loop407_breakloop;
					}
					
				}
_loop407_breakloop:				;
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
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case ASSEMBLY_ATTRIBUTE_BEGIN:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
						goto _loop363_breakloop;
					}
					
				}
_loop363_breakloop:				;
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
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case ASSEMBLY_ATTRIBUTE_BEGIN:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case ASSEMBLY_ATTRIBUTE_BEGIN:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
				recover(ex,tokenSet_80_);
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
								if (((tokenSet_66_.member(LA(1))) && (tokenSet_89_.member(LA(2))))&&(_compact))
								{
									rhs=array_or_expression();
								}
								else {
									bool synPredMatched486 = false;
									if (((LA(1)==DEF||LA(1)==DO||LA(1)==COLON)))
									{
										int _m486 = mark();
										synPredMatched486 = true;
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
											synPredMatched486 = false;
										}
										rewind(_m486);
										inputState.guessing--;
									}
									if ( synPredMatched486 )
									{
										rhs=callable_expression();
									}
									else if ((tokenSet_66_.member(LA(1))) && (tokenSet_90_.member(LA(2)))) {
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
				case THEN:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case COMMA:
				case SPLICE_BEGIN:
				case DOT:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
				recover(ex,tokenSet_91_);
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
						{
							if (((tokenSet_92_.member(LA(1))) && (tokenSet_93_.member(LA(2))))&&(_compact))
							{
								initializer=simple_initializer();
							}
							else if ((tokenSet_92_.member(LA(1))) && (tokenSet_94_.member(LA(2)))) {
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
				case THEN:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case COMMA:
				case SPLICE_BEGIN:
				case DOT:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
				reportError(ex);
				recover(ex,tokenSet_87_);
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
				recover(ex,tokenSet_21_);
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
				recover(ex,tokenSet_21_);
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
				case THEN:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
				case DOT:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
				reportError(ex);
				recover(ex,tokenSet_87_);
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
				recover(ex,tokenSet_21_);
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
				reportError(ex);
				recover(ex,tokenSet_19_);
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
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case COMMA:
				case SPLICE_BEGIN:
				case DOT:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
				else if ((tokenSet_95_.member(LA(1)))) {
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
				reportError(ex);
				recover(ex,tokenSet_95_);
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
				recover(ex,tokenSet_87_);
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
								
												BinaryExpression be = new BinaryExpression(ToLexicalInfo(ot));
												be.Operator = BinaryOperatorType.Or;
												be.Left = e;
												be.Right = r;
												e = be;
											
							}
						}
						else
						{
							goto _loop435_breakloop;
						}
						
					}
_loop435_breakloop:					;
				}    // ( ... )*
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
			case THEN:
			case TRUE:
			case TYPEOF:
			case TRIPLE_QUOTED_STRING:
			case LPAREN:
			case DOUBLE_QUOTED_STRING:
			case SINGLE_QUOTED_STRING:
			case ID:
			case LBRACK:
			case COMMA:
			case SPLICE_BEGIN:
			case DOT:
			case MULTIPLY:
			case LBRACE:
			case QQ_BEGIN:
			case SUBTRACT:
			case LONG:
			case INCREMENT:
			case DECREMENT:
			case ONES_COMPLEMENT:
			case INT:
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
						goto _loop331_breakloop;
					}
					
				}
_loop331_breakloop:				;
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
				case THEN:
				case TRUE:
				case TYPEOF:
				case YIELD:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case COMMA:
				case SPLICE_BEGIN:
				case DOT:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
								bool synPredMatched337 = false;
								if (((LA(1)==ID) && (LA(2)==AS||LA(2)==COMMA)))
								{
									int _m337 = mark();
									synPredMatched337 = true;
									inputState.guessing++;
									try {
										{
											declaration();
											match(COMMA);
										}
									}
									catch (RecognitionException)
									{
										synPredMatched337 = false;
									}
									rewind(_m337);
									inputState.guessing--;
								}
								if ( synPredMatched337 )
								{
									stmt=unpack();
								}
								else if (((LA(1)==THEN||LA(1)==ID) && (tokenSet_97_.member(LA(2))))&&(IsValidClosureMacroArgument(LA(2)))) {
									stmt=closure_macro_stmt();
								}
								else if ((tokenSet_66_.member(LA(1))) && (tokenSet_98_.member(LA(2)))) {
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
				reportError(ex);
				recover(ex,tokenSet_95_);
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
				reportError(ex);
				recover(ex,tokenSet_87_);
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
				bool synPredMatched343 = false;
				if (((tokenSet_99_.member(LA(1))) && (tokenSet_100_.member(LA(2)))))
				{
					int _m343 = mark();
					synPredMatched343 = true;
					inputState.guessing++;
					try {
						{
							closure_parameters_test();
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
					{
						parameter_declaration_list(parameters);
						match(BITWISE_OR);
					}
				}
				else if ((tokenSet_101_.member(LA(1))) && (tokenSet_102_.member(LA(2)))) {
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
								case TRIPLE_QUOTED_STRING:
								case LPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case ID:
								case LBRACK:
								case COMMA:
								case SPLICE_BEGIN:
								case DOT:
								case MULTIPLY:
								case LBRACE:
								case QQ_BEGIN:
								case SUBTRACT:
								case LONG:
								case INCREMENT:
								case DECREMENT:
								case ONES_COMPLEMENT:
								case INT:
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
							goto _loop348_breakloop;
						}
						
					}
_loop348_breakloop:					;
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
				reportError(ex);
				recover(ex,tokenSet_39_);
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
		IToken  u = null;
		
				ExceptionHandler eh = null;		
				TypeReference tr = null;
				Expression e = null;
			
		
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
				reportError(ex);
				recover(ex,tokenSet_103_);
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
						
									BinaryExpression be = new BinaryExpression(ToLexicalInfo(token));
									be.Operator = binaryOperator;
									be.Left = e;
									be.Right = r;
									e = be;
								
					}
					break;
				}
				case EOF:
				case DEDENT:
				case ESEPARATOR:
				case EOL:
				case ABSTRACT:
				case AND:
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
				case FOR:
				case FALSE:
				case GOTO:
				case INTERFACE:
				case INTERNAL:
				case IF:
				case NEW:
				case NULL:
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
				case TRIPLE_QUOTED_STRING:
				case EOS:
				case LPAREN:
				case RPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case RBRACK:
				case COMMA:
				case ASSEMBLY_ATTRIBUTE_BEGIN:
				case SPLICE_BEGIN:
				case DOT:
				case COLON:
				case MULTIPLY:
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
				recover(ex,tokenSet_104_);
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
						goto _loop415_breakloop;
					}
					
				}
_loop415_breakloop:				;
			}    // ( ... )*
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
				if ((LA(1)==IF||LA(1)==UNLESS||LA(1)==WHILE) && (tokenSet_5_.member(LA(2))))
				{
					filter=stmt_modifier();
					if (0==inputState.guessing)
					{
						ge.Filter = filter;
					}
				}
				else if ((tokenSet_14_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
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
				recover(ex,tokenSet_14_);
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
							
										BinaryExpression be = new BinaryExpression(ToLexicalInfo(at));
										be.Operator = BinaryOperatorType.And;
										be.Left = e;
										be.Right = r; 
										e = be;
									
						}
					}
					else
					{
						goto _loop438_breakloop;
					}
					
				}
_loop438_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_106_);
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
				case THEN:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
				case DOT:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
				recover(ex,tokenSet_104_);
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
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case COMMA:
				case SPLICE_BEGIN:
				case DOT:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
				reportError(ex);
				recover(ex,tokenSet_39_);
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
			bool synPredMatched447 = false;
			if (((tokenSet_107_.member(LA(1))) && (tokenSet_108_.member(LA(2)))))
			{
				int _m447 = mark();
				synPredMatched447 = true;
				inputState.guessing++;
				try {
					{
						ast_literal_module_prediction();
					}
				}
				catch (RecognitionException)
				{
					synPredMatched447 = false;
				}
				rewind(_m447);
				inputState.guessing--;
			}
			if ( synPredMatched447 )
			{
				{
					ast_literal_module(e);
				}
			}
			else {
				bool synPredMatched457 = false;
				if (((tokenSet_33_.member(LA(1))) && (tokenSet_34_.member(LA(2)))))
				{
					int _m457 = mark();
					synPredMatched457 = true;
					inputState.guessing++;
					try {
						{
							attributes();
							{
								if ((tokenSet_29_.member(LA(1))) && (true))
								{
									type_member_modifier();
								}
								else if ((tokenSet_109_.member(LA(1))) && (true)) {
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
											case SPLICE_BEGIN:
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
						synPredMatched457 = false;
					}
					rewind(_m457);
					inputState.guessing--;
				}
				if ( synPredMatched457 )
				{
					{
						{ // ( ... )+
							int _cnt460=0;
							for (;;)
							{
								if ((tokenSet_33_.member(LA(1))))
								{
									type_definition_member(collection);
								}
								else
								{
									if (_cnt460 >= 1) { goto _loop460_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
								}
								
								_cnt460++;
							}
_loop460_breakloop:							;
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
				else if ((tokenSet_17_.member(LA(1))) && (tokenSet_88_.member(LA(2)))) {
					{ // ( ... )+
						int _cnt462=0;
						for (;;)
						{
							if ((tokenSet_17_.member(LA(1))))
							{
								stmt(statements);
							}
							else
							{
								if (_cnt462 >= 1) { goto _loop462_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
							}
							
							_cnt462++;
						}
_loop462_breakloop:						;
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
					recover(ex,tokenSet_63_);
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
			bool synPredMatched469 = false;
			if (((tokenSet_5_.member(LA(1))) && (tokenSet_110_.member(LA(2)))))
			{
				int _m469 = mark();
				synPredMatched469 = true;
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
					synPredMatched469 = false;
				}
				rewind(_m469);
				inputState.guessing--;
			}
			if ( synPredMatched469 )
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
			else if ((tokenSet_101_.member(LA(1))) && (tokenSet_98_.member(LA(2)))) {
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
									case TRIPLE_QUOTED_STRING:
									case LPAREN:
									case DOUBLE_QUOTED_STRING:
									case SINGLE_QUOTED_STRING:
									case ID:
									case LBRACK:
									case COMMA:
									case SPLICE_BEGIN:
									case DOT:
									case MULTIPLY:
									case LBRACE:
									case QQ_BEGIN:
									case SUBTRACT:
									case LONG:
									case INCREMENT:
									case DECREMENT:
									case ONES_COMPLEMENT:
									case INT:
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
								goto _loop476_breakloop;
							}
							
						}
_loop476_breakloop:						;
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
				reportError(ex);
				recover(ex,tokenSet_111_);
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
				recover(ex,tokenSet_63_);
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
					if ((tokenSet_112_.member(LA(1))))
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
											else if ((LA(1)==IS) && (tokenSet_113_.member(LA(2)))) {
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
						goto _loop519_breakloop;
					}
					
				}
_loop519_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_114_);
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
					if ((tokenSet_115_.member(LA(1))) && (tokenSet_113_.member(LA(2))))
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
						goto _loop523_breakloop;
					}
					
				}
_loop523_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_116_);
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
					if ((tokenSet_117_.member(LA(1))) && (tokenSet_113_.member(LA(2))))
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
						goto _loop527_breakloop;
					}
					
				}
_loop527_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_118_);
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
						goto _loop531_breakloop;
					}
					
				}
_loop531_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_119_);
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
				if ((LA(1)==AS))
				{
					t = LT(1);
					match(AS);
					tr=type_reference();
					if (0==inputState.guessing)
					{
						e = new TryCastExpression(ToLexicalInfo(t)) { Target = e, Type = tr };
					}
				}
				else if ((LA(1)==CAST) && (tokenSet_41_.member(LA(2)))) {
					c = LT(1);
					match(CAST);
					tr=type_reference();
					if (0==inputState.guessing)
					{
						e = new CastExpression(ToLexicalInfo(c)) { Target = e, Type = tr };
					}
				}
				else if ((tokenSet_120_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==EXPONENTIATION) && (tokenSet_113_.member(LA(2))))
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
						goto _loop535_breakloop;
					}
					
				}
_loop535_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_120_);
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
				bool synPredMatched539 = false;
				if (((LA(1)==SUBTRACT||LA(1)==LONG||LA(1)==INT) && (tokenSet_72_.member(LA(2)))))
				{
					int _m539 = mark();
					synPredMatched539 = true;
					inputState.guessing++;
					try {
						{
							match(SUBTRACT);
							match(LONG);
						}
					}
					catch (RecognitionException)
					{
						synPredMatched539 = false;
					}
					rewind(_m539);
					inputState.guessing--;
				}
				if ( synPredMatched539 )
				{
					{
						e=integer_literal();
					}
				}
				else if ((tokenSet_121_.member(LA(1))) && (tokenSet_113_.member(LA(2)))) {
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
				else if ((tokenSet_32_.member(LA(1))) && (tokenSet_122_.member(LA(2)))) {
					{
						e=slicing_expression();
						{
							if ((LA(1)==INCREMENT) && (tokenSet_72_.member(LA(2))))
							{
								postinc = LT(1);
								match(INCREMENT);
								if (0==inputState.guessing)
								{
									op = postinc; uOperator = UnaryOperatorType.PostIncrement;
								}
							}
							else if ((LA(1)==DECREMENT) && (tokenSet_72_.member(LA(2)))) {
								postdec = LT(1);
								match(DECREMENT);
								if (0==inputState.guessing)
								{
									op = postdec; uOperator = UnaryOperatorType.PostDecrement;
								}
							}
							else if ((tokenSet_72_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
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
				recover(ex,tokenSet_72_);
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
					if ((LA(1)==SUBTRACT||LA(1)==LONG||LA(1)==INT) && (tokenSet_39_.member(LA(2))))
					{
						e=integer_literal();
					}
					else {
						bool synPredMatched602 = false;
						if (((LA(1)==LBRACE) && (tokenSet_70_.member(LA(2)))))
						{
							int _m602 = mark();
							synPredMatched602 = true;
							inputState.guessing++;
							try {
								{
									hash_literal_test();
								}
							}
							catch (RecognitionException)
							{
								synPredMatched602 = false;
							}
							rewind(_m602);
							inputState.guessing--;
						}
						if ( synPredMatched602 )
						{
							e=hash_literal();
						}
						else if ((LA(1)==LBRACE) && (tokenSet_123_.member(LA(2)))) {
							e=closure_expression();
						}
						else if ((LA(1)==SUBTRACT||LA(1)==DOUBLE||LA(1)==FLOAT) && (tokenSet_39_.member(LA(2)))) {
							e=double_literal();
						}
						else if ((LA(1)==SUBTRACT||LA(1)==TIMESPAN) && (tokenSet_39_.member(LA(2)))) {
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
					recover(ex,tokenSet_39_);
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
				reportError(ex);
				recover(ex,tokenSet_39_);
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
				reportError(ex);
				recover(ex,tokenSet_39_);
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
			bool synPredMatched559 = false;
			if (((LA(1)==LPAREN) && (LA(2)==OF)))
			{
				int _m559 = mark();
				synPredMatched559 = true;
				inputState.guessing++;
				try {
					{
						match(LPAREN);
						match(OF);
					}
				}
				catch (RecognitionException)
				{
					synPredMatched559 = false;
				}
				rewind(_m559);
				inputState.guessing--;
			}
			if ( synPredMatched559 )
			{
				e=typed_array();
			}
			else if ((LA(1)==LPAREN) && (tokenSet_66_.member(LA(2)))) {
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
				reportError(ex);
				recover(ex,tokenSet_39_);
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
				reportError(ex);
				recover(ex,tokenSet_39_);
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
				recover(ex,tokenSet_39_);
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
				recover(ex,tokenSet_39_);
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
				case FALSE:
				case NOT:
				case NULL:
				case SELF:
				case SUPER:
				case THEN:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
				case DOT:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
								if ((LA(1)==COMMA) && (tokenSet_5_.member(LA(2))))
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
									goto _loop566_breakloop;
								}
								
							}
_loop566_breakloop:							;
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
				recover(ex,tokenSet_39_);
			}
			else
			{
				throw ex;
			}
		}
		return e;
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
							case THEN:
							case TRUE:
							case TYPEOF:
							case TRIPLE_QUOTED_STRING:
							case LPAREN:
							case DOUBLE_QUOTED_STRING:
							case SINGLE_QUOTED_STRING:
							case ID:
							case LBRACK:
							case SPLICE_BEGIN:
							case DOT:
							case MULTIPLY:
							case LBRACE:
							case QQ_BEGIN:
							case SUBTRACT:
							case LONG:
							case INCREMENT:
							case DECREMENT:
							case ONES_COMPLEMENT:
							case INT:
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
				case THEN:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
				case DOT:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
								case THEN:
								case TRUE:
								case TYPEOF:
								case TRIPLE_QUOTED_STRING:
								case LPAREN:
								case DOUBLE_QUOTED_STRING:
								case SINGLE_QUOTED_STRING:
								case ID:
								case LBRACK:
								case SPLICE_BEGIN:
								case DOT:
								case MULTIPLY:
								case LBRACE:
								case QQ_BEGIN:
								case SUBTRACT:
								case LONG:
								case INCREMENT:
								case DECREMENT:
								case ONES_COMPLEMENT:
								case INT:
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
				recover(ex,tokenSet_47_);
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
		
				
				Expression value = null;
				ExpressionPair pair = null;
			
		
		try {      // for error handling
			bool synPredMatched649 = false;
			if (((tokenSet_5_.member(LA(1))) && (tokenSet_124_.member(LA(2)))))
			{
				int _m649 = mark();
				synPredMatched649 = true;
				inputState.guessing++;
				try {
					{
						expression_pair();
					}
				}
				catch (RecognitionException)
				{
					synPredMatched649 = false;
				}
				rewind(_m649);
				inputState.guessing--;
			}
			if ( synPredMatched649 )
			{
				{
					pair=expression_pair();
					if (0==inputState.guessing)
					{
						if (pair != null) node.NamedArguments.Add(pair);
					}
				}
			}
			else if ((tokenSet_5_.member(LA(1))) && (tokenSet_125_.member(LA(2)))) {
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
				case THEN:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
				case DOT:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
				dle = new HashLiteralExpression(ToLexicalInfo(lbrace));
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
				case THEN:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
				case DOT:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
							if ((LA(1)==COMMA) && (tokenSet_5_.member(LA(2))))
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
								goto _loop631_breakloop;
							}
							
						}
_loop631_breakloop:						;
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
				recover(ex,tokenSet_39_);
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
				recover(ex,tokenSet_71_);
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
				case THEN:
				case TRUE:
				case TYPEOF:
				case TRIPLE_QUOTED_STRING:
				case LPAREN:
				case DOUBLE_QUOTED_STRING:
				case SINGLE_QUOTED_STRING:
				case ID:
				case LBRACK:
				case SPLICE_BEGIN:
				case DOT:
				case MULTIPLY:
				case LBRACE:
				case QQ_BEGIN:
				case SUBTRACT:
				case LONG:
				case INCREMENT:
				case DECREMENT:
				case ONES_COMPLEMENT:
				case INT:
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
								if ((LA(1)==COMMA) && (tokenSet_5_.member(LA(2))))
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
									goto _loop623_breakloop;
								}
								
							}
_loop623_breakloop:							;
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
				recover(ex,tokenSet_126_);
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
				recover(ex,tokenSet_39_);
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
				recover(ex,tokenSet_39_);
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
				reportError(ex);
				recover(ex,tokenSet_39_);
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
				recover(ex,tokenSet_39_);
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
				recover(ex,tokenSet_39_);
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
				recover(ex,tokenSet_39_);
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
				recover(ex,tokenSet_39_);
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
				reportError(ex);
				recover(ex,tokenSet_39_);
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
				reportError(ex);
				recover(ex,tokenSet_39_);
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
		IToken  formatString = null;
		IToken  endsep = null;
		IToken  lastseparator = null;
		
			e = null;
			Expression param = null;
			LexicalInfo info = null;
		
		
		try {      // for error handling
			{
				if ((LA(1)==ESEPARATOR) && (LA(2)==ESEPARATOR))
				{
					firstseparator = LT(1);
					match(ESEPARATOR);
				}
				else if ((LA(1)==ESEPARATOR) && (tokenSet_5_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			{ // ( ... )+
				int _cnt616=0;
				for (;;)
				{
					if ((LA(1)==ESEPARATOR) && (tokenSet_5_.member(LA(2))))
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
						endsep = LT(1);
						match(ESEPARATOR);
					}
					else
					{
						if (_cnt616 >= 1) { goto _loop616_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt616++;
				}
_loop616_breakloop:				;
			}    // ( ... )+
			{
				if ((LA(1)==ESEPARATOR) && (tokenSet_39_.member(LA(2))))
				{
					lastseparator = LT(1);
					match(ESEPARATOR);
				}
				else if ((tokenSet_39_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
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
				recover(ex,tokenSet_39_);
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
				recover(ex,tokenSet_127_);
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
		@"""TRIPLE_QUOTED_STRING""",
		@"""EOS""",
		@"""LPAREN""",
		@"""RPAREN""",
		@"""DOUBLE_QUOTED_STRING""",
		@"""SINGLE_QUOTED_STRING""",
		@"""ID""",
		@"""LBRACK""",
		@"""RBRACK""",
		@"""ASSIGN""",
		@"""COMMA""",
		@"""ASSEMBLY_ATTRIBUTE_BEGIN""",
		@"""SPLICE_BEGIN""",
		@"""DOT""",
		@"""COLON""",
		@"""MULTIPLY""",
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
		long[] data = { -361877334580009182L, 71917956748605439L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { -1407377895065806L, 72057594021148671L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { -1416173988088014L, 72057594021148671L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { -7493882019214851328L, 71917956748080988L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { -7493884218238107392L, 71917956743886092L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = { 6951886980444881920L, 32802L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = { -1911197068903545856L, 1130530L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	private static long[] mk_tokenSet_8_()
	{
		long[] data = { 34L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
	private static long[] mk_tokenSet_9_()
	{
		long[] data = { -146786956609742L, 71917958091044863L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
	private static long[] mk_tokenSet_10_()
	{
		long[] data = { -361877334445791438L, 71917956748605439L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
	private static long[] mk_tokenSet_11_()
	{
		long[] data = { -2147485902L, 72057594021148671L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
	private static long[] mk_tokenSet_12_()
	{
		long[] data = { -361886130673031390L, 71917956748605439L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
	private static long[] mk_tokenSet_13_()
	{
		long[] data = { -361422752938721502L, 72057594037927935L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_13_ = new BitSet(mk_tokenSet_13_());
	private static long[] mk_tokenSet_14_()
	{
		long[] data = { -361886199254096094L, 71917958091112447L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_14_ = new BitSet(mk_tokenSet_14_());
	private static long[] mk_tokenSet_15_()
	{
		long[] data = { -8796093022414L, 72057594037927935L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_15_ = new BitSet(mk_tokenSet_15_());
	private static long[] mk_tokenSet_16_()
	{
		long[] data = { -361886199392508126L, 71917956748605439L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_16_ = new BitSet(mk_tokenSet_16_());
	private static long[] mk_tokenSet_17_()
	{
		long[] data = { -7313773179836342016L, 71917956748080605L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_17_ = new BitSet(mk_tokenSet_17_());
	private static long[] mk_tokenSet_18_()
	{
		long[] data = { 34L, 524288L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_18_ = new BitSet(mk_tokenSet_18_());
	private static long[] mk_tokenSet_19_()
	{
		long[] data = { 512L, 512L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_19_ = new BitSet(mk_tokenSet_19_());
	private static long[] mk_tokenSet_20_()
	{
		long[] data = { -7312649483248770800L, 71917956748080524L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_20_ = new BitSet(mk_tokenSet_20_());
	private static long[] mk_tokenSet_21_()
	{
		long[] data = { 2199023256064L, 592L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_21_ = new BitSet(mk_tokenSet_21_());
	private static long[] mk_tokenSet_22_()
	{
		long[] data = { -361886199258290398L, 71917956748605439L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_22_ = new BitSet(mk_tokenSet_22_());
	private static long[] mk_tokenSet_23_()
	{
		long[] data = { 512L, 1073742336L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_23_ = new BitSet(mk_tokenSet_23_());
	private static long[] mk_tokenSet_24_()
	{
		long[] data = { 2147488256L, 1073743360L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_24_ = new BitSet(mk_tokenSet_24_());
	private static long[] mk_tokenSet_25_()
	{
		long[] data = { 373799061263745024L, 16384L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_25_ = new BitSet(mk_tokenSet_25_());
	private static long[] mk_tokenSet_26_()
	{
		long[] data = { 2147488256L, 1073742336L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_26_ = new BitSet(mk_tokenSet_26_());
	private static long[] mk_tokenSet_27_()
	{
		long[] data = { 2199023256064L, 1346374224L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_27_ = new BitSet(mk_tokenSet_27_());
	private static long[] mk_tokenSet_28_()
	{
		long[] data = { -1334736316602739712L, 13647906L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_28_ = new BitSet(mk_tokenSet_28_());
	private static long[] mk_tokenSet_29_()
	{
		long[] data = { 2340200824510088192L, 34L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_29_ = new BitSet(mk_tokenSet_29_());
	private static long[] mk_tokenSet_30_()
	{
		long[] data = { -3746994735150759936L, 1064960L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_30_ = new BitSet(mk_tokenSet_30_());
	private static long[] mk_tokenSet_31_()
	{
		long[] data = { 373799061263745024L, 1064960L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_31_ = new BitSet(mk_tokenSet_31_());
	private static long[] mk_tokenSet_32_()
	{
		long[] data = { -7493919402610196224L, 69947631898522892L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_32_ = new BitSet(mk_tokenSet_32_());
	private static long[] mk_tokenSet_33_()
	{
		long[] data = { -1695024303972252672L, 1097762L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_33_ = new BitSet(mk_tokenSet_33_());
	private static long[] mk_tokenSet_34_()
	{
		long[] data = { -181707051263617280L, 71917956748277630L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_34_ = new BitSet(mk_tokenSet_34_());
	private static long[] mk_tokenSet_35_()
	{
		long[] data = { 576460752438689792L, 49152L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_35_ = new BitSet(mk_tokenSet_35_());
	private static long[] mk_tokenSet_36_()
	{
		long[] data = { 0L, 65536L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_36_ = new BitSet(mk_tokenSet_36_());
	private static long[] mk_tokenSet_37_()
	{
		long[] data = { 0L, 67176448L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_37_ = new BitSet(mk_tokenSet_37_());
	private static long[] mk_tokenSet_38_()
	{
		long[] data = { -9223372036854611968L, 16384L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_38_ = new BitSet(mk_tokenSet_38_());
	private static long[] mk_tokenSet_39_()
	{
		long[] data = { -361422755086205150L, 72057594037927935L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_39_ = new BitSet(mk_tokenSet_39_());
	private static long[] mk_tokenSet_40_()
	{
		long[] data = { -9223231299366256640L, 9454592L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_40_ = new BitSet(mk_tokenSet_40_());
	private static long[] mk_tokenSet_41_()
	{
		long[] data = { -9223372036854611968L, 1065984L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_41_ = new BitSet(mk_tokenSet_41_());
	private static long[] mk_tokenSet_42_()
	{
		long[] data = { -72529906019866880L, 71917956748081151L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_42_ = new BitSet(mk_tokenSet_42_());
	private static long[] mk_tokenSet_43_()
	{
		long[] data = { 32L, 1097728L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_43_ = new BitSet(mk_tokenSet_43_());
	private static long[] mk_tokenSet_44_()
	{
		long[] data = { -1695024303972252640L, 1097762L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_44_ = new BitSet(mk_tokenSet_44_());
	private static long[] mk_tokenSet_45_()
	{
		long[] data = { -73374330085972190L, 71917956748605439L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_45_ = new BitSet(mk_tokenSet_45_());
	private static long[] mk_tokenSet_46_()
	{
		long[] data = { -1134698147350734L, 72057594021148671L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_46_ = new BitSet(mk_tokenSet_46_());
	private static long[] mk_tokenSet_47_()
	{
		long[] data = { 0L, 327680L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_47_ = new BitSet(mk_tokenSet_47_());
	private static long[] mk_tokenSet_48_()
	{
		long[] data = { 0L, 2048L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_48_ = new BitSet(mk_tokenSet_48_());
	private static long[] mk_tokenSet_49_()
	{
		long[] data = { 0L, 4194304L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_49_ = new BitSet(mk_tokenSet_49_());
	private static long[] mk_tokenSet_50_()
	{
		long[] data = { -361886199258290912L, 71917956748080639L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_50_ = new BitSet(mk_tokenSet_50_());
	private static long[] mk_tokenSet_51_()
	{
		long[] data = { -7493919402610192128L, 69947631902717196L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_51_ = new BitSet(mk_tokenSet_51_());
	private static long[] mk_tokenSet_52_()
	{
		long[] data = { 2628431217841669120L, 32802L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_52_ = new BitSet(mk_tokenSet_52_());
	private static long[] mk_tokenSet_53_()
	{
		long[] data = { -7493919402610191616L, 69947631898654476L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_53_ = new BitSet(mk_tokenSet_53_());
	private static long[] mk_tokenSet_54_()
	{
		long[] data = { 576460752438689824L, 49152L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_54_ = new BitSet(mk_tokenSet_54_());
	private static long[] mk_tokenSet_55_()
	{
		long[] data = { -4611686018426437632L, 1065984L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_55_ = new BitSet(mk_tokenSet_55_());
	private static long[] mk_tokenSet_56_()
	{
		long[] data = { 72057594037927936L, 13683712L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_56_ = new BitSet(mk_tokenSet_56_());
	private static long[] mk_tokenSet_57_()
	{
		long[] data = { 0L, 328704L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_57_ = new BitSet(mk_tokenSet_57_());
	private static long[] mk_tokenSet_58_()
	{
		long[] data = { 864691145770271264L, 49664L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_58_ = new BitSet(mk_tokenSet_58_());
	private static long[] mk_tokenSet_59_()
	{
		long[] data = { 288230393331580960L, 32768L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_59_ = new BitSet(mk_tokenSet_59_());
	private static long[] mk_tokenSet_60_()
	{
		long[] data = { 1125899906842624L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_60_ = new BitSet(mk_tokenSet_60_());
	private static long[] mk_tokenSet_61_()
	{
		long[] data = { 950259813567168512L, 1064960L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_61_ = new BitSet(mk_tokenSet_61_());
	private static long[] mk_tokenSet_62_()
	{
		long[] data = { -7312647279929498880L, 71917956748081117L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_62_ = new BitSet(mk_tokenSet_62_());
	private static long[] mk_tokenSet_63_()
	{
		long[] data = { 32L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_63_ = new BitSet(mk_tokenSet_63_());
	private static long[] mk_tokenSet_64_()
	{
		long[] data = { 2628431217841669152L, 32802L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_64_ = new BitSet(mk_tokenSet_64_());
	private static long[] mk_tokenSet_65_()
	{
		long[] data = { -6939800365666467568L, 71917957083952524L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_65_ = new BitSet(mk_tokenSet_65_());
	private static long[] mk_tokenSet_66_()
	{
		long[] data = { -7493884218238107392L, 71917956744148236L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_66_ = new BitSet(mk_tokenSet_66_());
	private static long[] mk_tokenSet_67_()
	{
		long[] data = { -6939512839086073072L, 72057592942688140L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_67_ = new BitSet(mk_tokenSet_67_());
	private static long[] mk_tokenSet_68_()
	{
		long[] data = { -7493743480749752064L, 71917956748080396L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_68_ = new BitSet(mk_tokenSet_68_());
	private static long[] mk_tokenSet_69_()
	{
		long[] data = { -7493884218238107392L, 71917956743888140L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_69_ = new BitSet(mk_tokenSet_69_());
	private static long[] mk_tokenSet_70_()
	{
		long[] data = { -7493884218238107392L, 71917957012321548L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_70_ = new BitSet(mk_tokenSet_70_());
	private static long[] mk_tokenSet_71_()
	{
		long[] data = { -361422755086205150L, 72057594021150719L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_71_ = new BitSet(mk_tokenSet_71_());
	private static long[] mk_tokenSet_72_()
	{
		long[] data = { -361563492574560478L, 72057594021150719L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_72_ = new BitSet(mk_tokenSet_72_());
	private static long[] mk_tokenSet_73_()
	{
		long[] data = { -1134699013996750L, 72057594021150719L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_73_ = new BitSet(mk_tokenSet_73_());
	private static long[] mk_tokenSet_74_()
	{
		long[] data = { -361886199254096094L, 71917958090784767L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_74_ = new BitSet(mk_tokenSet_74_());
	private static long[] mk_tokenSet_75_()
	{
		long[] data = { -8798240506062L, 72057594037927935L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_75_ = new BitSet(mk_tokenSet_75_());
	private static long[] mk_tokenSet_76_()
	{
		long[] data = { -7312649483248770816L, 71917956748080524L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_76_ = new BitSet(mk_tokenSet_76_());
	private static long[] mk_tokenSet_77_()
	{
		long[] data = { -6939800365666467568L, 71917957084083596L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_77_ = new BitSet(mk_tokenSet_77_());
	private static long[] mk_tokenSet_78_()
	{
		long[] data = { -7457890571231469312L, 71917956748080524L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_78_ = new BitSet(mk_tokenSet_78_());
	private static long[] mk_tokenSet_79_()
	{
		long[] data = { -6939792119334497520L, 72057592942688220L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_79_ = new BitSet(mk_tokenSet_79_());
	private static long[] mk_tokenSet_80_()
	{
		long[] data = { -361886199258290910L, 71917956748604927L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_80_ = new BitSet(mk_tokenSet_80_());
	private static long[] mk_tokenSet_81_()
	{
		long[] data = { -1406793910640671744L, 1064994L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_81_ = new BitSet(mk_tokenSet_81_());
	private static long[] mk_tokenSet_82_()
	{
		long[] data = { 0L, 67438592L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_82_ = new BitSet(mk_tokenSet_82_());
	private static long[] mk_tokenSet_83_()
	{
		long[] data = { 0L, 264192L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_83_ = new BitSet(mk_tokenSet_83_());
	private static long[] mk_tokenSet_84_()
	{
		long[] data = { -7493884218238106880L, 71917956743886604L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_84_ = new BitSet(mk_tokenSet_84_());
	private static long[] mk_tokenSet_85_()
	{
		long[] data = { -6939800365671710448L, 71917957079889292L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_85_ = new BitSet(mk_tokenSet_85_());
	private static long[] mk_tokenSet_86_()
	{
		long[] data = { -6939794318357753072L, 72057592942688140L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_86_ = new BitSet(mk_tokenSet_86_());
	private static long[] mk_tokenSet_87_()
	{
		long[] data = { 2199023256064L, 1342177872L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_87_ = new BitSet(mk_tokenSet_87_());
	private static long[] mk_tokenSet_88_()
	{
		long[] data = { -6939792119329254640L, 72057592946882524L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_88_ = new BitSet(mk_tokenSet_88_());
	private static long[] mk_tokenSet_89_()
	{
		long[] data = { -1134699018191566L, 72057592947406335L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_89_ = new BitSet(mk_tokenSet_89_());
	private static long[] mk_tokenSet_90_()
	{
		long[] data = { -6939510640057574640L, 72057592946882524L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_90_ = new BitSet(mk_tokenSet_90_());
	private static long[] mk_tokenSet_91_()
	{
		long[] data = { 4398046511104L, 393216L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_91_ = new BitSet(mk_tokenSet_91_());
	private static long[] mk_tokenSet_92_()
	{
		long[] data = { -7493884218232864512L, 71917956748342540L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_92_ = new BitSet(mk_tokenSet_92_());
	private static long[] mk_tokenSet_93_()
	{
		long[] data = { -8799111348430L, 72057592947406847L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_93_ = new BitSet(mk_tokenSet_93_());
	private static long[] mk_tokenSet_94_()
	{
		long[] data = { -6938386904814224624L, 72057592946882444L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_94_ = new BitSet(mk_tokenSet_94_());
	private static long[] mk_tokenSet_95_()
	{
		long[] data = { 512L, 1342177792L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_95_ = new BitSet(mk_tokenSet_95_());
	private static long[] mk_tokenSet_96_()
	{
		long[] data = { -361886199237318878L, 71917958091112447L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_96_ = new BitSet(mk_tokenSet_96_());
	private static long[] mk_tokenSet_97_()
	{
		long[] data = { -7493882019214851328L, 71917958086063964L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_97_ = new BitSet(mk_tokenSet_97_());
	private static long[] mk_tokenSet_98_()
	{
		long[] data = { -6939510640062817520L, 72057594016430044L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_98_ = new BitSet(mk_tokenSet_98_());
	private static long[] mk_tokenSet_99_()
	{
		long[] data = { 72057594037927936L, 76595200L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_99_ = new BitSet(mk_tokenSet_99_());
	private static long[] mk_tokenSet_100_()
	{
		long[] data = { -7313740233143283456L, 71917956811322766L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_100_ = new BitSet(mk_tokenSet_100_());
	private static long[] mk_tokenSet_101_()
	{
		long[] data = { -7313740233143287552L, 71917956744148364L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_101_ = new BitSet(mk_tokenSet_101_());
	private static long[] mk_tokenSet_102_()
	{
		long[] data = { -6939510640062817520L, 72057592942688220L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_102_ = new BitSet(mk_tokenSet_102_());
	private static long[] mk_tokenSet_103_()
	{
		long[] data = { -361886198419430110L, 71917956748604927L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_103_ = new BitSet(mk_tokenSet_103_());
	private static long[] mk_tokenSet_104_()
	{
		long[] data = { -361604724260606174L, 71917958091112447L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_104_ = new BitSet(mk_tokenSet_104_());
	private static long[] mk_tokenSet_105_()
	{
		long[] data = { 4398046511104L, 131072L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_105_ = new BitSet(mk_tokenSet_105_());
	private static long[] mk_tokenSet_106_()
	{
		long[] data = { -361604724260608222L, 71917958091112447L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_106_ = new BitSet(mk_tokenSet_106_());
	private static long[] mk_tokenSet_107_()
	{
		long[] data = { -361877334580009184L, 71917956748605439L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_107_ = new BitSet(mk_tokenSet_107_());
	private static long[] mk_tokenSet_108_()
	{
		long[] data = { -1407377895065808L, 72057594021148671L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_108_ = new BitSet(mk_tokenSet_108_());
	private static long[] mk_tokenSet_109_()
	{
		long[] data = { 6951886980579099648L, 1064994L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_109_ = new BitSet(mk_tokenSet_109_());
	private static long[] mk_tokenSet_110_()
	{
		long[] data = { -6939512839086073584L, 72057594020623756L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_110_ = new BitSet(mk_tokenSet_110_());
	private static long[] mk_tokenSet_111_()
	{
		long[] data = { 0L, 1073741824L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_111_ = new BitSet(mk_tokenSet_111_());
	private static long[] mk_tokenSet_112_()
	{
		long[] data = { 41231686041600L, 481036337152L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_112_ = new BitSet(mk_tokenSet_112_());
	private static long[] mk_tokenSet_113_()
	{
		long[] data = { -7493919402610196224L, 71917956743886092L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_113_ = new BitSet(mk_tokenSet_113_());
	private static long[] mk_tokenSet_114_()
	{
		long[] data = { -361604724260606174L, 71918024663236607L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_114_ = new BitSet(mk_tokenSet_114_());
	private static long[] mk_tokenSet_115_()
	{
		long[] data = { 0L, 3848357806080L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_115_ = new BitSet(mk_tokenSet_115_());
	private static long[] mk_tokenSet_116_()
	{
		long[] data = { -361563492574564574L, 71918505699573759L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_116_ = new BitSet(mk_tokenSet_116_());
	private static long[] mk_tokenSet_117_()
	{
		long[] data = { 0L, 30786333966336L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_117_ = new BitSet(mk_tokenSet_117_());
	private static long[] mk_tokenSet_118_()
	{
		long[] data = { -361563492574564574L, 71921254545752063L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_118_ = new BitSet(mk_tokenSet_118_());
	private static long[] mk_tokenSet_119_()
	{
		long[] data = { -361563492574564574L, 71952040871329791L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_119_ = new BitSet(mk_tokenSet_119_());
	private static long[] mk_tokenSet_120_()
	{
		long[] data = { -361563492574564574L, 72057594021150719L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_120_ = new BitSet(mk_tokenSet_120_());
	private static long[] mk_tokenSet_121_()
	{
		long[] data = { 0L, 1971424356990976L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_121_ = new BitSet(mk_tokenSet_121_());
	private static long[] mk_tokenSet_122_()
	{
		long[] data = { -1134698997219534L, 72057594021150719L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_122_ = new BitSet(mk_tokenSet_122_());
	private static long[] mk_tokenSet_123_()
	{
		long[] data = { -7241682639105359616L, 71917956811257228L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_123_ = new BitSet(mk_tokenSet_123_());
	private static long[] mk_tokenSet_124_()
	{
		long[] data = { -6939512839086073584L, 72057592946881932L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_124_ = new BitSet(mk_tokenSet_124_());
	private static long[] mk_tokenSet_125_()
	{
		long[] data = { -6939512839086073584L, 72057592942689676L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_125_ = new BitSet(mk_tokenSet_125_());
	private static long[] mk_tokenSet_126_()
	{
		long[] data = { 0L, 268500992L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_126_ = new BitSet(mk_tokenSet_126_());
	private static long[] mk_tokenSet_127_()
	{
		long[] data = { 0L, 268699648L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_127_ = new BitSet(mk_tokenSet_127_());
	
}
}
