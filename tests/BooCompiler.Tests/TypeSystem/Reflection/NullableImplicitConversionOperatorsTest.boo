namespace BooCompiler.Tests.TypeSystem.Reflection

import BooCompiler.Tests.TypeSystem
import NUnit.Framework

[TestFixture]
class NullableImplicitConversionOperatorsTest(AbstractTypeSystemTest):
	[Test]
	def ImplicitConversionFromNullableToValue():
		RunInCompilerContextEnvironment() do:
			nullableDouble = TypeSystemServices.Map(typeof(double?))
			doubleType = TypeSystemServices.Map(typeof(double))
			conversionOperator = TypeSystemServices.FindExplicitConversionOperator(nullableDouble, doubleType)
			Assert.IsNotNull(conversionOperator)
