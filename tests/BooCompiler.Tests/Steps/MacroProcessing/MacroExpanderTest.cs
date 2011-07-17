using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps.MacroProcessing;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Environments;
using Moq;
using NUnit.Framework;

namespace BooCompiler.Tests.Steps.MacroProcessing
{
	[TestFixture]
	public class MacroExpanderTest
	{
		[Test]
		public void MacroCompilerIsTakenFromTheEnvironment()
		{
			var compiler = new Mock<MacroCompiler>(MockBehavior.Strict);

			ActiveEnvironment.With(CompilerContextEnvironmentWith(compiler), ()=>
			{
				var module = CreateModule();
				var macroApplication = new MacroStatement(new LexicalInfo("file.boo", 1, 1), "foo");
				module.Globals.Add(macroApplication);

				var macroDefinition = CreateClassOn(module, "FooMacro");

				compiler.Setup(o => o.AlreadyCompiled(macroDefinition)).Returns(false);
				compiler.Setup(o => o.Compile(macroDefinition)).Returns((Type)null);

				var expander = My<MacroExpander>.Instance;
				Assert.IsFalse(expander.ExpandAll());

				var errors = CompilerErrors();
				Assert.AreEqual(1, errors.Count);
				Assert.AreEqual(CompilerErrorFactory.AstMacroMustBeExternal(macroApplication, (IType) macroDefinition.Entity).ToString(), errors[0].ToString());
			});

			compiler.VerifyAll();
		}

		private CompilerErrorCollection CompilerErrors()
		{
			return My<CompilerErrorCollection>.Instance;
		}

		private ClassDefinition CreateClassOn(Module module, string className)
		{
			var classDefinition = CodeBuilder().CreateClass(className).ClassDefinition;
			module.Members.Add(classDefinition);
			return classDefinition;
		}

		private Module CreateModule()
		{
			var module = CodeBuilder().CreateModule("test", null);
			CompileUnit().Modules.Add(module);
			return module;
		}

		private CompileUnit CompileUnit()
		{
			return My<CompileUnit>.Instance;
		}

		private BooCodeBuilder CodeBuilder()
		{
			return My<BooCodeBuilder>.Instance;
		}

		private IEnvironment CompilerContextEnvironmentWith(Mock<MacroCompiler> compiler)
		{
			var parameters = new CompilerParameters(false) { Environment = new ClosedEnvironment(compiler.Object) };
			return new CompilerContext(parameters).Environment;
		}
	}
}
