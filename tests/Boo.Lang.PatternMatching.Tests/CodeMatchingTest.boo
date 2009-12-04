namespace Boo.Lang.PatternMatching.Tests

import NUnit.Framework
import Boo.Lang.PatternMatching
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

[TestFixture]
class CodeMatchingTest:
	
	[Test]
	def MacroApplication():
		Assert.AreEqual("42", firstPrintArgument([| print 42 |]).ToString())
		
	[Test]
	[ExpectedException(MatchError)]
	def MacroApplicationWithMismatchedArguments():
		firstPrintArgument([| print "arg1", "arg2" |])
		
	[Test]
	[ExpectedException(MatchError)]
	def WrongMacroApplicationNae():
		firstPrintArgument([| print_ "arg" |])
		
	[Test]
	def Assignment():
		code = [| a = 21*2 |]
		Assert.AreEqual("a", variableName(code))
		Assert.AreEqual(code.Right, rvalue(code))
		
	[Test]
	def Slicing():
		code = [| a[b] |]
		match code:
			case [| $target[$arg] |]:
				assert [| a |].Matches(target)
				assert [| b |].Matches(arg)
						
	[Test]
	def TryCast():
		code = [| a as int |]
		match code:
			case [| $name as $type |]:
				assert name.ToString() == "a"
				assert type.ToString() == "int"
				
	[Test]
	def NoArgInvocationPatternMatchesAnyInvocation():
		assert methodTarget([| foo() |]) == "foo"
		assert methodTarget([| bar(42) |]) == "bar"
		
	[Test]
	def InvocationPatternWithArguments():
		assert delegateMethod([| ThreadStart(null, __addressof__(foo)) |]) == "foo"
	
	[Test]
	[ExpectedException(MatchError)]
	def InvocationPatternWithArgumentsMismatch():
		delegateMethod([| ThreadStart(null) |])
		
		
	[Test]
	def BoolLiteral():
		assert "false" == boolLiteral([| false |])
		assert "true" == boolLiteral([| true |])
		assert "42" == boolLiteral([| true or 42 |])
		
	[Test]
	def FullyQualifiedMethodName():
		Assert.AreEqual("foo.bar", methodTarget([| foo.bar() |]))
		
	[Test]
	def SimpleMemberReference():
		code = [| foo.bar |]
		match code:
			case [| foo.bar |]:
				pass
				
	[Test]
	def SingleReference():
		code = [| foo |]
		match code:
			case [| foo |]:
				pass
		
	[Test]
	def FullyQualifiedNameMatching():
		code = [| foo.bar() |]
		match code:
			case [| foo.bar() |]:
				pass
				
	[Test]
	def FullyQualifiedNameMatchingWithCapture():
		code = [| foo.bar(1, 2) |]
		match code:
			case [| foo.bar($first, $second) |]:
				assert [| 1 |].Matches(first)
				assert [| 2 |].Matches(second)
				
	[Test]
	def MemberReferenceTargetCapture():
		code = [| foo.bar() |]
		match code:
			case [| $target.bar() |]:
				assert [| foo |].Matches(target)
				
	[Test]
	def OmittedTargetReferenceExpression():
		code = [| .bar() |]
		match code:
			case [| .bar() |]:
				pass
				
	[Test]
	def StringLiteralExpression():
		code = [| "foo" |]
		match code:
			case [| "foo" |]:
				pass
				
	[Test]
	[ExpectedException(MatchError)]
	def StringLiteralExpressionMismatch():
		code = [| "foo" |]
		match code:
			case [| "bar" |]:
				pass
				
	[Test]
	def IntegerLiteralExpression():
		code = [| 42 |]
		match code:
			case [| 42 |]:
				pass
	
	[Test]
	[ExpectedException(MatchError)]
	def IntegerLiteralExpressionMismatch():
		code = [| 42 |]
		match code:
			case [| 1 |]:
				pass
				
	[Test]
	def Typeof():
		code = [| typeof(Foo) |]
		match code:
			case [| typeof($type) |]:
				assert SimpleTypeReference("Foo").Matches(type)
				
	[Test]
	def Self():
		code = [| self |]
		match code:
			case [| self |]:
				pass
		
	def boolLiteral(code as Expression):
		match code:
			case [| true |]:
				return "true"
			case [| false |]:
				return "false"
			case [| true or $e |]:
				return e.ToString()
		
	def delegateMethod(code as Expression):
		match code:
			case [| $type(null, __addressof__($method)) |]:
				assert type is not null
				return method.ToString()
		
	def methodTarget(code as Expression):
		match code:
			case [| $target() |]:
				return target.ToString()
		
	def variableName(code as Expression):
		match code:
			case [| $(ReferenceExpression(Name: l)) = $_ |]:
				return l
				
	def rvalue(code as Expression):
		match code:
			case [| $_ = $r |]:
				return r
	
	def firstPrintArgument(code as Node):
		match code:
			case [| print $arg |]:
				return arg 