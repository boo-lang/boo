namespace BooCompiler.Tests	
{
	using NUnit.Framework;

	[TestFixture]
	public class ExtensionsIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void extension_properties_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\extensions\extension-properties-1.boo");
		}
		
		[Test]
		public void extensions_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\extensions\extensions-1.boo");
		}
		
		[Test]
		public void extensions_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\extensions\extensions-2.boo");
		}
		
		[Test]
		public void extensions_3()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\extensions\extensions-3.boo");
		}
		
		[Test]
		public void extensions_4()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\extensions\extensions-4.boo");
		}
		
		[Test]
		public void extensions_5()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\extensions\extensions-5.boo");
		}
		
		[Test]
		public void extensions_6()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\extensions\extensions-6.boo");
		}
		
		[Test]
		public void extensions_7()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\extensions\extensions-7.boo");
		}
		
		[Test]
		public void implicit_conversion_extension_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\extensions\implicit-conversion-extension-1.boo");
		}
		
		[Test]
		public void implicit_conversion_extension_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\extensions\implicit-conversion-extension-2.boo");
		}
		
	}
}
