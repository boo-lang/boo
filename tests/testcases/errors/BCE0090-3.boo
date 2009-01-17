"""
BCE0090-3.boo(10,17): BCE0090: Derived method 'Derived.Foo' can not reduce the accessibility of 'Base.Foo' from 'internal' to 'private'.
"""

class Base:
	internal def Foo():
		pass

class Derived(Base):
	private def Foo():
		pass

