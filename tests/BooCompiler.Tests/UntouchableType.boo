namespace BooCompiler.Tests

import System
import System.Collections.Generic
import Boo.Lang.Compiler.TypeSystem

class UntouchableType(IType):
	"""
	An IType that is only ever an identity. Every member throws, so a test
	handing one out finds out if anything looks inside it.
	"""

	private def Untouched():
		raise InvalidOperationException("UntouchableType is an identity, not a type")

	Name as string:
		get: Untouched()

	FullName as string:
		get: Untouched()

	EntityType as EntityType:
		get: Untouched()

	Type as IType:
		get: Untouched()

	def IsDefined(attributeType as IType) as bool:
		Untouched()

	IsDuckTyped as bool:
		get: Untouched()

	DeclaringType as IType:
		get: Untouched()

	IsStatic as bool:
		get: Untouched()

	IsPublic as bool:
		get: Untouched()

	IsClass as bool:
		get: Untouched()

	IsAbstract as bool:
		get: Untouched()

	IsInterface as bool:
		get: Untouched()

	IsEnum as bool:
		get: Untouched()

	IsByRef as bool:
		get: Untouched()

	IsValueType as bool:
		get: Untouched()

	IsFinal as bool:
		get: Untouched()

	IsArray as bool:
		get: Untouched()

	IsPointer as bool:
		get: Untouched()

	DeclaringEntity as IEntity:
		get: Untouched()

	def GetTypeDepth() as int:
		Untouched()

	ElementType as IType:
		get: Untouched()

	BaseType as IType:
		get: Untouched()

	def GetDefaultMember() as IEntity:
		Untouched()

	def GetInterfaces() as (IType):
		Untouched()

	def IsSubclassOf(other as IType) as bool:
		Untouched()

	def IsAssignableFrom(other as IType) as bool:
		Untouched()

	GenericInfo as IGenericTypeInfo:
		get: Untouched()

	ConstructedInfo as IConstructedTypeInfo:
		get: Untouched()

	def MakeArrayType(rank as int) as IArrayType:
		Untouched()

	def MakePointerType() as IType:
		Untouched()

	ParentNamespace as INamespace:
		get: Untouched()

	def Resolve(resultingSet as ICollection[of IEntity], name as string, typesToConsider as EntityType) as bool:
		Untouched()

	def GetMembers() as IEnumerable[of IEntity]:
		Untouched()
