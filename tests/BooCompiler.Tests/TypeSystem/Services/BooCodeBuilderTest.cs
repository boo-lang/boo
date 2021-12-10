using System;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Services
{
	[TestFixture]
	public class BooCodeBuilderTest : AbstractTypeSystemTest
	{	
		[Test]
		public void GenericTypeReference()
		{
			RunInCompilerContextEnvironment(() =>
			{
				var genericType = TypeSystemServices.Map(typeof(System.Collections.Generic.List<int>));
				var resultingTypeRef = CodeBuilder.CreateTypeReference(genericType);
				Assert.IsInstanceOf<GenericTypeReference>(resultingTypeRef);

				var genericTypeRef = (GenericTypeReference)resultingTypeRef;
				Assert.AreEqual(1, genericTypeRef.GenericArguments.Count);
				Assert.AreSame(TypeSystemServices.IntType, genericTypeRef.GenericArguments[0].Entity);

				Assert.AreSame(genericType, resultingTypeRef.Entity);
			});
		}

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
				var type = TypeSystemServices.Map(typeof(string));

				var e = CodeBuilder.CreateTypeofExpression(type);
				Assert.IsNull(e.Entity);
				Assert.AreSame(TypeSystemServices.Map(typeof(Type)), e.ExpressionType);

				Assert.AreSame(type, e.Type.Entity);
			});
		}

		public T GenericMethodPrototype<T>(T[] arrayOfT)
		{
			throw new NotImplementedException();
		}

		private IMethod GetMethod(string methodName)
		{
			return TypeSystemServices.Map(GetType().GetMethod(methodName));
		}
	}
}
