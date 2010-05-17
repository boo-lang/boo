"""
54321
hello world
"""
class MyClass:
	[property(Value)]
	value = null
	
	def constructor(inValue):
		value = inValue

def ChangeValue(inst as MyClass):
    inst.Value = "hello world"

inst = MyClass(54321)
print(inst.Value)
ChangeValue(inst)
print(inst.Value)
