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
