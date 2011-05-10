using System.Reflection;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.MetaProgramming;
using NUnit.Framework;

namespace BooCompiler.Tests
{
	[TestFixture]
	public class AssemblyEmitterTest
	{
		[Test]
		[Ignore("work in progress")]
		public void VirtualMethodsAreTaggedNewSlot()
		{
			var classDefinition = new ClassDefinition
          	{
          		Name = "Foo",
				Members =
				{
					new Method
					{
						Name = "Bar",
						Modifiers = TypeMemberModifiers.Virtual
					}
				}
          	};

			var type = Compilation.compile(classDefinition);
			var method = type.GetMethod("Bar");
			Assert.AreEqual(
				MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.HideBySig,
				method.Attributes);
		}
	}
}
