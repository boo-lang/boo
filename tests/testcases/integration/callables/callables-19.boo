import System
import System.Reflection

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
	assert type is not null, name
	assert type.IsSealed, "${name}.IsSealed"
	assert type.IsSubclassOf(System.MulticastDelegate), "${name}.IsSubclassOf(System.MulticastDelegate)"
	assert typeof(ICallable).IsAssignableFrom(type), "${name} must implement ICallable!"
	
	expectedImplAttributes = MethodImplAttributes.Runtime|MethodImplAttributes.Managed
	
	constructors = type.GetConstructors()
	assert 1 == len(constructors), "${name}.GetConstructors()"
	assert expectedImplAttributes == constructors[0].GetMethodImplementationFlags(), "${name} constructor implementation flags"	
	actualParameters = constructors[0].GetParameters()
	assert 2 == len(actualParameters), "${name} constructor parameters"
	assert object is actualParameters[0].ParameterType
	assert "instance" == actualParameters[0].Name
	assert IntPtr is actualParameters[1].ParameterType
	assert "method" == actualParameters[1].Name
	
	invoke = type.GetMethod("Invoke")
	assert invoke is not null, "${name}.Invoke"
	assert invoke.IsVirtual, "${name}.Invoke must be virtual!"
	assert not invoke.IsStatic
	assert expectedImplAttributes == invoke.GetMethodImplementationFlags()
	assert returnType is invoke.ReturnType

	actualParameters = invoke.GetParameters()
	assert len(parameters) == len(actualParameters)
	for i in range(len(parameters)):
		assert parameters[i].Name == actualParameters[i].Name
		assert parameters[i].Type is actualParameters[i].ParameterType


CheckDelegate("MyFirstCallable",
			(ParamInfo("arg0", object), ParamInfo("arg1", object)),
			void)
			
CheckDelegate("MySecondCallable",
			(ParamInfo("arg", string),),
			bool)
