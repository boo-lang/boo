using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Environments;
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
        		var nullableDouble = Map(typeof(double?));
        		var doubleType = Map(typeof(double));
        		var conversionOperator = TypeSystemServices.FindExplicitConversionOperator(nullableDouble, doubleType);
				Assert.IsNotNull(conversionOperator);
        	});
		}
	}
}
