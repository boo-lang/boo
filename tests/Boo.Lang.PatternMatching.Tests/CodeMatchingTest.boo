namespace Boo.Lang.PatternMatching.Tests

import NUnit.Framework
import Boo.Lang.PatternMatching
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

[TestFixture]
class CodeMatchingTest:
	
	[Test]
	def ListPatterns():
		Assert.AreEqual("42", firstElementOfNestedTwoElementList([| [[42, "foo"]] |]).ToString())
		
	[Test]
	def MismatchedListSize():
		expectingMatchError:
			firstElementOfNestedTwoElementList([| [] |])
		expectingMatchError:
			firstElementOfNestedTwoElementList([| [[]] |])
		expectingMatchError:
			firstElementOfNestedTwoElementList([| [[1]] |])
		expectingMatchError:
			firstElementOfNestedTwoElementList([| [[1, 2], []] |])
		
	def firstElementOfNestedTwoElementList(code):
		match code:
			case [| [[$first, $_]] |]:
				return first
	
	[Test]
	def MacroApplication():
		Assert.AreEqual("42", firstPrintArgument([| print 42 |]).ToString())
		
	[Test]
	def MacroApplicationWithMismatchedArguments():
		expectingMatchError:
			firstPrintArgument([| print "arg1", "arg2" |])
		
	[Test]
	def WrongMacroApplicationNae():
		expectingMatchError:
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
	def Cast():
		code = [| a cast int |]
		match code:
			case [| $name cast $type |]:
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
	def InvocationPatternWithArgumentsMismatch():
		expectingMatchError:
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
	def TypeExtractionFromGenericReference():
		code = [| array of int |]
		match code:
			case [| array of $type |]:
				assert type.Matches(SimpleTypeReference("int"))
				
	[Test]
	def TypeAndArgumentExtractionFromGenericInvocation():
		code = [| array of int(42) |]
		match code:
			case [| array of $type($count) |]:
				assert type.Matches(SimpleTypeReference("int"))
				assert count.Matches([| 42 |])
				
	[Test]
	def TypeExtractionFromGenericTypeReference():
		code = [| typeof(List of int) |]
		match code:
			case [| typeof(List of $type) |]:
				assert type.Matches(SimpleTypeReference("int"))
				
	[Test]
	def TypeExtractionFromDeepGenericTypeReference():
		code = [| typeof(List of List of int) |]
		match code:
			case [| typeof(List of List of $type) |]:
				assert type.Matches(SimpleTypeReference("int"))
		
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
	def StringLiteralExpressionMismatch():
		expectingMatchError:
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
	def IntegerLiteralExpressionMismatch():
		expectingMatchError:
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
	def SimpleTypeReferenceSuccess():
		code = [| typeof(Foo) |]
		match code:
			case [| typeof(Foo) |]:
				pass
				
	[Test]
	def SimpleTypeReferenceFailure():
		code = [| typeof(Foo) |]
		match code:
			case [| typeof(Bar) |]:
				Assert.Fail("wrong match")
			otherwise:
				pass
				
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
				
def expectingMatchError(code as callable()):
	try:
		code()
		Assert.Fail()
	except as MatchError:
		pass