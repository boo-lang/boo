using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Environments;
using NUnit.Framework;

namespace BooCompiler.Tests
{
	[TestFixture]
	public class CompilerContextTest
	{
		[Test]
		public void CompileUnitIsProvidedToTheEnvironment()
		{
			var compileUnit = new CompileUnit();
			ActiveEnvironment.With(
				new CompilerContext(compileUnit).Environment,
				() => Assert.AreSame(compileUnit, My<CompileUnit>.Instance));
		}
	}

}
