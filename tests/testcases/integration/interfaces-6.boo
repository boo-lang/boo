"""
2
True
True
True
"""
interface IFoo:
	pass
	
interface IBar(IFoo):
	pass
	
class Foo(IBar):
	pass

interfaces = typeof(Foo).GetInterfaces()
print(len(interfaces))
print(IFoo in interfaces)
print(IBar in interfaces)

print(IFoo in typeof(IBar).GetInterfaces())
