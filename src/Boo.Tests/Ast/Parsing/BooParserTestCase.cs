#region license
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
#endregion

using System;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Boo.Ast;
using Boo.Ast.Parsing;
using Boo.Tests;

namespace Boo.Tests.Ast.Parsing
{
	/// <summary>
	/// Test cases for the BooParser class.
	/// </summary>
	[TestFixture]
	public class BooParserTestCase : Assertion
	{
		[Test]
		public void TestSimple()
		{
			string fname = BooTestCaseUtil.GetTestCasePath("simple.boo");
			CompileUnit cu = BooParser.ParseFile(fname);
			AssertNotNull(cu);

			Boo.Ast.Module module = cu.Modules[0];
			AssertNotNull(module);
			AssertEquals("simple", module.Name);
			AssertEquals("module doc string", module.Documentation);
			AssertEquals("Empty.simple", module.FullyQualifiedName);
			AssertEquals(fname, module.LexicalInfo.FileName);

			AssertNotNull(module.Package);

			AssertEquals("Empty", module.Package.Name);
			AssertEquals(4, module.Package.LexicalInfo.Line);
			AssertEquals(1, module.Package.LexicalInfo.StartColumn);
			AssertEquals(fname, module.Package.LexicalInfo.FileName);
		}

		[Test]
		public void TestSimpleClasses()
		{
			string fname = BooTestCaseUtil.GetTestCasePath("simple_classes.boo");

			Boo.Ast.Module module = BooParser.ParseFile(fname).Modules[0];
			AssertEquals("Foo.Bar", module.Package.Name);
			
			AssertNotNull(module.Members);
			AssertEquals(2, module.Members.Count);

			TypeMember cd = module.Members[0];
			Assert(cd is ClassDefinition);
			AssertEquals("Customer", cd.Name);
			AssertEquals("Foo.Bar.Customer", ((TypeDefinition)cd).FullyQualifiedName);
			AssertSame(module.Package, ((TypeDefinition)cd).EnclosingPackage);

			cd = module.Members[1];
			AssertEquals("Person", cd.Name);
		}

		[Test]
		public void TestSimpleClassMethods()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("simple_class_methods.boo");
			AssertEquals("ITL.Content", module.Package.Name);
			AssertEquals(1, module.Using.Count);

			Using i = module.Using[0];
			AssertEquals("System", i.Namespace);
			AssertEquals(3, i.LexicalInfo.Line);

			AssertEquals(1, module.Members.Count);

			ClassDefinition cd = (ClassDefinition)module.Members[0];
			AssertEquals("Article", cd.Name);

			AssertEquals(3, cd.Members.Count);
			
			Method m = (Method)cd.Members[0];
			AssertEquals("getTitle", m.Name);
			AssertNotNull("ReturnType", m.ReturnType);
			AssertEquals("string", m.ReturnType.Name);

			m = (Method)cd.Members[1];
			AssertEquals("getBody", m.Name);
			AssertNotNull("ReturnType", m.ReturnType);
			AssertEquals("string", m.ReturnType.Name);

			m = (Method)cd.Members[2];
			AssertEquals("getTag", m.Name);
			AssertNull("M�todos sem tipo de retorno deve ter refer�ncia ReturnType nula!", 
				m.ReturnType);
		}

		[Test]
		public void TestSimpleClassFields()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("simple_class_fields.boo");

			AssertEquals(1, module.Members.Count);
			ClassDefinition cd = (ClassDefinition)module.Members[0];
			
			AssertEquals("Members", 3, cd.Members.Count);

			Field f = (Field)cd.Members[0];
			AssertEquals("_name", f.Name);
			AssertNotNull("Field.Type", f.Type);
			AssertEquals("string", f.Type.Name);

			Constructor c = (Constructor)cd.Members[1];
			AssertEquals("constructor", c.Name);
			AssertNull(c.ReturnType);
			AssertEquals("Parameters.Count", 1, c.Parameters.Count);
			AssertEquals("name", c.Parameters[0].Name);
			AssertEquals("string", c.Parameters[0].Type.Name);

			Method m = (Method)cd.Members[2];
			AssertEquals("getName", m.Name);
			AssertNull(m.ReturnType);
			AssertEquals(0, m.Parameters.Count);
			AssertNotNull("Body", m.Body);
			AssertEquals(1, m.Body.Statements.Count);

			ReturnStatement rs = (ReturnStatement)m.Body.Statements[0];
			ReferenceExpression i = (ReferenceExpression)rs.Expression;
			AssertEquals("_name", i.Name);
		}

		[Test]
		public void TestSimpleGlobalDefs()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("simple_global_defs.boo");
			AssertEquals("Math", module.Package.Name);
			AssertEquals(3, module.Members.Count);
			AssertEquals("Rational", module.Members[0].Name);
			AssertEquals("pi", module.Members[1].Name);
			AssertEquals("rationalPI", module.Members[2].Name);
			AssertEquals(0, module.Globals.Statements.Count);
		}

		/* s6.boo n�o traz nada de novo
		[Test]
		public void TestS6()
		{
		}
		*/

		[Test]
		public void TestGlobalDefs2()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("global_defs_2.boo");			

			Method m = (Method)module.Members[0];
			AssertEquals("square", m.Name);
			AssertEquals(1, m.Parameters.Count);
			AssertEquals("x", m.Parameters[0].Name);
			AssertEquals("int", m.Parameters[0].Type.Name);
			AssertEquals("int", m.ReturnType.Name);
			
			Block b = m.Body;
			AssertEquals(1, b.Statements.Count);

			ReturnStatement rs = b.Statements[0] as ReturnStatement;
			AssertNotNull("ReturnStatement", rs);

			BinaryExpression bs = rs.Expression as BinaryExpression;
			AssertNotNull("BinaryExpression", bs);

			AssertEquals(BinaryOperatorType.Multiply, bs.Operator);
			Assert(bs.Left is ReferenceExpression);
			Assert(bs.Right is ReferenceExpression);
			AssertEquals("x", ((ReferenceExpression)bs.Left).Name);
			AssertEquals("x", ((ReferenceExpression)bs.Right).Name);

			m = (Method)module.Members[1];
			b = m.Body;

			AssertEquals(2, b.Statements.Count);

			ExpressionStatement es = b.Statements[0] as ExpressionStatement;
			AssertNotNull("ExpressionStatement", es);

			MethodInvocationExpression mce = es.Expression as MethodInvocationExpression;
			AssertNotNull("MethodInvocationExpression", mce);
			AssertEquals("print", ((ReferenceExpression)mce.Target).Name);
			AssertEquals(1, mce.Arguments.Count);
			Assert(mce.Arguments[0] is MethodInvocationExpression);
			mce = (MethodInvocationExpression)mce.Arguments[0];
			AssertEquals(3, mce.Arguments.Count);
			AssertEquals("x = {0}, y = {1}", ((StringLiteralExpression)mce.Arguments[0]).Value);

			rs = b.Statements[1] as ReturnStatement;
			AssertNotNull("rs", rs);
			bs = rs.Expression as BinaryExpression;
			AssertNotNull("bs", bs);

			AssertEquals(BinaryOperatorType.Add, bs.Operator);			
			AssertEquals("x", ((ReferenceExpression)bs.Left).Name);
			AssertEquals("y", ((ReferenceExpression)bs.Right).Name);
		}

		[Test]
		public void TestGlobalStmts1()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("global_stmts_1.boo");
			
			Block g = module.Globals;
			AssertEquals(1, g.Statements.Count);

			ExpressionStatement es = (ExpressionStatement)g.Statements[0];
			MethodInvocationExpression mce = (MethodInvocationExpression)es.Expression;
			AssertEquals(1, mce.Arguments.Count);

			BinaryExpression be = (BinaryExpression)mce.Arguments[0];
			AssertEquals(BinaryOperatorType.Add, be.Operator);

			mce = (MethodInvocationExpression)be.Left;
			IntegerLiteralExpression ile = (IntegerLiteralExpression)mce.Arguments[0];
			AssertEquals("3", ile.Value);

			mce = (MethodInvocationExpression)be.Right;
			ile = (IntegerLiteralExpression)mce.Arguments[0];
			AssertEquals("5", ile.Value);
		}

		[Test]
		public void TestStmtModifiers1()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("stmt_modifiers_1.boo");

			Method m = (Method)module.Members[0];
			ReturnStatement rs = (ReturnStatement)m.Body.Statements[0];
			AssertNotNull("Modifier", rs.Modifier);
			AssertEquals(StatementModifierType.If, rs.Modifier.Type);

			BinaryExpression be = (BinaryExpression)rs.Modifier.Condition;
			AssertEquals(BinaryOperatorType.LessThan, be.Operator);
			AssertEquals("n", ((ReferenceExpression)be.Left).Name);
			AssertEquals("2", ((IntegerLiteralExpression)be.Right).Value);
		}

		[Test]
		public void TestStmtModifiers2()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("stmt_modifiers_2.boo");

			ExpressionStatement s = (ExpressionStatement)module.Globals.Statements[0];
			BinaryExpression a = (BinaryExpression)s.Expression;			
			AssertEquals(BinaryOperatorType.Assign, a.Operator);
			AssertEquals("f", ((ReferenceExpression)a.Left).Name);
			AssertEquals(BinaryOperatorType.Divide, ((BinaryExpression)a.Right).Operator);
		}

		[Test]
		public void TestStaticMethod()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("static_method.boo");
			AssertEquals(1, module.Members.Count);

			ClassDefinition cd = (ClassDefinition)module.Members[0];
			AssertEquals("Math", cd.Name);
			AssertEquals(1, cd.Members.Count);

			Method m = (Method)cd.Members[0];
			AssertEquals(TypeMemberModifiers.Static, m.Modifiers);
			AssertEquals("square", m.Name);
			AssertEquals("int", m.ReturnType.Name);
		}

		/*
		 * Nada de novo
		[Test]
		public void TestS12()
		{
		}
		*/

		[Test]
		public void TestClass2()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("class_2.boo");
			ClassDefinition cd = (ClassDefinition)module.Members[0];

			AssertEquals(6, cd.Members.Count);
			for (int i=0; i<5; ++i)
			{
				AssertEquals(TypeMemberModifiers.None, cd.Members[i].Modifiers);
			}
			AssertEquals(TypeMemberModifiers.Public | TypeMemberModifiers.Static, cd.Members[5].Modifiers);
		}

		[Test]
		public void TestForStmt1()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("for_stmt_1.boo");

			ForStatement fs = (ForStatement)module.Globals.Statements[0];
			AssertEquals(1, fs.Declarations.Count);
			
			Declaration d = fs.Declarations[0];
			AssertEquals("i", d.Name);
			AssertNull(d.Type);

			ListLiteralExpression lle = (ListLiteralExpression)fs.Iterator;
			AssertEquals(3, lle.Items.Count);
			for (int i=0; i<3; ++i)
			{
				AssertEquals((i+1).ToString(), ((IntegerLiteralExpression)lle.Items[i]).Value);
			}

			AssertEquals(1, fs.Statements.Count);
			AssertEquals("print", ((ReferenceExpression)((MethodInvocationExpression)((ExpressionStatement)fs.Statements[0]).Expression).Target).Name);
		}

		[Test]
		public void TestRELiteral1()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("re_literal_1.boo");
			AssertEquals(2, module.Globals.Statements.Count);

			ExpressionStatement es = (ExpressionStatement)module.Globals.Statements[1];
			AssertEquals("print", ((ReferenceExpression)((MethodInvocationExpression)es.Expression).Target).Name);

			AssertEquals(StatementModifierType.If, es.Modifier.Type);
			
			BinaryExpression be = (BinaryExpression)es.Modifier.Condition;
			AssertEquals(BinaryOperatorType.Match, be.Operator);
			AssertEquals("s", ((ReferenceExpression)be.Left).Name);
			AssertEquals("/foo/", ((RELiteralExpression)be.Right).Value);
		}

		[Test]
		public void TestRELiteral2()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("re_literal_2.boo");

			StatementCollection stmts = module.Globals.Statements;
			AssertEquals(2, stmts.Count);

			BinaryExpression ae = (BinaryExpression)((ExpressionStatement)stmts[0]).Expression;
			AssertEquals(BinaryOperatorType.Assign, ae.Operator);
			AssertEquals("\\\"Bamboo\\\"\\n", ((StringLiteralExpression)ae.Right).Value);

			ae = (BinaryExpression)((ExpressionStatement)stmts[1]).Expression;
			AssertEquals(BinaryOperatorType.Assign, ae.Operator);
			AssertEquals("/foo\\(bar\\)/", ((RELiteralExpression)ae.Right).Value);
		}

		[Test]
		public void TestIfElse1()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("if_else_1.boo");

			StatementCollection stmts = module.Globals.Statements;
			AssertEquals(1, stmts.Count);

			IfStatement s = (IfStatement)stmts[0];
			BinaryExpression be = (BinaryExpression)s.Expression;
			AssertEquals(BinaryOperatorType.Match, be.Operator);
			AssertEquals("gets", ((ReferenceExpression)((MethodInvocationExpression)be.Left).Target).Name);
			AssertEquals("/foo/", ((RELiteralExpression)be.Right).Value);
			AssertEquals(3, s.TrueBlock.Statements.Count);
			AssertNull(s.FalseBlock);

			s = (IfStatement)s.TrueBlock.Statements[2];
			be = (BinaryExpression)s.Expression;
			AssertEquals("/bar/", ((RELiteralExpression)be.Right).Value);
			AssertEquals(1, s.TrueBlock.Statements.Count);
			AssertNotNull(s.FalseBlock);
			AssertEquals(1, s.FalseBlock.Statements.Count);
			AssertEquals("foobar, eh?", ((StringLiteralExpression)((MethodInvocationExpression)((ExpressionStatement)s.TrueBlock.Statements[0]).Expression).Arguments[0]).Value);
			AssertEquals("nah?", ((StringLiteralExpression)((MethodInvocationExpression)((ExpressionStatement)s.FalseBlock.Statements[0]).Expression).Arguments[0]).Value);
		}

		[Test]
		public void TestInterface1()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("interface_1.boo");

			AssertEquals(1, module.Members.Count);

			InterfaceDefinition id = (InterfaceDefinition)module.Members[0];
			AssertEquals("IContentItem", id.Name);

			AssertEquals(5, id.Members.Count);
			
			Property p = (Property)id.Members[0];
			AssertEquals("Parent", p.Name);
			AssertEquals("IContentItem", p.Type.Name);
			AssertNotNull("Getter", p.Getter);
			AssertNull("Setter", p.Setter);

			p = (Property)id.Members[1];
			AssertEquals("Name", p.Name);
			AssertEquals("string", p.Type.Name);
			AssertNotNull("Getter", p.Getter);
			AssertNotNull("Setter", p.Setter);

			Method m = (Method)id.Members[2];
			AssertEquals("SelectItem", m.Name);
			AssertEquals("IContentItem", m.ReturnType.Name);
			AssertEquals("expression", m.Parameters[0].Name);
			AssertEquals("string", m.Parameters[0].Type.Name);

			AssertEquals("Validate", ((Method)id.Members[3]).Name);
			AssertEquals("OnRemove", ((Method)id.Members[4]).Name);
		}

		[Test]
		public void TestEnum1()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("enum_1.boo");

			AssertEquals(2, module.Members.Count);

			EnumDefinition ed = (EnumDefinition)module.Members[0];
			AssertEquals("Priority", ed.Name);
			AssertEquals(3, ed.Members.Count);
			AssertEquals("Low", ed.Members[0].Name);
			AssertEquals("Normal", ed.Members[1].Name);
			AssertEquals("High", ed.Members[2].Name);

			ed = (EnumDefinition)module.Members[1];
			AssertEquals(3, ed.Members.Count);
			AssertEquals("Easy", ed.Members[0].Name);
			AssertEquals("0", ((EnumMember)ed.Members[0]).Initializer.Value);
			AssertEquals("Normal", ed.Members[1].Name);
			AssertEquals("5", ((EnumMember)ed.Members[1]).Initializer.Value);
			AssertEquals("Hard", ed.Members[2].Name);
			AssertNull("Initializer", ((EnumMember)ed.Members[2]).Initializer);
		}

		[Test]
		public void TestProperties1()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("properties_1.boo");

			ClassDefinition cd = (ClassDefinition)module.Members[0];
			AssertEquals("Person", cd.Name);
			AssertEquals("_id", cd.Members[0].Name);
			AssertEquals("_name", cd.Members[1].Name);

			Property p = (Property)cd.Members[3];
			AssertEquals("ID", p.Name);
			AssertEquals("string", p.Type.Name);
			AssertNotNull("Getter", p.Getter);
			AssertEquals(1, p.Getter.Body.Statements.Count);
			AssertEquals("_id", ((ReferenceExpression)((ReturnStatement)p.Getter.Body.Statements[0]).Expression).Name);
			AssertNull("Setter", p.Setter);

			p = (Property)cd.Members[4];
			AssertEquals("Name", p.Name);
			AssertEquals("string", p.Type.Name);
			AssertNotNull("Getter ", p.Getter);
			AssertEquals(1, p.Getter.Body.Statements.Count);
			AssertEquals("_name", ((ReferenceExpression)((ReturnStatement)p.Getter.Body.Statements[0]).Expression).Name);

			AssertNotNull("Setter", p.Setter);
			AssertEquals(1, p.Setter.Body.Statements.Count);

			BinaryExpression a = (BinaryExpression)((ExpressionStatement)p.Setter.Body.Statements[0]).Expression;
			AssertEquals(BinaryOperatorType.Assign, a.Operator);
			AssertEquals("_name", ((ReferenceExpression)a.Left).Name);
			AssertEquals("value", ((ReferenceExpression)a.Right).Name);
		}

		[Test]
		public void TestWhileStmt1()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("while_stmt_1.boo");

			WhileStatement ws = (WhileStatement)module.Globals.Statements[3];
			BinaryExpression condition = (BinaryExpression)ws.Condition;
			AssertEquals(BinaryOperatorType.Inequality, condition.Operator);
			AssertEquals("guess", ((ReferenceExpression)condition.Left).Name);
			AssertEquals("number", ((ReferenceExpression)condition.Right).Name);

			AssertEquals(4, ws.Statements.Count);

			BreakStatement bs = (BreakStatement)ws.Statements[3];
			condition = (BinaryExpression)bs.Modifier.Condition;
			AssertEquals(BinaryOperatorType.Equality, condition.Operator);
		}

		[Test]
		public void TestCppComments()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("cpp_comments.boo");
			AssertEquals("CPlusPlusStyleComments", module.Package.Name);
		}

		[Test]
		public void TestUnpackStmt1()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("unpack_stmt_1.boo");
			UnpackStatement us = (UnpackStatement)module.Globals.Statements[0];
			AssertEquals(2, us.Declarations.Count);
			AssertEquals("arg0", us.Declarations[0].Name);
			AssertEquals("arg1", us.Declarations[1].Name);

			MethodInvocationExpression mce = (MethodInvocationExpression)us.Expression;
			MemberReferenceExpression mre = ((MemberReferenceExpression)mce.Target);
			AssertEquals("GetCommandLineArgs", mre.Name);
			AssertEquals("Environment", ((ReferenceExpression)mre.Target).Name);
		}

		[Test]
		public void TestYieldStmt1()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("yield_stmt_1.boo");

			Method m = (Method)module.Members[0];
			ForStatement fs = (ForStatement)m.Body.Statements[0];
			YieldStatement ys = (YieldStatement)fs.Statements[0];
			AssertEquals("i", ((ReferenceExpression)ys.Expression).Name);
			AssertEquals(StatementModifierType.If, ys.Modifier.Type);

		}

		[Test]
		public void TestNonSignificantWhitespaceRegions1()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("nonsignificant_ws_regions_1.boo");

			StatementCollection stmts = module.Globals.Statements;
			AssertEquals(2, stmts.Count);

			ExpressionStatement es = (ExpressionStatement)stmts[0];
			BinaryExpression ae = (BinaryExpression)es.Expression;
			AssertEquals(BinaryOperatorType.Assign, ae.Operator);
			AssertEquals("a", ((ReferenceExpression)ae.Left).Name);
			AssertEquals(2, ((ListLiteralExpression)ae.Right).Items.Count);

			ForStatement fs = (ForStatement)stmts[1];
			MethodInvocationExpression mce = (MethodInvocationExpression)fs.Iterator;
			AssertEquals("map", ((ReferenceExpression)mce.Target).Name);
			AssertEquals(2, mce.Arguments.Count);

			AssertEquals(1, fs.Statements.Count);
		}

		[Test]
		public void TestTuples1()
		{
			Boo.Ast.Module module = BooTestCaseUtil.ParseTestCase("tuples_1.boo");

			StatementCollection sc = module.Globals.Statements;
			AssertEquals(4, sc.Count);

			BinaryExpression ae = (BinaryExpression)((ExpressionStatement)sc[0]).Expression;
			AssertEquals("names", ((ReferenceExpression)ae.Left).Name);

			TupleLiteralExpression tle = (TupleLiteralExpression)ae.Right;
			AssertEquals(3, tle.Items.Count);

			ae = (BinaryExpression)((ExpressionStatement)sc[1]).Expression;
			tle = (TupleLiteralExpression)ae.Right;
			AssertEquals(3, tle.Items.Count);

			ae = (BinaryExpression)((ExpressionStatement)sc[3]).Expression;
			tle = (TupleLiteralExpression)ae.Right;
			AssertEquals(1, tle.Items.Count);
		}

		[Test]
		public void TestExpressions1()
		{
			RunXmlTestCase("expressions_1.boo");
		}

		[Test]
		public void TestExpressions2()
		{
			RunXmlTestCase("expressions_2.boo");
		}

		[Test]
		public void TestExpressions3()
		{
			RunXmlTestCase("expressions_3.boo");
		}

		[Test]
		public void TestBoolLiterals()
		{
			RunXmlTestCase("bool_literals.boo");
		}

		[Test]
		public void TestNullLiteral()
		{
			RunXmlTestCase("null_literal.boo");
		}

		[Test]
		public void TestSelf()
		{
			RunXmlTestCase("self.boo");
		}

		[Test]
		public void TestTernaryOperator()
		{
			RunXmlTestCase("ternary_operator.boo");
		}

		[Test]
		public void TestStringInterpolation()
		{
			RunXmlTestCase("string_interpolation.boo");
		}

		[Test]
		public void TestBaseMembers()
		{
			RunXmlTestCase("base_types.boo");
		}

		[Test]
		public void TestTimeSpanLiteral()
		{
			RunXmlTestCase("timespan.boo");
		}

		[Test]
		public void TestAssert()
		{
			RunXmlTestCase("assert.boo");
		}

		[Test]
		public void TestRichAssign()
		{
			RunXmlTestCase("rich_assign.boo");
		}

		[Test]
		public void TestTryCatchRetry()
		{
			RunXmlTestCase("try_catch_retry.boo");
		}

		[Test]
		public void TestSlicing()
		{
			RunXmlTestCase("slicing.boo");
		}

		[Test]
		public void TestDict()
		{
			RunXmlTestCase("dict.boo");
		}

		[Test]
		public void TestListDisplay()
		{
			RunXmlTestCase("list_display.boo");
		}

		[Test]
		public void TestTuples2()
		{
			RunXmlTestCase("tuples_2.boo");
		}

		[Test]
		public void TestMethodCalls()
		{
			RunXmlTestCase("method_calls.boo");
		}

		[Test]
		public void TestAttributes()
		{
			RunXmlTestCase("attributes.boo");
		}

		[Test]
		public void TestAttributeWithNamedParameters()
		{
			RunXmlTestCase("named_parameters_1.boo");
		}

		[Test]
		public void TestConstructorWithNamedParameters()
		{
			RunXmlTestCase("named_parameters_2.boo");
		}

		[Test]
		public void TestUsing()
		{
			RunXmlTestCase("using.boo");
		}

		void RunXmlTestCase(string sample)
		{
			BooTestCaseUtil.RunXmlTestCase(sample);			
		}
	}
}
