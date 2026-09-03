namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class MacrosTestFixture(AbstractCompilerTestCase):

	[Test]
	def @assert_1():
		RunCompilerTestCase("assert-1.boo")

	[Test]
	def @custom_class_macro_as_generic_argument():
		RunCompilerTestCase("custom-class-macro-as-generic-argument.boo")

	[Test]
	def @custom_class_macro_with_internal_field():
		RunCompilerTestCase("custom-class-macro-with-internal-field.boo")

	[Test]
	def @custom_class_macro_with_internal_property():
		RunCompilerTestCase("custom-class-macro-with-internal-property.boo")

	[Test]
	def @custom_class_macro_with_method_override():
		RunCompilerTestCase("custom-class-macro-with-method-override.boo")

	[Test]
	def @custom_class_macro_with_properties_and_field():
		RunCompilerTestCase("custom-class-macro-with-properties-and-field.boo")

	[Test]
	def @custom_class_macro_with_properties():
		RunCompilerTestCase("custom-class-macro-with-properties.boo")

	[Test]
	def @custom_class_macro_with_property_macro():
		RunCompilerTestCase("custom-class-macro-with-property-macro.boo")

	[Test]
	def @custom_class_macro_with_simple_method_and_field():
		RunCompilerTestCase("custom-class-macro-with-simple-method-and-field.boo")

	[Test]
	def @custom_class_macro_with_simple_method():
		RunCompilerTestCase("custom-class-macro-with-simple-method.boo")

	[Test]
	def @debug_1():
		RunCompilerTestCase("debug-1.boo")

	[Test]
	def @generator_macro_1():
		RunCompilerTestCase("generator-macro-1.boo")

	[Test]
	def @generator_macro_2():
		RunCompilerTestCase("generator-macro-2.boo")

	[Test]
	def @generator_macro_3():
		RunCompilerTestCase("generator-macro-3.boo")

	[Test]
	def @generator_macro_4():
		RunCompilerTestCase("generator-macro-4.boo")

	[Test]
	def @generator_macro_5():
		RunCompilerTestCase("generator-macro-5.boo")

	[Test]
	def @ifdef_1():
		RunCompilerTestCase("ifdef-1.boo")

	[Test]
	def @internal_macro_is_preferred():
		RunCompilerTestCase("internal-macro-is-preferred.boo")

	[Test]
	def @macro_1():
		RunCompilerTestCase("macro-1.boo")

	[Test]
	def @macro_2():
		RunCompilerTestCase("macro-2.boo")

	[Test]
	def @macro_3():
		RunCompilerTestCase("macro-3.boo")

	[Test]
	def @macro_4():
		RunCompilerTestCase("macro-4.boo")

	[Test]
	def @macro_5():
		RunCompilerTestCase("macro-5.boo")

	[Test]
	def @macro_arguments_1():
		RunCompilerTestCase("macro-arguments-1.boo")

	[Test]
	def @macro_arguments_2():
		RunCompilerTestCase("macro-arguments-2.boo")

	[Test]
	def @macro_attribute_fpa():
		RunCompilerTestCase("macro-attribute-fpa.boo")

	[Test]
	def @macro_case_1():
		RunCompilerTestCase("macro-case-1.boo")

	[Test]
	def @macro_case_otherwise_2():
		RunCompilerTestCase("macro-case-otherwise-2.boo")

	[Test]
	def @macro_case_otherwise():
		RunCompilerTestCase("macro-case-otherwise.boo")

	[Test]
	def @macro_expansion_order_1():
		RunCompilerTestCase("macro-expansion-order-1.boo")

	[Test]
	def @macro_imports_1():
		RunCompilerTestCase("macro-imports-1.boo")

	[Test]
	def @macro_should_be_able_to_reach_module():
		RunCompilerTestCase("macro-should-be-able-to-reach-module.boo")

	[Test]
	def @macro_yielding_generic_class():
		RunCompilerTestCase("macro-yielding-generic-class.boo")

	[Test]
	def @macro_yielding_generic_method_1():
		RunCompilerTestCase("macro-yielding-generic-method-1.boo")

	[Test]
	def @macro_yielding_partial_enum_with_existing_partial_definition():
		RunCompilerTestCase("macro-yielding-partial-enum-with-existing-partial-definition.boo")

	[Test]
	def @macro_yielding_partial_enums():
		RunCompilerTestCase("macro-yielding-partial-enums.boo")

	[Test]
	def @member_macro_changing_all_sibling_method_bodies():
		RunCompilerTestCase("member-macro-changing-all-sibling-method-bodies.boo")

	[Test]
	def @member_macro_contributing_initialization_code():
		RunCompilerTestCase("member-macro-contributing-initialization-code.boo")

	[Test]
	def @member_macro_initialization_order():
		RunCompilerTestCase("member-macro-initialization-order.boo")

	[Test]
	def @member_macro_nodes_inherit_visibility_and_attributes():
		RunCompilerTestCase("member-macro-nodes-inherit-visibility-and-attributes.boo")

	[Test]
	def @member_macro_nodes_inherit_visibility_only_when_not_set():
		RunCompilerTestCase("member-macro-nodes-inherit-visibility-only-when-not-set.boo")

	[Test]
	def @member_macro_producing_field_and_constructor():
		RunCompilerTestCase("member-macro-producing-field-and-constructor.boo")

	[Test]
	def @member_macro_producing_field_and_property():
		RunCompilerTestCase("member-macro-producing-field-and-property.boo")

	[Test]
	def @nested_macros_1():
		RunCompilerTestCase("nested-macros-1.boo")

	[Test]
	def @nested_macros_2():
		RunCompilerTestCase("nested-macros-2.boo")

	[Test]
	def @nested_macros_3():
		RunCompilerTestCase("nested-macros-3.boo")

	[Test]
	def @nested_macros_4():
		RunCompilerTestCase("nested-macros-4.boo")

	[Test]
	def @nested_macros_5():
		RunCompilerTestCase("nested-macros-5.boo")

	[Test]
	def @nested_macros_6():
		RunCompilerTestCase("nested-macros-6.boo")

	[Ignore("Use of external extension within another external extension does not work yet")][Test]
	def @nested_macros_7():
		RunCompilerTestCase("nested-macros-7.boo")

	[Test]
	def @nested_macros_8():
		RunCompilerTestCase("nested-macros-8.boo")

	[Test]
	def @nested_macros():
		RunCompilerTestCase("nested-macros.boo")

	[Test]
	def @preserving_1():
		RunCompilerTestCase("preserving-1.boo")

	[Test]
	def @print_1():
		RunCompilerTestCase("print-1.boo")

	[Test]
	def @print_2():
		RunCompilerTestCase("print-2.boo")

	[Test]
	def @then_can_be_used_as_macro_name():
		RunCompilerTestCase("then-can-be-used-as-macro-name.boo")

	[Test]
	def @type_member_macro_yielding_member_with_member_generating_attribute():
		RunCompilerTestCase("type-member-macro-yielding-member-with-member-generating-attribute.boo")

	[Test]
	def @using_1():
		RunCompilerTestCase("using-1.boo")

	[Test]
	def @using_2():
		RunCompilerTestCase("using-2.boo")

	[Test]
	def @using_3():
		RunCompilerTestCase("using-3.boo")

	[Test]
	def @using_4():
		RunCompilerTestCase("using-4.boo")

	[Test]
	def @using_5():
		RunCompilerTestCase("using-5.boo")

	[Test]
	def @var_1():
		RunCompilerTestCase("var-1.boo")

	[Test]
	def @yieldAll_1():
		RunCompilerTestCase("yieldAll-1.boo")

	[Test]
	def @yieldAll_2():
		RunCompilerTestCase("yieldAll-2.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "macros"
