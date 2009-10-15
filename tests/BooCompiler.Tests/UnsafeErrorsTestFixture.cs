
namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class UnsafeErrorsTestFixture : AbstractCompilerErrorsTestFixture
	{
		protected override void CustomizeCompilerParameters()
		{
			_parameters.Unsafe = true;
		}

		[Test]
		public void BCE0168_1()
		{
			RunCompilerTestCase(@"BCE0168-1.boo");
		}
		
		[Test]
		public void sizeof_usage_1()
		{
			RunCompilerTestCase(@"sizeof-usage-1.boo");
		}
		
		[Test]
		public void unsafe_usage_1()
		{
			RunCompilerTestCase(@"unsafe-usage-1.boo");
		}
		
		[Test]
		public void unsafe_usage_2()
		{
			RunCompilerTestCase(@"unsafe-usage-2.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "unsafe/errors";
		}
	}
}
