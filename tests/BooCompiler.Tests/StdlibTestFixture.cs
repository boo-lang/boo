
namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class StdlibTestFixture : AbstractCompilerTestCase
	{


		[Test]
		public void DynamicVariable_1()
		{
			RunCompilerTestCase(@"DynamicVariable-1.boo");
		}
		
		[Test]
		public void cat_1()
		{
			RunCompilerTestCase(@"cat-1.boo");
		}
		
		[Test]
		public void cat_2()
		{
			RunCompilerTestCase(@"cat-2.boo");
		}
		
		[Test]
		public void environments_1()
		{
			RunCompilerTestCase(@"environments-1.boo");
		}
		
		[Test]
		public void formatting_1()
		{
			RunCompilerTestCase(@"formatting-1.boo");
		}
		
		[Test]
		public void hash_1()
		{
			RunCompilerTestCase(@"hash-1.boo");
		}
		
		[Test]
		public void hash_2()
		{
			RunCompilerTestCase(@"hash-2.boo");
		}
		
		[Test]
		public void hash_3()
		{
			RunCompilerTestCase(@"hash-3.boo");
		}
		
		[Test]
		public void join_1()
		{
			RunCompilerTestCase(@"join-1.boo");
		}
		
		[Test]
		public void len_1()
		{
			RunCompilerTestCase(@"len-1.boo");
		}
		
		[Test]
		public void list_add_1()
		{
			RunCompilerTestCase(@"list-add-1.boo");
		}
		
		[Test]
		public void list_equals_1()
		{
			RunCompilerTestCase(@"list-equals-1.boo");
		}
		
		[Test]
		public void list_find_1()
		{
			RunCompilerTestCase(@"list-find-1.boo");
		}
		
		[Test]
		public void list_indexof_1()
		{
			RunCompilerTestCase(@"list-indexof-1.boo");
		}
		
		[Test]
		public void list_indexof_2()
		{
			RunCompilerTestCase(@"list-indexof-2.boo");
		}
		
		[Test]
		public void list_indexof_3()
		{
			RunCompilerTestCase(@"list-indexof-3.boo");
		}
		
		[Test]
		public void list_removeall_1()
		{
			RunCompilerTestCase(@"list-removeall-1.boo");
		}
		
		[Test]
		public void list_sort_1()
		{
			RunCompilerTestCase(@"list-sort-1.boo");
		}
		
		[Test]
		public void map_1()
		{
			RunCompilerTestCase(@"map-1.boo");
		}
		
		[Test]
		public void range_1()
		{
			RunCompilerTestCase(@"range-1.boo");
		}
		
		[Test]
		public void range_2()
		{
			RunCompilerTestCase(@"range-2.boo");
		}
		
		[Test]
		public void range_3()
		{
			RunCompilerTestCase(@"range-3.boo");
		}
		
		[Test]
		public void reversed_1()
		{
			RunCompilerTestCase(@"reversed-1.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "stdlib";
		}
	}
}
