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
import Boo.Lang.Compiler.TypeSystem
import NUnit.Framework

[TestFixture]
class CallableSignatureFixture:
	
	_services as TypeSystemServices
	_signature1 as CallableSignature
	_signature2 as CallableSignature
	_signature3 as CallableSignature
	_signature4 as CallableSignature
	
	def NoArgsAsVoid():
		pass
		
	def NoArgsAsInt() as int:
		pass
		
	def IntAsVoid(i as int):
		pass
		
	def IntAsInt(i as int) as int:
		pass
		
	def IntIntAsInt(lhs as int, rhs as int) as int:
		pass
	
	[SetUp]
	def SetUp():
		_services = TypeSystemServices()
		_signature1 = CallableSignature(GetMethod("NoArgsAsVoid"))
		_signature2 = CallableSignature(GetMethod("NoArgsAsInt"))
		_signature3 = CallableSignature(GetMethod("IntAsVoid"))
		_signature4 = CallableSignature(GetMethod("IntAsInt"))
		
	def GetMethod(name as string) as IMethod:
		return _services.Map(GetType().GetMethod(name))
	
	[Test]
	def TestGetHashCode():
		Assert.AreEqual(CallableSignature(array(IParameter, 0), _services.VoidType).GetHashCode(),
						_signature1.GetHashCode())
						
		Assert.AreEqual(CallableSignature(array(IParameter, 0), _services.IntType).GetHashCode(),
						_signature2.GetHashCode())
						
		Assert.IsTrue(_signature1.GetHashCode() != _signature2.GetHashCode())
		Assert.IsTrue(_signature2.GetHashCode() != _signature3.GetHashCode())
		Assert.IsTrue(_signature2.GetHashCode() != _signature3.GetHashCode())
		Assert.IsTrue(_signature3.GetHashCode() != _signature4.GetHashCode())
		
	[Test]
	def TestEquals():
		Assert.AreEqual(CallableSignature(array(IParameter, 0), _services.VoidType),
						_signature1)
						
		Assert.AreEqual(CallableSignature(array(IParameter, 0), _services.IntType),
						_signature2)
						
		Assert.AreEqual(CallableSignature(GetMethod("IntAsVoid")),
						_signature3)
						
		Assert.IsTrue(_signature1 != _signature2)
		Assert.IsTrue(_signature2 != _signature3)
		Assert.IsTrue(_signature3 != _signature4)
		
	[Test]
	def TestToString():
		Assert.AreEqual("callable() as void", _signature1.ToString())
		Assert.AreEqual("callable() as int", _signature2.ToString())
		Assert.AreEqual("callable(int) as void", _signature3.ToString())
		Assert.AreEqual("callable(int) as int", _signature4.ToString())
		Assert.AreEqual("callable(int, int) as int",
						CallableSignature(GetMethod("IntIntAsInt")).ToString())
						
	[Test]
	def UseAsHashKeys():
		h = {}
		h[_signature1] = "token1"
		h[_signature2] = "token2"		
		
		Assert.IsNull(h[_signature3])
		Assert.IsNull(h[_signature4])
		Assert.AreEqual("token1", h[_signature1])
		Assert.AreEqual("token2", h[_signature2])
		
		Assert.AreEqual("token1", h[CallableSignature(array(IParameter, 0), _services.VoidType)])
		Assert.AreEqual("token2", h[CallableSignature(array(IParameter, 0), _services.IntType)])
		
		
		 
		
		
