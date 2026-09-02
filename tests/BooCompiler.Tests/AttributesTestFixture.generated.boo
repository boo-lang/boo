namespace BooCompiler.Tests

import NUnit.Framework
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Steps

[TestFixture]
class AttributesTestFixture(AbstractCompilerTestCase):
	override protected def SetUpCompilerPipeline() as CompilerPipeline:
		pipeline as CompilerPipeline = Boo.Lang.Compiler.Pipelines.ExpandMacros()
		pipeline.Add(PrintBoo())
		return pipeline

	[Test]
	def @allparametersrequired():
		RunCompilerTestCase("allparametersrequired.boo")

	[Test]
	def @default_1():
		RunCompilerTestCase("default-1.boo")

	[Test]
	def @default_2():
		RunCompilerTestCase("default-2.boo")

	[Test]
	def @getter_1():
		RunCompilerTestCase("getter-1.boo")

	[Test]
	def @property_1():
		RunCompilerTestCase("property-1.boo")

	[Test]
	def @property_2():
		RunCompilerTestCase("property-2.boo")

	[Test]
	def @property_3():
		RunCompilerTestCase("property-3.boo")

	[Test]
	def @property_4():
		RunCompilerTestCase("property-4.boo")

	[Test]
	def @property_5():
		RunCompilerTestCase("property-5.boo")

	[Test]
	def @property_6():
		RunCompilerTestCase("property-6.boo")

	[Test]
	def @required_1():
		RunCompilerTestCase("required-1.boo")

	[Test]
	def @required_2():
		RunCompilerTestCase("required-2.boo")

	[Test]
	def @required_3():
		RunCompilerTestCase("required-3.boo")

	[Test]
	def @required_4():
		RunCompilerTestCase("required-4.boo")

	[Test]
	def @required_5():
		RunCompilerTestCase("required-5.boo")

	[Test]
	def @viewstate():
		RunCompilerTestCase("viewstate.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "attributes"
