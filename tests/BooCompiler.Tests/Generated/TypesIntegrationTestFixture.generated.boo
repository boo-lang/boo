namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class TypesIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @abstract_1():
		RunCompilerTestCase("abstract-1.boo")

	[Test]
	def @abstract_2():
		RunCompilerTestCase("abstract-2.boo")

	[Test]
	def @abstract_3():
		RunCompilerTestCase("abstract-3.boo")

	[Test]
	def @abstract_4():
		RunCompilerTestCase("abstract-4.boo")

	[Test]
	def @automatic_stubs_for_interface_methods():
		RunCompilerTestCase("automatic-stubs-for-interface-methods.boo")

	[Test]
	def @baseclass_1():
		RunCompilerTestCase("baseclass-1.boo")

	[Test]
	def @baseclass_2():
		RunCompilerTestCase("baseclass-2.boo")

	[Test]
	def @baseclass_3():
		RunCompilerTestCase("baseclass-3.boo")

	[Test]
	def @baseclass_4():
		RunCompilerTestCase("baseclass-4.boo")

	[Test]
	def @baseclass_5():
		RunCompilerTestCase("baseclass-5.boo")

	[Test]
	def @classes_1():
		RunCompilerTestCase("classes-1.boo")

	[Test]
	def @classes_10():
		RunCompilerTestCase("classes-10.boo")

	[Test]
	def @classes_11():
		RunCompilerTestCase("classes-11.boo")

	[Test]
	def @classes_2():
		RunCompilerTestCase("classes-2.boo")

	[Test]
	def @classes_3():
		RunCompilerTestCase("classes-3.boo")

	[Test]
	def @classes_4():
		RunCompilerTestCase("classes-4.boo")

	[Test]
	def @classes_5():
		RunCompilerTestCase("classes-5.boo")

	[Test]
	def @classes_6():
		RunCompilerTestCase("classes-6.boo")

	[Test]
	def @classes_7():
		RunCompilerTestCase("classes-7.boo")

	[Test]
	def @classes_8():
		RunCompilerTestCase("classes-8.boo")

	[Test]
	def @classes_9():
		RunCompilerTestCase("classes-9.boo")

	[Test]
	def @constructor_return_1():
		RunCompilerTestCase("constructor-return-1.boo")

	[Test]
	def @constructors_1():
		RunCompilerTestCase("constructors-1.boo")

	[Test]
	def @enum_cast_to_single():
		RunCompilerTestCase("enum-cast-to-single.boo")

	[Test]
	def @enums_1():
		RunCompilerTestCase("enums-1.boo")

	[Test]
	def @enums_10():
		RunCompilerTestCase("enums-10.boo")

	[Test]
	def @enums_11():
		RunCompilerTestCase("enums-11.boo")

	[Test]
	def @enums_12():
		RunCompilerTestCase("enums-12.boo")

	[Test]
	def @enums_13():
		RunCompilerTestCase("enums-13.boo")

	[Test]
	def @enums_14():
		RunCompilerTestCase("enums-14.boo")

	[Test]
	def @enums_15():
		RunCompilerTestCase("enums-15.boo")

	[Test]
	def @enums_16():
		RunCompilerTestCase("enums-16.boo")

	[Test]
	def @enums_17():
		RunCompilerTestCase("enums-17.boo")

	[Test]
	def @enums_2():
		RunCompilerTestCase("enums-2.boo")

	[Test]
	def @enums_3():
		RunCompilerTestCase("enums-3.boo")

	[Test]
	def @enums_4():
		RunCompilerTestCase("enums-4.boo")

	[Test]
	def @enums_5():
		RunCompilerTestCase("enums-5.boo")

	[Test]
	def @enums_6():
		RunCompilerTestCase("enums-6.boo")

	[Test]
	def @enums_7():
		RunCompilerTestCase("enums-7.boo")

	[Test]
	def @enums_8():
		RunCompilerTestCase("enums-8.boo")

	[Test]
	def @enums_9():
		RunCompilerTestCase("enums-9.boo")

	[Test]
	def @events_1():
		RunCompilerTestCase("events-1.boo")

	[Test]
	def @events_2():
		RunCompilerTestCase("events-2.boo")

	[Test]
	def @events_3():
		RunCompilerTestCase("events-3.boo")

	[Test]
	def @events_4():
		RunCompilerTestCase("events-4.boo")

	[Test]
	def @events_5():
		RunCompilerTestCase("events-5.boo")

	[Test]
	def @events_6():
		RunCompilerTestCase("events-6.boo")

	[Test]
	def @events_7():
		RunCompilerTestCase("events-7.boo")

	[Test]
	def @explicit_interface_1():
		RunCompilerTestCase("explicit-interface-1.boo")

	[Test]
	def @explicit_interface_2():
		RunCompilerTestCase("explicit-interface-2.boo")

	[Test]
	def @explicit_interface_3():
		RunCompilerTestCase("explicit-interface-3.boo")

	[Test]
	def @explicit_interface_4():
		RunCompilerTestCase("explicit-interface-4.boo")

	[Test]
	def @explicit_interface_5():
		RunCompilerTestCase("explicit-interface-5.boo")

	[Test]
	def @explicit_interface_6():
		RunCompilerTestCase("explicit-interface-6.boo")

	[Test]
	def @explicit_value_type_conversion_operator():
		RunCompilerTestCase("explicit-value-type-conversion-operator.boo")

	[Test]
	def @fields_1():
		RunCompilerTestCase("fields-1.boo")

	[Test]
	def @fields_10():
		RunCompilerTestCase("fields-10.boo")

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
	def @fields_9():
		RunCompilerTestCase("fields-9.boo")

	[Test]
	def @implicit_1():
		RunCompilerTestCase("implicit-1.boo")

	[Test]
	def @implicit_2():
		RunCompilerTestCase("implicit-2.boo")

	[Test]
	def @implicit_3():
		RunCompilerTestCase("implicit-3.boo")

	[Test]
	def @implicit_4():
		RunCompilerTestCase("implicit-4.boo")

	[Test]
	def @implicit_5():
		RunCompilerTestCase("implicit-5.boo")

	[Test]
	def @implicit_bool_1():
		RunCompilerTestCase("implicit-bool-1.boo")

	[Test]
	def @implicit_bool_2():
		RunCompilerTestCase("implicit-bool-2.boo")

	[Test]
	def @implicit_bool_3():
		RunCompilerTestCase("implicit-bool-3.boo")

	[Test]
	def @implicit_bool_4():
		RunCompilerTestCase("implicit-bool-4.boo")

	[Test]
	def @implicit_bool_5():
		RunCompilerTestCase("implicit-bool-5.boo")

	[Test]
	def @implicit_bool_6():
		RunCompilerTestCase("implicit-bool-6.boo")

	[Test]
	def @implicit_bool_7():
		RunCompilerTestCase("implicit-bool-7.boo")

	[Test]
	def @implicit_bool_8():
		RunCompilerTestCase("implicit-bool-8.boo")

	[Test]
	def @implicit_bool_with_shortcircuited_and_condition():
		RunCompilerTestCase("implicit-bool-with-shortcircuited-and-condition.boo")

	[Test]
	def @implicit_runtime_conversion():
		RunCompilerTestCase("implicit-runtime-conversion.boo")

	[Test]
	def @innerclasses_1():
		RunCompilerTestCase("innerclasses-1.boo")

	[Test]
	def @innerclasses_10():
		RunCompilerTestCase("innerclasses-10.boo")

	[Test]
	def @innerclasses_11():
		RunCompilerTestCase("innerclasses-11.boo")

	[Test]
	def @innerclasses_12():
		RunCompilerTestCase("innerclasses-12.boo")

	[Test]
	def @innerclasses_2():
		RunCompilerTestCase("innerclasses-2.boo")

	[Test]
	def @innerclasses_3():
		RunCompilerTestCase("innerclasses-3.boo")

	[Test]
	def @innerclasses_4():
		RunCompilerTestCase("innerclasses-4.boo")

	[Test]
	def @innerclasses_5():
		RunCompilerTestCase("innerclasses-5.boo")

	[Test]
	def @innerclasses_6():
		RunCompilerTestCase("innerclasses-6.boo")

	[Test]
	def @innerclasses_7():
		RunCompilerTestCase("innerclasses-7.boo")

	[Test]
	def @innerclasses_8():
		RunCompilerTestCase("innerclasses-8.boo")

	[Test]
	def @innerclasses_9():
		RunCompilerTestCase("innerclasses-9.boo")

	[Test]
	def @interface_implementation_inheritance_1():
		RunCompilerTestCase("interface-implementation-inheritance-1.boo")

	[Test]
	def @interface_implementation_inheritance_2():
		RunCompilerTestCase("interface-implementation-inheritance-2.boo")

	[Test]
	def @interface_implementation_inheritance_3():
		RunCompilerTestCase("interface-implementation-inheritance-3.boo")

	[Test]
	def @interface_implementation_inheritance_4():
		RunCompilerTestCase("interface-implementation-inheritance-4.boo")

	[Category("FailsOnMono")][Test]
	def @interface_implementation_inheritance_5():
		RunCompilerTestCase("interface-implementation-inheritance-5.boo")

	[Test]
	def @interfaces_1():
		RunCompilerTestCase("interfaces-1.boo")

	[Test]
	def @interfaces_10():
		RunCompilerTestCase("interfaces-10.boo")

	[Test]
	def @interfaces_11():
		RunCompilerTestCase("interfaces-11.boo")

	[Test]
	def @interfaces_12():
		RunCompilerTestCase("interfaces-12.boo")

	[Test]
	def @interfaces_13():
		RunCompilerTestCase("interfaces-13.boo")

	[Test]
	def @interfaces_14():
		RunCompilerTestCase("interfaces-14.boo")

	[Test]
	def @interfaces_15():
		RunCompilerTestCase("interfaces-15.boo")

	[Test]
	def @interfaces_16():
		RunCompilerTestCase("interfaces-16.boo")

	[Test]
	def @interfaces_17():
		RunCompilerTestCase("interfaces-17.boo")

	[Test]
	def @interfaces_19():
		RunCompilerTestCase("interfaces-19.boo")

	[Test]
	def @interfaces_2():
		RunCompilerTestCase("interfaces-2.boo")

	[Test]
	def @interfaces_20():
		RunCompilerTestCase("interfaces-20.boo")

	[Test]
	def @interfaces_21():
		RunCompilerTestCase("interfaces-21.boo")

	[Test]
	def @interfaces_22():
		RunCompilerTestCase("interfaces-22.boo")

	[Test]
	def @interfaces_23():
		RunCompilerTestCase("interfaces-23.boo")

	[Test]
	def @interfaces_3():
		RunCompilerTestCase("interfaces-3.boo")

	[Test]
	def @interfaces_4():
		RunCompilerTestCase("interfaces-4.boo")

	[Test]
	def @interfaces_5():
		RunCompilerTestCase("interfaces-5.boo")

	[Test]
	def @interfaces_6():
		RunCompilerTestCase("interfaces-6.boo")

	[Test]
	def @interfaces_7():
		RunCompilerTestCase("interfaces-7.boo")

	[Test]
	def @interfaces_8():
		RunCompilerTestCase("interfaces-8.boo")

	[Test]
	def @interfaces_9():
		RunCompilerTestCase("interfaces-9.boo")

	[Test]
	def @internal_base_type_is_preferred():
		RunCompilerTestCase("internal-base-type-is-preferred.boo")

	[Test]
	def @internal_event_initializer():
		RunCompilerTestCase("internal-event-initializer.boo")

	[Test]
	def @internal_field_initializer():
		RunCompilerTestCase("internal-field-initializer.boo")

	[Test]
	def @internal_property_initializer():
		RunCompilerTestCase("internal-property-initializer.boo")

	[Test]
	def @local_hiding_field():
		RunCompilerTestCase("local-hiding-field.boo")

	[Test]
	def @method_shadowing_1():
		RunCompilerTestCase("method-shadowing-1.boo")

	[Test]
	def @method_shadowing_2():
		RunCompilerTestCase("method-shadowing-2.boo")

	[Test]
	def @method_shadowing_3():
		RunCompilerTestCase("method-shadowing-3.boo")

	[Test]
	def @method_shadowing_4():
		RunCompilerTestCase("method-shadowing-4.boo")

	[Test]
	def @methods_1():
		RunCompilerTestCase("methods-1.boo")

	[Test]
	def @methods_2():
		RunCompilerTestCase("methods-2.boo")

	[Test]
	def @methods_3():
		RunCompilerTestCase("methods-3.boo")

	[Test]
	def @methods_4():
		RunCompilerTestCase("methods-4.boo")

	[Test]
	def @methods_5():
		RunCompilerTestCase("methods-5.boo")

	[Test]
	def @methods_6():
		RunCompilerTestCase("methods-6.boo")

	[Test]
	def @methods_7():
		RunCompilerTestCase("methods-7.boo")

	[Test]
	def @name_lookup_1():
		RunCompilerTestCase("name-lookup-1.boo")

	[Test]
	def @name_lookup_2():
		RunCompilerTestCase("name-lookup-2.boo")

	[Test]
	def @negative_enum_value():
		RunCompilerTestCase("negative-enum-value.boo")

	[Test]
	def @overloading_1():
		RunCompilerTestCase("overloading-1.boo")

	[Test]
	def @overloading_2():
		RunCompilerTestCase("overloading-2.boo")

	[Test]
	def @overloading_3():
		RunCompilerTestCase("overloading-3.boo")

	[Test]
	def @overloading_4():
		RunCompilerTestCase("overloading-4.boo")

	[Test]
	def @overloading_5():
		RunCompilerTestCase("overloading-5.boo")

	[Test]
	def @overloading_6():
		RunCompilerTestCase("overloading-6.boo")

	[Test]
	def @overloading_7():
		RunCompilerTestCase("overloading-7.boo")

	[Test]
	def @overloading_8():
		RunCompilerTestCase("overloading-8.boo")

	[Test]
	def @override_1():
		RunCompilerTestCase("override-1.boo")

	[Test]
	def @override_2():
		RunCompilerTestCase("override-2.boo")

	[Test]
	def @override_3():
		RunCompilerTestCase("override-3.boo")

	[Test]
	def @override_4():
		RunCompilerTestCase("override-4.boo")

	[Test]
	def @override_5():
		RunCompilerTestCase("override-5.boo")

	[Test]
	def @partial_1():
		RunCompilerTestCase("partial-1.boo")

	[Test]
	def @partial_2():
		RunCompilerTestCase("partial-2.boo")

	[Test]
	def @partial_class_with_nested_types():
		RunCompilerTestCase("partial-class-with-nested-types.boo")

	[Test]
	def @partial_enums():
		RunCompilerTestCase("partial-enums.boo")

	[Test]
	def @partial_interfaces():
		RunCompilerTestCase("partial-interfaces.boo")

	[Test]
	def @properties_1():
		RunCompilerTestCase("properties-1.boo")

	[Test]
	def @properties_10():
		RunCompilerTestCase("properties-10.boo")

	[Test]
	def @properties_11():
		RunCompilerTestCase("properties-11.boo")

	[Test]
	def @properties_12():
		RunCompilerTestCase("properties-12.boo")

	[Test]
	def @properties_13():
		RunCompilerTestCase("properties-13.boo")

	[Test]
	def @properties_14():
		RunCompilerTestCase("properties-14.boo")

	[Test]
	def @properties_15():
		RunCompilerTestCase("properties-15.boo")

	[Test]
	def @properties_16():
		RunCompilerTestCase("properties-16.boo")

	[Test]
	def @properties_17():
		RunCompilerTestCase("properties-17.boo")

	[Test]
	def @properties_18():
		RunCompilerTestCase("properties-18.boo")

	[Test]
	def @properties_19():
		RunCompilerTestCase("properties-19.boo")

	[Test]
	def @properties_2():
		RunCompilerTestCase("properties-2.boo")

	[Test]
	def @properties_20():
		RunCompilerTestCase("properties-20.boo")

	[Test]
	def @properties_21():
		RunCompilerTestCase("properties-21.boo")

	[Test]
	def @properties_22():
		RunCompilerTestCase("properties-22.boo")

	[Test]
	def @properties_23():
		RunCompilerTestCase("properties-23.boo")

	[Test]
	def @properties_24():
		RunCompilerTestCase("properties-24.boo")

	[Test]
	def @properties_25():
		RunCompilerTestCase("properties-25.boo")

	[Test]
	def @properties_26():
		RunCompilerTestCase("properties-26.boo")

	[Test]
	def @properties_27():
		RunCompilerTestCase("properties-27.boo")

	[Test]
	def @properties_28():
		RunCompilerTestCase("properties-28.boo")

	[Test]
	def @properties_29():
		RunCompilerTestCase("properties-29.boo")

	[Test]
	def @properties_3():
		RunCompilerTestCase("properties-3.boo")

	[Test]
	def @properties_30():
		RunCompilerTestCase("properties-30.boo")

	[Test]
	def @properties_31():
		RunCompilerTestCase("properties-31.boo")

	[Test]
	def @properties_32():
		RunCompilerTestCase("properties-32.boo")

	[Test]
	def @properties_33():
		RunCompilerTestCase("properties-33.boo")

	[Test]
	def @properties_4():
		RunCompilerTestCase("properties-4.boo")

	[Test]
	def @properties_5():
		RunCompilerTestCase("properties-5.boo")

	[Test]
	def @properties_6():
		RunCompilerTestCase("properties-6.boo")

	[Test]
	def @properties_7():
		RunCompilerTestCase("properties-7.boo")

	[Test]
	def @properties_8():
		RunCompilerTestCase("properties-8.boo")

	[Test]
	def @properties_9():
		RunCompilerTestCase("properties-9.boo")

	[Test]
	def @refreturn_1():
		RunCompilerTestCase("refreturn-1.boo")

	[Test]
	def @reserved_keywords_1():
		RunCompilerTestCase("reserved-keywords-1.boo")

	[Test]
	def @static_1():
		RunCompilerTestCase("static-1.boo")

	[Test]
	def @static_2():
		RunCompilerTestCase("static-2.boo")

	[Test]
	def @static_CompilerGlobalScope_class_is_sealed_and_abstract():
		RunCompilerTestCase("static-CompilerGlobalScope-class-is-sealed-and-abstract.boo")

	[Test]
	def @static_class_is_sealed_abstract_and_transient():
		RunCompilerTestCase("static-class-is-sealed-abstract-and-transient.boo")

	[Test]
	def @static_final_1():
		RunCompilerTestCase("static-final-1.boo")

	[Test]
	def @static_final_2():
		RunCompilerTestCase("static-final-2.boo")

	[Test]
	def @super_1():
		RunCompilerTestCase("super-1.boo")

	[Test]
	def @super_2():
		RunCompilerTestCase("super-2.boo")

	[Test]
	def @super_3():
		RunCompilerTestCase("super-3.boo")

	[Test]
	def @value_type_propagation_1():
		RunCompilerTestCase("value-type-propagation-1.boo")

	[Test]
	def @value_types_1():
		RunCompilerTestCase("value-types-1.boo")

	[Test]
	def @value_types_10():
		RunCompilerTestCase("value-types-10.boo")

	[Test]
	def @value_types_11():
		RunCompilerTestCase("value-types-11.boo")

	[Test]
	def @value_types_12():
		RunCompilerTestCase("value-types-12.boo")

	[Test]
	def @value_types_13():
		RunCompilerTestCase("value-types-13.boo")

	[Test]
	def @value_types_14():
		RunCompilerTestCase("value-types-14.boo")

	[Category("FailsOnMono")][Test]
	def @value_types_15():
		RunCompilerTestCase("value-types-15.boo")

	[Test]
	def @value_types_16():
		RunCompilerTestCase("value-types-16.boo")

	[Test]
	def @value_types_17():
		RunCompilerTestCase("value-types-17.boo")

	[Test]
	def @value_types_18():
		RunCompilerTestCase("value-types-18.boo")

	[Test]
	def @value_types_19():
		RunCompilerTestCase("value-types-19.boo")

	[Test]
	def @value_types_2():
		RunCompilerTestCase("value-types-2.boo")

	[Test]
	def @value_types_20():
		RunCompilerTestCase("value-types-20.boo")

	[Test]
	def @value_types_21():
		RunCompilerTestCase("value-types-21.boo")

	[Test]
	def @value_types_3():
		RunCompilerTestCase("value-types-3.boo")

	[Test]
	def @value_types_4():
		RunCompilerTestCase("value-types-4.boo")

	[Test]
	def @value_types_5():
		RunCompilerTestCase("value-types-5.boo")

	[Test]
	def @value_types_6():
		RunCompilerTestCase("value-types-6.boo")

	[Test]
	def @value_types_7():
		RunCompilerTestCase("value-types-7.boo")

	[Test]
	def @value_types_8():
		RunCompilerTestCase("value-types-8.boo")

	[Test]
	def @value_types_9():
		RunCompilerTestCase("value-types-9.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/types"
