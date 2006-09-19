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
		entity = _crs.ResolveCallableReference(
			NewExpressionCollection(NewObjectInvocationExpression()),
			(GetMethod("foo", int), GetMethod("foo", string)))
		Assert.AreEqual(2, _crs.ValidCandidates.Count, _crs.ValidCandidates.ToString())
		assert entity is null
		
	def GetLogicalTypeDepth(type as System.Type):
		return _crs.GetLogicalTypeDepth(TypeSystemServices.Map(type))
		
	def NewExpressionCollection(*items as (Expression)):
		e = ExpressionCollection()
		e.Extend(items)
		return e
		
	def NewObjectInvocationExpression():
		return CodeBuilder.CreateConstructorInvocation(
			TypeSystemServices.Map(typeof(object).GetConstructors()[0]))
		
	def GetMethod(methodName as string, *types as (System.Type)):
		return TypeSystemServices.Map(GetType().GetMethod(methodName, types))
		
	CodeBuilder:
		get:
			return _context.CodeBuilder
		
	TypeSystemServices:
		get:
			return _context.TypeSystemServices
		
	static def foo(i as int):
		pass
		
	static def foo(s as string):
		pass
		

