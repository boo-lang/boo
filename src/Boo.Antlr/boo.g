// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
options
{
	language = "CSharp";
	namespace = "Boo.Antlr";
}

{
using Boo.Lang.Ast;
using Boo.Antlr.Util;

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
	AND="and";
	AS="as";		
	BREAK="break";
	CONTINUE="continue";	
	CLASS="class";
	CONSTRUCTOR="constructor";	
	DEF="def";	
	ELSE="else";
	ENSURE="ensure";
	ENUM="enum";
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
	IF="if";	
	IN="in";
	KINDOF="kindof";
	NOT="not";	
	NULL="null";
	OR="or";
	OTHERWISE="otherwise";	
	PASS="pass";
	NAMESPACE="namespace";
	PROPERTY="property";
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
	UNLESS="unless";
	UNTIL="until";
	VOID="void";	
	WHEN="when";
	WHILE="while";
	YIELD="yield";
}

{		
	protected System.Text.StringBuilder _sbuilder = new System.Text.StringBuilder();
	
	protected AttributeCollection _attributes = new AttributeCollection();
	
	protected TypeMemberModifiers _modifiers = TypeMemberModifiers.None;	
	
	protected void ResetMemberData()
	{
		_modifiers = TypeMemberModifiers.None;
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
			case "<": return BinaryOperatorType.LessThan;
			case ">": return BinaryOperatorType.GreaterThan;
			case "==": return BinaryOperatorType.Equality;
			case "!=": return BinaryOperatorType.Inequality;
			case "=~": return BinaryOperatorType.Match;
			case "!~": return BinaryOperatorType.NotMatch;
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
				// assumes '\r\n'
				startIndex++;
				length--;
			}						
			if ('\n' == s[s.Length-1])
			{
				length--;
			}
			return s.Substring(startIndex, length);
		}
		return s;
	}

	protected bool IsValidMacroArgument(int token)
	{
		return token == ID ||
				token == COLON;
	}
}

protected
start returns [Module module]
	{
		module = new Module();		
		module.LexicalInfo = new LexicalInfo(getFilename(), 0, 0, 0);
	}:
	docstring[module]
	(options { greedy=true;}: EOS)*			 
	(namespace_directive[module])?
	(import_directive[module])*
	(type_member[module.Members])*
	globals[module]
	EOF!
	;
			
protected docstring[Node node]:
	(
		doc:TRIPLE_QUOTED_STRING { node.Documentation = MassageDocString(doc.getText()); }
		(EOS)*
	)?
	;
			
protected
eos : (options { greedy = true; }: EOS)+;

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
		FROM id=identifier
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
		enum_definition[container]
	)			
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
		ed.Attributes.Add(_attributes);
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
		em.Attributes.Add(_attributes);
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
		Boo.Lang.Ast.Attribute attr = null;
	}
	:	
	id=identifier
	{
		attr = new Boo.Lang.Ast.Attribute(ToLexicalInfo(id));
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
	c:CLASS id:ID
	{
		cd = new ClassDefinition(ToLexicalInfo(c));
		cd.Name = id.getText();
		cd.Modifiers = _modifiers;
		cd.Attributes.Add(_attributes);
		container.Add(cd);
	}
	(base_types[cd.BaseTypes])?
	begin_with_doc[cd]					
	(
		(PASS! eos) |
		(
			(EOS)*
			attributes
			modifiers
			(						
				method[cd.Members] |
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
		itf.Attributes.Add(_attributes);
		container.Add(itf);
	}
	(base_types[itf.BaseTypes])?
	begin
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
	LPAREN tr=type_reference { container.Add(tr); }
	(COMMA tr=type_reference { container.Add(tr); })*
	RPAREN
	;
			
protected
interface_method [TypeMemberCollection container]
	{
		Method m = null;
		TypeReference rt = null;
	}: 
	t:DEF! id:ID
	{
		m = new Method(ToLexicalInfo(t));
		m.Name = id.getText();
		m.Attributes.Add(_attributes);
		container.Add(m);
	}
	LPAREN! parameter_declaration_list[m.Parameters] RPAREN!
	(AS! rt=type_reference { m.ReturnType=rt; })?			
	(
		eos | (empty_block (EOS)*)
	)
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
		p.Attributes.Add(_attributes);
		container.Add(p);
	}
	begin!
		(interface_property_accessor[p])+
	end!
	(EOS)*
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
		m.Attributes.Add(_attributes);
	}
	;
			
protected
empty_block: 
		begin!
			PASS! eos
		end!
		;

protected
method [TypeMemberCollection container]
	{
		Method m = null;
		TypeReference rt = null;
	}: 
	t:DEF
	(
		id:ID { m = new Method(ToLexicalInfo(t)); m.Name = id.getText(); } |
		c:CONSTRUCTOR { m = new Constructor(ToLexicalInfo(t)); m.Name = c.getText(); }
	)	
	{
		m.Modifiers = _modifiers;
		m.Attributes.Add(_attributes);
	}
	LPAREN! parameter_declaration_list[m.Parameters] RPAREN!
			(AS! rt=type_reference { m.ReturnType = rt; })?
			attributes { m.ReturnTypeAttributes.Add(_attributes); }
			begin_with_doc[m]
				block[m.Body.Statements]
			end
	{ container.Add(m); }
	;	
	
protected
field_or_property [TypeMemberCollection container]
	{
		TypeMember tm = null;
		TypeReference tr = null;
		Property p = null;
		Expression initializer = null;
	}: 
	id:ID (AS! tr=type_reference)? (ASSIGN! initializer=expression)?
	(
		eos
		{
			Field field = new Field(ToLexicalInfo(id));
			field.Type = tr;
			field.Initializer = initializer;
			tm = field;
			tm.Name = id.getText();
			tm.Modifiers = _modifiers;
			tm.Attributes.Add(_attributes);
		}
		docstring[tm]
		|		
		{
			p = new Property(ToLexicalInfo(id));			
			p.Type = tr;
			tm = p;
			tm.Name = id.getText();
			tm.Modifiers = _modifiers;
			tm.Attributes.Add(_attributes);
		}		
		begin_with_doc[p]
			(property_accessor[p])+
		end
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
		m.Attributes.Add(_attributes);
		m.Modifiers = _modifiers;
	}
	compound_stmt[m.Body.Statements]
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
		TRANSIENT! { _modifiers |= TypeMemberModifiers.Transient; }
	)*
	;
	
protected	
parameter_declaration_list[ParameterDeclarationCollection c] : 
	(parameter_declaration[c] (COMMA! parameter_declaration[c])* )?
	;

protected
parameter_declaration[ParameterDeclarationCollection c]
	{		
		TypeReference tr = null;
	}: 
	attributes
	id:ID (AS! tr=type_reference)? 
	{
		ParameterDeclaration pd = new ParameterDeclaration(ToLexicalInfo(id));
		pd.Name = id.getText();
		pd.Type = tr;
		pd.Attributes.Add(_attributes);
		c.Add(pd);
	} 
	;

protected
type_reference returns [TypeReference tr]
	{
		tr=null;
		Token id = null;
	}: 
	id=identifier
	{
		tr = new TypeReference(ToLexicalInfo(id));
		tr.Name = id.getText();
	}
	;

protected
begin : COLON INDENT;

protected
begin_with_doc[Node node]: COLON (EOS docstring[node])? INDENT;

protected
end : DEDENT;

protected
compound_stmt[StatementCollection c] :
		begin
			block[c]
		end
		(options { greedy=true; }: EOS)*
		;
		
protected
macro_stmt returns [MacroStatement macro]
	{
		macro = new MacroStatement();
	}:
	id:ID expression_list[macro.Arguments]
		compound_stmt[macro.Block.Statements]
	{
		macro.Name = id.getText();
		macro.LexicalInfo = ToLexicalInfo(id);
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
		(		
			(				
				s=return_stmt |
				s=yield_stmt |
				s=break_stmt |				
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
	{ container.Add(s); }
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
		i:IF { t = i; type = StatementModifierType.If; } |
		u:UNLESS { t = u; type = StatementModifierType.Unless; } |
		w:WHILE { t = w; type = StatementModifierType.While; } |
		ut:UNTIL { t = ut; type = StatementModifierType.Until; }
	)
	e=expression
	{
		m = new StatementModifier(ToLexicalInfo(t));
		m.Type = type;
		m.Condition = e;
	}
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
	c:EXCEPT x:ID (AS tr=type_reference)?
	{
		eh = new ExceptionHandler(ToLexicalInfo(c));
		eh.Declaration = new Declaration(ToLexicalInfo(x));
		eh.Declaration.Name = x.getText();		
		eh.Declaration.Type = tr;
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
	t:RAISE! e=expression
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
	id:ID AS! tr=type_reference (ASSIGN! initializer=tuple_or_expression)?
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
	e=expression
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
	r:RETURN (e=tuple_or_expression)?
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
	yt:YIELD! e=tuple_or_expression
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
		declaration_list[fs.Declarations] IN! iterator=tuple_or_expression
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
			when:WHEN! e=tuple_or_expression
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
		s.Expression = e;
		s.TrueBlock = new Block();
	}
	compound_stmt[s.TrueBlock.Statements]
	(
		et:ELSE { s.FalseBlock = new Block(ToLexicalInfo(et)); }
		compound_stmt[s.FalseBlock.Statements]
	)?
	;
		
protected
unpack_stmt returns [UnpackStatement s]
	{
		s = new UnpackStatement();
		Expression e = null;
	}:
	declaration_list[s.Declarations] t:ASSIGN! e=tuple_or_expression
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
tuple_or_expression returns [Expression e]
	{
		e = null;
		TupleLiteralExpression tle = null;
	} :
	(
		// tupla vazia: , ou (,)
		c:COMMA! { e = new TupleLiteralExpression(ToLexicalInfo(c)); }
	) |
	(
		e=expression
		( options { greedy=true; }:
			t:COMMA!
			{
				tle = new TupleLiteralExpression(e.LexicalInfo);
				tle.Items.Add(e);		
			}
			( options { greedy=true; }:
				e=expression { tle.Items.Add(e); }
				( options { greedy=true; }:
					COMMA!
					e=expression { tle.Items.Add(e); }
				)*
			)?
			{ e = tle; }
		)?
	)
	;
			
protected
expression returns [Expression e]
	{
		e = null;
		TypeReference tr = null;
		
		IteratorExpression lde = null;
		StatementModifier filter = null;
		Expression iterator = null;
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
			lde = new IteratorExpression(ToLexicalInfo(f));
			lde.Expression = e;
		}
		declaration_list[lde.Declarations]
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
		nt:NOT
		e=boolean_expression
		{
			UnaryExpression ue = new UnaryExpression(ToLexicalInfo(nt));
			ue.Operator = UnaryOperatorType.Not;
			ue.Operand = e;
			e = ue;
		}
	)
	|
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
	e=ternary_expression
	(
		at:AND
		r=ternary_expression
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
ternary_expression returns [Expression e]
	{
		e = null;			
		Expression te = null;
		Expression fe = null;
	}:
	e=assignment_expression	
	( options { greedy = true; } :
		t:QMARK
		te=ternary_expression COLON
		fe=assignment_expression
		{
			TernaryExpression finalExpression = new TernaryExpression(ToLexicalInfo(t));
			finalExpression.Condition = e;
			finalExpression.TrueExpression = te;
			finalExpression.FalseExpression = fe;
			e = finalExpression;
		}
	)*
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
		r=tuple_or_expression
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
	}:
	e=sum
	( options { greedy = true; } :
		op=cmp_operator
		r=sum
		{
			BinaryExpression be = new BinaryExpression(e.LexicalInfo);
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
	}:
	e=term
	( options { greedy = true; } :
		op:SUM_OPERATOR
		r=term
		{
			BinaryExpression be = new BinaryExpression(ToLexicalInfo(op));
			be.Operator = ParseSumOperator(op.getText());
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
	}:
	e=unary_expression
	( options { greedy = true; } :
		op:MULT_OPERATOR
		r=unary_expression
		{
			BinaryExpression be = new BinaryExpression(ToLexicalInfo(op));
			be.Operator = ParseMultOperator(op.getText());
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
	}: 
	(
		sum_op:SUM_OPERATOR { op = sum_op; } |
		inc:INCREMENT { op = inc; } |
		dec:DECREMENT { op = dec; }
	)?
	e=slicing_expression
	{
		if (null != op)
		{
			UnaryExpression ue = new UnaryExpression(ToLexicalInfo(op));
			ue.Operator = ParseUnaryOperator(op.getText());
			ue.Operand = e;
			e = ue; 
		}
	}
	;		
	
protected
cmp_operator returns [BinaryOperatorType op] { op = BinaryOperatorType.None; }:
	(t:CMP_OPERATOR { op = ParseCmpOperator(t.getText()); } ) |
	(IS { op = BinaryOperatorType.ReferenceEquality; }
		(NOT { op = BinaryOperatorType.ReferenceInequality; })?
		) |
	(KINDOF { op = BinaryOperatorType.TypeTest; } ) |
	(IN { op = BinaryOperatorType.Member; } ) |
	(NOT IN { op = BinaryOperatorType.NotMember; })
	;

protected
atom returns [Expression e]
	{
		e = null;
	}:	
	(
		e=literal |	
		e=reference_expression |
		e=paren_expression
	)
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
	LPAREN! e=tuple_or_expression RPAREN!
	;

protected
tuple returns [Expression e]
	{
		TupleLiteralExpression tle = null;
		e = null;
	}:
	t:LPAREN!
	e=expression
	(
		COMMA!
		{
			tle = new TupleLiteralExpression(ToLexicalInfo(t));
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
		e = new IntegerLiteralExpression(ToLexicalInfo(i));
		e.Value = i.getText();
	}
	;
	
protected
string_literal returns [Expression e]
	{
		e = null;
	}:
	(DOUBLE_QUOTED_STRING ESEPARATOR)=> e=string_formatting |
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
string_formatting returns [StringFormattingExpression e]
	{
		e = null;
		Expression param = null;
	}:
	dqs:DOUBLE_QUOTED_STRING
	{
		e = new StringFormattingExpression(ToLexicalInfo(dqs));
		e.Template = dqs.getText();
	}
	(  options { greedy = true; } :
		
		ESEPARATOR param=expression
		{ e.Arguments.Add(param); }
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
timespan_literal returns [TimeSpanLiteralExpression tsle] { tsle = null; }:
	value:TIMESPAN
	{ tsle = new TimeSpanLiteralExpression(ToLexicalInfo(value), value.getText()); }
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
		DOT
		id2:ID
		{ _sbuilder.Append('.'); _sbuilder.Append(id2.getText()); }
	)*
	{ value.setText(_sbuilder.ToString()); }
	;	 
{
using Boo.Antlr.Util;
}
class BooLexer extends Lexer;
options
{
	testLiterals = false;
	exportVocab = Boo;	
	k = 3;
	charVocabulary='\u0003'..'\uFFFF';
	// without inlining some bitset tests, ANTLR couldn't do unicode;
	// They need to make ANTLR generate smaller bitsets;
	codeGenBitsetTestThreshold=20;
}
{
	protected int _skipWhitespaceRegion = 0;
	
	// token separador de expressões
	antlr.Token _eseparator = new antlr.CommonToken(ESEPARATOR, "ESEPARATOR");
	
	// índice atual na expressão de formatação de strings ver ESCAPED_EXPRESSION
	int _eindex = 0;
	
	// lexer para expressões dentro de formatação de strings
	BooExpressionLexer _el;
	
	TokenStreamRecorder _erecorder;
	
	antlr.TokenStreamSelector _selector;
	
	internal void Initialize(antlr.TokenStreamSelector selector, int tabSize, string tokenObjectClass)
	{
		_selector = selector;
		_el = new BooExpressionLexer(getInputState());
		_el.setTabSize(tabSize);
		_el.setTokenObjectClass(tokenObjectClass);
		
		_erecorder = new TokenStreamRecorder(selector);
		
	}
	
	bool SkipWhitespace
	{
		get
		{
			return _skipWhitespaceRegion > 0;
		}
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

INT : (DIGIT)+ ( ("ms" | 's' | 'm' | 'h' | 'd') { $setType(TIMESPAN); } )?;

DOT : '.';

COLON : ':';

LPAREN : '(' { EnterSkipWhitespaceRegion(); };
	
RPAREN : ')' { LeaveSkipWhitespaceRegion(); };

LBRACK : '[' { EnterSkipWhitespaceRegion(); };

RBRACK : ']' { LeaveSkipWhitespaceRegion(); };

LBRACE : '{' { EnterSkipWhitespaceRegion(); };
	
RBRACE : '}' { LeaveSkipWhitespaceRegion(); };

QMARK : '?';

INCREMENT: "++";

DECREMENT: "--";

SUM_OPERATOR :
	('+' | '-')
	('=' { $setType(ASSIGN); })?
	;

MULT_OPERATOR :
	'%'| 
	'*' ('=' { $setType(ASSIGN); })? |
	(RE_LITERAL)=> RE_LITERAL { $setType(RE_LITERAL); } |
	'/' (
			('/' (~('\n'|'\r'))* { $setType(Token.SKIP); }) |
			('=' { $setType(ASSIGN); }) |
		)
	;

CMP_OPERATOR : '<' | "<=" | '>' | ">=" | "!~" | "!=";

ASSIGN : '=' ( ('=' | '~') { $setType(CMP_OPERATOR); } )?;

COMMA : ',';

protected
TRIPLE_QUOTED_STRING :
	"\"\""!
	(
	options { greedy=false; }:
		~('\n') |
		'\n' { newline(); }
	)*
	"\"\"\""!
	;

DOUBLE_QUOTED_STRING : { _eindex = 0; }
	'"'!
	(
		{LA(1)=='"' && LA(2)=='"'}?TRIPLE_QUOTED_STRING { $setType(TRIPLE_QUOTED_STRING); }
		|
		(
			(
				DQS_ESC |
				// Utilizar um predicado permite que
				// o símbolo $ apareça normalmente na string
				// sem necessidade de ser scaped
				// desde que não seja seguido por um {
				("${")=>ESCAPED_EXPRESSION |
				~('"' | '\\' | '\r' | '\n')
			)*
			'"'!
			{
				if (_erecorder.Count > 0)
				{
					_selector.push(_erecorder);
				}
			}
		)
	)
	;
		
SINGLE_QUOTED_STRING :
	'\''!
	(
		SQS_ESC |
		~('\'' | '\\' | '\r' | '\n')
	)*
	'\''!
	;

SL_COMMENT : "#" (~('\n'|'\r'))* { $setType(Token.SKIP); }
			;
			
WS :
	(
		' ' |
		'\t' |
		'\f' |
		'\r' |
		'\n' { newline(); }
	)+
	{
		if (SkipWhitespace)
		{
			$setType(Token.SKIP);
		}
	}
	;
		
EOS: ';';
		
protected
ESCAPED_EXPRESSION : "${"
	{			
		_erecorder.Enqueue(_eseparator);
		if (_erecorder.RecordUntil(_el, RBRACE) > 0)
		{
			$setText("{" + _eindex + "}");
			++_eindex;
		}
	}
	;

protected
DQS_ESC : '\\'! ( SESC | '"' | '$') ;	
	
protected
SQS_ESC : '\\'! ( SESC | '\'' );

protected
SESC : 
				( 'r' {$setText("\r"); }) |
				( 'n' {$setText("\n"); }) |
				( 't' {$setText("\t"); }) |
				( '\\' );

protected
RE_LITERAL : '/' (RE_CHAR)+ '/';

protected
RE_CHAR : RE_ESC | ~('/' | '\\' | ' ' | '\t' | '\r' | '\n');

protected
RE_ESC : '\\' ('\\' | '/' | 'r' | 'n' | 't' | '(' | ')' | '.' | '*' | '?' | '[' | ']');

protected
ID_LETTER : ('_' | 'a'..'z' | 'A'..'Z' );

protected
DIGIT : '0'..'9';
