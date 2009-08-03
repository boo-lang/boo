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

public delegate void ParserErrorHandler(antlr.RecognitionException x);

abstract
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
	INDENT;
	DEDENT;
	ELIST; // expression list
	DLIST; // declaration list
	ESEPARATOR; // expression separator (imaginary token)
	EOL;
	ABSTRACT="abstract";
	AND="and";
	AS="as";
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
	OF="of";
	OR="or";
	OVERRIDE="override";	
	PASS="pass";
	NAMESPACE="namespace";
	PARTIAL="partial";
	PUBLIC="public";
	PROTECTED="protected";
	PRIVATE="private";
	RAISE="raise";
	REF="ref";
	RETURN="return";
	SET="set";	
	SELF="self";
	SUPER="super";
	STATIC="static";
	STRUCT="struct";
	THEN="then";
	TRY="try";
	TRANSIENT="transient";
	TRUE="true";
	TYPEOF="typeof";
	UNLESS="unless";
	VIRTUAL="virtual";
	WHILE="while";
	YIELD="yield";
}

{		
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
	
	protected abstract Module NewQuasiquoteModule(LexicalInfo li);

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
	
	private MemberReferenceExpression MemberReferenceForToken(Expression target, IToken memberName)
	{
		MemberReferenceExpression mre = new MemberReferenceExpression(ToLexicalInfo(memberName));
		mre.Target = target;
		mre.Name = memberName.getText();
		return mre;	
	}
	
	protected abstract void EmitIndexedPropertyDeprecationWarning(Property deprecated);
}

protected
start[CompileUnit cu] returns [Module module]
{
	module = new Module();		
	module.LexicalInfo = new LexicalInfo(getFilename(), 1, 1);
	
	cu.Modules.Add(module);
}:
	parse_module[module]
	EOF
;
	
protected
parse_module[Module module]
{
}:
	(eos)?
	docstring[module]
	(eos)?
	(namespace_directive[module])?
	(import_directive[module])*
	(
		(ID (expression)?)=>{IsValidMacroArgument(LA(2))}? module_macro[module] |
		type_member[module.Members]
	)*	
	globals[module]
	(assembly_attribute[module] eos)*
;

protected
module_macro[Module module]
{
	MacroStatement s = null;
}:
	s=macro_stmt { module.Globals.Add(s); }
;
			
protected docstring[Node node]:
	(
		doc:TRIPLE_QUOTED_STRING { node.Documentation = DocStringFormatter.Format(doc.getText()); }
		(eos)?
	)?
	;
			
protected
eos : (options { greedy = true; }: (EOL|EOS))+;

protected
import_directive[Module container]
{
	Import node = null;
}: 
	node=import_directive_
	{
		container.Imports.Add(node);
	}
	eos
;

protected
import_directive_ returns [Import returnValue]
{
	IToken id;
	returnValue = null;
}:
	IMPORT id=identifier
	{
		returnValue = new Import(ToLexicalInfo(id));
		returnValue.Namespace = id.getText();
	}
	(
		FROM
			(
					id=identifier |
					dqs:DOUBLE_QUOTED_STRING { id=dqs; } |
					sqs:SINGLE_QUOTED_STRING { id=sqs; }
			)
		{
			returnValue.AssemblyReference = new ReferenceExpression(ToLexicalInfo(id));
			returnValue.AssemblyReference.Name = id.getText();
		}				
	)?
	(
		AS alias:ID
		{
			returnValue.Alias = new ReferenceExpression(ToLexicalInfo(alias));
			returnValue.Alias.Name = alias.getText();
		}
	)?
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
		GenericParameterDeclarationCollection genericParameters = null;
	}:
	CALLABLE id:ID
	{
		cd = new CallableDefinition(ToLexicalInfo(id));
		cd.Name = id.getText();
		cd.Modifiers = _modifiers;
		AddAttributes(cd.Attributes);
		container.Add(cd);
		genericParameters = cd.GenericParameters;
	}
	(LBRACK (OF)? generic_parameter_declaration_list[genericParameters] RBRACK)?
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
		Expression initializer = null;
	}: 
	attributes
	id:ID (ASSIGN initializer=simple_initializer)?
	{
		em = new EnumMember(ToLexicalInfo(id));
		em.Name = id.getText();
		em.Initializer = initializer;
		AddAttributes(em.Attributes);
		container.Members.Add(em);
	}
	eos
	docstring[em]
	;

protected
attributes
{
}
:
	{ _attributes.Clear(); }
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
		(eos)?
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
		GenericParameterDeclarationCollection genericParameters = null;
		Expression nameSplice = null;
	}:
	(
		CLASS { td = new ClassDefinition(); } |
		STRUCT { td = new StructDefinition(); }
	)
	(
		id:ID
		| begin:SPLICE_BEGIN nameSplice=atom
	)				
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
	(LBRACK (OF)? generic_parameter_declaration_list[genericParameters] RBRACK)?
	(base_types[baseTypes])?
	begin_with_doc[td]					
	(
		(PASS eos) |
		(
			(eos)?
			(type_definition_member[members])+			
		)
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
		GenericParameterDeclarationCollection genericParameters = null;
		Expression nameSplice = null;
	} :
	INTERFACE (id:ID | (begin:SPLICE_BEGIN nameSplice=atom))
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
	(LBRACK (OF)? generic_parameter_declaration_list[genericParameters] RBRACK)?
	(base_types[itf.BaseTypes])?
	begin_with_doc[itf]
	(
		(PASS eos) |
		(
			attributes
			(
				interface_method[members] |
				event_declaration[members] |
				interface_property[members]
			)
		)+
	)
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
		Expression nameSplice = null;
	}: 
	DEF (id:ID | (begin:SPLICE_BEGIN nameSplice=atom))
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
	(
		(
			LBRACK (OF)? generic_parameter_declaration_list[m.GenericParameters] RBRACK
		)
		|
		(
			OF generic_parameter_declaration[m.GenericParameters]
		)
	)?
	LPAREN parameter_declaration_list[m.Parameters] RPAREN
	(AS rt=type_reference { m.ReturnType=rt; })?			
	(
		(eos docstring[m]) | (empty_block[m] (eos)?)
	)
	;

protected
interface_property [TypeMemberCollection container]
        {
		IToken id = null;
                Property p = null;
                TypeReference tr = null;
                ParameterDeclarationCollection parameters = null;
        }:
        (id1:ID {id=id1;} | s:SELF {id=s;})
        {
                p = new Property(ToLexicalInfo(id));
                p.Name = id.getText();
                AddAttributes(p.Attributes);
                container.Add(p);
                parameters = p.Parameters;
        }
        ((LBRACK|LPAREN) parameter_declaration_list[parameters] (RBRACK|RPAREN))?
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
			PASS eos
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
		GenericParameterDeclarationCollection genericParameters = null;
		Block body = null;
		StatementCollection statements = null;
		Expression nameSplice = null;
		TypeMember typeMember = null;
	}: 
	t:DEF
	(		
		(
			(
				(emi=explicit_member_info)?
				(id:ID | spliceBegin:SPLICE_BEGIN nameSplice=atom)
			) {
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
		)
		| c:CONSTRUCTOR { typeMember = m = new Constructor(ToLexicalInfo(c)); }
		| d:DESTRUCTOR { typeMember = m = new Destructor(ToLexicalInfo(d)); }
	)
	{
		m.Modifiers = _modifiers;
		AddAttributes(m.Attributes);
		parameters = m.Parameters;
		genericParameters = m.GenericParameters;
		body = m.Body;
		statements = body.Statements;
	}
	(LBRACK (OF)? generic_parameter_declaration_list[genericParameters] RBRACK)?
	LPAREN parameter_declaration_list[parameters] RPAREN
	attributes { AddAttributes(m.ReturnTypeAttributes); }
	(AS rt=type_reference { m.ReturnType = rt; })?
	begin_block_with_doc[m, body]
		block[statements]
	end[body]
	{ 
		container.Add(typeMember);
	}
;

protected
property_header:	
	((ID|SELF|(SPLICE_BEGIN atom)) (DOT ID)*)
	(
		LBRACK |
		(
			(LPAREN parameter_declaration_list[null] RPAREN)?
			(AS type_reference)?
			begin_with_doc[null]
			attributes
			modifiers
			(GET|SET)
		)
	)
	;
	
protected
field_or_property [TypeMemberCollection container]
{
	IToken id = null;
	TypeMember tm = null;
	TypeReference tr = null;
	Property p = null;
	Field field = null;
	ExplicitMemberInfo emi = null;
	Expression initializer = null;
	ParameterDeclarationCollection parameters = null;
	Expression nameSplice = null;
}:
(	
	(property_header)=>(
		(emi=explicit_member_info)?
		(id1:ID {id=id1;}
		| begin1:SPLICE_BEGIN nameSplice=atom {id=begin1;}
		| s:SELF {id=s;})
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
				tm = p;
			}
			(
				(lparen:LPAREN parameter_declaration_list[parameters] RPAREN { EmitIndexedPropertyDeprecationWarning(p); })
				| (LBRACK parameter_declaration_list[parameters] RBRACK)
			)?
			(AS tr=type_reference)?
			{							
				p.Type = tr;
				p.Modifiers = _modifiers;
			}		
			begin_with_doc[p]
				(property_accessor[p])+
			end[p]
		)
	)
	|
	(ID expression_list[null] (eos | begin_with_doc[null]))
		=> tm=member_macro
	|	
	(
		(id2:ID | begin2:SPLICE_BEGIN nameSplice=atom)
		{
			IToken token = id2 ?? begin2;
			field = new Field(ToLexicalInfo(token));
			field.Name = token.getText();
			field.Modifiers = _modifiers;
			AddAttributes(field.Attributes);
			tm = field;
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
)
{
	if (null != nameSplice) {
		tm = new SpliceTypeMember(tm, nameSplice);
	}
	container.Add(tm);
}
;

protected
member_macro returns [TypeMember result]
{
	MacroStatement s = null;
	result = null;
}:
	s=macro_stmt
	{
		result = new StatementTypeMember(s);
		result.Modifiers = _modifiers;
		AddAttributes(result.Attributes);
	}
;

	
declaration_initializer returns [Expression e]
{
	e = null;
}:
	(slicing_expression (COLON|DO|DEF))=>(e=slicing_expression e=method_invocation_block[e]) |
	(e=array_or_expression eos) |
	(e=callable_expression)
;

simple_initializer returns [Expression e]
{
	e = null;
}:
	e=array_or_expression |
	e=callable_expression
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
		{ null != p && null == p.Getter }?
		(
			gt:GET
			{
				p.Getter = m = new Method(ToLexicalInfo(gt));		
				m.Name = "get";
			}
		)
		|
		{ null != p && null == p.Setter }?
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
	(eos)?
	(stmt[container.Globals.Statements])*
	;
	
protected
block[StatementCollection container]:
	(eos)?
	(
		(PASS eos) |
		(			
			stmt[container]
		)+
	)
	; 
	
protected
modifiers
{
}:
	{ _modifiers = TypeMemberModifiers.None; }
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
	{ c.HasParamArray = variableArguments; }
	;

protected
parameter_declaration[ParameterDeclarationCollection c]
	returns [bool variableArguments]
	{		
		IToken id = null;
		TypeReference tr = null;
		ParameterModifiers pm = ParameterModifiers.None;
		variableArguments = false;
		Expression nameSplice = null;
	}: 
	attributes
	(
		(
			MULTIPLY { variableArguments=true; }
			(id1:ID { id = id1; }
			| begin1:SPLICE_BEGIN nameSplice=atom { id = begin1; })
			(AS tr=array_type_reference)?
		)
		|
		(
			(pm=parameter_modifier)?
			(id2:ID { id = id2; }
			| begin2:SPLICE_BEGIN nameSplice=atom { id = begin2; })
			(AS tr=type_reference)?
		)
	)
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
generic_parameter_declaration_list[GenericParameterDeclarationCollection c]:
	generic_parameter_declaration[c]
	(
		COMMA generic_parameter_declaration[c]
	)*
	;

protected 
generic_parameter_declaration[GenericParameterDeclarationCollection c]
	{
		GenericParameterDeclaration gpd = null;
	}:
	id:ID 
	{
		gpd = new GenericParameterDeclaration(ToLexicalInfo(id));
		gpd.Name = id.getText();
		c.Add(gpd);
	}
	(LPAREN generic_parameter_constraints[gpd] RPAREN)?
	;
	
	
protected 
generic_parameter_constraints[GenericParameterDeclaration gpd]
	{
		TypeReference tr = null;
	}:
	(
		CLASS
		{
			gpd.Constraints |= GenericParameterConstraints.ReferenceType;
		}
		|
		STRUCT
		{
			gpd.Constraints |= GenericParameterConstraints.ValueType;
		}
		|
		CONSTRUCTOR
		{
			gpd.Constraints |= GenericParameterConstraints.Constructable;
		}
		|
		tr=type_reference
		{
			gpd.BaseTypes.Add(tr);
		}
	) 
	(COMMA generic_parameter_constraints[gpd])?
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
type_reference_list [TypeReferenceCollection container]
	{
		TypeReference tr = null;
	}:
	tr=type_reference { container.Add(tr); }
	(options { greedy=true; }:
		COMMA tr=type_reference { container.Add(tr); }
	)*
;

protected
splice_type_reference returns [SpliceTypeReference tr]
{
	tr = null;
	Expression e = null;
}:
	begin:SPLICE_BEGIN e=atom
	{
		tr = new SpliceTypeReference(ToLexicalInfo(begin), e);
	}
;

protected
type_reference returns [TypeReference tr]
	{
		tr = null;
		IToken id = null;
		TypeReferenceCollection arguments = null;
		GenericTypeDefinitionReference gtdr = null;
	}: 
	tr=splice_type_reference
	|
	tr=array_type_reference
	|
	(CALLABLE LPAREN)=>(tr=callable_type_reference)
	|
	(
		id=type_name
		(
			(
				LBRACK (OF)? 
				(
					(
						MULTIPLY
						{
							gtdr = new GenericTypeDefinitionReference(ToLexicalInfo(id));
							gtdr.Name = id.getText();
							gtdr.GenericPlaceholders = 1;
							tr = gtdr;										
						}
						( 
							COMMA MULTIPLY
							{
								gtdr.GenericPlaceholders++;
							}
						)*
						RBRACK
					)
					|
					(
						{
							GenericTypeReference gtr = new GenericTypeReference(ToLexicalInfo(id), id.getText());
							arguments = gtr.GenericArguments;
							tr = gtr;
						}
						type_reference_list[arguments]
						RBRACK
					)
				)
			)
			|
			(
				OF MULTIPLY
				{
					gtdr = new GenericTypeDefinitionReference(ToLexicalInfo(id));
					gtdr.Name = id.getText();
					gtdr.GenericPlaceholders = 1;
					tr = gtdr;
				}
			)
			|
			(
				OF tr=type_reference
				{
					GenericTypeReference gtr = new GenericTypeReference(ToLexicalInfo(id), id.getText());
					gtr.GenericArguments.Add(tr);
					tr = gtr;
				}
			)
			|
			{
				SimpleTypeReference str = new SimpleTypeReference(ToLexicalInfo(id));
				str.Name = id.getText();
				tr = str;
			}
		)
		(NULLABLE_SUFFIX {
				GenericTypeReference ntr = new GenericTypeReference(tr.LexicalInfo, "System.Nullable");
				ntr.GenericArguments.Add(tr);
				tr = ntr;
			}
		)?
		(MULTIPLY {
				GenericTypeReference etr = new GenericTypeReference(tr.LexicalInfo, "System.Collections.Generic.IEnumerable");
				etr.GenericArguments.Add(tr);
				tr = etr;
			}
		)?
	)
	;
	
protected
type_name returns [IToken id]
	{
		id = null;
	}:
	id=identifier |	c:CALLABLE { id=c; } | ch:CHAR { id=ch; }
	;

protected
begin: COLON INDENT;

protected
begin_with_doc[Node node]: COLON (eos docstring[node])? INDENT;
	
protected
begin_block_with_doc[Node node, Block block]:
	COLON (eos docstring[node])?
	begin:INDENT
	{
		block.LexicalInfo = ToLexicalInfo(begin);
	}
	;

protected
end[Node node] :
	t:DEDENT { node.EndSourceLocation = SourceLocationFactory.ToSourceLocation(t); }
	(eos)?
	;

protected
compound_stmt[Block b]
{
	StatementCollection statements = b != null ? b.Statements : null;
	IToken lastEOL = null;
}:
		(
			COLON
			(
				PASS
				|
				(
				simple_stmt[statements]
				(options { greedy = true; }: EOS (simple_stmt[statements])?)*
				)
			)
			(options { greedy = true; }: eolToken:EOL { lastEOL = eolToken; })+
			{ b.EndSourceLocation = SourceLocationFactory.ToSourceLocation(lastEOL); }
		) |
		(
			COLON begin:INDENT
			{
				b.LexicalInfo = ToLexicalInfo(begin);
			}
			block[statements]
			end[b]
		)
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
		compound_stmt[macro.Body] { macro.Annotate("compound"); } |
		eos |
		modifier=stmt_modifier eos { macro.Modifier = modifier; } |
		(
			begin_with_doc[macro] 
				block[macro.Body.Statements]
			end[macro.Body] { macro.Annotate("compound" ); }
		) 
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
nested_function returns [Statement stmt]
{
	stmt = null;
	BlockExpression be = null;
	Block body = null;
	TypeReference rt = null;
}:
	def:DEF id:ID
	{
		be = new BlockExpression(ToLexicalInfo(id));
		body = be.Body;
	}
	(
		LPAREN parameter_declaration_list[be.Parameters] RPAREN
		(AS rt=type_reference { be.ReturnType = rt; })?
	)?
		compound_stmt[body]		
	{
		string name = id.getText();
		stmt = new DeclarationStatement(
					ToLexicalInfo(def),
					new Declaration(
						ToLexicalInfo(id),
						name),
					be);
		be["ClosureName"] = name;
	}
;
	

protected
stmt [StatementCollection container]
	{
		Statement s = null;
		StatementModifier m = null;
	}:		
	(
		s=nested_function |
		s=for_stmt |
		s=while_stmt |
		s=if_stmt |
		s=unless_stmt |
		s=try_stmt |
		(ID (expression)?)=>{IsValidMacroArgument(LA(2))}? s=macro_stmt |
		(slicing_expression (ASSIGN|(COLON|DO|DEF)))=> s=assignment_or_method_invocation_with_block_stmt |
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
simple_stmt [StatementCollection container]
	{
		Statement s = null;
		_compact = true;
	}:
	(
		{IsValidMacroArgument(LA(2))}? s=closure_macro_stmt |
		(slicing_expression ASSIGN)=> s=assignment_or_method_invocation |
		s=return_expression_stmt |
		(declaration COMMA)=> s=unpack |
		s=declaration_stmt |
		(		
			(
				s=goto_stmt |
				s=label_stmt |
				s=yield_stmt |
				s=break_stmt |
				s=continue_stmt |				
				s=raise_stmt |
				s=expression_stmt				
			)
		)
	)
	{
		if (null != s)
		{
			container.Add(s);
		}
	}
	{_compact=false;}
	;

protected
stmt_modifier returns [StatementModifier m]
	{
		m = null;
		Expression e = null;
		IToken t = null;
		StatementModifierType type = StatementModifierType.None;
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
				{IsValidClosureMacroArgument(LA(2))}? stmt=closure_macro_stmt | 
				stmt=closure_expression_stmt |
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
closure_expression_stmt returns [Statement s]
{
	s = null;
	Expression e = null;
}:
	e=array_or_expression
	{ s = new ExpressionStatement(e); }
;	
	
protected
closure_expression returns [Expression e]
	{
		e = null;
		BlockExpression cbe = null;
		ParameterDeclarationCollection parameters = null;
		Block body = null;
	}:
	anchorBegin:LBRACE
		{
			e = cbe = new BlockExpression(ToLexicalInfo(anchorBegin));
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
	{
		body.EndSourceLocation = SourceLocationFactory.ToEndSourceLocation(anchorEnd);
	}
;
	
protected
callable_expression returns [Expression e]
{
	e = null;
	Block body = null;
	BlockExpression cbe = null;
	TypeReference rt = null;
	IToken anchor = null;
}:
	(COLON)=>(
		{ body = new Block(); }
		compound_stmt[body]
		{ e = new BlockExpression(body.LexicalInfo, body); }
	)
	|(
		(
			(doAnchor:DO { anchor = doAnchor; }) |
			(defAnchor:DEF { anchor = defAnchor; })
		)
		{
			e = cbe = new BlockExpression(ToLexicalInfo(anchor));
			body = cbe.Body;
		}
		(
			LPAREN parameter_declaration_list[cbe.Parameters] RPAREN
			(AS rt=type_reference { cbe.ReturnType = rt; })?
		)?
			compound_stmt[body]
	)
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
	(EXCEPT|FAILURE|ENSURE)=>
	(
		exception_handler[s]
	)*
	(
		ftoken:FAILURE { eblock = new Block(ToLexicalInfo(ftoken)); }
			compound_stmt[eblock]
		{ s.FailureBlock = eblock; }
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
		Expression e = null;
	}:
	c:EXCEPT (x:ID)? (AS tr=type_reference)? ((IF|u:UNLESS) e=boolean_expression)?
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
		(
			ASSIGN 
			(
				{_compact}?initializer=simple_initializer |
				initializer=declaration_initializer
			)
		) 
		|
		({!_compact}? (m=stmt_modifier)? eos)
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
	({!_compact}?modifier=stmt_modifier)?
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
					(COLON|DO)=>e=method_invocation_block[e]
					| ((modifier=stmt_modifier)? eos)
				)
			) |
			(
				e=callable_expression
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
	(
		or:OR { fs.OrBlock = new Block(ToLexicalInfo(or)); }
		compound_stmt[fs.OrBlock]
	)?
	(
		et:THEN { fs.ThenBlock = new Block(ToLexicalInfo(et)); }
		compound_stmt[fs.ThenBlock]
	)?
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
	(
		or:OR { ws.OrBlock = new Block(ToLexicalInfo(or)); }
		compound_stmt[ws.OrBlock]
	)?
	(
		et:THEN { ws.ThenBlock = new Block(ToLexicalInfo(et)); }
		compound_stmt[ws.ThenBlock]
	)?
	;
		
protected
if_stmt returns [IfStatement returnValue]
	{
		returnValue = null;
		
		IfStatement s = null;
		Expression e = null;
	}:
	it:IF e=expression
	{
		returnValue = s = new IfStatement(ToLexicalInfo(it));
		s.Condition = e;
		s.TrueBlock = new Block();
	}
	compound_stmt[s.TrueBlock]
	(
		ei:ELIF e=expression
		{
			s.FalseBlock = new Block();
			
			IfStatement elif = new IfStatement(ToLexicalInfo(ei));
			elif.TrueBlock = new Block();
			elif.Condition = e;
			
			s.FalseBlock.Add(elif);
			s = elif;
		}
		compound_stmt[s.TrueBlock]
	)*
	(
		et:ELSE { s.FalseBlock = new Block(ToLexicalInfo(et)); }
		compound_stmt[s.FalseBlock]
	)?
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
method_invocation_block[Expression e] returns [MethodInvocationExpression mi]
{
	Expression block = null;
	mi = null;
}:
	block=callable_expression
	{
		mi = e as MethodInvocationExpression;
		if (null == mi) 
		{
			mi = new MethodInvocationExpression(e.LexicalInfo, e);
		}
		mi.Arguments.Add(block);
	}
;
	
ast_literal_expression returns [QuasiquoteExpression e]
{
	e = null;
}:
	begin:QQ_BEGIN
	{ e = new QuasiquoteExpression(ToLexicalInfo(begin)); }
	(
		(INDENT ast_literal_block[e] DEDENT (eos)?)
		| ast_literal_closure[e]
	)
	end:QQ_END
	{ e.EndSourceLocation = SourceLocationFactory.ToSourceLocation(end); }
;

type_definition_member_prediction:
	attributes
	modifiers
	(CLASS|INTERFACE|STRUCT|DEF|EVENT|((ID|(SPLICE_BEGIN atom)) (AS|ASSIGN|(COLON INDENT (GET|SET)))))
;

ast_literal_module[QuasiquoteExpression e]
{
	Module m = NewQuasiquoteModule(e.LexicalInfo);
	e.Node = m;
}:
	parse_module[m]
;

ast_literal_block[QuasiquoteExpression e]
{
	// TODO: either cache or construct these objects on demand
	TypeMemberCollection collection = new TypeMemberCollection();
	Block block = new Block();
	StatementCollection statements = block.Statements;
	Node node = null;
}: 
	(type_definition_member_prediction)=>(
		(type_definition_member[collection])+ {
			if (collection.Count == 1) {
				e.Node = collection[0];
			} else {
				Module m = NewQuasiquoteModule(e.LexicalInfo);
				m.Members = collection;
				e.Node = m;
			}
		}
	)
	| (ast_literal_module_prediction)=>(ast_literal_module[e])
	| ((stmt[statements])+ { e.Node = block.Statements.Count > 1 ? block : block.Statements[0]; })
;

ast_literal_module_prediction
{
}:
	(eos)?
	(NAMESPACE | IMPORT)
;


ast_literal_closure[QuasiquoteExpression e]
{
	Block block = null;
	Node node = null;
}:
	(expression (COLON | QQ_END))=>(
		node=expression { e.Node = node; }
		(
			c:COLON node=expression
			{
				e.Node = new ExpressionPair(
								ToLexicalInfo(c),
								(Expression)e.Node,
								(Expression)node);
			}
		)?
	) 
	| (
		node=import_directive_
		{
			e.Node = node;
		}
	)
	|(
		{ block = new Block(); }
		internal_closure_stmt[block]
		(
			eos
			(internal_closure_stmt[block])?
		)*
		{ 
			e.Node = block;
			if (block.Statements.Count == 1)
			{
				e.Node = block.Statements[0];
			}
		}
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
			lhs=method_invocation_block[lhs]
			{ stmt = new ExpressionStatement(lhs); }
		) |
		(
			(
			op:ASSIGN { token = op; binaryOperator = OperatorParser.ParseAssignment(op.getText()); }
				(
					{_compact}?rhs=array_or_expression |
					(COLON|DEF|DO)=>rhs=callable_expression |
					(
						rhs=array_or_expression
						(
							(COLON|DO)=>rhs=method_invocation_block[rhs] |
							(modifier=stmt_modifier eos) |
							eos
						)					
					)
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
assignment_or_method_invocation returns [Statement stmt]
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
		op:ASSIGN { token = op; binaryOperator = OperatorParser.ParseAssignment(op.getText()); }
		rhs=array_or_expression
	)
	{
		stmt = new ExpressionStatement(
				new BinaryExpression(ToLexicalInfo(token),
					binaryOperator,
					lhs, rhs));
		stmt.Modifier = modifier;
	}
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
					binaryOperator = OperatorParser.ParseAssignment(op.getText());
				}
			) |
			(
				ipbo:INPLACE_BITWISE_OR	{
					token = ipbo;
					binaryOperator = BinaryOperatorType.InPlaceBitwiseOr;
				}
			) |
			(
				ipxo:INPLACE_EXCLUSIVE_OR	{
					token = ipxo;
					binaryOperator = BinaryOperatorType.InPlaceExclusiveOr;
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
			(t:CMP_OPERATOR { op = OperatorParser.ParseComparison(t.getText()); token = t; } ) |
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
				oc:ONES_COMPLEMENT { op = oc; uOperator = UnaryOperatorType.OnesComplement; } |
				explode:MULTIPLY { op = explode; uOperator = UnaryOperatorType.Explode; }
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
		e=typeof_expression |
		e=splice_expression |
		e=omitted_member_expression
	)
;

protected
omitted_member_expression returns [Expression e]
{
	e = null;
	IToken memberName = null;
}:
	dot:DOT memberName=member
	{
		e = MemberReferenceForToken(new OmittedExpression(ToLexicalInfo(dot)), memberName);
	}
;

protected
splice_expression returns [Expression e]
{
	e = null;
}:
	begin:SPLICE_BEGIN e=atom
	{
		e = new SpliceExpression(ToLexicalInfo(begin), e);
	}
;
	
protected
char_literal returns [Expression e]
{
	e = null;
}:
	charToken:CHAR LPAREN
	( 
		t:SINGLE_QUOTED_STRING 
		{
			e = new CharLiteralExpression(ToLexicalInfo(t), t.getText());
		}
		|
		i:INT
		{
			e = new CharLiteralExpression(ToLexicalInfo(i), (char) PrimitiveParser.ParseInt(i));
		}
		|
		{
			e = new MethodInvocationExpression(
						ToLexicalInfo(charToken),
						new ReferenceExpression(ToLexicalInfo(charToken), charToken.getText()));
		}
	)
	RPAREN
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
		e = new ReferenceExpression(ToLexicalInfo(t), t.getText());
	}	
;
	
protected
paren_expression returns [Expression e]
{
	e = null;
	Expression condition = null;
	Expression falseValue = null;
}:
    (LPAREN OF)=>e=typed_array
	|
	(
		lparen:LPAREN
		e=array_or_expression
		(
			IF condition=boolean_expression
			ELSE falseValue=array_or_expression
			{
				ConditionalExpression ce = new ConditionalExpression(ToLexicalInfo(lparen));
				ce.Condition = condition;
				ce.TrueValue = e;
				ce.FalseValue = falseValue;
				
				e = ce;
			}
		)?
		RPAREN
	)
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
			(COMMA)?
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
	ev:EVENT { name=ev; } |
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
		TypeReference genericArgument = null;
		TypeReferenceCollection genericArguments = null;
		Expression nameSplice = null;
	} :
	e=atom
	( options { greedy=true; }:
		(
			lbrack:LBRACK
			(
				(
					OF
					{
						GenericReferenceExpression gre = new GenericReferenceExpression(ToLexicalInfo(lbrack));
						gre.Target = e;
						e = gre;
						genericArguments = gre.GenericArguments;
					}
					type_reference_list[genericArguments]					
				)
				|				
				{
					se = new SlicingExpression(ToLexicalInfo(lbrack));				
					se.Target = e;
					e = se;
				}
				slice[se] (COMMA slice[se])*
			)
			RBRACK
		)
		|
		(
			oft:OF genericArgument=type_reference
			{
				GenericReferenceExpression gre = new GenericReferenceExpression(ToLexicalInfo(oft));
				gre.Target = e;
				e = gre;
				gre.GenericArguments.Add(genericArgument);
			}
		)
		|
		(
			DOT 
			(
				(
					memberName=member
					{
						e = MemberReferenceForToken(e, memberName);
					}
				)
				|
				(
					begin:SPLICE_BEGIN nameSplice=atom
					{
						e = new SpliceMemberReferenceExpression(
									ToLexicalInfo(begin),
									e,
									nameSplice);
					}
				)
			)
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
					argument[mce] 
					(
						COMMA
						argument[mce]
					)*
				)?
			RPAREN
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
			e = PrimitiveParser.ParseIntegerLiteralExpression(i, val, false);
		}
		|
		l:LONG
		{
			val = l.getText();
			val = val.Substring(0, val.Length-1);
			if (neg != null) val = neg.getText() + val;
			e = PrimitiveParser.ParseIntegerLiteralExpression(l, val, true);
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
	LexicalInfo info = null;
}:
	(firstseparator:ESEPARATOR)?
	(options { greedy = true; }:
		startsep:ESEPARATOR
		{
			if (info == null)
			{
				info = ToLexicalInfo(startsep);
				e = new ExpressionInterpolationExpression(info);
			}
		}
		param=expression
		((format_sep:COLON)?
			formatString:ID
		)?
		{
			if (null != param)
			{
				e.Expressions.Add(param);
				if (null != formatString)
					param.Annotate("formatString", formatString.getText());
			}
		}
		endsep:ESEPARATOR
	)+
	(lastseparator:ESEPARATOR)?
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
		(COMMA)?
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
		rle = new DoubleLiteralExpression(ToLexicalInfo(value), PrimitiveParser.ParseDouble(val));
	}
	|
	single:FLOAT
	{
		val = single.getText();
		val = val.Substring(0, val.Length-1);
		if (neg != null) val = neg.getText() + val;
		rle = new DoubleLiteralExpression(ToLexicalInfo(single), PrimitiveParser.ParseDouble(val, true), true);
	}
	;
	
protected
timespan_literal returns [TimeSpanLiteralExpression tsle] { tsle = null; }:
	(neg:SUBTRACT)?
	value:TIMESPAN
	{
		string val = value.getText();
		if (neg != null) val = neg.getText() + val;
		tsle = new TimeSpanLiteralExpression(ToLexicalInfo(value), PrimitiveParser.ParseTimeSpan(val)); 
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
class BooLexer extends Lexer;
options
{
	testLiterals = false;
	exportVocab = Boo;	
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
		BooExpressionLexer lexer = new BooExpressionLexer(getInputState());
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
	(ID_PREFIX)? ID_LETTER (ID_LETTER | DIGIT)*
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
 				{BooLexer.IsDigit(LA(2))}? 
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

EXCLUSIVE_OR: '^' ('=' { $setType(INPLACE_EXCLUSIVE_OR); })?;

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

SPLICE_BEGIN : "$";

QQ_BEGIN: "[|"; 

QQ_END: "|]";

INCREMENT: "++";

DECREMENT: "--";

ADD: ('+') ('=' { $setType(ASSIGN); })?;

SUBTRACT: ('-') ('=' { $setType(ASSIGN); })?;

MODULUS: '%' ('=' { $setType(ASSIGN); })?;

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
RE_LITERAL : '/' (RE_CHAR)+ '/' (RE_OPTIONS)?;

protected
RE_CHAR : RE_ESC | ~('/' | '\\' | '\r' | '\n' | ' ' | '\t' );

protected
X_RE_CHAR: RE_CHAR | ' ' | '\t';

protected
RE_OPTIONS : (ID_LETTER)+;

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
REVERSE_DIGIT_GROUP : (DIGIT DIGIT DIGIT ({BooLexer.IsDigit(LA(2))}? '_'!)? | DIGIT)+;

protected
ID_PREFIX : '@';

protected
ID_LETTER : ('_' | 'a'..'z' | 'A'..'Z' | {System.Char.IsLetter(LA(1))}? '\u0080'..'\uFFFE');

protected
DIGIT : '0'..'9';

protected
HEXDIGIT : ('a'..'f' | 'A'..'F' | '0'..'9');

NULLABLE_SUFFIX: '?';

