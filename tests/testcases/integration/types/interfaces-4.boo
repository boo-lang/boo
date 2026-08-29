"""
1
True
"""
interface IFoo:
	pass
	
class Foo(IFoo):
	pass

interfaces = typeof(Foo).GetInterfaces()
print(len(interfaces))
print(IFoo in interfaces)
