using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace BooCompiler.Tests.Ast
{
	public static class AstAssert
	{
		public static void Matches(Node expected, Node actual)
		{
			if (expected.Matches(actual))
				return;

			Assert.Fail(string.Format("{0}('{1}') !~ {2}('{3}')", expected.GetType(), expected.ToCodeString(), actual.GetType(), actual.ToCodeString()));
		}
	}
}