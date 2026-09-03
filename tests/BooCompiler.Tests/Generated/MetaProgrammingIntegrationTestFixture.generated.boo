namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class MetaProgrammingIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @CodeReifierMergeIntoWithEmptyArrayLiteral():
		RunCompilerTestCase("CodeReifierMergeIntoWithEmptyArrayLiteral.boo")

	[Test]
	def @CodeReifierMergeIntoWithEvent():
		RunCompilerTestCase("CodeReifierMergeIntoWithEvent.boo")

	[Test]
	def @CodeReifierMergeIntoWithGenerators():
		RunCompilerTestCase("CodeReifierMergeIntoWithGenerators.boo")

	[Test]
	def @CodeReifierMergeIntoWithMacros():
		RunCompilerTestCase("CodeReifierMergeIntoWithMacros.boo")

	[Test]
	def @CodeReifierMergeIntoWithMultipleMethods():
		RunCompilerTestCase("CodeReifierMergeIntoWithMultipleMethods.boo")

	[Test]
	def @CodeReifierMergeIntoWithMultipleProperties():
		RunCompilerTestCase("CodeReifierMergeIntoWithMultipleProperties.boo")

	[Test]
	def @CodeReifierMergeIntoWithNestedGenericStruct():
		RunCompilerTestCase("CodeReifierMergeIntoWithNestedGenericStruct.boo")

	[Test]
	def @CodeReifierMergeIntoWithNestedTypes():
		RunCompilerTestCase("CodeReifierMergeIntoWithNestedTypes.boo")

	[Test]
	def @CodeReifierMergeIntoWithNestedTypesInDifferentOrder():
		RunCompilerTestCase("CodeReifierMergeIntoWithNestedTypesInDifferentOrder.boo")

	[Test]
	def @CodeReifierMergeIntoWithProperties():
		RunCompilerTestCase("CodeReifierMergeIntoWithProperties.boo")

	[Test]
	def @CodeReifierMergeIntoWithStatementModifiers():
		RunCompilerTestCase("CodeReifierMergeIntoWithStatementModifiers.boo")

	[Test]
	def @auto_lift_1():
		RunCompilerTestCase("auto-lift-1.boo")

	[Test]
	def @auto_lift_2():
		RunCompilerTestCase("auto-lift-2.boo")

	[Test]
	def @block_lift():
		RunCompilerTestCase("block-lift.boo")

	[Test]
	def @class_body_splicing_1():
		RunCompilerTestCase("class-body-splicing-1.boo")

	[Test]
	def @class_name_splicing_1():
		RunCompilerTestCase("class-name-splicing-1.boo")

	[Test]
	def @class_name_splicing_2():
		RunCompilerTestCase("class-name-splicing-2.boo")

	[Test]
	def @compile_1():
		RunCompilerTestCase("compile-1.boo")

	[Test]
	def @compile_2():
		RunCompilerTestCase("compile-2.boo")

	[Test]
	def @field_splicing_1():
		RunCompilerTestCase("field-splicing-1.boo")

	[Test]
	def @field_splicing_in_expression_becomes_reference_to_field():
		RunCompilerTestCase("field-splicing-in-expression-becomes-reference-to-field.boo")

	[Test]
	def @field_splicing_null_initializer():
		RunCompilerTestCase("field-splicing-null-initializer.boo")

	[Test]
	def @generic_splicing_1():
		RunCompilerTestCase("generic-splicing-1.boo")

	[Test]
	def @interpolation_splicing_1():
		RunCompilerTestCase("interpolation-splicing-1.boo")

	[Test]
	def @interpolation_splicing_2():
		RunCompilerTestCase("interpolation-splicing-2.boo")

	[Test]
	def @lexical_info_is_preserved():
		RunCompilerTestCase("lexical-info-is-preserved.boo")

	[Test]
	def @macro_yielding_selective_import():
		RunCompilerTestCase("macro-yielding-selective-import.boo")

	[Test]
	def @macro_yielding_types_shouldnt_cause_module_class_to_be_defined():
		RunCompilerTestCase("macro-yielding-types-shouldnt-cause-module-class-to-be-defined.boo")

	[Test]
	def @macro_yielding_varargs():
		RunCompilerTestCase("macro-yielding-varargs.boo")

	[Test]
	def @meta_methods_1():
		RunCompilerTestCase("meta-methods-1.boo")

	[Test]
	def @meta_methods_2():
		RunCompilerTestCase("meta-methods-2.boo")

	[Test]
	def @meta_methods_3():
		RunCompilerTestCase("meta-methods-3.boo")

	[Test]
	def @meta_methods_4():
		RunCompilerTestCase("meta-methods-4.boo")

	[Test]
	def @meta_methods_5():
		RunCompilerTestCase("meta-methods-5.boo")

	[Test]
	def @meta_methods_6():
		RunCompilerTestCase("meta-methods-6.boo")

	[Test]
	def @meta_methods_can_return_null():
		RunCompilerTestCase("meta-methods-can-return-null.boo")

	[Test]
	def @meta_methods_with_closure():
		RunCompilerTestCase("meta-methods-with-closure.boo")

	[Test]
	def @meta_methods_with_generator():
		RunCompilerTestCase("meta-methods-with-generator.boo")

	[Test]
	def @meta_methods_with_macro():
		RunCompilerTestCase("meta-methods-with-macro.boo")

	[Test]
	def @meta_methods_with_modifier_inside_closure():
		RunCompilerTestCase("meta-methods-with-modifier-inside-closure.boo")

	[Test]
	def @meta_methods_with_statement_modifier():
		RunCompilerTestCase("meta-methods-with-statement-modifier.boo")

	[Test]
	def @name_splicing_1():
		RunCompilerTestCase("name-splicing-1.boo")

	[Test]
	def @name_splicing_2():
		RunCompilerTestCase("name-splicing-2.boo")

	[Test]
	def @name_splicing_3():
		RunCompilerTestCase("name-splicing-3.boo")

	[Test]
	def @name_splicing_4():
		RunCompilerTestCase("name-splicing-4.boo")

	[Test]
	def @name_splicing_5():
		RunCompilerTestCase("name-splicing-5.boo")

	[Test]
	def @name_splicing_6():
		RunCompilerTestCase("name-splicing-6.boo")

	[Test]
	def @parameter_splicing_1():
		RunCompilerTestCase("parameter-splicing-1.boo")

	[Test]
	def @parameter_splicing_2():
		RunCompilerTestCase("parameter-splicing-2.boo")

	[Test]
	def @parameter_splicing_3():
		RunCompilerTestCase("parameter-splicing-3.boo")

	[Test]
	def @property_splicing_1():
		RunCompilerTestCase("property-splicing-1.boo")

	[Test]
	def @quasi_quotation_1():
		RunCompilerTestCase("quasi-quotation-1.boo")

	[Test]
	def @quasi_quotation_2():
		RunCompilerTestCase("quasi-quotation-2.boo")

	[Test]
	def @quasi_quotation_3():
		RunCompilerTestCase("quasi-quotation-3.boo")

	[Test]
	def @quasi_quotation_4():
		RunCompilerTestCase("quasi-quotation-4.boo")

	[Test]
	def @reification_1():
		RunCompilerTestCase("reification-1.boo")

	[Test]
	def @splicing_1():
		RunCompilerTestCase("splicing-1.boo")

	[Test]
	def @splicing_2():
		RunCompilerTestCase("splicing-2.boo")

	[Test]
	def @splicing_3():
		RunCompilerTestCase("splicing-3.boo")

	[Test]
	def @splicing_4():
		RunCompilerTestCase("splicing-4.boo")

	[Test]
	def @splicing_5():
		RunCompilerTestCase("splicing-5.boo")

	[Test]
	def @splicing_6():
		RunCompilerTestCase("splicing-6.boo")

	[Test]
	def @splicing_7():
		RunCompilerTestCase("splicing-7.boo")

	[Test]
	def @splicing_8():
		RunCompilerTestCase("splicing-8.boo")

	[Test]
	def @splicing_9():
		RunCompilerTestCase("splicing-9.boo")

	[Test]
	def @splicing_reference_into_enum_body():
		RunCompilerTestCase("splicing-reference-into-enum-body.boo")

	[Test]
	def @typedef_splicing_1():
		RunCompilerTestCase("typedef-splicing-1.boo")

	[Test]
	def @typeref_splicing_1():
		RunCompilerTestCase("typeref-splicing-1.boo")

	[Test]
	def @typeref_splicing_2():
		RunCompilerTestCase("typeref-splicing-2.boo")

	[Test]
	def @typeref_splicing_3():
		RunCompilerTestCase("typeref-splicing-3.boo")

	[Test]
	def @typeref_splicing_4():
		RunCompilerTestCase("typeref-splicing-4.boo")

	[Test]
	def @typeref_splicing_5():
		RunCompilerTestCase("typeref-splicing-5.boo")

	[Test]
	def @typeref_splicing_null():
		RunCompilerTestCase("typeref-splicing-null.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/meta-programming"
