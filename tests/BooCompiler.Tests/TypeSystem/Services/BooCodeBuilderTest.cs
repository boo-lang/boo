using System;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Environments;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Services
{
	[TestFixture]
	public class BooCodeBuilderTest : AbstractTypeSystemTest
	{
		[Test]
		public void CreateMethodFromPrototypeRemapsGenericMethodParametersAndReturnType()
		{
			RunInCompilerContextEnvironment(() =>
			{
				var result = CodeBuilder.CreateMethodFromPrototype(GetMethod("GenericMethodPrototype"), TypeMemberModifiers.Override);
				
				var genericParameters = result.GenericParameters;
				Assert.AreEqual(1, genericParameters.Count);

				var genericParameterType = (IType)genericParameters[0].Entity;
				Assert.AreSame(genericParameterType, result.ReturnType.Entity);

				var parameters = result.Parameters;
				Assert.AreEqual(1, parameters.Count);
				Assert.AreSame(genericParameterType.MakeArrayType(1), parameters[0].Type.Entity);
			});
		}

		[Test]
		public void CreateTypeofExpression()
		{
			RunInCompilerContextEnvironment(() =>
			{
				var type = Map(typeof(string));

				var e = CodeBuilder.CreateTypeofExpression(type);
				Assert.IsNull(e.Entity);
				Assert.AreSame(Map(typeof(Type)), e.ExpressionType);

				Assert.AreSame(type, e.Type.Entity);
			});
		}

		private static IType Map(Type type)
		{
			return TypeSystemServices().Map(type);
		}

		public T GenericMethodPrototype<T>(T[] arrayOfT)
		{
			throw new NotImplementedException();
		}

		private IMethod GetMethod(string methodName)
		{
			return TypeSystemServices().Map(GetType().GetMethod(methodName));
		}

		private static TypeSystemServices TypeSystemServices()
		{
			return My<TypeSystemServices>.Instance;
		}
	}
}
