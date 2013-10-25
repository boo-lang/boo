namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class DuckTypingIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void duck_1()
		{
			RunCompilerTestCase(@"duck-1.boo");
		}
		
		[Test]
		public void duck_10()
		{
			RunCompilerTestCase(@"duck-10.boo");
		}
		
		[Test]
		public void duck_11()
		{
			RunCompilerTestCase(@"duck-11.boo");
		}
		
		[Test]
		public void duck_12()
		{
			RunCompilerTestCase(@"duck-12.boo");
		}
		
		[Test]
		public void duck_13()
		{
			RunCompilerTestCase(@"duck-13.boo");
		}
		
		[Test]
		public void duck_14()
		{
			RunCompilerTestCase(@"duck-14.boo");
		}
		
		[Test]
		public void duck_15()
		{
			RunCompilerTestCase(@"duck-15.boo");
		}
		
		[Test]
		public void duck_16()
		{
			RunCompilerTestCase(@"duck-16.boo");
		}
		
		[Test]
		public void duck_17()
		{
			RunCompilerTestCase(@"duck-17.boo");
		}
		
		[Test]
		public void duck_18()
		{
			RunCompilerTestCase(@"duck-18.boo");
		}
		
		[Test]
		public void duck_19()
		{
			RunCompilerTestCase(@"duck-19.boo");
		}
		
		[Test]
		public void duck_2()
		{
			RunCompilerTestCase(@"duck-2.boo");
		}
		
		[Test]
		public void duck_20()
		{
			RunCompilerTestCase(@"duck-20.boo");
		}
		
		[Test]
		public void duck_21()
		{
			RunCompilerTestCase(@"duck-21.boo");
		}
		
		[Test]
		public void duck_3()
		{
			RunCompilerTestCase(@"duck-3.boo");
		}
		
		[Test]
		public void duck_4()
		{
			RunCompilerTestCase(@"duck-4.boo");
		}
		
		[Test]
		public void duck_5()
		{
			RunCompilerTestCase(@"duck-5.boo");
		}
		
		[Test]
		public void duck_6()
		{
			RunCompilerTestCase(@"duck-6.boo");
		}
		
		[Test]
		public void duck_7()
		{
			RunCompilerTestCase(@"duck-7.boo");
		}
		
		[Test]
		public void duck_8()
		{
			RunCompilerTestCase(@"duck-8.boo");
		}
		
		[Test]
		public void duck_9()
		{
			RunCompilerTestCase(@"duck-9.boo");
		}
		
		[Category("FailsOnMono")][Test]
		public void exceptions_1()
		{
			RunCompilerTestCase(@"exceptions-1.boo");
		}
		
		[Category("FailsOnMono")][Test]
		public void exceptions_2()
		{
			RunCompilerTestCase(@"exceptions-2.boo");
		}
		
		[Category("FailsOnMono")][Test]
		public void exceptions_3()
		{
			RunCompilerTestCase(@"exceptions-3.boo");
		}
		
		[Category("FailsOnMono")][Test]
		public void exceptions_4()
		{
			RunCompilerTestCase(@"exceptions-4.boo");
		}
		
		[Category("FailsOnMono")][Test]
		public void exceptions_5()
		{
			RunCompilerTestCase(@"exceptions-5.boo");
		}
		
		[Category("FailsOnMono")][Test]
		public void exceptions_6()
		{
			RunCompilerTestCase(@"exceptions-6.boo");
		}
		
		[Category("FailsOnMono")][Test]
		public void exceptions_7()
		{
			RunCompilerTestCase(@"exceptions-7.boo");
		}
		
		[Category("FailsOnMono")][Test]
		public void exceptions_8()
		{
			RunCompilerTestCase(@"exceptions-8.boo");
		}
		
		[Category("FailsOnMono")][Test]
		public void exceptions_9()
		{
			RunCompilerTestCase(@"exceptions-9.boo");
		}
		
		[Test]
		public void indexer_1()
		{
			RunCompilerTestCase(@"indexer-1.boo");
		}
		
		[Test]
		public void promotion_1()
		{
			RunCompilerTestCase(@"promotion-1.boo");
		}
		
		[Test]
		public void promotion_2()
		{
			RunCompilerTestCase(@"promotion-2.boo");
		}
		
		[Test]
		public void promotion_3()
		{
			RunCompilerTestCase(@"promotion-3.boo");
		}
		
		[Test]
		public void promotion_4()
		{
			RunCompilerTestCase(@"promotion-4.boo");
		}
		
		[Test]
		public void promotion_5()
		{
			RunCompilerTestCase(@"promotion-5.boo");
		}
		
		[Test]
		public void runtime_extensions_1()
		{
			RunCompilerTestCase(@"runtime-extensions-1.boo");
		}
		
		[Test]
		public void runtime_extensions_2()
		{
			RunCompilerTestCase(@"runtime-extensions-2.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "integration/duck-typing";
		}
	}
}
