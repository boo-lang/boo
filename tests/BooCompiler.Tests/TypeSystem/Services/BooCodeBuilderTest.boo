namespace BooCompiler.Tests.TypeSystem.Services

import System
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem
import BooCompiler.Tests.TypeSystem
import NUnit.Framework

[TestFixture]
class BooCodeBuilderTest(AbstractTypeSystemTest):
	[Test]
	def GenericTypeReference():
		RunInCompilerContextEnvironment() do:
			genericType = TypeSystemServices.Map(typeof(System.Collections.Generic.List[of int]))
			resultingTypeRef = CodeBuilder.CreateTypeReference(genericType)
			Assert.IsInstanceOf(typeof(GenericTypeReference), resultingTypeRef)

			genericTypeRef = resultingTypeRef as GenericTypeReference
			Assert.AreEqual(1, genericTypeRef.GenericArguments.Count)
			Assert.AreSame(TypeSystemServices.IntType, genericTypeRef.GenericArguments[0].Entity)

			Assert.AreSame(genericType, resultingTypeRef.Entity)

	[Test]
	def CreateMethodFromPrototypeRemapsGenericMethodParametersAndReturnType():
		RunInCompilerContextEnvironment() do:
			result = CodeBuilder.CreateMethodFromPrototype(GetMethod("GenericMethodPrototype"), TypeMemberModifiers.Override)

			genericParameters = result.GenericParameters
			Assert.AreEqual(1, genericParameters.Count)

			genericParameterType = genericParameters[0].Entity as IType
			Assert.AreSame(genericParameterType, result.ReturnType.Entity)

			parameters = result.Parameters
			Assert.AreEqual(1, parameters.Count)
			Assert.AreSame(genericParameterType.MakeArrayType(1), parameters[0].Type.Entity)

	[Test]
	def CreateTypeofExpression():
		RunInCompilerContextEnvironment() do:
			type = TypeSystemServices.Map(typeof(string))

			e = CodeBuilder.CreateTypeofExpression(type)
			Assert.IsNull(e.Entity)
			Assert.AreSame(TypeSystemServices.Map(typeof(Type)), e.ExpressionType)

			Assert.AreSame(type, e.Type.Entity)

	public def GenericMethodPrototype[of T](arrayOfT as (T)) as T:
		raise NotImplementedException()

	private def GetMethod(methodName as string) as IMethod:
		return TypeSystemServices.Map(GetType().GetMethod(methodName))
