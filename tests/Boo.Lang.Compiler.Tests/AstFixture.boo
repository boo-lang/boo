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
