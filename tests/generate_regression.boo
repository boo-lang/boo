import System
import System.IO

def GetTestCaseName(fname as string):
	return Path.GetFileNameWithoutExtension(fname).Replace("-", "_")

using writer=StreamWriter("build/RegressionTestFixture.cs"):
	writer.Write("""
namespace BooCompiler.Tests
{
	using NUnit.Framework;
	using System.IO;
	using Boo.Lang.Compiler;
	
	[TestFixture]		
	public class RegressionTestFixture : AbstractCompilerTestCase
	{
		override protected CompilerPipeline SetUpCompilerPipeline()
		{
			return new Boo.Lang.Compiler.Pipelines.Run();
		}		
""")

	for fname in Directory.GetFiles("testcases/regression", "*.boo"):
		print(fname)
		writer.Write("""
		[Test]
		public void ${GetTestCaseName(fname)}()
		{
			RunCompilerTestCase("${Path.GetFullPath(fname)}");
		}
		""")

	writer.Write("""
	}
}
""")
