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
		
		
		
		
	
