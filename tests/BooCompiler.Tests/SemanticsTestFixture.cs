
namespace BooCompiler.Tests
{
	using NUnit.Framework;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Pipelines;

	[TestFixture]
	public class SemanticsTestFixture : AbstractCompilerTestCase
	{
		protected override CompilerPipeline SetUpCompilerPipeline()
		{
			return new CompileToBoo();
		}


		[Test]
		public void abstract_method0()
		{
			RunCompilerTestCase(@"abstract_method0.boo");
		}
		
		[Test]
		public void abstract_method_stubs0()
		{
			RunCompilerTestCase(@"abstract_method_stubs0.boo");
		}
		
		[Test]
		public void assert0()
		{
			RunCompilerTestCase(@"assert0.boo");
		}
		
		[Test]
		public void assert1()
		{
			RunCompilerTestCase(@"assert1.boo");
		}
		
		[Test]
		public void assign_property()
		{
			RunCompilerTestCase(@"assign_property.boo");
		}
		
		[Test]
		public void callables_1()
		{
			RunCompilerTestCase(@"callables-1.boo");
		}
		
		[Test]
		public void classes0()
		{
			RunCompilerTestCase(@"classes0.boo");
		}
		
		[Test]
		public void classes1()
		{
			RunCompilerTestCase(@"classes1.boo");
		}
		
		[Test]
		public void collection_initializer()
		{
			RunCompilerTestCase(@"collection-initializer.boo");
		}
		
		[Test]
		public void enum0()
		{
			RunCompilerTestCase(@"enum0.boo");
		}
		
		[Test]
		public void enum1()
		{
			RunCompilerTestCase(@"enum1.boo");
		}
		
		[Test]
		public void equality0()
		{
			RunCompilerTestCase(@"equality0.boo");
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
		public void fields_7()
		{
			RunCompilerTestCase(@"fields-7.boo");
		}
		
		[Test]
		public void fields_8()
		{
			RunCompilerTestCase(@"fields-8.boo");
		}
		
		[Test]
		public void folding_0()
		{
			RunCompilerTestCase(@"folding-0.boo");
		}
		
		[Test]
		public void for_1()
		{
			RunCompilerTestCase(@"for-1.boo");
		}
		
		[Test]
		public void for_2()
		{
			RunCompilerTestCase(@"for-2.boo");
		}
		
		[Test]
		public void hash_initializer()
		{
			RunCompilerTestCase(@"hash-initializer.boo");
		}
		
		[Test]
		public void hash0()
		{
			RunCompilerTestCase(@"hash0.boo");
		}
		
		[Test]
		public void in_string()
		{
			RunCompilerTestCase(@"in_string.boo");
		}
		
		[Test]
		public void interfaces_0()
		{
			RunCompilerTestCase(@"interfaces-0.boo");
		}
		
		[Test]
		public void interfaces_1()
		{
			RunCompilerTestCase(@"interfaces-1.boo");
		}
		
		[Test]
		public void interfaces_2()
		{
			RunCompilerTestCase(@"interfaces-2.boo");
		}
		
		[Test]
		public void is_0()
		{
			RunCompilerTestCase(@"is-0.boo");
		}
		
		[Test]
		public void len()
		{
			RunCompilerTestCase(@"len.boo");
		}
		
		[Test]
		public void lock0()
		{
			RunCompilerTestCase(@"lock0.boo");
		}
		
		[Test]
		public void lock1()
		{
			RunCompilerTestCase(@"lock1.boo");
		}
		
		[Test]
		public void lock2()
		{
			RunCompilerTestCase(@"lock2.boo");
		}
		
		[Test]
		public void method2()
		{
			RunCompilerTestCase(@"method2.boo");
		}
		
		[Test]
		public void method3()
		{
			RunCompilerTestCase(@"method3.boo");
		}
		
		[Test]
		public void method6()
		{
			RunCompilerTestCase(@"method6.boo");
		}
		
		[Test]
		public void method7()
		{
			RunCompilerTestCase(@"method7.boo");
		}
		
		[Test]
		public void module0()
		{
			RunCompilerTestCase(@"module0.boo");
		}
		
		[Test]
		public void null0()
		{
			RunCompilerTestCase(@"null0.boo");
		}
		
		[Test]
		public void null1()
		{
			RunCompilerTestCase(@"null1.boo");
		}
		
		[Test]
		public void numericpromo0()
		{
			RunCompilerTestCase(@"numericpromo0.boo");
		}
		
		[Test]
		public void omitted_target_1()
		{
			RunCompilerTestCase(@"omitted-target-1.boo");
		}
		
		[Test]
		public void regex_is_cached_in_static_field_unless_assigned()
		{
			RunCompilerTestCase(@"regex-is-cached-in-static-field-unless-assigned.boo");
		}
		
		[Test]
		public void slice_property()
		{
			RunCompilerTestCase(@"slice_property.boo");
		}
		
		[Test]
		public void slice_property_int()
		{
			RunCompilerTestCase(@"slice_property_int.boo");
		}
		
		[Test]
		public void static_field_initializer()
		{
			RunCompilerTestCase(@"static_field_initializer.boo");
		}
		
		[Test]
		public void stringslice0()
		{
			RunCompilerTestCase(@"stringslice0.boo");
		}
		
		[Test]
		public void stringslice1()
		{
			RunCompilerTestCase(@"stringslice1.boo");
		}
		
		[Test]
		public void struct_1()
		{
			RunCompilerTestCase(@"struct-1.boo");
		}
		
		[Test]
		public void type_resolution0()
		{
			RunCompilerTestCase(@"type_resolution0.boo");
		}
		
		[Test]
		public void using0()
		{
			RunCompilerTestCase(@"using0.boo");
		}
		
		[Test]
		public void using1()
		{
			RunCompilerTestCase(@"using1.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "semantics";
		}
	}
}
