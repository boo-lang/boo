"""
2
3
1
"""
import System

class Foo(object):
	pass

class Bar(Foo):
	pass
	
def classDepth(type as Type):

	value = 1
	while type is not object:
		++value
		type = type.BaseType
		
	return value

print(classDepth(Foo))
print(classDepth(Bar))
print(classDepth(object))
