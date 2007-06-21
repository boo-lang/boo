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

using System;
using System.Xml.Serialization;
using System.Reflection;
using NUnit.Framework;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Parser.Tests
{
	/// <summary>
	/// Test cases for the BooParser class.
	/// </summary>
	[TestFixture]
	public class BooParserTestCase : AbstractParserTestFixture
	{
		[Test]
		public void TestEndSourceLocationForInlineClosures()
		{
			string code = @"foo = { a = 3;
return a; }";
			EnsureClosureEndSourceLocation(code, 2, 11);
		}
		
		[Test]
		public void TestEndSourceLocationForBlockClosures()
		{
			string code = @"
foo = def():
    return a
";
			EnsureClosureEndSourceLocation(code, 3, 13);
		}
		
		void EnsureClosureEndSourceLocation(string code, int line, int column)
		{
			CompileUnit cu = BooParser.ParseString("closures", code);
			Expression e = ((ExpressionStatement)cu.Modules[0].Globals.Statements[0]).Expression;
			BlockExpression cbe = (BlockExpression)((BinaryExpression)e).Right;
			SourceLocation esl = cbe.Body.EndSourceLocation;
			Assert.AreEqual(line, esl.Line);
			Assert.AreEqual(column, esl.Column);
		}		
		
		[Test]
		public void TestParseExpression()
		{
			string code = @"3 + 2 * 5";
			Expression e = BooParser.ParseExpression("test", code);
			Assert.AreEqual("3 + (2 * 5)", e.ToString());
		}
		
		[Test]
		public void TestSimple()
		{
			string fname = GetTestCasePath("simple.boo");
			CompileUnit cu = BooParser.ParseFile(fname);
			Assert.IsNotNull(cu);
			
			Boo.Lang.Compiler.Ast.Module module = cu.Modules[0];
			Assert.IsNotNull(module);
			Assert.AreEqual("simple", module.Name);
			Assert.AreEqual("module doc string", module.Documentation);
			Assert.AreEqual("Empty.simple", module.FullName);
			Assert.AreEqual(fname, module.LexicalInfo.FileName);

			Assert.IsNotNull(module.Namespace);

			Assert.AreEqual("Empty", module.Namespace.Name);
			Assert.AreEqual(4, module.Namespace.LexicalInfo.Line);
			Assert.AreEqual(1, module.Namespace.LexicalInfo.Column);
			Assert.AreEqual(fname, module.Namespace.LexicalInfo.FileName);
		}

		[Test]
		public void TestSimpleClasses()
		{
			string fname = GetTestCasePath("simple_classes.boo");

			Boo.Lang.Compiler.Ast.Module module = BooParser.ParseFile(fname).Modules[0];
			Assert.AreEqual("Foo.Bar", module.Namespace.Name);
			
			Assert.IsNotNull(module.Members);
			Assert.AreEqual(2, module.Members.Count);

			TypeMember cd = module.Members[0];
			Assert.IsTrue(cd is ClassDefinition);
			Assert.AreEqual("Customer", cd.Name);
			Assert.AreEqual("Foo.Bar.Customer", ((TypeDefinition)cd).FullName);
			Assert.AreSame(module.Namespace, ((TypeDefinition)cd).EnclosingNamespace);

			cd = module.Members[1];
			Assert.AreEqual("Person", cd.Name);
		}

		[Test]
		public void TestSimpleClassMethods()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("simple_class_methods.boo");
			Assert.AreEqual("ITL.Content", module.Namespace.Name);
			Assert.AreEqual(1, module.Imports.Count);

			Import i = module.Imports[0];
			Assert.AreEqual("System", i.Namespace);
			Assert.AreEqual(3, i.LexicalInfo.Line);

			Assert.AreEqual(1, module.Members.Count);

			ClassDefinition cd = (ClassDefinition)module.Members[0];
			Assert.AreEqual("Article", cd.Name);

			Assert.AreEqual(3, cd.Members.Count);
			
			Method m = (Method)cd.Members[0];
			Assert.AreEqual("getTitle", m.Name);
			Assert.IsNotNull(m.ReturnType, "ReturnType");
			Assert.AreEqual("string", ((SimpleTypeReference)m.ReturnType).Name);

			m = (Method)cd.Members[1];
			Assert.AreEqual("getBody", m.Name);
			Assert.IsNotNull(m.ReturnType, "ReturnType");
			Assert.AreEqual("string", ((SimpleTypeReference)m.ReturnType).Name);

			m = (Method)cd.Members[2];
			Assert.AreEqual("getTag", m.Name);
			Assert.IsNull(m.ReturnType, "methods without a return type must have ReturnType set to null!");
		}

		[Test]
		public void TestSimpleClassFields()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("simple_class_fields.boo");

			Assert.AreEqual(1, module.Members.Count);
			ClassDefinition cd = (ClassDefinition)module.Members[0];
			
			Assert.AreEqual(3, cd.Members.Count, "Members");

			Field f = (Field)cd.Members[0];
			Assert.AreEqual("_name", f.Name);
			Assert.IsNotNull(f.Type, "Field.Type");
			Assert.AreEqual("string", ((SimpleTypeReference)f.Type).Name);

			Constructor c = (Constructor)cd.Members[1];
			Assert.AreEqual("constructor", c.Name);
			Assert.IsNull(c.ReturnType);
			Assert.AreEqual(1, c.Parameters.Count, "Parameters.Count");
			Assert.AreEqual("name", c.Parameters[0].Name);
			Assert.AreEqual("string", ((SimpleTypeReference)c.Parameters[0].Type).Name);

			Method m = (Method)cd.Members[2];
			Assert.AreEqual("getName", m.Name);
			Assert.IsNull(m.ReturnType);
			Assert.AreEqual(0, m.Parameters.Count);
			Assert.IsNotNull(m.Body, "Body");
			Assert.AreEqual(1, m.Body.Statements.Count);

			ReturnStatement rs = (ReturnStatement)m.Body.Statements[0];
			ReferenceExpression i = (ReferenceExpression)rs.Expression;
			Assert.AreEqual("_name", i.Name);
		}

		[Test]
		public void TestSimpleGlobalDefs()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("simple_global_defs.boo");
			Assert.AreEqual("Math", module.Namespace.Name);
			Assert.AreEqual(3, module.Members.Count);
			Assert.AreEqual("Rational", module.Members[0].Name);
			Assert.AreEqual("pi", module.Members[1].Name);
			Assert.AreEqual("rationalPI", module.Members[2].Name);
			Assert.AreEqual(0, module.Globals.Statements.Count);
		}
		
		[Test]
		public void StatementModifiersOnUnpackStatement()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("stmt_modifiers_3.boo");
			
			Block body = module.Globals;
			Assert.AreEqual(2, body.Statements.Count);
			
			UnpackStatement stmt = (UnpackStatement)body.Statements[0];
			Assert.IsNotNull(stmt.Modifier, "Modifier");
			Assert.AreEqual(StatementModifierType.If, stmt.Modifier.Type);
			Assert.IsTrue(stmt.Modifier.Condition is BoolLiteralExpression);
			Assert.AreEqual(true, ((BoolLiteralExpression)stmt.Modifier.Condition).Value);
			
			RunParserTestCase("stmt_modifiers_3.boo");
		}

		[Test]
		public void TestStmtModifiers1()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("stmt_modifiers_1.boo");

			Method m = (Method)module.Members[0];
			ReturnStatement rs = (ReturnStatement)m.Body.Statements[0];
			Assert.IsNotNull(rs.Modifier, "Modifier");
			Assert.AreEqual(StatementModifierType.If, rs.Modifier.Type);

			BinaryExpression be = (BinaryExpression)rs.Modifier.Condition;
			Assert.AreEqual(BinaryOperatorType.LessThan, be.Operator);
			Assert.AreEqual("n", ((ReferenceExpression)be.Left).Name);
			Assert.AreEqual(2, ((IntegerLiteralExpression)be.Right).Value);
		}

		[Test]
		public void TestStmtModifiers2()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("stmt_modifiers_2.boo");

			ExpressionStatement s = (ExpressionStatement)module.Globals.Statements[0];
			BinaryExpression a = (BinaryExpression)s.Expression;			
			Assert.AreEqual(BinaryOperatorType.Assign, a.Operator);
			Assert.AreEqual("f", ((ReferenceExpression)a.Left).Name);
			Assert.AreEqual(BinaryOperatorType.Division, ((BinaryExpression)a.Right).Operator);
		}

		[Test]
		public void TestStaticMethod()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("static_method.boo");
			Assert.AreEqual(1, module.Members.Count);

			ClassDefinition cd = (ClassDefinition)module.Members[0];
			Assert.AreEqual("Math", cd.Name);
			Assert.AreEqual(1, cd.Members.Count);

			Method m = (Method)cd.Members[0];
			Assert.AreEqual(TypeMemberModifiers.Static, m.Modifiers);
			Assert.AreEqual("square", m.Name);
			Assert.AreEqual("int", ((SimpleTypeReference)m.ReturnType).Name);
		}

		[Test]
		public void TestClass2()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("class_2.boo");
			ClassDefinition cd = (ClassDefinition)module.Members[0];

			Assert.AreEqual(6, cd.Members.Count);
			for (int i=0; i<5; ++i)
			{
				Assert.AreEqual(TypeMemberModifiers.None, cd.Members[i].Modifiers);
			}
			Assert.AreEqual(TypeMemberModifiers.Public | TypeMemberModifiers.Static, cd.Members[5].Modifiers);
		}

		[Test]
		public void TestForStmt1()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("for_stmt_1.boo");

			ForStatement fs = (ForStatement)module.Globals.Statements[0];
			Assert.AreEqual(1, fs.Declarations.Count);
			
			Declaration d = fs.Declarations[0];
			Assert.AreEqual("i", d.Name);
			Assert.IsNull(d.Type);

			ListLiteralExpression lle = (ListLiteralExpression)fs.Iterator;
			Assert.AreEqual(3, lle.Items.Count);
			for (int i=0; i<3; ++i)
			{
				Assert.AreEqual(i+1, ((IntegerLiteralExpression)lle.Items[i]).Value);
			}

			Assert.AreEqual(1, fs.Block.Statements.Count);
			Assert.AreEqual("print", ((ReferenceExpression)((MethodInvocationExpression)((ExpressionStatement)fs.Block.Statements[0]).Expression).Target).Name);
		}

		[Test]
		public void TestRELiteral1()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("re_literal_1.boo");
			Assert.AreEqual(2, module.Globals.Statements.Count);

			ExpressionStatement es = (ExpressionStatement)module.Globals.Statements[1];
			Assert.AreEqual("print", ((ReferenceExpression)((MethodInvocationExpression)es.Expression).Target).Name);

			Assert.AreEqual(StatementModifierType.If, es.Modifier.Type);
			
			BinaryExpression be = (BinaryExpression)es.Modifier.Condition;
			Assert.AreEqual(BinaryOperatorType.Match, be.Operator);
			Assert.AreEqual("s", ((ReferenceExpression)be.Left).Name);
			Assert.AreEqual("/foo/", ((RELiteralExpression)be.Right).Value);
		}

		[Test]
		public void TestRELiteral2()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("re_literal_2.boo");

			StatementCollection stmts = module.Globals.Statements;
			Assert.AreEqual(2, stmts.Count);

			BinaryExpression ae = (BinaryExpression)((ExpressionStatement)stmts[0]).Expression;
			Assert.AreEqual(BinaryOperatorType.Assign, ae.Operator);
			Assert.AreEqual("\"Bamboo\"\n", ((StringLiteralExpression)ae.Right).Value);

			ae = (BinaryExpression)((ExpressionStatement)stmts[1]).Expression;
			Assert.AreEqual(BinaryOperatorType.Assign, ae.Operator);
			Assert.AreEqual("/foo\\(bar\\)/", ((RELiteralExpression)ae.Right).Value);
		}

		[Test]
		public void TestRELiteral3()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("re_literal_3.boo");
			
			StatementCollection stmts = module.Globals.Statements;
			Assert.AreEqual(2, stmts.Count);
			
			BinaryExpression ae = (BinaryExpression)((ExpressionStatement)stmts[0]).Expression;
			Assert.AreEqual(BinaryOperatorType.Assign, ae.Operator);
			Assert.AreEqual("/\\x2f\\u002f/", ((RELiteralExpression)ae.Right).Value);
		}

		[Test]
		public void TestIfElse1()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("if_else_1.boo");

			StatementCollection stmts = module.Globals.Statements;
			Assert.AreEqual(1, stmts.Count);

			IfStatement s = (IfStatement)stmts[0];
			BinaryExpression be = (BinaryExpression)s.Condition;
			Assert.AreEqual(BinaryOperatorType.Match, be.Operator);
			Assert.AreEqual("gets", ((ReferenceExpression)((MethodInvocationExpression)be.Left).Target).Name);
			Assert.AreEqual("/foo/", ((RELiteralExpression)be.Right).Value);
			Assert.AreEqual(3, s.TrueBlock.Statements.Count);
			Assert.IsNull(s.FalseBlock);

			s = (IfStatement)s.TrueBlock.Statements[2];
			be = (BinaryExpression)s.Condition;
			Assert.AreEqual("/bar/", ((RELiteralExpression)be.Right).Value);
			Assert.AreEqual(1, s.TrueBlock.Statements.Count);
			Assert.IsNotNull(s.FalseBlock);
			Assert.AreEqual(1, s.FalseBlock.Statements.Count);
			Assert.AreEqual("foobar, eh?", ((StringLiteralExpression)((MethodInvocationExpression)((ExpressionStatement)s.TrueBlock.Statements[0]).Expression).Arguments[0]).Value);
			Assert.AreEqual("nah?", ((StringLiteralExpression)((MethodInvocationExpression)((ExpressionStatement)s.FalseBlock.Statements[0]).Expression).Arguments[0]).Value);
		}

		[Test]
		public void TestInterface1()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("interface_1.boo");

			Assert.AreEqual(1, module.Members.Count);

			InterfaceDefinition id = (InterfaceDefinition)module.Members[0];
			Assert.AreEqual("IContentItem", id.Name);

			Assert.AreEqual(5, id.Members.Count);
			
			Property p = (Property)id.Members[0];
			Assert.AreEqual("Parent", p.Name);
			Assert.AreEqual("IContentItem", ((SimpleTypeReference)p.Type).Name);
			Assert.IsNotNull(p.Getter, "Getter");
			Assert.IsNull(p.Setter, "Setter");

			p = (Property)id.Members[1];
			Assert.AreEqual("Name", p.Name);
			Assert.AreEqual("string", ((SimpleTypeReference)p.Type).Name);
			Assert.IsNotNull(p.Getter, "Getter");
			Assert.IsNotNull(p.Setter, "Setter");

			Method m = (Method)id.Members[2];
			Assert.AreEqual("SelectItem", m.Name);
			Assert.AreEqual("IContentItem", ((SimpleTypeReference)m.ReturnType).Name);
			Assert.AreEqual("expression", m.Parameters[0].Name);
			Assert.AreEqual("string", ((SimpleTypeReference)m.Parameters[0].Type).Name);

			Assert.AreEqual("Validate", ((Method)id.Members[3]).Name);
			Assert.AreEqual("OnRemove", ((Method)id.Members[4]).Name);
		}

		[Test]
		public void TestEnum1()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("enum_1.boo");

			Assert.AreEqual(2, module.Members.Count);

			EnumDefinition ed = (EnumDefinition)module.Members[0];
			Assert.AreEqual("Priority", ed.Name);
			Assert.AreEqual(3, ed.Members.Count);
			Assert.AreEqual("Low", ed.Members[0].Name);
			Assert.AreEqual("Normal", ed.Members[1].Name);
			Assert.AreEqual("High", ed.Members[2].Name);

			ed = (EnumDefinition)module.Members[1];
			Assert.AreEqual(3, ed.Members.Count);
			Assert.AreEqual("Easy", ed.Members[0].Name);
			Assert.AreEqual(0, ((EnumMember)ed.Members[0]).Initializer.Value);
			Assert.AreEqual("Normal", ed.Members[1].Name);
			Assert.AreEqual(5, ((EnumMember)ed.Members[1]).Initializer.Value);
			Assert.AreEqual("Hard", ed.Members[2].Name);
			Assert.IsNull(((EnumMember)ed.Members[2]).Initializer, "Initializer");
		}

		[Test]
		public void TestProperties1()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("properties_1.boo");

			ClassDefinition cd = (ClassDefinition)module.Members[0];
			Assert.AreEqual("Person", cd.Name);
			Assert.AreEqual("_id", cd.Members[0].Name);
			Assert.AreEqual("_name", cd.Members[1].Name);

			Property p = (Property)cd.Members[3];
			Assert.AreEqual("ID", p.Name);
			Assert.AreEqual("string", ((SimpleTypeReference)p.Type).Name);
			Assert.IsNotNull(p.Getter, "Getter");
			Assert.AreEqual(1, p.Getter.Body.Statements.Count);
			Assert.AreEqual("_id", ((ReferenceExpression)((ReturnStatement)p.Getter.Body.Statements[0]).Expression).Name);
			Assert.IsNull(p.Setter, "Setter");

			p = (Property)cd.Members[4];
			Assert.AreEqual("Name", p.Name);
			Assert.AreEqual("string", ((SimpleTypeReference)p.Type).Name);
			Assert.IsNotNull(p.Getter, "Getter ");
			Assert.AreEqual(1, p.Getter.Body.Statements.Count);
			Assert.AreEqual("_name", ((ReferenceExpression)((ReturnStatement)p.Getter.Body.Statements[0]).Expression).Name);

			Assert.IsNotNull(p.Setter, "Setter");
			Assert.AreEqual(1, p.Setter.Body.Statements.Count);

			BinaryExpression a = (BinaryExpression)((ExpressionStatement)p.Setter.Body.Statements[0]).Expression;
			Assert.AreEqual(BinaryOperatorType.Assign, a.Operator);
			Assert.AreEqual("_name", ((ReferenceExpression)a.Left).Name);
			Assert.AreEqual("value", ((ReferenceExpression)a.Right).Name);
		}

		[Test]
		public void TestWhileStmt1()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("while_stmt_1.boo");

			WhileStatement ws = (WhileStatement)module.Globals.Statements[3];
			Assert.AreEqual(true, ((BoolLiteralExpression)ws.Condition).Value); 
			Assert.AreEqual(4, ws.Block.Statements.Count);

			BreakStatement bs = (BreakStatement)ws.Block.Statements[3];
			BinaryExpression condition = (BinaryExpression)bs.Modifier.Condition;
			Assert.AreEqual(BinaryOperatorType.Equality, condition.Operator);
		}

		[Test]
		public void TestUnpackStmt1()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("unpack_stmt_1.boo");
			UnpackStatement us = (UnpackStatement)module.Globals.Statements[0];
			Assert.AreEqual(2, us.Declarations.Count);
			Assert.AreEqual("arg0", us.Declarations[0].Name);
			Assert.AreEqual("arg1", us.Declarations[1].Name);

			MethodInvocationExpression mce = (MethodInvocationExpression)us.Expression;
			MemberReferenceExpression mre = ((MemberReferenceExpression)mce.Target);
			Assert.AreEqual("GetCommandLineArgs", mre.Name);
			Assert.AreEqual("Environment", ((ReferenceExpression)mre.Target).Name);
		}

		[Test]
		public void TestYieldStmt1()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("yield_stmt_1.boo");

			Method m = (Method)module.Members[0];
			ForStatement fs = (ForStatement)m.Body.Statements[0];
			YieldStatement ys = (YieldStatement)fs.Block.Statements[0];
			Assert.AreEqual("i", ((ReferenceExpression)ys.Expression).Name);
			Assert.AreEqual(StatementModifierType.If, ys.Modifier.Type);

		}

		[Test]
		public void TestNonSignificantWhitespaceRegions1()
		{
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("nonsignificant_ws_regions_1.boo");

			StatementCollection stmts = module.Globals.Statements;
			Assert.AreEqual(2, stmts.Count);

			ExpressionStatement es = (ExpressionStatement)stmts[0];
			BinaryExpression ae = (BinaryExpression)es.Expression;
			Assert.AreEqual(BinaryOperatorType.Assign, ae.Operator);
			Assert.AreEqual("a", ((ReferenceExpression)ae.Left).Name);
			Assert.AreEqual(2, ((ListLiteralExpression)ae.Right).Items.Count);

			ForStatement fs = (ForStatement)stmts[1];
			MethodInvocationExpression mce = (MethodInvocationExpression)fs.Iterator;
			Assert.AreEqual("map", ((ReferenceExpression)mce.Target).Name);
			Assert.AreEqual(2, mce.Arguments.Count);

			Assert.AreEqual(1, fs.Block.Statements.Count);
		}
		
		[Test]
		public void Docstrings()
		{
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
			
			Boo.Lang.Compiler.Ast.Module module = ParseTestCase("docstrings_1.boo");
			Assert.AreEqual("A module can have a docstring.", module.Documentation);
			Assert.AreEqual("And so can the namespace declaration.", module.Namespace.Documentation);
			
			ClassDefinition person = (ClassDefinition)module.Members[0];
			Assert.AreEqual("A class can have it.\nWith multiple lines.", person.Documentation);
			Assert.AreEqual("Fields can have one.", person.Members[0].Documentation);
			Assert.AreEqual("\tAnd so can a method or constructor.\n\t", person.Members[1].Documentation);
			Assert.AreEqual("And why couldn't a property?", person.Members[2].Documentation);
			
			InterfaceDefinition customer = (InterfaceDefinition)module.Members[1];
			Assert.AreEqual("an interface.", customer.Documentation);
			
			Assert.AreEqual("interface method", customer.Members[0].Documentation);
			Assert.AreEqual("interface property", customer.Members[1].Documentation);
			
			EnumDefinition anEnum = (EnumDefinition)module.Members[2];
			Assert.AreEqual("and so can an enum", anEnum.Documentation);
			Assert.AreEqual("and its items", anEnum.Members[0].Documentation);
			
		}
	}
}
