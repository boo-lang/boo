
namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class StdlibTestFixture : AbstractCompilerTestCase
	{

		[Test]
		public void cat_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\cat-1.boo");
		}
		
		[Test]
		public void cat_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\cat-2.boo");
		}
		
		[Test]
		public void formatting_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\formatting-1.boo");
		}
		
		[Test]
		public void hash_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\hash-1.boo");
		}
		
		[Test]
		public void hash_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\hash-2.boo");
		}
		
		[Test]
		public void hash_3()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\hash-3.boo");
		}
		
		[Test]
		public void join_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\join-1.boo");
		}
		
		[Test]
		public void list_equals_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\list-equals-1.boo");
		}
		
		[Test]
		public void list_find_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\list-find-1.boo");
		}
		
		[Test]
		public void list_indexof_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\list-indexof-1.boo");
		}
		
		[Test]
		public void list_indexof_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\list-indexof-2.boo");
		}
		
		[Test]
		public void list_indexof_3()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\list-indexof-3.boo");
		}
		
		[Test]
		public void list_removeall_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\list-removeall-1.boo");
		}
		
		[Test]
		public void list_sort_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\list-sort-1.boo");
		}
		
		[Test]
		public void map_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\map-1.boo");
		}
		
		[Test]
		public void range_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\range-1.boo");
		}
		
		[Test]
		public void range_2()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\range-2.boo");
		}
		
		[Test]
		public void range_3()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\range-3.boo");
		}
		
		[Test]
		public void reversed_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\stdlib\reversed-1.boo");
		}
		
	}
}
