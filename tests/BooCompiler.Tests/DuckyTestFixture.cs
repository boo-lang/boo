
namespace BooCompiler.Tests
{
	using NUnit.Framework;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Pipelines;
	
	[TestFixture]
	public class DuckyTestFixture : AbstractCompilerTestCase
	{
		protected override void CustomizeCompilerParameters()
		{
			_parameters.Ducky = true;
		}

		[Test]
		public void duck_12()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\ducky\duck-12.boo");
		}
		
		[Test]
		public void ducky_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\ducky\ducky-1.boo");
		}
		
		[Test]
		public void ducky_10()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\ducky\ducky-10.boo");
		}
		
		[Test]
		public void ducky_11()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\ducky\ducky-11.boo");
		}
		
		[Test]
		public void ducky_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\ducky\ducky-2.boo");
		}
		
		[Test]
		public void ducky_3()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\ducky\ducky-3.boo");
		}
		
		[Test]
		public void ducky_4()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\ducky\ducky-4.boo");
		}
		
		[Test]
		public void ducky_5()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\ducky\ducky-5.boo");
		}
		
		[Test]
		public void ducky_6()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\ducky\ducky-6.boo");
		}
		
		[Test]
		public void ducky_7()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\ducky\ducky-7.boo");
		}
		
		[Test]
		public void ducky_8()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\ducky\ducky-8.boo");
		}
		
		[Test]
		public void ducky_9()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\ducky\ducky-9.boo");
		}
		
		[Test]
		public void implicit_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\ducky\implicit-1.boo");
		}
		
		[Test]
		public void method_dispatch_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\ducky\method-dispatch-1.boo");
		}
		
		[Test]
		public void method_dispatch_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\ducky\method-dispatch-2.boo");
		}
		
	}
}
