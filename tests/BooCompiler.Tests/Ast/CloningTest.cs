using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace BooCompiler.Tests.Ast
{
	[TestFixture]
	public class CloningTest
	{
		[Test]
		public void CloningShouldPreserveIsSynthetic()
		{
			foreach (var flag in new[] { true, false })
			{
				var node = new LabelStatement {IsSynthetic = flag};
				Assert.AreEqual(flag, node.CloneNode().IsSynthetic);
			}
		}
	}
}
