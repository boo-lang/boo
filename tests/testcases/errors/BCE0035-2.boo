"""
BCE0035-2.boo(10,5): BCE0035: 'A.Foo' conflicts with inherited member 'Base.Foo'.
"""
class Base:
	abstract Foo as int:
		get:
			pass
		
class A(Base):
	Foo as object:
		get:
			return null
