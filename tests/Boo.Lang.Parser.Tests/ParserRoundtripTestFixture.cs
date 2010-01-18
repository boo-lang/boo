
namespace Boo.Lang.Parser.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class ParserRoundtripTestFixture : AbstractParserTestFixture
	{
		void RunCompilerTestCase(string fname)
		{
			RunParserTestCase(fname);
		}

		[Test]
		public void and_or_1()
		{
			RunCompilerTestCase(@"and-or-1.boo");
		}
		
		[Test]
		public void arrays_1()
		{
			RunCompilerTestCase(@"arrays-1.boo");
		}
		
		[Test]
		public void arrays_2()
		{
			RunCompilerTestCase(@"arrays-2.boo");
		}
		
		[Test]
		public void arrays_3()
		{
			RunCompilerTestCase(@"arrays-3.boo");
		}
		
		[Test]
		public void arrays_4()
		{
			RunCompilerTestCase(@"arrays-4.boo");
		}
		
		[Test]
		public void arrays_5()
		{
			RunCompilerTestCase(@"arrays-5.boo");
		}
		
		[Test]
		public void arrays_6()
		{
			RunCompilerTestCase(@"arrays-6.boo");
		}
		
		[Test]
		public void as_1()
		{
			RunCompilerTestCase(@"as-1.boo");
		}
		
		[Test]
		public void assignment_1()
		{
			RunCompilerTestCase(@"assignment-1.boo");
		}
		
		[Test]
		public void ast_literals_1()
		{
			RunCompilerTestCase(@"ast-literals-1.boo");
		}
		
		[Test]
		public void ast_literals_2()
		{
			RunCompilerTestCase(@"ast-literals-2.boo");
		}
		
		[Test]
		public void ast_literals_3()
		{
			RunCompilerTestCase(@"ast-literals-3.boo");
		}
		
		[Test]
		public void ast_literals_4()
		{
			RunCompilerTestCase(@"ast-literals-4.boo");
		}
		
		[Test]
		public void ast_literals_5()
		{
			RunCompilerTestCase(@"ast-literals-5.boo");
		}
		
		[Test]
		public void ast_literals_6()
		{
			RunCompilerTestCase(@"ast-literals-6.boo");
		}
		
		[Test]
		public void ast_literals_7()
		{
			RunCompilerTestCase(@"ast-literals-7.boo");
		}
		
		[Test]
		public void ast_literals_8()
		{
			RunCompilerTestCase(@"ast-literals-8.boo");
		}
		
		[Test]
		public void ast_literals_9()
		{
			RunCompilerTestCase(@"ast-literals-9.boo");
		}
		
		[Test]
		public void attributes_1()
		{
			RunCompilerTestCase(@"attributes-1.boo");
		}
		
		[Test]
		public void attributes_2()
		{
			RunCompilerTestCase(@"attributes-2.boo");
		}
		
		[Test]
		public void bool_literals_1()
		{
			RunCompilerTestCase(@"bool-literals-1.boo");
		}
		
		[Test]
		public void callables_1()
		{
			RunCompilerTestCase(@"callables-1.boo");
		}
		
		[Test]
		public void callables_2()
		{
			RunCompilerTestCase(@"callables-2.boo");
		}
		
		[Test]
		public void callables_with_varags()
		{
			RunCompilerTestCase(@"callables-with-varags.boo");
		}
		
		[Test]
		public void char_1()
		{
			RunCompilerTestCase(@"char-1.boo");
		}
		
		[Test]
		public void char_2()
		{
			RunCompilerTestCase(@"char-2.boo");
		}
		
		[Test]
		public void class_1()
		{
			RunCompilerTestCase(@"class-1.boo");
		}
		
		[Test]
		public void class_2()
		{
			RunCompilerTestCase(@"class-2.boo");
		}
		
		[Test]
		public void class_3()
		{
			RunCompilerTestCase(@"class-3.boo");
		}
		
		[Test]
		public void closures_1()
		{
			RunCompilerTestCase(@"closures-1.boo");
		}
		
		[Test]
		public void closures_10()
		{
			RunCompilerTestCase(@"closures-10.boo");
		}
		
		[Test]
		public void closures_11()
		{
			RunCompilerTestCase(@"closures-11.boo");
		}
		
		[Test]
		public void closures_12()
		{
			RunCompilerTestCase(@"closures-12.boo");
		}
		
		[Test]
		public void closures_13()
		{
			RunCompilerTestCase(@"closures-13.boo");
		}
		
		[Test]
		public void closures_14()
		{
			RunCompilerTestCase(@"closures-14.boo");
		}
		
		[Test]
		public void closures_15()
		{
			RunCompilerTestCase(@"closures-15.boo");
		}
		
		[Test]
		public void closures_16()
		{
			RunCompilerTestCase(@"closures-16.boo");
		}
		
		[Test]
		public void closures_17()
		{
			RunCompilerTestCase(@"closures-17.boo");
		}
		
		[Test]
		public void closures_18()
		{
			RunCompilerTestCase(@"closures-18.boo");
		}
		
		[Test]
		public void closures_19()
		{
			RunCompilerTestCase(@"closures-19.boo");
		}
		
		[Test]
		public void closures_2()
		{
			RunCompilerTestCase(@"closures-2.boo");
		}
		
		[Test]
		public void closures_20()
		{
			RunCompilerTestCase(@"closures-20.boo");
		}
		
		[Test]
		public void closures_21()
		{
			RunCompilerTestCase(@"closures-21.boo");
		}
		
		[Test]
		public void closures_22()
		{
			RunCompilerTestCase(@"closures-22.boo");
		}
		
		[Test]
		public void closures_3()
		{
			RunCompilerTestCase(@"closures-3.boo");
		}
		
		[Test]
		public void closures_4()
		{
			RunCompilerTestCase(@"closures-4.boo");
		}
		
		[Test]
		public void closures_5()
		{
			RunCompilerTestCase(@"closures-5.boo");
		}
		
		[Test]
		public void closures_6()
		{
			RunCompilerTestCase(@"closures-6.boo");
		}
		
		[Test]
		public void closures_7()
		{
			RunCompilerTestCase(@"closures-7.boo");
		}
		
		[Test]
		public void closures_8()
		{
			RunCompilerTestCase(@"closures-8.boo");
		}
		
		[Test]
		public void closures_9()
		{
			RunCompilerTestCase(@"closures-9.boo");
		}
		
		[Test]
		public void comments_1()
		{
			RunCompilerTestCase(@"comments-1.boo");
		}
		
		[Test]
		public void comments_2()
		{
			RunCompilerTestCase(@"comments-2.boo");
		}
		
		[Test]
		public void comments_3()
		{
			RunCompilerTestCase(@"comments-3.boo");
		}
		
		[Test]
		public void comments_4()
		{
			RunCompilerTestCase(@"comments-4.boo");
		}
		
		[Test]
		public void conditional_1()
		{
			RunCompilerTestCase(@"conditional-1.boo");
		}
		
		[Test]
		public void declarations_1()
		{
			RunCompilerTestCase(@"declarations-1.boo");
		}
		
		[Test]
		public void declarations_2()
		{
			RunCompilerTestCase(@"declarations-2.boo");
		}
		
		[Test]
		public void declarations_3()
		{
			RunCompilerTestCase(@"declarations-3.boo");
		}
		
		[Test]
		public void double_literals_1()
		{
			RunCompilerTestCase(@"double-literals-1.boo");
		}
		
		[Test]
		public void dsl_1()
		{
			RunCompilerTestCase(@"dsl-1.boo");
		}
		
		[Test]
		public void elif_1()
		{
			RunCompilerTestCase(@"elif-1.boo");
		}
		
		[Test]
		public void elif_2()
		{
			RunCompilerTestCase(@"elif-2.boo");
		}
		
		[Test]
		public void enums_1()
		{
			RunCompilerTestCase(@"enums-1.boo");
		}
		
		[Test]
		public void events_1()
		{
			RunCompilerTestCase(@"events-1.boo");
		}
		
		[Test]
		public void explode_1()
		{
			RunCompilerTestCase(@"explode-1.boo");
		}
		
		[Test]
		public void explode_2()
		{
			RunCompilerTestCase(@"explode-2.boo");
		}
		
		[Test]
		public void expressions_1()
		{
			RunCompilerTestCase(@"expressions-1.boo");
		}
		
		[Test]
		public void expressions_2()
		{
			RunCompilerTestCase(@"expressions-2.boo");
		}
		
		[Test]
		public void expressions_3()
		{
			RunCompilerTestCase(@"expressions-3.boo");
		}
		
		[Test]
		public void expressions_4()
		{
			RunCompilerTestCase(@"expressions-4.boo");
		}
		
		[Test]
		public void expressions_5()
		{
			RunCompilerTestCase(@"expressions-5.boo");
		}
		
		[Test]
		public void extensions_1()
		{
			RunCompilerTestCase(@"extensions-1.boo");
		}
		
		[Test]
		public void fields_1()
		{
			RunCompilerTestCase(@"fields-1.boo");
		}
		
		[Test]
		public void fields_2()
		{
			RunCompilerTestCase(@"fields-2.boo");
		}
		
		[Test]
		public void fields_3()
		{
			RunCompilerTestCase(@"fields-3.boo");
		}
		
		[Test]
		public void fields_4()
		{
			RunCompilerTestCase(@"fields-4.boo");
		}
		
		[Test]
		public void fields_5()
		{
			RunCompilerTestCase(@"fields-5.boo");
		}
		
		[Test]
		public void fields_6()
		{
			RunCompilerTestCase(@"fields-6.boo");
		}
		
		[Test]
		public void for_or_1()
		{
			RunCompilerTestCase(@"for_or-1.boo");
		}
		
		[Test]
		public void for_or_then_1()
		{
			RunCompilerTestCase(@"for_or_then-1.boo");
		}
		
		[Test]
		public void for_then_1()
		{
			RunCompilerTestCase(@"for_then-1.boo");
		}
		
		[Test]
		public void generators_1()
		{
			RunCompilerTestCase(@"generators-1.boo");
		}
		
		[Test]
		public void generators_2()
		{
			RunCompilerTestCase(@"generators-2.boo");
		}
		
		[Test]
		public void generators_3()
		{
			RunCompilerTestCase(@"generators-3.boo");
		}
		
		[Test]
		public void generic_method_1()
		{
			RunCompilerTestCase(@"generic-method-1.boo");
		}
		
		[Test]
		public void generic_method_2()
		{
			RunCompilerTestCase(@"generic-method-2.boo");
		}
		
		[Test]
		public void generic_method_3()
		{
			RunCompilerTestCase(@"generic-method-3.boo");
		}
		
		[Test]
		public void generic_parameter_constraints()
		{
			RunCompilerTestCase(@"generic-parameter-constraints.boo");
		}
		
		[Test]
		public void generics_1()
		{
			RunCompilerTestCase(@"generics-1.boo");
		}
		
		[Test]
		public void generics_2()
		{
			RunCompilerTestCase(@"generics-2.boo");
		}
		
		[Test]
		public void generics_3()
		{
			RunCompilerTestCase(@"generics-3.boo");
		}
		
		[Test]
		public void generics_4()
		{
			RunCompilerTestCase(@"generics-4.boo");
		}
		
		[Test]
		public void generics_5()
		{
			RunCompilerTestCase(@"generics-5.boo");
		}
		
		[Test]
		public void getset_1()
		{
			RunCompilerTestCase(@"getset-1.boo");
		}
		
		[Test]
		public void goto_1()
		{
			RunCompilerTestCase(@"goto-1.boo");
		}
		
		[Test]
		public void goto_2()
		{
			RunCompilerTestCase(@"goto-2.boo");
		}
		
		[Test]
		public void hash_1()
		{
			RunCompilerTestCase(@"hash-1.boo");
		}
		
		[Test]
		public void iif_1()
		{
			RunCompilerTestCase(@"iif-1.boo");
		}
		
		[Test]
		public void import_1()
		{
			RunCompilerTestCase(@"import-1.boo");
		}
		
		[Test]
		public void import_2()
		{
			RunCompilerTestCase(@"import-2.boo");
		}
		
		[Test]
		public void in_not_in_1()
		{
			RunCompilerTestCase(@"in-not-in-1.boo");
		}
		
		[Test]
		public void in_not_in_2()
		{
			RunCompilerTestCase(@"in-not-in-2.boo");
		}
		
		[Test]
		public void in_not_in_3()
		{
			RunCompilerTestCase(@"in-not-in-3.boo");
		}
		
		[Test]
		public void inplace_1()
		{
			RunCompilerTestCase(@"inplace-1.boo");
		}
		
		[Test]
		public void internal_generic_callable_type_1()
		{
			RunCompilerTestCase(@"internal-generic-callable-type-1.boo");
		}
		
		[Test]
		public void internal_generic_type_1()
		{
			RunCompilerTestCase(@"internal-generic-type-1.boo");
		}
		
		[Test]
		public void internal_generic_type_2()
		{
			RunCompilerTestCase(@"internal-generic-type-2.boo");
		}
		
		[Test]
		public void internal_generic_type_3()
		{
			RunCompilerTestCase(@"internal-generic-type-3.boo");
		}
		
		[Test]
		public void internal_generic_type_4()
		{
			RunCompilerTestCase(@"internal-generic-type-4.boo");
		}
		
		[Test]
		public void internal_generic_type_5()
		{
			RunCompilerTestCase(@"internal-generic-type-5.boo");
		}
		
		[Test]
		public void internal_generic_type_6()
		{
			RunCompilerTestCase(@"internal-generic-type-6.boo");
		}
		
		[Test]
		public void interpolation_1()
		{
			RunCompilerTestCase(@"interpolation-1.boo");
		}
		
		[Test]
		public void interpolation_2()
		{
			RunCompilerTestCase(@"interpolation-2.boo");
		}
		
		[Test]
		public void interpolation_3()
		{
			RunCompilerTestCase(@"interpolation-3.boo");
		}
		
		[Test]
		public void invocation_1()
		{
			RunCompilerTestCase(@"invocation-1.boo");
		}
		
		[Test]
		public void isa_1()
		{
			RunCompilerTestCase(@"isa-1.boo");
		}
		
		[Test]
		public void keywords_as_members_1()
		{
			RunCompilerTestCase(@"keywords-as-members-1.boo");
		}
		
		[Test]
		public void line_continuation_1()
		{
			RunCompilerTestCase(@"line-continuation-1.boo");
		}
		
		[Test]
		public void list_1()
		{
			RunCompilerTestCase(@"list-1.boo");
		}
		
		[Test]
		public void long_literals_1()
		{
			RunCompilerTestCase(@"long-literals-1.boo");
		}
		
		[Test]
		public void macro_doc()
		{
			RunCompilerTestCase(@"macro-doc.boo");
		}
		
		[Test]
		public void macros_1()
		{
			RunCompilerTestCase(@"macros-1.boo");
		}
		
		[Test]
		public void macros_2()
		{
			RunCompilerTestCase(@"macros-2.boo");
		}
		
		[Test]
		public void macros_3()
		{
			RunCompilerTestCase(@"macros-3.boo");
		}
		
		[Test]
		public void macros_anywhere_1()
		{
			RunCompilerTestCase(@"macros-anywhere-1.boo");
		}
		
		[Test]
		public void module_1()
		{
			RunCompilerTestCase(@"module-1.boo");
		}
		
		[Test]
		public void module_2()
		{
			RunCompilerTestCase(@"module-2.boo");
		}
		
		[Test]
		public void module_3()
		{
			RunCompilerTestCase(@"module-3.boo");
		}
		
		[Test]
		public void named_arguments_1()
		{
			RunCompilerTestCase(@"named-arguments-1.boo");
		}
		
		[Test]
		public void named_arguments_2()
		{
			RunCompilerTestCase(@"named-arguments-2.boo");
		}
		
		[Test]
		public void not_1()
		{
			RunCompilerTestCase(@"not-1.boo");
		}
		
		[Test]
		public void not_2()
		{
			RunCompilerTestCase(@"not-2.boo");
		}
		
		[Test]
		public void null_1()
		{
			RunCompilerTestCase(@"null-1.boo");
		}
		
		[Test]
		public void omitted_member_target_1()
		{
			RunCompilerTestCase(@"omitted-member-target-1.boo");
		}
		
		[Test]
		public void ones_complement_1()
		{
			RunCompilerTestCase(@"ones-complement-1.boo");
		}
		
		[Test]
		public void regex_literals_1()
		{
			RunCompilerTestCase(@"regex-literals-1.boo");
		}
		
		[Test]
		public void regex_literals_2()
		{
			RunCompilerTestCase(@"regex-literals-2.boo");
		}
		
		[Test]
		public void return_1()
		{
			RunCompilerTestCase(@"return-1.boo");
		}
		
		[Test]
		public void return_2()
		{
			RunCompilerTestCase(@"return-2.boo");
		}
		
		[Test]
		public void self_1()
		{
			RunCompilerTestCase(@"self-1.boo");
		}
		
		[Test]
		public void semicolons_1()
		{
			RunCompilerTestCase(@"semicolons-1.boo");
		}
		
		[Test]
		public void slicing_1()
		{
			RunCompilerTestCase(@"slicing-1.boo");
		}
		
		[Test]
		public void slicing_2()
		{
			RunCompilerTestCase(@"slicing-2.boo");
		}
		
		[Test]
		public void splicing_1()
		{
			RunCompilerTestCase(@"splicing-1.boo");
		}
		
		[Test]
		public void string_literals_1()
		{
			RunCompilerTestCase(@"string-literals-1.boo");
		}
		
		[Test]
		public void struct_1()
		{
			RunCompilerTestCase(@"struct-1.boo");
		}
		
		[Test]
		public void timespan_literals_1()
		{
			RunCompilerTestCase(@"timespan-literals-1.boo");
		}
		
		[Test]
		public void try_1()
		{
			RunCompilerTestCase(@"try-1.boo");
		}
		
		[Test]
		public void try_2()
		{
			RunCompilerTestCase(@"try-2.boo");
		}
		
		[Test]
		public void unless_1()
		{
			RunCompilerTestCase(@"unless-1.boo");
		}
		
		[Test]
		public void varargs_1()
		{
			RunCompilerTestCase(@"varargs-1.boo");
		}
		
		[Test]
		public void while_or_1()
		{
			RunCompilerTestCase(@"while_or-1.boo");
		}
		
		[Test]
		public void while_or_then_1()
		{
			RunCompilerTestCase(@"while_or_then-1.boo");
		}
		
		[Test]
		public void while_then_1()
		{
			RunCompilerTestCase(@"while_then-1.boo");
		}
		
		[Test]
		public void xor_1()
		{
			RunCompilerTestCase(@"xor-1.boo");
		}
		
		[Test]
		public void yield_1()
		{
			RunCompilerTestCase(@"yield-1.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "parser/roundtrip";
		}
	}
}
