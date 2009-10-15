
namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class UnsafeTestFixture : AbstractCompilerTestCase
	{
		protected override void CustomizeCompilerParameters()
		{
			_parameters.Unsafe = true;
		}
		
		protected override bool VerifyGeneratedAssemblies
		{
			get { return false; }
		}

		[Test]
		public void sizeof_1()
		{
			RunCompilerTestCase(@"sizeof-1.boo");
		}
		
		[Test]
		public void unsafe_1()
		{
			RunCompilerTestCase(@"unsafe-1.boo");
		}
		
		[Test]
		public void unsafe_2()
		{
			RunCompilerTestCase(@"unsafe-2.boo");
		}
		
		[Test]
		public void unsafe_3()
		{
			RunCompilerTestCase(@"unsafe-3.boo");
		}
		
		[Test]
		public void unsafe_4()
		{
			RunCompilerTestCase(@"unsafe-4.boo");
		}
		
		[Test]
		public void unsafe_5()
		{
			RunCompilerTestCase(@"unsafe-5.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "unsafe";
		}
	}
}
