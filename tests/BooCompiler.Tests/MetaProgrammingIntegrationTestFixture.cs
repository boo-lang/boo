namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class MetaProgrammingIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void CodeReifierMergeIntoWithEmptyArrayLiteral()
		{
			RunCompilerTestCase(@"CodeReifierMergeIntoWithEmptyArrayLiteral.boo");
		}
		
		[Test]
		public void CodeReifierMergeIntoWithEvent()
		{
			RunCompilerTestCase(@"CodeReifierMergeIntoWithEvent.boo");
		}
		
		[Test]
		public void CodeReifierMergeIntoWithGenerators()
		{
			RunCompilerTestCase(@"CodeReifierMergeIntoWithGenerators.boo");
		}
		
		[Test]
		public void CodeReifierMergeIntoWithMacros()
		{
			RunCompilerTestCase(@"CodeReifierMergeIntoWithMacros.boo");
		}
		
		[Test]
		public void CodeReifierMergeIntoWithMultipleMethods()
		{
			RunCompilerTestCase(@"CodeReifierMergeIntoWithMultipleMethods.boo");
		}
		
		[Test]
		public void CodeReifierMergeIntoWithMultipleProperties()
		{
			RunCompilerTestCase(@"CodeReifierMergeIntoWithMultipleProperties.boo");
		}
		
		[Test]
		public void CodeReifierMergeIntoWithNestedGenericStruct()
		{
			RunCompilerTestCase(@"CodeReifierMergeIntoWithNestedGenericStruct.boo");
		}
		
		[Test]
		public void CodeReifierMergeIntoWithNestedTypes()
		{
			RunCompilerTestCase(@"CodeReifierMergeIntoWithNestedTypes.boo");
		}
		
		[Test]
		public void CodeReifierMergeIntoWithNestedTypesInDifferentOrder()
		{
			RunCompilerTestCase(@"CodeReifierMergeIntoWithNestedTypesInDifferentOrder.boo");
		}
		
		[Test]
		public void CodeReifierMergeIntoWithProperties()
		{
			RunCompilerTestCase(@"CodeReifierMergeIntoWithProperties.boo");
		}
		
		[Test]
		public void CodeReifierMergeIntoWithStatementModifiers()
		{
			RunCompilerTestCase(@"CodeReifierMergeIntoWithStatementModifiers.boo");
		}
		
		[Test]
		public void auto_lift_1()
		{
			RunCompilerTestCase(@"auto-lift-1.boo");
		}
		
		[Test]
		public void auto_lift_2()
		{
			RunCompilerTestCase(@"auto-lift-2.boo");
		}
		
		[Test]
		public void block_lift()
		{
			RunCompilerTestCase(@"block-lift.boo");
		}
		
		[Test]
		public void class_body_splicing_1()
		{
			RunCompilerTestCase(@"class-body-splicing-1.boo");
		}
		
		[Test]
		public void class_name_splicing_1()
		{
			RunCompilerTestCase(@"class-name-splicing-1.boo");
		}
		
		[Test]
		public void class_name_splicing_2()
		{
			RunCompilerTestCase(@"class-name-splicing-2.boo");
		}
		
		[Test]
		public void compile_1()
		{
			RunCompilerTestCase(@"compile-1.boo");
		}
		
		[Test]
		public void compile_2()
		{
			RunCompilerTestCase(@"compile-2.boo");
		}
		
		[Test]
		public void field_splicing_1()
		{
			RunCompilerTestCase(@"field-splicing-1.boo");
		}
		
		[Test]
		public void field_splicing_in_expression_becomes_reference_to_field()
		{
			RunCompilerTestCase(@"field-splicing-in-expression-becomes-reference-to-field.boo");
		}
		
		[Test]
		public void field_splicing_null_initializer()
		{
			RunCompilerTestCase(@"field-splicing-null-initializer.boo");
		}
		
		[Test]
		public void generic_splicing_1()
		{
			RunCompilerTestCase(@"generic-splicing-1.boo");
		}
		
		[Test]
		public void interpolation_splicing_1()
		{
			RunCompilerTestCase(@"interpolation-splicing-1.boo");
		}
		
		[Test]
		public void interpolation_splicing_2()
		{
			RunCompilerTestCase(@"interpolation-splicing-2.boo");
		}
		
		[Test]
		public void lexical_info_is_preserved()
		{
			RunCompilerTestCase(@"lexical-info-is-preserved.boo");
		}
		
		[Test]
		public void macro_yielding_selective_import()
		{
			RunCompilerTestCase(@"macro-yielding-selective-import.boo");
		}
		
		[Test]
		public void macro_yielding_types_shouldnt_cause_module_class_to_be_defined()
		{
			RunCompilerTestCase(@"macro-yielding-types-shouldnt-cause-module-class-to-be-defined.boo");
		}
		
		[Test]
		public void macro_yielding_varargs()
		{
			RunCompilerTestCase(@"macro-yielding-varargs.boo");
		}
		
		[Test]
		public void meta_methods_1()
		{
			RunCompilerTestCase(@"meta-methods-1.boo");
		}
		
		[Test]
		public void meta_methods_2()
		{
			RunCompilerTestCase(@"meta-methods-2.boo");
		}
		
		[Test]
		public void meta_methods_3()
		{
			RunCompilerTestCase(@"meta-methods-3.boo");
		}
		
		[Test]
		public void meta_methods_4()
		{
			RunCompilerTestCase(@"meta-methods-4.boo");
		}
		
		[Test]
		public void meta_methods_5()
		{
			RunCompilerTestCase(@"meta-methods-5.boo");
		}
		
		[Test]
		public void meta_methods_6()
		{
			RunCompilerTestCase(@"meta-methods-6.boo");
		}
		
		[Test]
		public void meta_methods_can_return_null()
		{
			RunCompilerTestCase(@"meta-methods-can-return-null.boo");
		}
		
		[Test]
		public void meta_methods_with_closure()
		{
			RunCompilerTestCase(@"meta-methods-with-closure.boo");
		}
		
		[Test]
		public void meta_methods_with_generator()
		{
			RunCompilerTestCase(@"meta-methods-with-generator.boo");
		}
		
		[Test]
		public void meta_methods_with_macro()
		{
			RunCompilerTestCase(@"meta-methods-with-macro.boo");
		}
		
		[Test]
		public void meta_methods_with_modifier_inside_closure()
		{
			RunCompilerTestCase(@"meta-methods-with-modifier-inside-closure.boo");
		}
		
		[Test]
		public void meta_methods_with_statement_modifier()
		{
			RunCompilerTestCase(@"meta-methods-with-statement-modifier.boo");
		}
		
		[Test]
		public void name_splicing_1()
		{
			RunCompilerTestCase(@"name-splicing-1.boo");
		}
		
		[Test]
		public void name_splicing_2()
		{
			RunCompilerTestCase(@"name-splicing-2.boo");
		}
		
		[Test]
		public void name_splicing_3()
		{
			RunCompilerTestCase(@"name-splicing-3.boo");
		}
		
		[Test]
		public void name_splicing_4()
		{
			RunCompilerTestCase(@"name-splicing-4.boo");
		}
		
		[Test]
		public void name_splicing_5()
		{
			RunCompilerTestCase(@"name-splicing-5.boo");
		}
		
		[Test]
		public void name_splicing_6()
		{
			RunCompilerTestCase(@"name-splicing-6.boo");
		}
		
		[Test]
		public void parameter_splicing_1()
		{
			RunCompilerTestCase(@"parameter-splicing-1.boo");
		}
		
		[Test]
		public void parameter_splicing_2()
		{
			RunCompilerTestCase(@"parameter-splicing-2.boo");
		}
		
		[Test]
		public void parameter_splicing_3()
		{
			RunCompilerTestCase(@"parameter-splicing-3.boo");
		}
		
		[Test]
		public void property_splicing_1()
		{
			RunCompilerTestCase(@"property-splicing-1.boo");
		}
		
		[Test]
		public void quasi_quotation_1()
		{
			RunCompilerTestCase(@"quasi-quotation-1.boo");
		}
		
		[Test]
		public void quasi_quotation_2()
		{
			RunCompilerTestCase(@"quasi-quotation-2.boo");
		}
		
		[Test]
		public void quasi_quotation_3()
		{
			RunCompilerTestCase(@"quasi-quotation-3.boo");
		}
		
		[Test]
		public void quasi_quotation_4()
		{
			RunCompilerTestCase(@"quasi-quotation-4.boo");
		}
		
		[Test]
		public void reification_1()
		{
			RunCompilerTestCase(@"reification-1.boo");
		}
		
		[Test]
		public void splicing_1()
		{
			RunCompilerTestCase(@"splicing-1.boo");
		}
		
		[Test]
		public void splicing_2()
		{
			RunCompilerTestCase(@"splicing-2.boo");
		}
		
		[Test]
		public void splicing_3()
		{
			RunCompilerTestCase(@"splicing-3.boo");
		}
		
		[Test]
		public void splicing_4()
		{
			RunCompilerTestCase(@"splicing-4.boo");
		}
		
		[Test]
		public void splicing_5()
		{
			RunCompilerTestCase(@"splicing-5.boo");
		}
		
		[Test]
		public void splicing_6()
		{
			RunCompilerTestCase(@"splicing-6.boo");
		}
		
		[Test]
		public void splicing_7()
		{
			RunCompilerTestCase(@"splicing-7.boo");
		}
		
		[Test]
		public void splicing_8()
		{
			RunCompilerTestCase(@"splicing-8.boo");
		}
		
		[Test]
		public void splicing_9()
		{
			RunCompilerTestCase(@"splicing-9.boo");
		}
		
		[Test]
		public void splicing_reference_into_enum_body()
		{
			RunCompilerTestCase(@"splicing-reference-into-enum-body.boo");
		}
		
		[Test]
		public void typedef_splicing_1()
		{
			RunCompilerTestCase(@"typedef-splicing-1.boo");
		}
		
		[Test]
		public void typeref_splicing_1()
		{
			RunCompilerTestCase(@"typeref-splicing-1.boo");
		}
		
		[Test]
		public void typeref_splicing_2()
		{
			RunCompilerTestCase(@"typeref-splicing-2.boo");
		}
		
		[Test]
		public void typeref_splicing_3()
		{
			RunCompilerTestCase(@"typeref-splicing-3.boo");
		}
		
		[Test]
		public void typeref_splicing_4()
		{
			RunCompilerTestCase(@"typeref-splicing-4.boo");
		}
		
		[Test]
		public void typeref_splicing_5()
		{
			RunCompilerTestCase(@"typeref-splicing-5.boo");
		}
		
		[Test]
		public void typeref_splicing_null()
		{
			RunCompilerTestCase(@"typeref-splicing-null.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "integration/meta-programming";
		}
	}
}
