#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Lang.Compiler.Tests

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Util
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Builders
import NUnit.Framework
import Boo.Lang.Environments

[TestFixture]
class TypeSystemFixture:
	
	_context as CompilerContext
	_tss as TypeSystemServices
	
	def IntAsBool(i as int) as bool:
		pass
		
	def IntAsBool2(i as int) as bool:
		pass
	
	[OneTimeSetUp]
	def SetUpFixture():
		_context = CompilerContext()
		_context.Environment.Run:
			_tss = my(TypeSystemServices)
		
	def GetMethod(name as string) as IMethod:
		return _tss.Map(GetType().GetMethod(name))
		
	def GetCallableType(methodName as string) as ICallableType:
		return _tss.GetCallableType(GetMethod(methodName))
		
	test TypeFieldsMustBeMutable:
	"""
	The fields listed below need to be public and 
	mutable for now because a different backend (say boojay)
	might need different values for some of these.
	"""
		AssertTypeSystemFieldIsMutable "DuckType"
		AssertTypeSystemFieldIsMutable "IQuackFuType"
		AssertTypeSystemFieldIsMutable "MulticastDelegateType"
		AssertTypeSystemFieldIsMutable "DelegateType"
		AssertTypeSystemFieldIsMutable "IntPtrType"
		AssertTypeSystemFieldIsMutable "UIntPtrType"
		AssertTypeSystemFieldIsMutable "ObjectType"
		AssertTypeSystemFieldIsMutable "ValueTypeType"
		AssertTypeSystemFieldIsMutable "EnumType"
		AssertTypeSystemFieldIsMutable "RegexType"
		AssertTypeSystemFieldIsMutable "ArrayType"
		AssertTypeSystemFieldIsMutable "TypeType"
		AssertTypeSystemFieldIsMutable "ObjectArrayType"
		AssertTypeSystemFieldIsMutable "VoidType"
		AssertTypeSystemFieldIsMutable "StringType"
		AssertTypeSystemFieldIsMutable "BoolType"
		AssertTypeSystemFieldIsMutable "CharType"
		AssertTypeSystemFieldIsMutable "SByteType"
		AssertTypeSystemFieldIsMutable "ByteType"
		AssertTypeSystemFieldIsMutable "ShortType"
		AssertTypeSystemFieldIsMutable "UShortType"
		AssertTypeSystemFieldIsMutable "IntType"
		AssertTypeSystemFieldIsMutable "UIntType"
		AssertTypeSystemFieldIsMutable "LongType"
		AssertTypeSystemFieldIsMutable "ULongType"
		AssertTypeSystemFieldIsMutable "SingleType"
		AssertTypeSystemFieldIsMutable "DoubleType"
		AssertTypeSystemFieldIsMutable "DecimalType"
		AssertTypeSystemFieldIsMutable "TimeSpanType"
		AssertTypeSystemFieldIsMutable "DateTimeType"
		AssertTypeSystemFieldIsMutable "RuntimeServicesType"
		AssertTypeSystemFieldIsMutable "BuiltinsType"
		AssertTypeSystemFieldIsMutable "ListType"
		AssertTypeSystemFieldIsMutable "HashType"
		AssertTypeSystemFieldIsMutable "ICallableType"
		AssertTypeSystemFieldIsMutable "IEnumerableType"
		AssertTypeSystemFieldIsMutable "IEnumeratorType"
		AssertTypeSystemFieldIsMutable "IEnumerableGenericType"
		AssertTypeSystemFieldIsMutable "IEnumeratorGenericType"
		AssertTypeSystemFieldIsMutable "ICollectionType"
		AssertTypeSystemFieldIsMutable "SystemAttribute"
		AssertTypeSystemFieldIsMutable "ConditionalAttribute"
			
	def AssertTypeSystemFieldIsMutable(fieldName as string):
		assert not _tss.GetType().GetField(fieldName).IsInitOnly, "Field ${fieldName} must be mutable!"
		
	test TestMostGenericType:
		Assert.AreSame(_tss.LongType,
				_tss.GetMostGenericType(_tss.IntType, _tss.LongType),
				"long > int")
		Assert.AreSame(_tss.IntType,
				_tss.GetMostGenericType(_tss.ShortType, _tss.IntType),
				"int > short")
		Assert.AreSame(_tss.DoubleType,
				_tss.GetMostGenericType(_tss.DoubleType, _tss.IntType),
				"double > int")
		Assert.AreSame(_tss.BoolType,
				_tss.GetMostGenericType(_tss.BoolType, _tss.BoolType),
				"bool == bool")
		Assert.AreSame(_tss.IntType,
				_tss.GetMostGenericType(_tss.BoolType, _tss.IntType),
				"int > bool")
		
	test ExternalMethodType:
		method = GetMethod("IntAsBool")
		
		Assert.IsNotNull(method)
		Assert.AreEqual("IntAsBool", method.Name)
		Assert.AreSame(_tss.BoolType, method.ReturnType, "ReturnType")
		
		parameters = method.GetParameters()
		Assert.AreEqual(1, len(parameters))
		Assert.AreSame(_tss.IntType, parameters[0].Type)
		Assert.AreEqual("i", parameters[0].Name)
		
		type = method.Type
		Assert.IsNotNull(type, "method.Type")
		Assert.AreSame(type, method.CallableType, "method.CallableType")
		
		
	test GetCallableTypeReturnsTheSameObjectForCompatibleMethods:
		type1 = GetCallableType("IntAsBool")
		type2 = GetCallableType("IntAsBool2")
		Assert.AreSame(type1, type2)
		
		Assert.AreSame(GetMethod("IntAsBool").CallableType,
					GetMethod("IntAsBool2").CallableType)
		
	test MapReturnsTheSameObjectForTheSameMember:
		Assert.AreSame(GetMethod("IntAsBool"),
						GetMethod("IntAsBool"))
						
		Assert.AreSame(GetMethod("IntAsBool2"), GetMethod("IntAsBool2"))
		
	test ResolveFromExternalTypesDontAddDuplicatedMethods:
		type1 = _tss.ICallableType
		type2 = _tss.MulticastDelegateType
		
		methods = Set of IEntity()
		type1.Resolve(methods, "GetType", EntityType.Method)
		type2.Resolve(methods, "GetType", EntityType.Method)
		
		Assert.AreEqual(1, len(methods))
		
		found, = methods
		Assert.AreSame(_tss.Map(typeof(object).GetMethod("GetType")), found)
					
	test ExternalInterfaceDepth:
		Assert.AreEqual(1, _tss.ICallableType.GetTypeDepth())
		
	test ExternalTypeDepth:
		Assert.AreEqual(0, _tss.ObjectType.GetTypeDepth())
		Assert.AreEqual(2, _tss.MulticastDelegateType.GetTypeDepth())
		
	test CallableTypeDepth:
		type = _tss.GetCallableType(GetMethod("IntAsBool"))
		Assert.AreEqual(3, type.GetTypeDepth())
		
	test CreateCallableDefinition:
		cd = my(CallableTypeBuilder).ForCallableDefinition(CallableDefinition(Name: "Function"))
		Assert.AreEqual(3, cast(IType, cd.Entity).GetTypeDepth())
		
	def Function() as void:
		pass
		
	def BoolFunction() as bool:
		pass
		
	def SingleArg(item as object) as object:
		pass
		
	test IsCallableTypeAssignableFrom:
		c1 = GetCallableType("IntAsBool")
		c2 = GetCallableType("IntAsBool2")		
		
		AssertCallableAssignableFrom(c1, c2)
		AssertCallableAssignableFrom(c2, c1)
		
		c3 = GetCallableType("Function")
		AssertCallableAssignableFrom(c1, c3)
		AssertCallableNotAssignableFrom(c3, c1)
		AssertCallableAssignableFrom(c2, c3)
		AssertCallableNotAssignableFrom(c3, c2)
		
		c4 = GetCallableType("SingleArg")
		AssertCallableAssignableFrom(c1, c4)
		AssertCallableAssignableFrom(c4, c1)
		AssertCallableAssignableFrom(c2, c4)
		AssertCallableAssignableFrom(c4, c2)
		AssertCallableAssignableFrom(c4, c3)
		AssertCallableNotAssignableFrom(c3, c4)
		
		c5 = GetCallableType("BoolFunction")
		AssertCallableAssignableFrom(c1, c5)
		AssertCallableAssignableFrom(c2, c5)
		AssertCallableAssignableFrom(c3, c5)
		AssertCallableAssignableFrom(c5, c3)
		
	def AssertCallableAssignableFrom(lvalue, rvalue):
		Assert.IsTrue(_tss.IsCallableTypeAssignableFrom(lvalue, rvalue),
					"${lvalue} should be assignable from ${rvalue}")
					
	def AssertCallableNotAssignableFrom(lvalue, rvalue):
		Assert.IsFalse(_tss.IsCallableTypeAssignableFrom(lvalue, rvalue),
					"${lvalue} should NOT be assignable from ${rvalue}")
		
		
		
		
		
	
