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
	namespace = "Boo.Lang.Parser";
}

{
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Parser.Util;
using System.Globalization;
}

class WSABooParserBase extends Parser;
options
{
	k = 2;
	exportVocab = WSABoo; 
	defaultErrorHandler = true;
}
tokens
{	
	ELIST; // expression list
	DLIST; // declaration list
	ESEPARATOR; // expression separator (imaginary token)
	ABSTRACT="abstract";
	AND="and";
	AS="as";
	AST="ast";
	BREAK="break";
	CONTINUE="continue";
	CALLABLE="callable";
	CAST="cast";
	CHAR="char";
	CLASS="class";
	CONSTRUCTOR="constructor";	
	DEF="def";
	DESTRUCTOR="destructor";
	DO="do";	
	ELIF="elif";
	ELSE="else";
	END="end";
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
	GOTO="goto";
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
	NAMESPACE="namespace";
	PUBLIC="public";
	PROTECTED="protected";
	PRIVATE="private";
	RAISE="raise";
	REF="ref";
	RETURN="return";
	RETRY="retry";
	SET="set";	
	SELF="self";
	SUPER="super";
	STATIC="static";
	SUCCESS="success";
	STRUCT="struct";
	TRY="try";
	TRANSIENT="transient";
	TRUE="true";
	TYPEOF="typeof";
	UNLESS="unless";
	VIRTUAL="virtual";
	PARTIAL="partial";
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
		return ParseDouble(s,false);
	}
	
	static double ParseDouble(string s, bool isSingle)
	{
		double val;
		if (isSingle)
		{
			val = float.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
		}
		else
		{
			val = double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
		}
		return val;
	}
	
	protected IntegerLiteralExpression ParseIntegerLiteralExpression(
		antlr.IToken token, string s, bool isLong)
	{
		const string HEX_PREFIX = "0x";
		
		long value;
		NumberStyles style = NumberStyles.Integer | NumberStyles.AllowExponent;
		int hex_start = s.IndexOf(HEX_PREFIX);
		bool negative = false;

		if (hex_start >=0)
		{
			if (s.StartsWith("-"))
			{
				negative = true;
			}
			s = s.Substring(hex_start+HEX_PREFIX.Length);
			style = NumberStyles.HexNumber;
		}
		if (isLong)
		{
			value = long.Parse(s, style, CultureInfo.InvariantCulture);
		}
		else
		{
			value = int.Parse(s, style, CultureInfo.InvariantCulture);
		}
		if (negative) //negative hex number
		{
			value *= -1;
		}
		return new IntegerLiteralExpression(ToLexicalInfo(token), value, isLong);
	}
	
}

protected
start[CompileUnit cu] returns [Module module]
	{
		module = new Module();		
		module.LexicalInfo = new LexicalInfo(getFilename(), 1, 1);
		
		cu.Modules.Add(module);
	}:
	(options { greedy=true;}: EOS)*
	docstring[module]
	(options { greedy=true;}: EOS)*			 
	(namespace_directive[module])?
	(import_directive[module])*
	(type_member[module.Members])*	
	globals[module]
	(assembly_attribute[module] eos)*
	EOF
	;
			
protected docstring[Node node]:
	(
		doc:TRIPLE_QUOTED_STRING { node.Documentation = MassageDocString(doc.getText()); }
		(options { greedy=true; }: EOS)*
	)?
	;
			
protected
eos : (options { greedy = true; }: EOS)+;

protected
import_directive[Module container]
	{
		IToken id;
		Import usingNode = null;
	}: 
	IMPORT id=identifier
	{
		usingNode = new Import(ToLexicalInfo(id));
		usingNode.Namespace = id.getText();
		container.Imports.Add(usingNode);
	}
	(
		FROM
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
		AS alias:ID
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
		IToken id;
		NamespaceDeclaration p = null;
	}:
	t:NAMESPACE id=identifier
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
	}:
	CALLABLE id:ID
	{
		cd = new CallableDefinition(ToLexicalInfo(id));
		cd.Name = id.getText();
		cd.Modifiers = _modifiers;
		AddAttributes(cd.Attributes);
		container.Add(cd);
	}
	LPAREN parameter_declaration_list[cd.Parameters] RPAREN
	(AS returnType=type_reference { cd.ReturnType=returnType; })?			
	eos
	docstring[cd]
	;

protected
enum_definition [TypeMemberCollection container]
	{
		EnumDefinition ed = null;
	}:
	ENUM id:ID { ed = new EnumDefinition(ToLexicalInfo(id)); }
	begin_with_doc[ed]
	{
		ed.Name = id.getText();
		ed.Modifiers = _modifiers;
		AddAttributes(ed.Attributes);
		container.Add(ed);
	}
	(
		(enum_member[ed])+
	)
	end[ed]
	;
	
protected
enum_member [EnumDefinition container]
	{	
		EnumMember em = null;	
		IntegerLiteralExpression initializer = null;
		bool negative = false;		
	}: 
	attributes
	id:ID (ASSIGN (SUBTRACT { negative = true; })? initializer=integer_literal)?
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
	eos
	docstring[em]
	;
			
protected
attributes
	{
		_attributes.Clear();
	}:
	(
		LBRACK
		(
			attribute
			(
				COMMA
				attribute
			)*
		)?
		RBRACK		
		(EOS)*
	)*
	;
			
protected
attribute
	{		
		antlr.IToken id = null;
		Boo.Lang.Compiler.Ast.Attribute attr = null;
	}:	
	id=identifier
	{
		attr = new Boo.Lang.Compiler.Ast.Attribute(ToLexicalInfo(id), id.getText());
		_attributes.Add(attr);
	} 
	(
		LPAREN
		argument_list[attr]
		RPAREN
	)?
	;
	
protected
assembly_attribute[Module module]
	{
		antlr.IToken id = null;
		Boo.Lang.Compiler.Ast.Attribute attr = null;
	}:
	ASSEMBLY_ATTRIBUTE_BEGIN
	id=identifier { attr = new Boo.Lang.Compiler.Ast.Attribute(ToLexicalInfo(id), id.getText()); }
	(
		LPAREN
		argument_list[attr]
		RPAREN
	)?
	RBRACK
	{ module.AssemblyAttributes.Add(attr); }
	;
			
protected
class_definition [TypeMemberCollection container]
	{
		TypeDefinition td = null;
		TypeReferenceCollection baseTypes = null;
		TypeMemberCollection members = null;
	}:
	(
		CLASS { td = new ClassDefinition(); } |
		STRUCT { td = new StructDefinition(); }
	)
	id:ID
	{		
		td.LexicalInfo = ToLexicalInfo(id);
		td.Name = id.getText();
		td.Modifiers = _modifiers;
		AddAttributes(td.Attributes);
		container.Add(td);
		baseTypes = td.BaseTypes;
		members = td.Members;
	}
	(base_types[baseTypes])?
	begin_with_doc[td]					
	(
		(EOS)*
		(type_definition_member[members])+
	)
	end[td]
	;
	
type_definition_member[TypeMemberCollection container]
{
}:
	attributes
	modifiers
	(						
		method[container] |
		event_declaration[container] |
		field_or_property[container] |
		type_definition[container]
	)
;
			
protected
interface_definition [TypeMemberCollection container]
	{
		InterfaceDefinition itf = null;
		TypeMemberCollection members = null;
	} :
	INTERFACE id:ID
	{
		itf = new InterfaceDefinition(ToLexicalInfo(id));
		itf.Name = id.getText();
		itf.Modifiers = _modifiers;
		AddAttributes(itf.Attributes);
		container.Add(itf);
		members = itf.Members;
	}
	(base_types[itf.BaseTypes])?
	begin_with_doc[itf]
	(
		attributes
		(
			interface_method[members] |
			event_declaration[members] |
			interface_property[members]
		)
	)+
	end[itf]
	;
			
protected
base_types[TypeReferenceCollection container]
	{
		TypeReference tr = null;
	}:
	LPAREN 
	(
		tr=type_reference { container.Add(tr); }
		(COMMA tr=type_reference { container.Add(tr); })*
	)?
	RPAREN
	;
			
protected
interface_method [TypeMemberCollection container]
	{
		Method m = null;
		TypeReference rt = null;
	}: 
	DEF id:ID
	{
		m = new Method(ToLexicalInfo(id));
		m.Name = id.getText();
		AddAttributes(m.Attributes);
		container.Add(m);
	}
	LPAREN parameter_declaration_list[m.Parameters] RPAREN
	(AS rt=type_reference { m.ReturnType=rt; })?			
	(
		(eos docstring[m]) | (empty_block[m] (EOS)*)
	)
	;
			
protected
interface_property [TypeMemberCollection container]
        {
                Property p = null;
                TypeReference tr = null;
                ParameterDeclarationCollection parameters = null;
        }:
        (id:ID)
        {
                p = new Property(ToLexicalInfo(id));
                p.Name = id.getText();
                AddAttributes(p.Attributes);
                container.Add(p);
                parameters = p.Parameters;
        }
        (LPAREN parameter_declaration_list[parameters] RPAREN)?
        (AS tr=type_reference)?
        {
                p.Type = tr;
        }
        begin_with_doc[p]
                (interface_property_accessor[p])+
        end[p]
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
			gt:GET { m = p.Getter = new Method(ToLexicalInfo(gt)); m.Name = "get"; }
		)
		|
		{ null == p.Setter }?
		(
			st:SET { m = p.Setter = new Method(ToLexicalInfo(st)); m.Name = "set"; }
		)				
	)
	(
		eos | empty_block[m]
	)
	{
		AddAttributes(m.Attributes);
	}
	;
			
protected
empty_block[Node node]: 
		begin
			eos
		end[node]
		;
		
protected
event_declaration [TypeMemberCollection container]
	{
		Event e = null;
		TypeReference tr = null;
	}:
	t:EVENT
	id:ID AS tr=type_reference eos
	{
		e = new Event(ToLexicalInfo(id), id.getText(), tr);
		e.Modifiers = _modifiers;
		AddAttributes(e.Attributes);
		container.Add(e);
	}
	docstring[e]
	;

protected
explicit_member_info returns [ExplicitMemberInfo emi]
	{
		emi = null; _sbuilder.Length = 0;
	}:
	(ID DOT)=>(
		(
			(id:ID DOT)
			{
				emi = new ExplicitMemberInfo(ToLexicalInfo(id));
				_sbuilder.Append(id.getText());
			}
			(
				(id2:ID DOT)
				{
					_sbuilder.Append('.');
					_sbuilder.Append(id2.getText());
				}
			)*
		)
	)
	{
		if (emi != null)
		{
			emi.InterfaceType = new SimpleTypeReference(emi.LexicalInfo);
			emi.InterfaceType.Name = _sbuilder.ToString();
		}
	}
	;

protected
method [TypeMemberCollection container]
	{
		Method m = null;
		TypeReference rt = null;
		TypeReference it = null;
		ExplicitMemberInfo emi = null;
		ParameterDeclarationCollection parameters = null;
		Block body = null;
		StatementCollection statements = null;
	}: 
	t:DEF
	(
		(emi=explicit_member_info)? id:ID {
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
		|
		c:CONSTRUCTOR { m = new Constructor(ToLexicalInfo(c)); } |
		d:DESTRUCTOR { m = new Destructor(ToLexicalInfo(d)); }
	)	
	{
		m.Modifiers = _modifiers;
		AddAttributes(m.Attributes);
		parameters = m.Parameters;
		body = m.Body;
		statements = body.Statements;
	}
	LPAREN parameter_declaration_list[parameters] RPAREN
			(AS rt=type_reference { m.ReturnType = rt; })?
			attributes { AddAttributes(m.ReturnTypeAttributes); }
			begin_block_with_doc[m, body]
				block[statements]
			end[body]
	{ 
		container.Add(m);
	}
	;	

protected
property_header:	
	(ID (DOT ID)*)
	(
		LPAREN |
		((AS type_reference)? COLON)
	)
	;
	
protected
field_or_property [TypeMemberCollection container]
	{
		TypeMember tm = null;
		TypeReference tr = null;
		Property p = null;
		Field field = null;
		ExplicitMemberInfo emi = null;
		Expression initializer = null;
		ParameterDeclarationCollection parameters = null;
	}: 
	(property_header)=>(
		(emi=explicit_member_info)? id:ID
		(		
			
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
			(LPAREN parameter_declaration_list[parameters] RPAREN)?
			(AS tr=type_reference)?
			{							
				p.Type = tr;
				tm = p;
				tm.Modifiers = _modifiers;
			}		
			begin_with_doc[p]
				(property_accessor[p])+
			end[p]
		)
	)
	{ container.Add(tm); }
	|
	(
		id2:ID
		{
			tm = field = new Field(ToLexicalInfo(id2));
			field.Name = id2.getText();
			field.Modifiers = _modifiers;
			AddAttributes(field.Attributes);
		}
		(		
			(AS tr=type_reference { field.Type = tr; })?
			(
				(
					ASSIGN initializer=declaration_initializer
					{ field.Initializer = initializer;	}
				) |
				eos
			)
			docstring[field]
		)
	)
	{ container.Add(tm); }
;
	
declaration_initializer returns [Expression e]
{
	e = null;
}:
	(slicing_expression (DO|DEF))=>(e=slicing_expression method_invocation_block[e]) |
	(e=array_or_expression eos) |
	(e=callable_expression) |
	(e=ast_literal)
;

protected
property_accessor[Property p]
	{		
		Method m = null;
		Block body = null;
	}:
	attributes
	modifiers
	(
		{ null == p.Getter }?
		(
			gt:GET
			{
				p.Getter = m = new Method(ToLexicalInfo(gt));		
				m.Name = "get";
			}
		)
		|
		{ null == p.Setter }?
		(
			st:SET
			{
				p.Setter = m = new Method(ToLexicalInfo(st));
				m.Name = "set";
			}
		)
	)
	{
		AddAttributes(m.Attributes);
		m.Modifiers = _modifiers;
		body = m.Body;
	}
	compound_stmt[body]
	;
	
protected
globals[Module container]:	
	(EOS)*
	(stmt[container.Globals.Statements])*
	;
	
protected
block[StatementCollection container]:
	(EOS)*
	(			
		stmt[container]
	)*
	; 
	
protected
modifiers
{
	_modifiers = TypeMemberModifiers.None;
}:
	(
	STATIC { _modifiers |= TypeMemberModifiers.Static; } |
	PUBLIC { _modifiers |= TypeMemberModifiers.Public; } |
	PROTECTED { _modifiers |= TypeMemberModifiers.Protected; } |
	PRIVATE { _modifiers |= TypeMemberModifiers.Private; } |
	INTERNAL { _modifiers |= TypeMemberModifiers.Internal; } |			
	FINAL { _modifiers |= TypeMemberModifiers.Final; } |
	TRANSIENT { _modifiers |= TypeMemberModifiers.Transient; } |
	OVERRIDE { _modifiers |= TypeMemberModifiers.Override; } |
	ABSTRACT { _modifiers |= TypeMemberModifiers.Abstract; } |
	VIRTUAL { _modifiers |= TypeMemberModifiers.Virtual; } |
	PARTIAL { _modifiers |= TypeMemberModifiers.Partial; }	
	)*
;

protected
parameter_modifier returns [ParameterModifiers pm]
	{
		pm = ParameterModifiers.None;
	}:
	(
		REF { pm = ParameterModifiers.Ref; }
	)
	;

protected	
parameter_declaration_list[ParameterDeclarationCollection c]
	{
		bool variableArguments = false;
	}: 
	(variableArguments=parameter_declaration[c]
	( {!variableArguments}?(COMMA variableArguments=parameter_declaration[c]) )* )?
	{ c.VariableNumber = variableArguments; }
	;

protected
parameter_declaration[ParameterDeclarationCollection c]
	returns [bool variableArguments]
	{		
		IToken id = null;
		TypeReference tr = null;
		ParameterModifiers pm = ParameterModifiers.None;
		variableArguments = false;
	}: 
	attributes
	(
		(
			MULTIPLY { variableArguments=true; }
			id1:ID (AS tr=array_type_reference)?
			{ id = id1; }
		)
		|
		(
			(pm=parameter_modifier)?
			id2:ID (AS tr=type_reference)?
			{ id = id2; }
		)
	)
	{
		ParameterDeclaration pd = new ParameterDeclaration(ToLexicalInfo(id));
		pd.Name = id.getText();
		pd.Type = tr;
		pd.Modifiers = pm;
		AddAttributes(pd.Attributes);
		c.Add(pd);
	} 
	;
	
protected	
callable_parameter_declaration_list[ParameterDeclarationCollection c]:
	(callable_parameter_declaration[c]
	(COMMA callable_parameter_declaration[c])*)?
	;

protected
callable_parameter_declaration[ParameterDeclarationCollection c]
	{		
		TypeReference tr = null;
		ParameterModifiers pm = ParameterModifiers.None;
	}: 
	(
		(pm=parameter_modifier)?
		(tr=type_reference)
	)
	{
		ParameterDeclaration pd = new ParameterDeclaration(tr.LexicalInfo);
		pd.Name = "arg" + c.Count;
		pd.Type = tr;
		pd.Modifiers = pm;
		c.Add(pd);
	} 
	;
	
protected
callable_type_reference returns [CallableTypeReference ctr]
	{
		ctr = null;
		TypeReference tr = null;
		ParameterDeclarationCollection parameters = null;
	}:	
	c:CALLABLE LPAREN
	{
		ctr = new CallableTypeReference(ToLexicalInfo(c));
		parameters = ctr.Parameters;
	}
	callable_parameter_declaration_list[parameters]
	RPAREN
	(AS tr=type_reference { 
		ctr.ReturnType = tr; 
		}
	)?
	;
	
protected
array_type_reference returns [ArrayTypeReference atr]
	{
		TypeReference tr = null;
		atr = null;
		IntegerLiteralExpression rank = null;
	}:
	lparen:LPAREN
	{
		atr = new ArrayTypeReference(ToLexicalInfo(lparen));
	}
	(
		tr=type_reference { atr.ElementType = tr; }
		(COMMA rank=integer_literal { atr.Rank = rank; })?
	)
	rparen:RPAREN
	;

protected
type_reference returns [TypeReference tr]
	{
		tr=null;
		IToken id = null;
	}: 
	tr=array_type_reference
	|
	(CALLABLE LPAREN)=>(tr=callable_type_reference)
	|
	(
		id=type_name
		{
			SimpleTypeReference str = new SimpleTypeReference(ToLexicalInfo(id));
			str.Name = id.getText();
			tr = str;
		}
	)
	;
	
protected
type_name returns [IToken id]
	{
		id = null;
	}:
	id=identifier | c:CALLABLE { id=c; } | ch:CHAR { id=ch; }
	;

protected
begin: COLON;

protected
begin_with_doc[Node node]: COLON (EOS docstring[node])?;
	
protected
begin_block_with_doc[Node node, Block block]:
	begin:COLON (EOS docstring[node])?
	{
		block.LexicalInfo = ToLexicalInfo(begin);
	}
	;

protected
end[Node node] :
	t:END { node.EndSourceLocation = ToSourceLocation(t); }
	(options { greedy=true; }: EOS)*
	;

protected
compound_stmt[Block b]
{
	StatementCollection statements = null;
}:
		begin:COLON
		{
			b.LexicalInfo = ToLexicalInfo(begin);
			statements = b.Statements;
		}
			block[statements]
		end[b]
		;
		
protected
closure_macro_stmt returns [MacroStatement returnValue]
	{
		returnValue = null;
		MacroStatement macro = new MacroStatement();
	}:
	id:ID expression_list[macro.Arguments]
	{
		macro.Name = id.getText();
		macro.LexicalInfo = ToLexicalInfo(id);		
		returnValue = macro;
	}
;

		
protected
macro_stmt returns [MacroStatement returnValue]
	{
		returnValue = null;
		MacroStatement macro = new MacroStatement();
		StatementModifier modifier = null;
	}:
	id:ID expression_list[macro.Arguments]
	(
		compound_stmt[macro.Block] |
		eos |
		modifier=stmt_modifier eos { macro.Modifier = modifier; }
	)
	{
		macro.Name = id.getText();
		macro.LexicalInfo = ToLexicalInfo(id);
		
		returnValue = macro;
	}
;

protected
goto_stmt returns [GotoStatement stmt]
	{
		stmt = null;
	}:
	token:GOTO label:ID
	{
		stmt = new GotoStatement(ToLexicalInfo(token),
					new ReferenceExpression(ToLexicalInfo(label), label.getText()));
	}
	;
	
protected
label_stmt returns [LabelStatement stmt]
	{
		stmt = null;
	}:
	token:COLON label:ID
	{
		stmt = new LabelStatement(ToLexicalInfo(token), label.getText());
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
		(ID (expression)?)=>{IsValidMacroArgument(LA(2))}? s=macro_stmt |
		(slicing_expression (ASSIGN|(DO|DEF)))=> s=assignment_or_method_invocation_with_block_stmt |
		s=return_stmt |
		(declaration COMMA)=> s=unpack_stmt |
		s=declaration_stmt |
		(		
			(
				s=goto_stmt |
				s=label_stmt |
				s=yield_stmt |
				s=break_stmt |
				s=continue_stmt |				
				s=raise_stmt |
				s=retry_stmt |
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
		IToken t = null;
		StatementModifierType type = StatementModifierType.Uninitialized;
	}:
	(
		i:IF { t = i; type = StatementModifierType.If; } |
		u:UNLESS { t = u; type = StatementModifierType.Unless; } |
		w:WHILE { t = w; type = StatementModifierType.While; }
	)
	e=boolean_expression
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
	(parameter_modifier)?
	(ID (AS type_reference)?)
	(COMMA ID (AS type_reference)?)*
	BITWISE_OR
	;
	
protected
internal_closure_stmt[Block block]
	{
		Statement stmt = null;
		StatementModifier modifier = null;
	}:
	(
		stmt=return_expression_stmt |
		(
			(
				(declaration COMMA)=>stmt=unpack |
				{IsValidMacroArgument(LA(2))}? stmt=closure_macro_stmt | 
				stmt=expression_stmt |
				stmt=raise_stmt |
				stmt=yield_stmt			
			)
			(modifier=stmt_modifier { stmt.Modifier = modifier; })?		
		)
	)
	{
		if (null != stmt)
		{
			block.Add(stmt);
		}
	}
	;
	
protected
closure_expression returns [Expression e]
	{
		e = null;
		CallableBlockExpression cbe = null;
		ParameterDeclarationCollection parameters = null;
		Block body = null;
	}:
	anchorBegin:LBRACE
		{
			e = cbe = new CallableBlockExpression(ToLexicalInfo(anchorBegin));
			cbe.Annotate("inline");
			parameters = cbe.Parameters;
			body = cbe.Body;
		}
		
		(
			(closure_parameters_test)=>(
				parameter_declaration_list[parameters]
				BITWISE_OR
			) |
		)
		(
			internal_closure_stmt[body]
			(
				eos
				(internal_closure_stmt[body])?
			)*
		)
	anchorEnd:RBRACE
	;
	
protected
callable_expression returns [Expression e]
	{
		e = null;
		CallableBlockExpression cbe = null;
		TypeReference rt = null;
		IToken anchor = null;
	}:
	(
		(doAnchor:DO { anchor = doAnchor; }) |
		(defAnchor:DEF { anchor = defAnchor; })
	)
	{
		e = cbe = new CallableBlockExpression(ToLexicalInfo(anchor));
	}
	(
		LPAREN parameter_declaration_list[cbe.Parameters] RPAREN
		(AS rt=type_reference { cbe.ReturnType = rt; })?
	)?
		compound_stmt[cbe.Body]
	;
	
	
protected
retry_stmt returns [RetryStatement rs]
	{
		rs = null;
	}:
	t:RETRY { rs = new RetryStatement(ToLexicalInfo(t)); }
	;
	
protected
try_stmt returns [TryStatement s]
	{
		s = null;		
		Block sblock = null;
		Block eblock = null;
	}:
	t:TRY { s = new TryStatement(ToLexicalInfo(t)); }
		compound_stmt[s.ProtectedBlock]
	(
		exception_handler[s]
	)*
	(
		stoken:SUCCESS { sblock = new Block(ToLexicalInfo(stoken)); }
			compound_stmt[sblock]
		{ s.SuccessBlock = sblock; }
	)?
	(
		etoken:ENSURE { eblock = new Block(ToLexicalInfo(etoken)); }
			compound_stmt[eblock]
		{ s.EnsureBlock = eblock; }
	)?
	;
	
protected
exception_handler [TryStatement t]
	{
		ExceptionHandler eh = null;		
		TypeReference tr = null;
	}:
	c:EXCEPT (x:ID (AS tr=type_reference)?)?
	{
		eh = new ExceptionHandler(ToLexicalInfo(c));
		
		if (x != null)
		{
			eh.Declaration = new Declaration(ToLexicalInfo(x));
			eh.Declaration.Name = x.getText();		
			eh.Declaration.Type = tr;
		}
	}		
	compound_stmt[eh.Block]
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
	t:RAISE (e=expression)?
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
		StatementModifier m = null;
	}:
	id:ID AS tr=type_reference
	(
		(ASSIGN initializer=declaration_initializer) |
		((m=stmt_modifier)? eos)
	)
	{
		Declaration d = new Declaration(ToLexicalInfo(id));
		d.Name = id.getText();
		d.Type = tr;
		
		s = new DeclarationStatement(d.LexicalInfo);
		s.Declaration = d;
		s.Initializer = initializer;
		s.Modifier = m;
	}
	;

protected
expression_stmt returns [ExpressionStatement s]
	{
		s = null;
		Expression e = null;
	}:
	e=assignment_expression
	{
		s = new ExpressionStatement(e);
	}
	;	
protected
return_expression_stmt returns [ReturnStatement s]
	{
		s = null;
		Expression e = null;
		StatementModifier modifier = null;
	}:
	r:RETURN (e=array_or_expression)?
	(modifier=stmt_modifier)?
	{
		s = new ReturnStatement(ToLexicalInfo(r));
		s.Modifier = modifier;
		s.Expression = e;
	}
	;

protected
return_stmt returns [ReturnStatement s]
	{
		s = null;
		Expression e = null;
		StatementModifier modifier = null;
	}:
	r:RETURN 
		(
			(
				e=array_or_expression
				(
					(DO)=>method_invocation_block[e] |
					((modifier=stmt_modifier)? eos)
				)
			) |
			(
				e=callable_expression
			) |
			(AST)=>(
				e=ast_literal
			) |
			(
				(modifier=stmt_modifier)?
				eos
			)
		)
	{
		s = new ReturnStatement(ToLexicalInfo(r));
		s.Modifier = modifier;
		s.Expression = e;
	}
	;

protected
yield_stmt returns [YieldStatement s]
	{
		s = null;
		Expression e = null;
	}:
	yt:YIELD (e=array_or_expression)?
	{
		s = new YieldStatement(ToLexicalInfo(yt));
		s.Expression = e;
	}
	;

protected
break_stmt returns [BreakStatement s]
	{ s = null; }:
	b:BREAK
	{ s = new BreakStatement(ToLexicalInfo(b)); }
	;

protected
continue_stmt returns [Statement s]
	{ s = null; }:
	c:CONTINUE
	{ s = new ContinueStatement(ToLexicalInfo(c)); }
	;
	
protected
unless_stmt returns [UnlessStatement us]
	{
		us = null;
		Expression condition = null;
	}:
	u:UNLESS condition=expression
	{
		us = new UnlessStatement(ToLexicalInfo(u));
		us.Condition = condition;
	}
	compound_stmt[us.Block]
	;

protected
for_stmt returns [ForStatement fs]
	{
		fs = null;
		Expression iterator = null;
		DeclarationCollection declarations = null;
		Block body = null;
	}:
	f:FOR
	{
		fs = new ForStatement(ToLexicalInfo(f));
		declarations = fs.Declarations;
		body = fs.Block;
	}
		declaration_list[declarations] IN iterator=array_or_expression
		{ fs.Iterator = iterator; }
		compound_stmt[body]
	;
		
protected
while_stmt returns [WhileStatement ws]
	{
		ws = null;
		Expression e = null;
	}:
	w:WHILE e=expression
	{
		ws = new WhileStatement(ToLexicalInfo(w));
		ws.Condition = e;
	}
	compound_stmt[ws.Block]
	;
		
protected
given_stmt returns [GivenStatement gs]
	{
		gs = null;		
		Expression e = null;
		WhenClause wc = null;
	}:
	given:GIVEN e=expression
	{
		gs = new GivenStatement(ToLexicalInfo(given));
		gs.Expression = e;
	}
	begin
		(
			when:WHEN e=array_or_expression
			{
				wc = new WhenClause(ToLexicalInfo(when));
				wc.Condition = e;
				gs.WhenClauses.Add(wc);
			}				
				compound_stmt[wc.Block]
		)+
		(
			otherwise:OTHERWISE
			{
				gs.OtherwiseBlock = new Block(ToLexicalInfo(otherwise));
			}
			compound_stmt[gs.OtherwiseBlock]
		)?
	end[gs]
	;
		
protected
if_stmt returns [IfStatement returnValue]
	{
		returnValue = null;
		
		IfStatement s = null;
		Expression e = null;
		Block lastBlock = null;
	}:
	it:IF e=expression
	{
		returnValue = s = new IfStatement(ToLexicalInfo(it));
		s.Condition = e;
		lastBlock = s.TrueBlock = new Block();
	}
	begin block[s.TrueBlock.Statements]
	(
		ei:ELIF e=expression
		{
			s.FalseBlock = new Block();
			
			IfStatement elif = new IfStatement(ToLexicalInfo(ei));
			lastBlock = elif.TrueBlock = new Block();
			elif.Condition = e;
			
			s.FalseBlock.Add(elif);
			s = elif;
		}
		begin block[s.TrueBlock.Statements]
	)*
	(
		et:ELSE { lastBlock = s.FalseBlock = new Block(ToLexicalInfo(et)); }
		begin block[s.FalseBlock.Statements]
	)?
	end[lastBlock]
	;
		
protected
unpack_stmt returns [UnpackStatement s]
	{
		s = null;
		StatementModifier m = null;
	}:	
	s=unpack (m=stmt_modifier)? eos
	{
		s.Modifier = m;
	}
;		

protected
unpack returns [UnpackStatement s]
{
	Declaration d = null;
	s = new UnpackStatement();
	Expression e = null;
}:
	d=declaration COMMA { s.Declarations.Add(d); }
	(declaration_list[s.Declarations])?
	t:ASSIGN e=array_or_expression
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
	(COMMA d=declaration { dc.Add(d); })*
	;
		
protected
declaration returns [Declaration d]
	{
		d = null;
		TypeReference tr = null;
	}:
	id:ID (AS tr=type_reference)?
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
			c:COMMA { e = new ArrayLiteralExpression(ToLexicalInfo(c)); }
		) |
		(
			e=expression
			( options { greedy=true; }:
				t:COMMA
				{					
					tle = new ArrayLiteralExpression(e.LexicalInfo);
					tle.Items.Add(e);		
				}
				( options { greedy=true; }:
					e=expression { tle.Items.Add(e); }
					( options { greedy=true; }:
						COMMA
						e=expression { tle.Items.Add(e); }
					)*
					(COMMA)?
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
		
		ExtendedGeneratorExpression mge = null;
		GeneratorExpression ge = null;
	} :
	e=boolean_expression
		
	( options { greedy = true; } :
		f:FOR
		{
			ge = new GeneratorExpression(ToLexicalInfo(f));
			ge.Expression = e;
			e = ge;
		}
		generator_expression_body[ge]
		(
			f2:FOR
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
			generator_expression_body[ge]
		)*
	)?
;

generator_expression_body[GeneratorExpression ge]
{
	StatementModifier filter = null;
	Expression iterator = null;
	DeclarationCollection declarations = null == ge ? null : ge.Declarations;
}:
	declaration_list[declarations]
	IN
	iterator=boolean_expression { ge.Iterator = iterator; }
	(
		filter=stmt_modifier { ge.Filter = filter; }
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
		e=boolean_term
		(
			ot:OR
			r=boolean_term
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
	e=not_expression
	(
		at:AND
		r=not_expression
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
method_invocation_block[Expression mi]
	{
		Expression block = null;
	}:
	{IsMethodInvocationExpression(mi)}?
	block=callable_expression
	{
		((MethodInvocationExpression)mi).Arguments.Add(block);
	}
	;
	
ast_literal_expression returns [AstLiteralExpression e]
{
	e = null;
}:
	t:AST { e = new AstLiteralExpression(ToLexicalInfo(t)); }
	ast_literal_closure[e]
;
	
ast_literal returns [AstLiteralExpression e]
{
	e = null;
}:
	t:AST { e = new AstLiteralExpression(ToLexicalInfo(t)); }
	(ast_literal_block[e] | ast_literal_closure[e] eos)
;

type_definition_member_prediction:
	attributes
	modifiers
	(CLASS|INTERFACE|STRUCT|DEF|EVENT|(ID (AS|ASSIGN)))
;

ast_literal_block[AstLiteralExpression e]
{
	// TODO: either cache or construct these objects on demand
	TypeMemberCollection collection = new TypeMemberCollection();
	Block block = new Block();
	StatementCollection statements = block.Statements;
	Node node = null;
}:
	begin 
	(
		(type_definition_member_prediction)=>(type_definition_member[collection] { e.Node = collection[0]; }) |
		((stmt[statements])+ { e.Node = block.Statements.Count > 1 ? block : block.Statements[0]; })
	)
	end[e]
;

ast_literal_closure[AstLiteralExpression e]
{
	Node node = null;
}:
	LBRACE
	(
		(expression)=>node=expression { e.Node = node; } |
		node=ast_literal_closure_stmt { e.Node = node; }
	)
	RBRACE
;

ast_literal_closure_stmt returns [Statement s]
{
	s = null;
	StatementModifier modifier;
}:
	s=return_expression_stmt |
	(
		(
			(declaration COMMA)=>s=unpack |
			{IsValidMacroArgument(LA(2))}? s=closure_macro_stmt |
			s=raise_stmt |
			s=yield_stmt			
		)
		(modifier=stmt_modifier { s.Modifier = modifier; })?		
	)
;

protected
assignment_or_method_invocation_with_block_stmt returns [Statement stmt]
	{
		stmt = null;
		Expression lhs = null;
		Expression rhs = null;
		StatementModifier modifier = null;
		BinaryOperatorType binaryOperator = BinaryOperatorType.None;
		IToken token = null;
	}:
	lhs=slicing_expression
	(
		(DO)=>(
			method_invocation_block[lhs]
			{ stmt = new ExpressionStatement(lhs); }
		) |
		(
			(
			op:ASSIGN { token = op; binaryOperator = ParseAssignOperator(op.getText()); }
				(
					(DEF|DO)=>rhs=callable_expression |
					(
						rhs=array_or_expression
						(		
							(DO)=>method_invocation_block[rhs] |
							(modifier=stmt_modifier eos) |
							eos
						)					
					) |
					rhs=ast_literal
				)
			)
			{
				stmt = new ExpressionStatement(
						new BinaryExpression(ToLexicalInfo(token),
							binaryOperator,
							lhs, rhs));
				stmt.Modifier = modifier;
			}
		)
	)
	;
	
protected
not_expression returns [Expression e]
	{
		e = null;
	}
	:
	(
		(nt:NOT e=not_expression) |
		e=assignment_expression
	)
	{
		if (nt != null)
		{
			UnaryExpression ue = new UnaryExpression(ToLexicalInfo(nt));
			ue.Operator = UnaryOperatorType.LogicalNot;
			ue.Operand = e;
			e = ue;
		}
	}
	;
	
protected
assignment_expression returns [Expression e]
	{
		e = null;
		Expression r=null;
		IToken token = null;
		BinaryOperatorType binaryOperator = BinaryOperatorType.None;
	}:
	e=conditional_expression
	(
		options { greedy = true; }:
		
		(
			(
				op:ASSIGN {
					token = op;
					binaryOperator = ParseAssignOperator(op.getText());
				}
			) |
			(
				ipbo:INPLACE_BITWISE_OR	{
					token = ipbo;
					binaryOperator = BinaryOperatorType.InPlaceBitwiseOr;
				}
			) |
			(
				ipba:INPLACE_BITWISE_AND {
					token = ipba;
					binaryOperator = BinaryOperatorType.InPlaceBitwiseAnd;
				}
			) |
			(
				ipsl:INPLACE_SHIFT_LEFT {
					token = ipsl;
					binaryOperator = BinaryOperatorType.InPlaceShiftLeft;
				}
			) |
			(
				ipsr:INPLACE_SHIFT_RIGHT {
					token = ipsr;
					binaryOperator = BinaryOperatorType.InPlaceShiftRight;
				}
			)
		)
		r=assignment_expression
		{
			BinaryExpression be = new BinaryExpression(ToLexicalInfo(token));
			be.Operator = binaryOperator;
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
		IToken token = null;
		TypeReference tr = null;
	}:
	e=sum
	( options { greedy = true; } :
	 (
	  (
		 (
			(t:CMP_OPERATOR { op = ParseCmpOperator(t.getText()); token = t; } ) |
			(tgt:GREATER_THAN { op = BinaryOperatorType.GreaterThan; token = tgt; } ) |
			(tlt:LESS_THAN { op = BinaryOperatorType.LessThan; token = tlt; }) |
			(tnot:IS NOT { op = BinaryOperatorType.ReferenceInequality; token = tnot; }) |
			(tis:IS { op = BinaryOperatorType.ReferenceEquality; token = tis; }) |
			(tnint:NOT IN { op = BinaryOperatorType.NotMember; token = tnint; }) |
			(tin:IN { op = BinaryOperatorType.Member; token = tin; } )
		 )
		 r=sum
	  ) |
	  (
	  	tisa:ISA
		tr=type_reference
		{
			op = BinaryOperatorType.TypeTest;
			token = tisa;
			r = new TypeofExpression(tr.LexicalInfo, tr);
		}
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
		IToken op = null;
		BinaryOperatorType bOperator = BinaryOperatorType.None;
	}:
	e=term
	( options { greedy = true; } :
		(
			add:ADD { op=add; bOperator = BinaryOperatorType.Addition; } |
			sub:SUBTRACT { op=sub; bOperator = BinaryOperatorType.Subtraction; } |
			bitor:BITWISE_OR { op=bitor; bOperator = BinaryOperatorType.BitwiseOr; } |
			eo:EXCLUSIVE_OR { op=eo; bOperator = BinaryOperatorType.ExclusiveOr; }
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
		IToken token = null;
		BinaryOperatorType op = BinaryOperatorType.None; 
	}:
	e=factor
	( options { greedy = true; } :
	 	(
		 m:MULTIPLY { op=BinaryOperatorType.Multiply; token=m; } |
		 d:DIVISION { op=BinaryOperatorType.Division; token=d; } |
		 md:MODULUS { op=BinaryOperatorType.Modulus; token=md; } |
		 ba:BITWISE_AND { op=BinaryOperatorType.BitwiseAnd; token=ba; }
		 )
		r=factor
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
factor returns [Expression e]
	{
		e = null;
		Expression r = null;
		IToken token = null;
		BinaryOperatorType op = BinaryOperatorType.None;
	}:
	e=exponentiation
	(options { greedy = true; }:
		(
		shl:SHIFT_LEFT { op=BinaryOperatorType.ShiftLeft; token = shl; } |
		shr:SHIFT_RIGHT { op=BinaryOperatorType.ShiftRight; token = shr; }
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
		TypeReference tr = null;
	}:
	e=unary_expression
	(
		t:AS
		tr=type_reference
		{
			TryCastExpression ae = new TryCastExpression(ToLexicalInfo(t));
			ae.Target = e;
			ae.Type = tr;
			e = ae; 
		}
	)?
	
	( options { greedy = true; }:
	 	token:EXPONENTIATION
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
			IToken op = null;
			UnaryOperatorType uOperator = UnaryOperatorType.None;
	}: 
	(
		(
			(
				sub:SUBTRACT { op = sub; uOperator = UnaryOperatorType.UnaryNegation; } |
				inc:INCREMENT { op = inc; uOperator = UnaryOperatorType.Increment; } |
				dec:DECREMENT { op = dec; uOperator = UnaryOperatorType.Decrement; } |
				oc:ONES_COMPLEMENT { op = oc; uOperator = UnaryOperatorType.OnesComplement; }
			)
			e=unary_expression
		) |
		(
			e=slicing_expression
			(
				postinc:INCREMENT { op = postinc; uOperator = UnaryOperatorType.PostIncrement; } |
				postdec:DECREMENT { op = postdec; uOperator = UnaryOperatorType.PostDecrement; }
			)?
		)
	)
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
		(CHAR LPAREN)=>e=char_literal |
		e=reference_expression |
		e=paren_expression |
		e=cast_expression |
		e=typeof_expression
	)
	;
	
protected
char_literal returns [Expression e]
{
	e = null;
}:
	CHAR LPAREN t:SINGLE_QUOTED_STRING RPAREN
	{
		e = new CharLiteralExpression(ToLexicalInfo(t), t.getText());
	}
;
	
protected
cast_expression returns [Expression e]
	{
		e = null;
		TypeReference tr = null;
		Expression target = null;
	}:
	t:CAST LPAREN tr=type_reference COMMA target=expression RPAREN
	{
		e = new CastExpression(ToLexicalInfo(t), target, tr);
	}
	;
	
protected
typeof_expression returns [Expression e]
	{
		e = null;
		TypeReference tr = null;
	}:
	t:TYPEOF LPAREN tr=type_reference RPAREN
	{
		e = new TypeofExpression(ToLexicalInfo(t), tr);
	}
	;
	

protected
reference_expression returns [ReferenceExpression e]
{
	e = null;
	IToken t = null;
}:
	(
		id:ID { t = id; } |
		ch:CHAR { t = ch; }
	)
	{
		e = new ReferenceExpression(ToLexicalInfo(t));
		e.Name = t.getText();
	}	
;
	
protected
paren_expression returns [Expression e] { e = null; }:
    (LPAREN OF)=>e=typed_array
	| LPAREN e=array_or_expression RPAREN
;

protected
typed_array returns [Expression e]
	{
		e = null;
		ArrayLiteralExpression tle = null;
		TypeReference tr = null;
		Expression item = null;
	}:
	t:LPAREN
	OF tr=type_reference COLON
	{
		e = tle = new ArrayLiteralExpression(ToLexicalInfo(t));
		tle.Type = new ArrayTypeReference(tr.LexicalInfo, tr);
	}
	(
		COMMA
		|
		(
			item=expression { tle.Items.Add(item); }
			(
				COMMA
				item=expression { tle.Items.Add(item); }
			)*
		)
	)
	RPAREN
;
	
protected
method_invocation_with_block returns [Statement s]
	{
		s = null;
		MethodInvocationExpression mie = null;
		Expression block = null;
	}:
	RPAREN
	(
		block=callable_expression
		{
			mie.Arguments.Add(block);
		}
	)?
	;
	
protected
member returns [IToken name]
	{
		name = null;
	}:
	id:ID { name=id; } |
	set:SET { name=set; } |
	get:GET { name=get; } |
	t1:INTERNAL { name=t1; } |
	t2:PUBLIC { name=t2; } |
	t3:PROTECTED { name=t3; } |
	r:REF { name=r; }
	;
	
protected
slice[SlicingExpression se]
	{
		Expression begin = null;
		Expression end = null;
		Expression step = null;
	} :
	(
		( 
			// [:
			COLON { begin = OmittedExpression.Default; }
			(
				// [:end]
				end=expression
				|
				(
					// [::step]
					COLON { end = OmittedExpression.Default; }
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
			COLON
			(
				end=expression | { end = OmittedExpression.Default; } 
			)
			(
				COLON
				step=expression
			)?
		)?
	)
	{
	
		se.Indices.Add(new Slice(begin, end, step));
	}
	;
	
protected
slicing_expression returns [Expression e]
	{
		e = null;
		SlicingExpression se = null;
		MethodInvocationExpression mce = null;
		IToken memberName = null;
	} :
	e=atom
	( options { greedy=true; }:
		(
			lbrack:LBRACK
			{
				se = new SlicingExpression(ToLexicalInfo(lbrack));				
				se.Target = e;
				e = se;
			}
			slice[se] (COMMA slice[se])*
			RBRACK
		)
		|
		(
			DOT memberName=member
				{
					MemberReferenceExpression mre = new MemberReferenceExpression(ToLexicalInfo(memberName));
					mre.Target = e;
					mre.Name = memberName.getText();
					e = mre;
				}
		)
		|
		(
			lparen:LPAREN
				{
					mce = new MethodInvocationExpression(ToLexicalInfo(lparen));
					mce.Target = e;
					e = mce;
				}
				(
					method_invocation_argument[mce] 
					(
						COMMA
						method_invocation_argument[mce]
					)*
				)?
			RPAREN
		)
	)*
	;
	
protected
method_invocation_argument[MethodInvocationExpression mie]
	{
		Expression arg = null;
	}:
	(
		t:MULTIPLY arg=expression
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
	) |
	argument[mie]
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
		(hash_literal_test)=>e=hash_literal |
		e=closure_expression |
		e=ast_literal_expression |
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
	t:SELF { e = new SelfLiteralExpression(ToLexicalInfo(t)); }
	;
	
protected
super_literal returns [SuperLiteralExpression e] { e = null; }:
	t:SUPER { e = new SuperLiteralExpression(ToLexicalInfo(t)); }
	;
		
protected
null_literal returns [NullLiteralExpression e] { e = null; }:
	t:NULL { e = new NullLiteralExpression(ToLexicalInfo(t)); }
	;
		
protected
bool_literal returns [BoolLiteralExpression e] { e = null; }:
	t:TRUE
	{
		e = new BoolLiteralExpression(ToLexicalInfo(t));
		e.Value = true;
	} |
	f:FALSE
	{
		e = new BoolLiteralExpression(ToLexicalInfo(f));
		e.Value = false;
	}
	;

protected
integer_literal returns [IntegerLiteralExpression e] 
	{
		e = null;
		string val;
	} :
	(neg:SUBTRACT)?
	(
		i:INT
		{
			val = i.getText();
			if (neg != null) val = neg.getText() + val;
			e = ParseIntegerLiteralExpression(i, val, false);
		}
		|
		l:LONG
		{
			val = l.getText();
			val = val.Substring(0, val.Length-1);
			if (neg != null) val = neg.getText() + val;
			e = ParseIntegerLiteralExpression(l, val, true);
		}
	)
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
		
		ESEPARATOR		
		param=expression { if (null != param) { e.Expressions.Add(param); } }
		ESEPARATOR
	)*
	;
	

protected
list_literal returns [Expression e]
	{
		e = null;
		ListLiteralExpression lle = null;
		Expression item = null;
	}:
	lbrack:LBRACK
	(
		(
			item=expression
			(
				{
					e = lle = new ListLiteralExpression(ToLexicalInfo(lbrack));
					lle.Items.Add(item);
				}
				(  options { greedy = true; } :
					COMMA item=expression { lle.Items.Add(item); }
				)*
			)
			(COMMA)?
		)
		|
		{ e = new ListLiteralExpression(ToLexicalInfo(lbrack)); }
	)
	RBRACK
	;
	
protected
hash_literal_test:
	LBRACE	(RBRACE|(expression COLON))
	;
		
protected
hash_literal returns [HashLiteralExpression dle]
	{
		dle = null;
		ExpressionPair pair = null;
	}:
	lbrace:LBRACE { dle = new HashLiteralExpression(ToLexicalInfo(lbrace)); }
	(
		pair=expression_pair			
		{ dle.Items.Add(pair); }
		(
			COMMA
			pair=expression_pair
			{ dle.Items.Add(pair); }
		)*
	)?
	RBRACE
	;
		
protected
expression_pair returns [ExpressionPair ep]
	{
		ep = null;
		Expression key = null;
		Expression value = null;
	}:
	key=expression t:COLON value=expression
	{ ep = new ExpressionPair(ToLexicalInfo(t), key, value); }
	;
		
protected
re_literal returns [RELiteralExpression re] { re = null; }:
	value:RE_LITERAL
	{ re = new RELiteralExpression(ToLexicalInfo(value), value.getText()); }
	;
	
protected
double_literal returns [DoubleLiteralExpression rle]
	{
		rle = null;
		string val;
	}:
	(neg:SUBTRACT)?
	value:DOUBLE
	{
		val = value.getText();
		if (neg != null) val = neg.getText() + val;
		rle = new DoubleLiteralExpression(ToLexicalInfo(value), ParseDouble(val));
	}
	|
	single:FLOAT
	{
		val = single.getText();
		val = val.Substring(0, val.Length-1);
		if (neg != null) val = neg.getText() + val;
		rle = new DoubleLiteralExpression(ToLexicalInfo(single), ParseDouble(val, true), true);
	}
	;
	
protected
timespan_literal returns [TimeSpanLiteralExpression tsle] { tsle = null; }:
	(neg:SUBTRACT)?
	value:TIMESPAN
	{
		string val = value.getText();
		if (neg != null) val = neg.getText() + val;
		tsle = new TimeSpanLiteralExpression(ToLexicalInfo(value), ParseTimeSpan(val)); 
	}
	;

protected
expression_list[ExpressionCollection ec]
	{
		Expression e = null;
	} :
	(
		e=expression { ec.Add(e); }
		(
			COMMA
			e=expression { ec.Add(e); }
		)*
	)?
	;
	
protected
argument_list[INodeWithArguments node]:
	(
		argument[node] 
		(
			COMMA
			argument[node]
		)*
	)?
	;
	
protected
argument[INodeWithArguments node]
	{		
		Expression value = null;
	}:
	(ID COLON)=>(
		id:ID colon:COLON value=expression
		{
			node.NamedArguments.Add(
				new ExpressionPair(
					ToLexicalInfo(colon),
					new ReferenceExpression(ToLexicalInfo(id), id.getText()),
					value));
		}
	) |
	(
		value=expression
		{ if (null != value) { node.Arguments.Add(value); } }
	)
	;

protected
identifier returns [IToken value]
	{
		value = null; _sbuilder.Length = 0;
		IToken id2 = null;
	}:
	id1:ID
	{					
		_sbuilder.Append(id1.getText());
		value = id1;
	}				
	( options { greedy = true; } :
		DOT
		id2=member
		{ _sbuilder.Append('.'); _sbuilder.Append(id2.getText()); }
	)*
	{ value.setText(_sbuilder.ToString()); }
	;	 
{
using Boo.Lang.Parser.Util;
}
class WSABooLexer extends Lexer;
options
{
	testLiterals = false;
	exportVocab = WSABoo;	
	k = 3;
	charVocabulary='\u0003'..'\uFFFE';
	caseSensitiveLiterals=true;
	// without inlining some bitset tests, ANTLR couldn't do unicode;
	// They need to make ANTLR generate smaller bitsets;
	codeGenBitsetTestThreshold=20;
}
{
	protected int _skipWhitespaceRegion = 0;
	
	TokenStreamRecorder _erecorder;
	
	antlr.TokenStreamSelector _selector;
	
	internal void Initialize(antlr.TokenStreamSelector selector, int tabSize, antlr.TokenCreator tokenCreator)
	{
		setTabSize(tabSize);
		setTokenCreator(tokenCreator);
		
		_selector = selector;
		_erecorder = new TokenStreamRecorder(selector);
	}
	
	internal antlr.TokenStream CreateExpressionLexer()
	{
		WSABooExpressionLexer lexer = new WSABooExpressionLexer(getInputState());
		lexer.setTabSize(getTabSize());
		lexer.setTokenCreator(tokenCreator);
		return lexer;
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

	void Enqueue(antlr.IToken token, string text)
	{
		token.setText(text);
		_erecorder.Enqueue(makeESEPARATOR());
		_erecorder.Enqueue(token);
		_erecorder.Enqueue(makeESEPARATOR());
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
}

ID options { testLiterals = true; }:
		ID_LETTER (ID_LETTER | DIGIT)*
	;
	
LINE_CONTINUATION:
	'\\'! NEWLINE
	{ $setType(Token.SKIP); }
	;
	
INT : 
  	("0x"(HEXDIGIT)+)(('l' | 'L') { $setType(LONG); })? |
  	DIGIT_GROUP
 	(('e'|'E')('+'|'-')? DIGIT_GROUP)?
  	(
  		('l' | 'L') { $setType(LONG); } |
		(('f' | 'F') { $setType(FLOAT); }) |
  		(
 			(
 				{WSABooLexer.IsDigit(LA(2))}? 
 				(
 					'.' REVERSE_DIGIT_GROUP
 					(('e'|'E')('+'|'-')? DIGIT_GROUP)?
 				)
				(
					(('f' | 'F') { $setType(FLOAT); }) |
					{ $setType(DOUBLE); }
				)
 			)?
  			(("ms" | 's' | 'm' | 'h' | 'd') { $setType(TIMESPAN); })?
  		)
  	)
;
  
DOT : '.' 
	(
		REVERSE_DIGIT_GROUP (('e'|'E')('+'|'-')? DIGIT_GROUP)?
		(
			(('f' | 'F')  { $setType(FLOAT); }) |
			(("ms" | 's' | 'm' | 'h' | 'd') { $setType(TIMESPAN); }) |
			{$setType(DOUBLE);}
		)
	)?
;

COLON : ':';

BITWISE_OR: '|' ('=' { $setType(INPLACE_BITWISE_OR); })?;

BITWISE_AND: '&' ('=' { $setType(INPLACE_BITWISE_AND); })?;

EXCLUSIVE_OR: '^' ('=' { $setType(ASSIGN); })?;

LPAREN : '(' { EnterSkipWhitespaceRegion(); };
	
RPAREN : ')' { LeaveSkipWhitespaceRegion(); };

protected
ASSEMBLY_ATTRIBUTE_BEGIN: "assembly:";

LBRACK : '[' { EnterSkipWhitespaceRegion(); }
	(
		("assembly:")=> "assembly:" { $setType(ASSEMBLY_ATTRIBUTE_BEGIN); } |
	)
	;

RBRACK : ']' { LeaveSkipWhitespaceRegion(); };

LBRACE : '{' { EnterSkipWhitespaceRegion(); };
	
RBRACE : '}' { LeaveSkipWhitespaceRegion(); };

INCREMENT: "++";

DECREMENT: "--";

ADD: ('+') ('=' { $setType(ASSIGN); })?;

SUBTRACT: ('-') ('=' { $setType(ASSIGN); })?;

MODULUS: '%';

MULTIPLY: '*' ('=' { $setType(ASSIGN); })?;

EXPONENTIATION: "**";

DIVISION: 
	("/*")=> ML_COMMENT { $setType(Token.SKIP); } |
	(RE_LITERAL)=> RE_LITERAL { $setType(RE_LITERAL); } |	
	'/' (
		('/' (~('\r'|'\n'))* { $setType(Token.SKIP); }) |			
			('=' { $setType(ASSIGN); }) |
		)
	;

LESS_THAN: '<';

SHIFT_LEFT: "<<";

INPLACE_SHIFT_LEFT: "<<=";

GREATER_THAN: '>';

SHIFT_RIGHT: ">>";

INPLACE_SHIFT_RIGHT: ">>=";

ONES_COMPLEMENT: '~';

CMP_OPERATOR :  "<=" | ">=" | "!~" | "!=";

ASSIGN : '=' ( ('=' | '~') { $setType(CMP_OPERATOR); } )?;

COMMA : ',';

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
		'\t' { tab(); } |
		'\f'
	)+
	{
		$setType(Token.SKIP);
	}
	;
		
EOS: ';';

X_RE_LITERAL: '@'!'/' (X_RE_CHAR)+ '/' { $setType(RE_LITERAL); };

NEWLINE:
	(
		(
			'\n'
			|
			(
			'\r' ('\n')?
			)
		)
		{ newline(); }
	)+
	{
		if (SkipWhitespace)
		{
			$setType(Token.SKIP);
		}
		else
		{
			$setType(EOS);
		}
	}
;
		
protected
ESCAPED_EXPRESSION : "${"!
	{			
		_erecorder.Enqueue(makeESEPARATOR());
		if (0 == _erecorder.RecordUntil(CreateExpressionLexer(), RBRACE, LBRACE))
		{	
			_erecorder.Dequeue();			
		}
		else
		{
			_erecorder.Enqueue(makeESEPARATOR());
		}
		refresh();
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
				( 'a'! {text.Length = _begin; text.Append("\a"); }) |
				( 'b'! {text.Length = _begin; text.Append("\b"); }) |
				( 'f'! {text.Length = _begin; text.Append("\f"); }) |
				( '0'! {text.Length = _begin; text.Append("\0"); }) |
				( 'u'!
					HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT
					{
						char ch = (char)int.Parse(text.ToString(_begin, 4), System.Globalization.NumberStyles.HexNumber);
						text.Length = _begin;
						text.Append(ch);
					}
				) |
				( '\\'! {$setText("\\"); });
				

protected
RE_LITERAL : '/' (RE_CHAR)+ '/';

protected
RE_CHAR : RE_ESC | ~('/' | '\\' | '\r' | '\n' | ' ' | '\t' );

protected
X_RE_CHAR: RE_CHAR | ' ' | '\t';

protected
RE_ESC : '\\' (				
				'+' |
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
				']' |
				'{' |
				'}' |
	
	// character scapes
	// ms-help://MS.NETFrameworkSDKv1.1/cpgenref/html/cpconcharacterescapes.htm
	
				'a' |
				'b' |
				('c' 'A'..'Z') |
				't' |
				'r' |
				'v' |
				'f' |
				'n' |
				'e' |
				(DIGIT)+ |
				('x' HEXDIGIT HEXDIGIT) |
				('u' HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT) |
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
				'k'			
			 )
			 ;
			 
protected
DIGIT_GROUP : DIGIT (('_'! DIGIT DIGIT DIGIT) | DIGIT)*;

protected
REVERSE_DIGIT_GROUP : (DIGIT DIGIT DIGIT ({WSABooLexer.IsDigit(LA(2))}? '_'!)? | DIGIT)+;

protected
ID_LETTER : ('_' | 'a'..'z' | 'A'..'Z' );

protected
DIGIT : '0'..'9';

protected
HEXDIGIT : ('a'..'f' | 'A'..'F' | '0'..'9');
