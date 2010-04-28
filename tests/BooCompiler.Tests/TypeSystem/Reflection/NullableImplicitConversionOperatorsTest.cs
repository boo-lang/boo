using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Reflection
{
	[TestFixture]
	public class NullableImplicitConversionOperatorsTest : AbstractTypeSystemTest
	{
		[Test]
		public void ImplicitConversionFromNullableToValue()
		{
			context.Run(delegate
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
