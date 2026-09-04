namespace booc.Tests

import System
import System.IO
import Boo.Lang.Compiler
import Boo.Lang.Compiler.TypeSystem.Services
import Boo.Lang.Compiler.Util
import Boo.Lang.Environments
import NUnit.Framework

[TestFixture]
class CommandLineParserTest:
	[Test]
	def LibCanBeWrappedInDoubleQuotes():
		libPath = Path.GetTempPath()
		compilerParameters = CompilerParameters()
		booc.CommandLineParser.ParseInto(compilerParameters, string.Format('"-lib:{0}"', libPath))
		Assert.AreEqual(libPath, compilerParameters.LibPaths[0])

	[Test]
	def LibValueCanBeWrappedInDoubleQuotes():
		libPath = Path.GetTempPath()
		compilerParameters = CompilerParameters()
		booc.CommandLineParser.ParseInto(compilerParameters, string.Format('-lib:"{0}"', libPath))
		Assert.AreEqual(libPath, compilerParameters.LibPaths[0])

	[Test]
	def LibValueToleratesAnUnbalancedTrailingQuote():
		libPath = Path.GetTempPath()
		compilerParameters = CompilerParameters()
		booc.CommandLineParser.ParseInto(compilerParameters, string.Format('-lib:{0}"', libPath))
		Assert.AreEqual(libPath, compilerParameters.LibPaths[0])

	[Test]
	def SingleQuotesCanBeUsedAroundFileNames():
		fileName = "foo.boo"
		compilerParameters = CompilerParameters()
		booc.CommandLineParser.ParseInto(compilerParameters, string.Format("'{0}'", fileName))
		Assert.AreEqual(fileName, compilerParameters.Input[0].Name)

	[Test]
	def CustomTypeInferenceRuleAttribute():
		customAttributeName = typeof(CustomAttribute).FullName.Replace(char('+'), char('.'))

		parameters = CompilerParameters()
		parameters.References.Add(typeof(CustomAttribute).Assembly)

		booc.CommandLineParser.ParseInto(parameters, "-x-type-inference-rule-attribute:" + customAttributeName)
		ActiveEnvironment.With(CompilerContext(parameters).Environment):
			m = Methods.Of[of object](MethodWithCustomTypeInferenceRule)
			Assert.AreEqual("custom", My[of TypeInferenceRuleProvider].Instance.TypeInferenceRuleFor(m))

	private def ReferencesExtensions(args as (string)) as bool:
		# False, as booc does it: the parser loads the defaults itself.
		parameters = CompilerParameters(false)
		booc.CommandLineParser.ParseInto(parameters, *args)
		for reference in parameters.References:
			return true if reference.Name == "Boo.Lang.Extensions"
		return false

	[Test]
	def ExtensionsAreReferencedByDefault():
		Assert.IsTrue(ReferencesExtensions((of string: "foo.boo")))

	[Test]
	def NoExtensionsLeavesThemOut():
	"""Compiling Boo.Lang.Extensions needs this: the loaded copy collides."""
		Assert.IsFalse(ReferencesExtensions(("-noextensions", "foo.boo")))

	[CommandLineParserTest.CustomAttribute]
	public static def MethodWithCustomTypeInferenceRule() as object:
		return null

	public class CustomAttribute(Attribute):
		public override def ToString() as string:
			return "custom"
