namespace BooCompiler.Tests.TypeSystem.Services

import System
import Boo.Lang
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Services
import Boo.Lang.Compiler.Util
import Boo.Lang.Environments
import BooCompiler.Tests.TypeSystem
import NUnit.Framework

[TestFixture]
class InvocationTypeInferenceRulesTest(AbstractTypeSystemTest):
	[Test]
	def TypeReferencedByFirstArgumentRule():
		RunInCompilerContextEnvironment() do:
			type = TypeSystemServices.Map(typeof(string))
			method = TypeSystemServices.Map(Methods.Of[of Type, object](Create))
			invocation = CodeBuilder.CreateMethodInvocation(method, TypeReference(type))

			Assert.AreSame(type, Subject.ApplyTo(invocation, method))

	[Test]
	def TypeReferencedBySecondArgumentRule():
		RunInCompilerContextEnvironment() do:
			type = TypeSystemServices.Map(typeof(string))
			method = TypeSystemServices.Map(Methods.Of[of string, Type, object](Load))
			invocation = CodeBuilder.CreateMethodInvocation(method, StringLiteral(), TypeReference(type))

			Assert.AreSame(type, Subject.ApplyTo(invocation, method))

	[Test]
	def TypeOfFirstArgumentRule():
		RunInCompilerContextEnvironment() do:
			type = TypeSystemServices.Map(typeof(string))
			method = TypeSystemServices.Map(Methods.Of[of object, object](Instantiate))
			invocation = CodeBuilder.CreateMethodInvocation(method, StringLiteral())

			Assert.AreSame(type, Subject.ApplyTo(invocation, method))

	[Test]
	def MisspelledRuleNameCausesWarning():
		RunInCompilerContextEnvironment() do:
			method = TypeSystemServices.Map(Methods.Of[of object](MethodWithMisspelledRule))
			invocation = CodeBuilder.CreateMethodInvocation(method)

			Assert.IsNull(Subject.ApplyTo(invocation, method))

			warnings = My[of CompilerWarningCollection].Instance
			Assert.AreEqual(1, warnings.Count)

			message = warnings[0].Message
			Assert.IsTrue(message.Contains("UnknownRule"))
			Assert.IsTrue(message.Contains("MethodWithMisspelledRule"))

	private def TypeReference(type as IType) as ReferenceExpression:
		return CodeBuilder.CreateReference(type)

	private def StringLiteral() as StringLiteralExpression:
		return CodeBuilder.CreateStringLiteral("foo")

	private static Subject as InvocationTypeInferenceRules:
		get: return My[of InvocationTypeInferenceRules].Instance

	[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
	private static def Instantiate(prototype as object) as object:
		return null

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	private static def Create(type as Type) as object:
		return null

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
	private static def Load(path as string, type as Type) as object:
		return null

	[TypeInferenceRule("UnknownRule")]
	private static def MethodWithMisspelledRule() as object:
		return null
