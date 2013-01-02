using System.Linq;
using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace BooCompiler.Tests.Ast
{
	[TestFixture]
	public class AncestorsTest
	{
		[Test]
		public void NoAncestorsForUnparentedNode()
		{
			Assert.AreEqual(0, new SimpleTypeReference().GetAncestors<Node>().Count());
		}
	}
}
