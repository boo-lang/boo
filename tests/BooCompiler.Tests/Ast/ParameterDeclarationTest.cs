using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace BooCompiler.Tests.Ast
{
	[TestFixture]
	public class ParameterDeclarationTest
	{
		[Test]
		public void WhenEmptyToStringReturnsEmpty()
		{
			Assert.AreEqual(string.Empty, new ParameterDeclaration().ToString());
		}
	}
}
