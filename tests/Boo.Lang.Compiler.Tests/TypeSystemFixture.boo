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
import Boo.Lang.Compiler.TypeSystem
import NUnit.Framework

[TestFixture]
class TypeSystemFixture:
	
	_services as TypeSystemServices
	
	def IntAsBool(i as int) as bool:
		pass
		
	def IntAsBool2(i as int) as bool:
		pass
	
	[TestFixtureSetUp]
	def SetUpFixture():
		_services = TypeSystemServices()
		
	def GetMethod(name as string) as IMethod:
		return _services.Map(GetType().GetMethod(name))
		
	[Test]
	def ExternalMethodType():
		method = GetMethod("IntAsBool")
		
		Assert.IsNotNull(method)
		Assert.AreEqual("IntAsBool", method.Name)
		Assert.AreSame(_services.BoolType, method.ReturnType, "ReturnType")
		
		parameters = method.GetParameters()
		Assert.AreEqual(1, len(parameters))
		Assert.AreSame(_services.IntType, parameters[0].Type)
		Assert.AreEqual("i", parameters[0].Name)
		
		type = method.Type
		Assert.IsNotNull(type, "method.Type")
		Assert.AreSame(type, method.CallableType, "method.CallableType")
		
		
	[Test]
	def GetCallableTypeReturnsTheSameObjectForCompatibleMethods():
		type1 = _services.GetCallableType(GetMethod("IntAsBool"))
		type2 = _services.GetCallableType(GetMethod("IntAsBool2"))
		Assert.AreSame(type1, type2)
		
		Assert.AreSame(GetMethod("IntAsBool").CallableType,
					GetMethod("IntAsBool2").CallableType)
		
	[Test]
	def MapReturnsTheSameObjectForTheSameMember():
		Assert.AreSame(GetMethod("IntAsBool"),
						GetMethod("IntAsBool"))
						
		Assert.AreSame(GetMethod("IntAsBool2"), GetMethod("IntAsBool2"))
		
	[Test]
	def ResolveFromExternalTypesDontAddDuplicatedMethods():
		type1 = _services.ICallableType
		type2 = _services.MulticastDelegateType
		
		methods = []
		type1.Resolve(methods, "GetType", EntityType.Method)
		type2.Resolve(methods, "GetType", EntityType.Method)
		
		Assert.AreEqual(2, len(methods))
		Assert.AreSame(_services.Map(typeof(object).GetMethod("GetType")),
					methods[0])
		
		
		
		
	
