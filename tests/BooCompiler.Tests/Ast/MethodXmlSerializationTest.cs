using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace BooCompiler.Tests.Ast
{
	[TestFixture]
	public class MethodXmlSerializationTest
	{
		[Test]
		public void MethodWithParamArrayCanBeSerialized()
		{
			var method = new Method("foo")
			{
			    Parameters = { new ParameterDeclaration { Name = "args" } }
			};
			AssertIsXmlSerializable(method);
		}

		[Test]
		[Ignore("Requires ast restructuring - not very high on the priority list")]
		public void HasParamArrayIsPreserved()
		{
			var methodWithParamArray = new Method("foo")
			{
				Parameters = { new ParameterDeclaration { Name = "args" } }
			};
			methodWithParamArray.Parameters.HasParamArray = true;

			var xmlClone = XmlRoundtripOf(methodWithParamArray);
			Assert.IsTrue(xmlClone.Parameters.HasParamArray);
		}

		private static void AssertIsXmlSerializable(Node node)
		{
			AstAssert.Matches(node, XmlRoundtripOf(node));
		}

		private static T XmlRoundtripOf<T>(T node) where T: Node
		{
			return (T)AstUtil.FromXml(node.GetType(), AstUtil.ToXml(node));
		}
	}
}
