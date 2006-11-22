namespace BooCompiler.Tests	
{
	using NUnit.Framework;

	[TestFixture]
	public class PrimitivesIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void bool_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\bool-1.boo");
		}
		
		[Test]
		public void char_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\char-1.boo");
		}
		
		[Test]
		public void char_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\char-2.boo");
		}
		
		[Test]
		public void char_3()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\char-3.boo");
		}
		
		[Test]
		public void char_4()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\char-4.boo");
		}
		
		[Test]
		public void checked_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\checked-1.boo");
		}
		
		[Test]
		public void decimal_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\decimal-1.boo");
		}
		
		[Test]
		public void double_as_bool_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\double-as-bool-1.boo");
		}
		
		[Test]
		public void hash_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\hash-1.boo");
		}
		
		[Test]
		public void hex_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\hex-1.boo");
		}
		
		[Test]
		public void hex_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\hex-2.boo");
		}
		
		[Test]
		public void interpolation_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\interpolation-1.boo");
		}
		
		[Test]
		public void len_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\len-1.boo");
		}
		
		[Test]
		public void list_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\list-1.boo");
		}
		
		[Test]
		public void list_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\list-2.boo");
		}
		
		[Test]
		public void list_3()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\list-3.boo");
		}
		
		[Test]
		public void primitives_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\primitives-1.boo");
		}
		
		[Test]
		public void promotion_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\promotion-1.boo");
		}
		
		[Test]
		public void promotion_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\promotion-2.boo");
		}
		
		[Test]
		public void single_as_bool_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\single-as-bool-1.boo");
		}
		
		[Test]
		public void string_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\string-1.boo");
		}
		
		[Test]
		public void typeof_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\typeof-1.boo");
		}
		
		[Test]
		public void typeof_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\typeof-2.boo");
		}
		
		[Test]
		public void unsigned_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\unsigned-1.boo");
		}
		
		[Test]
		public void unsigned_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\unsigned-2.boo");
		}
		
		[Test]
		public void __eval___1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\__eval__-1.boo");
		}
		
		[Test]
		public void __switch___1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\primitives\__switch__-1.boo");
		}
		
	}
}
