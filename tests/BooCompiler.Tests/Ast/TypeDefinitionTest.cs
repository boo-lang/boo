using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace BooCompiler.Tests.Ast
{
	[TestFixture]
	public class TypeDefinitionTest
	{
		[Test]
		public void MergeIgnoresMatchingBaseTypes()
		{
			var foo = new SimpleTypeReference("Foo");
			var bar = new SimpleTypeReference("Bar");
			var fooOfBar = new GenericTypeReference("Foo", bar);
			var barOfFoo = new GenericTypeReference("Bar", foo);
			
			var subject = new ClassDefinition();
			subject.BaseTypes.Add(bar);
			subject.BaseTypes.Add(fooOfBar);

			var node = new ClassDefinition();
			node.BaseTypes.Add(foo);
			node.BaseTypes.Add(bar.CloneNode());
			node.BaseTypes.Add(fooOfBar.CloneNode());
			node.BaseTypes.Add(barOfFoo);

			subject.Merge(node);

			Assert.AreEqual(new[] { bar, fooOfBar, foo, barOfFoo }, subject.BaseTypes.ToArray());

		}
	}
}
