"""
Foo[]
Base[]
Base[]
System.Object[]
"""
class Base:
	pass
	
class Foo(Base):
	pass
	
class Bar(Base):
	pass
	
print((Foo(), Foo()).GetType())
print((Foo(), Bar()).GetType())
print((Foo(), Base()).GetType())
print((Foo(), object()).GetType())
