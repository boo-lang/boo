import System
import NUnit.Framework

a = def (a):
	print("closure")
	return a.ToString()

Assert.IsTrue(a isa ICallable)
Assert.IsTrue(a isa Delegate)

invoke = a.GetType().GetMethod("Invoke")
Assert.IsNotNull(invoke)
Assert.AreSame(string, invoke.ReturnType)

parameters = invoke.GetParameters()
Assert.AreEqual(1, len(parameters))
Assert.AreSame(object, parameters[0].ParameterType)

