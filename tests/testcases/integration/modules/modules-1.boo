import System
import System.Reflection
import NUnit.Framework

def AssertModule(type as Type):
	Assert.IsNotNull(type)
	Assert.IsTrue(type.Name.EndsWith("Module"), "Module type name must end with Module!")
	
	attribute = Attribute.GetCustomAttribute(type, System.Runtime.CompilerServices.CompilerGlobalScopeAttribute)
	Assert.IsNotNull(attribute, "Module must be marked with System.Runtime.CompilerServices.CompilerGlobalScopeAttribute!")
	
	Assert.IsTrue(type.IsSealed, "Module must be sealed!")
	Assert.IsFalse(type.IsSerializable, "Module must be transient!")

asm = Assembly.LoadWithPartialName("BooModules")
types = asm.GetExportedTypes()
Assert.AreEqual(3, len(types))

for type in types:
	AssertModule(type)

