
namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class GenericsTestFixture : AbstractCompilerTestCase
	{
		override protected void RunCompilerTestCase(string name)
		{
			if (System.Environment.Version.Major < 2) Assert.Ignore("Test requires .net 2.");
			System.ResolveEventHandler resolver = InstallAssemblyResolver(BaseTestCasesPath);
			try
			{
				base.RunCompilerTestCase(name);
			}
			finally
			{
				RemoveAssemblyResolver(resolver);
			}
		}

		override protected void CopyDependencies()
		{
			CopyAssembliesFromTestCasePath();
		}


		[Test]
		public void ambiguous_1()
		{
			RunCompilerTestCase(@"ambiguous-1.boo");
		}
		
		[Test]
		public void array_enumerable_1()
		{
			RunCompilerTestCase(@"array-enumerable-1.boo");
		}
		
		[Test]
		public void array_enumerable_2()
		{
			RunCompilerTestCase(@"array-enumerable-2.boo");
		}
		
		[Test]
		public void automatic_generic_method_stub()
		{
			RunCompilerTestCase(@"automatic-generic-method-stub.boo");
		}
		
		[Test]
		public void callable_1()
		{
			RunCompilerTestCase(@"callable-1.boo");
		}
		
		[Test]
		public void callable_2()
		{
			RunCompilerTestCase(@"callable-2.boo");
		}
		
		[Test]
		public void collections_1()
		{
			RunCompilerTestCase(@"collections-1.boo");
		}
		
		[Test]
		public void collections_2()
		{
			RunCompilerTestCase(@"collections-2.boo");
		}
		
		[Test]
		public void enumerable_shorthand_1()
		{
			RunCompilerTestCase(@"enumerable-shorthand-1.boo");
		}
		
		[Test]
		public void enumerable_type_inference_1()
		{
			RunCompilerTestCase(@"enumerable-type-inference-1.boo");
		}
		
		[Test]
		public void enumerable_type_inference_2()
		{
			RunCompilerTestCase(@"enumerable-type-inference-2.boo");
		}
		
		[Test]
		public void enumerable_type_inference_4()
		{
			RunCompilerTestCase(@"enumerable-type-inference-4.boo");
		}
		
		[Test]
		public void enumerable_type_inference_5()
		{
			RunCompilerTestCase(@"enumerable-type-inference-5.boo");
		}
		
		[Test]
		public void generator_with_type_constraint_1()
		{
			RunCompilerTestCase(@"generator-with-type-constraint-1.boo");
		}

        [Test]
        public void generator_with_type_constraint_manually_expanded()
        {
            RunCompilerTestCase(@"generator-with-type-constraint-manually-expanded.boo");
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
		public void generators_4()
		{
			RunCompilerTestCase(@"generators-4.boo");
		}
		
		[Test]
		public void generators_5()
		{
			RunCompilerTestCase(@"generators-5.boo");
		}
		
		[Test]
		public void generators_6()
		{
			RunCompilerTestCase(@"generators-6.boo");
		}
		
		[Test]
		public void generators_7()
		{
			RunCompilerTestCase(@"generators-7.boo");
		}
		
		[Test]
		public void generic_array_1()
		{
			RunCompilerTestCase(@"generic-array-1.boo");
		}
		
		[Test]
		public void generic_array_2()
		{
			RunCompilerTestCase(@"generic-array-2.boo");
		}
		
		[Test]
		public void generic_array_3()
		{
			RunCompilerTestCase(@"generic-array-3.boo");
		}

	    [Test]
	    public void generic_closures()
	    {
	        RunCompilerTestCase(@"generic-closures.boo");
	    }

        [Test]
        public void generic_closures_2()
        {
            RunCompilerTestCase(@"generic-closures-2.boo");
        }

        [Test]
        public void generic_closures_3()
        {
            RunCompilerTestCase(@"generic-closures-3.boo");
        }

        [Test]
		public void generic_extension_1()
		{
			RunCompilerTestCase(@"generic-extension-1.boo");
		}
		
		[Test]
		public void generic_extension_2()
		{
			RunCompilerTestCase(@"generic-extension-2.boo");
		}
		
		[Test]
		public void generic_field_1()
		{
			RunCompilerTestCase(@"generic-field-1.boo");
		}
		
		[Test]
		public void generic_generator_type_1()
		{
			RunCompilerTestCase(@"generic-generator-type-1.boo");
		}
		
		[Test]
		public void generic_inheritance_1()
		{
			RunCompilerTestCase(@"generic-inheritance-1.boo");
		}
		
		[Test]
		public void generic_instance_overload()
		{
			RunCompilerTestCase(@"generic-instance-overload.boo");
		}
		
		[Test]
		public void generic_list_of_callable()
		{
			RunCompilerTestCase(@"generic-list-of-callable.boo");
		}
		
		[Test]
		public void generic_matrix_1()
		{
			RunCompilerTestCase(@"generic-matrix-1.boo");
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
		public void generic_method_4()
		{
			RunCompilerTestCase(@"generic-method-4.boo");
		}
		
		[Test]
		public void generic_method_5()
		{
			RunCompilerTestCase(@"generic-method-5.boo");
		}
		
		[Test]
		public void generic_method_invocation_1()
		{
			RunCompilerTestCase(@"generic-method-invocation-1.boo");
		}
		
		[Test]
		public void generic_method_invocation_2()
		{
			RunCompilerTestCase(@"generic-method-invocation-2.boo");
		}
		
		[Test]
		public void generic_overload_1()
		{
			RunCompilerTestCase(@"generic-overload-1.boo");
		}
		
		[Test]
		public void generic_overload_2()
		{
			RunCompilerTestCase(@"generic-overload-2.boo");
		}
		
		[Test]
		public void generic_overload_3()
		{
			RunCompilerTestCase(@"generic-overload-3.boo");
		}
		
		[Test]
		public void generic_overload_4()
		{
			RunCompilerTestCase(@"generic-overload-4.boo");
		}
		
		[Test]
		public void generic_overload_5()
		{
			RunCompilerTestCase(@"generic-overload-5.boo");
		}
		
		[Ignore("FIXME: Covariance support is incomplete, generates unverifiable IL (BOO-1155)")][Test]
		public void generic_overload_6()
		{
			RunCompilerTestCase(@"generic-overload-6.boo");
		}

        [Test]
        public void generic_overload_7()
        {
            RunCompilerTestCase(@"generic-overload-7.boo");
        }
	        

		[Test]
		public void generic_ref_parameter()
		{
			RunCompilerTestCase(@"generic-ref-parameter.boo");
		}
		
		[Test]
		public void generic_type_resolution_1()
		{
			RunCompilerTestCase(@"generic-type-resolution-1.boo");
		}
		
		[Test]
		public void generic_type_resolution_2()
		{
			RunCompilerTestCase(@"generic-type-resolution-2.boo");
		}
		
		[Test]
		public void generic_type_resolution_3()
		{
			RunCompilerTestCase(@"generic-type-resolution-3.boo");
		}
		
		[Test]
		public void inference_1()
		{
			RunCompilerTestCase(@"inference-1.boo");
		}
		
		[Test]
		public void inference_10()
		{
			RunCompilerTestCase(@"inference-10.boo");
		}
		
		[Test]
		public void inference_2()
		{
			RunCompilerTestCase(@"inference-2.boo");
		}
		
		[Test]
		public void inference_3()
		{
			RunCompilerTestCase(@"inference-3.boo");
		}
		
		[Test]
		public void inference_4()
		{
			RunCompilerTestCase(@"inference-4.boo");
		}
		
		[Test]
		public void inference_5()
		{
			RunCompilerTestCase(@"inference-5.boo");
		}
		
		[Test]
		public void inference_6()
		{
			RunCompilerTestCase(@"inference-6.boo");
		}
		
		[Test]
		public void inference_7()
		{
			RunCompilerTestCase(@"inference-7.boo");
		}
		
		[Ignore("Anonymous callable types involving generic type arguments are not handled correctly yet (BOO-854)")][Test]
		public void inference_8()
		{
			RunCompilerTestCase(@"inference-8.boo");
		}
		
		[Test]
		public void inference_9()
		{
			RunCompilerTestCase(@"inference-9.boo");
		}
		
		[Test]
		public void inheritance_1()
		{
			RunCompilerTestCase(@"inheritance-1.boo");
		}
		
		[Test]
		public void inheritance_2()
		{
			RunCompilerTestCase(@"inheritance-2.boo");
		}
		
		[Test]
		public void inheritance_3()
		{
			RunCompilerTestCase(@"inheritance-3.boo");
		}
		
		[Test]
		public void interface_1()
		{
			RunCompilerTestCase(@"interface-1.boo");
		}
		
		[Test]
		public void interface_with_generic_method()
		{
			RunCompilerTestCase(@"interface-with-generic-method.boo");
		}
		
		[Test]
		public void internal_generic_callable_type_1()
		{
			RunCompilerTestCase(@"internal-generic-callable-type-1.boo");
		}
		
		[Test]
		public void internal_generic_callable_type_2()
		{
			RunCompilerTestCase(@"internal-generic-callable-type-2.boo");
		}
		
		[Test]
		public void internal_generic_callable_type_3()
		{
			RunCompilerTestCase(@"internal-generic-callable-type-3.boo");
		}
		
		[Test]
		public void internal_generic_type_1()
		{
			RunCompilerTestCase(@"internal-generic-type-1.boo");
		}
		
		[Test]
		public void internal_generic_type_10()
		{
			RunCompilerTestCase(@"internal-generic-type-10.boo");
		}
		
		[Test]
		public void internal_generic_type_11()
		{
			RunCompilerTestCase(@"internal-generic-type-11.boo");
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
		public void internal_generic_type_7()
		{
			RunCompilerTestCase(@"internal-generic-type-7.boo");
		}
		
		[Test]
		public void internal_generic_type_8()
		{
			RunCompilerTestCase(@"internal-generic-type-8.boo");
		}
		
		[Test]
		public void internal_generic_type_9()
		{
			RunCompilerTestCase(@"internal-generic-type-9.boo");
		}
		
		[Test]
		public void method_ref_1()
		{
			RunCompilerTestCase(@"method-ref-1.boo");
		}
		
		[Test]
		public void mixed_1()
		{
			RunCompilerTestCase(@"mixed-1.boo");
		}
		
		[Test]
		public void mixed_2()
		{
			RunCompilerTestCase(@"mixed-2.boo");
		}
		
		[Test]
		public void mixed_3()
		{
			RunCompilerTestCase(@"mixed-3.boo");
		}
		
		[Test]
		public void mixed_4()
		{
			RunCompilerTestCase(@"mixed-4.boo");
		}
		
		[Test]
		public void mixed_ref_parameter_1()
		{
			RunCompilerTestCase(@"mixed-ref-parameter-1.boo");
		}
		
		[Test]
		public void mixed_ref_parameter_2()
		{
			RunCompilerTestCase(@"mixed-ref-parameter-2.boo");
		}
		
		[Test]
		public void naked_type_constraints_1()
		{
			RunCompilerTestCase(@"naked-type-constraints-1.boo");
		}
		
		[Ignore("generics with nested types not supported yet")][Test]
		public void nested_generic_type_1()
		{
			RunCompilerTestCase(@"nested-generic-type-1.boo");
		}
		
		[Test]
		public void nullable_1()
		{
			RunCompilerTestCase(@"nullable-1.boo");
		}
		
		[Test]
		public void nullable_2()
		{
			RunCompilerTestCase(@"nullable-2.boo");
		}
		
		[Test]
		public void nullable_3()
		{
			RunCompilerTestCase(@"nullable-3.boo");
		}
		
		[Test]
		public void nullable_4()
		{
			RunCompilerTestCase(@"nullable-4.boo");
		}
		
		[Test]
		public void nullable_5()
		{
			RunCompilerTestCase(@"nullable-5.boo");
		}
		
		[Test]
		public void nullable_6()
		{
			RunCompilerTestCase(@"nullable-6.boo");
		}

		[Test]
		public void nullable_7()
		{
			RunCompilerTestCase(@"nullable-7.boo");
		}

		[Test]
		public void override_1()
		{
			RunCompilerTestCase(@"override-1.boo");
		}
		
		[Test]
		public void override_2()
		{
			RunCompilerTestCase(@"override-2.boo");
		}
		
		[Test]
		public void override_3()
		{
			RunCompilerTestCase(@"override-3.boo");
		}
		
		[Test]
		public void override_4()
		{
			RunCompilerTestCase(@"override-4.boo");
		}
		
		[Test]
		public void override_5()
		{
			RunCompilerTestCase(@"override-5.boo");
		}
		
		[Test]
		public void type_reference_1()
		{
			RunCompilerTestCase(@"type-reference-1.boo");
		}
		
		[Test]
		public void type_reference_2()
		{
			RunCompilerTestCase(@"type-reference-2.boo");
		}
		
		[Test]
		public void type_reference_3()
		{
			RunCompilerTestCase(@"type-reference-3.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "net2/generics";
		}
	}
}
