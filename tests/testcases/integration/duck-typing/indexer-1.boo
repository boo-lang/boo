"""
Bar
42
indexer
named indexer
quack indexer
"""

class Foo (IQuackFu):
	def Bar():
		return "Bar"

	Baz:
		get: return 42

	self[s as string]:
		get: return s

	Named[s as string]:
		get: return string.Concat("named ", s)

	def QuackGet(name as string, args as (object)) as object:
		return string.Concat("quack ", args[0])


f = Foo()
print f.Bar()
print f.Baz
print f["indexer"]
print f.Named["indexer"]
print f.Quack["indexer"]

