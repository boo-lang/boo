#region license
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
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
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
#endregion

namespace Boo.Lang.Parser.Tests
import NUnit.Framework
import Boo.Lang.Compiler.Ast
import Boo.Lang.Parser

[TestFixture]
class BooParserTestCase(AbstractParserTestFixture):
	"""
	Test cases for the BooParser class.
	"""
	[Test]
	def TestEndSourceLocationForInlineClosures():
		code = `foo = { a = 3;
return a; }`
		EnsureClosureEndSourceLocation(code, 2, 11)

	[Test]
	def TestEndSourceLocationForBlockClosures():
		code = `
foo = def():
    return a
`
		EnsureClosureEndSourceLocation(code, 3, 13)

	def EnsureClosureEndSourceLocation(code as string, line as int, column as int):
		cu = BooParser.ParseString("closures", code)
		e = (cu.Modules[0].Globals.Statements[0] cast ExpressionStatement).Expression
		cbe = (e cast BinaryExpression).Right cast BlockExpression
		esl = cbe.Body.EndSourceLocation
		Assert.AreEqual(line, esl.Line)
		Assert.AreEqual(column, esl.Column)

	[Test]
	def TestParseExpression():
		code = "3 + 2 * 5"
		e = BooParser.ParseExpression("test", code)
		Assert.AreEqual("3 + (2 * 5)", e.ToString())

	[Test]
	def TestSimple():
		fname = GetTestCasePath("simple.boo")
		cu = BooParser.ParseFile(fname)
		Assert.IsNotNull(cu)

		module = cu.Modules[0]
		Assert.IsNotNull(module)
		Assert.AreEqual("simple", module.Name)
		Assert.AreEqual("module doc string", module.Documentation)
		Assert.AreEqual("Empty.simple", module.FullName)
		Assert.AreEqual(fname, module.LexicalInfo.FileName)

		Assert.IsNotNull(module.Namespace)

		Assert.AreEqual("Empty", module.Namespace.Name)
		Assert.AreEqual(4, module.Namespace.LexicalInfo.Line)
		Assert.AreEqual(1, module.Namespace.LexicalInfo.Column)
		Assert.AreEqual(fname, module.Namespace.LexicalInfo.FileName)

	[Test]
	def TestSimpleClasses():
		fname = GetTestCasePath("simple_classes.boo")

		module = BooParser.ParseFile(fname).Modules[0]
		Assert.AreEqual("Foo.Bar", module.Namespace.Name)

		Assert.IsNotNull(module.Members)
		Assert.AreEqual(2, module.Members.Count)

		cd as TypeMember = module.Members[0]
		Assert.IsTrue(cd isa ClassDefinition)
		Assert.AreEqual("Customer", cd.Name)
		Assert.AreEqual("Foo.Bar.Customer", cd.FullName)
		Assert.AreSame(module.Namespace, cd.EnclosingNamespace)

		cd = module.Members[1]
		Assert.AreEqual("Person", cd.Name)

	[Test]
	def TestSimpleClassMethods():
		module = ParseTestCase("simple_class_methods.boo")
		Assert.AreEqual("ITL.Content", module.Namespace.Name)
		Assert.AreEqual(1, module.Imports.Count)

		i = module.Imports[0]
		Assert.AreEqual("System", i.Namespace)
		Assert.AreEqual(3, i.LexicalInfo.Line)

		Assert.AreEqual(1, module.Members.Count)

		cd = module.Members[0] cast ClassDefinition
		Assert.AreEqual("Article", cd.Name)

		Assert.AreEqual(3, cd.Members.Count)

		m = cd.Members[0] cast Method
		Assert.AreEqual("getTitle", m.Name)
		Assert.IsNotNull(m.ReturnType, "ReturnType")
		Assert.AreEqual("string", (m.ReturnType cast SimpleTypeReference).Name)

		m = cd.Members[1] cast Method
		Assert.AreEqual("getBody", m.Name)
		Assert.IsNotNull(m.ReturnType, "ReturnType")
		Assert.AreEqual("string", (m.ReturnType cast SimpleTypeReference).Name)

		m = cd.Members[2] cast Method
		Assert.AreEqual("getTag", m.Name)
		Assert.IsNull(m.ReturnType, "methods without a return type must have ReturnType set to null!")

	[Test]
	def TestSimpleClassFields():
		module = ParseTestCase("simple_class_fields.boo")

		Assert.AreEqual(1, module.Members.Count)
		cd = module.Members[0] cast ClassDefinition

		Assert.AreEqual(3, cd.Members.Count, "Members")

		f = cd.Members[0] cast Field
		Assert.AreEqual("_name", f.Name)
		Assert.IsNotNull(f.Type, "Field.Type")
		Assert.AreEqual("string", (f.Type cast SimpleTypeReference).Name)

		c = cd.Members[1] cast Constructor
		Assert.AreEqual("constructor", c.Name)
		Assert.IsNull(c.ReturnType)
		Assert.AreEqual(1, c.Parameters.Count, "Parameters.Count")
		Assert.AreEqual("name", c.Parameters[0].Name)
		Assert.AreEqual("string", (c.Parameters[0].Type cast SimpleTypeReference).Name)

		m = cd.Members[2] cast Method
		Assert.AreEqual("getName", m.Name)
		Assert.IsNull(m.ReturnType)
		Assert.AreEqual(0, m.Parameters.Count)
		Assert.IsNotNull(m.Body, "Body")
		Assert.AreEqual(1, m.Body.Statements.Count)

		rs = m.Body.Statements[0] cast ReturnStatement
		i = rs.Expression cast ReferenceExpression
		Assert.AreEqual("_name", i.Name)

	[Test]
	def TestSimpleGlobalDefs():
		module = ParseTestCase("simple_global_defs.boo")
		Assert.AreEqual("Math", module.Namespace.Name)
		Assert.AreEqual(3, module.Members.Count)
		Assert.AreEqual("Rational", module.Members[0].Name)
		Assert.AreEqual("pi", module.Members[1].Name)
		Assert.AreEqual("rationalPI", module.Members[2].Name)
		Assert.AreEqual(0, module.Globals.Statements.Count)

	[Test]
	def StatementModifiersOnUnpackStatement():
		module = ParseTestCase("stmt_modifiers_3.boo")

		body = module.Globals
		Assert.AreEqual(2, body.Statements.Count)

		stmt = body.Statements[0] cast UnpackStatement
		Assert.IsNotNull(stmt.Modifier, "Modifier")
		Assert.AreEqual(StatementModifierType.If, stmt.Modifier.Type)
		Assert.IsTrue(stmt.Modifier.Condition isa BoolLiteralExpression)
		Assert.AreEqual(true, (stmt.Modifier.Condition cast BoolLiteralExpression).Value)

		RunParserTestCase("stmt_modifiers_3.boo")

	[Test]
	def TestStmtModifiers1():
		module = ParseTestCase("stmt_modifiers_1.boo")

		m = module.Members[0] cast Method
		rs = m.Body.Statements[0] cast ReturnStatement
		Assert.IsNotNull(rs.Modifier, "Modifier")
		Assert.AreEqual(StatementModifierType.If, rs.Modifier.Type)

		be = rs.Modifier.Condition cast BinaryExpression
		Assert.AreEqual(BinaryOperatorType.LessThan, be.Operator)
		Assert.AreEqual("n", (be.Left cast ReferenceExpression).Name)
		Assert.AreEqual(2, (be.Right cast IntegerLiteralExpression).Value)

	[Test]
	def TestStmtModifiers2():
		module = ParseTestCase("stmt_modifiers_2.boo")

		s = module.Globals.Statements[0] cast ExpressionStatement
		a = s.Expression cast BinaryExpression
		Assert.AreEqual(BinaryOperatorType.Assign, a.Operator)
		Assert.AreEqual("f", (a.Left cast ReferenceExpression).Name)
		Assert.AreEqual(BinaryOperatorType.Division, (a.Right cast BinaryExpression).Operator)

	[Test]
	def TestStaticMethod():
		module = ParseTestCase("static_method.boo")
		Assert.AreEqual(1, module.Members.Count)

		cd = module.Members[0] cast ClassDefinition
		Assert.AreEqual("Math", cd.Name)
		Assert.AreEqual(1, cd.Members.Count)

		m = cd.Members[0] cast Method
		Assert.AreEqual(TypeMemberModifiers.Static, m.Modifiers)
		Assert.AreEqual("square", m.Name)
		Assert.AreEqual("int", (m.ReturnType cast SimpleTypeReference).Name)

	[Test]
	def TestClass2():
		module = ParseTestCase("class_2.boo")
		cd = module.Members[0] cast ClassDefinition

		Assert.AreEqual(6, cd.Members.Count)
		for i in range(0, 5):
			Assert.AreEqual(TypeMemberModifiers.None, cd.Members[i].Modifiers)
		Assert.AreEqual(TypeMemberModifiers.Public | TypeMemberModifiers.Static, cd.Members[5].Modifiers)

	[Test]
	def TestForStmt1():
		module = ParseTestCase("for_stmt_1.boo")

		fs = module.Globals.Statements[0] cast ForStatement
		Assert.AreEqual(1, fs.Declarations.Count)

		d = fs.Declarations[0]
		Assert.AreEqual("i", d.Name)
		Assert.IsNull(d.Type)

		lle = fs.Iterator cast ListLiteralExpression
		Assert.AreEqual(3, lle.Items.Count)
		for i in range(0, 3):
			Assert.AreEqual(i+1, (lle.Items[i] cast IntegerLiteralExpression).Value)

		Assert.AreEqual(1, fs.Block.Statements.Count)
		Assert.AreEqual("print", (((fs.Block.Statements[0] cast ExpressionStatement).Expression cast MethodInvocationExpression).Target cast ReferenceExpression).Name)

	[Test]
	def TestRELiteral1():
		module = ParseTestCase("re_literal_1.boo")
		Assert.AreEqual(2, module.Globals.Statements.Count)

		es = module.Globals.Statements[1] cast ExpressionStatement
		Assert.AreEqual("print", ((es.Expression cast MethodInvocationExpression).Target cast ReferenceExpression).Name)

		Assert.AreEqual(StatementModifierType.If, es.Modifier.Type)

		be = es.Modifier.Condition cast BinaryExpression
		Assert.AreEqual(BinaryOperatorType.Match, be.Operator)
		Assert.AreEqual("s", (be.Left cast ReferenceExpression).Name)
		Assert.AreEqual("/foo/", (be.Right cast RELiteralExpression).Value)

	[Test]
	def TestRELiteral2():
		module = ParseTestCase("re_literal_2.boo")

		stmts = module.Globals.Statements
		Assert.AreEqual(2, stmts.Count)

		ae = (stmts[0] cast ExpressionStatement).Expression cast BinaryExpression
		Assert.AreEqual(BinaryOperatorType.Assign, ae.Operator)
		Assert.AreEqual("\"Bamboo\"\n", (ae.Right cast StringLiteralExpression).Value)

		ae = (stmts[1] cast ExpressionStatement).Expression cast BinaryExpression
		Assert.AreEqual(BinaryOperatorType.Assign, ae.Operator)
		Assert.AreEqual("/foo\\(bar\\)/", (ae.Right cast RELiteralExpression).Value)

	[Test]
	def TestRELiteral3():
		module = ParseTestCase("re_literal_3.boo")

		stmts = module.Globals.Statements
		Assert.AreEqual(2, stmts.Count)

		ae = (stmts[0] cast ExpressionStatement).Expression cast BinaryExpression
		Assert.AreEqual(BinaryOperatorType.Assign, ae.Operator)
		Assert.AreEqual("/\\x2f\\u002f/", (ae.Right cast RELiteralExpression).Value)

	[Test]
	def TestIfElse1():
		module = ParseTestCase("if_else_1.boo")

		stmts = module.Globals.Statements
		Assert.AreEqual(1, stmts.Count)

		s = stmts[0] cast IfStatement
		be = s.Condition cast BinaryExpression
		Assert.AreEqual(BinaryOperatorType.Match, be.Operator)
		Assert.AreEqual("gets", ((be.Left cast MethodInvocationExpression).Target cast ReferenceExpression).Name)
		Assert.AreEqual("/foo/", (be.Right cast RELiteralExpression).Value)
		Assert.AreEqual(3, s.TrueBlock.Statements.Count)
		Assert.IsNull(s.FalseBlock)

		s = s.TrueBlock.Statements[2] cast IfStatement
		be = s.Condition cast BinaryExpression
		Assert.AreEqual("/bar/", (be.Right cast RELiteralExpression).Value)
		Assert.AreEqual(1, s.TrueBlock.Statements.Count)
		Assert.IsNotNull(s.FalseBlock)
		Assert.AreEqual(1, s.FalseBlock.Statements.Count)
		Assert.AreEqual("foobar, eh?", (((s.TrueBlock.Statements[0] cast ExpressionStatement).Expression cast MethodInvocationExpression).Arguments[0] cast StringLiteralExpression).Value)
		Assert.AreEqual("nah?", (((s.FalseBlock.Statements[0] cast ExpressionStatement).Expression cast MethodInvocationExpression).Arguments[0] cast StringLiteralExpression).Value)

	[Test]
	def TestInterface1():
		module = ParseTestCase("interface_1.boo")

		Assert.AreEqual(1, module.Members.Count)

		id = module.Members[0] cast InterfaceDefinition
		Assert.AreEqual("IContentItem", id.Name)

		Assert.AreEqual(5, id.Members.Count)

		p = id.Members[0] cast Property
		Assert.AreEqual("Parent", p.Name)
		Assert.AreEqual("IContentItem", (p.Type cast SimpleTypeReference).Name)
		Assert.IsNotNull(p.Getter, "Getter")
		Assert.IsNull(p.Setter, "Setter")

		p = id.Members[1] cast Property
		Assert.AreEqual("Name", p.Name)
		Assert.AreEqual("string", (p.Type cast SimpleTypeReference).Name)
		Assert.IsNotNull(p.Getter, "Getter")
		Assert.IsNotNull(p.Setter, "Setter")

		m = id.Members[2] cast Method
		Assert.AreEqual("SelectItem", m.Name)
		Assert.AreEqual("IContentItem", (m.ReturnType cast SimpleTypeReference).Name)
		Assert.AreEqual("expression", m.Parameters[0].Name)
		Assert.AreEqual("string", (m.Parameters[0].Type cast SimpleTypeReference).Name)

		Assert.AreEqual("Validate", (id.Members[3] cast Method).Name)
		Assert.AreEqual("OnRemove", (id.Members[4] cast Method).Name)

	[Test]
	def TestEnum1():
		module = ParseTestCase("enum_1.boo")

		Assert.AreEqual(2, module.Members.Count)

		ed = module.Members[0] cast EnumDefinition
		Assert.AreEqual("Priority", ed.Name)
		Assert.AreEqual(3, ed.Members.Count)
		Assert.AreEqual("Low", ed.Members[0].Name)
		Assert.AreEqual("Normal", ed.Members[1].Name)
		Assert.AreEqual("High", ed.Members[2].Name)

		ed = module.Members[1] cast EnumDefinition
		Assert.AreEqual(3, ed.Members.Count)
		Assert.AreEqual("Easy", ed.Members[0].Name)
		Assert.AreEqual(0, ((ed.Members[0] cast EnumMember).Initializer cast IntegerLiteralExpression).Value)
		Assert.AreEqual("Normal", ed.Members[1].Name)
		Assert.AreEqual(5, ((ed.Members[1] cast EnumMember).Initializer cast IntegerLiteralExpression).Value)
		Assert.AreEqual("Hard", ed.Members[2].Name)
		Assert.IsNull((ed.Members[2] cast EnumMember).Initializer, "Initializer")

	[Test]
	def TestProperties1():
		module = ParseTestCase("properties_1.boo")

		cd = module.Members[0] cast ClassDefinition
		Assert.AreEqual("Person", cd.Name)
		Assert.AreEqual("_id", cd.Members[0].Name)
		Assert.AreEqual("_name", cd.Members[1].Name)

		p = cd.Members[3] cast Property
		Assert.AreEqual("ID", p.Name)
		Assert.AreEqual("string", (p.Type cast SimpleTypeReference).Name)
		Assert.IsNotNull(p.Getter, "Getter")
		Assert.AreEqual(1, p.Getter.Body.Statements.Count)
		Assert.AreEqual("_id", ((p.Getter.Body.Statements[0] cast ReturnStatement).Expression cast ReferenceExpression).Name)
		Assert.IsNull(p.Setter, "Setter")

		p = cd.Members[4] cast Property
		Assert.AreEqual("Name", p.Name)
		Assert.AreEqual("string", (p.Type cast SimpleTypeReference).Name)
		Assert.IsNotNull(p.Getter, "Getter ")
		Assert.AreEqual(1, p.Getter.Body.Statements.Count)
		Assert.AreEqual("_name", ((p.Getter.Body.Statements[0] cast ReturnStatement).Expression cast ReferenceExpression).Name)

		Assert.IsNotNull(p.Setter, "Setter")
		Assert.AreEqual(1, p.Setter.Body.Statements.Count)

		a = (p.Setter.Body.Statements[0] cast ExpressionStatement).Expression cast BinaryExpression
		Assert.AreEqual(BinaryOperatorType.Assign, a.Operator)
		Assert.AreEqual("_name", (a.Left cast ReferenceExpression).Name)
		Assert.AreEqual("value", (a.Right cast ReferenceExpression).Name)

	[Test]
	def TestWhileStmt1():
		module = ParseTestCase("while_stmt_1.boo")

		ws = module.Globals.Statements[3] cast WhileStatement
		Assert.AreEqual(true, (ws.Condition cast BoolLiteralExpression).Value)
		Assert.AreEqual(4, ws.Block.Statements.Count)

		bs = ws.Block.Statements[3] cast BreakStatement
		condition = bs.Modifier.Condition cast BinaryExpression
		Assert.AreEqual(BinaryOperatorType.Equality, condition.Operator)

	[Test]
	def TestUnpackStmt1():
		module = ParseTestCase("unpack_stmt_1.boo")
		us = module.Globals.Statements[0] cast UnpackStatement
		Assert.AreEqual(2, us.Declarations.Count)
		Assert.AreEqual("arg0", us.Declarations[0].Name)
		Assert.AreEqual("arg1", us.Declarations[1].Name)

		mce = us.Expression cast MethodInvocationExpression
		mre = (mce.Target cast MemberReferenceExpression)
		Assert.AreEqual("GetCommandLineArgs", mre.Name)
		Assert.AreEqual("Environment", (mre.Target cast ReferenceExpression).Name)

	[Test]
	def TestYieldStmt1():
		module = ParseTestCase("yield_stmt_1.boo")

		m = module.Members[0] cast Method
		fs = m.Body.Statements[0] cast ForStatement
		ys = fs.Block.Statements[0] cast YieldStatement
		Assert.AreEqual("i", (ys.Expression cast ReferenceExpression).Name)
		Assert.AreEqual(StatementModifierType.If, ys.Modifier.Type)

	[Test]
	def TestNonSignificantWhitespaceRegions1():
		module = ParseTestCase("nonsignificant_ws_regions_1.boo")

		stmts = module.Globals.Statements
		Assert.AreEqual(2, stmts.Count)

		es = stmts[0] cast ExpressionStatement
		ae = es.Expression cast BinaryExpression
		Assert.AreEqual(BinaryOperatorType.Assign, ae.Operator)
		Assert.AreEqual("a", (ae.Left cast ReferenceExpression).Name)
		Assert.AreEqual(2, (ae.Right cast ListLiteralExpression).Items.Count)

		fs = stmts[1] cast ForStatement
		mce = fs.Iterator cast MethodInvocationExpression
		Assert.AreEqual("map", (mce.Target cast ReferenceExpression).Name)
		Assert.AreEqual(2, mce.Arguments.Count)

		Assert.AreEqual(1, fs.Block.Statements.Count)

	[Test]
	def TripleSingleQuotedStrings():
		"""
		Three single quotes open a triple-quoted string, as three double
		quotes do.
		"""
		// Written with \n rather than a verbatim string: a triple-quoted
		// string keeps the newlines it was given, and this file is checked
		// out with CRLF on Windows.
		code = \
			"def Foo():\n" + \
			"\t'''describe foo'''\n" + \
			"\tpass\n" + \
			"s = '''hello\nworld'''\n" + \
			"d = '''a \"\"\" inside'''\n" + \
			"i = '''one \$(1 + 1) two'''\n"

		module = BooParser.ParseString("tsq", code)
		globals = module.Modules[0].Globals.Statements
		Assert.AreEqual("hello\nworld", Value(globals[0]))
		Assert.AreEqual("a \"\"\" inside", Value(globals[1]))
		Assert.IsInstanceOf[of ExpressionInterpolationExpression](Assigned(globals[2]))
		Assert.AreEqual("describe foo", module.Modules[0].Members[0].Documentation)

	private static def Assigned(stmt as Statement) as Expression:
		return ((stmt cast ExpressionStatement).Expression cast BinaryExpression).Right

	private static def Value(stmt as Statement) as string:
		return (Assigned(stmt) cast StringLiteralExpression).Value

	[Test]
	def IndentedDocstrings():
		"""
		A docstring may sit with the body, the way Python writes one. The
		older unindented position still works.
		"""
		module = BooParser.ParseString("indented", `
def Foo():
	"""describe foo"""
	pass

class Bar:
	"""describe bar"""
	pass
`)
		Assert.AreEqual("describe foo", module.Modules[0].Members[0].Documentation)
		Assert.AreEqual("describe bar", module.Modules[0].Members[1].Documentation)

	[Test]
	def IndentedDocstringSpanningLines():
		module = BooParser.ParseString("indented", `
def Foo():
	"""
	describe foo
	more lines
	"""
	pass

def Bar():
	"""describe bar
	more lines"""
	pass
`)
		Assert.AreEqual("describe foo\nmore lines", module.Modules[0].Members[0].Documentation)
		Assert.AreEqual("describe bar\nmore lines", module.Modules[0].Members[1].Documentation)

	[Test]
	def Docstrings():
		/*
"""
A module can have a docstring.
"""
namespace Foo.Bar
"""
And so can the namespace declaration.
"""

class Person:
"""
A class can have it.
With multiple lines.
"""
	_fname as string
	"""Fields can have one."""
	
	def constructor([required] fname as string):
	"""
	And so can a method or constructor.
	"""
		_fname = fname
		
	FirstName as string:
	"""And why couldn't a property?"""
		get:
			return _fname
interface ICustomer:
"""an interface."""

	def Initialize()
	"""interface method"""
	
	Name as string:
	"""interface property"""
		get
	
enum AnEnum:
"""and so can an enum"""
	AnItem
	"""and its items"""
	AnotherItem
		*/

		module = ParseTestCase("docstrings_1.boo")
		Assert.AreEqual("A module can have a docstring.", module.Documentation)
		Assert.AreEqual("And so can the namespace declaration.", module.Namespace.Documentation)

		person = module.Members[0] cast ClassDefinition
		Assert.AreEqual("A class can have it.\nWith multiple lines.", person.Documentation)
		Assert.AreEqual("Fields can have one.", person.Members[0].Documentation)
		Assert.AreEqual("And so can a method or constructor.", person.Members[1].Documentation)
		Assert.AreEqual("And why couldn't a property?", person.Members[2].Documentation)

		customer = module.Members[1] cast InterfaceDefinition
		Assert.AreEqual("an interface.", customer.Documentation)

		Assert.AreEqual("interface method", customer.Members[0].Documentation)
		Assert.AreEqual("interface property", customer.Members[1].Documentation)

		anEnum = module.Members[2] cast EnumDefinition
		Assert.AreEqual("and so can an enum", anEnum.Documentation)
		Assert.AreEqual("and its items", anEnum.Members[0].Documentation)

	[Test]
	def TestEndKeywordAsIdentifier():
		module = ParseTestCase("end_identifier.boo")
		t = module.Members[0] cast ClassDefinition
		Assert.AreEqual("T", t.Name)
