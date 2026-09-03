namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class ExtensionsIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @extension_properties_1():
		RunCompilerTestCase("extension-properties-1.boo")

	[Test]
	def @extensions_1():
		RunCompilerTestCase("extensions-1.boo")

	[Test]
	def @extensions_2():
		RunCompilerTestCase("extensions-2.boo")

	[Test]
	def @extensions_3():
		RunCompilerTestCase("extensions-3.boo")

	[Test]
	def @extensions_4():
		RunCompilerTestCase("extensions-4.boo")

	[Test]
	def @extensions_5():
		RunCompilerTestCase("extensions-5.boo")

	[Test]
	def @extensions_6():
		RunCompilerTestCase("extensions-6.boo")

	[Test]
	def @extensions_7():
		RunCompilerTestCase("extensions-7.boo")

	[Test]
	def @extensions_8():
		RunCompilerTestCase("extensions-8.boo")

	[Test]
	def @extensions_9():
		RunCompilerTestCase("extensions-9.boo")

	[Test]
	def @extensions_for_self_1():
		RunCompilerTestCase("extensions-for-self-1.boo")

	[Test]
	def @extensions_for_self_2():
		RunCompilerTestCase("extensions-for-self-2.boo")

	[Test]
	def @generic_extension_1():
		RunCompilerTestCase("generic-extension-1.boo")

	[Test]
	def @generic_extension_2():
		RunCompilerTestCase("generic-extension-2.boo")

	[Test]
	def @generic_extension_3():
		RunCompilerTestCase("generic-extension-3.boo")

	[Test]
	def @generic_extension_4():
		RunCompilerTestCase("generic-extension-4.boo")

	[Test]
	def @generic_extension_5():
		RunCompilerTestCase("generic-extension-5.boo")

	[Test]
	def @generic_extension_overloads_in_generic_invocations():
		RunCompilerTestCase("generic-extension-overloads-in-generic-invocations.boo")

	[Test]
	def @implicit_conversion_extension_1():
		RunCompilerTestCase("implicit-conversion-extension-1.boo")

	[Test]
	def @implicit_conversion_extension_2():
		RunCompilerTestCase("implicit-conversion-extension-2.boo")

	[Test]
	def @linq_operator():
		RunCompilerTestCase("linq-operator.boo")

	[Test]
	def @per_module_extensions():
		RunCompilerTestCase("per-module-extensions.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/extensions"
