namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class ClosuresIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @closure_inference_1():
		RunCompilerTestCase("closure-inference-1.boo")

	[Test]
	def @closure_inference_2():
		RunCompilerTestCase("closure-inference-2.boo")

	[Test]
	def @closure_inference_3():
		RunCompilerTestCase("closure-inference-3.boo")

	[Test]
	def @closure_inference_4():
		RunCompilerTestCase("closure-inference-4.boo")

	[Test]
	def @closure_inference_5():
		RunCompilerTestCase("closure-inference-5.boo")

	[Test]
	def @closure_inference_6():
		RunCompilerTestCase("closure-inference-6.boo")

	[Test]
	def @closure_inference_7():
		RunCompilerTestCase("closure-inference-7.boo")

	[Test]
	def @closures_1():
		RunCompilerTestCase("closures-1.boo")

	[Test]
	def @closures_10():
		RunCompilerTestCase("closures-10.boo")

	[Test]
	def @closures_11():
		RunCompilerTestCase("closures-11.boo")

	[Test]
	def @closures_12():
		RunCompilerTestCase("closures-12.boo")

	[Test]
	def @closures_13():
		RunCompilerTestCase("closures-13.boo")

	[Test]
	def @closures_14():
		RunCompilerTestCase("closures-14.boo")

	[Test]
	def @closures_15():
		RunCompilerTestCase("closures-15.boo")

	[Test]
	def @closures_16():
		RunCompilerTestCase("closures-16.boo")

	[Test]
	def @closures_17():
		RunCompilerTestCase("closures-17.boo")

	[Test]
	def @closures_18():
		RunCompilerTestCase("closures-18.boo")

	[Test]
	def @closures_19():
		RunCompilerTestCase("closures-19.boo")

	[Test]
	def @closures_2():
		RunCompilerTestCase("closures-2.boo")

	[Test]
	def @closures_20():
		RunCompilerTestCase("closures-20.boo")

	[Test]
	def @closures_21():
		RunCompilerTestCase("closures-21.boo")

	[Test]
	def @closures_22():
		RunCompilerTestCase("closures-22.boo")

	[Test]
	def @closures_23():
		RunCompilerTestCase("closures-23.boo")

	[Test]
	def @closures_24():
		RunCompilerTestCase("closures-24.boo")

	[Test]
	def @closures_25():
		RunCompilerTestCase("closures-25.boo")

	[Test]
	def @closures_26():
		RunCompilerTestCase("closures-26.boo")

	[Test]
	def @closures_27():
		RunCompilerTestCase("closures-27.boo")

	[Test]
	def @closures_28():
		RunCompilerTestCase("closures-28.boo")

	[Test]
	def @closures_29():
		RunCompilerTestCase("closures-29.boo")

	[Test]
	def @closures_3():
		RunCompilerTestCase("closures-3.boo")

	[Test]
	def @closures_30():
		RunCompilerTestCase("closures-30.boo")

	[Test]
	def @closures_31():
		RunCompilerTestCase("closures-31.boo")

	[Test]
	def @closures_32():
		RunCompilerTestCase("closures-32.boo")

	[Test]
	def @closures_4():
		RunCompilerTestCase("closures-4.boo")

	[Test]
	def @closures_5():
		RunCompilerTestCase("closures-5.boo")

	[Test]
	def @closures_6():
		RunCompilerTestCase("closures-6.boo")

	[Test]
	def @closures_7():
		RunCompilerTestCase("closures-7.boo")

	[Test]
	def @closures_8():
		RunCompilerTestCase("closures-8.boo")

	[Test]
	def @closures_9():
		RunCompilerTestCase("closures-9.boo")

	[Test]
	def @explicit_closure_types_for_generic_method():
		RunCompilerTestCase("explicit-closure-types-for-generic-method.boo")

	[Test]
	def @nested_functions_1():
		RunCompilerTestCase("nested-functions-1.boo")

	[Test]
	def @nested_functions_2():
		RunCompilerTestCase("nested-functions-2.boo")

	[Test]
	def @nested_functions_3():
		RunCompilerTestCase("nested-functions-3.boo")

	[Test]
	def @nested_functions_4():
		RunCompilerTestCase("nested-functions-4.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/closures"
