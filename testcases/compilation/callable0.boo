"""
FOO
"""
class ToUpper(ICallable):
	def Call(args as (object)) as object:
		return (args[0] as string).ToUpper()

a = ToUpper()
print(a("foo"))
