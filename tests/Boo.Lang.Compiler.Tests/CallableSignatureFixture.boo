#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.Tests

import System
import Boo.Lang.Compiler
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
		Assert.AreEqual("callable() as System.Void", _signature1.ToString())
		Assert.AreEqual("callable() as System.Int32", _signature2.ToString())
		Assert.AreEqual("callable(System.Int32) as System.Void", _signature3.ToString())
		Assert.AreEqual("callable(System.Int32) as System.Int32", _signature4.ToString())
		Assert.AreEqual("callable(System.Int32, System.Int32) as System.Int32",
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
		
		
		 
		
		
