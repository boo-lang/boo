namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class ArraysIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @array_polymorphic_foreach():
		RunCompilerTestCase("array-polymorphic-foreach.boo")

	[Test]
	def @arrays_1():
		RunCompilerTestCase("arrays-1.boo")

	[Test]
	def @arrays_10():
		RunCompilerTestCase("arrays-10.boo")

	[Test]
	def @arrays_11():
		RunCompilerTestCase("arrays-11.boo")

	[Test]
	def @arrays_12():
		RunCompilerTestCase("arrays-12.boo")

	[Test]
	def @arrays_13():
		RunCompilerTestCase("arrays-13.boo")

	[Test]
	def @arrays_14():
		RunCompilerTestCase("arrays-14.boo")

	[Test]
	def @arrays_15():
		RunCompilerTestCase("arrays-15.boo")

	[Test]
	def @arrays_16():
		RunCompilerTestCase("arrays-16.boo")

	[Test]
	def @arrays_17():
		RunCompilerTestCase("arrays-17.boo")

	[Test]
	def @arrays_18():
		RunCompilerTestCase("arrays-18.boo")

	[Test]
	def @arrays_19():
		RunCompilerTestCase("arrays-19.boo")

	[Test]
	def @arrays_2():
		RunCompilerTestCase("arrays-2.boo")

	[Test]
	def @arrays_20():
		RunCompilerTestCase("arrays-20.boo")

	[Test]
	def @arrays_21():
		RunCompilerTestCase("arrays-21.boo")

	[Test]
	def @arrays_22():
		RunCompilerTestCase("arrays-22.boo")

	[Test]
	def @arrays_23():
		RunCompilerTestCase("arrays-23.boo")

	[Test]
	def @arrays_24():
		RunCompilerTestCase("arrays-24.boo")

	[Test]
	def @arrays_25():
		RunCompilerTestCase("arrays-25.boo")

	[Test]
	def @arrays_26():
		RunCompilerTestCase("arrays-26.boo")

	[Test]
	def @arrays_27():
		RunCompilerTestCase("arrays-27.boo")

	[Test]
	def @arrays_28():
		RunCompilerTestCase("arrays-28.boo")

	[Test]
	def @arrays_29():
		RunCompilerTestCase("arrays-29.boo")

	[Test]
	def @arrays_3():
		RunCompilerTestCase("arrays-3.boo")

	[Test]
	def @arrays_30():
		RunCompilerTestCase("arrays-30.boo")

	[Test]
	def @arrays_31():
		RunCompilerTestCase("arrays-31.boo")

	[Test]
	def @arrays_32():
		RunCompilerTestCase("arrays-32.boo")

	[Test]
	def @arrays_33():
		RunCompilerTestCase("arrays-33.boo")

	[Test]
	def @arrays_34():
		RunCompilerTestCase("arrays-34.boo")

	[Test]
	def @arrays_35():
		RunCompilerTestCase("arrays-35.boo")

	[Test]
	def @arrays_36():
		RunCompilerTestCase("arrays-36.boo")

	[Test]
	def @arrays_37():
		RunCompilerTestCase("arrays-37.boo")

	[Test]
	def @arrays_38():
		RunCompilerTestCase("arrays-38.boo")

	[Test]
	def @arrays_39():
		RunCompilerTestCase("arrays-39.boo")

	[Test]
	def @arrays_4():
		RunCompilerTestCase("arrays-4.boo")

	[Test]
	def @arrays_40():
		RunCompilerTestCase("arrays-40.boo")

	[Test]
	def @arrays_41():
		RunCompilerTestCase("arrays-41.boo")

	[Test]
	def @arrays_42():
		RunCompilerTestCase("arrays-42.boo")

	[Test]
	def @arrays_43():
		RunCompilerTestCase("arrays-43.boo")

	[Test]
	def @arrays_44():
		RunCompilerTestCase("arrays-44.boo")

	[Test]
	def @arrays_5():
		RunCompilerTestCase("arrays-5.boo")

	[Test]
	def @arrays_6():
		RunCompilerTestCase("arrays-6.boo")

	[Test]
	def @arrays_7():
		RunCompilerTestCase("arrays-7.boo")

	[Test]
	def @arrays_8():
		RunCompilerTestCase("arrays-8.boo")

	[Test]
	def @arrays_9():
		RunCompilerTestCase("arrays-9.boo")

	[Test]
	def @empty_array_inference_1():
		RunCompilerTestCase("empty-array-inference-1.boo")

	[Test]
	def @empty_array_inference_as_enumerable_of_int():
		RunCompilerTestCase("empty-array-inference-as-enumerable-of-int.boo")

	[Test]
	def @empty_array_inference_as_enumerable():
		RunCompilerTestCase("empty-array-inference-as-enumerable.boo")

	[Test]
	def @empty_array_inference_as_object():
		RunCompilerTestCase("empty-array-inference-as-object.boo")

	[Test]
	def @empty_array_inference_in_closure():
		RunCompilerTestCase("empty-array-inference-in-closure.boo")

	[Test]
	def @empty_array_inference_with_varargs():
		RunCompilerTestCase("empty-array-inference-with-varargs.boo")

	[Test]
	def @matrix_with_type_reference():
		RunCompilerTestCase("matrix-with-type-reference.boo")

	[Test]
	def @per_module_raw_array_indexing():
		RunCompilerTestCase("per-module-raw-array-indexing.boo")

	[Test]
	def @rawarrayindexing_1():
		RunCompilerTestCase("rawarrayindexing-1.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/arrays"
