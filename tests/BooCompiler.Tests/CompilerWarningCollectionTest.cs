using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace BooCompiler.Tests
{
	[TestFixture]
	public class CompilerWarningCollectionTest
	{
		[Test]
		public void TestSuppressWarning()
		{
			CompilerWarningCollection warnings = new CompilerWarningCollection();
			warnings.Adding +=
				delegate(object sender, CompilerWarningEventArgs args) { if (args.Warning.Code == "foo") args.Cancel(); };
			warnings.Add(new CompilerWarning(LexicalInfo.Empty, "foo", "foo"));
			Assert.AreEqual(0, warnings.Count);
			warnings.Add(new CompilerWarning(LexicalInfo.Empty, "bar", "bar"));
			Assert.AreEqual(1, warnings.Count);
		}
	}
}
