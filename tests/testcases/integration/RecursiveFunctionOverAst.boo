"""
False
True
False
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Ast

def IsSubclassOf(node as ClassDefinition, typename as string):
	for typeref as SimpleTypeReference in node.BaseTypes:
		if typename == typeref.Name:
			return true
		baseType = node.DeclaringType.Members[typeref.Name]
		if baseType and IsSubclassOf(baseType, typename):
			return true
	return false

code =  """
class Person:
	pass

class Customer(Person):
	pass
	
class Address:
	pass
"""

compiler = BooCompiler()
compiler.Parameters.Pipeline = Boo.Lang.Compiler.Pipelines.Parse()
compiler.Parameters.Input.Add(StringInput("code", code))

types = compiler.Run().CompileUnit.Modules[0].Members

print(IsSubclassOf(types["Person"], "Customer"))
print(IsSubclassOf(types["Customer"], "Person"))
print(IsSubclassOf(types["Address"], "Person"))


