class Functor(ICallable):
	def Call(args as (object)) as object:
		print("called with: " + join(args, ", "))

def each(items, function as callable):
	for item in items:
		function(item)

items = List(range(5))
each(items, Functor())
each(items, { item | print(item) })