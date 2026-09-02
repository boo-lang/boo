namespace BooCompiler.Tests

import NUnit.Framework

partial class SemanticsTestFixture:

	[Test]
	def @abstract_method0():
		RunCompilerTestCase("abstract_method0.boo")

	[Test]
	def @abstract_method_stubs0():
		RunCompilerTestCase("abstract_method_stubs0.boo")

	[Test]
	def @assert0():
		RunCompilerTestCase("assert0.boo")

	[Test]
	def @assert1():
		RunCompilerTestCase("assert1.boo")

	[Test]
	def @assign_property():
		RunCompilerTestCase("assign_property.boo")

	[Test]
	def @callables_1():
		RunCompilerTestCase("callables-1.boo")

	[Test]
	def @classes0():
		RunCompilerTestCase("classes0.boo")

	[Test]
	def @classes1():
		RunCompilerTestCase("classes1.boo")

	[Test]
	def @collection_initializer():
		RunCompilerTestCase("collection-initializer.boo")

	[Test]
	def @enum0():
		RunCompilerTestCase("enum0.boo")

	[Test]
	def @enum1():
		RunCompilerTestCase("enum1.boo")

	[Test]
	def @equality0():
		RunCompilerTestCase("equality0.boo")

	[Test]
	def @fields_1():
		RunCompilerTestCase("fields-1.boo")

	[Test]
	def @fields_2():
		RunCompilerTestCase("fields-2.boo")

	[Test]
	def @fields_3():
		RunCompilerTestCase("fields-3.boo")

	[Test]
	def @fields_4():
		RunCompilerTestCase("fields-4.boo")

	[Test]
	def @fields_5():
		RunCompilerTestCase("fields-5.boo")

	[Test]
	def @fields_6():
		RunCompilerTestCase("fields-6.boo")

	[Test]
	def @fields_7():
		RunCompilerTestCase("fields-7.boo")

	[Test]
	def @fields_8():
		RunCompilerTestCase("fields-8.boo")

	[Test]
	def @folding_0():
		RunCompilerTestCase("folding-0.boo")

	[Test]
	def @for_1():
		RunCompilerTestCase("for-1.boo")

	[Test]
	def @for_2():
		RunCompilerTestCase("for-2.boo")

	[Test]
	def @hash_initializer():
		RunCompilerTestCase("hash-initializer.boo")

	[Test]
	def @hash0():
		RunCompilerTestCase("hash0.boo")

	[Test]
	def @in_string():
		RunCompilerTestCase("in_string.boo")

	[Test]
	def @interfaces_0():
		RunCompilerTestCase("interfaces-0.boo")

	[Test]
	def @interfaces_1():
		RunCompilerTestCase("interfaces-1.boo")

	[Test]
	def @interfaces_2():
		RunCompilerTestCase("interfaces-2.boo")

	[Test]
	def @is_0():
		RunCompilerTestCase("is-0.boo")

	[Test]
	def @len():
		RunCompilerTestCase("len.boo")

	[Test]
	def @lock0():
		RunCompilerTestCase("lock0.boo")

	[Test]
	def @lock1():
		RunCompilerTestCase("lock1.boo")

	[Test]
	def @lock2():
		RunCompilerTestCase("lock2.boo")

	[Test]
	def @method2():
		RunCompilerTestCase("method2.boo")

	[Test]
	def @method3():
		RunCompilerTestCase("method3.boo")

	[Test]
	def @method6():
		RunCompilerTestCase("method6.boo")

	[Test]
	def @method7():
		RunCompilerTestCase("method7.boo")

	[Test]
	def @module0():
		RunCompilerTestCase("module0.boo")

	[Test]
	def @null0():
		RunCompilerTestCase("null0.boo")

	[Test]
	def @null1():
		RunCompilerTestCase("null1.boo")

	[Test]
	def @numericpromo0():
		RunCompilerTestCase("numericpromo0.boo")

	[Test]
	def @omitted_target_1():
		RunCompilerTestCase("omitted-target-1.boo")

	[Test]
	def @regex_is_cached_in_static_field_unless_assigned():
		RunCompilerTestCase("regex-is-cached-in-static-field-unless-assigned.boo")

	[Test]
	def @slice_property():
		RunCompilerTestCase("slice_property.boo")

	[Test]
	def @slice_property_int():
		RunCompilerTestCase("slice_property_int.boo")

	[Test]
	def @static_field_initializer():
		RunCompilerTestCase("static_field_initializer.boo")

	[Test]
	def @stringslice0():
		RunCompilerTestCase("stringslice0.boo")

	[Test]
	def @stringslice1():
		RunCompilerTestCase("stringslice1.boo")

	[Test]
	def @struct_1():
		RunCompilerTestCase("struct-1.boo")

	[Test]
	def @type_resolution0():
		RunCompilerTestCase("type_resolution0.boo")

	[Test]
	def @using0():
		RunCompilerTestCase("using0.boo")

	[Test]
	def @using1():
		RunCompilerTestCase("using1.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "semantics"
