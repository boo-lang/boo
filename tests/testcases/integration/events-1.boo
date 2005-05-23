import System
import System.Reflection
import NUnit.Framework

class Button:
	event Click as EventHandler
	
	transient event Clicked as EventHandler
	
def CheckEventMethod(expectedName, method as MethodInfo):
	Assert.IsNotNull(method, expectedName)
	Assert.AreEqual(expectedName, method.Name)
	Assert.AreEqual(1, len(method.GetParameters()))
	Assert.AreSame(EventHandler, method.GetParameters()[0].ParameterType)
	Assert.AreSame(void, method.ReturnType)
	Assert.IsTrue(method.IsSpecialName, "IsSpecialName")
	Assert.IsTrue(method.IsPublic, "IsPublic")
	
def CheckEventField(field as FieldInfo, serializable as bool, eventType as Type):
	assert field.IsFamily
	assert serializable != field.IsNotSerialized
	assert field.FieldType is eventType
	
def CheckEvent(name, serializable):

	type = Button
	eventInfo = type.GetEvent(name)
	Assert.IsNotNull(eventInfo, name)
	Assert.AreSame(EventHandler, eventInfo.EventHandlerType)

	CheckEventMethod("add_${name}", eventInfo.GetAddMethod())
	CheckEventMethod("remove_${name}", eventInfo.GetRemoveMethod())
	CheckEventField(type.GetField("___${name}", BindingFlags.NonPublic|BindingFlags.Instance), serializable, eventInfo.EventHandlerType)

	raiseMethod = eventInfo.GetRaiseMethod(true)
	assert raiseMethod is not null
	assert raiseMethod.ReturnType is void
	assert raiseMethod.IsFamilyOrAssembly
	assert raiseMethod.IsSpecialName
	assert 2 == len(raiseMethod.GetParameters())
	assert raiseMethod.GetParameters()[0].ParameterType is object
	assert raiseMethod.GetParameters()[1].ParameterType is EventArgs
	
CheckEvent("Click", true)
CheckEvent("Clicked", false)
