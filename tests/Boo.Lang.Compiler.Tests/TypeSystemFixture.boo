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
		
		
		
		
	
