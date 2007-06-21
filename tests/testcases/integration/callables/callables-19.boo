import System
import System.Reflection
import NUnit.Framework

callable MyFirstCallable(arg0, arg1)
callable MySecondCallable(arg as string) as bool

class ParamInfo:
	public Name as string
	public Type as Type
	
	def constructor(name, type):
		Name = name
		Type = type

def CheckDelegate(name as string, parameters as (ParamInfo), returnType as Type):	
	type = Assembly.GetExecutingAssembly().GetType(name)
	Assert.IsNotNull(type, name)
	Assert.IsTrue(type.IsSealed, "${name}.IsSealed")
	Assert.IsTrue(type.IsSubclassOf(System.MulticastDelegate), "${name}.IsSubclassOf(System.MulticastDelegate)")
	Assert.IsTrue(typeof(ICallable).IsAssignableFrom(type), "${name} must implement ICallable!")
	
	expectedImplAttributes = MethodImplAttributes.Runtime|MethodImplAttributes.Managed
	
	constructors = type.GetConstructors()
	Assert.AreEqual(1, len(constructors), "${name}.GetConstructors()")
	Assert.AreEqual(expectedImplAttributes, constructors[0].GetMethodImplementationFlags(),
					"${name} constructor implementation flags")	
	actualParameters = constructors[0].GetParameters()
	Assert.AreEqual(2, len(actualParameters), "${name} constructor parameters")
	Assert.AreSame(object, actualParameters[0].ParameterType, "constructor parameter")
	Assert.AreEqual("instance", actualParameters[0].Name, "constructor parameter name")
	Assert.AreSame(IntPtr, actualParameters[1].ParameterType, "constructor parameter")
	Assert.AreEqual("method", actualParameters[1].Name, "constructor parameter name")
	
	invoke = type.GetMethod("Invoke")
	Assert.IsNotNull(invoke, "${name}.Invoke")
	Assert.IsTrue(invoke.IsVirtual, "${name}.Invoke must be virtual!")
	Assert.IsFalse(invoke.IsStatic, "${name}.Invoke cannot be static!")
	Assert.AreEqual(expectedImplAttributes,
					invoke.GetMethodImplementationFlags(),
					"${name}.Invoke implementation flags")
	Assert.AreSame(returnType, invoke.ReturnType, "${name}.Invoke.ReturnType")

	actualParameters = invoke.GetParameters()
	Assert.AreEqual(len(parameters), len(actualParameters), "${name}.Parameters")
	for i in range(len(parameters)):
		Assert.AreEqual(parameters[i].Name, actualParameters[i].Name)
		Assert.AreSame(parameters[i].Type, actualParameters[i].ParameterType)


CheckDelegate("MyFirstCallable",
			(ParamInfo("arg0", object), ParamInfo("arg1", object)),
			void)
			
CheckDelegate("MySecondCallable",
			(ParamInfo("arg", string),),
			bool)
