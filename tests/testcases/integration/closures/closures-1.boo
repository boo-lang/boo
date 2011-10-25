import System

a = def (a):
	print("closure")
	return a.ToString()

assert a isa ICallable
assert a isa Delegate

invoke = a.GetType().GetMethod("Invoke")
assert invoke is not null
assert string is invoke.ReturnType

parameters = invoke.GetParameters()
assert 1 == len(parameters)
assert object is parameters[0].ParameterType

