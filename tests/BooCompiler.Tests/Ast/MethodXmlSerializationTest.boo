namespace BooCompiler.Tests.Ast

import Boo.Lang.Compiler.Ast
import NUnit.Framework

[TestFixture]
class MethodXmlSerializationTest:
	[Test]
	def MethodWithParamArrayCanBeSerialized():
		method = Method("foo")
		method.Parameters.Add(ParameterDeclaration(Name: "args"))
		AssertIsXmlSerializable(method)

	[Test]
	[Ignore("Requires ast restructuring - not very high on the priority list")]
	def HasParamArrayIsPreserved():
		methodWithParamArray = Method("foo")
		methodWithParamArray.Parameters.Add(ParameterDeclaration(Name: "args"))
		methodWithParamArray.Parameters.HasParamArray = true

		xmlClone = XmlRoundtripOf(methodWithParamArray)
		Assert.IsTrue(xmlClone.Parameters.HasParamArray)

	private static def AssertIsXmlSerializable(node as Node):
		AstAssert.Matches(node, XmlRoundtripOf(node))

	private static def XmlRoundtripOf[of T(Node)](node as T) as T:
		return AstUtil.FromXml(node.GetType(), AstUtil.ToXml(node))
