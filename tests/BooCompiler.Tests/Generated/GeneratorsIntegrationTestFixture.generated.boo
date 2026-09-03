namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class GeneratorsIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @generator_calling_external_super_with_arguments_2():
		RunCompilerTestCase("generator-calling-external-super-with-arguments-2.boo")

	[Test]
	def @generator_calling_external_super_with_arguments():
		RunCompilerTestCase("generator-calling-external-super-with-arguments.boo")

	[Test]
	def @generator_calling_super_with_arguments_2():
		RunCompilerTestCase("generator-calling-super-with-arguments-2.boo")

	[Test]
	def @generator_calling_super_with_arguments():
		RunCompilerTestCase("generator-calling-super-with-arguments.boo")

	[Test]
	def @generator_calling_super():
		RunCompilerTestCase("generator-calling-super.boo")

	[Test]
	def @generator_of_static_class_is_transient():
		RunCompilerTestCase("generator-of-static-class-is-transient.boo")

	[Test]
	def @generator_of_transient_class_is_transient():
		RunCompilerTestCase("generator-of-transient-class-is-transient.boo")

	[Test]
	def @generators_1():
		RunCompilerTestCase("generators-1.boo")

	[Test]
	def @generators_10():
		RunCompilerTestCase("generators-10.boo")

	[Test]
	def @generators_11():
		RunCompilerTestCase("generators-11.boo")

	[Test]
	def @generators_12():
		RunCompilerTestCase("generators-12.boo")

	[Test]
	def @generators_13():
		RunCompilerTestCase("generators-13.boo")

	[Test]
	def @generators_14():
		RunCompilerTestCase("generators-14.boo")

	[Test]
	def @generators_15():
		RunCompilerTestCase("generators-15.boo")

	[Test]
	def @generators_16():
		RunCompilerTestCase("generators-16.boo")

	[Test]
	def @generators_17():
		RunCompilerTestCase("generators-17.boo")

	[Test]
	def @generators_18():
		RunCompilerTestCase("generators-18.boo")

	[Test]
	def @generators_19():
		RunCompilerTestCase("generators-19.boo")

	[Test]
	def @generators_2():
		RunCompilerTestCase("generators-2.boo")

	[Test]
	def @generators_20():
		RunCompilerTestCase("generators-20.boo")

	[Test]
	def @generators_21():
		RunCompilerTestCase("generators-21.boo")

	[Test]
	def @generators_3():
		RunCompilerTestCase("generators-3.boo")

	[Test]
	def @generators_4():
		RunCompilerTestCase("generators-4.boo")

	[Test]
	def @generators_5():
		RunCompilerTestCase("generators-5.boo")

	[Test]
	def @generators_6():
		RunCompilerTestCase("generators-6.boo")

	[Test]
	def @generators_7():
		RunCompilerTestCase("generators-7.boo")

	[Test]
	def @generators_8():
		RunCompilerTestCase("generators-8.boo")

	[Test]
	def @generators_9():
		RunCompilerTestCase("generators-9.boo")

	[Test]
	def @generic_generator_1():
		RunCompilerTestCase("generic-generator-1.boo")

	[Test]
	def @generic_generator_2():
		RunCompilerTestCase("generic-generator-2.boo")

	[Test]
	def @label_issue_1():
		RunCompilerTestCase("label-issue-1.boo")

	[Test]
	def @list_generators_1():
		RunCompilerTestCase("list-generators-1.boo")

	[Test]
	def @list_generators_2():
		RunCompilerTestCase("list-generators-2.boo")

	[Test]
	def @list_generators_3():
		RunCompilerTestCase("list-generators-3.boo")

	[Test]
	def @list_generators_4():
		RunCompilerTestCase("list-generators-4.boo")

	[Test]
	def @list_generators_5():
		RunCompilerTestCase("list-generators-5.boo")

	[Test]
	def @to_string():
		RunCompilerTestCase("to-string.boo")

	[Test]
	def @yield_1():
		RunCompilerTestCase("yield-1.boo")

	[Test]
	def @yield_10():
		RunCompilerTestCase("yield-10.boo")

	[Test]
	def @yield_11():
		RunCompilerTestCase("yield-11.boo")

	[Test]
	def @yield_12():
		RunCompilerTestCase("yield-12.boo")

	[Test]
	def @yield_13():
		RunCompilerTestCase("yield-13.boo")

	[Test]
	def @yield_14():
		RunCompilerTestCase("yield-14.boo")

	[Test]
	def @yield_15():
		RunCompilerTestCase("yield-15.boo")

	[Test]
	def @yield_16():
		RunCompilerTestCase("yield-16.boo")

	[Test]
	def @yield_17():
		RunCompilerTestCase("yield-17.boo")

	[Test]
	def @yield_2():
		RunCompilerTestCase("yield-2.boo")

	[Test]
	def @yield_3():
		RunCompilerTestCase("yield-3.boo")

	[Test]
	def @yield_4():
		RunCompilerTestCase("yield-4.boo")

	[Test]
	def @yield_5():
		RunCompilerTestCase("yield-5.boo")

	[Test]
	def @yield_6():
		RunCompilerTestCase("yield-6.boo")

	[Test]
	def @yield_7():
		RunCompilerTestCase("yield-7.boo")

	[Test]
	def @yield_8():
		RunCompilerTestCase("yield-8.boo")

	[Test]
	def @yield_9():
		RunCompilerTestCase("yield-9.boo")

	[Test]
	def @yield_null_as_IEnumerator():
		RunCompilerTestCase("yield-null-as-IEnumerator.boo")

	[Test]
	def @yield_null():
		RunCompilerTestCase("yield-null.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/generators"
