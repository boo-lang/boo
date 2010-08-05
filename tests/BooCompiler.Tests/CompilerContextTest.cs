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
			new CompilerContext(compileUnit).Environment.Run(() => Assert.AreSame(compileUnit, My<CompileUnit>.Instance));
		}
	}

}
