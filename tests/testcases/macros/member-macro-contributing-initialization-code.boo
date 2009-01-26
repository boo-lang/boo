"""
before ClassWithNoConstructors()
Base.constructor
contributed by macro: ClassWithNoConstructors
before ClassWithExistingConstructors('')
Base.constructor
_field1
contributed by macro: ClassWithExistingConstructors
_field2
string
before ClassWithExistingConstructors()
Base.constructor
_field1
contributed by macro: ClassWithExistingConstructors
_field2
parameterless
"""
import Boo.Lang.Compiler.Ast

macro trace_construction:

	# statements are contributed as initializers
	typeDefinition = trace_construction.GetAncestor[of TypeDefinition]()
	yield [| print "contributed by macro:", $(typeDefinition.Name) |]
	
class Base:
	def constructor():
		print "Base.constructor"
		
class ClassWithNoConstructors(Base):
	
	trace_construction
	
class ClassWithExistingConstructors(Base):

	_field1 = DebugInitializerOrder("_field1")
	trace_construction
	_field2 = DebugInitializerOrder("_field2")
	
	def constructor(value as string):
		print "string"
		
	def constructor():
		super()
		print "parameterless"
		
	static def DebugInitializerOrder(value):
		print value
		return value
		
print "before ClassWithNoConstructors()"
ClassWithNoConstructors()
print "before ClassWithExistingConstructors('')"
ClassWithExistingConstructors('')
print "before ClassWithExistingConstructors()"
ClassWithExistingConstructors()
