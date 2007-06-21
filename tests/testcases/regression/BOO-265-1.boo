"""
Bar = 1
Foo = 42
Bar = 3
"""
class Dynamic(IQuackFu):
	def QuackGet(name as string, parameters as (object)) as object:
		pass
	def QuackInvoke(name as string, args as (object)) as object:
		pass
	def QuackSet(name as string, parameters as (object), value as object) as object:
		assert parameters is null
		print name, "=", value

Dynamic(Bar: 1)
Dynamic()
Dynamic(Foo: 42, Bar: 3)
