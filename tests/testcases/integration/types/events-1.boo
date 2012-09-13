import System
import System.Reflection

class Button:
	event Click as EventHandler
	
	transient event Clicked as EventHandler
	
def CheckEventMethod(expectedName, method as MethodInfo):
	assert method is not null, expectedName
	assert expectedName == method.Name
	assert 1 == len(method.GetParameters())
	assert EventHandler is method.GetParameters()[0].ParameterType
	assert void is method.ReturnType
	assert method.IsSpecialName
	assert method.IsPublic
	
def CheckEventField(field as FieldInfo, serializable as bool, eventType as Type):
	assert field.IsPrivate
	assert serializable != field.IsNotSerialized
	assert field.FieldType is eventType
	
def CheckEvent(name, serializable):

	type = Button
	eventInfo = type.GetEvent(name)
	assert eventInfo is not null, name 
	assert EventHandler is eventInfo.EventHandlerType

	CheckEventMethod("add_$name", eventInfo.GetAddMethod())
	CheckEventMethod("remove_$name", eventInfo.GetRemoveMethod())
	CheckEventField(type.GetField("\$event\$$name", BindingFlags.NonPublic|BindingFlags.Instance), serializable, eventInfo.EventHandlerType)

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
