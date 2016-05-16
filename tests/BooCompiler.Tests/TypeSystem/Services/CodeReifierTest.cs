using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Services
{
	[TestFixture]
	public class CodeReifierTest
	{
		[Test]
		public void ReifyIntoShouldFailWithAlreadyConnectedMember()
		{
			var module = new Module();
			RunCompilerStepAfterExpressionResolutionOn(new CompileUnit(module), new ActionStep(() =>
			{
				var klass = new ClassDefinition { Name = "Foo" };
				module.Members.Add(klass);
				Exceptions.Expecting<ArgumentException>(() => CodeReifier().ReifyInto(module, klass));
			}));
		}

		private static CodeReifier CodeReifier()
		{
			return My<CodeReifier>.Instance;
		}

		[Test]
		public void ReifyStatementShouldRefuseDisconnectedStatement()
		{
			RunCompilerStepAfterExpressionResolution(new ActionStep(() =>
			{
				Exceptions.Expecting<CompilerError>(() => CodeReifier().Reify(new ReturnStatement()));
			}));
		}

		[Test]
		public void ReifyExpressionShouldRefuseDisconnectedExpression()
		{
			RunCompilerStepAfterExpressionResolution(new ActionStep(() =>
			{
				Exceptions.Expecting<CompilerError>(() => CodeReifier().Reify(new NullLiteralExpression()));
			}));
		}

		[Test]
		public void ReifyClassAfterExpressionResolution()
		{
			RunCompilerStepAfterExpressionResolution(new ReifyClass());
		}

		private void RunCompilerStepAfterExpressionResolution(ICompilerStep step)
		{
			RunCompilerStepAfterExpressionResolutionOn(new CompileUnit(new Module()), step);
		}

		private void RunCompilerStepAfterExpressionResolutionOn(CompileUnit compileUnit, ICompilerStep step)
		{
			var pipeline = new Boo.Lang.Compiler.Pipelines.ResolveExpressions { step };

			var compiler = new Boo.Lang.Compiler.BooCompiler(new CompilerParameters { Pipeline = pipeline });
			var result = compiler.Run(compileUnit);

			if (result.Errors.Count > 0)
				Assert.Fail(result.Errors.ToString(true));
		}

		class ReifyClass : AbstractCompilerStep
		{
			public override void Run()
			{
				var klass = new ClassDefinition { Name = "Foo" };
				var baseType = new SimpleTypeReference("object");
				klass.BaseTypes.Add(baseType);

				var method = new Method { Name = "Bar" };
				method.Body.Add(
					new ReturnStatement(
						new IntegerLiteralExpression(42)));
				klass.Members.Add(method);

				var module = CompileUnit.Modules[0];
				Assert.AreEqual(0, module.Members.Count);

				My<CodeReifier>.Instance.ReifyInto(module, klass);

				Assert.AreEqual(1, module.Members.Count);
				Assert.AreSame(klass, module.Members[0]);

				var klassEntity = (IType) klass.Entity;
				Assert.IsTrue(klassEntity.IsClass);
				Assert.AreSame(TypeSystemServices.ObjectType, klassEntity.BaseType);

				var methodEntity = (IMethod)method.Entity;
				Assert.AreEqual(method.Name, methodEntity.Name);
				Assert.AreSame(TypeSystemServices.IntType, methodEntity.ReturnType);
			}
		}
	}
}
