using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Reflection
{
	[TestFixture]
	public class NullableImplicitConversionOperatorsTest : AbstractTypeSystemTest
	{
		[Test]
		public void ImplicitConversionFromNullableToValue()
		{
			RunInCompilerContextEnvironment(delegate
        	{
        		var nullableDouble = TypeSystemServices.Map(typeof(double?));
        		var doubleType = TypeSystemServices.Map(typeof(double));
        		var conversionOperator = TypeSystemServices.FindExplicitConversionOperator(nullableDouble, doubleType);
				Assert.IsNotNull(conversionOperator);
        	});
		}
	}
}
