#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.Tests

import System
import Boo.Lang.Compiler.Ast
import NUnit.Framework

[TestFixture]
class AstTestFixture:
	_unit as CompileUnit
	_module as Module
	_method as Method	

	[SetUp]
	def SetUp():
		_module = Module(Name: "Module",
							Namespace: NamespaceDeclaration("Foo.Bar"))
		
		_unit = CompileUnit()
		_unit.Modules.Add(_module)
		
		_module.Members.Add(_method = Method(Name: "test"))
		_method.Parameters.Add(ParameterDeclaration("foo", null))
	
	[Test]
	def TestFullyQualifiedName():
		Assert.AreEqual("Foo.Bar.Module", _module.FullName)
	
	[Test]
	def TestEnclosingNamespace():
		Assert.AreSame(_module.Namespace, _module.EnclosingNamespace)

	[Test]
	def TestParentNode():
		Assert.IsNull(_unit.ParentNode)
		Assert.AreSame(_unit, _module.ParentNode)
		Assert.AreSame(_module, _method.ParentNode)
		Assert.AreSame(_method, _method.Parameters[0].ParentNode)

	[Test]
	def TestDeclaringMethod():
		for pd in _method.Parameters:
			Assert.AreSame(_method, pd.ParentNode, "parameter.ParentNode")
	
	[Test]
	def TestCloneBinaryExpression():
		be = BinaryExpression(BinaryOperatorType.Assign,
									ReferenceExpression("foo"),
									StringLiteralExpression("bar"))
		Assert.AreSame(be, be.Left.ParentNode);
		Assert.AreSame(be, be.Right.ParentNode);
		
		clone = be.CloneNode()
		Assert.AreSame(clone, clone.Left.ParentNode, "clone.Left")
		Assert.AreSame(clone, clone.Right.ParentNode, "clone.Right")
