import System
import System.Reflection
import NUnit.Framework

class Button:
	event Click as EventHandler
	
def CheckEventMethod(expectedName, method as MethodInfo):
	Assert.IsNotNull(method, expectedName)
	Assert.AreEqual(expectedName, method.Name)
	Assert.AreEqual(1, len(method.GetParameters()))
	Assert.AreSame(EventHandler, method.GetParameters()[0].ParameterType)
	Assert.AreSame(void, method.ReturnType)
	Assert.IsTrue(method.IsSpecialName, "IsSpecialName")
	Assert.IsTrue(method.IsPublic, "IsPublic")

type = Button
eventInfo = type.GetEvent("Click")
Assert.IsNotNull(eventInfo, "Click")
Assert.AreSame(EventHandler, eventInfo.EventHandlerType)

CheckEventMethod("add_Click", eventInfo.GetAddMethod())
CheckEventMethod("remove_Click", eventInfo.GetRemoveMethod())
