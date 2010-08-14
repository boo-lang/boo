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
        		var typeSystemServices = My<TypeSystemServices>.Instance;
        		var nullableDouble = typeSystemServices.Map(typeof(double?));
        		var doubleType = typeSystemServices.Map(typeof(double));
        		var conversionOperator = typeSystemServices.FindExplicitConversionOperator(nullableDouble, doubleType);
				Assert.IsNotNull(conversionOperator);
        	});
		}
	}
}
