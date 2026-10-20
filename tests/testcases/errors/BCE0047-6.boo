"""
BCE0047-6.boo(11,9): BCE0047: Non virtual method 'Base.get_Value()' cannot be overridden.
"""
class Base:
	public Value as int:
		get:
			return 1

class Derived(Base):
	public Value as int:
		get:
			return 2
