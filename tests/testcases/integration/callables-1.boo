"""
FOO
"""
class ToUpper(ICallable):
	def Call(args as (object)) as object:
		return cast(string, args[0]).ToUpper()

a = ToUpper()
print(a("foo"))
