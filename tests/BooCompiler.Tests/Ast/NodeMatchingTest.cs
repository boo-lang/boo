using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace BooCompiler.Tests.Ast
{
	[TestFixture]
	public class NodeMatchingTest
	{
		[Test]
		public void SimpleTypeReferenceWontMatchGenericTypeReferenceWithSamePrefix()
		{
			Assert.IsFalse(new SimpleTypeReference("Foo").Matches(new GenericTypeReference("Foo")));
		}

		[Test]
		public void GenericTypeReferenceMatchesGenericTypeReferenceWithSamePrefix()
		{
			Assert.IsTrue(new GenericTypeReference("Foo").Matches(new GenericTypeReference("Foo")));
		}
	}
}
