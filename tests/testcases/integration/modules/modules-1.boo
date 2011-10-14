import System
import System.Reflection

def AssertModule(type as Type):
	assert type is not null
	assert type.Name.EndsWith("Module"), "Module type name must end with Module!"
	
	attribute = Attribute.GetCustomAttribute(type, System.Runtime.CompilerServices.CompilerGlobalScopeAttribute)
	assert attribute is not null, "Module must be marked with System.Runtime.CompilerServices.CompilerGlobalScopeAttribute!"
	
	assert type.IsSealed, "Module must be sealed!"
	assert not type.IsSerializable, "Module must be transient!"

asm = Assembly.LoadWithPartialName("BooModules")
types = asm.GetExportedTypes()
assert 3 == len(types)

for type in types:
	AssertModule(type)

