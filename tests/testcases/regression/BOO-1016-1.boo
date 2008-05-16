interface ITest:
	def Do() as string
	def Do[of T]() as T

class Test(ITest):
	def Do() as string:
		return "non-generic"
	def Do[of T]() as T:
		pass

t = Test()
assert "non-generic" == t.Do()
#t.Do[of int]()

