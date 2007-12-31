#region license
// Copyright (c) 2004, 2005, 2006 Rodrigo B. de Oliveira (rbo@acm.org)
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
import Boo.Lang.Compiler.TypeSystem
import NUnit.Framework

[TestFixture]
class CallableResolutionServiceTestFixture:
	
	_context as CompilerContext
	_crs as CallableResolutionService
	
	[SetUp]
	def SetUp():
		_context = CompilerContext(
			TypeSystemServices: Boo.Lang.Compiler.TypeSystem.TypeSystemServices())
		_crs = CallableResolutionService()
		_crs.Initialize(_context)
		
	[Test]
	def TestGetLogicalTypeDepth():
		Assert.AreEqual(0, GetLogicalTypeDepth(object))
		Assert.AreEqual(0, _crs.GetLogicalTypeDepth(TypeSystemServices.DuckType))
		Assert.AreEqual(1, GetLogicalTypeDepth(System.ValueType))
		Assert.AreEqual(1, GetLogicalTypeDepth(int))
		Assert.AreEqual(1, GetLogicalTypeDepth(string))
		
	[Test]
	def TestResolveAmbiguousCallable():
		entity = ResolveCallableReference(
			NewExpressionCollection(NewObjectInvocationExpression()),
			GetMethod("foo", E), GetMethod("foo", string))
		Assert.AreEqual(2, _crs.ValidCandidates.Count, _crs.ValidCandidates.ToString())
		assert entity is null, entity.ToString()
		
	[Test]
	def CloserMemberWins():
		entity = ResolveCallableReference(
			ExpressionCollection(),
			intMethod = GetMethod(int, "ToString"),
			GetMethod(System.ValueType, "ToString"),
			GetMethod(object, "ToString"))
		Assert.AreSame(intMethod, entity)
		
	[Test]
	def UpcastPlusMatchBetterThanMatchPlusDowncast():
		entity = ResolveCallableReference(
			NewExpressionCollection(
				CodeBuilder.CreateIntegerLiteral(42),
				NewObjectInvocationExpression()),
			objectMethod = GetMethod("bar", object, object),
			GetMethod("bar", int, int))
		Assert.AreSame(objectMethod, entity)
		
	def ResolveCallableReference(args as ExpressionCollection, *candidates as (IMethod)):
		return _crs.ResolveCallableReference(args, candidates)
		
	def GetLogicalTypeDepth(type as System.Type):
		return _crs.GetLogicalTypeDepth(TypeSystemServices.Map(type))
		
	def NewExpressionCollection(*items as (Expression)):
		return ExpressionCollection.FromArray(*items)
		
	def NewObjectInvocationExpression():
		return CodeBuilder.CreateConstructorInvocation(
			TypeSystemServices.Map(typeof(object).GetConstructors()[0]))
		
	def GetMethod(methodName as string, *types as (System.Type)):
		return GetMethod(GetType(), methodName, *types)
		
	def GetMethod(type as System.Type, methodName as string, *types as (System.Type)):
		return TypeSystemServices.Map(type.GetMethod(methodName, types))
		
	CodeBuilder:
		get:
			return _context.CodeBuilder
		
	TypeSystemServices:
		get:
			return _context.TypeSystemServices
			
	enum E:
		A
		B
		C
		
	static def foo(i as E):
		pass
		
	static def foo(s as string):
		pass
		
	static def bar(x as int, y as int):
		pass
		
	static def bar(x as object, y as object):
		pass
		

