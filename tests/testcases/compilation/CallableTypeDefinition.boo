import NUnit.Framework
import System.Reflection

callable MyFirstCallable(arg0, arg1)
callable MySecondCallable(arg as string) as bool

currentAssembly = Assembly.GetExecutingAssembly()

type = currentAssembly.GetType("MyFirstCallable")
Assert.IsNotNull(type, "MyFirstCallable")
Assert.IsTrue(type.IsSubclassOf(System.Delegate), "IsSubclassOf(System.Delegate)")

invoke = type.GetMethod("Invoke")
Assert.IsNotNull(invoke, "MyFirstCallable.Invoke")

Assert.AreSame(void, invoke.ReturnType, "MyFirstCallable.Invoke.ReturnType")

parameters = invoke.GetParameters()
Assert.AreEqual(2, len(parameters), "MyFirstCallable.Parameters")
Assert.AreEqual("arg0", parameters[0].Name)
Assert.AreSame(object, parameters[0].ParameterType)

Assert.AreEqual("arg1", parameters[1].Name)
Assert.AreSame(object, parameters[1].ParameterType)

type = currentAssembly.GetType("MySecondCallable")
Assert.IsNotNull(type, "MySecondCallable")
Assert.IsTrue(type.IsSubclassOf(System.Delegate), "IsSubclassOf(System.Delegate)")

invoke = type.GetMethod("Invoke")
Assert.IsNotNull(invoke, "MySecondCallable.Invoke")

Assert.AreSame(bool, invoke.ReturnType, "MySecondCallable.Invoke.ReturnType")

parameter = invoke.GetParameters()
Assert.AreEqual(1, len(parameters), "MySecondCallable.Invoke.Parameters")

Assert.AreEqual("arg", parameters[0].Name)
Assert.AreSame(string, parameters[0].ParameterType)


