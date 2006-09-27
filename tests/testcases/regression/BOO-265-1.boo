"""
Bar = 1
Foo = 42
Bar = 3
"""
class Dynamic(IQuackFu):
	def QuackGet(name as string) as object:
		pass
	def QuackInvoke(name as string, args as (object)) as object:
		pass
	def QuackSet(name as string, value as object) as object:
		print name, "=", value

Dynamic(Bar: 1)
Dynamic()
Dynamic(Foo: 42, Bar: 3)
