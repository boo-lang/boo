using System.Collections.Generic;
using System.IO;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using NUnit.Framework;

namespace BooCompiler.Tests
{
	[TestFixture]
	public class ExtensionsCompilationTest
	{
		[Test]
		public void MacroMacroCompilation()
		{
			CompilerParameters parameters = new CompilerParameters(false);
			parameters.References.Add(typeof(IEnumerable<>).Assembly);
			
			parameters.Input.Add(BooLangExtensionsSource("Macros/MacroMacro.boo"));
			parameters.Input.Add(BooLangExtensionsSource("Macros/AssertMacro.boo"));

			parameters.Pipeline = new Boo.Lang.Compiler.Pipelines.ResolveExpressions();

			Boo.Lang.Compiler.BooCompiler compiler = new Boo.Lang.Compiler.BooCompiler(parameters);
			CompilerContext results = compiler.Run();
			Assert.AreEqual(0, results.Errors.Count, results.Errors.ToString());
		}

		private FileInput BooLangExtensionsSource(string file)
		{
			return new FileInput(Path.Combine(BooTestCaseUtil.BasePath, "src/Boo.Lang.Extensions/" + file));
		}
	}
}
