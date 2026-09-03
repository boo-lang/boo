namespace BooCompiler.Tests.TypeSystem.Services

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Services
import Boo.Lang.Environments
import BooCompiler.Tests
import NUnit.Framework

[TestFixture]
class CodeReifierTest:
	[Test]
	def ReifyIntoShouldFailWithAlreadyConnectedMember():
		module = Module()
		reifyIntoConnectedMember = def ():
			klass = ClassDefinition(Name: "Foo")
			module.Members.Add(klass)
			Exceptions.Expecting[of ArgumentException]({ CodeReifier().ReifyInto(module, klass) })

		RunCompilerStepAfterExpressionResolutionOn(CompileUnit(module), ActionStep(reifyIntoConnectedMember))

	private static def CodeReifier() as CodeReifier:
		return My[of CodeReifier].Instance

	[Test]
	def ReifyStatementShouldRefuseDisconnectedStatement():
		RunCompilerStepAfterExpressionResolution(ActionStep(
			{ Exceptions.Expecting[of ArgumentException]({ CodeReifier().Reify(ReturnStatement()) }) }))

	[Test]
	def ReifyExpressionShouldRefuseDisconnectedExpression():
		RunCompilerStepAfterExpressionResolution(ActionStep(
			{ Exceptions.Expecting[of ArgumentException]({ CodeReifier().Reify(NullLiteralExpression()) }) }))

	[Test]
	def ReifyClassAfterExpressionResolution():
		RunCompilerStepAfterExpressionResolution(ReifyClass())

	private def RunCompilerStepAfterExpressionResolution(step as ICompilerStep):
		RunCompilerStepAfterExpressionResolutionOn(CompileUnit(Module()), step)

	private def RunCompilerStepAfterExpressionResolutionOn(compileUnit as CompileUnit, step as ICompilerStep):
		pipeline = Boo.Lang.Compiler.Pipelines.ResolveExpressions()
		pipeline.Add(step)

		compiler = Boo.Lang.Compiler.BooCompiler(CompilerParameters(Pipeline: pipeline))
		result = compiler.Run(compileUnit)

		if result.Errors.Count > 0:
			Assert.Fail(result.Errors.ToString(true))

	class ReifyClass(AbstractCompilerStep):
		override def Run():
			klass = ClassDefinition(Name: "Foo")
			baseType = SimpleTypeReference("object")
			klass.BaseTypes.Add(baseType)

			method = Method(Name: "Bar")
			method.Body.Add(
				ReturnStatement(
					IntegerLiteralExpression(42)))
			klass.Members.Add(method)

			module = CompileUnit.Modules[0]
			Assert.AreEqual(0, module.Members.Count)

			My[of CodeReifier].Instance.ReifyInto(module, klass)

			Assert.AreEqual(1, module.Members.Count)
			Assert.AreSame(klass, module.Members[0])

			klassEntity = klass.Entity as IType
			Assert.IsTrue(klassEntity.IsClass)
			Assert.AreSame(TypeSystemServices.ObjectType, klassEntity.BaseType)

			methodEntity = method.Entity as IMethod
			Assert.AreEqual(method.Name, methodEntity.Name)
			Assert.AreSame(TypeSystemServices.IntType, methodEntity.ReturnType)
