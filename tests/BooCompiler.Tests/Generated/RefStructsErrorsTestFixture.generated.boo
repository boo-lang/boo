namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class RefStructsErrorsTestFixture(AbstractCompilerErrorsTestFixture):

	[Test]
	def @array_builtin():
		RunCompilerTestCase("array-builtin.boo")

	[Test]
	def @array_local():
		RunCompilerTestCase("array-local.boo")

	[Test]
	def @array_return():
		RunCompilerTestCase("array-return.boo")

	[Test]
	def @array_span():
		RunCompilerTestCase("array-span.boo")

	[Test]
	def @async_span():
		RunCompilerTestCase("async-span.boo")

	[Test]
	def @box_argument():
		RunCompilerTestCase("box-argument.boo")

	[Test]
	def @box_array_element():
		RunCompilerTestCase("box-array-element.boo")

	[Test]
	def @box_cast():
		RunCompilerTestCase("box-cast.boo")

	[Test]
	def @box_concat():
		RunCompilerTestCase("box-concat.boo")

	[Test]
	def @box_duck():
		RunCompilerTestCase("box-duck.boo")

	[Test]
	def @box_inherited_member():
		RunCompilerTestCase("box-inherited-member.boo")

	[Test]
	def @box_list():
		RunCompilerTestCase("box-list.boo")

	[Test]
	def @box_return():
		RunCompilerTestCase("box-return.boo")

	[Test]
	def @box_span():
		RunCompilerTestCase("box-span.boo")

	[Test]
	def @box_trycast():
		RunCompilerTestCase("box-trycast.boo")

	[Test]
	def @box_varargs():
		RunCompilerTestCase("box-varargs.boo")

	[Test]
	def @closure_span():
		RunCompilerTestCase("closure-span.boo")

	[Test]
	def @declared_box():
		RunCompilerTestCase("declared-box.boo")

	[Test]
	def @declared_field():
		RunCompilerTestCase("declared-field.boo")

	[Test]
	def @field_span():
		RunCompilerTestCase("field-span.boo")

	[Test]
	def @generator_span():
		RunCompilerTestCase("generator-span.boo")

	[Test]
	def @generic_declared_argument():
		RunCompilerTestCase("generic-declared-argument.boo")

	[Test]
	def @generic_declared_box():
		RunCompilerTestCase("generic-declared-box.boo")

	[Test]
	def @generic_span():
		RunCompilerTestCase("generic-span.boo")

	[Test]
	def @readonlyspan_refarg():
		RunCompilerTestCase("readonlyspan-refarg.boo")

	[Test]
	def @readonlyspan_refreturn_write():
		RunCompilerTestCase("readonlyspan-refreturn-write.boo")

	[Test]
	def @readonlyspan_write():
		RunCompilerTestCase("readonlyspan-write.boo")

	[Test]
	def @static_byreflike_field():
		RunCompilerTestCase("static-byreflike-field.boo")

	[Test]
	def @typetest_span():
		RunCompilerTestCase("typetest-span.boo")

	[Test]
	def @using_byreflike():
		RunCompilerTestCase("using-byreflike.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "byreflike/errors"
