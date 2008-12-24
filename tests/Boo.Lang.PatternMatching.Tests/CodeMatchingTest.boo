namespace Boo.Lang.PatternMatching.Tests

import NUnit.Framework
import Boo.Lang.PatternMatching
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

[TestFixture]
class CodeMatchingTest:
	
	[Test]
	def TestAssignment():
		
		code = [| a = 21*2 |]
		
		Assert.AreEqual("a", variableName(code))
		Assert.AreEqual(code.Right, rvalue(code))
		
	[Test]
	def TestSlicing():
		
		code = [| a[b] |]
		match code:
			case [| $target[$arg] |]:
				match target:
					case ReferenceExpression(Name: "a"):
						pass
				match arg:
					case ReferenceExpression(Name: "b"):
						pass
						
	[Test]
	def TestTryCast():
		code = [| a as int |]
		match code:
			case [| $name as $type |]:
				assert name.ToString() == "a"
				assert type.ToString() == "int"
		
	def variableName(code as Expression):
		match code:
			case [| $(ReferenceExpression(Name: l)) = $_ |]:
				return l
				
	def rvalue(code as Expression):
		match code:
			case [| $_ = $r |]:
				return r