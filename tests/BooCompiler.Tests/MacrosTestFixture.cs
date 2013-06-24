
namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class MacrosTestFixture : AbstractCompilerTestCase
	{


		[Test]
		public void assert_1()
		{
			RunCompilerTestCase(@"assert-1.boo");
		}
		
		[Test]
		public void custom_class_macro_as_generic_argument()
		{
			RunCompilerTestCase(@"custom-class-macro-as-generic-argument.boo");
		}
		
		[Test]
		public void custom_class_macro_with_internal_field()
		{
			RunCompilerTestCase(@"custom-class-macro-with-internal-field.boo");
		}
		
		[Test]
		public void custom_class_macro_with_internal_property()
		{
			RunCompilerTestCase(@"custom-class-macro-with-internal-property.boo");
		}
		
		[Test]
		public void custom_class_macro_with_method_override()
		{
			RunCompilerTestCase(@"custom-class-macro-with-method-override.boo");
		}
		
		[Test]
		public void custom_class_macro_with_properties_and_field()
		{
			RunCompilerTestCase(@"custom-class-macro-with-properties-and-field.boo");
		}
		
		[Test]
		public void custom_class_macro_with_properties()
		{
			RunCompilerTestCase(@"custom-class-macro-with-properties.boo");
		}
		
		[Test]
		public void custom_class_macro_with_property_macro()
		{
			RunCompilerTestCase(@"custom-class-macro-with-property-macro.boo");
		}
		
		[Test]
		public void custom_class_macro_with_simple_method_and_field()
		{
			RunCompilerTestCase(@"custom-class-macro-with-simple-method-and-field.boo");
		}
		
		[Test]
		public void custom_class_macro_with_simple_method()
		{
			RunCompilerTestCase(@"custom-class-macro-with-simple-method.boo");
		}
		
		[Test]
		public void debug_1()
		{
			RunCompilerTestCase(@"debug-1.boo");
		}
		
		[Test]
		public void generator_macro_1()
		{
			RunCompilerTestCase(@"generator-macro-1.boo");
		}
		
		[Test]
		public void generator_macro_2()
		{
			RunCompilerTestCase(@"generator-macro-2.boo");
		}
		
		[Test]
		public void generator_macro_3()
		{
			RunCompilerTestCase(@"generator-macro-3.boo");
		}
		
		[Test]
		public void generator_macro_4()
		{
			RunCompilerTestCase(@"generator-macro-4.boo");
		}
		
		[Test]
		public void generator_macro_5()
		{
			RunCompilerTestCase(@"generator-macro-5.boo");
		}
		
		[Test]
		public void ifdef_1()
		{
			RunCompilerTestCase(@"ifdef-1.boo");
		}
		
		[Test]
		public void internal_macro_is_preferred()
		{
			RunCompilerTestCase(@"internal-macro-is-preferred.boo");
		}
		
		[Test]
		public void macro_1()
		{
			RunCompilerTestCase(@"macro-1.boo");
		}
		
		[Test]
		public void macro_2()
		{
			RunCompilerTestCase(@"macro-2.boo");
		}
		
		[Test]
		public void macro_3()
		{
			RunCompilerTestCase(@"macro-3.boo");
		}
		
		[Test]
		public void macro_4()
		{
			RunCompilerTestCase(@"macro-4.boo");
		}
		
		[Test]
		public void macro_5()
		{
			RunCompilerTestCase(@"macro-5.boo");
		}
		
		[Test]
		public void macro_arguments_1()
		{
			RunCompilerTestCase(@"macro-arguments-1.boo");
		}
		
		[Test]
		public void macro_arguments_2()
		{
			RunCompilerTestCase(@"macro-arguments-2.boo");
		}
		
		[Test]
		public void macro_attribute_fpa()
		{
			RunCompilerTestCase(@"macro-attribute-fpa.boo");
		}
		
		[Test]
		public void macro_case_1()
		{
			RunCompilerTestCase(@"macro-case-1.boo");
		}
		
		[Test]
		public void macro_case_otherwise_2()
		{
			RunCompilerTestCase(@"macro-case-otherwise-2.boo");
		}
		
		[Test]
		public void macro_case_otherwise()
		{
			RunCompilerTestCase(@"macro-case-otherwise.boo");
		}
		
		[Test]
		public void macro_expansion_order_1()
		{
			RunCompilerTestCase(@"macro-expansion-order-1.boo");
		}
		
		[Test]
		public void macro_imports_1()
		{
			RunCompilerTestCase(@"macro-imports-1.boo");
		}
		
		[Test]
		public void macro_should_be_able_to_reach_module()
		{
			RunCompilerTestCase(@"macro-should-be-able-to-reach-module.boo");
		}
		
		[Test]
		public void macro_yielding_generic_class()
		{
			RunCompilerTestCase(@"macro-yielding-generic-class.boo");
		}
		
		[Test]
		public void macro_yielding_generic_method_1()
		{
			RunCompilerTestCase(@"macro-yielding-generic-method-1.boo");
		}
		
		[Test]
		public void macro_yielding_partial_enum_with_existing_partial_definition()
		{
			RunCompilerTestCase(@"macro-yielding-partial-enum-with-existing-partial-definition.boo");
		}
		
		[Test]
		public void macro_yielding_partial_enums()
		{
			RunCompilerTestCase(@"macro-yielding-partial-enums.boo");
		}
		
		[Test]
		public void member_macro_changing_all_sibling_method_bodies()
		{
			RunCompilerTestCase(@"member-macro-changing-all-sibling-method-bodies.boo");
		}
		
		[Test]
		public void member_macro_contributing_initialization_code()
		{
			RunCompilerTestCase(@"member-macro-contributing-initialization-code.boo");
		}
		
		[Test]
		public void member_macro_initialization_order()
		{
			RunCompilerTestCase(@"member-macro-initialization-order.boo");
		}
		
		[Test]
		public void member_macro_nodes_inherit_visibility_and_attributes()
		{
			RunCompilerTestCase(@"member-macro-nodes-inherit-visibility-and-attributes.boo");
		}
		
		[Test]
		public void member_macro_nodes_inherit_visibility_only_when_not_set()
		{
			RunCompilerTestCase(@"member-macro-nodes-inherit-visibility-only-when-not-set.boo");
		}
		
		[Test]
		public void member_macro_producing_field_and_constructor()
		{
			RunCompilerTestCase(@"member-macro-producing-field-and-constructor.boo");
		}
		
		[Test]
		public void member_macro_producing_field_and_property()
		{
			RunCompilerTestCase(@"member-macro-producing-field-and-property.boo");
		}
		
		[Test]
		public void nested_macros_1()
		{
			RunCompilerTestCase(@"nested-macros-1.boo");
		}
		
		[Test]
		public void nested_macros_2()
		{
			RunCompilerTestCase(@"nested-macros-2.boo");
		}
		
		[Test]
		public void nested_macros_3()
		{
			RunCompilerTestCase(@"nested-macros-3.boo");
		}
		
		[Test]
		public void nested_macros_4()
		{
			RunCompilerTestCase(@"nested-macros-4.boo");
		}
		
		[Test]
		public void nested_macros_5()
		{
			RunCompilerTestCase(@"nested-macros-5.boo");
		}
		
		[Test]
		public void nested_macros_6()
		{
			RunCompilerTestCase(@"nested-macros-6.boo");
		}
		
		[Ignore("Use of external extension within another external extension does not work yet")][Test]
		public void nested_macros_7()
		{
			RunCompilerTestCase(@"nested-macros-7.boo");
		}
		
		[Test]
		public void nested_macros_8()
		{
			RunCompilerTestCase(@"nested-macros-8.boo");
		}
		
		[Test]
		public void nested_macros()
		{
			RunCompilerTestCase(@"nested-macros.boo");
		}
		
		[Test]
		public void preserving_1()
		{
			RunCompilerTestCase(@"preserving-1.boo");
		}
		
		[Test]
		public void print_1()
		{
			RunCompilerTestCase(@"print-1.boo");
		}
		
		[Test]
		public void print_2()
		{
			RunCompilerTestCase(@"print-2.boo");
		}
		
		[Test]
		public void then_can_be_used_as_macro_name()
		{
			RunCompilerTestCase(@"then-can-be-used-as-macro-name.boo");
		}
		
		[Test]
		public void type_member_macro_yielding_member_with_member_generating_attribute()
		{
			RunCompilerTestCase(@"type-member-macro-yielding-member-with-member-generating-attribute.boo");
		}
		
		[Test]
		public void using_1()
		{
			RunCompilerTestCase(@"using-1.boo");
		}
		
		[Test]
		public void using_2()
		{
			RunCompilerTestCase(@"using-2.boo");
		}
		
		[Test]
		public void using_3()
		{
			RunCompilerTestCase(@"using-3.boo");
		}
		
		[Test]
		public void using_4()
		{
			RunCompilerTestCase(@"using-4.boo");
		}
		
		[Test]
		public void using_5()
		{
			RunCompilerTestCase(@"using-5.boo");
		}
		
		[Test]
		public void var_1()
		{
			RunCompilerTestCase(@"var-1.boo");
		}
		
		[Test]
		public void yieldAll_1()
		{
			RunCompilerTestCase(@"yieldAll-1.boo");
		}
		
		[Test]
		public void yieldAll_2()
		{
			RunCompilerTestCase(@"yieldAll-2.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "macros";
		}
	}
}
