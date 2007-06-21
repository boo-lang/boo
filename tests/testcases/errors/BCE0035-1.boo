"""
BCE0035-1.boo(9,9): BCE0035: 'A.Foo' conflicts with inherited member 'Base.Foo'.
"""
class Base:
	abstract def Foo() as int:
		pass
		
class A(Base):
	def Foo() as object:
		pass
