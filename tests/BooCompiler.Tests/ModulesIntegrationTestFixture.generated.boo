namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class ModulesIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @dotted_module_name():
		RunCompilerTestCase("dotted.module.name.boo")

	[Test]
	def @import_1():
		RunCompilerTestCase("import-1.boo")

	[Test]
	def @import_2():
		RunCompilerTestCase("import-2.boo")

	[Test]
	def @import_3():
		RunCompilerTestCase("import-3.boo")

	[Test]
	def @import_4():
		RunCompilerTestCase("import-4.boo")

	[Test]
	def @import_5():
		RunCompilerTestCase("import-5.boo")

	[Test]
	def @import_6():
		RunCompilerTestCase("import-6.boo")

	[Test]
	def @import_7():
		RunCompilerTestCase("import-7.boo")

	[Test]
	def @import_8():
		RunCompilerTestCase("import-8.boo")

	[Test]
	def @import_9():
		RunCompilerTestCase("import-9.boo")

	[Test]
	def @modules_1():
		RunCompilerTestCase("modules-1.boo")

	[Test]
	def @modules_2():
		RunCompilerTestCase("modules-2.boo")

	[Test]
	def @modules_3():
		RunCompilerTestCase("modules-3.boo")

	[Test]
	def @modules_4():
		RunCompilerTestCase("modules-4.boo")

	[Test]
	def @modules_5():
		RunCompilerTestCase("modules-5.boo")

	[Test]
	def @modules_6():
		RunCompilerTestCase("modules-6.boo")

	[Test]
	def @peek_a():
		RunCompilerTestCase("peek.a.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/modules"
