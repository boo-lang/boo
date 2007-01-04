namespace BooCompiler.Tests	
{
	using NUnit.Framework;

	[TestFixture]
	public class AttributesIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void assembly_attributes_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\attributes\assembly-attributes-1.boo");
		}
		
		[Test]
		public void assembly_attributes_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\attributes\assembly-attributes-2.boo");
		}
		
		[Test]
		public void attributes_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\attributes\attributes-1.boo");
		}
		
		[Test]
		public void attributes_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\attributes\attributes-2.boo");
		}
		
		[Test]
		public void attributes_3()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\attributes\attributes-3.boo");
		}
		
		[Test]
		public void attributes_4()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\attributes\attributes-4.boo");
		}
		
		[Test]
		public void attributes_5()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\attributes\attributes-5.boo");
		}
		
		[Test]
		public void attributes_6()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\attributes\attributes-6.boo");
		}
		
		[Test]
		public void attributes_7()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\attributes\attributes-7.boo");
		}
		
		[Test]
		public void attributes_8()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\integration\attributes\attributes-8.boo");
		}
		
	}
}
