
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
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\abstract_method0.boo");
		}
		
		[Test]
		public void assert0()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\assert0.boo");
		}
		
		[Test]
		public void assert1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\assert1.boo");
		}
		
		[Test]
		public void assign_property()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\assign_property.boo");
		}
		
		[Test]
		public void classes0()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\classes0.boo");
		}
		
		[Test]
		public void classes1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\classes1.boo");
		}
		
		[Test]
		public void enum0()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\enum0.boo");
		}
		
		[Test]
		public void equality0()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\equality0.boo");
		}
		
		[Test]
		public void fields_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\fields-1.boo");
		}
		
		[Test]
		public void fields_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\fields-2.boo");
		}
		
		[Test]
		public void fields_3()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\fields-3.boo");
		}
		
		[Test]
		public void fields_4()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\fields-4.boo");
		}
		
		[Test]
		public void fields_5()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\fields-5.boo");
		}
		
		[Test]
		public void fields_6()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\fields-6.boo");
		}
		
		[Test]
		public void fields_7()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\fields-7.boo");
		}
		
		[Test]
		public void hash0()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\hash0.boo");
		}
		
		[Test]
		public void interfaces_0()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\interfaces-0.boo");
		}
		
		[Test]
		public void interfaces_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\interfaces-1.boo");
		}
		
		[Test]
		public void interfaces_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\interfaces-2.boo");
		}
		
		[Test]
		public void in_string()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\in_string.boo");
		}
		
		[Test]
		public void is_0()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\is-0.boo");
		}
		
		[Test]
		public void len()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\len.boo");
		}
		
		[Test]
		public void lock0()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\lock0.boo");
		}
		
		[Test]
		public void lock1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\lock1.boo");
		}
		
		[Test]
		public void lock2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\lock2.boo");
		}
		
		[Test]
		public void method2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\method2.boo");
		}
		
		[Test]
		public void method3()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\method3.boo");
		}
		
		[Test]
		public void method6()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\method6.boo");
		}
		
		[Test]
		public void method7()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\method7.boo");
		}
		
		[Test]
		public void module0()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\module0.boo");
		}
		
		[Test]
		public void null0()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\null0.boo");
		}
		
		[Test]
		public void null1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\null1.boo");
		}
		
		[Test]
		public void numericpromo0()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\numericpromo0.boo");
		}
		
		[Test]
		public void slice_property()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\slice_property.boo");
		}
		
		[Test]
		public void slice_property_int()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\slice_property_int.boo");
		}
		
		[Test]
		public void static_field_initializer()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\static_field_initializer.boo");
		}
		
		[Test]
		public void stringslice0()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\stringslice0.boo");
		}
		
		[Test]
		public void stringslice1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\stringslice1.boo");
		}
		
		[Test]
		public void struct_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\struct-1.boo");
		}
		
		[Test]
		public void type_resolution0()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\type_resolution0.boo");
		}
		
		[Test]
		public void using0()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\using0.boo");
		}
		
		[Test]
		public void using1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\semantics\using1.boo");
		}
		
	}
}
