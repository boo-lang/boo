namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class RefStructsTestFixture(AbstractCompilerTestCase):

	[Test]
	def @declare_byreflike():
		RunCompilerTestCase("declare-byreflike.boo")

	[Test]
	def @declared_interface():
		RunCompilerTestCase("declared-interface.boo")

	[Test]
	def @declared_override():
		RunCompilerTestCase("declared-override.boo")

	[Test]
	def @nested_byreflike():
		RunCompilerTestCase("nested-byreflike.boo")

	[Test]
	def @span_compound():
		RunCompilerTestCase("span-compound.boo")

	[Test]
	def @span_contexts():
		RunCompilerTestCase("span-contexts.boo")

	[Test]
	def @span_foreach_contexts():
		RunCompilerTestCase("span-foreach-contexts.boo")

	[Test]
	def @span_hello_world():
		RunCompilerTestCase("span-hello-world.boo")

	[Test]
	def @span_hello_world2():
		RunCompilerTestCase("span-hello-world2.boo")

	[Test]
	def @span_operators():
		RunCompilerTestCase("span-operators.boo")

	[Test]
	def @span_refarg():
		RunCompilerTestCase("span-refarg.boo")

	[Test]
	def @span_refproperty_write():
		RunCompilerTestCase("span-refproperty-write.boo")

	[Test]
	def @span_refproperty():
		RunCompilerTestCase("span-refproperty.boo")

	[Test]
	def @span_refreturn_write():
		RunCompilerTestCase("span-refreturn-write.boo")

	[Test]
	def @span_refreturn():
		RunCompilerTestCase("span-refreturn.boo")

	[Test]
	def @span_return_and_refparam():
		RunCompilerTestCase("span-return-and-refparam.boo")

	[Test]
	def @span_write_param():
		RunCompilerTestCase("span-write-param.boo")

	[Test]
	def @span_write():
		RunCompilerTestCase("span-write.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "byreflike"
