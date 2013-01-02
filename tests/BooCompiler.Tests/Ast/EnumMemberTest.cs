using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace BooCompiler.Tests.Ast
{
	[TestFixture]
	public class EnumMemberTest
	{
		[Test]
		public void EnumMemberIsStatic()
		{
			Assert.IsTrue(new EnumMember().IsStatic);
		}
	}
}
