using System;
using Boo.Ast;
using NUnit.Framework;

namespace Boo.Tests
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
			_module.Package = new Package("Foo.Bar");

			_module.Members.Add(_method = new Method());
			_method.Name = "test";

			_method.Parameters.Add(new ParameterDeclaration("foo", null));
		}
		
		[Test]
		public void TestFullyQualifiedName()
		{
			AssertEquals("Foo.Bar.Module", _module.FullyQualifiedName);
		}
		
		[Test]
		public void TestEnclosingPackage()
		{
			AssertSame(_module.Package, _module.EnclosingPackage);
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
	}
}
