"""
2 1
3 1
"""
class Base:
	public Value as int:
		get:
			return 1

class WithNewGetter(Base):
	public Value as int:
		new get:
			return 2

class WithNewProperty(Base):
	new public Value as int:
		get:
			return 3

derived = WithNewGetter()
base as Base = derived
print derived.Value, base.Value

derived2 = WithNewProperty()
base = derived2
print derived2.Value, base.Value
