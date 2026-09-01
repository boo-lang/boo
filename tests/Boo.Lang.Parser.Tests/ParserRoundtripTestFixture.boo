namespace Boo.Lang.Parser.Tests

import NUnit.Framework

[TestFixture]
class ParserRoundtripTestFixture(AbstractParserTestFixture):
	def RunCompilerTestCase(fname as string):
		RunParserTestCase(fname)

	[Test]
	def and_or_1():
		RunCompilerTestCase("and-or-1.boo")

	[Test]
	def arrays_1():
		RunCompilerTestCase("arrays-1.boo")

	[Test]
	def arrays_2():
		RunCompilerTestCase("arrays-2.boo")

	[Test]
	def arrays_3():
		RunCompilerTestCase("arrays-3.boo")

	[Test]
	def arrays_4():
		RunCompilerTestCase("arrays-4.boo")

	[Test]
	def arrays_5():
		RunCompilerTestCase("arrays-5.boo")

	[Test]
	def arrays_6():
		RunCompilerTestCase("arrays-6.boo")

	[Test]
	def as_1():
		RunCompilerTestCase("as-1.boo")

	[Test]
	def assignment_1():
		RunCompilerTestCase("assignment-1.boo")

	[Test]
	def ast_literal_enum():
		RunCompilerTestCase("ast-literal-enum.boo")

	[Test]
	def ast_literal_varargs_method():
		RunCompilerTestCase("ast-literal-varargs-method.boo")

	[Test]
	def ast_literals_1():
		RunCompilerTestCase("ast-literals-1.boo")

	[Test]
	def ast_literals_10():
		RunCompilerTestCase("ast-literals-10.boo")

	[Test]
	def ast_literals_11():
		RunCompilerTestCase("ast-literals-11.boo")

	[Test]
	def ast_literals_2():
		RunCompilerTestCase("ast-literals-2.boo")

	[Test]
	def ast_literals_3():
		RunCompilerTestCase("ast-literals-3.boo")

	[Test]
	def ast_literals_4():
		RunCompilerTestCase("ast-literals-4.boo")

	[Test]
	def ast_literals_5():
		RunCompilerTestCase("ast-literals-5.boo")

	[Test]
	def ast_literals_6():
		RunCompilerTestCase("ast-literals-6.boo")

	[Test]
	def ast_literals_7():
		RunCompilerTestCase("ast-literals-7.boo")

	[Test]
	def ast_literals_8():
		RunCompilerTestCase("ast-literals-8.boo")

	[Test]
	def ast_literals_9():
		RunCompilerTestCase("ast-literals-9.boo")

	[Test]
	def ast_literals_if_it_looks_like_a_block_1():
		RunCompilerTestCase("ast-literals-if-it-looks-like-a-block-1.boo")

	[Test]
	def at_operator():
		RunCompilerTestCase("at-operator.boo")

	[Test]
	def attributes_1():
		RunCompilerTestCase("attributes-1.boo")

	[Test]
	def attributes_2():
		RunCompilerTestCase("attributes-2.boo")

	[Test]
	def bool_literals_1():
		RunCompilerTestCase("bool-literals-1.boo")

	[Test]
	def callables_1():
		RunCompilerTestCase("callables-1.boo")

	[Test]
	def callables_2():
		RunCompilerTestCase("callables-2.boo")

	[Test]
	def callables_with_varags():
		RunCompilerTestCase("callables-with-varags.boo")

	[Test]
	def cast_1():
		RunCompilerTestCase("cast-1.boo")

	[Test]
	def char_1():
		RunCompilerTestCase("char-1.boo")

	[Test]
	def char_2():
		RunCompilerTestCase("char-2.boo")

	[Test]
	def class_1():
		RunCompilerTestCase("class-1.boo")

	[Test]
	def class_2():
		RunCompilerTestCase("class-2.boo")

	[Test]
	def class_3():
		RunCompilerTestCase("class-3.boo")

	[Test]
	def closures_1():
		RunCompilerTestCase("closures-1.boo")

	[Test]
	def closures_10():
		RunCompilerTestCase("closures-10.boo")

	[Test]
	def closures_11():
		RunCompilerTestCase("closures-11.boo")

	[Test]
	def closures_12():
		RunCompilerTestCase("closures-12.boo")

	[Test]
	def closures_13():
		RunCompilerTestCase("closures-13.boo")

	[Test]
	def closures_14():
		RunCompilerTestCase("closures-14.boo")

	[Test]
	def closures_15():
		RunCompilerTestCase("closures-15.boo")

	[Test]
	def closures_16():
		RunCompilerTestCase("closures-16.boo")

	[Test]
	def closures_17():
		RunCompilerTestCase("closures-17.boo")

	[Test]
	def closures_18():
		RunCompilerTestCase("closures-18.boo")

	[Test]
	def closures_19():
		RunCompilerTestCase("closures-19.boo")

	[Test]
	def closures_2():
		RunCompilerTestCase("closures-2.boo")

	[Test]
	def closures_20():
		RunCompilerTestCase("closures-20.boo")

	[Test]
	def closures_21():
		RunCompilerTestCase("closures-21.boo")

	[Test]
	def closures_22():
		RunCompilerTestCase("closures-22.boo")

	[Test]
	def closures_3():
		RunCompilerTestCase("closures-3.boo")

	[Test]
	def closures_4():
		RunCompilerTestCase("closures-4.boo")

	[Test]
	def closures_5():
		RunCompilerTestCase("closures-5.boo")

	[Test]
	def closures_6():
		RunCompilerTestCase("closures-6.boo")

	[Test]
	def closures_7():
		RunCompilerTestCase("closures-7.boo")

	[Test]
	def closures_8():
		RunCompilerTestCase("closures-8.boo")

	[Test]
	def closures_9():
		RunCompilerTestCase("closures-9.boo")

	[Test]
	def collection_initializer():
		RunCompilerTestCase("collection-initializer.boo")

	[Test]
	def comments_1():
		RunCompilerTestCase("comments-1.boo")

	[Test]
	def comments_2():
		RunCompilerTestCase("comments-2.boo")

	[Test]
	def comments_3():
		RunCompilerTestCase("comments-3.boo")

	[Test]
	def comments_4():
		RunCompilerTestCase("comments-4.boo")

	[Test]
	def conditional_1():
		RunCompilerTestCase("conditional-1.boo")

	[Test]
	def declarations_1():
		RunCompilerTestCase("declarations-1.boo")

	[Test]
	def declarations_2():
		RunCompilerTestCase("declarations-2.boo")

	[Test]
	def declarations_3():
		RunCompilerTestCase("declarations-3.boo")

	[Test]
	def double_literals_1():
		RunCompilerTestCase("double-literals-1.boo")

	[Test]
	def dsl_1():
		RunCompilerTestCase("dsl-1.boo")

	[Test]
	def elif_1():
		RunCompilerTestCase("elif-1.boo")

	[Test]
	def elif_2():
		RunCompilerTestCase("elif-2.boo")

	[Test]
	def enumerable_type_shortcut():
		RunCompilerTestCase("enumerable-type-shortcut.boo")

	[Test]
	def enums_1():
		RunCompilerTestCase("enums-1.boo")

	[Test]
	def events_1():
		RunCompilerTestCase("events-1.boo")

	[Test]
	def explode_1():
		RunCompilerTestCase("explode-1.boo")

	[Test]
	def explode_2():
		RunCompilerTestCase("explode-2.boo")

	[Test]
	def expressions_1():
		RunCompilerTestCase("expressions-1.boo")

	[Test]
	def expressions_2():
		RunCompilerTestCase("expressions-2.boo")

	[Test]
	def expressions_3():
		RunCompilerTestCase("expressions-3.boo")

	[Test]
	def expressions_4():
		RunCompilerTestCase("expressions-4.boo")

	[Test]
	def expressions_5():
		RunCompilerTestCase("expressions-5.boo")

	[Test]
	def extensions_1():
		RunCompilerTestCase("extensions-1.boo")

	[Test]
	def fields_1():
		RunCompilerTestCase("fields-1.boo")

	[Test]
	def fields_2():
		RunCompilerTestCase("fields-2.boo")

	[Test]
	def fields_3():
		RunCompilerTestCase("fields-3.boo")

	[Test]
	def fields_4():
		RunCompilerTestCase("fields-4.boo")

	[Test]
	def fields_5():
		RunCompilerTestCase("fields-5.boo")

	[Test]
	def fields_6():
		RunCompilerTestCase("fields-6.boo")

	[Test]
	def for_or_1():
		RunCompilerTestCase("for_or-1.boo")

	[Test]
	def for_or_then_1():
		RunCompilerTestCase("for_or_then-1.boo")

	[Test]
	def for_then_1():
		RunCompilerTestCase("for_then-1.boo")

	[Test]
	def generators_1():
		RunCompilerTestCase("generators-1.boo")

	[Test]
	def generators_2():
		RunCompilerTestCase("generators-2.boo")

	[Test]
	def generators_3():
		RunCompilerTestCase("generators-3.boo")

	[Test]
	def generic_method_1():
		RunCompilerTestCase("generic-method-1.boo")

	[Test]
	def generic_method_2():
		RunCompilerTestCase("generic-method-2.boo")

	[Test]
	def generic_method_3():
		RunCompilerTestCase("generic-method-3.boo")

	[Test]
	def generic_parameter_constraints():
		RunCompilerTestCase("generic-parameter-constraints.boo")

	[Test]
	def generics_1():
		RunCompilerTestCase("generics-1.boo")

	[Test]
	def generics_2():
		RunCompilerTestCase("generics-2.boo")

	[Test]
	def generics_3():
		RunCompilerTestCase("generics-3.boo")

	[Test]
	def generics_4():
		RunCompilerTestCase("generics-4.boo")

	[Test]
	def generics_5():
		RunCompilerTestCase("generics-5.boo")

	[Test]
	def getset_1():
		RunCompilerTestCase("getset-1.boo")

	[Test]
	def goto_1():
		RunCompilerTestCase("goto-1.boo")

	[Test]
	def goto_2():
		RunCompilerTestCase("goto-2.boo")

	[Test]
	def hash_1():
		RunCompilerTestCase("hash-1.boo")

	[Test]
	def hash_initializer():
		RunCompilerTestCase("hash-initializer.boo")

	[Test]
	def iif_1():
		RunCompilerTestCase("iif-1.boo")

	[Test]
	def import_1():
		RunCompilerTestCase("import-1.boo")

	[Test]
	def import_2():
		RunCompilerTestCase("import-2.boo")

	[Test]
	def in_not_in_1():
		RunCompilerTestCase("in-not-in-1.boo")

	[Test]
	def in_not_in_2():
		RunCompilerTestCase("in-not-in-2.boo")

	[Test]
	def in_not_in_3():
		RunCompilerTestCase("in-not-in-3.boo")

	[Test]
	def inplace_1():
		RunCompilerTestCase("inplace-1.boo")

	[Test]
	def internal_generic_callable_type_1():
		RunCompilerTestCase("internal-generic-callable-type-1.boo")

	[Test]
	def internal_generic_type_1():
		RunCompilerTestCase("internal-generic-type-1.boo")

	[Test]
	def internal_generic_type_2():
		RunCompilerTestCase("internal-generic-type-2.boo")

	[Test]
	def internal_generic_type_3():
		RunCompilerTestCase("internal-generic-type-3.boo")

	[Test]
	def internal_generic_type_4():
		RunCompilerTestCase("internal-generic-type-4.boo")

	[Test]
	def internal_generic_type_5():
		RunCompilerTestCase("internal-generic-type-5.boo")

	[Test]
	def internal_generic_type_6():
		RunCompilerTestCase("internal-generic-type-6.boo")

	[Test]
	def interpolation_1():
		RunCompilerTestCase("interpolation-1.boo")

	[Test]
	def interpolation_2():
		RunCompilerTestCase("interpolation-2.boo")

	[Test]
	def interpolation_3():
		RunCompilerTestCase("interpolation-3.boo")

	[Test]
	def interpolation_4():
		RunCompilerTestCase("interpolation-4.boo")

	[Test]
	def invocation_1():
		RunCompilerTestCase("invocation-1.boo")

	[Test]
	def isa_1():
		RunCompilerTestCase("isa-1.boo")

	[Test]
	def keywords_as_members_1():
		RunCompilerTestCase("keywords-as-members-1.boo")

	[Test]
	def line_continuation_1():
		RunCompilerTestCase("line-continuation-1.boo")

	[Test]
	def list_1():
		RunCompilerTestCase("list-1.boo")

	[Test]
	def long_literals_1():
		RunCompilerTestCase("long-literals-1.boo")

	[Test]
	def macro_doc():
		RunCompilerTestCase("macro-doc.boo")

	[Test]
	def macros_1():
		RunCompilerTestCase("macros-1.boo")

	[Test]
	def macros_2():
		RunCompilerTestCase("macros-2.boo")

	[Test]
	def macros_3():
		RunCompilerTestCase("macros-3.boo")

	[Test]
	def macros_anywhere_1():
		RunCompilerTestCase("macros-anywhere-1.boo")

	[Test]
	def method_declaration_in_macro_application():
		RunCompilerTestCase("method-declaration-in-macro-application.boo")

	[Test]
	def method_declarations_in_nested_macro_application():
		RunCompilerTestCase("method-declarations-in-nested-macro-application.boo")

	[Test]
	def module_1():
		RunCompilerTestCase("module-1.boo")

	[Test]
	def module_2():
		RunCompilerTestCase("module-2.boo")

	[Test]
	def module_3():
		RunCompilerTestCase("module-3.boo")

	[Test]
	def named_arguments_1():
		RunCompilerTestCase("named-arguments-1.boo")

	[Test]
	def named_arguments_2():
		RunCompilerTestCase("named-arguments-2.boo")

	[Test]
	def new_1():
		RunCompilerTestCase("new-1.boo")

	[Test]
	def not_1():
		RunCompilerTestCase("not-1.boo")

	[Test]
	def not_2():
		RunCompilerTestCase("not-2.boo")

	[Test]
	def null_1():
		RunCompilerTestCase("null-1.boo")

	[Test]
	def omitted_member_target_1():
		RunCompilerTestCase("omitted-member-target-1.boo")

	[Test]
	def ones_complement_1():
		RunCompilerTestCase("ones-complement-1.boo")

	[Test]
	def pass_among_statements_1():
		RunCompilerTestCase("pass-among-statements-1.boo")

	[Test]
	def pass_single_line_1():
		RunCompilerTestCase("pass-single-line-1.boo")

	[Test]
	def regex_literals_1():
		RunCompilerTestCase("regex-literals-1.boo")

	[Test]
	def regex_literals_2():
		RunCompilerTestCase("regex-literals-2.boo")

	[Test]
	def return_1():
		RunCompilerTestCase("return-1.boo")

	[Test]
	def return_2():
		RunCompilerTestCase("return-2.boo")

	[Test]
	def self_1():
		RunCompilerTestCase("self-1.boo")

	[Test]
	def semicolons_1():
		RunCompilerTestCase("semicolons-1.boo")

	[Test]
	def slicing_1():
		RunCompilerTestCase("slicing-1.boo")

	[Test]
	def slicing_2():
		RunCompilerTestCase("slicing-2.boo")

	[Test]
	def splicing_1():
		RunCompilerTestCase("splicing-1.boo")

	[Test]
	def splicing_class_body():
		RunCompilerTestCase("splicing-class-body.boo")

	[Test]
	def splicing_enum_body():
		RunCompilerTestCase("splicing-enum-body.boo")

	[Test]
	def string_literals_1():
		RunCompilerTestCase("string-literals-1.boo")

	[Test]
	def struct_1():
		RunCompilerTestCase("struct-1.boo")

	[Test]
	def timespan_literals_1():
		RunCompilerTestCase("timespan-literals-1.boo")

	[Test]
	def try_1():
		RunCompilerTestCase("try-1.boo")

	[Test]
	def try_2():
		RunCompilerTestCase("try-2.boo")

	[Test]
	def type_references_1():
		RunCompilerTestCase("type-references-1.boo")

	[Test]
	def unless_1():
		RunCompilerTestCase("unless-1.boo")

	[Test]
	def varargs_1():
		RunCompilerTestCase("varargs-1.boo")

	[Test]
	def while_or_1():
		RunCompilerTestCase("while_or-1.boo")

	[Test]
	def while_or_then_1():
		RunCompilerTestCase("while_or_then-1.boo")

	[Test]
	def while_then_1():
		RunCompilerTestCase("while_then-1.boo")

	[Test]
	def xor_1():
		RunCompilerTestCase("xor-1.boo")

	[Test]
	def yield_as_member_name():
		RunCompilerTestCase("yield-as-member-name.boo")

	[Test]
	def yield_1():
		RunCompilerTestCase("yield-1.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "parser/roundtrip"
