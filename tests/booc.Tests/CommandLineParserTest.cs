using System.IO;
using Boo.Lang.Compiler;
using NUnit.Framework;

namespace booc.Tests
{
	[TestFixture]
	public class CommandLineParserTest
	{
		[Test]
		public void LibCanBeWrappedInDoubleQuotes()
		{
			var libPath = Path.GetTempPath();
			var compilerParameters = new CompilerParameters();
			CommandLineParser.ParseInto(compilerParameters, string.Format(@"""-lib:{0}""", libPath));
			Assert.AreEqual(libPath, compilerParameters.LibPaths[0]);
		}

		[Test]
		public void LibValueCanBeWrappedInDoubleQuotes()
		{
			var libPath = Path.GetTempPath();
			var compilerParameters = new CompilerParameters();
			CommandLineParser.ParseInto(compilerParameters, string.Format(@"-lib:""{0}""", libPath));
			Assert.AreEqual(libPath, compilerParameters.LibPaths[0]);
		}

		[Test]
		public void WorkaroundForNAntBugCausingAdditionalDoubleQuoteSuffixOnLibValue()
		{
			var libPath = Path.GetTempPath();
			var compilerParameters = new CompilerParameters();
			CommandLineParser.ParseInto(compilerParameters, string.Format(@"-lib:{0}""", libPath));
			Assert.AreEqual(libPath, compilerParameters.LibPaths[0]);
		}

		[Test]
		public void SingleQuotesCanBeUsedAroundFileNames()
		{
			var fileName = "foo.boo";
			var compilerParameters = new CompilerParameters();
			CommandLineParser.ParseInto(compilerParameters, string.Format(@"'{0}'", fileName));
			Assert.AreEqual(fileName, compilerParameters.Input[0].Name);
		}
	}
}
