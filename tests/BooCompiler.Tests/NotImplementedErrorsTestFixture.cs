
namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class NotImplementedErrorsTestFixture : AbstractCompilerErrorsTestFixture
	{


		[Test]
		public void generic_parameter_reference_in_nested_type()
		{
			RunCompilerTestCase(@"generic-parameter-reference-in-nested-type.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "not-implemented";
		}
	}
}
