// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of the Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

options
{
	language = "CSharp";
	namespace = "Boo.AntlrParser";
}

{
using Boo.Lang.Compiler.Ast;
using Boo.AntlrParser.Util;

public delegate void ParserErrorHandler(antlr.RecognitionException x);
}

class BooParserBase extends Parser;
options
{
	k = 2;
	exportVocab = Boo; 
	defaultErrorHandler = true;
}
tokens
{
	TIMESPAN; // timespan literal
	DOUBLE; // real literal
	LONG; // long literal
	ESEPARATOR; // expression separator (imaginary token)	
	INDENT;
	DEDENT;
	COMPILATION_UNIT;
	PARAMETERS;
	PARAMETER;
	ELIST; // expression list
	DLIST; // declaration list
	TYPE;
	CALL;
	STMT;
	BLOCK;
	FIELD;
	MODIFIERS;
	MODULE;
	LITERAL;
	LIST_LITERAL;
	UNPACKING;
	ABSTRACT="abstract";
	AND="and";
	AS="as";		
	BREAK="break";
	CONTINUE="continue";
	CALLABLE="callable";
	CAST="cast";	
	CLASS="class";
	CONSTRUCTOR="constructor";	
	DEF="def";
	DO="do";	
	ELSE="else";
	ENSURE="ensure";
	ENUM="enum";
	EVENT="event";
	EXCEPT="except";
	FAILURE="failure";
	FINAL="final";	
	FROM="from";
	FOR="for";
	FALSE="false";
	GET="get";
	GIVEN="given";
	IMPORT="import";
	INTERFACE="interface";	
	INTERNAL="internal";
	IS="is";	
	ISA="isa";	
	IF="if";	
	IN="in";	
	NOT="not";	
	NULL="null";
	OR="or";
	OTHERWISE="otherwise";
	OVERRIDE="override";	
	PASS="pass";
	NAMESPACE="namespace";
	PUBLIC="public";
	PROTECTED="protected";
	PRIVATE="private";
	RAISE="raise";
	RETURN="return";
	RETRY="retry";
	SET="set";	
	SELF="self";
	SUPER="super";
	STATIC="static";
	SUCCESS="success";
	TRY="try";
	TRANSIENT="transient";
	TRUE="true";
	TYPEOF="typeof";
	UNLESS="unless";
	VIRTUAL="virtual";
	WHEN="when";
	WHILE="while";
	YIELD="yield";
}

{		
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

	protected LexicalInfo ToLexicalInfo(antlr.Token token)
	{
		int line = token.getLine();
		int startColumn = token.getColumn();
		int endColumn = token.getColumn() + token.getText().Length;
		string filename = token.getFilename();
		return new LexicalInfo(filename, line, startColumn, endColumn);
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
			case "+=": return BinaryOperatorType.InPlaceAdd;
			case "-=": return BinaryOperatorType.InPlaceSubtract;
			case "/=": return BinaryOperatorType.InPlaceDivide;
			case "*=": return BinaryOperatorType.InPlaceMultiply;
		}
		throw new ArgumentException(op, "op");
	}

	static double ParseDouble(string text)
	{
		return double.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
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
}

protected
start returns [Module module]
	{
		module = new Module();		
		module.LexicalInfo = new LexicalInfo(getFilename(), 0, 0, 0);
	}:
	(options { greedy=true;}: EOS!)*
	docstring[module]
	(options { greedy=true;}: EOS!)*			 
	(namespace_directive[module])?
	(import_directive[module])*
	(type_member[module.Members])*	
	globals[module]
	EOF!
	;
			
protected docstring[Node node]:
	(
		doc:TRIPLE_QUOTED_STRING { node.Documentation = MassageDocString(doc.getText()); }
		(options { greedy=true; }: EOS!)*
	)?
	;
			
protected
eos : (options { greedy = true; }: EOS!)+;

protected
import_directive[Module container]
	{
		Token id;
		Import usingNode = null;
	}: 
	t:IMPORT! id=identifier
	{
		usingNode = new Import(ToLexicalInfo(t));
		usingNode.Namespace = id.getText();
		container.Imports.Add(usingNode);
	}
	(
		FROM!
			(
					id=identifier |
					dqs:DOUBLE_QUOTED_STRING { id=dqs; } |
					sqs:SINGLE_QUOTED_STRING { id=sqs; }
			)
		{
			usingNode.AssemblyReference = new ReferenceExpression(ToLexicalInfo(id));
			usingNode.AssemblyReference.Name = id.getText();
		}				
	)?
	(
		AS! alias:ID
		{
			usingNode.Alias = new ReferenceExpression(ToLexicalInfo(alias));
			usingNode.Alias.Name = alias.getText();
		}
	)?
	eos
	;

protected
namespace_directive[Module container]
	{
		Token id;
		NamespaceDeclaration p = null;
	}:
	t:NAMESPACE! id=identifier
	{
		p = new NamespaceDeclaration(ToLexicalInfo(t));
		p.Name = id.getText();
		container.Namespace = p; 
	}
	eos
	docstring[p]
	;
			
protected
type_member[TypeMemberCollection container]:
	attributes
	modifiers
	(
		type_definition[container] |
		method[container]
	)
	;

protected
type_definition [TypeMemberCollection container]:
	(
		class_definition[container] |
		interface_definition[container] |
		enum_definition[container] |
		callable_definition[container]
	)			
	;
	
protected
callable_definition [TypeMemberCollection container]
	{
		CallableDefinition cd = null;
		TypeReference returnType = null;
		bool variableArguments = false;
	}:
	CALLABLE! id:ID
	{
		cd = new CallableDefinition(ToLexicalInfo(id));
		cd.Name = id.getText();
		cd.Modifiers = _modifiers;
		AddAttributes(cd.Attributes);
		container.Add(cd);
	}
	LPAREN! variableArguments=parameter_declaration_list[cd.Parameters] RPAREN!
	(AS! returnType=type_reference { cd.ReturnType=returnType; })?			
	eos
	docstring[cd]
	{
		cd.VariableArguments = variableArguments;
	}
	;

protected
enum_definition [TypeMemberCollection container]
	{
		EnumDefinition ed = null;
	}:
	ENUM! id:ID
	begin!
	{
		ed = new EnumDefinition(ToLexicalInfo(id));
		ed.Name = id.getText();
		ed.Modifiers = _modifiers;
		AddAttributes(ed.Attributes);
		container.Add(ed);
	}
	(
		(enum_member[ed])+
	)
	end!
	;
	
protected
enum_member [EnumDefinition container]
	{		
		IntegerLiteralExpression initializer = null;		
	}: 
	attributes
	id:ID (ASSIGN! initializer=integer_literal)?
	{
		EnumMember em = new EnumMember(ToLexicalInfo(id));
		em.Name = id.getText();
		em.Initializer = initializer;
		AddAttributes(em.Attributes);
		container.Members.Add(em);
	}
	eos
	;
			
protected
attributes
	{
		_attributes.Clear();
	}:
	(
		LBRACK!
		(
			attribute
			(
				COMMA!
				attribute
			)*
		)?
		RBRACK!		
		(EOS!)*
	)*
	;
			
protected
attribute
	{		
		antlr.Token id = null;
		Boo.Lang.Compiler.Ast.Attribute attr = null;
	}
	:	
	id=identifier
	{
		attr = new Boo.Lang.Compiler.Ast.Attribute(ToLexicalInfo(id));
		attr.Name = id.getText();
		_attributes.Add(attr);
	} 
	(
		LPAREN!
		parameter_list[attr]
		RPAREN!
	)?
	;
			
protected
class_definition [TypeMemberCollection container]
	{
		ClassDefinition cd = null;
	}:
	c:CLASS! id:ID
	{
		cd = new ClassDefinition(ToLexicalInfo(c));
		cd.Name = id.getText();
		cd.Modifiers = _modifiers;
		AddAttributes(cd.Attributes);
		container.Add(cd);
	}
	(base_types[cd.BaseTypes])?
	begin_with_doc[cd]					
	(
		(PASS! eos) |
		(
			(EOS!)*
			attributes
			modifiers
			(						
				method[cd.Members] |
				event_declaration[cd.Members] |
				field_or_property[cd.Members] |
				type_definition[cd.Members]
			)
		)+
	)
	end
	;
			
protected
interface_definition [TypeMemberCollection container]
	{
		InterfaceDefinition itf = null;
	} :
	it:INTERFACE! id:ID
	{
		itf = new InterfaceDefinition(ToLexicalInfo(it));
		itf.Name = id.getText();
		itf.Modifiers = _modifiers;
		AddAttributes(itf.Attributes);
		container.Add(itf);
	}
	(base_types[itf.BaseTypes])?
	begin_with_doc[itf]
	(
		(PASS! eos) |
		(
			attributes
			(
				interface_method[itf.Members] |
				interface_property[itf.Members]
			)
		)+
	)
	end!
	;
			
protected
base_types[TypeReferenceCollection container]
	{
		TypeReference tr = null;
	}:
	LPAREN! tr=type_reference { container.Add(tr); }
	(COMMA! tr=type_reference { container.Add(tr); })*
	RPAREN!
	;
			
protected
interface_method [TypeMemberCollection container]
	{
		Method m = null;
		TypeReference rt = null;
		bool variableArguments = false;
	}: 
	t:DEF! id:ID
	{
		m = new Method(ToLexicalInfo(t));
		m.Name = id.getText();
		AddAttributes(m.Attributes);
		container.Add(m);
	}
	LPAREN! variableArguments=parameter_declaration_list[m.Parameters] RPAREN!
	(AS! rt=type_reference { m.ReturnType=rt; })?			
	(
		(eos docstring[m]) | (empty_block (EOS!)*)
	)
	{
		m.VariableArguments = variableArguments;
	}
	;
			
protected
interface_property [TypeMemberCollection container]
	{
		Property p = null;
		TypeReference tr = null;
	}:
	id:ID (AS! tr=type_reference)?
	{
		p = new Property(ToLexicalInfo(id));
		p.Name = id.getText();
		p.Type = tr;
		AddAttributes(p.Attributes);
		container.Add(p);
	}
	begin_with_doc[p]!
		(interface_property_accessor[p])+
	end
	;
			
protected
interface_property_accessor[Property p]
	{
		Method m = null;
	}
	:
	attributes
	(
		{ null == p.Getter }?
		(
			gt:GET! { m = p.Getter = new Method(ToLexicalInfo(gt)); m.Name = "get"; }
		)
		|
		{ null == p.Setter }?
		(
			st:SET! { m = p.Setter = new Method(ToLexicalInfo(st)); m.Name = "set"; }
		)				
	)
	(
		eos | empty_block
	)
	{
		AddAttributes(m.Attributes);
	}
	;
			
protected
empty_block: 
		begin!
			PASS! eos
		end!
		;
		
protected
event_declaration [TypeMemberCollection container]
	{
		Event e = null;
		TypeReference tr = null;
	}:
	t:EVENT!
	id:ID AS! tr=type_reference eos
	{
		e = new Event(ToLexicalInfo(t), id.getText(), tr);
		e.Modifiers = _modifiers;
		AddAttributes(e.Attributes);
		container.Add(e);
	}
	;

protected
method [TypeMemberCollection container]
	{
		Method m = null;
		TypeReference rt = null;
		bool variableArguments = false;
	}: 
	t:DEF!
	(
		id:ID { m = new Method(ToLexicalInfo(t)); m.Name = id.getText(); } |
		c:CONSTRUCTOR! { m = new Constructor(ToLexicalInfo(t)); }
	)	
	{
		m.Modifiers = _modifiers;
		AddAttributes(m.Attributes);
	}
	LPAREN! variableArguments=parameter_declaration_list[m.Parameters] RPAREN!
			(AS! rt=type_reference { m.ReturnType = rt; })?
			attributes { AddAttributes(m.ReturnTypeAttributes); }
			begin_with_doc[m]
				block[m.Body.Statements]
			end
	{
		m.VariableArguments = variableArguments; 
		container.Add(m);
	}
	;	
	
protected
property_header:	
	LPAREN! |
	((AS! type_reference)? COLON!)
	;
	
protected
field_or_property [TypeMemberCollection container]
	{
		TypeMember tm = null;
		TypeReference tr = null;
		Property p = null;
		Expression initializer = null;
	}: 
	id:ID
	(		
		(property_header)=>(
			{ p = new Property(ToLexicalInfo(id)); }			
			(LPAREN! parameter_declaration_list[p.Parameters] RPAREN!)?
			(AS! tr=type_reference)?
			{							
				p.Type = tr;
				tm = p;
				tm.Name = id.getText();
				tm.Modifiers = _modifiers;
				AddAttributes(tm.Attributes);
			}		
			begin_with_doc[p]
				(property_accessor[p])+
			end
		)
		|
		(
			(AS! tr=type_reference)? (ASSIGN! initializer=expression)?
			eos
			{
				Field field = new Field(ToLexicalInfo(id));
				field.Type = tr;
				field.Initializer = initializer;
				tm = field;
				tm.Name = id.getText();
				tm.Modifiers = _modifiers;
				AddAttributes(tm.Attributes);
			}
			docstring[tm]
		)
	)
	{ container.Add(tm); }
	;
			
protected
property_accessor[Property p]
	{		
		Method m = null;
	}:
	attributes
	modifiers
	(
		{ null == p.Getter }?
		(
			gt:GET!
			{
				p.Getter = m = new Method(ToLexicalInfo(gt));		
				m.Name = "get";
			}
		)
		|
		{ null == p.Setter }?
		(
			st:SET!
			{
				p.Setter = m = new Method(ToLexicalInfo(st));
				m.Name = "set";
			}
		)
	)
	{
		AddAttributes(m.Attributes);
		m.Modifiers = _modifiers;
	}
	compound_stmt[m.Body.Statements]
	;
	
protected
globals[Module container]:	
	(EOS!)*
	(stmt[container.Globals.Statements])*
	;
	
protected
block[StatementCollection container]:
	(EOS!)* 
	(
		(PASS! eos) |
		(			
			stmt[container]
		)+
	)
	; 
	
protected
modifiers
	{
		_modifiers = TypeMemberModifiers.None;
	}:
	(
		STATIC! { _modifiers |= TypeMemberModifiers.Static; } |
		PUBLIC! { _modifiers |= TypeMemberModifiers.Public; } |
		PROTECTED! { _modifiers |= TypeMemberModifiers.Protected; } |
		PRIVATE! { _modifiers |= TypeMemberModifiers.Private; } |
		INTERNAL! { _modifiers |= TypeMemberModifiers.Internal; } |			
		FINAL! { _modifiers |= TypeMemberModifiers.Final; } |
		TRANSIENT! { _modifiers |= TypeMemberModifiers.Transient; } |
		OVERRIDE! { _modifiers |= TypeMemberModifiers.Override; } |
		ABSTRACT! { _modifiers |= TypeMemberModifiers.Abstract; } |
		VIRTUAL! { _modifiers |= TypeMemberModifiers.Virtual; }
	)*
	;
	
protected	
parameter_declaration_list[ParameterDeclarationCollection c]
	returns [bool variableArguments]
	{
		variableArguments = false;
	}: 
	(variableArguments=parameter_declaration[c]
	( {!variableArguments}?(COMMA! variableArguments=parameter_declaration[c]) )* )?
	;

protected
parameter_declaration[ParameterDeclarationCollection c]
	returns [bool variableArguments]
	{		
		TypeReference tr = null;
		variableArguments = false;
	}: 
	attributes
	(MULTIPLY { variableArguments=true; })?
	id:ID (AS! tr=type_reference)? 
	{
		ParameterDeclaration pd = new ParameterDeclaration(ToLexicalInfo(id));
		pd.Name = id.getText();
		pd.Type = tr;
		AddAttributes(pd.Attributes);
		c.Add(pd);
	} 
	;

protected
type_reference returns [TypeReference tr]
	{
		tr=null;
		Token id = null;
	}: 
	(
		lparen:LPAREN!
		tr=type_reference
		rparen:RPAREN!
		{
			ArrayTypeReference ttr = new ArrayTypeReference(ToLexicalInfo(lparen));
			ttr.ElementType = tr;
			tr = ttr;
		}
	)
	|
	(
		(id=identifier | c:CALLABLE! { id=c; })
		{
			SimpleTypeReference str = new SimpleTypeReference(ToLexicalInfo(id));
			str.Name = id.getText();
			tr = str;
		}
	)
	;

protected
begin : COLON INDENT;

protected
begin_with_doc[Node node]: COLON! (EOS! docstring[node])? INDENT!;

protected
end : DEDENT! (options { greedy=true; }: EOS!)*;

protected
compound_stmt[StatementCollection c] :
		begin
			block[c]
		end
		;
		
protected
macro_stmt returns [MacroStatement returnValue]
	{
		returnValue = null;
		MacroStatement macro = new MacroStatement();
	}:
	id:ID expression_list[macro.Arguments]
	(
		compound_stmt[macro.Block.Statements] |
		eos
	)
	{
		macro.Name = id.getText();
		macro.LexicalInfo = ToLexicalInfo(id);
		
		returnValue = macro;
	}
;

protected
stmt [StatementCollection container]
	{
		Statement s = null;
		StatementModifier m = null;
	}:		
	(
		s=for_stmt |
		s=while_stmt |
		s=if_stmt |
		s=unless_stmt |
		s=try_stmt |
		s=given_stmt |
		{IsValidMacroArgument(LA(2))}? s=macro_stmt |
		(slicing_expression (ASSIGN|DO))=> s=assignment_or_method_invocation_with_block_stmt |
		(RETURN DO) => s=return_callable_stmt |
		(		
			(				
				s=return_stmt |
				s=yield_stmt |
				s=break_stmt |
				s=continue_stmt |				
				s=raise_stmt |
				s=retry_stmt |
				(declaration COMMA)=> s=unpack_stmt |
				s=declaration_stmt |				
				s=expression_stmt				
			)
			(			
				m=stmt_modifier { s.Modifier = m; }
			)?
			eos
		)
	)
	{
		if (null != s)
		{
			container.Add(s);
		}
	}
	;		

protected
stmt_modifier returns [StatementModifier m]
	{
		m = null;
		Expression e = null;
		Token t = null;
		StatementModifierType type = StatementModifierType.Uninitialized;
	}:
	(
		i:IF! { t = i; type = StatementModifierType.If; } |
		u:UNLESS! { t = u; type = StatementModifierType.Unless; } |
		w:WHILE! { t = w; type = StatementModifierType.While; }
	)
	e=expression
	{
		m = new StatementModifier(ToLexicalInfo(t));
		m.Type = type;
		m.Condition = e;
	}
	;
	
protected
callable_or_expression returns [Expression e]
	{
		e = null;
	}:
	e=callable_expression|
	e=array_or_expression
	;
	

protected
closure_parameters_test:
	(ID! (AS! type_reference)?)+
	BITWISE_OR!
	;
	
protected
closure_expression returns [Expression e]
	{
		e = null;
		CallableBlockExpression cbe = null;
		ParameterDeclarationCollection parameters = null;
		Statement stmt = null;		
	}:
	anchorBegin:LESS_THAN!
		{
			e = cbe = new CallableBlockExpression(ToLexicalInfo(anchorBegin));
			parameters = cbe.Parameters;
		}
		
		(
			(closure_parameters_test)=>(
				parameter_declaration_list[parameters]
				BITWISE_OR!
			) |
		)
		(
			(
				stmt=return_stmt |
				stmt=expression_stmt
			)
			{ cbe.Body.Add(stmt); }
		)
	anchorEnd:GREATER_THAN!
	;
	
protected
callable_expression returns [Expression e]
	{
		e = null;
		CallableBlockExpression cbe = null;
		TypeReference rt = null;
	}:
	anchor:DO!
	{
		e = cbe = new CallableBlockExpression(ToLexicalInfo(anchor));
	}
	(
		LPAREN! parameter_declaration_list[cbe.Parameters] RPAREN!
		(AS! rt=type_reference { cbe.ReturnType = rt; })?
	)?
		compound_stmt[cbe.Body.Statements]
	;
	
	
protected
retry_stmt returns [RetryStatement rs]
	{
		rs = null;
	}:
	t:RETRY! { rs = new RetryStatement(ToLexicalInfo(t)); }
	;
	
protected
try_stmt returns [TryStatement s]
	{
		s = null;		
		Block sblock = null;
		Block eblock = null;
	}:
	t:TRY! { s = new TryStatement(ToLexicalInfo(t)); }
		compound_stmt[s.ProtectedBlock.Statements]
	(
		exception_handler[s]
	)*
	(
		stoken:SUCCESS! { sblock = new Block(ToLexicalInfo(stoken)); }
			compound_stmt[sblock.Statements]
		{ s.SuccessBlock = sblock; }
	)?
	(
		etoken:ENSURE! { eblock = new Block(ToLexicalInfo(etoken)); }
			compound_stmt[eblock.Statements]
		{ s.EnsureBlock = eblock; }
	)?
	;
	
protected
exception_handler [TryStatement t]
	{
		ExceptionHandler eh = null;		
		TypeReference tr = null;
	}:
	c:EXCEPT! (x:ID (AS tr=type_reference)?)?
	{
		eh = new ExceptionHandler(ToLexicalInfo(c));
		
		if (x != null)
		{
			eh.Declaration = new Declaration(ToLexicalInfo(x));
			eh.Declaration.Name = x.getText();		
			eh.Declaration.Type = tr;
		}
	}		
	compound_stmt[eh.Block.Statements]
	{
		t.ExceptionHandlers.Add(eh);
	}
	;
	
protected
raise_stmt returns [RaiseStatement s]
	{
		s = null;
		Expression e = null;
	}:
	t:RAISE! (e=expression)?
	{
		s = new RaiseStatement(ToLexicalInfo(t));
		s.Exception = e;
	}
	;
	
protected
declaration_stmt returns [DeclarationStatement s]
	{
		s = null;
		TypeReference tr = null;
		Expression initializer = null;
	}:
	id:ID AS! tr=type_reference (ASSIGN! initializer=array_or_expression)?
	{
		Declaration d = new Declaration(ToLexicalInfo(id));
		d.Name = id.getText();
		d.Type = tr;
		
		s = new DeclarationStatement(d.LexicalInfo);
		s.Declaration = d;
		s.Initializer = initializer;
	}
	;

protected
expression_stmt returns [ExpressionStatement s]
	{
		s = null;
		Expression e = null;
	}:
	//e=expression
	e=assignment_expression
	{
		s = new ExpressionStatement(e);
	}
	;	

protected
return_stmt returns [ReturnStatement s]
	{
		s = null;
		Expression e = null;
	}:
	r:RETURN! (e=array_or_expression)?
	{
		s = new ReturnStatement(ToLexicalInfo(r));
		s.Expression = e;
	}
	;
	
protected
return_callable_stmt returns [ReturnStatement s]
	{
		s = null;
		Expression e = null;
	}:
	r:RETURN! e=callable_expression
	{
		s = new ReturnStatement(ToLexicalInfo(r));
		s.Expression = e;
	}
	;

protected
yield_stmt returns [YieldStatement s]
	{
		s = null;
		Expression e = null;
	}:
	yt:YIELD! e=array_or_expression
	{
		s = new YieldStatement(ToLexicalInfo(yt));
		s.Expression = e;
	}
	;

protected
break_stmt returns [BreakStatement s]
	{ s = null; }:
	b:BREAK!
	{ s = new BreakStatement(ToLexicalInfo(b)); }
	;

protected
continue_stmt returns [Statement s]
	{ s = null; }:
	c:CONTINUE!
	{ s = new ContinueStatement(ToLexicalInfo(c)); }
	;
	
protected
unless_stmt returns [UnlessStatement us]
	{
		us = null;
		Expression condition = null;
	}:
	u:UNLESS! condition=expression
	{
		us = new UnlessStatement(ToLexicalInfo(u));
		us.Condition = condition;
	}
	compound_stmt[us.Block.Statements]
	;

protected
for_stmt returns [ForStatement fs]
	{
		fs = null;
		Expression iterator = null;
	}:
	f:FOR! { fs = new ForStatement(ToLexicalInfo(f)); }
		declaration_list[fs.Declarations] IN! iterator=array_or_expression
		{ fs.Iterator = iterator; }
		compound_stmt[fs.Block.Statements]
	;
		
protected
while_stmt returns [WhileStatement ws]
	{
		ws = null;
		Expression e = null;
	}:
	w:WHILE! e=expression
	{
		ws = new WhileStatement(ToLexicalInfo(w));
		ws.Condition = e;
	}
	compound_stmt[ws.Block.Statements]
	;
		
protected
given_stmt returns [GivenStatement gs]
	{
		gs = null;		
		Expression e = null;
		WhenClause wc = null;
	}:
	given:GIVEN! e=expression
	{
		gs = new GivenStatement(ToLexicalInfo(given));
		gs.Expression = e;
	}
	begin!
		(
			when:WHEN! e=array_or_expression
			{
				wc = new WhenClause(ToLexicalInfo(when));
				wc.Condition = e;
				gs.WhenClauses.Add(wc);
			}				
				compound_stmt[wc.Block.Statements]
		)+
		(
			otherwise:OTHERWISE!
			{
				gs.OtherwiseBlock = new Block(ToLexicalInfo(otherwise));
			}
			compound_stmt[gs.OtherwiseBlock.Statements]
		)?
	end!
	;
		
protected
if_stmt returns [IfStatement s]
	{
		s = null;
		Expression e = null;
	}:
	it:IF! e=expression
	{
		s = new IfStatement(ToLexicalInfo(it));
		s.Condition = e;
		s.TrueBlock = new Block();
	}
	compound_stmt[s.TrueBlock.Statements]
	(
		et:ELSE! { s.FalseBlock = new Block(ToLexicalInfo(et)); }
		compound_stmt[s.FalseBlock.Statements]
	)?
	;
		
protected
unpack_stmt returns [UnpackStatement s]
	{
		s = new UnpackStatement();
		Expression e = null;
	}:
	declaration_list[s.Declarations] t:ASSIGN! e=array_or_expression
	{
		s.Expression = e;
		s.LexicalInfo = ToLexicalInfo(t);
	}
	;		
		
protected
declaration_list[DeclarationCollection dc]
	{
		Declaration d = null;
	}:
	d=declaration { dc.Add(d); }
	(COMMA! d=declaration { dc.Add(d); })*
	;
		
protected
declaration returns [Declaration d]
	{
		d = null;
		TypeReference tr = null;
	}:
	id:ID (AS! tr=type_reference)?
	{
		d = new Declaration(ToLexicalInfo(id));
		d.Name = id.getText();
		d.Type = tr;
	}
	;
	
protected
array_or_expression returns [Expression e]
	{
		e = null;
		ArrayLiteralExpression tle = null;
	} :
		(
			// tupla vazia: , ou (,)
			c:COMMA! { e = new ArrayLiteralExpression(ToLexicalInfo(c)); }
		) |
		(
			e=expression
			( options { greedy=true; }:
				t:COMMA!
				{					
					tle = new ArrayLiteralExpression(e.LexicalInfo);
					tle.Items.Add(e);		
				}
				( options { greedy=true; }:
					e=expression { tle.Items.Add(e); }
					( options { greedy=true; }:
						COMMA!
						e=expression { tle.Items.Add(e); }
					)*
				)?
				{
					e = tle;
				}
			)?
		)
	;
			
protected
expression returns [Expression e]
	{
		e = null;
		TypeReference tr = null;
		
		GeneratorExpression lde = null;
		StatementModifier filter = null;
		Expression iterator = null;
		DeclarationCollection declarations = null;
	} :
	e=boolean_expression
	(
		t:AS!
		tr=type_reference
		{
			AsExpression ae = new AsExpression(ToLexicalInfo(t));
			ae.Target = e;
			ae.Type = tr;
			e = ae; 
		}
	)?
		
	( options { greedy = true; } :
		f:FOR!
		{
			lde = new GeneratorExpression(ToLexicalInfo(f));
			lde.Expression = e;
			
			declarations = lde.Declarations;
		}
		declaration_list[declarations]
		IN!
		iterator=expression { lde.Iterator = iterator; }
		(
			filter=stmt_modifier { lde.Filter = filter; }
		)?
		{ e = lde; }
	)?
	;
	
protected
boolean_expression returns [Expression e]
	{
		e = null;
		Expression r = null;
	}
	:
	(
		nt:NOT!
		e=boolean_expression
		{
			UnaryExpression ue = new UnaryExpression(ToLexicalInfo(nt));
			ue.Operator = UnaryOperatorType.LogicalNot;
			ue.Operand = e;
			e = ue;
		}
	)
	|
	(
		e=boolean_term
		(
			ot:OR!
			r=expression
			{
				BinaryExpression be = new BinaryExpression(ToLexicalInfo(ot));
				be.Operator = BinaryOperatorType.Or;
				be.Left = e;
				be.Right = r;
				e = be;
			}
		)*
	)
	;

protected
boolean_term returns [Expression e]
	{
		e = null;
		Expression r = null;
	}
	:
	e=assignment_expression
	(
		at:AND!
		r=expression
		{
			BinaryExpression be = new BinaryExpression(ToLexicalInfo(at));
			be.Operator = BinaryOperatorType.And;
			be.Left = e;
			be.Right = r; 
			e = be;
		}
	)*
	;

protected
assignment_or_method_invocation_with_block_stmt returns [Statement stmt]
	{
		stmt = null;
		Expression lhs = null;
		Expression rhs = null;
		StatementModifier modifier = null;		
	}:
	lhs=slicing_expression
	(
		(
			op:ASSIGN
			(
				(DO)=>rhs=callable_expression |
				(
					rhs=array_or_expression
					(			
						modifier=stmt_modifier
					)?
					eos
				)
			)
			{
				stmt = new ExpressionStatement(
						new BinaryExpression(ToLexicalInfo(op),
							ParseAssignOperator(op.getText()),
							lhs, rhs));
				stmt.Modifier = modifier;
			}
		)|
		(
			{IsMethodInvocationExpression(lhs)}?
			rhs=callable_expression
			{
				((MethodInvocationExpression)lhs).Arguments.Add(rhs);
				stmt = new ExpressionStatement(lhs);
			}
		)
	)
	;
	
protected
assignment_expression returns [Expression e]
	{
		e = null;
		Expression r=null;
	}:
	e=conditional_expression
	(
		op:ASSIGN
		r=conditional_expression
		{
			BinaryExpression be = new BinaryExpression(ToLexicalInfo(op));
			be.Operator = ParseAssignOperator(op.getText());
			be.Left = e;
			be.Right = r;
			e = be; 
		}
	)?
	;
	
protected
conditional_expression returns [Expression e]
	{
		e = null;		
		Expression r = null;
		BinaryOperatorType op = BinaryOperatorType.None;
		Token token = null;
	}:
	e=sum
	( options { greedy = true; } :
	 (
	  (
		 (
			(t:CMP_OPERATOR { op = ParseCmpOperator(t.getText()); token = t; } ) |
			(tgt:GREATER_THAN { op = BinaryOperatorType.GreaterThan; token = tgt; } ) |
			(tlt:LESS_THAN { op = BinaryOperatorType.LessThan; token = tlt; }) |
			(
				tis:IS! { op = BinaryOperatorType.ReferenceEquality; token = tis; }
				(NOT! { op = BinaryOperatorType.ReferenceInequality; })?
			) |	
			(tisa:ISA! { op = BinaryOperatorType.TypeTest; token = tisa; })
		 )
		 r=sum
	  ) |
	  (
	  	(
			(tin:IN! { op = BinaryOperatorType.Member; token = tin; } ) |
			(tnint:NOT! IN! { op = BinaryOperatorType.NotMember; token = tnint; })
		)		
		r=array_or_expression
	  )
	)
	{
		BinaryExpression be = new BinaryExpression(ToLexicalInfo(token));
		be.Operator = op;
		be.Left = e;
		be.Right = r;
		e = be;
	}
	)*
	;

protected
sum returns [Expression e]
	{
		e = null;
		Expression r = null;
		Token op = null;
		BinaryOperatorType bOperator = BinaryOperatorType.None;
	}:
	e=term
	( options { greedy = true; } :
		(
			add:ADD! { op=add; bOperator = BinaryOperatorType.Addition; } |
			sub:SUBTRACT! { op=sub; bOperator = BinaryOperatorType.Subtraction; } |
			bitor:BITWISE_OR! { op=bitor; bOperator = BinaryOperatorType.BitwiseOr; }
		)
		r=term
		{
			BinaryExpression be = new BinaryExpression(ToLexicalInfo(op));
			be.Operator = bOperator;
			be.Left = e;
			be.Right = r;
			e = be;
		}
	)*
	;

protected
term returns [Expression e]
	{
		e = null;
		Expression r = null;
		Token token = null;
		BinaryOperatorType op = BinaryOperatorType.None; 
	}:
	e=exponentiation
	( options { greedy = true; } :
	 	(
		 m:MULTIPLY! { op=BinaryOperatorType.Multiply; token=m; } |
		 d:DIVISION! { op=BinaryOperatorType.Division; token=d; } |
		 md:MODULUS! { op=BinaryOperatorType.Modulus; token=md; }
		 )
		r=exponentiation
		{
			BinaryExpression be = new BinaryExpression(ToLexicalInfo(token));
			be.Operator = op;
			be.Left = e;
			be.Right = r;
			e = be;
		}
	)*
	;
	
protected
exponentiation returns [Expression e]
	{
		e = null;
		Expression r = null;
	}:
	e=unary_expression
	( options { greedy = true; }:
	 	token:EXPONENTIATION!
		r=exponentiation
		{
			BinaryExpression be = new BinaryExpression(ToLexicalInfo(token));
			be.Operator = BinaryOperatorType.Exponentiation;
			be.Left = e;
			be.Right = r;
			e = be;
		}
	)*
	;
	
	
protected
unary_expression returns [Expression e]
	{
			e = null;
			Token op = null;
			UnaryOperatorType uOperator = UnaryOperatorType.None;
	}: 
	(
		sub:SUBTRACT! { op = sub; uOperator = UnaryOperatorType.UnaryNegation; } |
		inc:INCREMENT! { op = inc; uOperator = UnaryOperatorType.Increment; } |
		dec:DECREMENT! { op = dec; uOperator = UnaryOperatorType.Decrement; }
	)?
	e=slicing_expression
	{
		if (null != op)
		{
			UnaryExpression ue = new UnaryExpression(ToLexicalInfo(op));
			ue.Operator = uOperator;
			ue.Operand = e;
			e = ue; 
		}
	}
	;

protected
atom returns [Expression e]
	{
		e = null;
	}:	
	(
		e=literal |	
		e=reference_expression |
		e=paren_expression |
		e=cast_expression |
		e=typeof_expression |
		e=closure_expression
	)
	;
	
protected
cast_expression returns [Expression e]
	{
		e = null;
		TypeReference tr = null;
		Expression target = null;
	}:
	t:CAST! LPAREN! tr=type_reference COMMA! target=expression RPAREN!
	{
		e = new CastExpression(ToLexicalInfo(t), tr, target);
	}
	;
	
protected
typeof_expression returns [Expression e]
	{
		e = null;
		TypeReference tr = null;
	}:
	t:TYPEOF! LPAREN! tr=type_reference RPAREN!
	{
		e = new TypeofExpression(ToLexicalInfo(t), tr);
	}
	;
	

protected
reference_expression returns [ReferenceExpression e] { e = null; }:
	id:ID
	{
		e = new ReferenceExpression(ToLexicalInfo(id));
		e.Name = id.getText();
	}	
	;
	
protected
paren_expression returns [Expression e] { e = null; }:
	LPAREN! e=array_or_expression RPAREN!
	;

protected
array returns [Expression e]
	{
		ArrayLiteralExpression tle = null;
		e = null;
	}:
	t:LPAREN!
	e=expression
	(
		COMMA!
		{
			tle = new ArrayLiteralExpression(ToLexicalInfo(t));
			tle.Items.Add(e);
		}
		(
			e=expression { tle.Items.Add(e); }
			(
				COMMA!
				e=expression { tle.Items.Add(e); }
			)*
		)?
		{ e = tle; }
	)?
	RPAREN!
	;
	
protected
method_invocation_with_block returns [Statement s]
	{
		s = null;
		MethodInvocationExpression mie = null;
		Expression block = null;
	}:
	RPAREN!
	(
		block=callable_expression
		{
			mie.Arguments.Add(block);
		}
	)?
	;
	
protected
slicing_expression returns [Expression e]
	{
		e = null;
		Expression begin = null;
		Expression end = null;
		Expression step = null;		
		MethodInvocationExpression mce = null;
	} :
	e=atom
	( options { greedy=true; }:
		(
			lbrack:LBRACK!
			(
				( 
					// [:
					COLON! { begin = OmittedExpression.Default; }
					(
						// [:end]
						end=expression
						|
						(
							// [::step]
							COLON! { end = OmittedExpression.Default; }
							step=expression
						)
						|
						// [:]
					)			
				) |
				// [begin
				begin=expression
				(
					// [begin:
					COLON!
					(
						end=expression | { end = OmittedExpression.Default; } 
					)
					(
						COLON!
						step=expression
					)?
				)?
			)
			{
				SlicingExpression se = new SlicingExpression(ToLexicalInfo(lbrack));
				se.Target = e;
				se.Begin = begin;
				se.End = end;
				se.Step = step;
				e = se;
				
				begin = end = step = null;
			}
			RBRACK!
		)
		|
		(
			DOT! id:ID
				{
					MemberReferenceExpression mre = new MemberReferenceExpression(ToLexicalInfo(id));
					mre.Target = e;
					mre.Name = id.getText();
					e = mre;
				}
		)
		|
		(
			lparen:LPAREN!
				{
					mce = new MethodInvocationExpression(ToLexicalInfo(lparen));
					mce.Target = e;
					e = mce;
				}			
			parameter_list[mce]
			RPAREN!
		)
	)*
	;
				
protected
literal returns [Expression e]
	{
		e = null;
	}:
	(
		e=integer_literal |
		e=string_literal |
		e=list_literal |
		e=hash_literal |
		e=re_literal |
		e=bool_literal |
		e=null_literal |
		e=self_literal |
		e=super_literal |
		e=double_literal |
		e=timespan_literal
	)
	;
		
protected
self_literal returns [SelfLiteralExpression e] { e = null; }:
	t:SELF! { e = new SelfLiteralExpression(ToLexicalInfo(t)); }
	;
	
protected
super_literal returns [SuperLiteralExpression e] { e = null; }:
	t:SUPER! { e = new SuperLiteralExpression(ToLexicalInfo(t)); }
	;
		
protected
null_literal returns [NullLiteralExpression e] { e = null; }:
	t:NULL! { e = new NullLiteralExpression(ToLexicalInfo(t)); }
	;
		
protected
bool_literal returns [BoolLiteralExpression e] { e = null; }:
	t:TRUE!
	{
		e = new BoolLiteralExpression(ToLexicalInfo(t));
		e.Value = true;
	} |
	f:FALSE!
	{
		e = new BoolLiteralExpression(ToLexicalInfo(f));
		e.Value = false;
	}
	;

protected
integer_literal returns [IntegerLiteralExpression e] { e = null; } :
	i:INT
	{
		e = new IntegerLiteralExpression(ToLexicalInfo(i), long.Parse(i.getText()));
	}
	|
	l:LONG
	{
		string value = l.getText();
		value = value.Substring(0, value.Length-1);
		
		e = new IntegerLiteralExpression(ToLexicalInfo(l),
					long.Parse(value),
					true);
	}
	;
	
protected
string_literal returns [Expression e]
	{
		e = null;
	}:
	e=expression_interpolation |	
	dqs:DOUBLE_QUOTED_STRING
	{
		e = new StringLiteralExpression(ToLexicalInfo(dqs), dqs.getText());
	} |
	sqs:SINGLE_QUOTED_STRING
	{
		e = new StringLiteralExpression(ToLexicalInfo(sqs), sqs.getText());
	} |
	tqs:TRIPLE_QUOTED_STRING
	{
		e = new StringLiteralExpression(ToLexicalInfo(tqs), tqs.getText());
	}
	;
	
protected
expression_interpolation returns [ExpressionInterpolationExpression e]
	{
		e = null;
		Expression param = null;	
	}:
	separator:ESEPARATOR
	{
		LexicalInfo info = ToLexicalInfo(separator);
		e = new ExpressionInterpolationExpression(info);		
	}
	(  options { greedy = true; } :
		
		ESEPARATOR!		
		param=expression { e.Expressions.Add(param); }
		ESEPARATOR!
	)*
	;
	

protected
list_literal returns [Expression e]
	{
		e = null;
		ListLiteralExpression lle = null;
		Expression item = null;
	}:
	lbrack:LBRACK!
	(
		(
			item=expression
			(
				{
					e = lle = new ListLiteralExpression(ToLexicalInfo(lbrack));
					lle.Items.Add(item);
				}
				(
					COMMA! item=expression { lle.Items.Add(item); }
				)*
			)
		)
		|
		{ e = new ListLiteralExpression(ToLexicalInfo(lbrack)); }
	)
	RBRACK!
	;
		
protected
hash_literal returns [HashLiteralExpression dle]
	{
		dle = null;
		ExpressionPair pair = null;
	}:
	lbrace:LBRACE! { dle = new HashLiteralExpression(ToLexicalInfo(lbrace)); }
	(
		pair=expression_pair			
		{ dle.Items.Add(pair); }
		(
			COMMA!
			pair=expression_pair
			{ dle.Items.Add(pair); }
		)*
	)?
	RBRACE!
	;
		
protected
expression_pair returns [ExpressionPair ep]
	{
		ep = null;
		Expression key = null;
		Expression value = null;
	}:
	key=expression t:COLON! value=expression
	{ ep = new ExpressionPair(ToLexicalInfo(t), key, value); }
	;
		
protected
re_literal returns [RELiteralExpression re] { re = null; }:
	value:RE_LITERAL
	{ re = new RELiteralExpression(ToLexicalInfo(value), value.getText()); }
	;
	
protected
double_literal returns [DoubleLiteralExpression rle] { rle = null; }:
	value:DOUBLE
	{ rle = new DoubleLiteralExpression(ToLexicalInfo(value), ParseDouble(value.getText())); }
	;
	
protected
timespan_literal returns [TimeSpanLiteralExpression tsle] { tsle = null; }:
	value:TIMESPAN
	{ tsle = new TimeSpanLiteralExpression(ToLexicalInfo(value), ParseTimeSpan(value.getText())); }
	;

protected
expression_list[ExpressionCollection ec]
	{
		Expression e = null;
	} :
	(
		e=expression { ec.Add(e); }
		(
			COMMA!
			e=expression { ec.Add(e); }
		)*
	)?
	;
	
protected
parameter_list[INodeWithArguments node]:
	(
		parameter[node] 
		(
			COMMA!
			parameter[node]
		)*
	)?
	;
	
protected
parameter[INodeWithArguments node]
	{
		Expression e = null;
		Expression value = null;
	} :
	e=expression
	(
		(
			colon:COLON!
			value=expression
			{ node.NamedArguments.Add(new ExpressionPair(ToLexicalInfo(colon), e, value)); }
		) |
		{ node.Arguments.Add(e); }
	)
	;

protected
identifier returns [Token value]
	{
		value = null; _sbuilder.Length = 0;
	}:			
	id1:ID			
	{					
		_sbuilder.Append(id1.getText());
		value = id1;
	}				
	( options { greedy = true; } :
		DOT!
		id2:ID
		{ _sbuilder.Append('.'); _sbuilder.Append(id2.getText()); }
	)*
	{ value.setText(_sbuilder.ToString()); }
	;	 
{
using Boo.AntlrParser.Util;
}
class BooLexer extends Lexer;
options
{
	testLiterals = false;
	exportVocab = Boo;	
	k = 2;
	charVocabulary='\u0003'..'\uFFFE';
	// without inlining some bitset tests, ANTLR couldn't do unicode;
	// They need to make ANTLR generate smaller bitsets;
	codeGenBitsetTestThreshold=20;
}
{
	protected int _skipWhitespaceRegion = 0;
	
	BooExpressionLexer _el;
	
	TokenStreamRecorder _erecorder;
	
	antlr.TokenStreamSelector _selector;
	
	internal void Initialize(antlr.TokenStreamSelector selector, int tabSize, antlr.TokenCreator tokenCreator)
	{
		setTabSize(tabSize);
		setTokenCreator(tokenCreator);
		
		_selector = selector;
		_el = new BooExpressionLexer(getInputState());
		_el.setTabSize(tabSize);
		_el.setTokenCreator(tokenCreator);
		
		_erecorder = new TokenStreamRecorder(selector);
		
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

	void Enqueue(antlr.Token token, string text)
	{
		token.setText(text);
		_erecorder.Enqueue(makeESEPARATOR());
		_erecorder.Enqueue(token);
		_erecorder.Enqueue(makeESEPARATOR());
	}
	
	antlr.Token makeESEPARATOR()
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
}

ID options { testLiterals = true; }:
		ID_LETTER (ID_LETTER | DIGIT)*
	;
	
LINE_CONTINUATION:
	'\\'! NEWLINE
	{ $setType(Token.SKIP); }
	;

INT : (DIGIT)+
	(
		('l' | 'L') { $setType(LONG); } |
		(
	({BooLexer.IsDigit(LA(2))}? ('.' (DIGIT)+) { $setType(DOUBLE); })?
	(("ms" | 's' | 'm' | 'h' | 'd') { $setType(TIMESPAN); })?
		)
	)
	;

DOT : '.' ((DIGIT)+ {$setType(DOUBLE);})?;

COLON : ':';

BITWISE_OR: '|';

LPAREN : '(' { EnterSkipWhitespaceRegion(); };
	
RPAREN : ')' { LeaveSkipWhitespaceRegion(); };

LBRACK : '[' { EnterSkipWhitespaceRegion(); };

RBRACK : ']' { LeaveSkipWhitespaceRegion(); };

LBRACE : '{' { EnterSkipWhitespaceRegion(); };
	
RBRACE : '}' { LeaveSkipWhitespaceRegion(); };

INCREMENT: "++";

DECREMENT: "--";

ADD: ('+') ('=' { $setType(ASSIGN); })?;

SUBTRACT: ('-') ('=' { $setType(ASSIGN); })?;

MODULUS: '%';

MULTIPLY: '*' (
					'=' { $setType(ASSIGN); } |
					'*' { $setType(EXPONENTIATION); } | 
				);


DIVISION: 
	("/*")=> ML_COMMENT { $setType(Token.SKIP); } |
	(RE_LITERAL)=> RE_LITERAL { $setType(RE_LITERAL); } |	
	'/' (
		('/' (~('\r'|'\n'))* { $setType(Token.SKIP); }) |			
			('=' { $setType(ASSIGN); }) |
		)
	;

LESS_THAN: '<';

GREATER_THAN: '>';

CMP_OPERATOR :  "<=" | ">=" | "!~" | "!=";

ASSIGN : '=' ( ('=' | '~') { $setType(CMP_OPERATOR); } )?;

COMMA : ','!;

protected
TRIPLE_QUOTED_STRING:
	"\"\""!
	(
	options { greedy=false; }:		
		("${")=>		
		{					
			Enqueue(makeToken(TRIPLE_QUOTED_STRING), $getText);
			$setText("");
		}
		ESCAPED_EXPRESSION |
		("\\$")=>'\\'! '$' |
		~('\r'|'\n') |
		NEWLINE
	)*
	"\"\"\""!
	;

DOUBLE_QUOTED_STRING:
	'"'!
	(
		{LA(1)=='"' && LA(2)=='"'}?TRIPLE_QUOTED_STRING { $setType(TRIPLE_QUOTED_STRING); }
		|
		(
			(
				DQS_ESC |
				("${")=>
				{					
					Enqueue(makeToken(DOUBLE_QUOTED_STRING), $getText);
					$setText("");
				}
				ESCAPED_EXPRESSION |
				~('"' | '\\' | '\r' | '\n')
			)*
			'"'!			
		)
	)
	{
		if (_erecorder.Count > 0)
		{
			Enqueue(makeToken(DOUBLE_QUOTED_STRING), $getText);

			$setType(ESEPARATOR);
			$setText("");			
			_selector.push(_erecorder);
		}
	}
	;
		
SINGLE_QUOTED_STRING :
	'\''!
	(
		SQS_ESC |
		~('\'' | '\\' | '\r' | '\n')
	)*
	'\''!
	;

SL_COMMENT:
	"#" (~('\r'|'\n'))*
	{ $setType(Token.SKIP); }
	;
	
protected
ML_COMMENT:
	"/*"
    (
		{ LA(2) != '/' }? '*' |
		("/*")=>ML_COMMENT |
		NEWLINE |
		~('*'|'\r'|'\n')
    )*
    "*/"
    { $setType(Token.SKIP); }
	;   
			
WS :
	(
		' ' |
		'\t' |
		'\f' |
		NEWLINE
	)+
	{
		if (SkipWhitespace)
		{
			$setType(Token.SKIP);
		}
	}
	;
		
EOS: ';';

X_RE_LITERAL: '@'!'/' (X_RE_CHAR)+ '/' { $setType(RE_LITERAL); };

protected
NEWLINE:
	(
		'\n' |
		(
			'\r' ('\n')?
		)
	)
	{ newline(); }
	;
		
protected
ESCAPED_EXPRESSION : "${"!
	{		
		_erecorder.Enqueue(makeESEPARATOR());
		if (0 == _erecorder.RecordUntil(_el, RBRACE))
		{	
			_erecorder.Dequeue();			
		}
		else
		{
			_erecorder.Enqueue(makeESEPARATOR());
		}
		$setText("");
	}
	;

protected
DQS_ESC : '\\'! ( SESC | '"' | '$') ;	
	
protected
SQS_ESC : '\\'! ( SESC | '\'' );

protected
SESC : 
				( 'r'! {$setText("\r"); }) |
				( 'n'! {$setText("\n"); }) |
				( 't'! {$setText("\t"); }) |
				( '\\'! {$setText("\\"); });

protected
RE_LITERAL : '/' (RE_CHAR)+ '/';

protected
RE_CHAR : RE_ESC | ~('/' | '\\' | '\r' | '\n' | ' ' | '\t' );

protected
X_RE_CHAR: RE_CHAR | ' ' | '\t';


protected
RE_ESC : '\\' (
	
	// character scapes
	// ms-help://MS.NETFrameworkSDKv1.1/cpgenref/html/cpconcharacterescapes.htm
	
				'a' |
				'b' |
				'c' 'A'..'Z' |
				't' |
				'r' |
				'v' |
				'f' |
				'n' |
				'e' |
				(DIGIT)+ |
				'x' DIGIT DIGIT |
				'u' DIGIT DIGIT DIGIT DIGIT |
				'\\' |
				
	// character classes
	// ms-help://MS.NETFrameworkSDKv1.1/cpgenref/html/cpconcharacterclasses.htm
	// /\w\W\s\S\d\D/
	
				'w' |
				'W' |
				's' |
				'S' |
				'd' |
				'D' |
				'p' |
				'P' |
				
	// atomic zero-width assertions
	// ms-help://MS.NETFrameworkSDKv1.1/cpgenref/html/cpconatomiczero-widthassertions.htm
				'A' |
				'z' |
				'Z' |
				'g' |
				'B' |
				
				'k' |
				
				'/' |
				'(' |
				')' |
				'|' |
				'.' |
				'*' |
				'?' |
				'$' |
				'^' |
				'['	|
				']'
			 )
			 ;

protected
ID_LETTER : ('_' | 'a'..'z' | 'A'..'Z' );

protected
DIGIT : '0'..'9';
