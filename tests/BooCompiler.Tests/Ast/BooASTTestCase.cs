#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace BooCompiler.Tests.Ast
{
	/// <summary>
	/// Teste case para as classes que compem a 
	/// AST.
	/// </summary>
	[TestFixture]
	public class BooASTTestCase : Assertion
	{
		CompileUnit _unit;

		Module _module;

		Method _method;

		[SetUp]
		public void SetUp()
		{
			_unit = new CompileUnit();
			_unit.Modules.Add(_module = new Module());
			_module.Name = "Module";
			_module.Namespace = new NamespaceDeclaration("Foo.Bar");

			_module.Members.Add(_method = new Method());
			_method.Name = "test";

			_method.Parameters.Add(new ParameterDeclaration("foo", null));
		}
		
		[Test]
		public void TestFullyQualifiedName()
		{
			AssertEquals("Foo.Bar.Module", _module.FullName);
		}
		
		[Test]
		public void TestEnclosingNamespace()
		{
			AssertSame(_module.Namespace, _module.EnclosingNamespace);
		}

		[Test]
		public void TestParentNode()
		{
			AssertNull(_unit.ParentNode);
			AssertSame(_unit, _module.ParentNode);
			AssertSame(_module, _method.ParentNode);
			AssertSame(_method, _method.Parameters[0].ParentNode);
		}

		[Test]
		public void TestDeclaringMethod()
		{
			foreach (ParameterDeclaration pd in _method.Parameters)
			{
				AssertSame("DeclaringMethod", _method, pd.ParentNode);
			}
		}
		
		[Test]
		public void TestCloneBinaryExpression()
		{
			BinaryExpression be = new BinaryExpression(BinaryOperatorType.Assign,
			                                new ReferenceExpression("foo"),
			                                new StringLiteralExpression("bar"));
			AssertSame(be, be.Left.ParentNode);
			AssertSame(be, be.Right.ParentNode);
			
			BinaryExpression clone = be.CloneNode();
			AssertSame("clone.Left", clone, clone.Left.ParentNode);
			AssertSame("clone.Right", clone, clone.Right.ParentNode);
		}
	}
}
